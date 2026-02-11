using Domain.Composite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTO
{
    /// <summary>
    /// Data Transfer Object (DTO) para representar un usuario.
    /// </summary>
    public class UsuarioDTO
    {
        /// <summary>
        /// Identificador único del usuario.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nombre de usuario.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Estado del usuario.
        /// </summary>
        public string Estado { get; set; }

        /// <summary>
        /// Lista de permisos (Familias/Patentes) asignados al usuario.
        /// </summary>
        public List<Acceso> Permisos { get; set; } = new List<Acceso>();

        /// <summary>
        /// Email del usuario (Operador/Administrador).
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Rol de negocio del usuario (Operador, Administrador).
        /// </summary>
        public string RolNegocio { get; set; }
    }
}
