using System;
using System.Collections.Generic;
using BLL.DTOs;
using BLL.Services;

namespace TpIngSoftware_GestionDeEspaciosDeportivos.Business
{
    /// <summary>
    /// UI-layer facade over <see cref="MembresiaService"/> that decouples WinForms forms
    /// from direct BLL service instantiation.
    /// </summary>
    public class MembresiaManager
    {
        private readonly MembresiaService _service;

        public MembresiaManager()
        {
            _service = new MembresiaService();
        }

        /// <inheritdoc cref="MembresiaService.CrearMembresia"/>
        public void CrearMembresia(MembresiaDTO dto)
        {
            _service.CrearMembresia(dto);
        }

        /// <inheritdoc cref="MembresiaService.ActualizarMembresia"/>
        public void ActualizarMembresia(MembresiaDTO dto)
        {
            _service.ActualizarMembresia(dto);
        }

        /// <inheritdoc cref="MembresiaService.DeshabilitarMembresia"/>
        public void DeshabilitarMembresia(Guid id)
        {
            _service.DeshabilitarMembresia(id);
        }

        /// <inheritdoc cref="MembresiaService.ListarMembresias"/>
        public List<MembresiaDTO> ListarMembresias(bool soloActivas)
        {
            return _service.ListarMembresias(soloActivas);
        }

        /// <inheritdoc cref="MembresiaService.ObtenerMembresia"/>
        public MembresiaDTO ObtenerMembresia(Guid id)
        {
            return _service.ObtenerMembresia(id);
        }
    }
}
