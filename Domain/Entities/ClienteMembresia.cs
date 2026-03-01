using System;

namespace Domain.Entities
{
    public class ClienteMembresia
    {
        public Guid Id { get; set; }
        public Guid ClienteID { get; set; }
        public Guid MembresiaID { get; set; }
        public DateTime FechaAsignacion { get; set; }
        public DateTime? ProximaFechaPago { get; set; }
        public DateTime? FechaBaja { get; set; }

        public ClienteMembresia()
        {
            Id = Guid.NewGuid();
            FechaAsignacion = DateTime.Now;
        }
    }
}
