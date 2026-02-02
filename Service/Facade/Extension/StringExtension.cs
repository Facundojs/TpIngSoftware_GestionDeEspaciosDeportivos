using Service.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Facade.Extension
{
    /// <summary>
    /// Clase estática que agrupa las extensiones de cadena para funcionalidades adicionales.
    /// </summary>
    public static class StringExtension
    {
        /// <summary>
        /// Traducir una clave de texto usando la lógica de traducción.
        /// </summary>
        /// <param name="key">La clave del texto a traducir.</param>
        /// <returns>La cadena traducida correspondiente a la clave.</returns>
        public static string Translate(this string key)
        {
            return LanguageLogic.Translate(key);
        }
    }
}
