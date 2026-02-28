using System;

namespace Service.Facade.Extension
{
    public static class TranslationsExtension
    {
        public static string Translate(this Domain.Enums.Translations key)
        {
            return Service.Logic.LanguageLogic.Translate(key.ToString());
        }
    }
}
