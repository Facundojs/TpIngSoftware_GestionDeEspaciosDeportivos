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
    /// Business logic service for workout routine lifecycle management.
    /// </summary>
    /// <remarks>
    /// A client can have at most one active routine at any time. Creating a new routine
    /// automatically finalizes the existing one within the same transaction.
    /// Exercises are resolved by name and auto-created in the catalog if not found.
    /// </remarks>
    public class RutinaService
    {
        private readonly IRutinaRepository _rutinaRepository;
        private readonly IEjercicioRepository _ejercicioRepository;
        private readonly IRutinaEjercicioRepository _rutinaEjercicioRepository;
        private readonly IClienteRepository _clienteRepository;
        private readonly BitacoraService _bitacoraService;

        /// <summary>Initializes all dependencies from <see cref="DAL.Factory.DalFactory"/> singletons.</summary>
        public RutinaService()
        {
            _rutinaRepository = DalFactory.RutinaRepository;
            _ejercicioRepository = DalFactory.EjercicioRepository;
            _rutinaEjercicioRepository = DalFactory.RutinaEjercicioRepository;
            _clienteRepository = DalFactory.ClienteRepository;
            _bitacoraService = new BitacoraService();
        }

        /// <summary>
        /// Creates a new routine for a client. If an active routine exists it is finalized first.
        /// Exercises not found in the catalog are auto-created.
        /// </summary>
        /// <param name="dto">
        /// Routine data including a non-empty exercise list.
        /// Each exercise must have positive repetitions and a valid day (1–7).
        /// </param>
        /// <exception cref="Exception">
        /// Thrown for an empty exercise list, zero/negative repetitions, or out-of-range day values.
        /// </exception>
        /// <summary>
        /// Creates a new routine for a client. If an active routine already exists,
        /// it is finalized (its <c>Hasta</c> date is set) before the new one is created.
        /// New exercises are created in the catalog if they don't exist by name.
        /// All operations execute within a single transaction.
        /// </summary>
        /// <param name="dto">Routine data including a non-empty <see cref="RutinaDTO.Ejercicios"/> list.</param>
        /// <exception cref="Exception">Thrown if the exercise list is empty, reps are zero, or day of week is out of range.</exception>
        public void CrearRutina(RutinaDTO dto)
        {
            try
            {
                if (dto.Ejercicios == null || dto.Ejercicios.Count == 0)
                    throw new Exception("ERR_RUTINA_SIN_EJERCICIOS".Translate());

                foreach (var ex in dto.Ejercicios)
                {
                    if (ex.Repeticiones <= 0) throw new Exception(string.Format("ERR_EJERCICIO_REP_ZERO_N".Translate(), ex.Nombre));
                    if (ex.DiaSemana < 1 || ex.DiaSemana > 7) throw new Exception(string.Format("ERR_EJERCICIO_DIA_INVALIDO_N".Translate(), ex.Nombre));
                }

                var rutinaActiva = _rutinaRepository.GetActivaByCliente(dto.ClienteID);

                using (var uow = DalFactory.CreateUnitOfWork())
                {
                    try
                    {
                        uow.BeginTransaction();

                        if (rutinaActiva != null)
                        {
                            uow.RutinaRepository.FinalizarRutina(rutinaActiva.Id);
                        }

                        var nuevaRutina = RutinaMapper.ToEntity(dto);
                        if (nuevaRutina.Id == Guid.Empty) nuevaRutina.Id = Guid.NewGuid();
                        nuevaRutina.Desde = DateTime.Now;
                        nuevaRutina.Hasta = null;

                        uow.RutinaRepository.Add(nuevaRutina);

                        foreach (var exDto in dto.Ejercicios)
                        {
                            Ejercicio ejercicio = _ejercicioRepository.GetByNombre(exDto.Nombre);

                            if (ejercicio == null)
                            {
                                ejercicio = new Ejercicio { Id = Guid.NewGuid(), Nombre = exDto.Nombre };
                                uow.EjercicioRepository.Add(ejercicio);
                            }

                            var rutinaEjercicio = new RutinaEjercicio
                            {
                                RutinaId = nuevaRutina.Id,
                                EjercicioId = ejercicio.Id,
                                Repeticiones = exDto.Repeticiones,
                                DiaSemana = exDto.DiaSemana,
                                Orden = exDto.Orden
                            };

                            uow.RutinaEjercicioRepository.Insertar(rutinaEjercicio);
                        }

                        uow.Commit();
                        _bitacoraService.Log($"Routine created for client {dto.ClienteID}", "INFO");
                    }
                    catch (Exception)
                    {
                        uow.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                _bitacoraService.Log($"Error creating routine: {ex.Message}", "ERROR", ex);
                throw;
            }
        }

        /// <summary>
        /// Replaces the exercise set of an existing routine within a transaction
        /// (clears all existing routine-exercise links, then re-inserts the new set).
        /// </summary>
        /// <param name="rutinaId">The routine to modify.</param>
        /// <param name="ejercicios">Non-empty replacement exercise list.</param>
        /// <summary>
        /// Replaces the exercise set of an existing routine within a single transaction.
        /// All current exercise mappings are deleted and the new list is re-inserted.
        /// </summary>
        /// <param name="rutinaId">The routine to modify.</param>
        /// <param name="ejercicios">New exercise list. Must not be empty.</param>
        public void ModificarRutina(Guid rutinaId, List<EjercicioDTO> ejercicios)
        {
            try
            {
                if (ejercicios == null || ejercicios.Count == 0)
                    throw new Exception("Exercise list cannot be empty.");

                foreach (var ex in ejercicios)
                {
                    if (ex.Repeticiones <= 0) throw new Exception(string.Format("ERR_EJERCICIO_REP_ZERO_N".Translate(), ex.Nombre));
                    if (ex.DiaSemana < 1 || ex.DiaSemana > 7) throw new Exception(string.Format("ERR_EJERCICIO_DIA_INVALIDO_N".Translate(), ex.Nombre));
                }

                using (var uow = DalFactory.CreateUnitOfWork())
                {
                    try
                    {
                        uow.BeginTransaction();

                        uow.RutinaEjercicioRepository.EliminarPorRutina(rutinaId);

                        foreach (var exDto in ejercicios)
                        {
                            Ejercicio ejercicio = _ejercicioRepository.GetByNombre(exDto.Nombre);
                            if (ejercicio == null)
                            {
                                ejercicio = new Ejercicio { Id = Guid.NewGuid(), Nombre = exDto.Nombre };
                                uow.EjercicioRepository.Add(ejercicio);
                            }

                            var rutinaEjercicio = new RutinaEjercicio
                            {
                                RutinaId = rutinaId,
                                EjercicioId = ejercicio.Id,
                                Repeticiones = exDto.Repeticiones,
                                DiaSemana = exDto.DiaSemana,
                                Orden = exDto.Orden
                            };

                            uow.RutinaEjercicioRepository.Insertar(rutinaEjercicio);
                        }

                        uow.Commit();
                        _bitacoraService.Log($"Routine {rutinaId} modified", "INFO");
                    }
                    catch (Exception)
                    {
                        uow.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                _bitacoraService.Log($"Error modifying routine: {ex.Message}", "ERROR", ex);
                throw;
            }
        }

        /// <summary>Permanently deletes a routine and its associated exercise links.</summary>
        /// <param name="rutinaId">The routine to delete.</param>
        /// <summary>Permanently deletes a routine and its exercise mappings.</summary>
        /// <param name="rutinaId">The routine to delete.</param>
        public void BorrarRutina(Guid rutinaId)
        {
            try
            {
                _rutinaRepository.Remove(rutinaId);
                _bitacoraService.Log($"Routine {rutinaId} deleted", "INFO");
            }
            catch (Exception ex)
            {
                _bitacoraService.Log($"Error deleting routine: {ex.Message}", "ERROR", ex);
                throw;
            }
        }

        /// <summary>
        /// Returns the client's currently active routine with its full exercise list hydrated.
        /// </summary>
        /// <param name="clienteId">The client to look up.</param>
        /// <returns>The active <see cref="RutinaDTO"/>, or <c>null</c> if none exists.</returns>
        /// <summary>
        /// Retrieves the active (open-ended) routine for a client, with its exercise list hydrated.
        /// </summary>
        /// <param name="clienteId">The client to look up.</param>
        /// <returns>The active <see cref="RutinaDTO"/> including exercises, or <c>null</c> if none exists.</returns>
        public RutinaDTO ObtenerRutinaActiva(Guid clienteId)
        {
            try
            {
                var rutina = _rutinaRepository.GetActivaByCliente(clienteId);
                if (rutina == null) return null;

                var dto = RutinaMapper.ToDTO(rutina);
                var rutinaEjercicios = _rutinaEjercicioRepository.GetByRutina(rutina.Id);

                dto.Ejercicios = rutinaEjercicios.Select(re => EjercicioMapper.ToDTO(re)).ToList();

                return dto;
            }
            catch (Exception ex)
            {
                _bitacoraService.Log($"Error fetching active routine: {ex.Message}", "ERROR", ex);
                throw;
            }
        }

        /// <summary>
        /// Returns a specific routine by ID with its full exercise list hydrated.
        /// </summary>
        /// <param name="rutinaId">The routine identifier.</param>
        /// <returns>The <see cref="RutinaDTO"/>, or <c>null</c> if not found.</returns>
        /// <summary>Retrieves a specific routine by its identifier, with its exercise list hydrated.</summary>
        /// <param name="rutinaId">The routine identifier.</param>
        /// <returns>The <see cref="RutinaDTO"/> with exercises, or <c>null</c> if not found.</returns>
        public RutinaDTO ObtenerRutina(Guid rutinaId)
        {
            try
            {
                var rutina = _rutinaRepository.GetById(rutinaId);
                if (rutina == null) return null;

                var dto = RutinaMapper.ToDTO(rutina);
                var rutinaEjercicios = _rutinaEjercicioRepository.GetByRutina(rutina.Id);

                dto.Ejercicios = rutinaEjercicios.Select(re => EjercicioMapper.ToDTO(re)).ToList();

                return dto;
            }
            catch (Exception ex)
            {
                _bitacoraService.Log($"Error fetching routine: {ex.Message}", "ERROR", ex);
                throw;
            }
        }

        /// <summary>
        /// Returns all routines, optionally restricted to active ones, with <c>ClienteNombre</c> hydrated.
        /// </summary>
        /// <param name="soloActivas">When <c>true</c>, includes only routines with no end date.</param>
        /// <summary>Returns all routines, optionally filtered to active-only. Each DTO includes the client name.</summary>
        /// <param name="soloActivas">If <c>true</c>, only routines with no <c>Hasta</c> date are returned.</param>
        public List<RutinaDTO> ListarRutinas(bool soloActivas)
        {
            try
            {
                var rutinas = _rutinaRepository.GetAll();

                if (soloActivas)
                {
                    rutinas = rutinas.Where(r => r.Hasta == null).ToList();
                }

                var clientes = _clienteRepository.GetAll().ToDictionary(c => c.Id, c => $"{c.Nombre} {c.Apellido}");

                return rutinas.Select(r =>
                {
                    var dto = RutinaMapper.ToDTO(r);
                    if (clientes.ContainsKey(r.ClienteID))
                    {
                        dto.ClienteNombre = clientes[r.ClienteID];
                    }
                    else
                    {
                        dto.ClienteNombre = "Unknown";
                    }
                    return dto;
                }).ToList();
            }
            catch (Exception ex)
            {
                _bitacoraService.Log($"Error listing routines: {ex.Message}", "ERROR", ex);
                throw;
            }
        }
    }
}
