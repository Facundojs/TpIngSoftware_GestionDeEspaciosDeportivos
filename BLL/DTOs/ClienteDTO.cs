using System;

namespace BLL.DTOs
{
    public class ClienteDTO
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public int DNI { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public Guid? MembresiaID { get; set; }
        public bool Activo { get; set; }

        // Computed
        public string NombreCompleto => $"{Nombre} {Apellido}";

        // Extras
        public MembresiaDTO MembresiaDetalle { get; set; }
        public decimal Balance { get; set; }
        public EstadoBalance EstadoBalance { get; set; }

        public ClienteDTO()
        {
            Activo = true;
        }
    }
}
