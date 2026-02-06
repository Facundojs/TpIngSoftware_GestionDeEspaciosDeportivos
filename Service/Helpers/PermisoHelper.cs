using Domain;
using Domain.Composite;
using Service.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Helpers
{
    public static class PermisoHelper
    {
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
