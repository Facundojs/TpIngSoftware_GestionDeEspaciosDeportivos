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

        // Espacio Permissions
        public const string EspacioListar = "PERMISSION_ESPACIO_LIST";
        public const string EspacioCrear = "PERMISSION_ESPACIO_CREATE";
        public const string EspacioModificar = "PERMISSION_ESPACIO_UPDATE";
        public const string EspacioEliminar = "PERMISSION_ESPACIO_DELETE";

        // Rutina Permissions
        public const string RutinaVer = "PERMISSION_RUTINA_VIEW";
        public const string RutinaCrear = "PERMISSION_RUTINA_CREATE";
        public const string RutinaModificar = "PERMISSION_RUTINA_UPDATE";
        public const string RutinaEliminar = "PERMISSION_RUTINA_DELETE";

        // Ejercicio Permissions
        public const string EjercicioListar = "PERMISSION_EJERCICIO_LIST";
        public const string EjercicioCrear = "PERMISSION_EJERCICIO_CREATE";
        public const string EjercicioModificar = "PERMISSION_EJERCICIO_UPDATE";
        public const string EjercicioEliminar = "PERMISSION_EJERCICIO_DELETE";

        // Pago Permissions
        public const string PagoListar = "PERMISSION_PAGO_LIST";
        public const string PagoRegistrar = "PERMISSION_PAGO_CREATE";
        public const string PagoReembolsar = "PERMISSION_PAGO_REFUND";
        public const string PagoAdjuntarComprobante = "PERMISSION_PAGO_ATTACH";
    }
}
