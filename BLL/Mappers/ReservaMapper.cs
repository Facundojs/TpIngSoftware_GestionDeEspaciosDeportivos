using BLL.DTOs;
using Domain.Entities;
using System;

namespace BLL.Mappers
{
    public static class ReservaMapper
    {
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
