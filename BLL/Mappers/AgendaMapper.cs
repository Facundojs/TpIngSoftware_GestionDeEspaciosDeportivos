using BLL.DTOs;
using Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace BLL.Mappers
{
    public static class AgendaMapper
    {
        public static AgendaDTO ToDTO(Agenda entity)
        {
            if (entity == null) return null;

            return new AgendaDTO
            {
                EspacioID = entity.EspacioID,
                HoraDesde = entity.HoraDesde,
                HoraHasta = entity.HoraHasta
            };
        }

        public static Agenda ToEntity(AgendaDTO dto)
        {
            if (dto == null) return null;

            return new Agenda
            {
                EspacioID = dto.EspacioID,
                HoraDesde = dto.HoraDesde,
                HoraHasta = dto.HoraHasta
            };
        }

        public static List<AgendaDTO> Map(IEnumerable<Agenda> entities)
        {
            if (entities == null) return new List<AgendaDTO>();
            return entities.Select(ToDTO).ToList();
        }

        public static List<Agenda> Map(IEnumerable<AgendaDTO> dtos)
        {
            if (dtos == null) return new List<Agenda>();
            return dtos.Select(ToEntity).ToList();
        }
    }
}
