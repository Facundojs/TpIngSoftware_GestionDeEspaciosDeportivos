using BLL.DTOs;
using BLL.Services;
using System;
using System.Collections.Generic;

namespace TpIngSoftware_GestionDeEspaciosDeportivos.Business
{
    /// <summary>
    /// UI-layer facade over <see cref="PagoService"/> that decouples WinForms forms
    /// from direct BLL service instantiation.
    /// </summary>
    public class PagoManager
    {
        private readonly PagoService _service;

        public PagoManager()
        {
            _service = new PagoService();
        }

        /// <inheritdoc cref="PagoService.RegistrarPago"/>
        public void RegistrarPago(PagoDTO dto)
        {
            _service.RegistrarPago(dto);
        }

        /// <inheritdoc cref="PagoService.ReembolsarPago"/>
        public void ReembolsarPago(Guid pagoId)
        {
            _service.ReembolsarPago(pagoId);
        }

        /// <inheritdoc cref="PagoService.AdjuntarComprobante"/>
        public void AdjuntarComprobante(Guid pagoId, ComprobanteDTO dto)
        {
            _service.AdjuntarComprobante(pagoId, dto);
        }

        /// <inheritdoc cref="PagoService.ObtenerComprobante"/>
        public ComprobanteDTO ObtenerComprobante(Guid pagoId)
        {
            return _service.ObtenerComprobante(pagoId);
        }

        /// <inheritdoc cref="PagoService.ListarPagos"/>
        public List<PagoDTO> ListarPagos(Guid? clienteId, DateTime? desde, DateTime? hasta)
        {
            return _service.ListarPagos(clienteId, desde, hasta);
        }

        /// <inheritdoc cref="PagoService.ObtenerPagosPorReserva"/>
        public List<PagoDTO> ObtenerPagosPorReserva(Guid reservaId)
        {
            return _service.ObtenerPagosPorReserva(reservaId);
        }
    }
}
