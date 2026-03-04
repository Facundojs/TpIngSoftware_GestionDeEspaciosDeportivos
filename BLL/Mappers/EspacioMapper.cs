using BLL.DTOs;
using Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace BLL.Mappers
{
    /// <summary>
    /// Bidirectional mapper between <see cref="Espacio"/> domain entities and <see cref="EspacioDTO"/> projections.
    /// </summary>
    /// <remarks>
    /// All three overloads of <see cref="Map"/> are named identically to leverage C# method overload
    /// resolution, keeping call sites uniform regardless of direction or collection vs. single-item mapping.
    /// </remarks>
    public static class EspacioMapper
    {
        /// <summary>
        /// Projects a <see cref="Espacio"/> entity to an <see cref="EspacioDTO"/>.
        /// </summary>
        /// <param name="entity">Source entity. Returns <c>null</c> when <c>null</c>.</param>
        public static EspacioDTO Map(Espacio entity)
        {
            if (entity == null) return null;
            return new EspacioDTO
            {
                Id = entity.Id,
                Nombre = entity.Nombre,
                Descripcion = entity.Descripcion,
                PrecioHora = entity.PrecioHora,
                Estado = entity.Estado
            };
        }

        /// <summary>
        /// Projects an <see cref="EspacioDTO"/> back to a <see cref="Espacio"/> entity.
        /// </summary>
        /// <param name="dto">Source DTO. Returns <c>null</c> when <c>null</c>.</param>
        public static Espacio Map(EspacioDTO dto)
        {
            if (dto == null) return null;
            return new Espacio
            {
                Id = dto.Id,
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                PrecioHora = dto.PrecioHora,
                Estado = dto.Estado
            };
        }

        /// <summary>
        /// Projects a list of <see cref="Espacio"/> entities to a list of <see cref="EspacioDTO"/> objects.
        /// </summary>
        /// <param name="entities">Source list. Returns <c>null</c> when <c>null</c>.</param>
        public static List<EspacioDTO> Map(List<Espacio> entities)
        {
            if (entities == null) return null;
            return entities.Select(Map).ToList();
        }
    }
}
