using Domain.Composite;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
                        using (var cmd = new SqlCommand("UPDATE Familia SET Nombre = @Nom WHERE Id = @Id", conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@Nom", obj.Nombre);
                            cmd.Parameters.AddWithValue("@Id", obj.Id);
                            cmd.ExecuteNonQuery();
                        }

                        using (var cmd = new SqlCommand("DELETE FROM FamiliaPatente WHERE IdFamilia = @Id", conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@Id", obj.Id);
                            cmd.ExecuteNonQuery();
                        }

                        using (var cmd = new SqlCommand("DELETE FROM FamiliaFamilia WHERE IdFamiliaPadre = @Id", conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@Id", obj.Id);
                            cmd.ExecuteNonQuery();
                        }

                        foreach (var hijo in obj.Accesos)
                        {
                            if (hijo is Patente)
                            {
                                using (var cmd = new SqlCommand("INSERT INTO FamiliaPatente (IdFamilia, IdPatente) VALUES (@IdF, @IdP)", conn, tran))
                                {
                                    cmd.Parameters.AddWithValue("@IdF", obj.Id);
                                    cmd.Parameters.AddWithValue("@IdP", hijo.Id);
                                    cmd.ExecuteNonQuery();
                                }
                            }
                            else if (hijo is Familia)
                            {
                                using (var cmd = new SqlCommand("INSERT INTO FamiliaFamilia (IdFamiliaPadre, IdFamiliaHija) VALUES (@IdP, @IdH)", conn, tran))
                                {
                                    cmd.Parameters.AddWithValue("@IdP", obj.Id);
                                    cmd.Parameters.AddWithValue("@IdH", hijo.Id);
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }

                        tran.Commit();
                    }
                    catch { tran.Rollback(); throw; }
                }
            }
        }

        public void Remove(Guid id)
        {
            ExecuteNonQuery("DELETE FROM UsuarioFamilia WHERE IdFamilia = @Id", new[] { new SqlParameter("@Id", id) });
            ExecuteNonQuery("DELETE FROM FamiliaFamilia WHERE IdFamiliaPadre = @Id", new[] { new SqlParameter("@Id", id) });
            ExecuteNonQuery("DELETE FROM FamiliaFamilia WHERE IdFamiliaHija = @Id", new[] { new SqlParameter("@Id", id) });
            ExecuteNonQuery("DELETE FROM FamiliaPatente WHERE IdFamilia = @Id", new[] { new SqlParameter("@Id", id) });
            ExecuteNonQuery("DELETE FROM Familia WHERE Id = @Id", new[] { new SqlParameter("@Id", id) });
        }

        public Familia GetById(Guid id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                return GetByIdInternal(id, conn, new HashSet<Guid>());
            }
        }

        private Familia GetByIdInternal(Guid id, SqlConnection conn, HashSet<Guid> visited)
        {
            if (!visited.Add(id)) return null;

            Familia fam = null;
            using (var cmd = new SqlCommand("SELECT Id, Nombre FROM Familia WHERE Id = @Id", conn))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                using (var dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                        fam = new Familia { Id = dr.GetGuid(0), Nombre = dr.GetString(1) };
                }
            }

            if (fam == null) return null;

            string queryPatentes = @"SELECT p.Id, p.Nombre, p.TipoAcceso, p.DataKey
                                     FROM Patente p
                                     INNER JOIN FamiliaPatente fp ON p.Id = fp.IdPatente
                                     WHERE fp.IdFamilia = @Id";
            using (var cmd = new SqlCommand(queryPatentes, conn))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        fam.Agregar(new Patente
                        {
                            Id = dr.GetGuid(0),
                            Nombre = dr.GetString(1),
                            TipoAcceso = dr.GetString(2),
                            DataKey = dr.GetString(3)
                        });
                    }
                }
            }

            var childIds = new List<Guid>();
            using (var cmd = new SqlCommand("SELECT IdFamiliaHija FROM FamiliaFamilia WHERE IdFamiliaPadre = @Id", conn))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                        childIds.Add(dr.GetGuid(0));
                }
            }

            foreach (var childId in childIds)
            {
                var childFam = GetByIdInternal(childId, conn, visited);
                if (childFam != null)
                    fam.Agregar(childFam);
            }

            return fam;
        }

        public List<Familia> GetAll()
        {
            var lista = new List<Familia>();
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("SELECT Id, Nombre FROM Familia", conn))
            {
                conn.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                        lista.Add(new Familia { Id = dr.GetGuid(0), Nombre = dr.GetString(1) });
                }
            }
            return lista;
        }
    }
}
