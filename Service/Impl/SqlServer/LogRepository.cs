using Service.Contracts;
using Service.Helpers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;

namespace Service.Impl
{
    /// <summary>
    /// Clase PersistentLogger para manejar el registro de información y errores en una base de datos.
    /// Esta clase permite registrar logs de nivel informativo y de errores en la base de datos,
    /// y proporciona un método de respaldo en caso de que la base de datos no esté disponible.
    /// </summary>
    public class LogRepository : BaseRepository, ILogger
    {
        private static readonly ILogger defaultLogger = ConsoleRepository.Instance;

        public void Info(string message) => InsertLog("INFO", message, null);

        public void Error(string message, Exception ex) => InsertLog("ERROR", message, ex?.ToString());

        /// <summary>
        /// Inserta un registro en la base de datos en la tabla de bitácora.
        /// </summary>
        /// <param name="logLevel">Nivel del log ("Info" o "Error").</param>
        /// <param name="message">Mensaje del registro.</param>
        /// <param name="exceptionDetails">Detalles de la excepción (opcional).</param>
        private void InsertLog(string logLevel, string message, string exceptionDetails)
        {
            string query = "INSERT INTO Bitacora (BitacoraID, Timestamp, LogLevel, Message, ExceptionDetails) " +
                           "VALUES (@BitacoraID, @Timestamp, @LogLevel, @Message, @ExceptionDetails)";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@BitacoraID", Guid.NewGuid()),
                new SqlParameter("@Timestamp", DateTime.Now),
                new SqlParameter("@LogLevel", logLevel),
                new SqlParameter("@Message", message),
                new SqlParameter("@ExceptionDetails", (object)exceptionDetails ?? DBNull.Value)
            };
            try { 
            ExecuteNonQuery(query, parameters);
            }
            catch (Exception ex)
            {
                defaultLogger.Error($"Error al escribir en la bitácora: {ex.Message}\n{ex.StackTrace}");
            }

        }

        /// <summary>
        /// Obtiene una lista de logs de la base de datos de forma paginada y filtrada.
        /// </summary>
        public List<Log> GetLogs(int pageNumber, int pageSize, DateTime? from = null, DateTime? to = null, string logLevel = null, string message = null)
        {
            int offset = (pageNumber - 1) * pageSize;

            var sb = new StringBuilder();
            sb.Append("SELECT BitacoraID, Timestamp, LogLevel, Message, ExceptionDetails FROM Bitacora WHERE 1=1 ");

            var parameters = new List<SqlParameter>();

            if (from.HasValue)
            {
                sb.Append("AND Timestamp >= @From ");
                parameters.Add(new SqlParameter("@From", from.Value));
            }
            if (to.HasValue)
            {
                sb.Append("AND Timestamp <= @To ");
                parameters.Add(new SqlParameter("@To", to.Value));
            }
            if (!string.IsNullOrEmpty(logLevel) && !logLevel.Equals("Todos", StringComparison.OrdinalIgnoreCase))
            {
                sb.Append("AND LogLevel = @LogLevel ");
                parameters.Add(new SqlParameter("@LogLevel", logLevel));
            }
            if (!string.IsNullOrEmpty(message))
            {
                sb.Append("AND Message LIKE @Message ");
                parameters.Add(new SqlParameter("@Message", "%" + message + "%"));
            }

            sb.Append("ORDER BY Timestamp DESC OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY");

            parameters.Add(new SqlParameter("@Offset", offset));
            parameters.Add(new SqlParameter("@PageSize", pageSize));

            return ExecuteReader(sb.ToString(), parameters.ToArray(), reader =>
            {
                var list = new List<Log>();
                while (reader.Read())
                {
                    list.Add(new Log
                    {
                        BitacoraID = reader.GetGuid(reader.GetOrdinal("BitacoraID")),
                        Timestamp = reader.GetDateTime(reader.GetOrdinal("Timestamp")),
                        LogLevel = reader.GetString(reader.GetOrdinal("LogLevel")),
                        Message = reader.GetString(reader.GetOrdinal("Message")),
                        ExceptionDetails = reader.IsDBNull(reader.GetOrdinal("ExceptionDetails"))
                                           ? null
                                           : reader.GetString(reader.GetOrdinal("ExceptionDetails"))
                    });
                }
                return list;
            });
        }
    }
}
