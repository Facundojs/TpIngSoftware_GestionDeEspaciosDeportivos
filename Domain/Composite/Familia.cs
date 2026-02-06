using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Composite
{
    public class Familia : Acceso
    {
        private readonly List<Acceso> _hijos = new List<Acceso>();

        public override List<Acceso> Accesos => _hijos;

        public override void Agregar(Acceso componente)
        {
            if (!_hijos.Any(x => x.Id == componente.Id))
            {
                _hijos.Add(componente);
            }
        }

        public override void Eliminar(Acceso componente)
        {
            var item = _hijos.FirstOrDefault(x => x.Id == componente.Id);
            if (item != null) _hijos.Remove(item);
        }
    }
}
