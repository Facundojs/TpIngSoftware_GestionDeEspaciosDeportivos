using Domain.Entities;
using Service.Contracts;
using System;

namespace DAL.Contracts
{
    public interface IRutinaRepository : IGenericRepository<Rutina>
    {
        Rutina GetActivaByCliente(Guid clienteId);
        void FinalizarRutina(Guid rutinaId);
    }
}
