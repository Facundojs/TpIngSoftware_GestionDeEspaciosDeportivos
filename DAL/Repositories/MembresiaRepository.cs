using DAL.Contracts;
using Domain.Entities;
using System;
using System.Data.SqlClient;

namespace DAL.Repositories
{
    public class MembresiaRepository : BaseBusinessRepository, IMembresiaRepository
    {
        public MembresiaRepository() : base()
        {
        }

        public Membresia GetById(Guid id)
        {
            string query = "SELECT Id, Codigo, Nombre, Precio, Regularidad, Activa, Detalle FROM Membresia WHERE Id = @Id";
            SqlParameter[] parameters = { new SqlParameter("@Id", id) };

            return ExecuteReader(query, parameters, reader =>
            {
                if (reader.Read())
                {
                    return new Membresia
                    {
                        Id = reader.GetGuid(0),
                        Codigo = reader.GetInt32(1),
                        Nombre = reader.GetString(2),
                        Precio = reader.GetDecimal(3),
                        Regularidad = reader.GetInt32(4),
                        Activa = reader.GetBoolean(5),
                        Detalle = reader.IsDBNull(6) ? null : reader.GetString(6)
                    };
                }
                return null;
            });
        }
    }
}
