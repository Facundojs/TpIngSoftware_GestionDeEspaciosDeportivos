using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Composite
{
    public class Patente : Acceso
    {
        public string TipoAcceso { get; set; }
        public string DataKey { get; set; }

        // Una patente no tiene hijos, devolvemos lista vacía
        public override List<Acceso> Accesos => new List<Acceso>();

        public override void Agregar(Acceso componente)
        {
            throw new InvalidOperationException(Translations.ERR_PATENTE_ADD_CHILD.ToString());
            }

            public override void Eliminar(Acceso acceso)
            {
            throw new InvalidOperationException(Translations.ERR_PATENTE_REMOVE_CHILD.ToString());
        }
    }
}
