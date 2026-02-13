using Service.Helpers;
using Service.Impl;
using System;
using System.Data.SqlClient;

namespace DAL.Impl
{
    public class BaseBusinessSqlRepository : BaseRepository
    {
        public BaseBusinessSqlRepository() : base(ConnectionManager.BusinessConnectionName)
        {
        }

        protected T ExecuteReader<T>(string query, SqlParameter[] parameters, Func<SqlDataReader, T> map, SqlConnection conn, SqlTransaction tran)
        {
            if (conn != null)
            {
                // Use existing connection (UoW)
                using (var cmd = new SqlCommand(query, conn, tran))
                {
                    if (parameters != null) cmd.Parameters.AddRange(parameters);
                    using (var reader = cmd.ExecuteReader())
                    {
                        return map(reader);
                    }
                }
            }
            else
            {
                // Call base method which handles connection creation/opening
                return base.ExecuteReader(query, parameters, map);
            }
        }
    }
}
