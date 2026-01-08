using Service.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Impl
{    /// <summary>
     /// Clase ConsoleLogger: Implementación de un logger que no realiza ninguna acción.
     /// Se utiliza como logger por defecto para evitar errores si no se proporciona un logger real.
     /// Esta clase es útil cuando no se desea realizar registros de logs en ciertos entornos o configuraciones.
     /// </summary>
    public class ConsoleRepository : ILogger
    {
        private static readonly ConsoleRepository _instance = new ConsoleRepository();
        private static string Format(string level, string msg) 
        {
            string now = DateTime.UtcNow.ToString();

            return $"[{now}] - {msg}";
        }

        /// <summary>
        /// Instancia única de la clase <see cref="ConsoleRepository"/>. Utilizada como el logger por defecto.
        /// </summary>
        public static ConsoleRepository Instance => _instance;

        private ConsoleRepository() { }

        /// <summary>
        /// Método para registrar información. No realiza ninguna acción.
        /// </summary>
        /// <param name="message">Mensaje informativo a registrar.</param>
        public void Info(string message)
        {
            Console.WriteLine(Format("INFO", message));
        }

        /// <summary>
        /// Método para registrar errores. No realiza ninguna acción.
        /// </summary>
        /// <param name="message">Mensaje de error.</param>
        /// <param name="ex">Excepción asociada al error (opcional).</param>
        public void Error(string message, Exception ex = null)
        {
            Console.WriteLine(Format("ERROR", message + ex.Message));
        }
    }
}
