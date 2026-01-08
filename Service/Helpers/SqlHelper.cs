using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Helpers
{
    /// <summary>
    /// Clase estática que proporciona métodos de ayuda para la ejecución de comandos SQL en una base de datos SQL Server.
    /// </summary>
    internal static class SqlHelper
    {
        // Cadena de conexión a la base de datos.
        public readonly static string conString;

        // Constructor estático que inicializa la cadena de conexión.
        static SqlHelper()
        {
            conString = ConnectionManager.GetConnectionString();
        }

        /// <summary>
        /// Ejecuta un comando SQL que no devuelve filas (INSERT, UPDATE, DELETE).
        /// </summary>
        public static Int32 ExecuteNonQuery(String commandText,
            CommandType commandType, params SqlParameter[] parameters)
        {
            CheckNullables(parameters);

            using (SqlConnection conn = new SqlConnection(conString))
            {
                using (SqlCommand cmd = new SqlCommand(commandText, conn))
                {
                    cmd.CommandType = commandType;
                    cmd.Parameters.AddRange(parameters);

                    conn.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Verifica si los parámetros son nulos y los establece en DBNull.Value si es necesario.
        /// </summary>
        private static void CheckNullables(SqlParameter[] parameters)
        {
            foreach (SqlParameter item in parameters)
            {
                if (item.SqlValue == null)
                {
                    item.SqlValue = DBNull.Value;
                }
            }
        }

        /// <summary>
        /// Ejecuta un comando SQL y devuelve un solo valor (por ejemplo, un conteo o un ID).
        /// </summary>
        public static Object ExecuteScalar(String commandText,
            CommandType commandType, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(conString))
            {
                using (SqlCommand cmd = new SqlCommand(commandText, conn))
                {
                    cmd.CommandType = commandType;
                    cmd.Parameters.AddRange(parameters);

                    conn.Open();
                    return cmd.ExecuteScalar();
                }
            }
        }

        /// <summary>
        /// Ejecuta un comando SQL y devuelve un SqlDataReader para leer los resultados.
        /// </summary>
        public static SqlDataReader ExecuteReader(String commandText,
            CommandType commandType, params SqlParameter[] parameters)
        {
            SqlConnection conn = new SqlConnection(conString);

            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                cmd.CommandType = commandType;
                cmd.Parameters.AddRange(parameters);

                conn.Open();
                // Cuando se usa CommandBehavior.CloseConnection, la conexión se cerrará cuando el IDataReader se cierre.
                SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                return reader;
            }
        }

        /// <summary>
        /// Ejecuta un comando SQL y devuelve un DataTable con los resultados.
        /// </summary>
        public static DataTable ExecuteDataTable(string commandText, CommandType commandType, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(conString))
            {
                using (SqlCommand cmd = new SqlCommand(commandText, conn))
                {
                    cmd.CommandType = commandType;

                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);  // Llenar el DataTable con los resultados de la consulta
                        return dt;
                    }
                }
            }
        }
    }
}
