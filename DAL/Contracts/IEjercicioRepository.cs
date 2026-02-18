using Domain.Entities;
using Service.Contracts;
using System.Data.SqlClient;

namespace DAL.Contracts
{
    public interface IEjercicioRepository : IGenericRepository<Ejercicio>
    {
        Ejercicio GetByNombre(string nombre);
        void Add(Ejercicio obj, SqlConnection conn, SqlTransaction tran);
    }
}
