-- Script to insert default Admin user with all permissions

-- 1. Insert Patents (Permissions)
-- IDs are generated manually here for the script, but usually handled by Guid.NewGuid() in code
DECLARE @P_Backup UNIQUEIDENTIFIER = NEWID();
DECLARE @P_Restore UNIQUEIDENTIFIER = NEWID();
DECLARE @P_ListBackups UNIQUEIDENTIFIER = NEWID();
DECLARE @P_DelBackup UNIQUEIDENTIFIER = NEWID();

DECLARE @P_ListUsers UNIQUEIDENTIFIER = NEWID();
DECLARE @P_CreateUser UNIQUEIDENTIFIER = NEWID();
DECLARE @P_ModUser UNIQUEIDENTIFIER = NEWID();
DECLARE @P_DelUser UNIQUEIDENTIFIER = NEWID();
DECLARE @P_AssignPerms UNIQUEIDENTIFIER = NEWID();

INSERT INTO Patente (Id, Nombre, TipoAcceso, DataKey) VALUES
(@P_Backup, 'RealizarBackup', 1, 'RealizarBackup'),
(@P_Restore, 'RealizarRestore', 2, 'RealizarRestore'),
(@P_ListBackups, 'ListarBackups', 3, 'ListarBackups'),
(@P_DelBackup, 'BorrarBackup', 4, 'BorrarBackup'),
(@P_ListUsers, 'ListarUsuarios', 5, 'ListarUsuarios'),
(@P_CreateUser, 'CrearUsuario', 6, 'CrearUsuario'),
(@P_ModUser, 'ModificarUsuario', 7, 'ModificarUsuario'),
(@P_DelUser, 'EliminarUsuario', 8, 'EliminarUsuario'),
(@P_AssignPerms, 'AsignarPermisos', 9, 'AsignarPermisos');

-- 2. Insert Admin Family
DECLARE @F_Admin UNIQUEIDENTIFIER = NEWID();
INSERT INTO Familia (Id, Nombre) VALUES (@F_Admin, 'Administrador');

-- 3. Link Patents to Family
INSERT INTO FamiliaPatente (IdFamilia, IdPatente) VALUES
(@F_Admin, @P_Backup),
(@F_Admin, @P_Restore),
(@F_Admin, @P_ListBackups),
(@F_Admin, @P_DelBackup),
(@F_Admin, @P_ListUsers),
(@F_Admin, @P_CreateUser),
(@F_Admin, @P_ModUser),
(@F_Admin, @P_DelUser),
(@F_Admin, @P_AssignPerms);

-- 4. Insert Admin User
-- Password 'admin123' hashed with SHA256 usually looks different,
-- but here we assume the app handles hashing.
-- For this script to work directly with the app, you need the hash.
-- SHA256('admin123') = '240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9'
DECLARE @U_Admin UNIQUEIDENTIFIER = NEWID();
INSERT INTO Usuario (Id, NombreUsuario, Password, Estado, DigitoVerificador) VALUES
(@U_Admin, 'admin', '240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9', 1, '');

-- 5. Link User to Family
INSERT INTO UsuarioFamilia (IdUsuario, IdFamilia) VALUES (@U_Admin, @F_Admin);

PRINT 'Admin user created successfully.';
