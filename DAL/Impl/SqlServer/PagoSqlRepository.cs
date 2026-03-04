using DAL.Contracts;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace DAL.Impl
{
    public class PagoSqlRepository : BaseBusinessSqlRepository, IPagoRepository
    {
        public void Add(Pago obj)
        {
            string query = "INSERT INTO Pago (Id, ClienteID, Monto, Metodo, Detalle, Fecha, Estado, MembresiaID, ReservaID) VALUES (@Id, @ClienteID, @Monto, @Metodo, @Detalle, @Fecha, @Estado, @MembresiaID, @ReservaID)";
            SqlParameter[] parameters = {
                new SqlParameter("@Id", obj.Id),
                new SqlParameter("@ClienteID", obj.ClienteID),
                new SqlParameter("@Monto", obj.Monto),
                new SqlParameter("@Metodo", obj.Metodo),
                new SqlParameter("@Detalle", (object)obj.Detalle ?? DBNull.Value),
                new SqlParameter("@Fecha", obj.Fecha),
                new SqlParameter("@Estado", obj.Estado),
                new SqlParameter("@MembresiaID", (object)obj.MembresiaID ?? DBNull.Value),
                new SqlParameter("@ReservaID", (object)obj.ReservaID ?? DBNull.Value)
            };
            ExecuteNonQuery(query, parameters);
        }

        public void Update(Pago obj)
        {
            string query = "UPDATE Pago SET ClienteID = @ClienteID, Monto = @Monto, Metodo = @Metodo, Detalle = @Detalle, Fecha = @Fecha, Estado = @Estado, MembresiaID = @MembresiaID, ReservaID = @ReservaID WHERE Id = @Id";
            SqlParameter[] parameters = {
                new SqlParameter("@Id", obj.Id),
                new SqlParameter("@ClienteID", obj.ClienteID),
                new SqlParameter("@Monto", obj.Monto),
                new SqlParameter("@Metodo", obj.Metodo),
                new SqlParameter("@Detalle", (object)obj.Detalle ?? DBNull.Value),
                new SqlParameter("@Fecha", obj.Fecha),
                new SqlParameter("@Estado", obj.Estado),
                new SqlParameter("@MembresiaID", (object)obj.MembresiaID ?? DBNull.Value),
                new SqlParameter("@ReservaID", (object)obj.ReservaID ?? DBNull.Value)
            };
            ExecuteNonQuery(query, parameters);
        }

        public void Remove(Guid id)
        {
            string query = "DELETE FROM Pago WHERE Id = @Id";
            SqlParameter[] parameters = { new SqlParameter("@Id", id) };
            ExecuteNonQuery(query, parameters);
        }

        public Pago GetById(Guid id)
        {
            string query = "SELECT Id, Codigo, ClienteID, Monto, Metodo, Detalle, Fecha, Estado, MembresiaID, ReservaID FROM Pago WHERE Id = @Id";
            SqlParameter[] parameters = { new SqlParameter("@Id", id) };

            return ExecuteReader(query, parameters, reader =>
            {
                if (reader.Read())
                {
                    return MapFromReader(reader);
                }
                return null;
            });
        }

        public List<Pago> GetAll()
        {
            string query = "SELECT Id, Codigo, ClienteID, Monto, Metodo, Detalle, Fecha, Estado, MembresiaID, ReservaID FROM Pago ORDER BY Fecha DESC";
            return ExecuteReader(query, null, reader =>
            {
                List<Pago> list = new List<Pago>();
                while (reader.Read())
                {
                    list.Add(MapFromReader(reader));
                }
                return list;
            });
        }

        public List<Pago> GetByCliente(Guid clienteId, DateTime? desde, DateTime? hasta)
        {
            string query = "SELECT Id, Codigo, ClienteID, Monto, Metodo, Detalle, Fecha, Estado, MembresiaID, ReservaID FROM Pago WHERE ClienteID = @ClienteID";

            if (desde.HasValue)
                query += " AND Fecha >= @Desde";
            if (hasta.HasValue)
                query += " AND Fecha <= @Hasta";

            query += " ORDER BY Fecha DESC";

            List<SqlParameter> paramsList = new List<SqlParameter>();
            paramsList.Add(new SqlParameter("@ClienteID", clienteId));
            if (desde.HasValue)
                paramsList.Add(new SqlParameter("@Desde", desde.Value));
            if (hasta.HasValue)
                paramsList.Add(new SqlParameter("@Hasta", hasta.Value));

            return ExecuteReader(query, paramsList.ToArray(), reader =>
            {
                List<Pago> list = new List<Pago>();
                while (reader.Read())
                {
                    list.Add(MapFromReader(reader));
                }
                return list;
            });
        }

        public Pago GetByCodigo(int codigo)
        {
            string query = "SELECT Id, Codigo, ClienteID, Monto, Metodo, Detalle, Fecha, Estado, MembresiaID, ReservaID FROM Pago WHERE Codigo = @Codigo";
            SqlParameter[] parameters = { new SqlParameter("@Codigo", codigo) };

            return ExecuteReader(query, parameters, reader =>
            {
                if (reader.Read())
                {
                    return MapFromReader(reader);
                }
                return null;
            });
        }

        public List<Pago> GetByReserva(Guid reservaId)
        {
            string query = "SELECT Id, Codigo, ClienteID, Monto, Metodo, Detalle, Fecha, Estado, MembresiaID, ReservaID FROM Pago WHERE ReservaID = @ReservaID ORDER BY Fecha DESC";
            SqlParameter[] parameters = { new SqlParameter("@ReservaID", reservaId) };

            return ExecuteReader(query, parameters, reader =>
            {
                List<Pago> list = new List<Pago>();
                while (reader.Read())
                {
                    list.Add(MapFromReader(reader));
                }
                return list;
            });
        }

        private Pago MapFromReader(SqlDataReader reader)
        {
            return new Pago
            {
                Id = reader.GetGuid(0),
                Codigo = reader.GetInt32(1),
                ClienteID = reader.GetGuid(2),
                Monto = reader.GetDecimal(3),
                Metodo = reader.GetString(4),
                Detalle = reader.IsDBNull(5) ? null : reader.GetString(5),
                Fecha = reader.GetDateTime(6),
                Estado = reader.GetString(7),
                MembresiaID = reader.IsDBNull(8) ? (Guid?)null : reader.GetGuid(8),
                ReservaID = reader.IsDBNull(9) ? (Guid?)null : reader.GetGuid(9)
            };
        }
    }
}
