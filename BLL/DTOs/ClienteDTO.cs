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
        public ClienteStatus Status { get; set; }

        public string NombreCompleto => $"{Nombre} {Apellido}";

        public MembresiaDTO MembresiaDetalle { get; set; }
        public decimal Balance { get; set; }
        public EstadoBalance EstadoBalance { get; set; }

        public ClienteDTO()
        {
            Status = ClienteStatus.Activo;
        }
    }
}
