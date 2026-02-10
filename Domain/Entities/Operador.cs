using System;

namespace Domain.Entities
{
    public class Operador : Usuario
    {
        public string Email { get; set; }
        public DateTime FechaIngreso { get; set; }

        public Operador() : base()
        {
        }
    }
}
