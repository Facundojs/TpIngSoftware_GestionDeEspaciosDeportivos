using Domain;
using Service.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Helpers
{
    /// <summary>
    /// Extension methods for permission checks on <see cref="UsuarioDTO"/>.
    /// </summary>
    public static class PermisoHelper
    {
        /// <summary>
        /// Determines whether <paramref name="user"/> holds the specified permission by
        /// recursively traversing their <see cref="Acceso"/> composite tree.
        /// </summary>
        /// <param name="user">The user whose permission tree will be searched.</param>
        /// <param name="permiso">
        /// The permission key to look for, matching a <see cref="Patente.TipoAcceso"/> value
        /// (see <see cref="PermisoKeys"/> for available constants).
        /// </param>
        /// <returns>
        /// <c>true</c> if any <see cref="Patente"/> leaf in the tree has
        /// <see cref="Patente.TipoAcceso"/> equal to <paramref name="permiso"/>; otherwise <c>false</c>.
        /// </returns>
        public static bool TienePermiso(this UsuarioDTO user, string permiso)
        {
            if (user == null || user.Permisos == null) return false;

            foreach (var acceso in user.Permisos)
            {
                if (Check(acceso, permiso)) return true;
            }

            return false;
        }

        private static bool Check(Acceso acceso, string permiso)
        {
            if (acceso is Patente patente)
            {
                return patente.TipoAcceso == permiso;
            }

            if (acceso is Familia familia)
            {
                foreach (var hijo in familia.Accesos)
                {
                    if (Check(hijo, permiso)) return true;
                }
            }

            return false;
        }
    }
}
