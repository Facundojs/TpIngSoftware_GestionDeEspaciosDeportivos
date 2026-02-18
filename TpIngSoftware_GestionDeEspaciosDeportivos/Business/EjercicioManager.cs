using System;
using System.Collections.Generic;
using BLL.DTOs;
using BLL.Services;

namespace TpIngSoftware_GestionDeEspaciosDeportivos.Business
{
    public class EjercicioManager
    {
        private readonly EjercicioService _service;

        public EjercicioManager()
        {
            _service = new EjercicioService();
        }

        public List<EjercicioDTO> ListarEjercicios()
        {
            return _service.ListarEjercicios();
        }

        public void CrearEjercicio(EjercicioDTO dto)
        {
            _service.CrearEjercicio(dto);
        }

        public void ModificarEjercicio(EjercicioDTO dto)
        {
            _service.ModificarEjercicio(dto);
        }

        public void EliminarEjercicio(Guid id)
        {
            _service.EliminarEjercicio(id);
        }
    }
}
