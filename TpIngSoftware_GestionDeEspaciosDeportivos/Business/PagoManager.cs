using BLL.DTOs;
using BLL.Services;
using System;
using System.Collections.Generic;

namespace TpIngSoftware_GestionDeEspaciosDeportivos.Business
{
    public class PagoManager
    {
        private readonly PagoService _service;

        public PagoManager()
        {
            _service = new PagoService();
        }

        public void RegistrarPago(PagoDTO dto)
        {
            _service.RegistrarPago(dto);
        }

        public void ReembolsarPago(Guid pagoId)
        {
            _service.ReembolsarPago(pagoId);
        }

        public void AdjuntarComprobante(Guid pagoId, ComprobanteDTO dto)
        {
            _service.AdjuntarComprobante(pagoId, dto);
        }

        public ComprobanteDTO ObtenerComprobante(Guid pagoId)
        {
            return _service.ObtenerComprobante(pagoId);
        }

        public List<PagoDTO> ListarPagos(Guid? clienteId, DateTime? desde, DateTime? hasta)
        {
            return _service.ListarPagos(clienteId, desde, hasta);
        }
    }
}
