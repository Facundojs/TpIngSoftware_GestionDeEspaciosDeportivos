using DAL.Contracts;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DAL.Impl.SqlServer
{
    public class IngresoSqlRepository : BaseBusinessSqlRepository, IIngresoRepository
    {
        public void Add(Ingreso entity)
        {
            var parameters = new[]
            {
                new SqlParameter("@Id", SqlDbType.UniqueIdentifier) { Value = entity.Id },
                new SqlParameter("@ClienteID", SqlDbType.UniqueIdentifier) { Value = entity.ClienteID },
                new SqlParameter("@FechaHora", SqlDbType.DateTime) { Value = entity.FechaHora }
            };

            ExecuteNonQuery("INSERT INTO Ingreso (Id, ClienteID, FechaHora) VALUES (@Id, @ClienteID, @FechaHora)", parameters);
        }

        public void Remove(Guid id)
        {
            var parameters = new[] { new SqlParameter("@Id", SqlDbType.UniqueIdentifier) { Value = id } };
            ExecuteNonQuery("DELETE FROM Ingreso WHERE Id = @Id", parameters);
        }

        public List<Ingreso> GetAll()
        {
            return ExecuteReader("SELECT Id, ClienteID, FechaHora FROM Ingreso", null, reader =>
            {
                var list = new List<Ingreso>();
                while (reader.Read())
                {
                    list.Add(new Ingreso
                    {
                        Id = (Guid)reader["Id"],
                        ClienteID = (Guid)reader["ClienteID"],
                        FechaHora = (DateTime)reader["FechaHora"]
                    });
                }
                return list;
            });
        }

        public Ingreso GetById(Guid id)
        {
            var parameters = new[] { new SqlParameter("@Id", SqlDbType.UniqueIdentifier) { Value = id } };
            return ExecuteReader("SELECT Id, ClienteID, FechaHora FROM Ingreso WHERE Id = @Id", parameters, reader =>
            {
                if (reader.Read())
                {
                    return new Ingreso
                    {
                        Id = (Guid)reader["Id"],
                        ClienteID = (Guid)reader["ClienteID"],
                        FechaHora = (DateTime)reader["FechaHora"]
                    };
                }
                return null;
            });
        }

        public void Update(Ingreso entity)
        {
            var parameters = new[]
            {
                new SqlParameter("@Id", SqlDbType.UniqueIdentifier) { Value = entity.Id },
                new SqlParameter("@ClienteID", SqlDbType.UniqueIdentifier) { Value = entity.ClienteID },
                new SqlParameter("@FechaHora", SqlDbType.DateTime) { Value = entity.FechaHora }
            };

            ExecuteNonQuery("UPDATE Ingreso SET ClienteID = @ClienteID, FechaHora = @FechaHora WHERE Id = @Id", parameters);
        }
    }
}
