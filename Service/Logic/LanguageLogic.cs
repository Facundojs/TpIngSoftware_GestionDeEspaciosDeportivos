using Service.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Logic
{
    internal static class LanguageLogic
    {
        public static string Translate(string key)
        {
            return FactoryDao.LanguageRepository.Translate(key);
        }

        public static Dictionary<string, string> GetLanguages()
        {
            return FactoryDao.LanguageRepository.GetLanguages();
        }

        public static void SaveUserLanguage(string languageCode)
        {
            FactoryDao.LanguageRepository.SaveUserLanguage(languageCode);
        }
    }
}
