using Domain.Entities;
using Service.Contracts;
using System;
using System.Collections.Generic;

namespace DAL.Contracts
{
    public interface IPagoRepository : IGenericRepository<Pago>
    {
        List<Pago> GetByCliente(Guid clienteId, DateTime? desde, DateTime? hasta);
        Pago GetByCodigo(int codigo);
        Pago GetByReserva(Guid reservaId);
    }
}
