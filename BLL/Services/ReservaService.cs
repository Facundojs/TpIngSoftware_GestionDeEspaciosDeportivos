using BLL.DTOs;
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
    /// <summary>
    /// Business logic service for reservation management.
    /// </summary>
    /// <remarks>
    /// Handles availability computation, reservation creation (with race-condition guard),
    /// cancellation with automatic refunds, and receipt generation (non-fatal).
    /// All multi-step write operations run inside <see cref="DAL.Contracts.IUnitOfWork"/> transactions.
    /// </remarks>
    public class ReservaService
    {
        private readonly IReservaRepository _reservaRepo;
        private readonly IMovimientoRepository _movimientoRepo;
        private readonly IPagoRepository _pagoRepo;
        private readonly IEspacioRepository _espacioRepo;
        private readonly IClienteRepository _clienteRepo;
        private readonly BitacoraService _bitacora;

        /// <summary>Initializes all dependencies from <see cref="DAL.Factory.DalFactory"/> singletons.</summary>
        public ReservaService()
        {
            _reservaRepo = DalFactory.ReservaRepository;
            _movimientoRepo = DalFactory.MovimientoRepository;
            _pagoRepo = DalFactory.PagoRepository;
            _espacioRepo = DalFactory.EspacioRepository;
            _clienteRepo = DalFactory.ClienteRepository;
            _bitacora = new BitacoraService();
        }

        /// <summary>
        /// Returns all free 30-minute time slots for a space on a given date based on the space's agenda.
        /// </summary>
        /// <param name="espacioId">The space to query.</param>
        /// <param name="fecha">The date to check (time component is ignored).</param>
        /// <returns>List of available start times as <see cref="TimeSpan"/> values.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown with code <c>"ERR_NO_AGENDA"</c> if no agenda is configured for this space.
        /// </exception>
        /// <summary>
        /// Returns the list of 30-minute time slots that are available for reservation on <paramref name="fecha"/>
        /// based on the space's agenda and existing non-cancelled bookings.
        /// </summary>
        /// <param name="espacioId">The space to query.</param>
        /// <param name="fecha">The date to check. Day-of-week is used to match agenda blocks.</param>
        /// <returns>List of available start times as <see cref="TimeSpan"/> values.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown with code <c>"ERR_NO_AGENDA"</c> if the space has no configured schedule.
        /// </exception>
        public List<TimeSpan> ObtenerHorariosDisponibles(Guid espacioId, DateTime fecha)
        {
            var agendaService = new AgendaService();
            var agenda = agendaService.GetAgendaPorEspacio(espacioId);
            if (agenda == null || agenda.Count == 0)
            {
                throw new InvalidOperationException("ERR_NO_AGENDA");
            }

            var reservas = _reservaRepo.GetByEspacio(espacioId, fecha.Date, fecha.Date.AddDays(1));

            var disponibles = new List<TimeSpan>();
            int dayOfWeek = (int)fecha.DayOfWeek;

            foreach (var bloque in agenda.Where(a => a.DiaSemana == dayOfWeek))
            {
                var inicio = bloque.HoraDesde;
                var fin = bloque.HoraHasta;

                var current = inicio;
                while (current.Add(TimeSpan.FromMinutes(30)) <= fin)
                {
                    var currTime = current;
                    var nextTime = current.Add(TimeSpan.FromMinutes(30));

                    bool overlaps = false;
                    foreach (var r in reservas)
                    {
                        if (r.Estado == EstadoReserva.Cancelada.ToString())
                            continue;

                        var rInicio = r.FechaHora.TimeOfDay;
                        var rFin = r.FechaHora.AddMinutes(r.Duracion).TimeOfDay;

                        if (currTime < rFin && rInicio < nextTime)
                        {
                            overlaps = true;
                            break;
                        }
                    }

                    if (!overlaps)
                    {
                        disponibles.Add(currTime);
                    }

                    current = nextTime;
                }
            }

            return disponibles;
        }

        /// <summary>
        /// Checks whether a space is available for the requested time slot (delegates to repository).
        /// </summary>
        /// <param name="espacioId">The space to check.</param>
        /// <param name="fechaHora">Proposed start date and time.</param>
        /// <param name="duracion">Duration in minutes.</param>
        /// <returns><c>true</c> if no conflicting active reservation exists.</returns>
        /// <summary>
        /// Performs a point-in-time availability check (pre-booking verification, outside a transaction).
        /// </summary>
        /// <param name="espacioId">The space to check.</param>
        /// <param name="fechaHora">Proposed reservation start.</param>
        /// <param name="duracion">Duration in minutes.</param>
        /// <returns><c>true</c> if the space is available; <c>false</c> if already booked.</returns>
        public bool VerificarDisponibilidad(Guid espacioId, DateTime fechaHora, int duracion)
        {
            return _reservaRepo.EspacioDisponible(espacioId, fechaHora, duracion);
        }

        /// <summary>
        /// Creates a reservation with a down payment, guarding against race conditions via
        /// a double-checked availability lock inside the transaction.
        /// </summary>
        /// <param name="dto">Reservation parameters including optional advance payment amount.</param>
        /// <returns>The generated reservation code (e.g., <c>"RES-123456-ABCD"</c>).</returns>
        /// <exception cref="ArgumentException">
        /// Thrown for invalid inputs: negative advance, zero duration, past date, advance exceeding total,
        /// or advance below the 10% minimum.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the space is no longer available at transaction time (race condition).
        /// </exception>
        /// <summary>
        /// Creates a reservation and optionally records an advance payment, all within one transaction.
        /// Availability is verified twice (before and inside the transaction) to guard against race conditions.
        /// A <c>DeudaReserva</c> movement and, if an advance is provided, a <c>PagoReserva</c> movement
        /// are inserted. An HTML receipt is generated post-commit (failure is non-fatal and only logged).
        /// </summary>
        /// <param name="dto">Reservation parameters including client, space, time, duration, and advance amount.</param>
        /// <returns>The unique reservation code (e.g., <c>"RES-123456-ABCD"</c>).</returns>
        /// <exception cref="ArgumentException">Thrown for invalid amount, duration, or past date.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the space is unavailable or the advance exceeds the total.</exception>
        public string GenerarReserva(GenerarReservaDTO dto)
        {
            if (dto.Adelanto < 0) throw new ArgumentException("ERR_ADELANTO_NEGATIVO".Translate());
            if (dto.Duracion <= 0) throw new ArgumentException("ERR_DURACION_CERO".Translate());
            if (dto.FechaHora < DateTime.Now) throw new ArgumentException("ERR_FECHA_PASADA".Translate());

            var cliente = _clienteRepo.GetById(dto.ClienteId);
            if (cliente == null) throw new InvalidOperationException("ERR_CLIENTE_NO_ENCONTRADO".Translate());

            var espacio = _espacioRepo.GetById(dto.EspacioId);
            if (espacio == null) throw new InvalidOperationException("ERR_ESPACIO_NO_ENCONTRADO".Translate());

            decimal montoTotal = espacio.PrecioHora * (dto.Duracion / 60.0m);

            if (dto.Adelanto > montoTotal) throw new ArgumentException("ERR_ADELANTO_MAYOR_TOTAL".Translate());

            decimal minimoRequerido = montoTotal * 0.1m;
            if (dto.Adelanto < minimoRequerido)
            {
                throw new ArgumentException("ERR_ADELANTO_MINIMO".Translate());
            }

            if (!_reservaRepo.EspacioDisponible(dto.EspacioId, dto.FechaHora, dto.Duracion))
            {
                throw new InvalidOperationException("MSG_ESPACIO_NO_DISPONIBLE".Translate());
            }

            try
            {
                using (var uow = DalFactory.CreateUnitOfWork())
                {
                    try
                    {
                        uow.BeginTransaction();

                        if (!uow.ReservaRepository.EspacioDisponible(dto.EspacioId, dto.FechaHora, dto.Duracion))
                        {
                            throw new InvalidOperationException("Space is no longer available (Race Condition).");
                        }

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

                        uow.ReservaRepository.Add(reserva);

                        var movDeuda = new Movimiento
                        {
                            ClienteID = dto.ClienteId,
                            Monto = -montoTotal,
                            Tipo = TipoMovimiento.DeudaReserva,
                            Descripcion = $"Reservation {reserva.CodigoReserva}",
                            Fecha = DateTime.Now
                        };
                        uow.MovimientoRepository.Insertar(movDeuda);

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
                                Detalle = $"Down payment Reservation {reserva.CodigoReserva}",
                                Fecha = DateTime.Now
                            };

                            uow.PagoRepository.Add(pago);

                            var movPago = new Movimiento
                            {
                                ClienteID = dto.ClienteId,
                                Monto = dto.Adelanto,
                                Tipo = TipoMovimiento.PagoReserva,
                                Descripcion = $"Down payment Reservation {reserva.CodigoReserva}",
                                Fecha = DateTime.Now,
                                PagoID = pago.Id
                            };
                            uow.MovimientoRepository.Insertar(movPago);
                        }

                        uow.Commit();

                        try
                        {
                            decimal saldo = montoTotal - dto.Adelanto;
                            var bytes = BLL.Helpers.ComprobanteGenerator.GenerarComprobanteReserva(
                                reserva.CodigoReserva,
                                cliente.DNI.ToString(),
                                espacio.Nombre,
                                reserva.FechaHora,
                                montoTotal,
                                dto.Adelanto,
                                saldo
                            );

                            var comprobanteDto = new ComprobanteDTO
                            {
                                ReservaID = reserva.Id,
                                NombreArchivo = $"Reservation_Receipt_{reserva.CodigoReserva}.html",
                                Contenido = bytes
                            };

                            var comprobanteFacade = new Facades.ComprobanteFacade();
                            comprobanteFacade.Adjuntar(comprobanteDto);
                        }
                        catch (Exception compEx)
                        {
                            _bitacora.Log($"Warning: Receipt generation failed for Reservation {reserva.CodigoReserva}: {compEx.Message}", "WARN");
                        }

                        _bitacora.Log($"CU-RES-01: Reservation {reserva.CodigoReserva} generated for client {cliente.DNI}", "INFO");

                        return reserva.CodigoReserva;
                    }
                    catch
                    {
                        uow.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                _bitacora.Log($"Error in CU-RES-01: {ex.Message}", "ERROR", ex);
                throw;
            }
        }

        /// <summary>
        /// Cancels a reservation: marks it <c>Cancelada</c>, inserts a reversal movement,
        /// and transitions all <c>Abonado</c> payments to <c>Reembolsado</c> with corresponding negative movements.
        /// </summary>
        /// <param name="reservaId">The reservation to cancel.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the reservation does not exist, is already cancelled, or is finalized.
        /// </exception>
        /// <summary>
        /// Cancels a reservation, reverses its debt movement, and refunds all <c>Abonado</c> payments.
        /// All operations execute within a single transaction.
        /// </summary>
        /// <param name="reservaId">The reservation to cancel.</param>
        /// <exception cref="InvalidOperationException">Thrown if the reservation is already cancelled or finalized.</exception>
        public void CancelarReserva(Guid reservaId)
        {
            try
            {
                using (var uow = DalFactory.CreateUnitOfWork())
                {
                    Reserva reservaParaLog = null;
                    try
                    {
                        uow.BeginTransaction();

                        var reserva = uow.ReservaRepository.GetById(reservaId);

                        if (reserva == null) throw new InvalidOperationException("ERR_RESERVA_NO_EXISTE");

                        if (reserva.Estado == EstadoReserva.Cancelada.ToString())
                        {
                            throw new InvalidOperationException("Reservation is already cancelled.");
                        }
                        if (reserva.Estado == EstadoReserva.Finalizada.ToString())
                        {
                            throw new InvalidOperationException("Cannot cancel a completed reservation.");
                        }

                        reservaParaLog = reserva;

                        reserva.Estado = EstadoReserva.Cancelada.ToString();
                        uow.ReservaRepository.Update(reserva);

                        var espacio = _espacioRepo.GetById(reserva.EspacioID);

                        decimal montoTotal = espacio.PrecioHora * (reserva.Duracion / 60.0m);

                        var movReversa = new Movimiento
                        {
                            ClienteID = reserva.ClienteID,
                            Monto = montoTotal,
                            Tipo = TipoMovimiento.CancelacionReserva,
                            Descripcion = $"Cancellation Reservation {reserva.CodigoReserva}",
                            Fecha = DateTime.Now
                        };
                        uow.MovimientoRepository.Insertar(movReversa);

                        var pagos = uow.PagoRepository.GetByReserva(reservaId);

                        foreach (var pago in pagos)
                        {
                            if (pago.Estado == EstadoPago.Abonado.ToString())
                            {
                                pago.Estado = EstadoPago.Reembolsado.ToString();
                                uow.PagoRepository.Update(pago);

                                var movReembolso = new Movimiento
                                {
                                    ClienteID = pago.ClienteID,
                                    Monto = -pago.Monto,
                                    Tipo = TipoMovimiento.Reembolso,
                                    Descripcion = $"Refund Reservation {reserva.CodigoReserva}",
                                    Fecha = DateTime.Now,
                                    PagoID = pago.Id
                                };
                                uow.MovimientoRepository.Insertar(movReembolso);
                            }
                        }

                        uow.Commit();

                        _bitacora.Log($"CU-RES-02: Reservation {reservaParaLog.CodigoReserva} cancelled", "INFO");
                    }
                    catch
                    {
                        uow.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                _bitacora.Log($"Error in CU-RES-02: {ex.Message}", "ERROR", ex);
                throw;
            }
        }

        /// <summary>
        /// Retrieves a reservation by its unique code, with <c>ClienteNombre</c> and <c>EspacioNombre</c> hydrated.
        /// </summary>
        /// <param name="codigo">The reservation code.</param>
        /// <returns>The hydrated <see cref="ReservaDTO"/>, or <c>null</c> if not found.</returns>
        /// <summary>Retrieves a single reservation by its unique code, with client and space names hydrated.</summary>
        /// <param name="codigo">The reservation code.</param>
        /// <returns>The hydrated <see cref="ReservaDTO"/>, or <c>null</c> if not found.</returns>
        public ReservaDTO ObtenerPorCodigo(string codigo)
        {
            var r = _reservaRepo.GetByCodigo(codigo);
            if (r == null) return null;

            var dto = ReservaMapper.ToDTO(r);
            var c = _clienteRepo.GetById(dto.ClienteID);
            dto.ClienteNombre = c != null ? $"{c.Nombre} {c.Apellido}" : "Unknown";

            var e = _espacioRepo.GetById(dto.EspacioID);
            dto.EspacioNombre = e != null ? e.Nombre : "Unknown";

            return dto;
        }

        /// <summary>
        /// Returns reservations filtered by any combination of client, space, and minimum date.
        /// All returned DTOs have <c>ClienteNombre</c> and <c>EspacioNombre</c> hydrated.
        /// </summary>
        /// <param name="clienteId">Optional client filter.</param>
        /// <param name="espacioId">Optional space filter.</param>
        /// <param name="desde">Optional minimum <c>FechaHora</c> filter.</param>
        /// <summary>
        /// Returns reservations filtered by any combination of client, space, and start date.
        /// All returned DTOs include client and space names.
        /// </summary>
        /// <param name="clienteId">Optional client filter.</param>
        /// <param name="espacioId">Optional space filter.</param>
        /// <param name="desde">Optional lower bound on reservation date/time.</param>
        public List<ReservaDTO> ListarReservas(Guid? clienteId, Guid? espacioId, DateTime? desde)
        {
            List<Reserva> reservas;

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

            foreach (var dto in dtos)
            {
                var c = _clienteRepo.GetById(dto.ClienteID);
                dto.ClienteNombre = c != null ? $"{c.Nombre} {c.Apellido}" : "Unknown";

                var e = _espacioRepo.GetById(dto.EspacioID);
                dto.EspacioNombre = e != null ? e.Nombre : "Unknown";
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
