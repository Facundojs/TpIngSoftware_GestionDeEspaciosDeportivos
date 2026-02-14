using DAL.Contracts;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace DAL.Impl
{
    public class EspacioSqlRepository : BaseBusinessSqlRepository, IEspacioRepository
    {
        public EspacioSqlRepository() : base()
        {
        }

        public void Add(Espacio obj)
        {
            string query = "INSERT INTO Espacio (Id, Nombre, Descripcion, PrecioHora) VALUES (@Id, @Nombre, @Descripcion, @PrecioHora)";
            SqlParameter[] parameters = {
                new SqlParameter("@Id", obj.Id),
                new SqlParameter("@Nombre", obj.Nombre),
                new SqlParameter("@Descripcion", (object)obj.Descripcion ?? DBNull.Value),
                new SqlParameter("@PrecioHora", obj.PrecioHora)
            };
            ExecuteNonQuery(query, parameters);
        }

        public void Update(Espacio obj)
        {
            string query = "UPDATE Espacio SET Nombre = @Nombre, Descripcion = @Descripcion, PrecioHora = @PrecioHora WHERE Id = @Id";
            SqlParameter[] parameters = {
                new SqlParameter("@Id", obj.Id),
                new SqlParameter("@Nombre", obj.Nombre),
                new SqlParameter("@Descripcion", (object)obj.Descripcion ?? DBNull.Value),
                new SqlParameter("@PrecioHora", obj.PrecioHora)
            };
            ExecuteNonQuery(query, parameters);
        }

        public void Remove(Guid id)
        {
            string query = "DELETE FROM Espacio WHERE Id = @Id";
            SqlParameter[] parameters = { new SqlParameter("@Id", id) };
            ExecuteNonQuery(query, parameters);
        }

        public Espacio GetById(Guid id)
        {
            string query = "SELECT Id, Nombre, Descripcion, PrecioHora FROM Espacio WHERE Id = @Id";
            SqlParameter[] parameters = { new SqlParameter("@Id", id) };

            return ExecuteReader(query, parameters, reader =>
            {
                if (reader.Read())
                {
                    return MapFromReader(reader);
                }
                return null;
            }, null, null);
        }

        public List<Espacio> GetAll()
        {
            return ListarDisponibles();
        }

        public List<Espacio> ListarDisponibles()
        {
            string query = "SELECT Id, Nombre, Descripcion, PrecioHora FROM Espacio ORDER BY Nombre";
            return ExecuteReader(query, null, reader =>
            {
                List<Espacio> list = new List<Espacio>();
                while (reader.Read())
                {
                    list.Add(MapFromReader(reader));
                }
                return list;
            }, null, null);
        }

        private Espacio MapFromReader(SqlDataReader reader)
        {
            return new Espacio
            {
                Id = reader.GetGuid(0),
                Nombre = reader.GetString(1),
                Descripcion = reader.IsDBNull(2) ? null : reader.GetString(2),
                PrecioHora = reader.GetDecimal(3)
            };
        }
    }
}
