using System;
using Domain.Entities;

namespace Service.Contracts
{
    public interface IClienteRepository
    {
        Cliente GetById(Guid id);
    }
}
