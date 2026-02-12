using Domain.Entities;
using DAL.Contracts;
using Service.Helpers;
using Service.Impl;
using System;
using System.Data.SqlClient;

namespace DAL.Repositories
{
    public class MovimientoRepository : BaseRepository, IMovimientoRepository
    {
        public MovimientoRepository() : base(ConnectionManager.BusinessConnectionName)
        {
        }

        public void Insert(Movimiento movimiento)
        {
            string query = "INSERT INTO Movimiento (Id, ClienteID, Tipo, Monto, Descripcion, Fecha, PagoID) VALUES (@Id, @ClienteID, @Tipo, @Monto, @Descripcion, @Fecha, @PagoID)";
            SqlParameter[] parameters = {
                new SqlParameter("@Id", movimiento.Id),
                new SqlParameter("@ClienteID", movimiento.ClienteID),
                new SqlParameter("@Tipo", movimiento.Tipo),
                new SqlParameter("@Monto", movimiento.Monto),
                new SqlParameter("@Descripcion", movimiento.Descripcion),
                new SqlParameter("@Fecha", movimiento.Fecha),
                new SqlParameter("@PagoID", (object)movimiento.PagoID ?? DBNull.Value)
            };

            ExecuteNonQuery(query, parameters);
        }

        public Movimiento GetByClienteAndMonth(Guid clienteId, int month, int year, string tipo)
        {
            string query = "SELECT Id, ClienteID, Tipo, Monto, Descripcion, Fecha, PagoID FROM Movimiento WHERE ClienteID = @ClienteID AND MONTH(Fecha) = @Month AND YEAR(Fecha) = @Year AND Tipo = @Tipo";
            SqlParameter[] parameters = {
                new SqlParameter("@ClienteID", clienteId),
                new SqlParameter("@Month", month),
                new SqlParameter("@Year", year),
                new SqlParameter("@Tipo", tipo)
            };

            return ExecuteReader(query, parameters, reader =>
            {
                if (reader.Read())
                {
                    return new Movimiento
                    {
                        Id = reader.GetGuid(0),
                        ClienteID = reader.GetGuid(1),
                        Tipo = reader.GetString(2),
                        Monto = reader.GetDecimal(3),
                        Descripcion = reader.GetString(4),
                        Fecha = reader.GetDateTime(5),
                        PagoID = reader.IsDBNull(6) ? (Guid?)null : reader.GetGuid(6)
                    };
                }
                return null;
            });
        }
    }
}
