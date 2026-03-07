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
    /// Business logic service for membership plan management.
    /// </summary>
    public class MembresiaService
    {
        private readonly IMembresiaRepository _repository;
        private readonly BitacoraService _bitacora;

        /// <summary>Initializes dependencies from <see cref="DAL.Factory.DalFactory"/> singletons.</summary>
        public MembresiaService()
        {
            _repository = DalFactory.MembresiaRepository;
            _bitacora = new BitacoraService();
        }

        /// <summary>
        /// Creates a new membership plan.
        /// </summary>
        /// <param name="dto">Membership data. <see cref="MembresiaDTO.Codigo"/> must be unique.</param>
        /// <exception cref="ArgumentException">Thrown for invalid price, regularidad, or missing name.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the numeric code is already in use.</exception>
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

        /// <summary>
        /// Updates an existing membership plan.
        /// The numeric code may be changed only if it is not already in use by another plan.
        /// </summary>
        /// <param name="dto">Updated membership data. <see cref="MembresiaDTO.Id"/> must identify an existing record.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the membership does not exist or the new code conflicts with another plan.
        /// </exception>
        /// <summary>
        /// Updates an existing membership plan. Code uniqueness is enforced across other plans.
        /// </summary>
        /// <param name="dto">Updated membership data including <see cref="MembresiaDTO.Id"/>.</param>
        /// <exception cref="InvalidOperationException">Thrown if the plan does not exist or the code conflicts with another plan.</exception>
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
                entity.Id = dto.Id;
                _repository.Update(entity);
            }
            catch (Exception ex)
            {
                _bitacora.Log($"Error en CU-ME-001: {ex.Message}", "ERROR", ex);
                throw;
            }
        }

        /// <summary>
        /// Disables a membership plan by setting <c>Activa = false</c>.
        /// Blocked when at least one active client is still subscribed to this plan.
        /// </summary>
        /// <param name="id">The membership to disable.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the membership does not exist or has active subscribers.
        /// </exception>
        /// <summary>
        /// Sets a membership plan's <c>Activa</c> flag to <c>false</c>.
        /// Blocked if any active client is currently subscribed to this plan.
        /// </summary>
        /// <param name="id">The plan to disable.</param>
        /// <exception cref="InvalidOperationException">Thrown if the plan has active subscribers.</exception>
        public void DeshabilitarMembresia(Guid id)
        {
            try
            {
                var entity = _repository.GetById(id);
                if (entity == null) throw new InvalidOperationException("La membresía no existe");

                var clienteRepo = DalFactory.ClienteRepository;
                if (clienteRepo.HasActiveClientsByMembresia(id))
                {
                    throw new InvalidOperationException("ERR_MEMBRESIA_CON_CLIENTES".Translate());
                }

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

        /// <summary>
        /// Returns all membership plans, optionally restricted to active ones.
        /// </summary>
        /// <param name="soloActivas">
        /// When <c>true</c>, returns only plans with <c>Activa = true</c>;
        /// when <c>false</c>, returns all plans including disabled ones.
        /// </param>
        /// <summary>Returns all membership plans, optionally filtered to active-only.</summary>
        /// <param name="soloActivas">If <c>true</c>, returns only plans with <c>Activa = true</c>.</param>
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

        /// <summary>Retrieves a single membership plan by primary key.</summary>
        /// <param name="id">The membership identifier.</param>
        /// <returns>The <see cref="MembresiaDTO"/>, or <c>null</c> if not found.</returns>
        /// <summary>Retrieves a single membership plan by its identifier.</summary>
        /// <param name="id">The plan identifier.</param>
        /// <returns>The matching <see cref="MembresiaDTO"/>, or <c>null</c> if not found.</returns>
        public MembresiaDTO ObtenerMembresia(Guid id)
        {
            var entity = _repository.GetById(id);
            return MembresiaMapper.ToDTO(entity);
        }
    }
}
