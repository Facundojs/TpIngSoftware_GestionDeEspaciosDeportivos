using System;
using Domain.Entities;

namespace Service.Contracts
{
    public interface IOperadorRepository
    {
        Operador GetById(Guid id);
    }
}
