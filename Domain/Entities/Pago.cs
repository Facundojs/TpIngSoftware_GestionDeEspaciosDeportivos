using System;

namespace Domain.Entities
{
    public class Pago
    {
        public Guid Id { get; set; }
        public int Codigo { get; set; }
        public Guid ClienteID { get; set; }
        public decimal Monto { get; set; }
        public string Metodo { get; set; }
        public string Detalle { get; set; }
        public DateTime Fecha { get; set; }
        public string Estado { get; set; }
        public Guid? MembresiaID { get; set; }
        public Guid? ReservaID { get; set; }

        public Pago()
        {
            Id = Guid.NewGuid();
        }
    }
}
