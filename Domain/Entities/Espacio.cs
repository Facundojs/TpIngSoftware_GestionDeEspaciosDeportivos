using System;
using Domain.Enums;

namespace Domain.Entities
{
    public class Espacio
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal PrecioHora { get; set; }
        public EstadoEspacio Estado { get; set; } = EstadoEspacio.Activo;

        public Espacio()
        {
            Id = Guid.NewGuid();
        }
    }
}
