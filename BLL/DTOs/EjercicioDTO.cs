using System;

namespace BLL.DTOs
{
    public class EjercicioDTO
    {
        public Guid Id { get; set; }
        public Guid RutinaID { get; set; }
        public string Nombre { get; set; }
        public int Repeticiones { get; set; }
        public int DiaSemana { get; set; }
        public int Orden { get; set; }

        public EjercicioDTO()
        {
            Id = Guid.NewGuid();
        }
    }
}
