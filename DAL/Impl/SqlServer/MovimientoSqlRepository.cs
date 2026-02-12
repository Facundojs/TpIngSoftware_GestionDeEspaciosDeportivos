using DAL.Contracts;
using Domain.Entities;
using System;
using System.Data.SqlClient;

namespace DAL.Impl
{
    public class MovimientoSqlRepository : BaseBusinessSqlRepository, IMovimientoRepository
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

        public bool ExisteDeudaMensual(int mes, int anio)
        {
            var fechaInicio = new DateTime(anio, mes, 1);
            var fechaFin = fechaInicio.AddMonths(1);

            string query = "SELECT COUNT(*) FROM Movimiento WHERE Tipo = 'DeudaMembresia' AND Fecha >= @FechaInicio AND Fecha < @FechaFin";

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@FechaInicio", fechaInicio),
                new SqlParameter("@FechaFin", fechaFin)
            };

            int count = ExecuteScalar<int>(query, parameters);
            return count > 0;
        }
    }
}
