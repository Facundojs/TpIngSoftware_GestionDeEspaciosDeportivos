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

                    // Check if there is any overlapping reservation
                    bool overlaps = false;
                    foreach (var r in reservas)
                    {
                        if (r.Estado == EstadoReserva.Cancelada.ToString())
                            continue;

                        var rInicio = r.FechaHora.TimeOfDay;
                        var rFin = r.FechaHora.AddMinutes(r.Duracion).TimeOfDay;

                        // overlap condition
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

        public bool VerificarDisponibilidad(Guid espacioId, DateTime fechaHora, int duracion)
        {
            return _reservaRepo.EspacioDisponible(espacioId, fechaHora, duracion);
        }

        public void GenerarReserva(GenerarReservaDTO dto)
        {
            if (dto.Adelanto < 0) throw new ArgumentException("El adelanto no puede ser negativo");
            if (dto.Duracion <= 0) throw new ArgumentException("La duración debe ser mayor a cero");
            if (dto.FechaHora < DateTime.Now) throw new ArgumentException("La fecha de reserva no puede ser en el pasado");

            var cliente = _clienteRepo.GetById(dto.ClienteId);
            if (cliente == null) throw new InvalidOperationException($"El cliente con ID {dto.ClienteId} no existe");

            var espacio = _espacioRepo.GetById(dto.EspacioId);
            if (espacio == null) throw new InvalidOperationException($"El espacio con ID {dto.EspacioId} no existe");

            decimal montoTotal = espacio.PrecioHora * (dto.Duracion / 60.0m);

            if (dto.Adelanto > montoTotal) throw new ArgumentException($"El adelanto ({dto.Adelanto:C}) no puede ser mayor al monto total ({montoTotal:C})");

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
                            if (!_reservaRepo.EspacioDisponible(dto.EspacioId, dto.FechaHora, dto.Duracion, conn, tran))
                            {
                                throw new InvalidOperationException("El espacio ya no está disponible (Race Condition detected)");
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

                            _reservaRepo.Add(reserva, conn, tran);

                            var movDeuda = new Movimiento
                            {
                                ClienteID = dto.ClienteId,
                                Monto = -montoTotal,
                                Tipo = TipoMovimiento.DeudaReserva,
                                Descripcion = $"Reserva {reserva.CodigoReserva}",
                                Fecha = DateTime.Now
                            };
                            _movimientoRepo.Insertar(movDeuda, conn, tran);

                            Guid pagoIdParaComprobante = Guid.Empty;
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
                                pagoIdParaComprobante = pago.Id;

                                _pagoRepo.Add(pago, conn, tran);

                                var movPago = new Movimiento
                                {
                                    ClienteID = dto.ClienteId,
                                    Monto = dto.Adelanto,
                                    Tipo = TipoMovimiento.PagoReserva,
                                    Descripcion = $"Pago Adelanto Reserva {reserva.CodigoReserva}",
                                    Fecha = DateTime.Now,
                                    PagoID = pago.Id
                                };
                                _movimientoRepo.Insertar(movPago, conn, tran);
                            }
                            else
                            {
                                // Create a 0-amount placeholder Pago to hold the Comprobante
                                var pagoCero = new Pago
                                {
                                    Id = Guid.NewGuid(),
                                    ClienteID = dto.ClienteId,
                                    Monto = 0,
                                    ReservaID = reserva.Id,
                                    Estado = EstadoPago.Abonado.ToString(),
                                    Metodo = "Reserva sin Adelanto",
                                    Detalle = $"Comprobante Reserva {reserva.CodigoReserva}",
                                    Fecha = DateTime.Now
                                };
                                pagoIdParaComprobante = pagoCero.Id;
                                _pagoRepo.Add(pagoCero, conn, tran);
                            }

                            tran.Commit();

                            // Generate and attach Comprobante de Reserva after successful commit
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
                                    PagoID = pagoIdParaComprobante,
                                    NombreArchivo = $"Comprobante_Reserva_{reserva.CodigoReserva}.html",
                                    Contenido = bytes
                                };

                                var comprobanteFacade = new Facades.ComprobanteFacade();
                                comprobanteFacade.Adjuntar(comprobanteDto);
                            }
                            catch (Exception compEx)
                            {
                                _bitacora.Log($"Warning: Error generating comprobante for Reserva {reserva.CodigoReserva}: {compEx.Message}", "WARN");
                            }

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
                            var reserva = _reservaRepo.GetById(reservaId, conn, tran);

                            if (reserva == null) throw new InvalidOperationException("ERR_RESERVA_NO_EXISTE");

                            if (reserva.Estado == EstadoReserva.Cancelada.ToString())
                            {
                                throw new InvalidOperationException("La reserva ya está cancelada");
                            }
                            if (reserva.Estado == EstadoReserva.Finalizada.ToString())
                            {
                                throw new InvalidOperationException("No se puede cancelar una reserva finalizada");
                            }

                            reservaParaLog = reserva;

                            reserva.Estado = EstadoReserva.Cancelada.ToString();
                            _reservaRepo.Update(reserva, conn, tran);

                            var espacio = _espacioRepo.GetById(reserva.EspacioID);

                            decimal montoTotal = espacio.PrecioHora * (reserva.Duracion / 60.0m);

                            var movReversa = new Movimiento
                            {
                                ClienteID = reserva.ClienteID,
                                Monto = montoTotal,
                                Tipo = TipoMovimiento.CancelacionReserva,
                                Descripcion = $"Cancelación Reserva {reserva.CodigoReserva}",
                                Fecha = DateTime.Now
                            };
                            _movimientoRepo.Insertar(movReversa, conn, tran);

                            var pagos = _pagoRepo.GetByReserva(reservaId, conn, tran);

                            foreach (var pago in pagos)
                            {
                                if (pago.Estado == EstadoPago.Abonado.ToString())
                                {
                                    pago.Estado = EstadoPago.Reembolsado.ToString();
                                    _pagoRepo.Update(pago, conn, tran);

                                    var movReembolso = new Movimiento
                                    {
                                        ClienteID = pago.ClienteID,
                                        Monto = -pago.Monto,
                                        Tipo = TipoMovimiento.Reembolso,
                                        Descripcion = $"Reembolso Reserva {reserva.CodigoReserva}",
                                        Fecha = DateTime.Now,
                                        PagoID = pago.Id
                                    };
                                    _movimientoRepo.Insertar(movReembolso, conn, tran);
                                }
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

        public ReservaDTO ObtenerPorCodigo(string codigo)
        {
            var r = _reservaRepo.GetByCodigo(codigo);
            if (r == null) return null;

            var dto = ReservaMapper.ToDTO(r);
            var c = _clienteRepo.GetById(dto.ClienteID);
            dto.ClienteNombre = c != null ? $"{c.Nombre} {c.Apellido}" : "Desconocido";

            var e = _espacioRepo.GetById(dto.EspacioID);
            dto.EspacioNombre = e != null ? e.Nombre : "Desconocido";

            return dto;
        }

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
