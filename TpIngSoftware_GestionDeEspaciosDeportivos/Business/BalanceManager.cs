using System;
using System.Collections.Generic;
using BLL;
using Domain.Entities;

namespace TpIngSoftware_GestionDeEspaciosDeportivos.Business
{
    /// <summary>
    /// UI-layer facade over <see cref="BalanceService"/> that decouples WinForms forms
    /// from direct BLL service instantiation.
    /// </summary>
    public class BalanceManager
    {
        private readonly BalanceService _service;

        public BalanceManager()
        {
            _service = new BalanceService();
        }

        /// <inheritdoc cref="BalanceService.ConsultarBalance"/>
        public Balance ConsultarBalance(Guid clienteId)
        {
            return _service.ConsultarBalance(clienteId);
        }

        /// <inheritdoc cref="BalanceService.ListarMovimientos"/>
        public List<Movimiento> ListarMovimientos(Guid clienteId, DateTime? desde, DateTime? hasta)
        {
            return _service.ListarMovimientos(clienteId, desde, hasta);
        }
    }
}
