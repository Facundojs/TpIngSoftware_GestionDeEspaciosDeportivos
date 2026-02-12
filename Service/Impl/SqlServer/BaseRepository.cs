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
        public BaseRepository(string connectionStringName = ConnectionManager.BaseConnectionName) => _connectionString = ConnectionManager.GetConnectionString(connectionStringName);


        /// <summary>
        /// Ejecuta una consulta SQL que no devuelve datos (INSERT, UPDATE, DELETE).
        /// </summary>
        /// <param name="query">Consulta SQL a ejecutar.</param>
        /// <param name="parameters">Parámetros de la consulta.</param>
        /// <param name="conn">Conexión SQL opcional (para transacciones).</param>
        /// <param name="tran">Transacción SQL opcional.</param>
        protected void ExecuteNonQuery(string query, SqlParameter[] parameters, SqlConnection conn = null, SqlTransaction tran = null)
        {
            if (conn != null)
            {
                // Usar conexión existente (sin dispose)
                using (var cmd = new SqlCommand(query, conn, tran))
                {
                    if (parameters != null) cmd.Parameters.AddRange(parameters);
                    cmd.ExecuteNonQuery();
                }
            }
            else
            {
                // Crear nueva conexión
                using (var connection = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand(query, connection))
                {
                    if (parameters != null) cmd.Parameters.AddRange(parameters);
                    connection.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Ejecuta una consulta SQL que devuelve un conjunto de resultados y los mapea a un objeto o lista.
        /// </summary>
        /// <typeparam name="T">El tipo de objeto que se espera retornar.</typeparam>
        /// <param name="query">Consulta SQL (SELECT) a ejecutar.</param>
        /// <param name="parameters">Arreglo de parámetros SQL para evitar SQL Injection.</param>
        /// <param name="map">Función delegada que define cómo leer cada fila del DataReader y transformarla en el tipo T.</param>
        /// <returns>Un objeto de tipo T con los datos recuperados.</returns>
        protected T ExecuteReader<T>(string query, SqlParameter[] parameters, Func<SqlDataReader, T> map)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                if (parameters != null) cmd.Parameters.AddRange(parameters);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    return map(reader);
                }
            }
        }

        /// <summary>
        /// Ejecuta una consulta SQL que devuelve un solo valor (escalar).
        /// </summary>
        /// <typeparam name="T">El tipo de dato esperado.</typeparam>
        /// <param name="query">Consulta SQL (SELECT COUNT, SELECT MAX, etc).</param>
        /// <param name="parameters">Parámetros SQL.</param>
        /// <returns>El valor escalar obtenido.</returns>
        protected T ExecuteScalar<T>(string query, SqlParameter[] parameters)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                if (parameters != null) cmd.Parameters.AddRange(parameters);
                conn.Open();
                object result = cmd.ExecuteScalar();
                if (result == null || result == DBNull.Value) return default(T);
                return (T)Convert.ChangeType(result, typeof(T));
            }
        }
    }
}
