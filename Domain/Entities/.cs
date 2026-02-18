using System;

namespace Domain.Entities
{
    public class RutinaEjercicio
    {
        public Guid RutinaId { get; set; }
        public Guid EjercicioId { get; set; }
        public int Repeticiones { get; set; }
        public int DiaSemana { get; set; }
        public int Orden { get; set; }

        public Ejercicio Ejercicio { get; set; }
    }
}
