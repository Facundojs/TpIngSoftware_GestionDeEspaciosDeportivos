using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    /// <summary>
    /// Composite (interior) node in the permission tree that groups zero or more <see cref="Acceso"/>
    /// children, which may themselves be <see cref="Familia"/> instances or <see cref="Patente"/> leaves.
    /// </summary>
    /// <remarks>
    /// Duplicate children (same <see cref="Acceso.Id"/>) are silently ignored on <see cref="Agregar"/>.
    /// </remarks>
    public class Familia : Acceso
    {
        private readonly List<Acceso> _hijos = new List<Acceso>();

        /// <summary>The direct children of this family node.</summary>
        public override List<Acceso> Accesos => _hijos;

        /// <summary>
        /// Adds <paramref name="componente"/> to this family's child list.
        /// If a child with the same <see cref="Acceso.Id"/> already exists, the call is a no-op.
        /// </summary>
        /// <param name="componente">The component to add.</param>
        public override void Agregar(Acceso componente)
        {
            if (!_hijos.Any(x => x.Id == componente.Id))
            {
                _hijos.Add(componente);
            }
        }

        /// <summary>
        /// Removes the child whose <see cref="Acceso.Id"/> matches <paramref name="componente"/>.
        /// If no such child exists the call is a no-op.
        /// </summary>
        /// <param name="componente">The component to remove.</param>
        public override void Eliminar(Acceso componente)
        {
            var item = _hijos.FirstOrDefault(x => x.Id == componente.Id);
            if (item != null) _hijos.Remove(item);
        }

        /// <summary>Removes all children from this family.</summary>
        public void ClearAccesos()
        {
            _hijos.Clear();
        }
    }
}
