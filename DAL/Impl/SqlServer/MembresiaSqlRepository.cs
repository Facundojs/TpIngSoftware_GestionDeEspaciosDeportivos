using DAL.Contracts;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace DAL.Impl
{
    public class MembresiaSqlRepository : BaseBusinessSqlRepository, IMembresiaRepository
    {
        public MembresiaSqlRepository() : base()
        {
        }

        #region IGenericRepository Implementation

        public void Add(Membresia obj)
        {
            Add(obj, null, null);
        }

        public void Update(Membresia obj)
        {
            Update(obj, null, null);
        }

        public void Remove(Guid id)
        {
            Remove(id, null, null);
        }

        public Membresia GetById(Guid id)
        {
            return GetById(id, null, null);
        }

        public List<Membresia> GetAll()
        {
            return GetAll(null, null);
        }

        #endregion

        #region Custom Methods Implementation

        public Membresia GetByCodigo(int codigo)
        {
            return GetByCodigo(codigo, null, null);
        }

        public List<Membresia> ListarActivas()
        {
            return ListarActivas(null, null);
        }

        #endregion

        #region UoW Overloads

        public void Add(Membresia obj, SqlConnection conn = null, SqlTransaction tran = null)
        {
            string query = "INSERT INTO Membresia (Id, Codigo, Nombre, Precio, Regularidad, Activa, Detalle) VALUES (@Id, @Codigo, @Nombre, @Precio, @Regularidad, @Activa, @Detalle)";
            SqlParameter[] parameters = {
                new SqlParameter("@Id", obj.Id),
                new SqlParameter("@Codigo", obj.Codigo),
                new SqlParameter("@Nombre", obj.Nombre),
                new SqlParameter("@Precio", obj.Precio),
                new SqlParameter("@Regularidad", obj.Regularidad),
                new SqlParameter("@Activa", obj.Activa),
                new SqlParameter("@Detalle", (object)obj.Detalle ?? DBNull.Value)
            };
            ExecuteNonQuery(query, parameters, conn, tran);
        }

        public void Update(Membresia obj, SqlConnection conn = null, SqlTransaction tran = null)
        {
            string query = "UPDATE Membresia SET Codigo = @Codigo, Nombre = @Nombre, Precio = @Precio, Regularidad = @Regularidad, Activa = @Activa, Detalle = @Detalle WHERE Id = @Id";
             SqlParameter[] parameters = {
                new SqlParameter("@Id", obj.Id),
                new SqlParameter("@Codigo", obj.Codigo),
                new SqlParameter("@Nombre", obj.Nombre),
                new SqlParameter("@Precio", obj.Precio),
                new SqlParameter("@Regularidad", obj.Regularidad),
                new SqlParameter("@Activa", obj.Activa),
                new SqlParameter("@Detalle", (object)obj.Detalle ?? DBNull.Value)
            };
            ExecuteNonQuery(query, parameters, conn, tran);
        }

        public void Remove(Guid id, SqlConnection conn = null, SqlTransaction tran = null)
        {
            string query = "DELETE FROM Membresia WHERE Id = @Id";
            SqlParameter[] parameters = { new SqlParameter("@Id", id) };
            ExecuteNonQuery(query, parameters, conn, tran);
        }

        public Membresia GetById(Guid id, SqlConnection conn = null, SqlTransaction tran = null)
        {
            string query = "SELECT Id, Codigo, Nombre, Precio, Regularidad, Activa, Detalle FROM Membresia WHERE Id = @Id";
            SqlParameter[] parameters = { new SqlParameter("@Id", id) };

            return ExecuteReader(query, parameters, reader =>
            {
                if (reader.Read())
                {
                    return MapFromReader(reader);
                }
                return null;
            }, conn, tran);
        }

        public List<Membresia> GetAll(SqlConnection conn = null, SqlTransaction tran = null)
        {
            string query = "SELECT Id, Codigo, Nombre, Precio, Regularidad, Activa, Detalle FROM Membresia ORDER BY Nombre";
             return ExecuteReader(query, null, reader =>
            {
                List<Membresia> list = new List<Membresia>();
                while (reader.Read())
                {
                    list.Add(MapFromReader(reader));
                }
                return list;
            }, conn, tran);
        }

        public Membresia GetByCodigo(int codigo, SqlConnection conn = null, SqlTransaction tran = null)
        {
            string query = "SELECT Id, Codigo, Nombre, Precio, Regularidad, Activa, Detalle FROM Membresia WHERE Codigo = @Codigo";
            SqlParameter[] parameters = { new SqlParameter("@Codigo", codigo) };

             return ExecuteReader(query, parameters, reader =>
            {
                if (reader.Read())
                {
                    return MapFromReader(reader);
                }
                return null;
            }, conn, tran);
        }

        public List<Membresia> ListarActivas(SqlConnection conn = null, SqlTransaction tran = null)
        {
            string query = "SELECT Id, Codigo, Nombre, Precio, Regularidad, Activa, Detalle FROM Membresia WHERE Activa = 1 ORDER BY Nombre";
             return ExecuteReader(query, null, reader =>
            {
                List<Membresia> list = new List<Membresia>();
                while (reader.Read())
                {
                    list.Add(MapFromReader(reader));
                }
                return list;
            }, conn, tran);
        }

        #endregion

        private Membresia MapFromReader(SqlDataReader reader)
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
    }
}
