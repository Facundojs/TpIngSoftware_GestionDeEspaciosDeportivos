using DAL.Contracts;
using Domain.Entities;
using Service.Impl;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace DAL.Impl
{
    public class EjercicioSqlRepository : BaseBusinessSqlRepository, IEjercicioRepository
    {
        public void Add(Ejercicio obj)
        {
            Add(obj, null, null);
        }

        public void Add(Ejercicio obj, SqlConnection conn, SqlTransaction tran)
        {
            string query = "INSERT INTO Ejercicio (Id, Nombre) VALUES (@Id, @Nombre)";
            SqlParameter[] parameters = {
                new SqlParameter("@Id", obj.Id),
                new SqlParameter("@Nombre", obj.Nombre)
            };
            ExecuteNonQuery(query, parameters, conn, tran);
        }

        public void Update(Ejercicio obj)
        {
            string query = "UPDATE Ejercicio SET Nombre = @Nombre WHERE Id = @Id";
            SqlParameter[] parameters = {
                new SqlParameter("@Id", obj.Id),
                new SqlParameter("@Nombre", obj.Nombre)
            };
            ExecuteNonQuery(query, parameters);
        }

        public void Remove(Guid id)
        {
            string query = "DELETE FROM Ejercicio WHERE Id = @Id";
            SqlParameter[] parameters = { new SqlParameter("@Id", id) };
            ExecuteNonQuery(query, parameters);
        }

        public Ejercicio GetById(Guid id)
        {
            string query = "SELECT Id, Nombre FROM Ejercicio WHERE Id = @Id";
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

        public List<Ejercicio> GetAll()
        {
            string query = "SELECT Id, Nombre FROM Ejercicio";

            return ExecuteReader(query, null, reader =>
            {
                var list = new List<Ejercicio>();
                while (reader.Read())
                {
                    list.Add(Map(reader));
                }
                return list;
            });
        }

        public Ejercicio GetByNombre(string nombre)
        {
            string query = "SELECT Id, Nombre FROM Ejercicio WHERE Nombre = @Nombre";
            SqlParameter[] parameters = { new SqlParameter("@Nombre", nombre) };

            return ExecuteReader(query, parameters, reader =>
            {
                if (reader.Read())
                {
                    return Map(reader);
                }
                return null;
            });
        }

        private Ejercicio Map(SqlDataReader reader)
        {
            return new Ejercicio
            {
                Id = reader.GetGuid(0),
                Nombre = reader.GetString(1)
            };
        }
    }
}
