using System;
using Domain.Entities;
using System.Collections.Generic;

namespace DAL.Contracts
{
    public interface IMovimientoRepository
    {
        void Insert(Movimiento movimiento);
        Movimiento GetByClienteAndMonth(Guid clienteId, int month, int year, string tipo);
    }
}
