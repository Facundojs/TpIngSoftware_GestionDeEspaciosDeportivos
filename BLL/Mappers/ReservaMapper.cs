using BLL.DTOs;
using Domain.Entities;
using System;

namespace BLL.Mappers
{
    /// <summary>
    /// Bidirectional mapper between <see cref="Reserva"/> domain entities and <see cref="ReservaDTO"/> projections.
    /// </summary>
    /// <remarks>
    /// <see cref="ReservaDTO.ClienteNombre"/> and <see cref="ReservaDTO.EspacioNombre"/> are denormalized
    /// display fields that cannot be populated from the entity alone. They must be set by the calling
    /// service after mapping.
    /// </remarks>
    public static class ReservaMapper
    {
        /// <summary>
        /// Projects a <see cref="Reserva"/> entity to a <see cref="ReservaDTO"/>.
        /// </summary>
        /// <param name="entity">Source entity. Returns <c>null</c> when <c>null</c>.</param>
        /// <remarks><see cref="ReservaDTO.ClienteNombre"/> and <see cref="ReservaDTO.EspacioNombre"/> are left empty and must be filled by the service.</remarks>
        public static ReservaDTO ToDTO(Reserva entity)
        {
            if (entity == null) return null;

            return new ReservaDTO
            {
                Id = entity.Id,
                ClienteID = entity.ClienteID,
                EspacioID = entity.EspacioID,
                Fecha = entity.Fecha,
                FechaHora = entity.FechaHora,
                Duracion = entity.Duracion,
                Adelanto = entity.Adelanto,
                CodigoReserva = entity.CodigoReserva,
                Estado = entity.Estado
                // ClienteNombre and EspacioNombre need to be filled by Service
            };
        }

        /// <summary>
        /// Projects a <see cref="ReservaDTO"/> back to a <see cref="Reserva"/> entity.
        /// </summary>
        /// <param name="dto">Source DTO. Returns <c>null</c> when <c>null</c>.</param>
        public static Reserva ToEntity(ReservaDTO dto)
        {
            if (dto == null) return null;

            return new Reserva
            {
                Id = dto.Id,
                ClienteID = dto.ClienteID,
                EspacioID = dto.EspacioID,
                Fecha = dto.Fecha,
                FechaHora = dto.FechaHora,
                Duracion = dto.Duracion,
                Adelanto = dto.Adelanto,
                CodigoReserva = dto.CodigoReserva,
                Estado = dto.Estado
            };
        }
    }
}
