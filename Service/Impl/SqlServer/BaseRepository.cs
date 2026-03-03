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

        protected void ExecuteNonQuery(string query, SqlParameter[] parameters, SqlConnection conn = null, SqlTransaction tran = null)
        {
            if (conn != null)
            {
                using (var cmd = new SqlCommand(query, conn, tran))
                {
                    if (parameters != null) cmd.Parameters.AddRange(parameters);
                    cmd.ExecuteNonQuery();
                }
            }
            else
            {
                using (var connection = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand(query, connection))
                {
                    if (parameters != null) cmd.Parameters.AddRange(parameters);
                    connection.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

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
