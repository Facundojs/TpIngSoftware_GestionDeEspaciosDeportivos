using System;

namespace BLL.DTOs
{
    /// <summary>
    /// Result returned by <c>ClienteService.ValidarIngreso</c> describing whether a client is
    /// permitted to enter the facility and the reason when access is denied.
    /// </summary>
    /// <remarks>
    /// When <see cref="Permitido"/> is <c>false</c>, <see cref="Razon"/> contains a localized
    /// message suitable for display (e.g., client inactive, outstanding debt).
    /// <see cref="Balance"/> is always populated so the UI can show the client's account state
    /// regardless of the access decision.
    /// </remarks>
    public class ResultadoIngresoDTO
    {
        /// <summary><c>true</c> if the client is allowed entry and a check-in record was created.</summary>
        public bool Permitido { get; set; }
        /// <summary>Localized denial reason, or <c>null</c> when <see cref="Permitido"/> is <c>true</c>.</summary>
        public string Razon { get; set; }
        /// <summary>Full name of the client (<c>"Nombre Apellido"</c>).</summary>
        public string NombreCliente { get; set; }
        /// <summary>Current account balance in currency units at the time of the validation.</summary>
        public decimal Balance { get; set; }
    }
}
