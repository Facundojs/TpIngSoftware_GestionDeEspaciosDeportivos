using Service.Helpers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Impl
{
    public abstract class BaseRepository
    {
        protected readonly string _connectionString;
        public BaseRepository() => _connectionString = ConnectionManager.GetConnectionString();


        /// <summary>
        /// Ejecuta una consulta SQL que no devuelve datos (INSERT, UPDATE, DELETE).
        /// </summary>
        /// <param name="query">Consulta SQL a ejecutar.</param>
        /// <param name="parameters">Parámetros de la consulta.</param>
        protected void ExecuteNonQuery(string query, SqlParameter[] parameters)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddRange(parameters);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
