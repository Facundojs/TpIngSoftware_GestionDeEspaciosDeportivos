using BLL.DTOs;
using Domain.Entities;

namespace BLL.Mappers
{
    /// <summary>
    /// Bidirectional mapper between exercise domain entities and <see cref="EjercicioDTO"/> projections.
    /// </summary>
    /// <remarks>
    /// The domain model separates the exercise definition (<see cref="Ejercicio"/>) from its
    /// scheduling within a routine (<see cref="RutinaEjercicio"/>). <see cref="ToDTO"/> reads from
    /// <see cref="RutinaEjercicio"/> (which contains repetitions, day, and order) and resolves the
    /// exercise name via the <c>Ejercicio</c> navigation property. <see cref="ToEntity"/> maps only to
    /// the base <see cref="Ejercicio"/> definition (name/id); scheduling fields are persisted separately.
    /// </remarks>
    public static class EjercicioMapper
    {
        /// <summary>
        /// Projects an <see cref="EjercicioDTO"/> to an <see cref="Ejercicio"/> definition entity.
        /// </summary>
        /// <param name="dto">Source DTO.</param>
        public static Ejercicio ToEntity(EjercicioDTO dto)
        {
            return new Ejercicio
            {
                Id = dto.Id,
                Nombre = dto.Nombre
            };
        }

        /// <summary>
        /// Projects a <see cref="RutinaEjercicio"/> scheduling record to an <see cref="EjercicioDTO"/>.
        /// </summary>
        /// <param name="entity">Source scheduling record with <c>Ejercicio</c> navigation property loaded.</param>
        /// <remarks>
        /// When <c>entity.Ejercicio</c> is <c>null</c> (navigation property not loaded),
        /// <see cref="EjercicioDTO.Nombre"/> defaults to <c>"Desconocido"</c>.
        /// </remarks>
        public static EjercicioDTO ToDTO(RutinaEjercicio entity)
        {
            return new EjercicioDTO
            {
                Id = entity.EjercicioId, // Using the Exercise ID
                RutinaID = entity.RutinaId,
                Nombre = entity.Ejercicio != null ? entity.Ejercicio.Nombre : "Desconocido",
                Repeticiones = entity.Repeticiones,
                DiaSemana = entity.DiaSemana,
                Orden = entity.Orden
            };
        }
    }
}
