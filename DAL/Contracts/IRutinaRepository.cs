using Domain.Entities;
using Service.Contracts;
using System;
using System.Collections.Generic;

namespace DAL.Contracts
{
    public interface IRutinaRepository : IGenericRepository<Rutina>
    {
        Rutina GetActivaByCliente(Guid clienteId);
        void FinalizarRutina(Guid rutinaId);
    }
}
