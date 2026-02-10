using System;

namespace Domain.Entities
{
    public class Cliente : Usuario
    {
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public int DNI { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public Guid? MembresiaID { get; set; }

        public Cliente() : base()
        {
        }
    }
}
