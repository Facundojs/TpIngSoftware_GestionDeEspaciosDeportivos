using System;

namespace Domain.Entities
{
    public class Cliente
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public int DNI { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public Guid? MembresiaID { get; set; }
        public int Estado { get; set; }

        public Cliente()
        {
            Estado = 0; // Activo
        }
    }
}
