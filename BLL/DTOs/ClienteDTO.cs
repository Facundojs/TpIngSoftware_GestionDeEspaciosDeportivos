using System;

namespace BLL.DTOs
{
    /// <summary>
    /// Projection of a <c>Cliente</c> entity used across all BLL service boundaries.
    /// </summary>
    /// <remarks>
    /// Combines core identity fields with hydrated membership details and a pre-computed balance
    /// so that consumers (UI managers, API controllers) never need to issue additional queries.
    /// <see cref="Status"/> defaults to <see cref="ClienteStatus.Activo"/> on construction.
    /// </remarks>
    public class ClienteDTO
    {
        /// <summary>Primary key of the client record.</summary>
        public Guid Id { get; set; }
        /// <summary>Client's first name.</summary>
        public string Nombre { get; set; }
        /// <summary>Client's last name.</summary>
        public string Apellido { get; set; }
        /// <summary>Argentine national identity document number.</summary>
        public int DNI { get; set; }
        /// <summary>Date of birth.</summary>
        public DateTime FechaNacimiento { get; set; }
        /// <summary>Contact email address.</summary>
        public string Email { get; set; }
        /// <summary>Timestamp of record creation.</summary>
        public DateTime CreatedAt { get; set; }
        /// <summary>FK to the client's currently assigned membership, or <c>null</c> if none.</summary>
        public Guid? MembresiaID { get; set; }
        /// <summary>
        /// Textual reason for the current <see cref="Status"/>.
        /// Populated when status is <see cref="ClienteStatus.Inactivo"/>.
        /// </summary>
        public string Razon { get; set; }
        /// <summary>Active/inactive status. Defaults to <see cref="ClienteStatus.Activo"/>.</summary>
        public ClienteStatus Status { get; set; }
        /// <summary>Next scheduled membership billing date, or <c>null</c> when no active membership row exists.</summary>
        public DateTime? ProximaFechaPago { get; set; }
        /// <summary>Concatenation of <see cref="Nombre"/> and <see cref="Apellido"/>.</summary>
        public string NombreCompleto => $"{Nombre} {Apellido}";
        /// <summary>Hydrated membership details. Populated by <c>ClienteService.ObtenerCliente</c> when membership exists.</summary>
        public MembresiaDTO MembresiaDetalle { get; set; }
        /// <summary>Current account balance in currency units (positive = credit, negative = debt).</summary>
        public decimal Balance { get; set; }
        /// <summary>Categorized balance state derived from <see cref="Balance"/>.</summary>
        public EstadoBalance EstadoBalance { get; set; }

        public ClienteDTO()
        {
            Status = ClienteStatus.Activo;
        }
    }
}
