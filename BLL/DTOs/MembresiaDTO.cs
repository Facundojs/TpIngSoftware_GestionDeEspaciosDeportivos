using System;

namespace BLL.DTOs
{
    /// <summary>
    /// Projection of a <c>Membresia</c> entity exposed through BLL service boundaries.
    /// </summary>
    /// <remarks>
    /// <see cref="Regularidad"/> expresses the billing cycle in days (e.g., 30 for monthly).
    /// <see cref="PrecioFormateado"/> is a display-only computed property; it is not persisted.
    /// </remarks>
    public class MembresiaDTO
    {
        /// <summary>Primary key of the membership plan.</summary>
        public Guid Id { get; set; }
        /// <summary>Auto-incremented sequential code assigned by the database.</summary>
        public int Codigo { get; set; }
        /// <summary>Display name of the membership plan.</summary>
        public string Nombre { get; set; }
        /// <summary>Billing amount per cycle in currency units.</summary>
        public decimal Precio { get; set; }
        /// <summary>Billing cycle length in days (e.g., 30 = monthly, 7 = weekly).</summary>
        public int Regularidad { get; set; }
        /// <summary><c>true</c> if the plan is available for assignment to clients.</summary>
        public bool Activa { get; set; }
        /// <summary>Optional description or notes about the membership plan.</summary>
        public string Detalle { get; set; }
        /// <summary>Currency-formatted representation of <see cref="Precio"/> (e.g., <c>"$1,500.00"</c>).</summary>
        public string PrecioFormateado => $"${Precio:N2}";
    }
}
