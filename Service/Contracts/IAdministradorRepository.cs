using System;
using Domain.Entities;

namespace Service.Contracts
{
    public interface IAdministradorRepository
    {
        Administrador GetById(Guid id);
    }
}
