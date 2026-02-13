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
    public class MembresiaService
    {
        private readonly IMembresiaRepository _repository;
        private readonly BitacoraService _bitacora;

        public MembresiaService()
        {
            _repository = DalFactory.MembresiaRepository;
            _bitacora = new BitacoraService();
        }

        public void CrearMembresia(MembresiaDTO dto)
        {
            try
            {
                if (dto.Precio <= 0) throw new ArgumentException("Precio debe ser mayor a cero");
                if (dto.Regularidad <= 0) throw new ArgumentException("Regularidad debe ser mayor a cero");
                if (string.IsNullOrWhiteSpace(dto.Nombre)) throw new ArgumentException("Nombre es requerido");

                var existing = _repository.GetByCodigo(dto.Codigo);
                if (existing != null)
                {
                    throw new InvalidOperationException($"Ya existe membresía con código {dto.Codigo}");
                }

                var entity = MembresiaMapper.ToEntity(dto);
                if (entity.Id == Guid.Empty) entity.Id = Guid.NewGuid();

                _repository.Add(entity);

                _bitacora.Log($"CU-ME-003: Membresía '{dto.Nombre}' creada con código {dto.Codigo} y precio ${dto.Precio}", "INFO");
            }
            catch (Exception ex)
            {
                _bitacora.Log($"Error en CU-ME-003: {ex.Message}", "ERROR", ex);
                throw;
            }
        }

        public void ActualizarMembresia(MembresiaDTO dto)
        {
            try
            {
                if (dto.Precio <= 0) throw new ArgumentException("Precio debe ser mayor a cero");
                if (dto.Regularidad <= 0) throw new ArgumentException("Regularidad debe ser mayor a cero");
                if (string.IsNullOrWhiteSpace(dto.Nombre)) throw new ArgumentException("Nombre es requerido");

                var existing = _repository.GetById(dto.Id);
                if (existing == null) throw new InvalidOperationException("La membresía no existe");

                var codeCheck = _repository.GetByCodigo(dto.Codigo);
                if (codeCheck != null && codeCheck.Id != dto.Id)
                {
                    throw new InvalidOperationException($"Ya existe membresía con código {dto.Codigo}");
                }

                var entity = MembresiaMapper.ToEntity(dto);
                // Ensure we are updating the correct ID
                entity.Id = dto.Id;
                _repository.Update(entity);
            }
            catch (Exception ex)
            {
                _bitacora.Log($"Error en CU-ME-001: {ex.Message}", "ERROR", ex);
                throw;
            }
        }

        public void DeshabilitarMembresia(Guid id)
        {
            try
            {
                var entity = _repository.GetById(id);
                if (entity == null) throw new InvalidOperationException("La membresía no existe");

                entity.Activa = false;
                _repository.Update(entity);

                _bitacora.Log($"CU-ME-004: Membresía '{entity.Nombre}' deshabilitada", "INFO");
            }
            catch (Exception ex)
            {
                _bitacora.Log($"Error en CU-ME-004: {ex.Message}", "ERROR", ex);
                throw;
            }
        }

        public List<MembresiaDTO> ListarMembresias(bool soloActivas)
        {
            List<Membresia> list;
            if (soloActivas)
            {
                list = _repository.ListarActivas();
            }
            else
            {
                list = _repository.GetAll();
            }

            return list.Select(m => MembresiaMapper.ToDTO(m)).ToList();
        }

        public MembresiaDTO ObtenerMembresia(Guid id)
        {
            var entity = _repository.GetById(id);
            return MembresiaMapper.ToDTO(entity);
        }
    }
}
