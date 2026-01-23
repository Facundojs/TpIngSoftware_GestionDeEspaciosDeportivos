using Service.Domain;
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
            _repository = FactoryDao.LogRepository;
        }

        public void Log(string message, string level = "INFO", Exception ex = null)
        {
            if (level.ToUpper() == "ERROR")
            {
                _repository.Error(message, ex);
            }
            else
            {
                _repository.Info(message);
            }
        }

        public List<Log> GetLogs(int page, int pageSize)
        {
            return _repository.GetLogs(page, pageSize);
        }
    }
}
