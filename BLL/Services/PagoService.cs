using BLL.DTOs;
using BLL.Facades;
using BLL.Mappers;
using DAL.Contracts;
using DAL.Factory;
using Domain.Entities;
using Domain.Enums;
using Service.Helpers;
using Service.Logic;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace BLL.Services
{
    public class PagoService
    {
        private readonly IPagoRepository _pagoRepo;
        private readonly IMovimientoRepository _movimientoRepo;
        private readonly ComprobanteFacade _comprobanteFacade;
        private readonly IClienteRepository _clienteRepo;
        private readonly IReservaRepository _reservaRepo;
        private readonly IEspacioRepository _espacioRepo;
        private readonly BitacoraService _bitacora;

        public PagoService()
        {
            _pagoRepo = DalFactory.PagoRepository;
            _movimientoRepo = DalFactory.MovimientoRepository;
            _comprobanteFacade = new ComprobanteFacade();
            _clienteRepo = DalFactory.ClienteRepository;
            _reservaRepo = DalFactory.ReservaRepository;
            _espacioRepo = DalFactory.EspacioRepository;
            _bitacora = new BitacoraService();
        }

        public void RegistrarPago(PagoDTO dto)
        {
            if (dto.Monto <= 0) throw new ArgumentException("El monto debe ser mayor a cero");
            if (string.IsNullOrWhiteSpace(dto.Metodo)) throw new ArgumentException("El método de pago es requerido");

            try
            {
                var cliente = _clienteRepo.GetById(dto.ClienteID);
                if (cliente == null) throw new InvalidOperationException($"El cliente con ID {dto.ClienteID} no existe");

                using (var conn = new SqlConnection(ConnectionManager.GetBusinessConnectionString()))
                {
                    conn.Open();
                    using (var tran = conn.BeginTransaction())
                    {
                        Guid pagoId = Guid.Empty;
                        try
                        {
                            Reserva reserva = null;
                            if (dto.ReservaID.HasValue)
                            {
                                reserva = _reservaRepo.GetById(dto.ReservaID.Value, conn, tran);
                                if (reserva == null) throw new InvalidOperationException("ERR_RESERVA_NO_EXISTE");

                                var espacio = _espacioRepo.GetById(reserva.EspacioID);
                                decimal montoTotal = espacio.PrecioHora * (reserva.Duracion / 60.0m);

                                var pagosAnteriores = _pagoRepo.GetByReserva(reserva.Id, conn, tran);
                                decimal pagadoHastaAhora = 0;
                                foreach (var p in pagosAnteriores)
                                {
                                    if (p.Estado == EstadoPago.Abonado.ToString())
                                    {
                                        pagadoHastaAhora += p.Monto;
                                    }
                                }

                                decimal saldoRestante = montoTotal - pagadoHastaAhora;

                                if (dto.Monto > saldoRestante)
                                {
                                    throw new InvalidOperationException("ERR_MONTO_SUPERA_SALDO");
                                }
                            }

                            // 1. INSERT Pago
                            dto.Estado = EstadoPago.Abonado;
                            // Ensure Fecha is set
                            if (dto.Fecha == default) dto.Fecha = DateTime.Now;

                            var pagoEntity = PagoMapper.ToEntity(dto);
                            // New ID if empty
                            if (pagoEntity.Id == Guid.Empty) pagoEntity.Id = Guid.NewGuid();

                            pagoId = pagoEntity.Id;

                            _pagoRepo.Add(pagoEntity, conn, tran);

                            // 2. INSERT Movimiento positivo
                            TipoMovimiento tipoMovimiento = TipoMovimiento.PagoGenerico;
                            string descMovimiento = $"Pago a Cuenta - {dto.Metodo}";

                            if (dto.EsMembresia)
                            {
                                tipoMovimiento = TipoMovimiento.PagoMembresia;
                                descMovimiento = $"Pago de Membresía - {dto.Metodo}";
                            }
                            else if (dto.ReservaID.HasValue)
                            {
                                tipoMovimiento = TipoMovimiento.PagoReserva;
                                descMovimiento = $"Pago de Reserva - {dto.Metodo}";
                            }

                            var movimiento = new Movimiento
                            {
                                ClienteID = dto.ClienteID,
                                Monto = dto.Monto,
                                Tipo = tipoMovimiento,
                                Descripcion = descMovimiento,
                                Fecha = DateTime.Now,
                                PagoID = pagoEntity.Id
                            };

                            _movimientoRepo.Insertar(movimiento, conn, tran);

                            if (reserva != null)
                            {
                                var pagosAnteriores = _pagoRepo.GetByReserva(reserva.Id, conn, tran);
                                decimal pagadoTotal = 0;
                                foreach (var p in pagosAnteriores)
                                {
                                    if (p.Estado == EstadoPago.Abonado.ToString())
                                    {
                                        pagadoTotal += p.Monto;
                                    }
                                }

                                var espacio = _espacioRepo.GetById(reserva.EspacioID);
                                decimal montoTotal = espacio.PrecioHora * (reserva.Duracion / 60.0m);

                                if (pagadoTotal == montoTotal)
                                {
                                    reserva.Estado = EstadoReserva.Pagada.ToString();
                                    _reservaRepo.Update(reserva, conn, tran);
                                }
                            }

                            tran.Commit();
                        }
                        catch
                        {
                            tran.Rollback();
                            throw;
                        }

                        // Retrieve Saved Pago to get Identity Code for logging (separate operation)
                        try
                        {
                            var savedPago = _pagoRepo.GetById(pagoId);
                            var codigo = savedPago?.Codigo ?? 0;
                            _bitacora.Log($"CU-PA-001: Pago #{codigo} registrado por ${dto.Monto} - Cliente {cliente.DNI}", "INFO");

                            // Auto-generate Comprobante de Pago
                            var bytes = BLL.Helpers.ComprobanteGenerator.GenerarComprobantePago(
                                dto.Fecha,
                                cliente.DNI.ToString(),
                                dto.Monto,
                                dto.Metodo,
                                codigo.ToString()
                            );
                            var comprobanteDto = new ComprobanteDTO
                            {
                                PagoID = pagoId,
                                NombreArchivo = $"Comprobante_Pago_{codigo}.html",
                                Contenido = bytes
                            };
                            _comprobanteFacade.Adjuntar(comprobanteDto);
                        }
                        catch (Exception logEx)
                        {
                            // Fallback logging if retrieval fails
                            _bitacora.Log($"CU-PA-001: Pago registrado (ID: {pagoId}) por ${dto.Monto} - Cliente {cliente.DNI}. Warning: Could not retrieve code for log or generate comprobante: {logEx.Message}", "INFO");

                            try
                            {
                                var bytes = BLL.Helpers.ComprobanteGenerator.GenerarComprobantePago(
                                    dto.Fecha,
                                    cliente.DNI.ToString(),
                                    dto.Monto,
                                    dto.Metodo,
                                    "N/A"
                                );
                                var comprobanteDto = new ComprobanteDTO
                                {
                                    PagoID = pagoId,
                                    NombreArchivo = $"Comprobante_Pago_{pagoId}.html",
                                    Contenido = bytes
                                };
                                _comprobanteFacade.Adjuntar(comprobanteDto);
                            }
                            catch { }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _bitacora.Log($"Error en CU-PA-001: {ex.Message}", "ERROR", ex);
                throw;
            }
        }

        public void ReembolsarPago(Guid pagoId)
        {
            try
            {
                using (var conn = new SqlConnection(ConnectionManager.GetBusinessConnectionString()))
                {
                    conn.Open();
                    Pago pagoParaLog = null;
                    using (var tran = conn.BeginTransaction())
                    {
                        try
                        {
                            // 1. Get Pago
                            var pago = _pagoRepo.GetById(pagoId, conn, tran);
                            if (pago == null) throw new InvalidOperationException("El pago no existe");

                            // Validate State
                            if (pago.Estado != EstadoPago.Abonado.ToString())
                            {
                                throw new InvalidOperationException($"Solo se pueden reembolsar pagos en estado 'Abonado'. Estado actual: {pago.Estado}");
                            }

                            pagoParaLog = pago;

                            // 2. UPDATE Pago
                            pago.Estado = EstadoPago.Reembolsado.ToString();
                            _pagoRepo.Update(pago, conn, tran);

                            // 3. INSERT Movimiento negativo
                            var movimiento = new Movimiento
                            {
                                ClienteID = pago.ClienteID,
                                Monto = -pago.Monto, // Negative
                                Tipo = TipoMovimiento.Reembolso,
                                Descripcion = $"Reembolso de Pago #{pago.Codigo}",
                                Fecha = DateTime.Now,
                                PagoID = pago.Id
                            };
                            _movimientoRepo.Insertar(movimiento, conn, tran);

                            tran.Commit();
                        }
                        catch
                        {
                            tran.Rollback();
                            throw;
                        }
                    }

                    if (pagoParaLog != null)
                    {
                        var cliente = _clienteRepo.GetById(pagoParaLog.ClienteID);
                        _bitacora.Log($"CU-PA-004: Pago #{pagoParaLog.Codigo} reembolsado - Cliente {cliente?.DNI}", "INFO");
                    }
                }
            }
            catch (Exception ex)
            {
                _bitacora.Log($"Error en CU-PA-004: {ex.Message}", "ERROR", ex);
                throw;
            }
        }

        public void AdjuntarComprobante(Guid pagoId, ComprobanteDTO dto)
        {
            try
            {
                dto.PagoID = pagoId;
                if (dto.FechaSubida == default) dto.FechaSubida = DateTime.Now;

                _comprobanteFacade.Adjuntar(dto);

                _bitacora.Log($"CU-PA-003: Comprobante adjuntado al Pago {pagoId}", "INFO");
            }
            catch (Exception ex)
            {
                _bitacora.Log($"Error en CU-PA-003: {ex.Message}", "ERROR", ex);
                throw;
            }
        }

        public ComprobanteDTO ObtenerComprobante(Guid pagoId)
        {
            try
            {
                return _comprobanteFacade.Obtener(pagoId);
            }
            catch (Exception ex)
            {
                _bitacora.Log($"Error al obtener comprobante del pago {pagoId}: {ex.Message}", "ERROR", ex);
                throw;
            }
        }

        public List<PagoDTO> ListarPagos(Guid? clienteId, DateTime? desde, DateTime? hasta)
        {
            List<Pago> list;
            if (clienteId.HasValue)
            {
                list = _pagoRepo.GetByCliente(clienteId.Value, desde, hasta);
            }
            else
            {
                list = _pagoRepo.GetAll();

                // InMemory filter for dates if provided
                if (desde.HasValue) list = list.Where(x => x.Fecha >= desde.Value).ToList();
                if (hasta.HasValue) list = list.Where(x => x.Fecha <= hasta.Value).ToList();
            }

            return list.Select(PagoMapper.ToDTO).ToList();
        }

        public List<PagoDTO> ObtenerPagosPorReserva(Guid reservaId)
        {
            try
            {
                var list = _pagoRepo.GetByReserva(reservaId);
                return list.Select(PagoMapper.ToDTO).ToList();
            }
            catch (Exception ex)
            {
                _bitacora.Log($"Error al obtener pagos de reserva {reservaId}: {ex.Message}", "ERROR", ex);
                throw;
            }
        }
    }
}
