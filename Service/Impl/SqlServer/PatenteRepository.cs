using Service.Contracts;
using Domain.Composite;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Impl.SqlServer
{
    public class PatenteRepository : BaseRepository, IGenericRepository<Patente>
    {
        public void Add(Patente obj)
        {
            string query = "INSERT INTO Patente (Id, Nombre, TipoAcceso, DataKey) VALUES (@Id, @Nom, @Tipo, @Key)";
            SqlParameter[] p = {
            new SqlParameter("@Id", obj.Id),
            new SqlParameter("@Nom", obj.Nombre),
            new SqlParameter("@Tipo", obj.TipoAcceso),
            new SqlParameter("@Key", obj.DataKey)
        };
            ExecuteNonQuery(query, p);
        }

        public void Update(Patente obj)
        {
            string query = "UPDATE Patente SET Nombre = @Nom, TipoAcceso = @Tipo, DataKey = @Key WHERE Id = @Id";
            SqlParameter[] p = {
            new SqlParameter("@Id", obj.Id),
            new SqlParameter("@Nom", obj.Nombre),
            new SqlParameter("@Tipo", obj.TipoAcceso),
            new SqlParameter("@Key", obj.DataKey)
        };
            ExecuteNonQuery(query, p);
        }

        public void Remove(Guid id)
        {
            string query = "DELETE FROM Patente WHERE Id = @Id";
            ExecuteNonQuery(query, new[] { new SqlParameter("@Id", id) });
        }

        public Patente GetById(Guid id)
        {
            string query = "SELECT Id, Nombre, TipoAcceso, DataKey FROM Patente WHERE Id = @Id";
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                conn.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        return new Patente
                        {
                            Id = dr.GetGuid(0),
                            Nombre = dr.GetString(1),
                            TipoAcceso = dr.GetString(2),
                            DataKey = dr.GetString(3)
                        };
                    }
                }
            }
            return null;
        }

        public List<Patente> GetAll()
        {
            List<Patente> lista = new List<Patente>();
            string query = "SELECT Id, Nombre, TipoAcceso, DataKey FROM Patente";
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new Patente
                        {
                            Id = dr.GetGuid(0),
                            Nombre = dr.GetString(1),
                            TipoAcceso = dr.GetString(2),
                            DataKey = dr.GetString(3)
                        });
                    }
                }
            }
            return lista;
        }
    }
}
