using BLL.DTOs;
using Domain.Entities;
using Domain.Enums;
using System;

namespace BLL.Mappers
{
    /// <summary>
    /// Bidirectional mapper between payment and receipt domain entities and their DTO projections.
    /// </summary>
    /// <remarks>
    /// Handles <see cref="Pago"/> ↔ <see cref="PagoDTO"/> and <see cref="Comprobante"/> ↔ <see cref="ComprobanteDTO"/>.
    /// <c>Estado</c> is stored as a string in the database and parsed to <see cref="EstadoPago"/> on read.
    /// When mapping to entity, <see cref="Pago.Id"/> is only set when the DTO's <see cref="PagoDTO.Id"/>
    /// is non-empty, allowing the database to assign the key on insert.
    /// </remarks>
    public static class PagoMapper
    {
        /// <summary>
        /// Projects a <see cref="Pago"/> entity to a <see cref="PagoDTO"/>.
        /// </summary>
        /// <param name="entity">Source entity. Returns <c>null</c> when <c>null</c>.</param>
        public static PagoDTO ToDTO(Pago entity)
        {
            if (entity == null) return null;

            return new PagoDTO
            {
                Id = entity.Id,
                Codigo = entity.Codigo,
                ClienteID = entity.ClienteID,
                Monto = entity.Monto,
                Metodo = entity.Metodo,
                Detalle = entity.Detalle,
                Fecha = entity.Fecha,
                Estado = (EstadoPago)Enum.Parse(typeof(EstadoPago), entity.Estado),
                MembresiaID = entity.MembresiaID,
                ReservaID = entity.ReservaID
            };
        }

        /// <summary>
        /// Projects a <see cref="PagoDTO"/> back to a <see cref="Pago"/> entity.
        /// </summary>
        /// <param name="dto">Source DTO. Returns <c>null</c> when <c>null</c>.</param>
        /// <remarks><see cref="Pago.Id"/> is only assigned when <see cref="PagoDTO.Id"/> is non-empty.</remarks>
        public static Pago ToEntity(PagoDTO dto)
        {
            if (dto == null) return null;

            var entity = new Pago
            {
                Codigo = dto.Codigo,
                ClienteID = dto.ClienteID,
                Monto = dto.Monto,
                Metodo = dto.Metodo,
                Detalle = dto.Detalle,
                Fecha = dto.Fecha,
                Estado = dto.Estado.ToString(),
                MembresiaID = dto.MembresiaID,
                ReservaID = dto.ReservaID
            };

            if (dto.Id != Guid.Empty)
            {
                entity.Id = dto.Id;
            }

            return entity;
        }

        /// <summary>
        /// Projects a <see cref="Comprobante"/> entity to a <see cref="ComprobanteDTO"/>.
        /// </summary>
        /// <param name="entity">Source entity. Returns <c>null</c> when <c>null</c>.</param>
        public static ComprobanteDTO ToDTO(Comprobante entity)
        {
            if (entity == null) return null;

            return new ComprobanteDTO
            {
                Id = entity.Id,
                PagoID = entity.PagoID,
                ReservaID = entity.ReservaID,
                NombreArchivo = entity.NombreArchivo,
                RutaArchivo = entity.RutaArchivo,
                FechaSubida = entity.FechaSubida,
                Contenido = entity.Contenido
            };
        }

        /// <summary>
        /// Projects a <see cref="ComprobanteDTO"/> back to a <see cref="Comprobante"/> entity.
        /// </summary>
        /// <param name="dto">Source DTO. Returns <c>null</c> when <c>null</c>.</param>
        /// <remarks><see cref="Comprobante.Id"/> is only assigned when <see cref="ComprobanteDTO.Id"/> is non-empty.</remarks>
        public static Comprobante ToEntity(ComprobanteDTO dto)
        {
            if (dto == null) return null;

            var entity = new Comprobante
            {
                PagoID = dto.PagoID,
                ReservaID = dto.ReservaID,
                NombreArchivo = dto.NombreArchivo,
                RutaArchivo = dto.RutaArchivo,
                FechaSubida = dto.FechaSubida,
                Contenido = dto.Contenido
            };

            if (dto.Id != Guid.Empty)
            {
                entity.Id = dto.Id;
            }

            return entity;
        }
    }
}
