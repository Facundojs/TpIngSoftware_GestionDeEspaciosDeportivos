using Service.Contracts;
using Service.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Impl.SqlServer
{
    internal class SqlServerBackup: IBackup
    {
        private readonly static string WorkDir = AppDomain.CurrentDomain.BaseDirectory;

        public void BackUpDataBase(string database, string path)
        {

            string fullBackUpPath = Path.Combine(WorkDir, path);

            string query1 = $@"CHECKPOINT; BACKUP DATABASE [{database}] TO DISK = @path WITH FORMAT, INIT";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@path", fullBackUpPath)
            };

            SqlHelper.ExecuteNonQuery(query1, CommandType.Text, parameters);
        }

        public void RestoreDataBase(string database, string path)
        {
            string fullBackUpPath = Path.Combine(WorkDir, path);

            string query = $@"ALTER DATABASE [{database}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                            RESTORE DATABASE [{database}] FROM DISK = @path WITH REPLACE;
                            ALTER DATABASE [{database}] SET MULTI_USER;";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@path", fullBackUpPath)
            };

            // Cambia la cadena de conexión para conectarse a la base de datos master.
            string masterConnString = SqlHelper.conString.Replace($"Initial Catalog={database};", "Initial Catalog=master;");

            try
            {
                using (SqlConnection conn = new SqlConnection(masterConnString))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddRange(parameters);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new InvalidOperationException($"Error realizando el restore: {ex.Message}", ex);
            }
        }
    }
}
