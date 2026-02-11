using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Helpers
{
    /// <summary>
    /// Clase ConnectionManager: Proporciona métodos para gestionar las cadenas de conexión desde el archivo de configuración.
    /// Esta clase facilita la recuperación de cadenas de conexión desde el archivo de configuración de la aplicación.
    /// </summary>
    public static class ConnectionManager
    {
        public const string BaseConnectionName = "IngSoftwareBase";
        public const string BusinessConnectionName = "IngSoftwareNegocio";

        /// <summary>
        /// Método estático para obtener una cadena de conexión desde App.config o Web.config.
        /// </summary>
        /// <param name="name">Nombre de la cadena de conexión a buscar en el archivo de configuración.</param>
        /// <returns>Devuelve la cadena de conexión correspondiente.</returns>
        /// <exception cref="ConfigurationErrorsException">Se lanza si la cadena de conexión no se encuentra o está vacía.</exception>
        public static string GetConnectionString(string name = BaseConnectionName)
        {
            var connectionString = ConfigurationManager.ConnectionStrings[name]?.ConnectionString;
            if (string.IsNullOrEmpty(connectionString))
                throw new ConfigurationErrorsException($"No se encontró la cadena de conexión '{name}' en el archivo de configuración.");

            return connectionString;
        }

        public static string GetBaseConnectionString() => GetConnectionString(BaseConnectionName);
        public static string GetBusinessConnectionString() => GetConnectionString(BusinessConnectionName);
    }
}
