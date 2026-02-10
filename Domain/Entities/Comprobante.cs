using System;

namespace Domain.Entities
{
    public class Comprobante
    {
        public Guid Id { get; set; }
        public Guid PagoID { get; set; }
        public string NombreArchivo { get; set; }
        public string RutaArchivo { get; set; }
        public DateTime FechaSubida { get; set; }

        public Comprobante()
        {
            Id = Guid.NewGuid();
        }
    }
}
