using Service.Helpers;
using Service.Impl;
using System;
using System.Data.SqlClient;

namespace DAL.Impl
{
    public abstract class BaseBusinessSqlRepository : BaseRepository
    {
        internal SqlConnection CurrentConnection { get; set; }
        internal SqlTransaction CurrentTransaction { get; set; }

        public BaseBusinessSqlRepository() : base(ConnectionManager.BusinessConnectionName)
        {
        }

        protected override void ExecuteNonQuery(string query, SqlParameter[] parameters, SqlConnection conn = null, SqlTransaction tran = null)
        {
            base.ExecuteNonQuery(query, parameters, conn ?? CurrentConnection, tran ?? CurrentTransaction);
        }

        protected override T ExecuteReader<T>(string query, SqlParameter[] parameters, Func<SqlDataReader, T> map)
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

        protected override T ExecuteScalar<T>(string query, SqlParameter[] parameters)
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
