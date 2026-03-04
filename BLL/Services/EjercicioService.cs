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
    /// <summary>
    /// Business logic service for managing the exercise catalog.
    /// </summary>
    /// <remarks>
    /// Exercises are shared across all routines (<see cref="RutinaDTO.Ejercicios"/>).
    /// CRUD operations are performed directly on the repository without a Unit of Work,
    /// as exercise management does not involve cross-entity transactional boundaries.
    /// </remarks>
    public class EjercicioService
    {
        private readonly IEjercicioRepository _ejercicioRepository;
        private readonly BitacoraService _bitacoraService;

        /// <summary>Initializes dependencies from <see cref="DalFactory"/> singletons.</summary>
        public EjercicioService()
        {
            _ejercicioRepository = DalFactory.EjercicioRepository;
            _bitacoraService = new BitacoraService();
        }

        /// <summary>Returns all exercises in the catalog.</summary>
        public List<EjercicioDTO> ListarEjercicios()
        {
            try
            {
                var ejercicios = _ejercicioRepository.GetAll();
                return ejercicios.Select(e => new EjercicioDTO
                {
                    Id = e.Id,
                    Nombre = e.Nombre
                }).ToList();
            }
            catch (Exception ex)
            {
                _bitacoraService.Log($"Error al listar ejercicios: {ex.Message}", "ERROR", ex);
                throw;
            }
        }

        /// <summary>
        /// Creates a new exercise in the catalog.
        /// </summary>
        /// <param name="dto">Exercise data. <see cref="EjercicioDTO.Nombre"/> is required and must be unique.</param>
        /// <exception cref="Exception">Thrown when the name is empty or already exists in the catalog.</exception>
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

        /// <summary>
        /// Updates the name of an existing exercise.
        /// </summary>
        /// <param name="dto">Exercise data with the updated <see cref="EjercicioDTO.Nombre"/>. <see cref="EjercicioDTO.Id"/> must identify an existing record.</param>
        /// <exception cref="Exception">Thrown when the name is empty or the exercise does not exist.</exception>
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

        /// <summary>
        /// Permanently deletes an exercise from the catalog.
        /// </summary>
        /// <param name="id">Primary key of the exercise to delete.</param>
        public void EliminarEjercicio(Guid id)
        {
            try
            {
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
