using DAL.Contracts;
using Domain.Entities;
using Service.Impl;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace DAL.Impl.SqlServer
{
    public class ClienteMembresiaSqlRepository : BaseBusinessSqlRepository, IClienteMembresiaRepository
    {
        public ClienteMembresiaSqlRepository() : base()
        {
        }

        public void Add(ClienteMembresia obj)
        {
            Add(obj, null, null);
        }

        public void Add(ClienteMembresia obj, SqlConnection conn, SqlTransaction tran)
        {
            string query = "INSERT INTO ClienteMembresia (Id, ClienteID, MembresiaID, FechaAsignacion, ProximaFechaPago, FechaBaja) VALUES (@Id, @ClienteID, @MembresiaID, @FechaAsignacion, @ProximaFechaPago, @FechaBaja)";
            SqlParameter[] parameters = {
                new SqlParameter("@Id", obj.Id),
                new SqlParameter("@ClienteID", obj.ClienteID),
                new SqlParameter("@MembresiaID", obj.MembresiaID),
                new SqlParameter("@FechaAsignacion", obj.FechaAsignacion),
                new SqlParameter("@ProximaFechaPago", (object)obj.ProximaFechaPago ?? DBNull.Value),
                new SqlParameter("@FechaBaja", (object)obj.FechaBaja ?? DBNull.Value)
            };
            ExecuteNonQuery(query, parameters, conn, tran);
        }

        public void Update(ClienteMembresia obj)
        {
            Update(obj, null, null);
        }

        public void Update(ClienteMembresia obj, SqlConnection conn, SqlTransaction tran)
        {
            string query = "UPDATE ClienteMembresia SET ClienteID = @ClienteID, MembresiaID = @MembresiaID, FechaAsignacion = @FechaAsignacion, ProximaFechaPago = @ProximaFechaPago, FechaBaja = @FechaBaja WHERE Id = @Id";
            SqlParameter[] parameters = {
                new SqlParameter("@Id", obj.Id),
                new SqlParameter("@ClienteID", obj.ClienteID),
                new SqlParameter("@MembresiaID", obj.MembresiaID),
                new SqlParameter("@FechaAsignacion", obj.FechaAsignacion),
                new SqlParameter("@ProximaFechaPago", (object)obj.ProximaFechaPago ?? DBNull.Value),
                new SqlParameter("@FechaBaja", (object)obj.FechaBaja ?? DBNull.Value)
            };
            ExecuteNonQuery(query, parameters, conn, tran);
        }

        public void Remove(Guid id)
        {
            string query = "DELETE FROM ClienteMembresia WHERE Id = @Id";
            SqlParameter[] parameters = { new SqlParameter("@Id", id) };
            ExecuteNonQuery(query, parameters);
        }

        public ClienteMembresia GetById(Guid id)
        {
            string query = "SELECT Id, ClienteID, MembresiaID, FechaAsignacion, ProximaFechaPago, FechaBaja FROM ClienteMembresia WHERE Id = @Id";
            SqlParameter[] parameters = { new SqlParameter("@Id", id) };

            return ExecuteReader(query, parameters, reader =>
            {
                if (reader.Read())
                {
                    return Map(reader);
                }
                return null;
            });
        }

        public List<ClienteMembresia> GetAll()
        {
            string query = "SELECT Id, ClienteID, MembresiaID, FechaAsignacion, ProximaFechaPago, FechaBaja FROM ClienteMembresia";

            return ExecuteReader(query, null, reader =>
            {
                var list = new List<ClienteMembresia>();
                while (reader.Read())
                {
                    list.Add(Map(reader));
                }
                return list;
            });
        }

        public ClienteMembresia GetActiveByClienteId(Guid clienteId, SqlConnection conn = null, SqlTransaction tran = null)
        {
            string query = "SELECT TOP 1 Id, ClienteID, MembresiaID, FechaAsignacion, ProximaFechaPago, FechaBaja FROM ClienteMembresia WHERE ClienteID = @ClienteID AND FechaBaja IS NULL ORDER BY FechaAsignacion DESC";
            SqlParameter[] parameters = { new SqlParameter("@ClienteID", clienteId) };

            return ExecuteReader(query, parameters, reader =>
            {
                if (reader.Read())
                {
                    return Map(reader);
                }
                return null;
            }, conn, tran);
        }

        private ClienteMembresia Map(SqlDataReader reader)
        {
            return new ClienteMembresia
            {
                Id = reader.GetGuid(0),
                ClienteID = reader.GetGuid(1),
                MembresiaID = reader.GetGuid(2),
                FechaAsignacion = reader.GetDateTime(3),
                ProximaFechaPago = reader.IsDBNull(4) ? (DateTime?)null : reader.GetDateTime(4),
                FechaBaja = reader.IsDBNull(5) ? (DateTime?)null : reader.GetDateTime(5)
            };
        }
    }
}
