using System;

namespace BLL.DTOs
{
    /// <summary>
    /// Command object carrying all inputs required to create a new reservation via <c>ReservaService.GenerarReserva</c>.
    /// </summary>
    /// <remarks>
    /// <see cref="Duracion"/> must be a positive integer representing minutes.
    /// <see cref="Adelanto"/> is the partial payment collected at booking time; it may be zero but must not be negative.
    /// Availability is validated against the space's agenda and existing reservations inside the service.
    /// </remarks>
    public class GenerarReservaDTO
    {
        /// <summary>FK to the client making the reservation.</summary>
        public Guid ClienteId { get; set; }
        /// <summary>FK to the space being reserved.</summary>
        public Guid EspacioId { get; set; }
        /// <summary>Exact start date and time of the requested reservation.</summary>
        public DateTime FechaHora { get; set; }
        /// <summary>Requested duration in minutes.</summary>
        public int Duracion { get; set; }
        /// <summary>Advance payment amount to register at the time of booking.</summary>
        public decimal Adelanto { get; set; }
    }
}
