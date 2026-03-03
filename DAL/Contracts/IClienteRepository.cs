using System;
using Domain.Entities;
using Service.Contracts;

namespace DAL.Contracts
{
    public interface IClienteRepository : IGenericRepository<Cliente>
    {
        Cliente GetByDNI(int dni);
        bool ExistsByDNI(int dni);
        void AsignarMembresia(Guid clienteId, Guid membresiaId);
        bool HasActiveClientsByMembresia(Guid membresiaId);
    }
}
