using System;
using System.Collections.Generic;

namespace BLL.DTOs
{
    /// <summary>
    /// Projection of a <c>Rutina</c> (training routine) entity with its associated exercises.
    /// </summary>
    /// <remarks>
    /// An active routine is identified by a <c>null</c> <see cref="Hasta"/> value.
    /// Only one routine per client may be active at a time; creating a new one via
    /// <c>RutinaService.CrearRutina</c> automatically finalizes the previous active routine.
    /// <see cref="Ejercicios"/> is initialized to an empty list on construction.
    /// </remarks>
    public class RutinaDTO
    {
        /// <summary>Primary key, auto-assigned on construction.</summary>
        public Guid Id { get; set; }
        /// <summary>FK to the client this routine belongs to.</summary>
        public Guid ClienteID { get; set; }
        /// <summary>Full name of the client, hydrated by the service layer.</summary>
        public string ClienteNombre { get; set; }
        /// <summary>Date the routine became active.</summary>
        public DateTime Desde { get; set; }
        /// <summary>Date the routine was finalized, or <c>null</c> if still active.</summary>
        public DateTime? Hasta { get; set; }
        /// <summary>Optional notes or description for the routine.</summary>
        public string Detalle { get; set; }
        /// <summary>Ordered list of exercises belonging to this routine.</summary>
        public List<EjercicioDTO> Ejercicios { get; set; }

        public RutinaDTO()
        {
            Id = Guid.NewGuid();
            Ejercicios = new List<EjercicioDTO>();
        }
    }
}
