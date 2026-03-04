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
    /// Business logic service for sports space (espacio) management.
    /// </summary>
    /// <remarks>
    /// Deletion is soft when the space has future non-cancelled/non-finalized reservations:
    /// the space is set to <c>Inactivo</c> instead of being physically removed.
    /// </remarks>
    public class EspacioService
    {
        private readonly IEspacioRepository _espacioRepository;
        private readonly BitacoraService _bitacora;

        /// <summary>Initializes dependencies from <see cref="DAL.Factory.DalFactory"/> singletons.</summary>
        public EspacioService()
        {
            _espacioRepository = DalFactory.EspacioRepository;
            _bitacora = new BitacoraService();
        }

        /// <summary>
        /// Creates a new sports space.
        /// </summary>
        /// <param name="dto">Space data. Name must be non-empty and price non-negative.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="dto"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown for negative price or empty name.</exception>
        /// <summary>Creates a new sports space.</summary>
        /// <param name="dto">Space data. <see cref="EspacioDTO.PrecioHora"/> must be non-negative and <see cref="EspacioDTO.Nombre"/> must be non-empty.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="dto"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown if price is negative or name is empty.</exception>
        public void CrearEspacio(EspacioDTO dto)
        {
            try
            {
                if (dto == null) throw new ArgumentNullException(nameof(dto));
                if (dto.PrecioHora < 0) throw new ArgumentException(Translations.ERR_PRECIO_NEGATIVO.Translate());
                if (string.IsNullOrWhiteSpace(dto.Nombre)) throw new ArgumentException(Translations.ERR_NOMBRE_REQUERIDO.Translate());

                var entity = EspacioMapper.Map(dto);
                if (entity.Id == Guid.Empty) entity.Id = Guid.NewGuid();

                _espacioRepository.Add(entity);
                _bitacora.Log($"Espacio '{entity.Nombre}' creado.", "INFO");
            }
            catch (Exception ex)
            {
                _bitacora.Log($"Error creando espacio: {ex.Message}", "ERROR");
                throw;
            }
        }

        /// <summary>
        /// Updates an existing sports space.
        /// </summary>
        /// <param name="dto">Updated space data. <see cref="EspacioDTO.Id"/> must identify an existing record.</param>
        /// <exception cref="ArgumentException">Thrown for negative price or empty name.</exception>
        /// <summary>Updates an existing sports space.</summary>
        /// <param name="dto">Updated data including the space <see cref="EspacioDTO.Id"/>.</param>
        public void ActualizarEspacio(EspacioDTO dto)
        {
            try
            {
                if (dto == null) throw new ArgumentNullException(nameof(dto));
                if (dto.PrecioHora < 0) throw new ArgumentException(Translations.ERR_PRECIO_NEGATIVO.Translate());
                if (string.IsNullOrWhiteSpace(dto.Nombre)) throw new ArgumentException(Translations.ERR_NOMBRE_REQUERIDO.Translate());

                var entity = EspacioMapper.Map(dto);
                _espacioRepository.Update(entity);
                _bitacora.Log($"Espacio '{entity.Nombre}' actualizado.", "INFO");
            }
            catch (Exception ex)
            {
                _bitacora.Log($"Error actualizando espacio: {ex.Message}", "ERROR");
                throw;
            }
        }

        /// <summary>
        /// Deletes or soft-deletes a space.
        /// If the space has future active reservations it is marked <c>Inactivo</c>; otherwise it is physically removed.
        /// </summary>
        /// <param name="id">The space to remove.</param>
        /// <summary>
        /// Deletes a space, or marks it as <c>Inactivo</c> if it has future non-cancelled reservations.
        /// </summary>
        /// <param name="id">The space to delete.</param>
        public void EliminarEspacio(Guid id)
        {
            try
            {
                var reservasFuturas = DalFactory.ReservaRepository.GetByEspacio(id, DateTime.Now, DateTime.MaxValue);
                if (reservasFuturas.Any(r => r.Estado != "Cancelada" && r.Estado != "Finalizada"))
                {
                    var entity = _espacioRepository.GetById(id);
                    if (entity != null)
                    {
                        entity.Estado = EstadoEspacio.Inactivo;
                        _espacioRepository.Update(entity);
                        _bitacora.Log($"Espacio {id} marcado como inactivo (soft-delete).", "INFO");
                    }
                }
                else
                {
                    _espacioRepository.Remove(id);
                    _bitacora.Log($"Espacio {id} eliminado.", "INFO");
                }
            }
            catch (Exception ex)
            {
                _bitacora.Log($"Error eliminando espacio: {ex.Message}", "ERROR");
                throw;
            }
        }

        /// <summary>Returns only active spaces (suitable for reservation booking).</summary>
        /// <summary>Returns only active spaces (status = <c>Activo</c>).</summary>
        public List<EspacioDTO> ListarEspacios()
        {
            var entities = _espacioRepository.ListarDisponibles();
            return EspacioMapper.Map(entities);
        }

        /// <summary>Returns all spaces regardless of status (for administrative views).</summary>
        /// <summary>Returns all spaces regardless of status.</summary>
        public List<EspacioDTO> ListarTodos()
        {
            var entities = _espacioRepository.GetAll();
            return EspacioMapper.Map(entities);
        }
    }
}
