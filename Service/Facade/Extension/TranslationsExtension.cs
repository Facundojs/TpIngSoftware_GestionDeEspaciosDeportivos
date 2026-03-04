using System;
using Domain.Enums;

namespace Service.Facade.Extension
{
    /// <summary>
    /// Extension methods that provide i18n translation for <see cref="Translations"/> enum values.
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
        public static string Translate(this Translations key)
        {
            return Logic.LanguageLogic.Translate(key.ToString());
        }

        /// <summary>
        /// Equivalent to <see cref="Translate"/>; overrides <see cref="object.ToString"/> on the enum
        /// so that string interpolation automatically returns the localized value.
        /// </summary>
        /// <param name="key">The translation key enum value.</param>
        /// <returns>The localized string.</returns>
        public static string ToString(this Translations key)
        {
            return key.Translate();
        }
    }
}
