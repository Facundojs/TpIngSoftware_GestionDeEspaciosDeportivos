using BLL.DTOs;
using Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace BLL.Mappers
{
    public static class EspacioMapper
    {
        public static EspacioDTO Map(Espacio entity)
        {
            if (entity == null) return null;
            return new EspacioDTO
            {
                Id = entity.Id,
                Nombre = entity.Nombre,
                Descripcion = entity.Descripcion,
                PrecioHora = entity.PrecioHora
            };
        }

        public static Espacio Map(EspacioDTO dto)
        {
            if (dto == null) return null;
            return new Espacio
            {
                Id = dto.Id,
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                PrecioHora = dto.PrecioHora
            };
        }

        public static List<EspacioDTO> Map(List<Espacio> entities)
        {
            if (entities == null) return null;
            return entities.Select(Map).ToList();
        }
    }
}
