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
        List<Familia> lista = new List<Familia>();
        string query = @"SELECT f.Id, f.Nombre FROM Familia f 
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
                    lista.Add(new Familia { 
                        Id = dr.GetGuid(0), 
                        Nombre = dr.GetString(1) 
                    });
                }
            }
        }
        return lista;
    }

    // Métodos restantes (GetAll, GetById, Update, Remove) seguirían la misma lógica Raw SQL...
    public List<Usuario> GetAll() => throw new NotImplementedException();
    public Usuario GetById(Guid id) => throw new NotImplementedException();
    public void Update(Usuario obj) => throw new NotImplementedException();
    public void Remove(Guid id) => throw new NotImplementedException();
    public List<UsuarioDTO> GetUsuariosDTO() => throw new NotImplementedException();
}
}
