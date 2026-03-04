using Domain.Enums;
using System;

namespace BLL.DTOs
{
    /// <summary>
    /// Projection of a <c>Pago</c> entity used as both input and output for payment operations.
    /// </summary>
    /// <remarks>
    /// When used as input to <c>PagoService.RegistrarPago</c>, set <see cref="ClienteID"/>,
    /// <see cref="Monto"/>, <see cref="Metodo"/>, and optionally <see cref="ReservaID"/> or
    /// <see cref="MembresiaID"/> to indicate the payment type.
    /// <see cref="Estado"/> is overwritten by the service; callers should not set it.
    /// <see cref="Id"/> is initialized to a new <see cref="Guid"/> on construction.
    /// </remarks>
    public class PagoDTO
    {
        /// <summary>Primary key, auto-assigned on construction.</summary>
        public Guid Id { get; set; }
        /// <summary>Auto-incremented sequential code assigned by the database.</summary>
        public int Codigo { get; set; }
        /// <summary>FK to the client who made the payment.</summary>
        public Guid ClienteID { get; set; }
        /// <summary>Payment amount in currency units. Must be positive.</summary>
        public decimal Monto { get; set; }
        /// <summary>Payment method label (e.g., <c>"Efectivo"</c>, <c>"Tarjeta"</c>).</summary>
        public string Metodo { get; set; }
        /// <summary>Optional free-text detail or reference number for the payment.</summary>
        public string Detalle { get; set; }
        /// <summary>Timestamp of the payment. Defaults to <c>DateTime.Now</c> when not set.</summary>
        public DateTime Fecha { get; set; }
        /// <summary>Payment lifecycle status. Set by the service layer, not by the caller.</summary>
        public EstadoPago Estado { get; set; }
        /// <summary>FK to the membership being paid, or <c>null</c> for non-membership payments.</summary>
        public Guid? MembresiaID { get; set; }
        /// <summary>FK to the reservation being settled, or <c>null</c> for non-reservation payments.</summary>
        public Guid? ReservaID { get; set; }
        /// <summary><c>true</c> when <see cref="MembresiaID"/> has a value; determines movement type inside <c>PagoService</c>.</summary>
        public bool EsMembresia => MembresiaID.HasValue;

        public PagoDTO()
        {
            Id = Guid.NewGuid();
        }
    }
}
