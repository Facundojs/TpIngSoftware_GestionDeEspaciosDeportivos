using System;
using System.Collections.Generic;
using BLL.DTOs;
using BLL.Services;

namespace TpIngSoftware_GestionDeEspaciosDeportivos.Business
{
    /// <summary>
    /// UI-layer facade over <see cref="RutinaService"/> that decouples WinForms forms
    /// from direct BLL service instantiation.
    /// </summary>
    public class RutinaManager
    {
        private readonly RutinaService _service;

        public RutinaManager()
        {
            _service = new RutinaService();
        }

        /// <inheritdoc cref="RutinaService.ObtenerRutinaActiva"/>
        public RutinaDTO ObtenerRutinaActiva(Guid clienteId)
        {
            return _service.ObtenerRutinaActiva(clienteId);
        }

        /// <inheritdoc cref="RutinaService.ObtenerRutina"/>
        public RutinaDTO ObtenerRutina(Guid rutinaId)
        {
            return _service.ObtenerRutina(rutinaId);
        }

        /// <inheritdoc cref="RutinaService.ListarRutinas"/>
        public List<RutinaDTO> ListarRutinas(bool soloActivas)
        {
            return _service.ListarRutinas(soloActivas);
        }

        /// <inheritdoc cref="RutinaService.CrearRutina"/>
        public void CrearRutina(RutinaDTO dto)
        {
            _service.CrearRutina(dto);
        }

        /// <inheritdoc cref="RutinaService.ModificarRutina"/>
        public void ModificarRutina(Guid rutinaId, List<EjercicioDTO> ejercicios)
        {
            _service.ModificarRutina(rutinaId, ejercicios);
        }

        /// <inheritdoc cref="RutinaService.BorrarRutina"/>
        public void BorrarRutina(Guid rutinaId)
        {
            _service.BorrarRutina(rutinaId);
        }
    }
}
