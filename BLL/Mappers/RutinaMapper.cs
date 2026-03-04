using BLL.DTOs;
using Domain.Entities;
using System.Collections.Generic;

namespace BLL.Mappers
{
    /// <summary>
    /// Bidirectional mapper between <see cref="Rutina"/> domain entities and <see cref="RutinaDTO"/> projections.
    /// </summary>
    /// <remarks>
    /// Exercises are not mapped here; callers must populate <see cref="RutinaDTO.Ejercicios"/>
    /// separately using <see cref="EjercicioMapper"/> after retrieving the routine's exercise records.
    /// </remarks>
    public static class RutinaMapper
    {
        /// <summary>
        /// Projects a <see cref="RutinaDTO"/> to a <see cref="Rutina"/> entity.
        /// </summary>
        /// <param name="dto">Source DTO.</param>
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

        /// <summary>
        /// Projects a <see cref="Rutina"/> entity to a <see cref="RutinaDTO"/>.
        /// </summary>
        /// <param name="entity">Source entity.</param>
        /// <remarks><see cref="RutinaDTO.Ejercicios"/> is initialized to an empty list; populate it separately with <see cref="EjercicioMapper"/>.</remarks>
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
