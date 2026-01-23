using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Contracts
{
    /// <summary>
    /// Interfaz que define los métodos para realizar copias de seguridad y restaurar bases de datos.
    /// </summary>
    public interface IBackup
    {
        /// <summary>
        /// Realiza una copia de seguridad de la base de datos especificada.
        /// </summary>
        /// <param name="database">El nombre de la base de datos a respaldar.</param>
        /// <param name="path">La ruta donde se almacenará la copia de seguridad.</param>
        void BackUpDataBase(string database, string path);

        /// <summary>
        /// Restaura la base de datos desde una copia de seguridad.
        /// </summary>
        /// <param name="database">El nombre de la base de datos a restaurar.</param>
        /// <param name="path">La ruta de la copia de seguridad desde la cual se restaurará.</param>
        void RestoreDataBase(string database, string path);
    }
}
