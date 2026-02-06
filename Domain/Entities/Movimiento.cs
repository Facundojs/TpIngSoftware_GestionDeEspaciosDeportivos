using System;

namespace Domain.Entities
{
    public class Movimiento
    {
        public Guid Id { get; set; }
        public Guid ClienteID { get; set; }
        public string Tipo { get; set; }
        public decimal Monto { get; set; }
        public string Descripcion { get; set; }
        public DateTime Fecha { get; set; }
        public Guid? PagoID { get; set; }

        public Movimiento()
        {
            Id = Guid.NewGuid();
        }
    }
}
