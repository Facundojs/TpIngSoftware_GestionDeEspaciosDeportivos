using Domain.Entities;
using Service.Contracts;
using System.Collections.Generic;

namespace DAL.Contracts
{
    public interface IEspacioRepository : IGenericRepository<Espacio>
    {
        List<Espacio> ListarDisponibles();
    }
}
