using System;
using System.Collections.Generic;
using Domain.Entities;

namespace DAL.Contracts
{
    public interface IClienteRepository
    {
        Cliente GetById(Guid id);
        List<Cliente> ListarTodos();
    }
}
