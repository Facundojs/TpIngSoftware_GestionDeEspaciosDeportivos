using DAL.Contracts;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace DAL.Impl
{
    public class BalanceSqlRepository : BaseBusinessSqlRepository, IBalanceRepository
    {
        public Balance ObtenerBalance(Guid clienteId)
        {
            string query = "SELECT ClienteID, Saldo, UltimaActualizacion FROM vw_Balance WHERE ClienteID = @Id";
            var parameters = new SqlParameter[] { new SqlParameter("@Id", clienteId) };

            return ExecuteReader(query, parameters, reader =>
            {
                if (reader.Read())
                {
                    return new Balance
                    {
                        ClienteID = reader.GetGuid(0),
                        Saldo = reader.GetDecimal(1),
                        UltimaActualizacion = reader.GetDateTime(2)
                    };
                }
                return new Balance
                {
                    ClienteID = clienteId,
                    Saldo = 0,
                    UltimaActualizacion = DateTime.Now
                };
            });
        }

        public List<Movimiento> ListarMovimientos(Guid clienteId, DateTime? desde, DateTime? hasta)
        {
            string query = "SELECT Id, ClienteID, Tipo, Monto, Descripcion, Fecha, PagoID FROM Movimiento WHERE ClienteID = @Id";
            var parameters = new List<SqlParameter> { new SqlParameter("@Id", clienteId) };

            if (desde.HasValue)
            {
                query += " AND Fecha >= @Desde";
                parameters.Add(new SqlParameter("@Desde", desde.Value));
            }

            if (hasta.HasValue)
            {
                query += " AND Fecha <= @Hasta";
                parameters.Add(new SqlParameter("@Hasta", hasta.Value));
            }

            query += " ORDER BY Fecha DESC";

            return ExecuteReader(query, parameters.ToArray(), reader =>
            {
                var list = new List<Movimiento>();
                while (reader.Read())
                {
                    list.Add(new Movimiento
                    {
                        Id = reader.GetGuid(0),
                        ClienteID = reader.GetGuid(1),
                        Tipo = reader.GetString(2),
                        Monto = reader.GetDecimal(3),
                        Descripcion = reader.IsDBNull(4) ? null : reader.GetString(4),
                        Fecha = reader.GetDateTime(5),
                        PagoID = reader.IsDBNull(6) ? (Guid?)null : reader.GetGuid(6)
                    });
                }
                return list;
            });
        }
    }
}
