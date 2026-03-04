using Domain;
using Service.Factory;
using Service.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Logic
{
    /// <summary>
    /// Application-wide audit log service. All BLL operations that have user-visible side effects
    /// call this service to record INFO, WARNING, or ERROR entries.
    /// </summary>
    /// <remarks>
    /// If the underlying <c>LogRepository</c> fails to initialize (e.g., database unavailable),
    /// the service falls back to <see cref="Console.WriteLine"/> so that logging failures never
    /// propagate to callers.
    /// </remarks>
    public class BitacoraService
    {
        private readonly LogRepository _repository;

        /// <summary>
        /// Initializes the service, resolving the log repository from the service factory.
        /// Silently degrades to console fallback if the repository cannot be constructed.
        /// </summary>
        public BitacoraService()
        {
            try
            {
                _repository = FactoryDao.LogRepository;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing log repository: {ex.Message}");
            }
        }

        /// <summary>
        /// Writes a log entry. Swallows any internal errors to avoid disrupting callers.
        /// </summary>
        /// <param name="message">The message to record.</param>
        /// <param name="level">Severity level: <c>"INFO"</c>, <c>"WARNING"</c>, or <c>"ERROR"</c>. Defaults to <c>"INFO"</c>.</param>
        /// <param name="ex">Optional exception to attach when <paramref name="level"/> is <c>"ERROR"</c>.</param>
        public void Log(string message, string level = "INFO", Exception ex = null)
        {
            try
            {
                if (_repository == null)
                {
                    Console.WriteLine($"FALLBACK: {level} - {message} {(ex != null ? ex.ToString() : "")}");
                    return;
                }

                if (level.ToUpper() == "ERROR")
                {
                    _repository.Error(message, ex);
                }
                else
                {
                    _repository.Info(message);
                }
            }
            catch (Exception logEx)
            {
                Console.WriteLine($"Fatal error writing to log: {logEx.Message}. Original message: {message}");
            }
        }

        /// <summary>
        /// Retrieves paginated log entries, optionally filtered by date range, level, and message text.
        /// </summary>
        /// <param name="page">1-based page number.</param>
        /// <param name="pageSize">Number of entries per page.</param>
        /// <param name="from">Optional lower bound on the log entry timestamp.</param>
        /// <param name="to">Optional upper bound on the log entry timestamp.</param>
        /// <param name="logLevel">Optional level filter (e.g., <c>"ERROR"</c>).</param>
        /// <param name="message">Optional substring filter applied to the message text.</param>
        /// <returns>A list of matching <see cref="Log"/> entries; empty list if the repository is unavailable.</returns>
        public List<Log> GetLogs(int page, int pageSize, DateTime? from = null, DateTime? to = null, string logLevel = null, string message = null)
        {
            if (_repository == null)
            {
                return new List<Log>();
            }
            return _repository.GetLogs(page, pageSize, from, to, logLevel, message);
        }
    }
}
