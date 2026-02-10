using Domain.Composite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public abstract class Usuario
    {
        public Guid Id { get; set; }
        public string NombreUsuario { get; set; }
        public string Password { get; set; }
        public bool Estado { get; set; }
        public string DigitoVerificador { get; set; }

        // Lista de componentes (pueden ser Familias o Patentes)
        public List<Acceso> Permisos { get; set; } = new List<Acceso>();

        protected Usuario()
        {
            Id = Guid.NewGuid();
        }
    }
}
