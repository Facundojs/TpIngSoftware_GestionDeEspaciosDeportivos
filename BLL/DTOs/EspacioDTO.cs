using System;

namespace BLL.DTOs
{
    public class EspacioDTO
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal PrecioHora { get; set; }
    }
}
