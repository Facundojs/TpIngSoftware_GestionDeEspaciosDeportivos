using BLL.DTOs;
using BLL.Mappers;
using DAL.Contracts;
using DAL.Factory;
using Domain.Entities;
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
        private readonly IComprobanteRepository _comprobanteRepo;
        private readonly IClienteRepository _clienteRepo;
        private readonly BitacoraService _bitacora;

        public PagoService()
        {
            _pagoRepo = DalFactory.PagoRepository;
            _movimientoRepo = DalFactory.MovimientoRepository;
            _comprobanteRepo = DalFactory.ComprobanteRepository;
            _clienteRepo = DalFactory.ClienteRepository;
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
                            // 1. INSERT Pago
                            dto.Estado = "Abonado";
                            // Ensure Fecha is set
                            if (dto.Fecha == default) dto.Fecha = DateTime.Now;

                            var pagoEntity = PagoMapper.ToEntity(dto);
                            // New ID if empty
                            if (pagoEntity.Id == Guid.Empty) pagoEntity.Id = Guid.NewGuid();

                            pagoId = pagoEntity.Id;

                            _pagoRepo.Add(pagoEntity, conn, tran);

                            // 2. INSERT Movimiento positivo
                            var movimiento = new Movimiento
                            {
                                ClienteID = dto.ClienteID,
                                Monto = dto.Monto,
                                Tipo = dto.EsMembresia ? "PagoMembresia" : "PagoReserva",
                                Descripcion = $"Pago de {(dto.EsMembresia ? "Membresía" : "Reserva")} - {dto.Metodo}",
                                Fecha = DateTime.Now,
                                PagoID = pagoEntity.Id
                            };

                            _movimientoRepo.Insertar(movimiento, conn, tran);

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
                            _bitacora.Log($"CU-PA-001: Pago #{codigo} registrado por ${dto.Monto} - Cliente {cliente.Dni}", "INFO");
                        }
                        catch (Exception logEx)
                        {
                            // Fallback logging if retrieval fails
                            _bitacora.Log($"CU-PA-001: Pago registrado (ID: {pagoId}) por ${dto.Monto} - Cliente {cliente.Dni}. Warning: Could not retrieve code for log: {logEx.Message}", "INFO");
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
                            if (pago.Estado != "Abonado")
                            {
                                throw new InvalidOperationException($"Solo se pueden reembolsar pagos en estado 'Abonado'. Estado actual: {pago.Estado}");
                            }

                            pagoParaLog = pago;

                            // 2. UPDATE Pago
                            pago.Estado = "Reembolsado";
                            _pagoRepo.Update(pago, conn, tran);

                            // 3. INSERT Movimiento negativo
                            var movimiento = new Movimiento
                            {
                                ClienteID = pago.ClienteID,
                                Monto = -pago.Monto, // Negative
                                Tipo = "Reembolso",
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
                        _bitacora.Log($"CU-PA-004: Pago #{pagoParaLog.Codigo} reembolsado - Cliente {cliente?.Dni ?? "Desconocido"}", "INFO");
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

                var entity = PagoMapper.ToEntity(dto);
                _comprobanteRepo.Agregar(entity);

                _bitacora.Log($"CU-PA-003: Comprobante adjuntado al Pago {pagoId}", "INFO");
            }
            catch (Exception ex)
            {
                _bitacora.Log($"Error en CU-PA-003: {ex.Message}", "ERROR", ex);
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
    }
}
