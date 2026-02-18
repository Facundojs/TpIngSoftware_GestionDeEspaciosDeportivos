using BLL.DTOs;
using BLL.Mappers;
using DAL.Contracts;
using DAL.Factory;
using Domain.Entities;
using Service.Logic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BLL.Services
{
    public class EjercicioService
    {
        private readonly IEjercicioRepository _ejercicioRepository;
        private readonly BitacoraService _bitacoraService;

        public EjercicioService()
        {
            _ejercicioRepository = DalFactory.EjercicioRepository;
            _bitacoraService = new BitacoraService();
        }

        public List<EjercicioDTO> ListarEjercicios()
        {
            try
            {
                var ejercicios = _ejercicioRepository.GetAll();
                return ejercicios.Select(e => new EjercicioDTO
                {
                    Id = e.Id,
                    Nombre = e.Nombre
                    // RutinaID, Reps, etc are context specific, but DTO has them.
                    // For pure list, we just map basic info.
                }).ToList();
            }
            catch (Exception ex)
            {
                _bitacoraService.Log($"Error al listar ejercicios: {ex.Message}", "ERROR", ex);
                throw;
            }
        }

        public void CrearEjercicio(EjercicioDTO dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Nombre))
                    throw new Exception("El nombre del ejercicio es requerido.");

                var existing = _ejercicioRepository.GetByNombre(dto.Nombre);
                if (existing != null)
                    throw new Exception("Ya existe un ejercicio con ese nombre.");

                var entity = new Ejercicio
                {
                    Id = Guid.NewGuid(),
                    Nombre = dto.Nombre
                };

                _ejercicioRepository.Add(entity);
                _bitacoraService.Log($"Ejercicio creado: {dto.Nombre}", "INFO");
            }
            catch (Exception ex)
            {
                _bitacoraService.Log($"Error al crear ejercicio: {ex.Message}", "ERROR", ex);
                throw;
            }
        }

        public void ModificarEjercicio(EjercicioDTO dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Nombre))
                    throw new Exception("El nombre del ejercicio es requerido.");

                var entity = _ejercicioRepository.GetById(dto.Id);
                if (entity == null) throw new Exception("El ejercicio no existe.");

                entity.Nombre = dto.Nombre;
                _ejercicioRepository.Update(entity);
                _bitacoraService.Log($"Ejercicio modificado: {dto.Id}", "INFO");
            }
            catch (Exception ex)
            {
                _bitacoraService.Log($"Error al modificar ejercicio: {ex.Message}", "ERROR", ex);
                throw;
            }
        }

        public void EliminarEjercicio(Guid id)
        {
            try
            {
                // Check dependencies? Usually cascading or prevent if used.
                // Assuming simple delete for now or database constraints handle it.
                _ejercicioRepository.Remove(id);
                _bitacoraService.Log($"Ejercicio eliminado: {id}", "INFO");
            }
            catch (Exception ex)
            {
                _bitacoraService.Log($"Error al eliminar ejercicio: {ex.Message}", "ERROR", ex);
                throw;
            }
        }
    }
}
