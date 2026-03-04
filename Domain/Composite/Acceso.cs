using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Composite
{
    /// <summary>
    /// Abstract base component of the Composite design pattern used to model the permission tree.
    /// </summary>
    /// <remarks>
    /// A permission tree is composed of <see cref="Familia"/> interior nodes and <see cref="Patente"/>
    /// leaf nodes. All tree traversal and permission checks operate against the <see cref="Acceso"/> abstraction,
    /// allowing uniform treatment of individual permissions and permission groups.
    /// </remarks>
    public abstract class Acceso
    {
        /// <summary>Unique identifier of this access component.</summary>
        public Guid Id { get; set; }

        /// <summary>Human-readable display name.</summary>
        public string Nombre { get; set; }

        /// <summary>
        /// Returns the direct children of this node.
        /// Leaf nodes (<see cref="Patente"/>) always return an empty list.
        /// </summary>
        public abstract List<Acceso> Accesos { get; }

        /// <summary>
        /// Adds <paramref name="componente"/> as a direct child of this node.
        /// </summary>
        /// <param name="componente">The component to add.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown by leaf nodes (<see cref="Patente"/>) that do not support children.
        /// </exception>
        public abstract void Agregar(Acceso componente);

        /// <summary>
        /// Removes <paramref name="componente"/> from the direct children of this node.
        /// </summary>
        /// <param name="componente">The component to remove, matched by <see cref="Id"/>.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown by leaf nodes (<see cref="Patente"/>) that do not support children.
        /// </exception>
        public abstract void Eliminar(Acceso componente);

        /// <inheritdoc/>
        public override string ToString() => Nombre;
    }
}
