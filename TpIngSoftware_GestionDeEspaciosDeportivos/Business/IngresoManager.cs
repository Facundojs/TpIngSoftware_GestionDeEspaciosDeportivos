using System.Collections.Generic;
using BLL.DTOs;
using BLL.Services;

namespace TpIngSoftware_GestionDeEspaciosDeportivos.Business
{
    public class IngresoManager
    {
        private readonly IngresoService _service;

        public IngresoManager()
        {
            _service = new IngresoService();
        }

        public List<IngresoDTO> ListarIngresos()
        {
            return _service.ListarIngresos();
        }
    }
}
