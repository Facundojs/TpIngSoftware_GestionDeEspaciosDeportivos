using Domain;
using Service.Facade.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    /// <summary>
    /// Leaf node in the permission tree representing a single, atomic permission.
    /// </summary>
    /// <remarks>
    /// <see cref="Patente"/> instances cannot have children; calling <see cref="Agregar"/> or
    /// <see cref="Eliminar"/> throws <see cref="InvalidOperationException"/>.
    /// Permission checks compare <see cref="TipoAcceso"/> against <see cref="PermisoKeys"/> constants
    /// via <see cref="Service.Helpers.PermisoHelper.TienePermiso"/>.
    /// </remarks>
    public class Patente : Acceso
    {
        /// <summary>
        /// Permission key matched against <see cref="PermisoKeys"/> constants during authorization checks.
        /// </summary>
        public string TipoAcceso { get; set; }

        /// <summary>
        /// Auxiliary data key used for UI display or additional filtering. Typically equals <see cref="TipoAcceso"/>.
        /// </summary>
        public string DataKey { get; set; }

        /// <summary>Always returns an empty list; leaf nodes have no children.</summary>
        public override List<Acceso> Accesos => new List<Acceso>();

        /// <inheritdoc/>
        /// <exception cref="InvalidOperationException">Always thrown — leaf nodes do not support children.</exception>
        public override void Agregar(Acceso componente)
        {
            throw new InvalidOperationException("ERR_PATENTE_ADD_CHILD".Translate());
            }

            /// <inheritdoc/>
            /// <exception cref="InvalidOperationException">Always thrown — leaf nodes do not support children.</exception>
            public override void Eliminar(Acceso acceso)
            {
            throw new InvalidOperationException("ERR_PATENTE_REMOVE_CHILD".Translate());
        }
    }
}
