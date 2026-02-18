using System;

namespace Domain.Entities
{
    public class Ejercicio
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }

        public Ejercicio()
        {
            Id = Guid.NewGuid();
        }
    }
}
