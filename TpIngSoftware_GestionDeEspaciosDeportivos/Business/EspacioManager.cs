using System;
using System.Collections.Generic;
using BLL.DTOs;
using BLL.Services;

namespace TpIngSoftware_GestionDeEspaciosDeportivos.Business
{
    /// <summary>
    /// UI-layer facade over <see cref="EspacioService"/> that decouples WinForms forms
    /// from direct BLL service instantiation.
    /// </summary>
    public class EspacioManager
    {
        private readonly EspacioService _service;

        public EspacioManager()
        {
            _service = new EspacioService();
        }

        /// <inheritdoc cref="EspacioService.CrearEspacio"/>
        public void CrearEspacio(EspacioDTO dto)
        {
            _service.CrearEspacio(dto);
        }

        /// <inheritdoc cref="EspacioService.ActualizarEspacio"/>
        public void ActualizarEspacio(EspacioDTO dto)
        {
            _service.ActualizarEspacio(dto);
        }

        /// <inheritdoc cref="EspacioService.EliminarEspacio"/>
        public void EliminarEspacio(Guid id)
        {
            _service.EliminarEspacio(id);
        }

        /// <inheritdoc cref="EspacioService.ListarEspacios"/>
        public List<EspacioDTO> ListarEspacios()
        {
            return _service.ListarEspacios();
        }

        /// <inheritdoc cref="EspacioService.ListarTodos"/>
        public List<EspacioDTO> ListarTodos()
        {
            return _service.ListarTodos();
        }
    }
}
