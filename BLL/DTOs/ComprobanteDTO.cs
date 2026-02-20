using System;

namespace BLL.DTOs
{
    public class ComprobanteDTO
    {
        public Guid Id { get; set; }
        public Guid PagoID { get; set; }
        public string NombreArchivo { get; set; }
        public string RutaArchivo { get; set; }
        public DateTime FechaSubida { get; set; }
        public byte[] Contenido { get; set; }

        public ComprobanteDTO()
        {
            Id = Guid.NewGuid();
        }
    }
}
