using System;
using System.Collections.Generic;
using BLL.DTOs;
using BLL.Services;

namespace TpIngSoftware_GestionDeEspaciosDeportivos.Business
{
    /// <summary>
    /// UI-layer facade over <see cref="EjercicioService"/> that decouples WinForms forms
    /// from direct BLL service instantiation.
    /// </summary>
    public class EjercicioManager
    {
        private readonly EjercicioService _service;

        public EjercicioManager()
        {
            _service = new EjercicioService();
        }

        /// <inheritdoc cref="EjercicioService.ListarEjercicios"/>
        public List<EjercicioDTO> ListarEjercicios()
        {
            return _service.ListarEjercicios();
        }

        /// <inheritdoc cref="EjercicioService.CrearEjercicio"/>
        public void CrearEjercicio(EjercicioDTO dto)
        {
            _service.CrearEjercicio(dto);
        }

        /// <inheritdoc cref="EjercicioService.ModificarEjercicio"/>
        public void ModificarEjercicio(EjercicioDTO dto)
        {
            _service.ModificarEjercicio(dto);
        }

        /// <inheritdoc cref="EjercicioService.EliminarEjercicio"/>
        public void EliminarEjercicio(Guid id)
        {
            _service.EliminarEjercicio(id);
        }
    }
}
