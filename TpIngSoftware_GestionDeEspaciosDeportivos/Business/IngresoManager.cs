using System.Collections.Generic;
using BLL.DTOs;
using BLL.Services;

namespace TpIngSoftware_GestionDeEspaciosDeportivos.Business
{
    /// <summary>
    /// UI-layer facade over <see cref="IngresoService"/> that decouples WinForms forms
    /// from direct BLL service instantiation.
    /// </summary>
    public class IngresoManager
    {
        private readonly IngresoService _service;

        public IngresoManager()
        {
            _service = new IngresoService();
        }

        /// <inheritdoc cref="IngresoService.ListarIngresos"/>
        public List<IngresoDTO> ListarIngresos()
        {
            return _service.ListarIngresos();
        }

        /// <inheritdoc cref="IngresoService.ListarIngresosPorCliente"/>
        public List<IngresoDTO> ListarIngresosPorCliente(System.Guid clienteId)
        {
            return _service.ListarIngresosPorCliente(clienteId);
        }
    }
}
