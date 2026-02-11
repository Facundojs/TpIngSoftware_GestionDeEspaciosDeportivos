using DAL.Contracts;
using Domain.Entities;
using System;
using System.Data.SqlClient;

namespace DAL.Repositories
{
    public class MovimientoRepository : BaseBusinessRepository, IMovimientoRepository
    {
        public void Insertar(Movimiento obj, SqlConnection conn = null, SqlTransaction tran = null)
        {
            string query = @"INSERT INTO Movimiento (Id, ClienteID, Tipo, Monto, Descripcion, Fecha, PagoID)
                             VALUES (@Id, @ClienteID, @Tipo, @Monto, @Descripcion, @Fecha, @PagoID)";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@Id", obj.Id),
                new SqlParameter("@ClienteID", obj.ClienteID),
                new SqlParameter("@Tipo", obj.Tipo),
                new SqlParameter("@Monto", obj.Monto),
                new SqlParameter("@Descripcion", (object)obj.Descripcion ?? DBNull.Value),
                new SqlParameter("@Fecha", obj.Fecha),
                new SqlParameter("@PagoID", (object)obj.PagoID ?? DBNull.Value)
            };

            ExecuteNonQuery(query, parameters, conn, tran);
        }
    }
}
