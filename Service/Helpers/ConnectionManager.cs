using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Helpers
{
    /// <summary>
    /// Provides strongly-typed access to named connection strings defined in <c>App.config</c>.
    /// </summary>
    /// <remarks>
    /// Two connection strings are expected:
    /// <list type="bullet">
    ///   <item><see cref="BaseConnectionName"/> — used for system/infrastructure repositories (users, logs, permissions).</item>
    ///   <item><see cref="BusinessConnectionName"/> — used for all business-domain repositories and transactional UoW operations.</item>
    /// </list>
    /// </remarks>
    public static class ConnectionManager
    {
        /// <summary>Connection string name for the infrastructure database (users, logs, permissions).</summary>
        public const string BaseConnectionName = "IngSoftwareBase";

        /// <summary>Connection string name for the business database (clients, reservations, payments, etc.).</summary>
        public const string BusinessConnectionName = "IngSoftwareNegocio";

        /// <summary>
        /// Retrieves a connection string by name from <c>App.config</c> / <c>Web.config</c>.
        /// </summary>
        /// <param name="name">The connection string key. Defaults to <see cref="BaseConnectionName"/>.</param>
        /// <returns>The raw connection string value.</returns>
        /// <exception cref="ConfigurationErrorsException">
        /// Thrown when the named connection string is absent or empty in the configuration file.
        /// </exception>
        public static string GetConnectionString(string name = BaseConnectionName)
        {
            var connectionString = ConfigurationManager.ConnectionStrings[name]?.ConnectionString;
            if (string.IsNullOrEmpty(connectionString))
                throw new ConfigurationErrorsException($"No se encontró la cadena de conexión '{name}' en el archivo de configuración.");

            return connectionString;
        }

        /// <summary>Returns the connection string for <see cref="BaseConnectionName"/>.</summary>
        public static string GetBaseConnectionString() => GetConnectionString(BaseConnectionName);

        /// <summary>Returns the connection string for <see cref="BusinessConnectionName"/>.</summary>
        public static string GetBusinessConnectionString() => GetConnectionString(BusinessConnectionName);
    }
}
