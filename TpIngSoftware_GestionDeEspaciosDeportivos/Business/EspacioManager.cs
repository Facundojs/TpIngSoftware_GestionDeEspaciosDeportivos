using System;
using System.Collections.Generic;
using BLL.DTOs;
using BLL.Services;

namespace TpIngSoftware_GestionDeEspaciosDeportivos.Business
{
    public class EspacioManager
    {
        private readonly EspacioService _service;

        public EspacioManager()
        {
            _service = new EspacioService();
        }

        public void CrearEspacio(EspacioDTO dto)
        {
            _service.CrearEspacio(dto);
        }

        public void ActualizarEspacio(EspacioDTO dto)
        {
            _service.ActualizarEspacio(dto);
        }

        public void EliminarEspacio(Guid id)
        {
            _service.EliminarEspacio(id);
        }

        public List<EspacioDTO> ListarEspacios()
        {
            return _service.ListarEspacios();
        }
    }
}
