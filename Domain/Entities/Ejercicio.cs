using System;

namespace Domain.Entities
{
    public class Ejercicio
    {
        public Guid Id { get; set; }
        public Guid RutinaID { get; set; }
        public string Nombre { get; set; }
        public int Repeticiones { get; set; }
        public int DiaSemana { get; set; }
        public int Orden { get; set; }

        public Ejercicio()
        {
            Id = Guid.NewGuid();
        }
    }
}
