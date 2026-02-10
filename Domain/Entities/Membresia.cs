using System;

namespace Domain.Entities
{
    public class Membresia
    {
        public Guid Id { get; set; }
        public int Codigo { get; set; }
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
        public int Regularidad { get; set; }
        public bool Activa { get; set; }
        public string Detalle { get; set; }

        public Membresia()
        {
            Id = Guid.NewGuid();
            Activa = true;
        }
    }
}
