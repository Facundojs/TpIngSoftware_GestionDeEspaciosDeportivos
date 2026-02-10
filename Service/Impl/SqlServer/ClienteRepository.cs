using Domain.Entities;
using Service.Contracts;
using Service.Helpers;
using System;
using System.Data.SqlClient;

namespace Service.Impl.SqlServer
{
    public class ClienteRepository : BaseRepository, IClienteRepository
    {
        public ClienteRepository() : base(ConnectionManager.BusinessConnectionName)
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
    }
}
