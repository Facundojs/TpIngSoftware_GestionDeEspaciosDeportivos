using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Domain.Composite
{
    public abstract class Acceso
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }

        public abstract List<Acceso> Accesos { get; }

        public abstract void Agregar(Acceso componente);
        public abstract void Eliminar(Acceso componente);

        public override string ToString() => Nombre;
    }
}
