namespace Service.Domain.Composite
{
    public enum TipoPermiso
    {
        // Backup Permissions
        /// <summary>
        /// Permite realizar un backup de la base de datos.
        /// </summary>
        PuedeRealizarBackup = 1,

        /// <summary>
        /// Permite restaurar un backup de la base de datos.
        /// </summary>
        PuedeRealizarRestore = 2,

        /// <summary>
        /// Permite listar los backups disponibles.
        /// </summary>
        PuedeListarBackups = 3,

        /// <summary>
        /// Permite borrar un archivo de backup.
        /// </summary>
        PuedeBorrarBackup = 4,

        // User Management Permissions
        /// <summary>
        /// Permite listar usuarios.
        /// </summary>
        PuedeListarUsuarios = 5,

        /// <summary>
        /// Permite crear un nuevo usuario.
        /// </summary>
        PuedeCrearUsuario = 6,

        /// <summary>
        /// Permite modificar datos de un usuario existente.
        /// </summary>
        PuedeModificarUsuario = 7,

        /// <summary>
        /// Permite eliminar un usuario.
        /// </summary>
        PuedeEliminarUsuario = 8,

        // Permission Management
        /// <summary>
        /// Permite gestionar los permisos (familias) de un usuario.
        /// </summary>
        PuedeAsignarPermisos = 9,
    }
}
