using BLL.DTOs;
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
    public class ReservaService
    {
        private readonly IReservaRepository _reservaRepo;
        private readonly IMovimientoRepository _movimientoRepo;
        private readonly IPagoRepository _pagoRepo;
        private readonly IEspacioRepository _espacioRepo;
        private readonly IClienteRepository _clienteRepo;
        private readonly BitacoraService _bitacora;

        public ReservaService()
        {
            _reservaRepo = DalFactory.ReservaRepository;
            _movimientoRepo = DalFactory.MovimientoRepository;
            _pagoRepo = DalFactory.PagoRepository;
            _espacioRepo = DalFactory.EspacioRepository;
            _clienteRepo = DalFactory.ClienteRepository;
            _bitacora = new BitacoraService();
        }

        public void GenerarReserva(GenerarReservaDTO dto)
        {
            // Validaciones Técnicas
            if (dto.Adelanto < 0) throw new ArgumentException("El adelanto no puede ser negativo");
            if (dto.Duracion <= 0) throw new ArgumentException("La duración debe ser mayor a cero");
            if (dto.FechaHora < DateTime.Now) throw new ArgumentException("La fecha de reserva no puede ser en el pasado");

            // Validaciones de Negocio (Existencia)
            var cliente = _clienteRepo.GetById(dto.ClienteId);
            if (cliente == null) throw new InvalidOperationException($"El cliente con ID {dto.ClienteId} no existe");

            var espacio = _espacioRepo.GetById(dto.EspacioId);
            if (espacio == null) throw new InvalidOperationException($"El espacio con ID {dto.EspacioId} no existe");

            // Calcular Monto Total
            decimal montoTotal = espacio.PrecioHora * (dto.Duracion / 60.0m);

            if (dto.Adelanto > montoTotal) throw new ArgumentException($"El adelanto ({dto.Adelanto:C}) no puede ser mayor al monto total ({montoTotal:C})");

            // 1. Validar disponibilidad (antes de transacción para performance, non-transactional check)
            if (!_reservaRepo.EspacioDisponible(dto.EspacioId, dto.FechaHora, dto.Duracion))
            {
                throw new InvalidOperationException("El espacio no está disponible en el horario seleccionado");
            }

            try
            {
                using (var conn = new SqlConnection(ConnectionManager.GetBusinessConnectionString()))
                {
                    conn.Open();
                    using (var tran = conn.BeginTransaction())
                    {
                        try
                        {
                            // Re-check Availability inside Transaction to prevent Race Conditions
                            if (!_reservaRepo.EspacioDisponible(dto.EspacioId, dto.FechaHora, dto.Duracion, conn, tran))
                            {
                                throw new InvalidOperationException("El espacio ya no está disponible (Race Condition detected)");
                            }

                            // 2. INSERT Reserva
                            var reserva = new Reserva
                            {
                                Id = Guid.NewGuid(),
                                ClienteID = dto.ClienteId,
                                EspacioID = dto.EspacioId,
                                Fecha = dto.FechaHora.Date,
                                FechaHora = dto.FechaHora,
                                Duracion = dto.Duracion,
                                Adelanto = dto.Adelanto,
                                CodigoReserva = GenerarCodigoUnico(),
                                Estado = EstadoReserva.Pendiente.ToString()
                            };

                            _reservaRepo.Add(reserva, conn, tran);

                            // 3. INSERT Movimiento deuda total (negativo)
                            var movDeuda = new Movimiento
                            {
                                ClienteID = dto.ClienteId,
                                Monto = -montoTotal,
                                Tipo = "DeudaReserva",
                                Descripcion = $"Reserva {reserva.CodigoReserva}",
                                Fecha = DateTime.Now
                            };
                            _movimientoRepo.Insertar(movDeuda, conn, tran);

                            // 4. Si adelanto > 0: registrar pago
                            if (dto.Adelanto > 0)
                            {
                                var pago = new Pago
                                {
                                    Id = Guid.NewGuid(),
                                    ClienteID = dto.ClienteId,
                                    Monto = dto.Adelanto,
                                    ReservaID = reserva.Id,
                                    Estado = EstadoPago.Abonado.ToString(),
                                    Metodo = "Adelanto",
                                    Detalle = $"Adelanto Reserva {reserva.CodigoReserva}",
                                    Fecha = DateTime.Now
                                };

                                _pagoRepo.Add(pago, conn, tran);

                                var movPago = new Movimiento
                                {
                                    ClienteID = dto.ClienteId,
                                    Monto = dto.Adelanto, // POSITIVO
                                    Tipo = "PagoReserva",
                                    Descripcion = $"Pago Adelanto Reserva {reserva.CodigoReserva}",
                                    Fecha = DateTime.Now,
                                    PagoID = pago.Id
                                };
                                _movimientoRepo.Insertar(movPago, conn, tran);
                            }

                            tran.Commit();

                            _bitacora.Log($"CU-RES-01: Reserva {reserva.CodigoReserva} generada para cliente {cliente.DNI} - Espacio {espacio.Nombre} - Fecha {reserva.FechaHora} - Seña ${dto.Adelanto}", "INFO");
                        }
                        catch
                        {
                            tran.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _bitacora.Log($"Error en CU-RES-01: {ex.Message}", "ERROR", ex);
                throw;
            }
        }

        public void CancelarReserva(Guid reservaId)
        {
            try
            {
                using (var conn = new SqlConnection(ConnectionManager.GetBusinessConnectionString()))
                {
                    conn.Open();
                    Reserva reservaParaLog = null;
                    using (var tran = conn.BeginTransaction())
                    {
                        try
                        {
                            // 1. Get Reserva (Transactional Read)
                            // Now using interface overload without casting
                            var reserva = _reservaRepo.GetById(reservaId, conn, tran);

                            if (reserva == null) throw new InvalidOperationException("La reserva no existe");

                            if (reserva.Estado == EstadoReserva.Cancelada.ToString())
                            {
                                throw new InvalidOperationException("La reserva ya está cancelada");
                            }
                            if (reserva.Estado == EstadoReserva.Finalizada.ToString())
                            {
                                throw new InvalidOperationException("No se puede cancelar una reserva finalizada");
                            }

                            reservaParaLog = reserva;

                            // 2. UPDATE Reserva
                            reserva.Estado = EstadoReserva.Cancelada.ToString();
                            _reservaRepo.Update(reserva, conn, tran);

                            // Calculate amount to reverse (Total Debt)
                            var espacio = _espacioRepo.GetById(reserva.EspacioID);

                            decimal montoTotal = espacio.PrecioHora * (reserva.Duracion / 60.0m);

                            // 2. INSERT Movimiento reversa (positivo) por monto total
                            var movReversa = new Movimiento
                            {
                                ClienteID = reserva.ClienteID,
                                Monto = montoTotal, // Positive to cancel debt
                                Tipo = "CancelacionReserva",
                                Descripcion = $"Cancelación Reserva {reserva.CodigoReserva}",
                                Fecha = DateTime.Now
                            };
                            _movimientoRepo.Insertar(movReversa, conn, tran);

                            // 3. Check for Payment (Adelanto)
                            // Use Transactional GetByReserva
                            var pago = _pagoRepo.GetByReserva(reservaId, conn, tran);

                            if (pago != null && pago.Estado == EstadoPago.Abonado.ToString())
                            {
                                // Refund Logic
                                pago.Estado = EstadoPago.Reembolsado.ToString();
                                _pagoRepo.Update(pago, conn, tran);

                                var movReembolso = new Movimiento
                                {
                                    ClienteID = pago.ClienteID,
                                    Monto = -pago.Monto, // Negative
                                    Tipo = "Reembolso",
                                    Descripcion = $"Reembolso Reserva {reserva.CodigoReserva}",
                                    Fecha = DateTime.Now,
                                    PagoID = pago.Id
                                };
                                _movimientoRepo.Insertar(movReembolso, conn, tran);
                            }

                            tran.Commit();

                            _bitacora.Log($"CU-RES-02: Reserva {reservaParaLog.CodigoReserva} cancelada", "INFO");
                        }
                        catch
                        {
                            tran.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _bitacora.Log($"Error en CU-RES-02: {ex.Message}", "ERROR", ex);
                throw;
            }
        }

        public List<ReservaDTO> ListarReservas(Guid? clienteId, Guid? espacioId, DateTime? desde)
        {
            List<Reserva> reservas;

            // Simple filtering strategy
            if (clienteId.HasValue)
            {
                reservas = _reservaRepo.GetByCliente(clienteId.Value);
                if (espacioId.HasValue) reservas = reservas.Where(r => r.EspacioID == espacioId.Value).ToList();
                if (desde.HasValue) reservas = reservas.Where(r => r.FechaHora >= desde.Value).ToList();
            }
            else if (espacioId.HasValue)
            {
                 if (desde.HasValue)
                    reservas = _reservaRepo.GetByEspacio(espacioId.Value, desde.Value, DateTime.MaxValue);
                 else
                    reservas = _reservaRepo.GetByEspacio(espacioId.Value, DateTime.MinValue, DateTime.MaxValue);
            }
            else
            {
                reservas = _reservaRepo.GetAll();
                if (desde.HasValue) reservas = reservas.Where(r => r.FechaHora >= desde.Value).ToList();
            }

            var dtos = reservas.Select(ReservaMapper.ToDTO).ToList();

            // Enrich with Names
            foreach (var dto in dtos)
            {
                var c = _clienteRepo.GetById(dto.ClienteID);
                dto.ClienteNombre = c != null ? $"{c.Nombre} {c.Apellido}" : "Desconocido";

                var e = _espacioRepo.GetById(dto.EspacioID);
                dto.EspacioNombre = e != null ? e.Nombre : "Desconocido";
            }

            return dtos;
        }

        private string GenerarCodigoUnico()
        {
            var timestamp = DateTime.Now.Ticks.ToString().Substring(10);
            var random = Guid.NewGuid().ToString().Substring(0, 4).ToUpper();
            return $"RES-{timestamp}-{random}";
        }
    }
}
