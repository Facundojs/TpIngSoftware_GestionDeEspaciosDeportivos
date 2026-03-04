using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Contracts
{
    /// <summary>
    /// Abstraction for structured application logging.
    /// </summary>
    interface ILogger
    {
        /// <summary>
        /// Records an informational message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        void Info(string message);

        /// <summary>
        /// Records an error message with an optional associated exception.
        /// </summary>
        /// <param name="message">Descriptive error message.</param>
        /// <param name="ex">The exception that caused the error, or <c>null</c> if not applicable.</param>
        void Error(string message, Exception ex = null);
    }
}
