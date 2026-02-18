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
        private readonly BitacoraService _bitacoraService;

        public RutinaService()
        {
            _rutinaRepository = DalFactory.RutinaRepository;
            _ejercicioRepository = DalFactory.EjercicioRepository;
            _bitacoraService = new BitacoraService();
        }

        public void CrearRutina(RutinaDTO dto)
        {
            try
            {
                if (dto.Ejercicios == null || dto.Ejercicios.Count == 0)
                    throw new Exception("La rutina debe tener al menos un ejercicio.");

                foreach (var ex in dto.Ejercicios)
                {
                    if (ex.Repeticiones <= 0) throw new Exception($"El ejercicio {ex.Nombre} debe tener repeticiones mayor a 0.");
                    if (ex.DiaSemana < 1 || ex.DiaSemana > 7) throw new Exception($"El ejercicio {ex.Nombre} tiene un día de semana inválido.");
                }

                // Obtener rutina activa antes de la transacción (para saber si finalizarla)
                // Nota: Podría haber una condición de carrera si dos usuarios crean rutina al mismo tiempo,
                // pero asumimos un solo operador por cliente en este contexto.
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
                            // Asegurar ID nuevo y fechas correctas
                            if (nuevaRutina.Id == Guid.Empty) nuevaRutina.Id = Guid.NewGuid();
                            nuevaRutina.Desde = DateTime.Now;
                            nuevaRutina.Hasta = null;

                            _rutinaRepository.Add(nuevaRutina, conn, tran);

                            foreach (var exDto in dto.Ejercicios)
                            {
                                var ejercicio = EjercicioMapper.ToEntity(exDto);
                                ejercicio.RutinaID = nuevaRutina.Id;
                                if (ejercicio.Id == Guid.Empty) ejercicio.Id = Guid.NewGuid();
                                _ejercicioRepository.Insertar(ejercicio, conn, tran);
                            }

                            tran.Commit();
                            _bitacoraService.Log($"Rutina creada para cliente {dto.ClienteID}", "INFO");
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
                _bitacoraService.Log($"Error al crear rutina: {ex.Message}", "ERROR", ex);
                throw;
            }
        }

        public void ModificarRutina(Guid rutinaId, List<EjercicioDTO> ejercicios)
        {
            try
            {
                if (ejercicios == null || ejercicios.Count == 0)
                    throw new Exception("La lista de ejercicios no puede estar vacía.");

                foreach (var ex in ejercicios)
                {
                    if (ex.Repeticiones <= 0) throw new Exception($"El ejercicio {ex.Nombre} debe tener repeticiones mayor a 0.");
                    if (ex.DiaSemana < 1 || ex.DiaSemana > 7) throw new Exception($"El ejercicio {ex.Nombre} tiene un día de semana inválido.");
                }

                using (var conn = new SqlConnection(ConnectionManager.GetBusinessConnectionString()))
                {
                    conn.Open();
                    using (var tran = conn.BeginTransaction())
                    {
                        try
                        {
                            // Eliminar ejercicios anteriores
                            _ejercicioRepository.EliminarPorRutina(rutinaId, conn, tran);

                            // Insertar nuevos
                            foreach (var exDto in ejercicios)
                            {
                                var ejercicio = EjercicioMapper.ToEntity(exDto);
                                ejercicio.RutinaID = rutinaId; // Asegurar FK
                                if (ejercicio.Id == Guid.Empty) ejercicio.Id = Guid.NewGuid();
                                _ejercicioRepository.Insertar(ejercicio, conn, tran);
                            }

                            tran.Commit();
                            _bitacoraService.Log($"Rutina {rutinaId} modificada", "INFO");
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
                _bitacoraService.Log($"Error al modificar rutina: {ex.Message}", "ERROR", ex);
                throw;
            }
        }

        public void BorrarRutina(Guid rutinaId)
        {
            try
            {
                _rutinaRepository.Remove(rutinaId);
                _bitacoraService.Log($"Rutina {rutinaId} eliminada", "INFO");
            }
            catch (Exception ex)
            {
                _bitacoraService.Log($"Error al borrar rutina: {ex.Message}", "ERROR", ex);
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
                var ejercicios = _ejercicioRepository.GetByRutina(rutina.Id);

                dto.Ejercicios = ejercicios.Select(e => EjercicioMapper.ToDTO(e)).ToList();

                return dto;
            }
            catch (Exception ex)
            {
                _bitacoraService.Log($"Error al obtener rutina activa: {ex.Message}", "ERROR", ex);
                throw;
            }
        }
    }
}
