using System;

namespace Domain.Entities
{
    public class Rutina
    {
        public Guid Id { get; set; }
        public Guid ClienteID { get; set; }
        public DateTime Desde { get; set; }
        public DateTime? Hasta { get; set; }
        public string Detalle { get; set; }

        public Rutina()
        {
            Id = Guid.NewGuid();
        }
    }
}
