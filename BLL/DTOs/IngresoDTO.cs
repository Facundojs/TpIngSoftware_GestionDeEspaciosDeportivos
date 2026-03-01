using System;

namespace BLL.DTOs
{
    public class IngresoDTO
    {
        public Guid Id { get; set; }
        public Guid ClienteID { get; set; }
        public string ClienteNombre { get; set; }
        public DateTime FechaHora { get; set; }
    }
}
