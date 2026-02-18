using System;
using System.Collections.Generic;
using BLL.DTOs;
using BLL.Services;

namespace TpIngSoftware_GestionDeEspaciosDeportivos.Business
{
    public class RutinaManager
    {
        private readonly RutinaService _service;

        public RutinaManager()
        {
            _service = new RutinaService();
        }

        public RutinaDTO ObtenerRutinaActiva(Guid clienteId)
        {
            return _service.ObtenerRutinaActiva(clienteId);
        }

        public void CrearRutina(RutinaDTO dto)
        {
            _service.CrearRutina(dto);
        }

        public void ModificarRutina(Guid rutinaId, List<EjercicioDTO> ejercicios)
        {
            _service.ModificarRutina(rutinaId, ejercicios);
        }

        public void BorrarRutina(Guid rutinaId)
        {
            _service.BorrarRutina(rutinaId);
        }
    }
}
