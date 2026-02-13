using System;

namespace BLL.DTOs
{
    public class MembresiaDTO
    {
        public Guid Id { get; set; }
        public int Codigo { get; set; }
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
        public int Regularidad { get; set; }
        public bool Activa { get; set; }
        public string Detalle { get; set; }

        public string PrecioFormateado => $"${Precio:N2}";
    }
}
