using Service.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Impl.Text
{
    public class LanguageManager : ILanguage
    {
        public string Translate(string key)
        {
            return LanguageRepository.Translate(key);
        }

        public Dictionary<string, string> GetLanguages()
        {
            return LanguageRepository.GetLanguages();
        }

        public void SaveUserLanguage(string languageCode)
        {
            LanguageRepository.SaveUserLanguage(languageCode);
        }

        public string LoadUserLanguage()
        {
            return LanguageRepository.LoadUserLanguage();
        }
    }
}
