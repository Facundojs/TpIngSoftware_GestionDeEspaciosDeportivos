using System;
using System.Collections.Generic;
using BLL.DTOs;
using BLL.Facades;
using BLL.Services;

namespace TpIngSoftware_GestionDeEspaciosDeportivos.Business
{
    /// <summary>
    /// UI-layer facade over <see cref="ReservaService"/> and <see cref="ComprobanteFacade"/>
    /// that decouples WinForms forms from direct BLL service instantiation.
    /// </summary>
    public class ReservaManager
    {
        private readonly ReservaService _service;

        public ReservaManager()
        {
            _service = new ReservaService();
        }

        /// <inheritdoc cref="ReservaService.ObtenerHorariosDisponibles"/>
        public List<TimeSpan> ObtenerHorariosDisponibles(Guid espacioId, DateTime fecha)
        {
            return _service.ObtenerHorariosDisponibles(espacioId, fecha);
        }

        /// <inheritdoc cref="ReservaService.VerificarDisponibilidad"/>
        public bool VerificarDisponibilidad(Guid espacioId, DateTime fechaHora, int duracion)
        {
            return _service.VerificarDisponibilidad(espacioId, fechaHora, duracion);
        }

        /// <inheritdoc cref="ReservaService.GenerarReserva"/>
        public string GenerarReserva(GenerarReservaDTO dto)
        {
            return _service.GenerarReserva(dto);
        }

        /// <inheritdoc cref="ReservaService.CancelarReserva"/>
        public void CancelarReserva(Guid reservaId)
        {
            _service.CancelarReserva(reservaId);
        }

        /// <inheritdoc cref="ReservaService.ObtenerPorCodigo"/>
        public ReservaDTO ObtenerPorCodigo(string codigo)
        {
            return _service.ObtenerPorCodigo(codigo);
        }

        /// <inheritdoc cref="ReservaService.ListarReservas"/>
        public List<ReservaDTO> ListarReservas(Guid? clienteId, Guid? espacioId, DateTime? desde)
        {
            return _service.ListarReservas(clienteId, espacioId, desde);
        }

        /// <summary>Retrieves the receipt attached to the given reservation, or <c>null</c> if none exists.</summary>
        /// <param name="reservaId">The reservation identifier.</param>
        public ComprobanteDTO ObtenerComprobantePorReserva(Guid reservaId)
        {
            return new ComprobanteFacade().ObtenerPorReserva(reservaId);
        }
    }
}
