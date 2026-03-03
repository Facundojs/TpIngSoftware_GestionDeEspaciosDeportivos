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
            if (dto.Precio <= 0) throw new ArgumentException(Domain.Enums.Translations.ERR_PRECIO_MAYOR_CERO.Translate());
            if (dto.Regularidad <= 0) throw new ArgumentException(Domain.Enums.Translations.ERR_REGULARIDAD_MAYOR_CERO.Translate());
            if (string.IsNullOrWhiteSpace(dto.Nombre)) throw new ArgumentException(Domain.Enums.Translations.ERR_NOMBRE_REQUERIDO.Translate());

            var existing = _repository.GetByCodigo(dto.Codigo);
            if (existing != null) throw new InvalidOperationException(Domain.Enums.Translations.ERR_MEMBRESIA_EXISTE.Translate());

            var entity = MembresiaMapper.ToEntity(dto);
            if (entity.Id == Guid.Empty) entity.Id = Guid.NewGuid();
            entity.Activa = true;

            _repository.Add(entity);
            _bitacora.Log($"CU-ME-003: Membership {entity.Nombre} (Code: {entity.Codigo}) created", "INFO");
        }

        public void ActualizarMembresia(MembresiaDTO dto)
        {
            if (dto.Precio <= 0) throw new ArgumentException(Domain.Enums.Translations.ERR_PRECIO_MAYOR_CERO.Translate());
            if (dto.Regularidad <= 0) throw new ArgumentException(Domain.Enums.Translations.ERR_REGULARIDAD_MAYOR_CERO.Translate());
            if (string.IsNullOrWhiteSpace(dto.Nombre)) throw new ArgumentException(Domain.Enums.Translations.ERR_NOMBRE_REQUERIDO.Translate());

            var existing = _repository.GetById(dto.Id);
            if (existing == null) throw new InvalidOperationException(Domain.Enums.Translations.ERR_MEMBRESIA_NO_EXISTE.Translate());

            var entity = MembresiaMapper.ToEntity(dto);
            _repository.Update(entity);
            _bitacora.Log($"CU-ME-001: Membership {entity.Nombre} (Code: {entity.Codigo}) updated", "INFO");
        }

        public void DeshabilitarMembresia(Guid id)
        {
            var entity = _repository.GetById(id);
            if (entity == null) throw new InvalidOperationException(Domain.Enums.Translations.ERR_MEMBRESIA_NO_EXISTE.Translate());

            bool hasClients = DalFactory.ClienteRepository.HasActiveClientsByMembresia(id);
            if (hasClients)
            {
                throw new InvalidOperationException("ERR_MEMBRESIA_CON_CLIENTES");
            }

            entity.Activa = false;
            _repository.Update(entity);
            _bitacora.Log($"CU-ME-004: Membership {entity.Nombre} disabled", "INFO");
        }

        public List<MembresiaDTO> ListarMembresias(bool soloActivas = false)
        {
            var entities = soloActivas ? _repository.ListarActivas() : _repository.GetAll();
            return entities.Select(m => MembresiaMapper.ToDTO(m)).ToList();
        }

        public MembresiaDTO ObtenerMembresia(Guid id)
        {
            var entity = _repository.GetById(id);
            return entity == null ? null : MembresiaMapper.ToDTO(entity);
        }
    }
}
