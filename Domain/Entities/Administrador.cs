using System;

namespace Domain.Entities
{
    public class Administrador : Usuario
    {
        public string Email { get; set; }

        public Administrador() : base()
        {
        }
    }
}
