using System;
using System.Collections.Generic;
using BLL.DTOs;
using BLL.Services;

namespace TpIngSoftware_GestionDeEspaciosDeportivos.Business
{
    /// <summary>
    /// UI-layer facade over <see cref="ClienteService"/> that decouples WinForms forms
    /// from direct BLL service instantiation.
    /// </summary>
    /// <remarks>
    /// All methods delegate directly to the underlying service. Error handling and
    /// transaction management remain in the service layer.
    /// </remarks>
    public class ClienteManager
    {
        private readonly ClienteService _service;

        public ClienteManager()
        {
            _service = new ClienteService();
        }

        /// <inheritdoc cref="ClienteService.RegistrarCliente"/>
        public void RegistrarCliente(ClienteDTO dto)
        {
            _service.RegistrarCliente(dto);
        }

        /// <inheritdoc cref="ClienteService.ActualizarMembresia"/>
        public void ActualizarMembresia(Guid clienteId, Guid nuevaMembresiaId)
        {
            _service.ActualizarMembresia(clienteId, nuevaMembresiaId);
        }

        /// <inheritdoc cref="ClienteService.DeshabilitarCliente"/>
        public void DeshabilitarCliente(Guid clienteId, string razon)
        {
            _service.DeshabilitarCliente(clienteId, razon);
        }

        /// <inheritdoc cref="ClienteService.HabilitarCliente"/>
        public void HabilitarCliente(Guid clienteId)
        {
            _service.HabilitarCliente(clienteId);
        }

        /// <inheritdoc cref="ClienteService.ValidarIngreso"/>
        public ResultadoIngresoDTO ValidarIngreso(int dni)
        {
            return _service.ValidarIngreso(dni);
        }

        /// <inheritdoc cref="ClienteService.ListarClientes"/>
        public List<ClienteDTO> ListarClientes()
        {
            return _service.ListarClientes();
        }

        /// <inheritdoc cref="ClienteService.ObtenerCliente"/>
        public ClienteDTO ObtenerCliente(Guid id)
        {
            return _service.ObtenerCliente(id);
        }

        /// <inheritdoc cref="ClienteService.ObtenerClientePorDNI"/>
        public ClienteDTO ObtenerClientePorDNI(int dni)
        {
            return _service.ObtenerClientePorDNI(dni);
        }
    }
}
