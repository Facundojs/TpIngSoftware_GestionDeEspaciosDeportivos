using Service.Helpers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Impl
{
    /// <summary>
    /// Abstract base class for all SQL Server repositories, providing low-level ADO.NET helpers.
    /// </summary>
    /// <remarks>
    /// Each helper opens its own <see cref="SqlConnection"/> unless a shared <paramref name="conn"/>
    /// is injected (used by <c>DAL.Impl.BaseBusinessSqlRepository</c> when enrolled in a
    /// <see cref="DAL.Impl.SqlServer.UnitOfWork"/> transaction).
    /// </remarks>
    public abstract class BaseRepository
    {
        /// <summary>Raw ADO.NET connection string resolved from <see cref="ConnectionManager"/>.</summary>
        protected readonly string _connectionString;

        /// <summary>
        /// Initializes the repository with the connection string identified by <paramref name="connectionStringName"/>.
        /// </summary>
        /// <param name="connectionStringName">
        /// Key used to look up the connection string via <see cref="ConnectionManager.GetConnectionString"/>.
        /// Defaults to <see cref="ConnectionManager.BaseConnectionName"/>.
        /// </param>
        public BaseRepository(string connectionStringName = ConnectionManager.BaseConnectionName) => _connectionString = ConnectionManager.GetConnectionString(connectionStringName);

        /// <summary>
        /// Executes a non-query SQL statement (INSERT, UPDATE, DELETE).
        /// </summary>
        /// <param name="query">Parameterized SQL statement.</param>
        /// <param name="parameters">SQL parameters to bind, or <c>null</c> for none.</param>
        /// <param name="conn">
        /// Optional shared connection. When provided, the command runs on this connection
        /// using <paramref name="tran"/> (if any), without opening a new connection.
        /// </param>
        /// <param name="tran">Optional transaction to enlist the command in.</param>
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

        /// <summary>
        /// Executes a SELECT query and projects the result via a mapping delegate.
        /// Always opens a new connection; use <c>BaseBusinessSqlRepository</c> overload for UoW reads.
        /// </summary>
        /// <typeparam name="T">The projection type returned by <paramref name="map"/>.</typeparam>
        /// <param name="query">Parameterized SQL SELECT statement.</param>
        /// <param name="parameters">SQL parameters to bind, or <c>null</c> for none.</param>
        /// <param name="map">Delegate that receives the open <see cref="SqlDataReader"/> and returns <typeparamref name="T"/>.</param>
        /// <returns>The value produced by <paramref name="map"/>.</returns>
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
        /// Executes a scalar query and returns the first column of the first row.
        /// </summary>
        /// <typeparam name="T">The expected return type. <c>DBNull</c> is converted to <c>default(T)</c>.</typeparam>
        /// <param name="query">Parameterized SQL statement.</param>
        /// <param name="parameters">SQL parameters to bind, or <c>null</c> for none.</param>
        /// <returns>The scalar value cast to <typeparamref name="T"/>, or <c>default(T)</c> if the result is NULL.</returns>
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
