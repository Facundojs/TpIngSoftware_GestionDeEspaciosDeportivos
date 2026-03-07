using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTO
{
    /// <summary>
    /// Data Transfer Object representing an authenticated or listed application user.
    /// </summary>
    /// <remarks>
    /// Carries the user's permission tree (<see cref="Permisos"/>) from the service layer to the UI.
    /// UI components use <see cref="Service.Helpers.PermisoHelper.TienePermiso"/> on this DTO to
    /// drive button visibility and access control without hitting the database again.
    /// </remarks>
    public class UsuarioDTO
    {
        /// <summary>Unique identifier of the user.</summary>
        public Guid Id { get; set; }

        /// <summary>Login name.</summary>
        public string Username { get; set; }

        /// <summary>Account status string (e.g., <c>"Activo"</c>, <c>"Inactivo"</c>).</summary>
        public string Estado { get; set; }

        /// <summary>
        /// Permission tree roots assigned to this user (mix of <see cref="Familia"/> and <see cref="Patente"/> nodes).
        /// </summary>
        public List<Acceso> Permisos { get; set; } = new List<Acceso>();

        /// <summary>
        /// Business role label derived from the user's family assignments (e.g., <c>"Administrador"</c>, <c>"Operador"</c>).
        /// </summary>
        public string RolNegocio { get; set; }
    }
}
