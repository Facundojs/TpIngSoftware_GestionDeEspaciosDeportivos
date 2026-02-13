using DAL.Contracts;
using Domain.Entities;
using Service.Impl;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace DAL.Impl
{
    public class ClienteSqlRepository : BaseBusinessSqlRepository, IClienteRepository
    {
        public ClienteSqlRepository() : base()
        {
        }

        public void Add(Cliente obj)
        {
            string query = "INSERT INTO Cliente (Id, Nombre, Apellido, DNI, FechaNacimiento, MembresiaID, Estado) VALUES (@Id, @Nombre, @Apellido, @DNI, @FechaNacimiento, @MembresiaID, @Estado)";
            SqlParameter[] parameters = {
                new SqlParameter("@Id", obj.Id),
                new SqlParameter("@Nombre", obj.Nombre),
                new SqlParameter("@Apellido", obj.Apellido),
                new SqlParameter("@DNI", obj.DNI),
                new SqlParameter("@FechaNacimiento", obj.FechaNacimiento),
                new SqlParameter("@MembresiaID", (object)obj.MembresiaID ?? DBNull.Value),
                new SqlParameter("@Estado", obj.Estado)
            };
            ExecuteNonQuery(query, parameters);
        }

        public void Update(Cliente obj)
        {
            string query = "UPDATE Cliente SET Nombre = @Nombre, Apellido = @Apellido, DNI = @DNI, FechaNacimiento = @FechaNacimiento, MembresiaID = @MembresiaID, Estado = @Estado WHERE Id = @Id";
            SqlParameter[] parameters = {
                new SqlParameter("@Id", obj.Id),
                new SqlParameter("@Nombre", obj.Nombre),
                new SqlParameter("@Apellido", obj.Apellido),
                new SqlParameter("@DNI", obj.DNI),
                new SqlParameter("@FechaNacimiento", obj.FechaNacimiento),
                new SqlParameter("@MembresiaID", (object)obj.MembresiaID ?? DBNull.Value),
                new SqlParameter("@Estado", obj.Estado)
            };
            ExecuteNonQuery(query, parameters);
        }

        public void Remove(Guid id)
        {
            string query = "DELETE FROM Cliente WHERE Id = @Id";
            SqlParameter[] parameters = { new SqlParameter("@Id", id) };
            ExecuteNonQuery(query, parameters);
        }

        public Cliente GetById(Guid id)
        {
            string query = "SELECT Id, Nombre, Apellido, DNI, FechaNacimiento, MembresiaID, Estado FROM Cliente WHERE Id = @Id";
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

        public List<Cliente> GetAll()
        {
            string query = "SELECT Id, Nombre, Apellido, DNI, FechaNacimiento, MembresiaID, Estado FROM Cliente";

            return ExecuteReader(query, null, reader =>
            {
                var list = new List<Cliente>();
                while (reader.Read())
                {
                    list.Add(Map(reader));
                }
                return list;
            });
        }

        public Cliente GetByDNI(int dni)
        {
            string query = "SELECT Id, Nombre, Apellido, DNI, FechaNacimiento, MembresiaID, Estado FROM Cliente WHERE DNI = @DNI";
            SqlParameter[] parameters = { new SqlParameter("@DNI", dni) };

            return ExecuteReader(query, parameters, reader =>
            {
                if (reader.Read())
                {
                    return Map(reader);
                }
                return null;
            });
        }

        public bool ExistsByDNI(int dni)
        {
            string query = "SELECT CASE WHEN EXISTS(SELECT 1 FROM Cliente WHERE DNI = @DNI) THEN 1 ELSE 0 END";
            SqlParameter[] parameters = { new SqlParameter("@DNI", dni) };
            return ExecuteScalar<int>(query, parameters) == 1;
        }

        public void AsignarMembresia(Guid clienteId, Guid membresiaId, SqlConnection conn, SqlTransaction tran)
        {
            string query = "UPDATE Cliente SET MembresiaID = @MembresiaID WHERE Id = @Id";
            SqlParameter[] parameters = {
                new SqlParameter("@Id", clienteId),
                new SqlParameter("@MembresiaID", membresiaId)
            };
            ExecuteNonQuery(query, parameters, conn, tran);
        }

        private Cliente Map(SqlDataReader reader)
        {
            return new Cliente
            {
                Id = reader.GetGuid(0),
                Nombre = reader.GetString(1),
                Apellido = reader.GetString(2),
                DNI = reader.GetInt32(3),
                FechaNacimiento = reader.GetDateTime(4),
                MembresiaID = reader.IsDBNull(5) ? (Guid?)null : reader.GetGuid(5),
                Estado = reader.IsDBNull(6) ? 0 : reader.GetInt32(6)
            };
        }
    }
}
