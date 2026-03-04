using Service.Helpers;
using Service.Impl;
using System;
using System.Data.SqlClient;

namespace DAL.Impl
{
    /// <summary>
    /// Base class for all business-domain SQL repositories.
    /// Extends <see cref="BaseRepository"/> with support for shared <see cref="SqlConnection"/>
    /// and <see cref="SqlTransaction"/> injection by <see cref="DAL.Impl.SqlServer.UnitOfWork"/>.
    /// </summary>
    /// <remarks>
    /// When <see cref="CurrentConnection"/> is set (i.e., the repository is enrolled in a UoW
    /// transaction), the shadowed helper methods use the shared connection/transaction instead of
    /// opening a new one. When not enrolled, they fall back to the base class behaviour.
    /// </remarks>
    public class BaseBusinessSqlRepository : BaseRepository
    {
        /// <summary>
        /// Shared connection injected by <see cref="DAL.Impl.SqlServer.UnitOfWork.BeginTransaction"/>.
        /// <c>null</c> when the repository is used outside of a transaction.
        /// </summary>
        internal SqlConnection CurrentConnection { get; set; }

        /// <summary>
        /// Active transaction injected by <see cref="DAL.Impl.SqlServer.UnitOfWork.BeginTransaction"/>.
        /// <c>null</c> when the repository is used outside of a transaction.
        /// </summary>
        internal SqlTransaction CurrentTransaction { get; set; }

        /// <summary>
        /// Initializes the repository using the <see cref="ConnectionManager.BusinessConnectionName"/> connection string.
        /// </summary>
        public BaseBusinessSqlRepository() : base(ConnectionManager.BusinessConnectionName)
        {
        }

        /// <summary>
        /// Executes a non-query, routing through the active UoW connection/transaction when available.
        /// </summary>
        protected new void ExecuteNonQuery(string query, SqlParameter[] parameters)
        {
            base.ExecuteNonQuery(query, parameters, CurrentConnection, CurrentTransaction);
        }

        /// <summary>
        /// Executes a reader query on the active UoW connection when enrolled; otherwise opens a new connection.
        /// </summary>
        protected new T ExecuteReader<T>(string query, SqlParameter[] parameters, Func<SqlDataReader, T> map)
        {
            if (CurrentConnection != null)
            {
                using (var cmd = new SqlCommand(query, CurrentConnection, CurrentTransaction))
                {
                    if (parameters != null) cmd.Parameters.AddRange(parameters);
                    using (var reader = cmd.ExecuteReader())
                    {
                        return map(reader);
                    }
                }
            }
            return base.ExecuteReader(query, parameters, map);
        }

        /// <summary>
        /// Executes a scalar query on the active UoW connection when enrolled; otherwise opens a new connection.
        /// </summary>
        protected new T ExecuteScalar<T>(string query, SqlParameter[] parameters)
        {
            if (CurrentConnection != null)
            {
                using (var cmd = new SqlCommand(query, CurrentConnection, CurrentTransaction))
                {
                    if (parameters != null) cmd.Parameters.AddRange(parameters);
                    object result = cmd.ExecuteScalar();
                    if (result == null || result == DBNull.Value) return default(T);
                    return (T)Convert.ChangeType(result, typeof(T));
                }
            }
            return base.ExecuteScalar<T>(query, parameters);
        }
    }
}
