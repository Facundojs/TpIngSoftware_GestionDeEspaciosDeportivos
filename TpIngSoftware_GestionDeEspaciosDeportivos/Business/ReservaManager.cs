using System;
using System.Collections.Generic;
using BLL.DTOs;
using BLL.Services;

namespace TpIngSoftware_GestionDeEspaciosDeportivos.Business
{
    public class ReservaManager
    {
        private readonly ReservaService _service;

        public ReservaManager()
        {
            _service = new ReservaService();
        }

        public List<TimeSpan> ObtenerHorariosDisponibles(Guid espacioId, DateTime fecha)
        {
            return _service.ObtenerHorariosDisponibles(espacioId, fecha);
        }

        public bool VerificarDisponibilidad(Guid espacioId, DateTime fechaHora, int duracion)
        {
            return _service.VerificarDisponibilidad(espacioId, fechaHora, duracion);
        }

        public string GenerarReserva(GenerarReservaDTO dto)
        {
            return _service.GenerarReserva(dto);
        }

        public void CancelarReserva(Guid reservaId)
        {
            _service.CancelarReserva(reservaId);
        }

        public ReservaDTO ObtenerPorCodigo(string codigo)
        {
            return _service.ObtenerPorCodigo(codigo);
        }

        public List<ReservaDTO> ListarReservas(Guid? clienteId, Guid? espacioId, DateTime? desde)
        {
            return _service.ListarReservas(clienteId, espacioId, desde);
        }
    }
}
