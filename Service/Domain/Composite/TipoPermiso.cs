namespace Service.Domain.Composite
{
    public enum TipoPermiso
    {
        // Backup Permissions
        /// <summary>
        /// Permite realizar un backup de la base de datos.
        /// </summary>
        RealizarBackup = 1,

        /// <summary>
        /// Permite restaurar un backup de la base de datos.
        /// </summary>
        RealizarRestore = 2,

        /// <summary>
        /// Permite listar los backups disponibles.
        /// </summary>
        ListarBackups = 3,

        /// <summary>
        /// Permite borrar un archivo de backup.
        /// </summary>
        BorrarBackup = 4,

        // User Management Permissions
        /// <summary>
        /// Permite listar usuarios.
        /// </summary>
        ListarUsuarios = 5,

        /// <summary>
        /// Permite crear un nuevo usuario.
        /// </summary>
        CrearUsuario = 6,

        /// <summary>
        /// Permite modificar datos de un usuario existente.
        /// </summary>
        ModificarUsuario = 7,

        /// <summary>
        /// Permite eliminar un usuario.
        /// </summary>
        EliminarUsuario = 8,

        // Permission Management
        /// <summary>
        /// Permite gestionar los permisos (familias) de un usuario.
        /// </summary>
        AsignarPermisos = 9,
    }
}
