using System;

namespace Service.Facade.Extension
{
    /// <summary>
    /// Extension methods that provide i18n translation.
    /// </summary>
    /// <remarks>
    /// Delegates to <c>Service.Logic.LanguageLogic.Translate</c>, which resolves the enum name
    /// against the active language file loaded by <c>LanguageRepository</c>.
    /// </remarks>
    public static class TranslationsExtension
    {
        /// <summary>
        /// Resolves <paramref name="key"/> to its localized string in the currently active language.
        /// </summary>
        /// <param name="key">The translation key enum value.</param>
        /// <returns>The localized string, or the key name itself if no translation is found.</returns>
        public static string Translate(this string key)
        {
            return Logic.LanguageLogic.Translate(key);
        }
    }
}
