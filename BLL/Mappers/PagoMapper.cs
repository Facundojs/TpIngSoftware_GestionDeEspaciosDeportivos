using BLL.DTOs;
using Domain.Entities;
using System;

namespace BLL.Mappers
{
    public static class PagoMapper
    {
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
                Estado = entity.Estado,
                MembresiaID = entity.MembresiaID,
                ReservaID = entity.ReservaID
            };
        }

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
                Estado = dto.Estado,
                MembresiaID = dto.MembresiaID,
                ReservaID = dto.ReservaID
            };

            if (dto.Id != Guid.Empty)
            {
                entity.Id = dto.Id;
            }

            return entity;
        }

        public static ComprobanteDTO ToDTO(Comprobante entity)
        {
            if (entity == null) return null;

            return new ComprobanteDTO
            {
                Id = entity.Id,
                PagoID = entity.PagoID,
                NombreArchivo = entity.NombreArchivo,
                RutaArchivo = entity.RutaArchivo,
                FechaSubida = entity.FechaSubida,
                Contenido = entity.Contenido
            };
        }

        public static Comprobante ToEntity(ComprobanteDTO dto)
        {
            if (dto == null) return null;

            var entity = new Comprobante
            {
                PagoID = dto.PagoID,
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
