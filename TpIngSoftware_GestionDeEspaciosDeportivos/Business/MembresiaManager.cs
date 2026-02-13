using System;
using System.Collections.Generic;
using BLL.DTOs;
using BLL.Services;

namespace TpIngSoftware_GestionDeEspaciosDeportivos.Business
{
    public class MembresiaManager
    {
        private readonly MembresiaService _service;

        public MembresiaManager()
        {
            _service = new MembresiaService();
        }

        public void CrearMembresia(MembresiaDTO dto)
        {
            _service.CrearMembresia(dto);
        }

        public void ActualizarMembresia(MembresiaDTO dto)
        {
            _service.ActualizarMembresia(dto);
        }

        public void DeshabilitarMembresia(Guid id)
        {
            _service.DeshabilitarMembresia(id);
        }

        public List<MembresiaDTO> ListarMembresias(bool soloActivas)
        {
            return _service.ListarMembresias(soloActivas);
        }

        public MembresiaDTO ObtenerMembresia(Guid id)
        {
            return _service.ObtenerMembresia(id);
        }
    }
}
