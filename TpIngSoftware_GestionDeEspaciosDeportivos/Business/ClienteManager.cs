using System;
using System.Collections.Generic;
using BLL.DTOs;
using BLL.Services;

namespace TpIngSoftware_GestionDeEspaciosDeportivos.Business
{
    public class ClienteManager
    {
        private readonly ClienteService _service;

        public ClienteManager()
        {
            _service = new ClienteService();
        }

        public void RegistrarCliente(ClienteDTO dto)
        {
            _service.RegistrarCliente(dto);
        }

        public void ActualizarMembresia(Guid clienteId, Guid nuevaMembresiaId)
        {
            _service.ActualizarMembresia(clienteId, nuevaMembresiaId);
        }

        public void DeshabilitarCliente(Guid clienteId, string razon)
        {
            _service.DeshabilitarCliente(clienteId, razon);
        }

        public void HabilitarCliente(Guid clienteId)
        {
            _service.HabilitarCliente(clienteId);
        }

        public ResultadoIngresoDTO ValidarIngreso(int dni)
        {
            return _service.ValidarIngreso(dni);
        }

        public List<ClienteDTO> ListarClientes()
        {
            return _service.ListarClientes();
        }

        public ClienteDTO ObtenerCliente(Guid id)
        {
            return _service.ObtenerCliente(id);
        }
    }
}
