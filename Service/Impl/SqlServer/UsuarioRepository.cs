using Service.Domain.Composite;
using Service.Domain;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Service.Helpers;
using Service.Contracts;
using Service.DTO;

namespace Service.Impl
{
public class UsuarioRepository : BaseRepository, IUsuarioRepository
{
    public void Add(Usuario obj)
    {
        string query = "INSERT INTO Usuario (Id, NombreUsuario, Password, Estado, DigitoVerificador) VALUES (@Id, @User, @Pass, @Est, @DV)";
        SqlParameter[] p = {
            new SqlParameter("@Id", obj.Id),
            new SqlParameter("@User", obj.NombreUsuario),
            new SqlParameter("@Pass", obj.Password),
            new SqlParameter("@Est", obj.Estado),
            new SqlParameter("@DV", obj.DigitoVerificador)
        };
        ExecuteNonQuery(query, p);
    }

    public void UpdateAccesos(Guid idUsuario, List<Acceso> accesos)
    {
        // En un esquema Composite, solemos limpiar y re-insertar las relaciones
        using (var conn = new SqlConnection(_connectionString))
        {
            conn.Open();
            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    // 1. Borrar relaciones actuales
                    string delete = "DELETE FROM UsuarioFamilia WHERE IdUsuario = @Id";
                    SqlCommand cmdDel = new SqlCommand(delete, conn, tran);
                    cmdDel.Parameters.AddWithValue("@Id", idUsuario);
                    cmdDel.ExecuteNonQuery();

                    // 2. Insertar nuevos (solo Familias en este ejemplo de tabla intermedia)
                    foreach (var acceso in accesos.OfType<Familia>())
                    {
                        string ins = "INSERT INTO UsuarioFamilia (IdUsuario, IdFamilia) VALUES (@IdU, @IdF)";
                        SqlCommand cmdIns = new SqlCommand(ins, conn, tran);
                        cmdIns.Parameters.AddWithValue("@IdU", idUsuario);
                        cmdIns.Parameters.AddWithValue("@IdF", acceso.Id);
                        cmdIns.ExecuteNonQuery();
                    }
                    tran.Commit();
                }
                catch { tran.Rollback(); throw; }
            }
        }
    }

    public List<Familia> GetFamiliasByUsuarioId(Guid usuarioId)
    {
        var ids = new List<Guid>();
        string query = @"SELECT f.Id FROM Familia f
                         INNER JOIN UsuarioFamilia uf ON f.Id = uf.IdFamilia 
                         WHERE uf.IdUsuario = @Id";
        
        using (var conn = new SqlConnection(_connectionString))
        using (var cmd = new SqlCommand(query, conn))
        {
            cmd.Parameters.AddWithValue("@Id", usuarioId);
            conn.Open();
            using (var dr = cmd.ExecuteReader())
            {
                while (dr.Read())
                {
                    ids.Add(dr.GetGuid(0));
                }
            }
        }

        var famRepo = new FamiliaRepository();
        var lista = new List<Familia>();
        foreach (var id in ids)
        {
            var fam = famRepo.GetById(id);
            if (fam != null) lista.Add(fam);
        }
        return lista;
    }

    // Métodos restantes (GetAll, GetById, Update, Remove) seguirían la misma lógica Raw SQL...
    public List<Usuario> GetAll()
    {
        string query = "SELECT Id, NombreUsuario, Password, Estado, DigitoVerificador FROM Usuario";
        return ExecuteReader(query, null, reader =>
        {
            var list = new List<Usuario>();
            while (reader.Read())
            {
                list.Add(new Usuario
                {
                    Id = reader.GetGuid(0),
                    NombreUsuario = reader.GetString(1),
                    Password = reader.GetString(2),
                    Estado = reader.GetBoolean(3),
                    DigitoVerificador = reader.GetString(4)
                });
            }
            return list;
        });
    }

    public Usuario GetById(Guid id)
    {
        string query = "SELECT Id, NombreUsuario, Password, Estado, DigitoVerificador FROM Usuario WHERE Id = @Id";
        SqlParameter[] p = { new SqlParameter("@Id", id) };
        return ExecuteReader(query, p, reader =>
        {
            if (reader.Read())
            {
                var u = new Usuario
                {
                    Id = reader.GetGuid(0),
                    NombreUsuario = reader.GetString(1),
                    Password = reader.GetString(2),
                    Estado = reader.GetBoolean(3),
                    DigitoVerificador = reader.GetString(4)
                };
                // Fill permissions
                u.Permisos.AddRange(GetFamiliasByUsuarioId(u.Id));
                return u;
            }
            return null;
        });
    }

    public Usuario GetByUsername(string username)
    {
        string query = "SELECT Id, NombreUsuario, Password, Estado, DigitoVerificador FROM Usuario WHERE NombreUsuario = @User";
        SqlParameter[] p = { new SqlParameter("@User", username) };
        return ExecuteReader(query, p, reader =>
        {
            if (reader.Read())
            {
                var u = new Usuario
                {
                    Id = reader.GetGuid(0),
                    NombreUsuario = reader.GetString(1),
                    Password = reader.GetString(2),
                    Estado = reader.GetBoolean(3),
                    DigitoVerificador = reader.GetString(4)
                };
                // Fill permissions
                u.Permisos.AddRange(GetFamiliasByUsuarioId(u.Id));
                return u;
            }
            return null;
        });
    }

    public void Update(Usuario obj)
    {
        string query = "UPDATE Usuario SET NombreUsuario=@User, Password=@Pass, Estado=@Est, DigitoVerificador=@DV WHERE Id=@Id";
        SqlParameter[] p = {
            new SqlParameter("@Id", obj.Id),
            new SqlParameter("@User", obj.NombreUsuario),
            new SqlParameter("@Pass", obj.Password),
            new SqlParameter("@Est", obj.Estado),
            new SqlParameter("@DV", obj.DigitoVerificador)
        };
        ExecuteNonQuery(query, p);
    }

    public void Remove(Guid id)
    {
        string queryrel = "DELETE FROM UsuarioFamilia WHERE IdUsuario = @Id";
        ExecuteNonQuery(queryrel, new SqlParameter[] { new SqlParameter("@Id", id) });

        string query = "DELETE FROM Usuario WHERE Id = @Id";
        ExecuteNonQuery(query, new SqlParameter[] { new SqlParameter("@Id", id) });
    }

    public List<UsuarioDTO> GetUsuariosDTO()
    {
        string query = "SELECT Id, NombreUsuario, Estado FROM Usuario";
        return ExecuteReader(query, null, reader =>
        {
            var list = new List<UsuarioDTO>();
            while (reader.Read())
            {
                list.Add(new UsuarioDTO
                {
                    Id = reader.GetGuid(0),
                    Username = reader.GetString(1),
                    Estado = reader.GetBoolean(2) ? "Activo" : "Bloqueado"
                });
            }
            return list;
        });
    }
}
}
