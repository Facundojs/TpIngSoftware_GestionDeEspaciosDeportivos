using System;

namespace Domain.Entities
{
    public class Reserva
    {
        public Guid Id { get; set; }
        public Guid ClienteID { get; set; }
        public Guid EspacioID { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime FechaHora { get; set; }
        public int Duracion { get; set; }
        public decimal Adelanto { get; set; }
        public string CodigoReserva { get; set; }
        public string Estado { get; set; }

        public Reserva()
        {
            Id = Guid.NewGuid();
        }
    }
}
