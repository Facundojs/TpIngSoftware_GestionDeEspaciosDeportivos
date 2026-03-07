using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    /// <summary>
    /// Domain entity representing an application user.
    /// </summary>
    /// <remarks>
    /// <see cref="Password"/> is stored as a SHA-256 hex digest
    /// (see <see cref="Service.Helpers.CryptographyHelper.HashPassword"/>).
    /// <see cref="DigitoVerificador"/> is a SHA-256 integrity verifier computed over
    /// <c>Id + NombreUsuario + Password + Estado</c> to detect out-of-band record tampering.
    /// </remarks>
    public class Usuario
    { 
        /// <summary>Unique identifier, auto-generated on construction.</summary>
        public Guid Id { get; set; }

        /// <summary>Unique login name.</summary>
        public string NombreUsuario { get; set; }

        /// <summary>SHA-256 hex digest of the user's password.</summary>
        public string Password { get; set; }

        /// <summary><c>true</c> if the account is active; <c>false</c> if disabled.</summary>
        public bool Estado { get; set; }

        /// <summary>
        /// Integrity verifier computed over core user fields.
        /// Recalculated on every update via <c>UsuarioService.UpdateDV()</c>.
        /// </summary>
        public string DigitoVerificador { get; set; }

        /// <summary>
        /// Permission tree roots assigned to this user (mix of <see cref="Familia"/> and <see cref="Patente"/> nodes).
        /// </summary>
        public List<Acceso> Permisos { get; set; } = new List<Acceso>();

        /// <summary>Initializes a new instance with a freshly generated <see cref="Id"/>.</summary>
        public Usuario()
        {
            Id = Guid.NewGuid();
        }
    }
}
