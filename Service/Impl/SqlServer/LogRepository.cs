using Service.Contracts;
using Service.Helpers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
