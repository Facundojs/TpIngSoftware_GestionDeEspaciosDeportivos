using System;

namespace Domain.Entities
{
    public class Ingreso
    {
        public Guid Id { get; set; }
        public Guid ClienteID { get; set; }
        public DateTime FechaHora { get; set; }
    }
}
