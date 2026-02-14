namespace Domain.Composite
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

        // Log Permissions
        public const string BitacoraVer = "PERMISSION_LOG_VIEW";

        // Membership Permissions
        public const string MembresiaListar = "PERMISSION_MEMBRESIA_LIST";
        public const string MembresiaCrear = "PERMISSION_MEMBRESIA_CREATE";
        public const string MembresiaModificar = "PERMISSION_MEMBRESIA_UPDATE";
        public const string MembresiaDeshabilitar = "PERMISSION_MEMBRESIA_DISABLE";

        // Client Permissions
        public const string ClienteListar = "PERMISSION_CLIENTE_LIST";
        public const string ClienteCrear = "PERMISSION_CLIENTE_CREATE";
        public const string ClienteModificar = "PERMISSION_CLIENTE_UPDATE";
        public const string ClienteDeshabilitar = "PERMISSION_CLIENTE_DISABLE";
        public const string ClienteCheckIn = "PERMISSION_CLIENTE_CHECKIN";
    }
}
