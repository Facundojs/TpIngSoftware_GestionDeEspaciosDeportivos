using Service.Contracts;
using Service.DTO;
using Service.Factory;
using Service.Impl.SqlServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Logic
{
    public class BackupService
    {
        private readonly IBackup _backupRepository;

        public BackupService()
        {
            _backupRepository = FactoryDao.BackupRepository;
        }

        public void Backup(string database, string path)
        {
            _backupRepository.BackUpDataBase(database, path);
        }

        public void Restore(string database, string path)
        {
            _backupRepository.RestoreDataBase(database, path);
        }

        public List<BackupFile> ListBackups()
        {
            return _backupRepository.ListBackups();
        }

        public void DeleteBackup(string filename)
        {
            _backupRepository.DeleteBackup(filename);
        }
    }
}
