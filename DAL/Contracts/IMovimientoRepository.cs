using Domain.Entities;
using System.Data.SqlClient;

namespace DAL.Contracts
{
    public interface IMovimientoRepository
    {
        void Insertar(Movimiento obj, SqlConnection conn = null, SqlTransaction tran = null);
    }
}
