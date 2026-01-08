using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Contracts
{
    interface ILogger
    {
        /// <summary>
        /// Registra un mensaje de información en los logs.
        /// </summary>
        /// <param name="message">El mensaje informativo a registrar.</param>
        /// <example>
        /// Ejemplo de uso:
        /// <code>
        /// ILogger logger = new Logger();
        /// logger.Info("Este es un mensaje informativo.");
        /// </code>
        /// </example>
        void Info(string message);

        /// <summary>
        /// Registra un mensaje de error en los logs con información opcional de la excepción asociada.
        /// </summary>
        /// <param name="message">El mensaje de error a registrar.</param>
        /// <param name="ex">La excepción asociada al error (opcional).</param>
        /// <example>
        /// Ejemplo de uso:
        /// <code>
        /// ILogger logger = new Logger();
        /// logger.Error("Ha ocurrido un error en el sistema.", exception);
        /// </code>
        /// </example>
        void Error(string message, Exception ex = null);
    }
}
