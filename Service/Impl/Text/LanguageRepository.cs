using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Service.Impl.Text
{
    /// <summary>
    /// Repositorio de idiomas que proporciona métodos para traducir claves a su correspondiente valor en el idioma actual.
    /// </summary>
    internal static class LanguageRepository
    {
        private static string LanguagePath = ConfigurationManager.AppSettings["LanguagePath"];
        private static readonly string UserLanguageConfigPath = ConfigurationManager.AppSettings["UserLanguageConfigPath"];

        /// <summary>
        /// Traduce una clave a su correspondiente valor en el idioma actual.
        /// </summary>
        /// <param name="key">La clave que se desea traducir.</param>
        /// <returns>La traducción correspondiente a la clave en el idioma actual.</returns>
        /// <exception cref="Exception">Lanza una excepción si no se encuentra el archivo de idioma o la clave no existe.</exception>
        public static string Translate(string key)
        {
            // Obtener el código de idioma actual (es-ES, en-EN)
            string language = Thread.CurrentThread.CurrentUICulture.Name;

            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string cleanLanguagePath = LanguagePath.Trim('\\', '/');
            string fileName = Path.Combine(baseDirectory, cleanLanguagePath, $"Language.{language}");
            System.Diagnostics.Debug.WriteLine($"Ruta buscada: {fileName}");
            if (!File.Exists(fileName))
            {
                throw new Exception($"No se encontró el archivo de idioma para {language}");
            }

            using (StreamReader reader = new StreamReader(fileName))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] columns = line.Split('=');

                    if (columns[0].ToLower() == key.ToLower())
                    {
                        return columns[1];
                    }
                }
            }

            return $"TODO: [{key}]";
            //throw new Exception($"No se encontró la palabra {key} en el archivo de idioma {fileName}"); // Esto luego se guarda en el log de errores
        }

        /// <summary>
        /// Obtiene un diccionario de códigos de idioma y sus nombres nativos desde los archivos de idioma disponibles.
        /// </summary>
        /// <returns>Un diccionario donde la clave es el código del idioma y el valor es el nombre nativo del idioma.</returns>
        public static Dictionary<string, string> GetLanguages()
        {
            Dictionary<string, string> languages = new Dictionary<string, string>();

            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            string languageFolderPath = Path.Combine(baseDirectory, LanguagePath);

            // Verifica si la ruta de idiomas existe
            if (Directory.Exists(languageFolderPath))
            {
                // Obtiene todos los archivos que coinciden con el patrón "Language.*"
                foreach (string file in Directory.GetFiles(languageFolderPath, "Language.*"))
                {
                    string fileName = Path.GetFileName(file);
                    if (fileName.StartsWith("Language."))
                    {
                        string languageCode = fileName.Substring("Language.".Length);

                        try
                        {
                            // Intenta crear un objeto CultureInfo para obtener el nombre nativo del idioma
                            var culture = new CultureInfo(languageCode);
                            string languageName = culture.NativeName;
                            languages.Add(languageCode, languageName); // Agrega el código y el nombre al diccionario
                        }
                        catch (CultureNotFoundException)
                        {
                            // Si el código de cultura no es válido, continúa con el siguiente archivo
                            continue;
                        }
                    }
                }
            }

            return languages;
        }


        /// <summary>
        /// Guarda el código del idioma del usuario en un archivo de configuración.
        /// </summary>
        /// <param name="languageCode">El código del idioma que se desea guardar.</param>
        public static void SaveUserLanguage(string languageCode)
        {
            // Sobrescribe el archivo de configuración con el nuevo código de idioma
            using (StreamWriter writer = new StreamWriter(UserLanguageConfigPath, false))
            {
                writer.WriteLine(languageCode);
            }
        }

        /// <summary>
        /// Carga el idioma de preferencia del usuario desde un archivo de configuración.
        /// </summary>
        /// <returns>El código del idioma guardado, o "es-ES" si no existe o está vacío.</returns>
        public static string LoadUserLanguage()
        {
            if (File.Exists(UserLanguageConfigPath))
            {
                using (StreamReader reader = new StreamReader(UserLanguageConfigPath))
                {
                    string languageCode = reader.ReadLine();
                    if (!string.IsNullOrEmpty(languageCode))
                    {
                        return languageCode;  // devuelve el idioma guardado
                    }
                }
            }
            // Si no existe el archivo o no tiene un valor retorna "es-ES" como idioma predeterminado
            return "es-ES";
        }
    }
}