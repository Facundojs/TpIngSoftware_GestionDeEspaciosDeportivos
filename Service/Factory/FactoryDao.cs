using Service.Contracts;
using Service.Impl;
using Service.Impl.SqlServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Factory
{
    public static class FactoryDao
    {
        private static IUsuarioRepository _usuarioRepository;
        private static LogRepository _logRepository;
        private static IBackup _backupRepository;
        private static ILanguage _languageRepository;

        public static IUsuarioRepository UsuarioRepository
        {
            get
            {
                if (_usuarioRepository == null)
                {
                    _usuarioRepository = new UsuarioRepository();
                }
                return _usuarioRepository;
            }
        }

        public static LogRepository LogRepository
        {
            get
            {
                if (_logRepository == null)
                {
                    _logRepository = new LogRepository();
                }
                return _logRepository;
            }
        }

        public static IBackup BackupRepository
        {
            get
            {
                if (_backupRepository == null)
                {
                    _backupRepository = new SqlServerBackup();
                }
                return _backupRepository;
            }
        }

        public static ILanguage LanguageRepository
        {
            get
            {
                if (_languageRepository == null)
                {
                    _languageRepository = new Service.Impl.Text.LanguageManager();
                }
                return _languageRepository;
            }
        }
    }
}
