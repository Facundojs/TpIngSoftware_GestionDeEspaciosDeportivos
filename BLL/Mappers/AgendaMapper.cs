using BLL.DTOs;
using Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace BLL.Mappers
{
    /// <summary>
    /// Bidirectional mapper between <see cref="Agenda"/> domain entities and <see cref="AgendaDTO"/> projections.
    /// </summary>
    /// <remarks>
    /// Provides both single-item and collection overloads via <see cref="Map"/> to support
    /// the delete-and-reinsert bulk pattern used by <c>AgendaService.ConfigurarAgenda</c>.
    /// </remarks>
    public static class AgendaMapper
    {
        /// <summary>
        /// Projects an <see cref="Agenda"/> entity to an <see cref="AgendaDTO"/>.
        /// </summary>
        /// <param name="entity">Source entity. Returns <c>null</c> when <c>null</c>.</param>
        public static AgendaDTO ToDTO(Agenda entity)
        {
            if (entity == null) return null;

            return new AgendaDTO
            {
                EspacioID = entity.EspacioID,
                DiaSemana = entity.DiaSemana,
                HoraDesde = entity.HoraDesde,
                HoraHasta = entity.HoraHasta
            };
        }

        /// <summary>
        /// Projects an <see cref="AgendaDTO"/> back to an <see cref="Agenda"/> entity.
        /// </summary>
        /// <param name="dto">Source DTO. Returns <c>null</c> when <c>null</c>.</param>
        public static Agenda ToEntity(AgendaDTO dto)
        {
            if (dto == null) return null;

            return new Agenda
            {
                EspacioID = dto.EspacioID,
                DiaSemana = dto.DiaSemana,
                HoraDesde = dto.HoraDesde,
                HoraHasta = dto.HoraHasta
            };
        }

        /// <summary>Projects a collection of <see cref="Agenda"/> entities to DTOs. Returns an empty list when <paramref name="entities"/> is <c>null</c>.</summary>
        public static List<AgendaDTO> Map(IEnumerable<Agenda> entities)
        {
            if (entities == null) return new List<AgendaDTO>();
            return entities.Select(ToDTO).ToList();
        }

        /// <summary>Projects a collection of <see cref="AgendaDTO"/> objects to entities. Returns an empty list when <paramref name="dtos"/> is <c>null</c>.</summary>
        public static List<Agenda> Map(IEnumerable<AgendaDTO> dtos)
        {
            if (dtos == null) return new List<Agenda>();
            return dtos.Select(ToEntity).ToList();
        }
    }
}
