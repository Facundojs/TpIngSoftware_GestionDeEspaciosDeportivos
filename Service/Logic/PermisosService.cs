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
    public class PermisosService
    {
        private readonly FamiliaRepository _familiaRepository;
        private readonly PatenteRepository _patenteRepository;

        public PermisosService()
        {
            _familiaRepository = new FamiliaRepository();
            _patenteRepository = new PatenteRepository();
        }

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

        public List<Patente> GetAllPatentes()
        {
            return _patenteRepository.GetAll();
        }

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

        public void CrearFamilia(Familia familia)
        {
            _familiaRepository.Add(familia);
        }

        public void EliminarFamilia(Guid id)
        {
            _familiaRepository.Remove(id);
        }

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
