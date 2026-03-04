using Service.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Contracts
{
    /// <summary>
    /// Contract for database backup and restore operations.
    /// </summary>
    public interface IBackup
    {
        /// <summary>
        /// Creates a backup of the specified database at the given path.
        /// </summary>
        /// <param name="database">Name of the database to back up.</param>
        /// <param name="path">Directory path where the backup file will be written.</param>
        void BackUpDataBase(string database, string path);

        /// <summary>
        /// Restores a database from a backup file.
        /// </summary>
        /// <param name="database">Name of the target database to restore into.</param>
        /// <param name="path">Full path of the backup file to restore from.</param>
        void RestoreDataBase(string database, string path);

        /// <summary>
        /// Lists all available backup files in the configured backup directory.
        /// </summary>
        /// <returns>A list of <see cref="BackupFile"/> descriptors; empty if no backups exist.</returns>
        List<BackupFile> ListBackups();

        /// <summary>
        /// Deletes a backup file by its file name.
        /// </summary>
        /// <param name="filename">Name of the backup file to delete (not a full path).</param>
        void DeleteBackup(string filename);
    }
}
