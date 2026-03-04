using Domain;
using Domain.Composite;
using Domain.Enums;
using Service.Facade.Extension;
using Service.Impl;
using Service.Impl.SqlServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Service.Logic
{
    /// <summary>
    /// Manages the permission tree: creation and modification of <see cref="Familia"/> groups,
    /// and provisioning of <see cref="Patente"/> leaf nodes from <see cref="PermisoKeys"/>.
    /// </summary>
    public class PermisosService
    {
        private readonly FamiliaRepository _familiaRepository;
        private readonly PatenteRepository _patenteRepository;

        /// <summary>Initializes a new instance with default SQL repositories.</summary>
        public PermisosService()
        {
            _familiaRepository = new FamiliaRepository();
            _patenteRepository = new PatenteRepository();
        }

        /// <summary>
        /// Returns all <see cref="Familia"/> records, each fully hydrated via <c>GetById</c>
        /// (recursive children loaded).
        /// </summary>
        /// <returns>List of fully-loaded families.</returns>
        public List<Familia> GetAllFamilias()
        {
            var familias = _familiaRepository.GetAll();
            var result = new List<Familia>();
            foreach (var f in familias)
            {
                var full = _familiaRepository.GetById(f.Id);
                if (full != null) result.Add(full);
            }
            return result;
        }

        /// <summary>Returns all <see cref="Patente"/> records (flat list, no hydration needed).</summary>
        public List<Patente> GetAllPatentes()
        {
            return _patenteRepository.GetAll();
        }

        /// <summary>
        /// Persists changes to a <see cref="Familia"/> after validating that the update
        /// would not introduce a cycle in the permission tree.
        /// </summary>
        /// <param name="familia">The family to save. Must have been loaded via <c>GetById</c> (full recursive load).</param>
        /// <exception cref="InvalidOperationException">Thrown when a circular reference is detected.</exception>
        public void GuardarFamilia(Familia familia)
        {
            DetectarCiclo(familia);
            _familiaRepository.Update(familia);
        }

        // Requires that each Familia in the tree was loaded via GetById (full recursive load),
        // not GetAll() which only returns shallow shells without children.
        private void DetectarCiclo(Familia familia)
        {
            var visited = new HashSet<Guid>();
            CheckDescendants(familia.Id, familia.Accesos.OfType<Familia>(), visited);
        }

        private void CheckDescendants(Guid origin, IEnumerable<Familia> children, HashSet<Guid> visited)
        {
            foreach (var child in children)
            {
                if (child.Id == origin)
                    throw new InvalidOperationException(Translations.ERR_FAMILIA_CICLO.Translate());
                if (!visited.Add(child.Id)) continue;
                CheckDescendants(origin, child.Accesos.OfType<Familia>(), visited);
            }
        }

        /// <summary>Persists a new <see cref="Familia"/>.</summary>
        /// <param name="familia">The family to create.</param>
        public void CrearFamilia(Familia familia)
        {
            _familiaRepository.Add(familia);
        }

        /// <summary>Deletes a <see cref="Familia"/> by its identifier.</summary>
        /// <param name="id">The unique identifier of the family to delete.</param>
        public void EliminarFamilia(Guid id)
        {
            _familiaRepository.Remove(id);
        }

        /// <summary>
        /// Ensures all constants declared in <see cref="PermisoKeys"/> exist as <see cref="Patente"/>
        /// records in the database, and that the <c>"Administrador"</c> family holds all of them.
        /// </summary>
        /// <remarks>
        /// Intended to run on application startup. Uses reflection to enumerate
        /// <see cref="PermisoKeys"/> string constants and inserts any that are missing.
        /// The <c>Administrador</c> family is created if it does not exist.
        /// </remarks>
        public void EnsurePermissions()
        {
            var existingPatents = _patenteRepository.GetAll();

            var permissionFields = typeof(Domain.Composite.PermisoKeys).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(string));

            foreach (var field in permissionFields)
            {
                string permisoValue = (string)field.GetValue(null);
                string permisoName = field.Name;

                if (!existingPatents.Any(x => x.TipoAcceso == permisoValue))
                {
                    var patente = new Patente
                    {
                        Id = Guid.NewGuid(),
                        Nombre = permisoName,
                        TipoAcceso = permisoValue,
                        DataKey = permisoValue
                    };
                    _patenteRepository.Add(patente);
                }
            }

            var families = _familiaRepository.GetAll();
            var adminFamily = families.FirstOrDefault(f => f.Nombre == "Administrador");

            if (adminFamily == null)
            {
                adminFamily = new Familia
                {
                    Id = Guid.NewGuid(),
                    Nombre = "Administrador"
                };
                _familiaRepository.Add(adminFamily);

                var allPatents = _patenteRepository.GetAll();
                foreach (var pat in allPatents)
                {
                    adminFamily.Agregar(pat);
                }
                _familiaRepository.Update(adminFamily);
            }
            else
            {
                var fullAdmin = _familiaRepository.GetById(adminFamily.Id);
                var allPatents = _patenteRepository.GetAll();
                bool changed = false;
                foreach (var pat in allPatents)
                {
                    if (!fullAdmin.Accesos.Any(x => x.Id == pat.Id))
                    {
                        fullAdmin.Agregar(pat);
                        changed = true;
                    }
                }
                if (changed) _familiaRepository.Update(fullAdmin);
            }
        }
    }
}
