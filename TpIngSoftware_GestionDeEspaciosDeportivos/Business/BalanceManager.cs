using System;
using System.Collections.Generic;
using BLL;
using Domain.Entities;

namespace TpIngSoftware_GestionDeEspaciosDeportivos.Business
{
    public class BalanceManager
    {
        private readonly BalanceService _service;

        public BalanceManager()
        {
            _service = new BalanceService();
        }

        public Balance ConsultarBalance(Guid clienteId)
        {
            return _service.ConsultarBalance(clienteId);
        }

        public List<Movimiento> ListarMovimientos(Guid clienteId, DateTime? desde, DateTime? hasta)
        {
            return _service.ListarMovimientos(clienteId, desde, hasta);
        }
    }
}
