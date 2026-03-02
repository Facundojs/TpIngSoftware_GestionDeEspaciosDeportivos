using System;
using Domain.Enums;

namespace Service.Facade.Extension
{
    public static class TranslationsExtension
    {
        public static string Translate(this Translations key)
        {
            return Service.Logic.LanguageLogic.Translate(key.ToString());
        }

        public static string ToString(this Translations key)
        {
            return key.Translate();
        }
    }
}
