using Domain.Entities;
using Service.Contracts;
using Service.Helpers;
using System;
using System.Data.SqlClient;

namespace Service.Impl.SqlServer
{
    public class AdministradorRepository : BaseRepository, IAdministradorRepository
    {
        public AdministradorRepository() : base(ConnectionManager.BusinessConnectionName)
        {
        }

        public Administrador GetById(Guid id)
        {
            string query = "SELECT Id, Email FROM Administrador WHERE Id = @Id";
            SqlParameter[] parameters = { new SqlParameter("@Id", id) };

            return ExecuteReader(query, parameters, reader =>
            {
                if (reader.Read())
                {
                    return new Administrador
                    {
                        Id = reader.GetGuid(0),
                        Email = reader.GetString(1)
                    };
                }
                return null;
            });
        }
    }
}
