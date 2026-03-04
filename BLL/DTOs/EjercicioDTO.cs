using System;

namespace BLL.DTOs
{
    /// <summary>
    /// Represents a single exercise entry within a <see cref="RutinaDTO"/>.
    /// </summary>
    /// <remarks>
    /// Exercises are scoped to a specific day of the week and ordered within that day via <see cref="Orden"/>.
    /// <see cref="Id"/> is auto-assigned on construction.
    /// </remarks>
    public class EjercicioDTO
    {
        /// <summary>Primary key, auto-assigned on construction.</summary>
        public Guid Id { get; set; }
        /// <summary>FK to the parent routine.</summary>
        public Guid RutinaID { get; set; }
        /// <summary>Exercise name or description.</summary>
        public string Nombre { get; set; }
        /// <summary>Number of repetitions to perform.</summary>
        public int Repeticiones { get; set; }
        /// <summary>Day of the week this exercise is scheduled for (0 = Sunday, 6 = Saturday).</summary>
        public int DiaSemana { get; set; }
        /// <summary>Display order within the day's exercise list (ascending).</summary>
        public int Orden { get; set; }

        public EjercicioDTO()
        {
            Id = Guid.NewGuid();
        }
    }
}
