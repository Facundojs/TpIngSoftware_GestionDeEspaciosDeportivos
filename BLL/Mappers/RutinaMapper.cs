using BLL.DTOs;
using Domain.Entities;
using System.Collections.Generic;

namespace BLL.Mappers
{
    public static class RutinaMapper
    {
        public static Rutina ToEntity(RutinaDTO dto)
        {
            return new Rutina
            {
                Id = dto.Id,
                ClienteID = dto.ClienteID,
                Desde = dto.Desde,
                Hasta = dto.Hasta,
                Detalle = dto.Detalle
            };
        }

        public static RutinaDTO ToDTO(Rutina entity)
        {
            return new RutinaDTO
            {
                Id = entity.Id,
                ClienteID = entity.ClienteID,
                Desde = entity.Desde,
                Hasta = entity.Hasta,
                Detalle = entity.Detalle,
                Ejercicios = new List<EjercicioDTO>()
            };
        }
    }
}
