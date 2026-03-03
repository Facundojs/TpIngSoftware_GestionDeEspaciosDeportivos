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
    public class BitacoraService
    {
        private readonly LogRepository _repository;

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
