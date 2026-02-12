using System;
using Domain.Entities;

namespace DAL.Contracts
{
    public interface IMembresiaRepository
    {
        Membresia GetById(Guid id);
    }
}
