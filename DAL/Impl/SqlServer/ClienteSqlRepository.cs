using Domain.Entities;
using DAL.Contracts;
using Service.Helpers;
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

        public Cliente GetById(Guid id)
        {
            string query = "SELECT Id, Nombre, Apellido, DNI, FechaNacimiento, MembresiaID FROM Cliente WHERE Id = @Id";
            SqlParameter[] parameters = { new SqlParameter("@Id", id) };

            return ExecuteReader(query, parameters, reader =>
            {
                if (reader.Read())
                {
                    return new Cliente
                    {
                        Id = reader.GetGuid(0),
                        Nombre = reader.GetString(1),
                        Apellido = reader.GetString(2),
                        DNI = reader.GetInt32(3),
                        FechaNacimiento = reader.GetDateTime(4),
                        MembresiaID = reader.IsDBNull(5) ? (Guid?)null : reader.GetGuid(5)
                    };
                }
                return null;
            });
        }

        public List<Cliente> ListarTodos()
        {
            string query = "SELECT Id, Nombre, Apellido, DNI, FechaNacimiento, MembresiaID FROM Cliente";

            return ExecuteReader(query, null, reader =>
            {
                var list = new List<Cliente>();
                while (reader.Read())
                {
                    list.Add(new Cliente
                    {
                        Id = reader.GetGuid(0),
                        Nombre = reader.GetString(1),
                        Apellido = reader.GetString(2),
                        DNI = reader.GetInt32(3),
                        FechaNacimiento = reader.GetDateTime(4),
                        MembresiaID = reader.IsDBNull(5) ? (Guid?)null : reader.GetGuid(5)
                    });
                }
                return list;
            });
        }
    }
}
