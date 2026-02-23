using System;

namespace BLL.DTOs
{
    public class GenerarReservaDTO
    {
        public Guid ClienteId { get; set; }
        public Guid EspacioId { get; set; }
        public DateTime FechaHora { get; set; }
        public int Duracion { get; set; }
        public decimal Adelanto { get; set; }
    }
}
