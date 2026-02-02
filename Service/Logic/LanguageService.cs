using Service.Contracts;
using Service.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Logic
{
    public class LanguageService
    {
        private readonly ILanguage _languageRepository;

        public LanguageService()
        {
            _languageRepository = FactoryDao.LanguageRepository;
        }

        public string Translate(string key)
        {
            return _languageRepository.Translate(key);
        }

        public Dictionary<string, string> GetLanguages()
        {
            return _languageRepository.GetLanguages();
        }

        public void SaveUserLanguage(string languageCode)
        {
            _languageRepository.SaveUserLanguage(languageCode);
        }

        public string LoadUserLanguage()
        {
            return _languageRepository.LoadUserLanguage();
        }
    }
}
