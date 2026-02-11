using System;
using Domain.Entities;

namespace DAL.Contracts
{
    public interface IClienteRepository
    {
        Cliente GetById(Guid id);
    }
}
