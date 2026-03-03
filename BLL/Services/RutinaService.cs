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
    public class RutinaService
    {
        private readonly IRutinaRepository _rutinaRepository;
        private readonly IEjercicioRepository _ejercicioRepository;
        private readonly IRutinaEjercicioRepository _rutinaEjercicioRepository;
        private readonly IClienteRepository _clienteRepository;
        private readonly BitacoraService _bitacoraService;

        public RutinaService()
        {
            _rutinaRepository = DalFactory.RutinaRepository;
            _ejercicioRepository = DalFactory.EjercicioRepository;
            _rutinaEjercicioRepository = DalFactory.RutinaEjercicioRepository;
            _clienteRepository = DalFactory.ClienteRepository;
            _bitacoraService = new BitacoraService();
        }

        public void CrearRutina(RutinaDTO dto)
        {
            try
            {
                if (dto.Ejercicios == null || dto.Ejercicios.Count == 0)
                    throw new Exception(Domain.Enums.Translations.ERR_RUTINA_SIN_EJERCICIOS.Translate());

                foreach (var ex in dto.Ejercicios)
                {
                    if (ex.Repeticiones <= 0) throw new Exception(string.Format(Domain.Enums.Translations.ERR_EJERCICIO_REP_ZERO_N.Translate(), ex.Nombre));
                    if (ex.DiaSemana < 1 || ex.DiaSemana > 7) throw new Exception(string.Format(Domain.Enums.Translations.ERR_EJERCICIO_DIA_INVALIDO_N.Translate(), ex.Nombre));
                }

                var rutinaActiva = _rutinaRepository.GetActivaByCliente(dto.ClienteID);

                using (var conn = new SqlConnection(ConnectionManager.GetBusinessConnectionString()))
                {
                    conn.Open();
                    using (var tran = conn.BeginTransaction())
                    {
                        try
                        {
                            if (rutinaActiva != null)
                            {
                                _rutinaRepository.FinalizarRutina(rutinaActiva.Id, conn, tran);
                            }

                            var nuevaRutina = RutinaMapper.ToEntity(dto);
                            if (nuevaRutina.Id == Guid.Empty) nuevaRutina.Id = Guid.NewGuid();
                            nuevaRutina.Desde = DateTime.Now;
                            nuevaRutina.Hasta = null;

                            _rutinaRepository.Add(nuevaRutina, conn, tran);

                            foreach (var exDto in dto.Ejercicios)
                            {
                                Ejercicio ejercicio = _ejercicioRepository.GetByNombre(exDto.Nombre);

                                if (ejercicio == null)
                                {
                                    ejercicio = new Ejercicio { Id = Guid.NewGuid(), Nombre = exDto.Nombre };
                                    _ejercicioRepository.Add(ejercicio, conn, tran);
                                }

                                var rutinaEjercicio = new RutinaEjercicio
                                {
                                    RutinaId = nuevaRutina.Id,
                                    EjercicioId = ejercicio.Id,
                                    Repeticiones = exDto.Repeticiones,
                                    DiaSemana = exDto.DiaSemana,
                                    Orden = exDto.Orden
                                };

                                _rutinaEjercicioRepository.Insertar(rutinaEjercicio, conn, tran);
                            }

                            tran.Commit();
                            _bitacoraService.Log($"Routine created for client {dto.ClienteID}", "INFO");
                        }
                        catch (Exception)
                        {
                            tran.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _bitacoraService.Log($"Error creating routine: {ex.Message}", "ERROR", ex);
                throw;
            }
        }

        public void ModificarRutina(Guid rutinaId, List<EjercicioDTO> ejercicios)
        {
            try
            {
                if (ejercicios == null || ejercicios.Count == 0)
                    throw new Exception("Exercise list cannot be empty.");

                foreach (var ex in ejercicios)
                {
                    if (ex.Repeticiones <= 0) throw new Exception(string.Format(Domain.Enums.Translations.ERR_EJERCICIO_REP_ZERO_N.Translate(), ex.Nombre));
                    if (ex.DiaSemana < 1 || ex.DiaSemana > 7) throw new Exception(string.Format(Domain.Enums.Translations.ERR_EJERCICIO_DIA_INVALIDO_N.Translate(), ex.Nombre));
                }

                using (var conn = new SqlConnection(ConnectionManager.GetBusinessConnectionString()))
                {
                    conn.Open();
                    using (var tran = conn.BeginTransaction())
                    {
                        try
                        {
                            _rutinaEjercicioRepository.EliminarPorRutina(rutinaId, conn, tran);

                            foreach (var exDto in ejercicios)
                            {
                                Ejercicio ejercicio = _ejercicioRepository.GetByNombre(exDto.Nombre);
                                if (ejercicio == null)
                                {
                                    ejercicio = new Ejercicio { Id = Guid.NewGuid(), Nombre = exDto.Nombre };
                                    _ejercicioRepository.Add(ejercicio, conn, tran);
                                }

                                var rutinaEjercicio = new RutinaEjercicio
                                {
                                    RutinaId = rutinaId,
                                    EjercicioId = ejercicio.Id,
                                    Repeticiones = exDto.Repeticiones,
                                    DiaSemana = exDto.DiaSemana,
                                    Orden = exDto.Orden
                                };

                                _rutinaEjercicioRepository.Insertar(rutinaEjercicio, conn, tran);
                            }

                            tran.Commit();
                            _bitacoraService.Log($"Routine {rutinaId} modified", "INFO");
                        }
                        catch (Exception)
                        {
                            tran.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _bitacoraService.Log($"Error modifying routine: {ex.Message}", "ERROR", ex);
                throw;
            }
        }

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
