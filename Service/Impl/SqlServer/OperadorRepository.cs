using Domain.Entities;
using Service.Contracts;
using Service.Helpers;
using System;
using System.Data.SqlClient;

namespace Service.Impl.SqlServer
{
    public class OperadorRepository : BaseRepository, IOperadorRepository
    {
        public OperadorRepository() : base(ConnectionManager.BusinessConnectionName)
        {
        }

        public Operador GetById(Guid id)
        {
            string query = "SELECT Id, Email, FechaIngreso FROM Operador WHERE Id = @Id";
            SqlParameter[] parameters = { new SqlParameter("@Id", id) };

            return ExecuteReader(query, parameters, reader =>
            {
                if (reader.Read())
                {
                    return new Operador
                    {
                        Id = reader.GetGuid(0),
                        Email = reader.GetString(1),
                        FechaIngreso = reader.GetDateTime(2)
                    };
                }
                return null;
            });
        }
    }
}
