using System;
using Domain.Entities;
using System.Collections.Generic;

namespace DAL.Contracts
{
    public interface IClienteRepository
    {
        Cliente GetById(Guid id);
        List<Cliente> ListarTodos();
    }
}
