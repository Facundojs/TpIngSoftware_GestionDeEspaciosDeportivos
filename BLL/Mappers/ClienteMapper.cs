using BLL.DTOs;
using Domain.Entities;
using System;

namespace BLL.Mappers
{
    /// <summary>
    /// Bidirectional mapper between <see cref="Cliente"/> domain entities and <see cref="ClienteDTO"/> projections.
    /// </summary>
    /// <remarks>
    /// <see cref="ToDTO"/> accepts an optional <c>Balance</c> and <c>proximaFechaPago</c> so the caller
    /// can hydrate the DTO in a single pass without additional round-trips. When <paramref name="balance"/>
    /// is <c>null</c>, <see cref="ClienteDTO.Balance"/> defaults to zero and
    /// <see cref="ClienteDTO.EstadoBalance"/> is set to <see cref="EstadoBalance.AlDia"/>.
    /// Unrecognized <c>Estado</c> string values are coerced to <see cref="ClienteStatus.Activo"/>
    /// and logged as warnings.
    /// </remarks>
    public static class ClienteMapper
    {
        /// <summary>
        /// Projects a <see cref="Cliente"/> entity to a <see cref="ClienteDTO"/>.
        /// </summary>
        /// <param name="entity">Source entity. Returns <c>null</c> when <c>null</c>.</param>
        /// <param name="membresia">Membership entity to embed as <see cref="ClienteDTO.MembresiaDetalle"/>, or <c>null</c>.</param>
        /// <param name="balance">Current balance record used to populate <see cref="ClienteDTO.Balance"/> and <see cref="ClienteDTO.EstadoBalance"/>.</param>
        /// <param name="proximaFechaPago">Next billing date from the active <c>ClienteMembresia</c> row, or <c>null</c>.</param>
        public static ClienteDTO ToDTO(Cliente entity, Membresia membresia, Balance balance, DateTime? proximaFechaPago = null)
        {
            if (entity == null) return null;

            ClienteStatus status;
            if (!Enum.TryParse(entity.Estado, out status))
            {
                status = ClienteStatus.Activo;
                new Service.Logic.BitacoraService().Log($"Valor de estado no reconocido: '{entity.Estado}' para el cliente ID: {entity.Id}. Se asume 'Activo'.", "WARNING");
            }

            var dto = new ClienteDTO
            {
                Id = entity.Id,
                Nombre = entity.Nombre,
                Apellido = entity.Apellido,
                DNI = entity.DNI,
                FechaNacimiento = entity.FechaNacimiento,
                Email = entity.Email,
                CreatedAt = entity.CreatedAt,
                MembresiaID = entity.MembresiaID,
                Razon = entity.Razon,
                Status = status,
                ProximaFechaPago = proximaFechaPago,
                Balance = balance != null ? balance.Saldo : 0,
                MembresiaDetalle = membresia != null ? MembresiaMapper.ToDTO(membresia) : null
            };

            if (dto.Balance < 0)
            {
                dto.EstadoBalance = EstadoBalance.Deudor;
            }
            else if (dto.Balance > 0)
            {
                dto.EstadoBalance = EstadoBalance.AFavor;
            }
            else
            {
                dto.EstadoBalance = EstadoBalance.AlDia;
            }

            return dto;
        }

        /// <summary>
        /// Projects a <see cref="ClienteDTO"/> back to a <see cref="Cliente"/> entity.
        /// </summary>
        /// <param name="dto">Source DTO. Returns <c>null</c> when <c>null</c>.</param>
        /// <remarks>Computed and hydrated properties (<see cref="ClienteDTO.Balance"/>, <see cref="ClienteDTO.MembresiaDetalle"/>, etc.) are not mapped.</remarks>
        public static Cliente ToEntity(ClienteDTO dto)
        {
            if (dto == null) return null;

            return new Cliente
            {
                Id = dto.Id,
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                DNI = dto.DNI,
                FechaNacimiento = dto.FechaNacimiento,
                Email = dto.Email,
                CreatedAt = dto.CreatedAt,
                MembresiaID = dto.MembresiaID,
                Estado = dto.Status.ToString(),
                Razon = dto.Razon
            };
        }
    }
}
