using System;
using Domain.Enums;

namespace BLL.DTOs
{
    /// <summary>
    /// Projection of a <c>Espacio</c> (sporting space/facility) entity exposed through BLL service boundaries.
    /// </summary>
    public class EspacioDTO
    {
        /// <summary>Primary key of the facility space.</summary>
        public Guid Id { get; set; }
        /// <summary>Display name of the space (e.g., <c>"Cancha 1"</c>).</summary>
        public string Nombre { get; set; }
        /// <summary>Optional description or notes about the space.</summary>
        public string Descripcion { get; set; }
        /// <summary>Billing rate per hour in currency units. Used to compute reservation totals.</summary>
        public decimal PrecioHora { get; set; }
        /// <summary>Operational status of the space. Only <c>Disponible</c> spaces accept new reservations.</summary>
        public EstadoEspacio Estado { get; set; }
    }
}
