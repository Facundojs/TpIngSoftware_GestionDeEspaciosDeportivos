using Domain.Entities;
using System;
using System.Collections.Generic;

namespace DAL.Contracts
{
    public interface IBalanceRepository
    {
        Balance ObtenerBalance(Guid clienteId);
        List<Movimiento> ListarMovimientos(Guid clienteId, DateTime? desde, DateTime? hasta);
    }
}
