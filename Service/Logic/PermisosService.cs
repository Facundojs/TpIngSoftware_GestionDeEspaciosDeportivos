using Service.Domain;
using Service.Domain.Composite;
using Service.Impl;
using Service.Impl.SqlServer;
using System;
using System.Collections.Generic;
using System.Linq;

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
            _familiaRepository.Update(familia);
        }

        public void EnsurePermissions()
        {
            // 1. Ensure all Enums exist as Patents
            var existingPatents = _patenteRepository.GetAll();
            foreach (TipoPermiso p in Enum.GetValues(typeof(TipoPermiso)))
            {
                if (!existingPatents.Any(x => x.TipoAcceso == (int)p))
                {
                    var patente = new Patente
                    {
                        Id = Guid.NewGuid(),
                        Nombre = p.ToString(),
                        TipoAcceso = (int)p,
                        DataKey = p.ToString()
                    };
                    _patenteRepository.Add(patente);
                }
            }

            // 2. Ensure "Administrador" family exists
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

                // Add all patents to Admin
                var allPatents = _patenteRepository.GetAll();
                foreach (var pat in allPatents)
                {
                    adminFamily.Agregar(pat);
                }
                _familiaRepository.Update(adminFamily);
            }
            else
            {
                // Update Admin family to have all patents (in case we added new ones)
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
