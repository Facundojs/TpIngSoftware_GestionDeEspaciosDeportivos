using Service.Domain;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Service.Contracts;

namespace Service.Impl
{
    public class FamiliaRepository : BaseRepository, IGenericRepository<Familia>
    {
        public void Add(Familia obj)
        {
            string query = "INSERT INTO Familia (Id, Nombre) VALUES (@Id, @Nom)";
            ExecuteNonQuery(query, new[] {
            new SqlParameter("@Id", obj.Id),
            new SqlParameter("@Nom", obj.Nombre)
        });
            // Nota: Los hijos se guardan usualmente en un método separado o mediante Update
        }

        public void Update(Familia obj)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        // 1. Actualizar nombre de la familia
                        string updateFam = "UPDATE Familia SET Nombre = @Nom WHERE Id = @Id";
                        SqlCommand cmdFam = new SqlCommand(updateFam, conn, tran);
                        cmdFam.Parameters.AddWithValue("@Nom", obj.Nombre);
                        cmdFam.Parameters.AddWithValue("@Id", obj.Id);
                        cmdFam.ExecuteNonQuery();

                        // 2. Limpiar patentes asociadas (Relación Composite)
                        string deleteRel = "DELETE FROM FamiliaPatente WHERE IdFamilia = @Id";
                        SqlCommand cmdDel = new SqlCommand(deleteRel, conn, tran);
                        cmdDel.Parameters.AddWithValue("@Id", obj.Id);
                        cmdDel.ExecuteNonQuery();

                        // 3. Insertar nuevas relaciones
                        foreach (var hijo in obj.Accesos)
                        {
                            string insertRel = "INSERT INTO FamiliaPatente (IdFamilia, IdPatente) VALUES (@IdF, @IdP)";
                            SqlCommand cmdIns = new SqlCommand(insertRel, conn, tran);
                            cmdIns.Parameters.AddWithValue("@IdF", obj.Id);
                            cmdIns.Parameters.AddWithValue("@IdP", hijo.Id);
                            cmdIns.ExecuteNonQuery();
                        }
                        tran.Commit();
                    }
                    catch { tran.Rollback(); throw; }
                }
            }
        }

        public void Remove(Guid id)
        {
            // Primero borrar relaciones por integridad referencial
            ExecuteNonQuery("DELETE FROM FamiliaPatente WHERE IdFamilia = @Id", new[] { new SqlParameter("@Id", id) });
            ExecuteNonQuery("DELETE FROM Familia WHERE Id = @Id", new[] { new SqlParameter("@Id", id) });
        }

        public Familia GetById(Guid id)
        {
            Familia fam = null;
            string query = "SELECT Id, Nombre FROM Familia WHERE Id = @Id";

            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            fam = new Familia { Id = dr.GetGuid(0), Nombre = dr.GetString(1) };
                        }
                    }
                }

                if (fam != null)
                {
                    // Cargar los hijos (Patentes) de esta familia
                    string queryHijos = @"SELECT p.Id, p.Nombre, p.TipoAcceso, p.DataKey 
                                     FROM Patente p
                                     INNER JOIN FamiliaPatente fp ON p.Id = fp.IdPatente
                                     WHERE fp.IdFamilia = @Id";

                    using (var cmdHijos = new SqlCommand(queryHijos, conn))
                    {
                        cmdHijos.Parameters.AddWithValue("@Id", id);
                        using (var drH = cmdHijos.ExecuteReader())
                        {
                            while (drH.Read())
                            {
                                fam.Agregar(new Patente
                                {
                                    Id = drH.GetGuid(0),
                                    Nombre = drH.GetString(1),
                                    TipoAcceso = drH.GetString(2),
                                    DataKey = drH.GetString(3)
                                });
                            }
                        }
                    }
                }
            }
            return fam;
        }

        public List<Familia> GetAll()
        {
            List<Familia> lista = new List<Familia>();
            string query = "SELECT Id, Nombre FROM Familia";
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new Familia { Id = dr.GetGuid(0), Nombre = dr.GetString(1) });
                    }
                }
            }
            return lista;
        }
    }
}
