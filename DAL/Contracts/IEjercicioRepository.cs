using Domain.Entities;
using Service.Contracts;

namespace DAL.Contracts
{
    public interface IEjercicioRepository : IGenericRepository<Ejercicio>
    {
        Ejercicio GetByNombre(string nombre);
    }
}
