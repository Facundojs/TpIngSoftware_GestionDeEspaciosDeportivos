using Service.Contracts;
using Service.DTO;
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
        private readonly static string BackupDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backups");

        public void BackUpDataBase(string database, string path)
        {
            if (!Directory.Exists(BackupDir))
            {
                Directory.CreateDirectory(BackupDir);
            }

            // Sanitize filename to prevent path traversal
            string filename = Path.GetFileName(path);
            string fullBackUpPath = Path.Combine(BackupDir, filename);

            string query1 = $@"CHECKPOINT; BACKUP DATABASE [{database}] TO DISK = @path WITH FORMAT, INIT";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@path", fullBackUpPath)
            };

            SqlHelper.ExecuteNonQuery(query1, CommandType.Text, parameters);
        }

        public void RestoreDataBase(string database, string path)
        {
            // Sanitize filename to prevent path traversal
            string filename = Path.GetFileName(path);
            string fullBackUpPath = Path.Combine(BackupDir, filename);

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

        public List<BackupFile> ListBackups()
        {
            if (!Directory.Exists(BackupDir))
            {
                return new List<BackupFile>();
            }

            return Directory.GetFiles(BackupDir)
                            .Select(path => new BackupFile
                            {
                                Nombre = Path.GetFileName(path),
                                Fecha = File.GetLastWriteTime(path).ToString("o"),
                                FileSize = new FileInfo(path).Length
                            })
                            .ToList();
        }

        public void DeleteBackup(string filename)
        {
            // Sanitize filename to prevent path traversal
            string safeFilename = Path.GetFileName(filename);
            string fullPath = Path.Combine(BackupDir, safeFilename);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
            else
            {
                throw new FileNotFoundException("El archivo de backup no existe.", safeFilename);
            }
        }
    }
}
