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
    public class EspacioService
    {
        private readonly IEspacioRepository _espacioRepository;
        private readonly BitacoraService _bitacora;

        public EspacioService()
        {
            _espacioRepository = DalFactory.EspacioRepository;
            _bitacora = new BitacoraService();
        }

        public void CrearEspacio(EspacioDTO dto)
        {
            try
            {
                if (dto == null) throw new ArgumentNullException(nameof(dto));
                if (dto.PrecioHora < 0) throw new ArgumentException("El precio por hora no puede ser negativo.");
                if (string.IsNullOrWhiteSpace(dto.Nombre)) throw new ArgumentException("El nombre es requerido.");

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

        public void ActualizarEspacio(EspacioDTO dto)
        {
            try
            {
                if (dto == null) throw new ArgumentNullException(nameof(dto));
                if (dto.PrecioHora < 0) throw new ArgumentException("El precio por hora no puede ser negativo.");
                if (string.IsNullOrWhiteSpace(dto.Nombre)) throw new ArgumentException("El nombre es requerido.");

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
                        entity.Estado = "Inactivo";
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

        public List<EspacioDTO> ListarEspacios()
        {
            var entities = _espacioRepository.GetAll();
            return EspacioMapper.Map(entities);
        }
    }
}
