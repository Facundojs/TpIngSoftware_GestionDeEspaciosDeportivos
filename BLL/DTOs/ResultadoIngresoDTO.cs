using System;

namespace BLL.DTOs
{
    public class ResultadoIngresoDTO
    {
        public bool Permitido { get; set; }
        public string Razon { get; set; }
        public string NombreCliente { get; set; }
        public decimal Balance { get; set; }
    }
}
