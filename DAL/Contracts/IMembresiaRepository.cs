using Domain.Entities;
using Service.Contracts;
using System;
using System.Collections.Generic;

namespace DAL.Contracts
{
    public interface IMembresiaRepository : IGenericRepository<Membresia>
    {
        Membresia GetByCodigo(int codigo);
        List<Membresia> ListarActivas();
    }
}
