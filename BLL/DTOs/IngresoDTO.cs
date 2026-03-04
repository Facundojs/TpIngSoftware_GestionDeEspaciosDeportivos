using System;

namespace BLL.DTOs
{
    /// <summary>
    /// Projection of a facility check-in (<c>Ingreso</c>) record with a hydrated client name.
    /// </summary>
    /// <remarks>
    /// Check-in records are written by <c>ClienteService.ValidarIngreso</c> and are read-only
    /// through <c>IngresoService</c>. <see cref="ClienteNombre"/> is hydrated at query time;
    /// it is set to <c>"Desconocido"</c> when the client record cannot be resolved.
    /// </remarks>
    public class IngresoDTO
    {
        /// <summary>Primary key of the check-in record.</summary>
        public Guid Id { get; set; }
        /// <summary>FK to the client who checked in.</summary>
        public Guid ClienteID { get; set; }
        /// <summary>Full name of the client (<c>"Nombre Apellido"</c>), hydrated from the client repository.</summary>
        public string ClienteNombre { get; set; }
        /// <summary>Timestamp when the check-in occurred.</summary>
        public DateTime FechaHora { get; set; }
    }
}
