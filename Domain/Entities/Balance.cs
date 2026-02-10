using System;

namespace Domain.Entities
{
    public class Balance
    {
        public Guid ClienteID { get; set; }
        public decimal Saldo { get; set; }
        public DateTime UltimaActualizacion { get; set; }
    }
}
