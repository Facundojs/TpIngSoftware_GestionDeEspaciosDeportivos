using DAL.Contracts;
using Domain.Entities;
using Service.Impl;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace DAL.Impl
{
    public class RutinaSqlRepository : BaseBusinessSqlRepository, IRutinaRepository
    {
        public void Add(Rutina obj)
        {
            Add(obj, null, null);
        }

        public void Add(Rutina obj, SqlConnection conn, SqlTransaction tran)
        {
            string query = "INSERT INTO Rutina (Id, ClienteID, Desde, Hasta, Detalle) VALUES (@Id, @ClienteID, @Desde, @Hasta, @Detalle)";
            SqlParameter[] parameters = {
                new SqlParameter("@Id", obj.Id),
                new SqlParameter("@ClienteID", obj.ClienteID),
                new SqlParameter("@Desde", obj.Desde),
                new SqlParameter("@Hasta", (object)obj.Hasta ?? DBNull.Value),
                new SqlParameter("@Detalle", (object)obj.Detalle ?? DBNull.Value)
            };
            ExecuteNonQuery(query, parameters, conn, tran);
        }

        public void Update(Rutina obj)
        {
            string query = "UPDATE Rutina SET ClienteID = @ClienteID, Desde = @Desde, Hasta = @Hasta, Detalle = @Detalle WHERE Id = @Id";
            SqlParameter[] parameters = {
                new SqlParameter("@Id", obj.Id),
                new SqlParameter("@ClienteID", obj.ClienteID),
                new SqlParameter("@Desde", obj.Desde),
                new SqlParameter("@Hasta", (object)obj.Hasta ?? DBNull.Value),
                new SqlParameter("@Detalle", (object)obj.Detalle ?? DBNull.Value)
            };
            ExecuteNonQuery(query, parameters);
        }

        public void Remove(Guid id)
        {
            string query = "DELETE FROM Rutina WHERE Id = @Id";
            SqlParameter[] parameters = { new SqlParameter("@Id", id) };
            ExecuteNonQuery(query, parameters);
        }

        public Rutina GetById(Guid id)
        {
            string query = "SELECT Id, ClienteID, Desde, Hasta, Detalle FROM Rutina WHERE Id = @Id";
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

        public List<Rutina> GetAll()
        {
            string query = "SELECT Id, ClienteID, Desde, Hasta, Detalle FROM Rutina";

            return ExecuteReader(query, null, reader =>
            {
                var list = new List<Rutina>();
                while (reader.Read())
                {
                    list.Add(Map(reader));
                }
                return list;
            });
        }

        public Rutina GetActivaByCliente(Guid clienteId)
        {
            string query = "SELECT Id, ClienteID, Desde, Hasta, Detalle FROM Rutina WHERE ClienteID = @ClienteID AND Hasta IS NULL";
            SqlParameter[] parameters = { new SqlParameter("@ClienteID", clienteId) };

            return ExecuteReader(query, parameters, reader =>
            {
                if (reader.Read())
                {
                    return Map(reader);
                }
                return null;
            });
        }

        public void FinalizarRutina(Guid rutinaId, SqlConnection conn, SqlTransaction tran)
        {
            string query = "UPDATE Rutina SET Hasta = @Hasta WHERE Id = @Id";
            SqlParameter[] parameters = {
                new SqlParameter("@Id", rutinaId),
                new SqlParameter("@Hasta", DateTime.Now)
            };
            ExecuteNonQuery(query, parameters, conn, tran);
        }

        private Rutina Map(SqlDataReader reader)
        {
            return new Rutina
            {
                Id = reader.GetGuid(0),
                ClienteID = reader.GetGuid(1),
                Desde = reader.GetDateTime(2),
                Hasta = reader.IsDBNull(3) ? (DateTime?)null : reader.GetDateTime(3),
                Detalle = reader.IsDBNull(4) ? null : reader.GetString(4)
            };
        }
    }
}
