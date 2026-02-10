using System;

namespace Domain.Entities
{
    public class Espacio
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal PrecioHora { get; set; }

        public Espacio()
        {
            Id = Guid.NewGuid();
        }
    }
}
