using System;

namespace BLL.DTOs
{
    /// <summary>
    /// Projection of a <c>Reserva</c> entity with denormalized client and space names.
    /// </summary>
    /// <remarks>
    /// <see cref="ClienteNombre"/> and <see cref="EspacioNombre"/> are hydrated by
    /// <c>ReservaService.ListarReservas</c> to avoid extra lookups in the UI layer.
    /// <see cref="Estado"/> mirrors <c>EstadoReserva</c> as a string so the UI can display
    /// it without a reference to the Domain enum assembly.
    /// </remarks>
    public class ReservaDTO
    {
        /// <summary>Primary key of the reservation.</summary>
        public Guid Id { get; set; }
        /// <summary>FK to the client who made the reservation.</summary>
        public Guid ClienteID { get; set; }
        /// <summary>Full name of the client, hydrated from the repository.</summary>
        public string ClienteNombre { get; set; }
        /// <summary>FK to the reserved space.</summary>
        public Guid EspacioID { get; set; }
        /// <summary>Display name of the space, hydrated from the repository.</summary>
        public string EspacioNombre { get; set; }
        /// <summary>Date portion of the reservation (date only, no time component).</summary>
        public DateTime Fecha { get; set; }
        /// <summary>Exact start date and time of the reservation.</summary>
        public DateTime FechaHora { get; set; }
        /// <summary>Duration of the reservation in minutes.</summary>
        public int Duracion { get; set; }
        /// <summary>Advance payment amount collected at booking time.</summary>
        public decimal Adelanto { get; set; }
        /// <summary>Human-readable reservation code (e.g., <c>RES-000123</c>).</summary>
        public string CodigoReserva { get; set; }
        /// <summary>String representation of <c>EstadoReserva</c> (e.g., <c>"Pendiente"</c>, <c>"Pagada"</c>, <c>"Cancelada"</c>).</summary>
        public string Estado { get; set; }
    }
}
