using System;
using Domain.Entities;
using Service.Contracts;

namespace DAL.Contracts
{
    public interface IClienteMembresiaRepository : IGenericRepository<ClienteMembresia>
    {
        ClienteMembresia GetActiveByClienteId(Guid clienteId);
    }
}
