namespace Service.Domain.Composite
{
    public static class PermisoKeys
    {
        // Backup Permissions
        public const string BackupRealizar = "PERMISSION_BACKUP_EXECUTE";
        public const string BackupRestore = "PERMISSION_BACKUP_RESTORE";
        public const string BackupListar = "PERMISSION_BACKUP_LIST";
        public const string BackupBorrar = "PERMISSION_BACKUP_DELETE";

        // User Management Permissions
        public const string UsuarioListar = "PERMISSION_USER_LIST";
        public const string UsuarioCrear = "PERMISSION_USER_CREATE";
        public const string UsuarioModificar = "PERMISSION_USER_UPDATE";
        public const string UsuarioEliminar = "PERMISSION_USER_DELETE";

        // Permission Management
        public const string PermisoAsignar = "PERMISSION_PERMISSION_ASSIGN";
    }
}
