using Domain.Entities;
using System;

namespace DAL.Contracts
{
    public interface IMembresiaRepository
    {
        Membresia GetById(Guid id);
    }
}
