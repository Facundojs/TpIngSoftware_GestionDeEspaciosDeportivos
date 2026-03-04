using BLL.DTOs;
using BLL.Facades;
using BLL.Mappers;
using DAL.Contracts;
using DAL.Factory;
using Domain.Entities;
using Domain.Enums;
using Service.Facade.Extension;
using Service.Logic;
using System;
using System.Collections.Generic;
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
            if (dto.Monto <= 0) throw new ArgumentException(Translations.ERR_MONTO_INVALIDO.Translate());
            if (string.IsNullOrWhiteSpace(dto.Metodo)) throw new ArgumentException(Translations.ERR_METODO_REQUERIDO.Translate());

            try
            {
                var cliente = _clienteRepo.GetById(dto.ClienteID);
                if (cliente == null) throw new InvalidOperationException($"Client with ID {dto.ClienteID} does not exist.");

                using (var uow = DalFactory.CreateUnitOfWork())
                {
                    Guid pagoId = Guid.Empty;
                    try
                    {
                        uow.BeginTransaction();

                        Reserva reserva = null;
                        if (dto.ReservaID.HasValue)
                        {
                            reserva = uow.ReservaRepository.GetById(dto.ReservaID.Value);
                            if (reserva == null) throw new InvalidOperationException("ERR_RESERVA_NO_EXISTE");

                            var espacio = _espacioRepo.GetById(reserva.EspacioID);
                            decimal montoTotal = espacio.PrecioHora * (reserva.Duracion / 60.0m);

                            var pagosAnteriores = uow.PagoRepository.GetByReserva(reserva.Id);
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

                        dto.Estado = EstadoPago.Abonado;
                        if (dto.Fecha == default) dto.Fecha = DateTime.Now;

                        var pagoEntity = PagoMapper.ToEntity(dto);
                        if (pagoEntity.Id == Guid.Empty) pagoEntity.Id = Guid.NewGuid();

                        pagoId = pagoEntity.Id;

                        uow.PagoRepository.Add(pagoEntity);

                        TipoMovimiento tipoMovimiento = TipoMovimiento.PagoGenerico;
                        string descMovimiento = $"Account Payment - {dto.Metodo}";

                        if (dto.EsMembresia)
                        {
                            tipoMovimiento = TipoMovimiento.PagoMembresia;
                            descMovimiento = $"Membership Payment - {dto.Metodo}";

                            var clienteMembresia = uow.ClienteMembresiaRepository.GetActiveByClienteId(dto.ClienteID);
                            if (clienteMembresia != null && clienteMembresia.ProximaFechaPago.HasValue && dto.MembresiaID.HasValue)
                            {
                                var membresia = DalFactory.MembresiaRepository.GetById(dto.MembresiaID.Value);
                                if (membresia != null)
                                {
                                    clienteMembresia.ProximaFechaPago = clienteMembresia.ProximaFechaPago.Value.AddDays(membresia.Regularidad);
                                    uow.ClienteMembresiaRepository.Update(clienteMembresia);
                                }
                            }
                        }
                        else if (dto.ReservaID.HasValue)
                        {
                            tipoMovimiento = TipoMovimiento.PagoReserva;
                            descMovimiento = $"Reservation Payment - {dto.Metodo}";
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

                        uow.MovimientoRepository.Insertar(movimiento);

                        if (reserva != null)
                        {
                            var pagosAnteriores = uow.PagoRepository.GetByReserva(reserva.Id);
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
                                uow.ReservaRepository.Update(reserva);
                            }
                        }

                        uow.Commit();
                    }
                    catch
                    {
                        uow.Rollback();
                        throw;
                    }

                    try
                    {
                        var savedPago = _pagoRepo.GetById(pagoId);
                        var codigo = savedPago?.Codigo ?? 0;
                        _bitacora.Log($"CU-PA-001: Payment #{codigo} registered for ${dto.Monto} - Client {cliente.DNI}", "INFO");

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
                            NombreArchivo = $"Payment_Receipt_{codigo}.html",
                            Contenido = bytes
                        };
                        _comprobanteFacade.Adjuntar(comprobanteDto);
                    }
                    catch (Exception logEx)
                    {
                        _bitacora.Log($"CU-PA-001: Payment registered (ID: {pagoId}) for ${dto.Monto} - Client {cliente.DNI}. Receipt generation failed: {logEx.Message}", "INFO");

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
                                NombreArchivo = $"Payment_Receipt_{pagoId}.html",
                                Contenido = bytes
                            };
                            _comprobanteFacade.Adjuntar(comprobanteDto);
                        }
                        catch { }
                    }
                }
            }
            catch (Exception ex)
            {
                _bitacora.Log($"Error in CU-PA-001: {ex.Message}", "ERROR", ex);
                throw;
            }
        }

        public void ReembolsarPago(Guid pagoId)
        {
            try
            {
                Pago pagoParaLog = null;
                using (var uow = DalFactory.CreateUnitOfWork())
                {
                    try
                    {
                        uow.BeginTransaction();

                        var pago = uow.PagoRepository.GetById(pagoId);
                        if (pago == null) throw new InvalidOperationException("Payment does not exist.");

                        if (pago.Estado != EstadoPago.Abonado.ToString())
                        {
                            throw new InvalidOperationException($"Only 'Paid' payments can be refunded. Current status: {pago.Estado}");
                        }

                        pagoParaLog = pago;

                        pago.Estado = EstadoPago.Reembolsado.ToString();
                        uow.PagoRepository.Update(pago);

                        var movimiento = new Movimiento
                        {
                            ClienteID = pago.ClienteID,
                            Monto = -pago.Monto,
                            Tipo = TipoMovimiento.Reembolso,
                            Descripcion = $"Refund of Payment #{pago.Codigo}",
                            Fecha = DateTime.Now,
                            PagoID = pago.Id
                        };
                        uow.MovimientoRepository.Insertar(movimiento);

                        uow.Commit();
                    }
                    catch
                    {
                        uow.Rollback();
                        throw;
                    }

                    if (pagoParaLog != null)
                    {
                        var cliente = _clienteRepo.GetById(pagoParaLog.ClienteID);
                        _bitacora.Log($"CU-PA-004: Payment #{pagoParaLog.Codigo} refunded - Client {cliente?.DNI}", "INFO");
                    }
                }
            }
            catch (Exception ex)
            {
                _bitacora.Log($"Error in CU-PA-004: {ex.Message}", "ERROR", ex);
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

                _bitacora.Log($"CU-PA-003: Receipt attached to Payment {pagoId}", "INFO");
            }
            catch (Exception ex)
            {
                _bitacora.Log($"Error in CU-PA-003: {ex.Message}", "ERROR", ex);
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
                _bitacora.Log($"Error fetching receipt for payment {pagoId}: {ex.Message}", "ERROR", ex);
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
                _bitacora.Log($"Error fetching payments for reservation {reservaId}: {ex.Message}", "ERROR", ex);
                throw;
            }
        }
    }
}
