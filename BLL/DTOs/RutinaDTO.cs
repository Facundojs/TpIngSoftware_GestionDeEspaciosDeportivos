using System;
using System.Collections.Generic;

namespace BLL.DTOs
{
    public class RutinaDTO
    {
        public Guid Id { get; set; }
        public Guid ClienteID { get; set; }
        public string ClienteNombre { get; set; }
        public DateTime Desde { get; set; }
        public DateTime? Hasta { get; set; }
        public string Detalle { get; set; }
        public List<EjercicioDTO> Ejercicios { get; set; }

        public RutinaDTO()
        {
            Id = Guid.NewGuid();
            Ejercicios = new List<EjercicioDTO>();
        }
    }
}
