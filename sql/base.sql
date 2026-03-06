-- Inline SQL script
USE [master]
GO

-- Crear la base de datos si no existe
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'IngSoftwareBase')
BEGIN
    CREATE DATABASE [IngSoftwareBase];
END
GO

USE [IngSoftwareBase]
GO

-------------------------------------------------------
-- 1. DROP TABLES (Orden inverso por FKs)
-------------------------------------------------------
IF OBJECT_ID('[dbo].[UsuarioFamilia]', 'U') IS NOT NULL DROP TABLE [dbo].[UsuarioFamilia];
IF OBJECT_ID('[dbo].[FamiliaPatente]', 'U') IS NOT NULL DROP TABLE [dbo].[FamiliaPatente];
IF OBJECT_ID('[dbo].[FamiliaFamilia]', 'U') IS NOT NULL DROP TABLE [dbo].[FamiliaFamilia];
IF OBJECT_ID('[dbo].[Usuario]', 'U') IS NOT NULL DROP TABLE [dbo].[Usuario];
IF OBJECT_ID('[dbo].[Patente]', 'U') IS NOT NULL DROP TABLE [dbo].[Patente];
IF OBJECT_ID('[dbo].[Familia]', 'U') IS NOT NULL DROP TABLE [dbo].[Familia];
IF OBJECT_ID('[dbo].[Bitacora]', 'U') IS NOT NULL DROP TABLE [dbo].[Bitacora];
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-------------------------------------------------------
-- 2. CREATE TABLES
-------------------------------------------------------

-- Bitacora
CREATE TABLE [dbo].[Bitacora](
	[BitacoraID] [uniqueidentifier] NOT NULL,
	[Timestamp] [datetime] NOT NULL,
	[LogLevel] [nvarchar](50) NULL,
	[Message] [nvarchar](max) NULL,
	[ExceptionDetails] [nvarchar](max) NULL,
	[UsuarioNombre] [nvarchar](150) NULL,
    PRIMARY KEY CLUSTERED ([BitacoraID] ASC)
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

-- Familia
CREATE TABLE [dbo].[Familia](
	[Id] [uniqueidentifier] NOT NULL DEFAULT (newid()),
	[Nombre] [varchar](100) NOT NULL,
    CONSTRAINT [PK_Familia] PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO

-- Patente
CREATE TABLE [dbo].[Patente](
	[Id] [uniqueidentifier] NOT NULL DEFAULT (newid()),
	[Nombre] [varchar](100) NOT NULL,
	[TipoAcceso] [varchar](100) NOT NULL,
	[DataKey] [varchar](100) NOT NULL,
    CONSTRAINT [PK_Patente] PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO

-- Usuario
CREATE TABLE [dbo].[Usuario](
	[Id] [uniqueidentifier] NOT NULL DEFAULT (newid()),
	[NombreUsuario] [varchar](150) NOT NULL,
	[Password] [varchar](150) NOT NULL,
	[Estado] [bit] NOT NULL,
	[DigitoVerificador] [varchar](200) NOT NULL,
    CONSTRAINT [PK_Usuario] PRIMARY KEY CLUSTERED ([Id] ASC)
) ON [PRIMARY]
GO

-- FamiliaFamilia (Relación muchos a muchos recursiva)
CREATE TABLE [dbo].[FamiliaFamilia](
	[IdFamiliaPadre] [uniqueidentifier] NOT NULL,
	[IdFamiliaHija] [uniqueidentifier] NOT NULL,
    CONSTRAINT [PK_FamiliaFamilia] PRIMARY KEY CLUSTERED ([IdFamiliaPadre] ASC, [IdFamiliaHija] ASC)
) ON [PRIMARY]
GO

-- FamiliaPatente
CREATE TABLE [dbo].[FamiliaPatente](
	[IdFamilia] [uniqueidentifier] NOT NULL,
	[IdPatente] [uniqueidentifier] NOT NULL,
    CONSTRAINT [PK_FamiliaPatente] PRIMARY KEY CLUSTERED ([IdFamilia] ASC, [IdPatente] ASC)
) ON [PRIMARY]
GO

-- UsuarioFamilia
CREATE TABLE [dbo].[UsuarioFamilia](
	[IdUsuario] [uniqueidentifier] NOT NULL,
	[IdFamilia] [uniqueidentifier] NOT NULL,
    CONSTRAINT [PK_UsuarioFamilia] PRIMARY KEY CLUSTERED ([IdUsuario] ASC, [IdFamilia] ASC)
) ON [PRIMARY]
GO

-------------------------------------------------------
-- 3. CONSTRAINTS (FKs y Checks)
-------------------------------------------------------

-- FKs FamiliaFamilia
ALTER TABLE [dbo].[FamiliaFamilia] WITH CHECK ADD CONSTRAINT [FK_FamiliaFamilia_Hija] FOREIGN KEY([IdFamiliaHija]) REFERENCES [dbo].[Familia] ([Id])
ALTER TABLE [dbo].[FamiliaFamilia] WITH CHECK ADD CONSTRAINT [FK_FamiliaFamilia_Padre] FOREIGN KEY([IdFamiliaPadre]) REFERENCES [dbo].[Familia] ([Id])
ALTER TABLE [dbo].[FamiliaFamilia] WITH CHECK ADD CONSTRAINT [CK_FamiliaFamilia_NoSelfRef] CHECK (([IdFamiliaPadre]<>[IdFamiliaHija]))
GO

-- FKs FamiliaPatente
ALTER TABLE [dbo].[FamiliaPatente] WITH CHECK ADD CONSTRAINT [FK_FamiliaPatente_Familia] FOREIGN KEY([IdFamilia]) REFERENCES [dbo].[Familia] ([Id])
ALTER TABLE [dbo].[FamiliaPatente] WITH CHECK ADD CONSTRAINT [FK_FamiliaPatente_Patente] FOREIGN KEY([IdPatente]) REFERENCES [dbo].[Patente] ([Id])
GO

-- FKs UsuarioFamilia
ALTER TABLE [dbo].[UsuarioFamilia] WITH CHECK ADD CONSTRAINT [FK_UsuarioFamilia_Familia] FOREIGN KEY([IdFamilia]) REFERENCES [dbo].[Familia] ([Id])
ALTER TABLE [dbo].[UsuarioFamilia] WITH CHECK ADD CONSTRAINT [FK_UsuarioFamilia_Usuario] FOREIGN KEY([IdUsuario]) REFERENCES [dbo].[Usuario] ([Id])
GO


-- =============================================
-- Script: Inicialización de Permisos y Usuario Admin
-- Descripción: Inserta todos los permisos (Patentes), 
-- crea el rol Administrador y asigna el usuario 'admin'.
-- =============================================

BEGIN TRANSACTION;

BEGIN TRY
    -- 1. Tabla Temporal para organizar los permisos antes de la inserción
    CREATE TABLE #PermisosTemporales (
        Nombre VARCHAR(100),
        DataKey VARCHAR(100)
    );

    INSERT INTO #PermisosTemporales (Nombre, DataKey) VALUES
        ('BackupRealizar','PERMISSION_BACKUP_EXECUTE'),
        ('BackupRestore','PERMISSION_BACKUP_RESTORE'),
        ('BackupListar','PERMISSION_BACKUP_LIST'),
        ('BackupBorrar','PERMISSION_BACKUP_DELETE'),
        ('UsuarioListar','PERMISSION_USER_LIST'),
        ('UsuarioCrear','PERMISSION_USER_CREATE'),
        ('UsuarioModificar','PERMISSION_USER_UPDATE'),
        ('UsuarioEliminar','PERMISSION_USER_DELETE'),
        ('PermisoAsignar','PERMISSION_PERMISSION_ASSIGN'),
        ('BitacoraVer','PERMISSION_LOG_VIEW'),
        ('MembresiaListar','PERMISSION_MEMBRESIA_LIST'),
        ('MembresiaCrear','PERMISSION_MEMBRESIA_CREATE'),
        ('MembresiaModificar','PERMISSION_MEMBRESIA_UPDATE'),
        ('MembresiaDeshabilitar','PERMISSION_MEMBRESIA_DISABLE'),
        ('ClienteListar','PERMISSION_CLIENTE_LIST'),
        ('ClienteCrear','PERMISSION_CLIENTE_CREATE'),
        ('ClienteModificar','PERMISSION_CLIENTE_UPDATE'),
        ('ClienteDeshabilitar','PERMISSION_CLIENTE_DISABLE'),
        ('ClienteCheckIn','PERMISSION_CLIENTE_CHECKIN'),
        ('EspacioListar','PERMISSION_ESPACIO_LIST'),
        ('EspacioCrear','PERMISSION_ESPACIO_CREATE'),
        ('EspacioModificar','PERMISSION_ESPACIO_UPDATE'),
        ('EspacioEliminar','PERMISSION_ESPACIO_DELETE'),
        ('RutinaVer','PERMISSION_RUTINA_VIEW'),
        ('RutinaCrear','PERMISSION_RUTINA_CREATE'),
        ('RutinaModificar','PERMISSION_RUTINA_UPDATE'),
        ('RutinaEliminar','PERMISSION_RUTINA_DELETE'),
        ('EjercicioListar','PERMISSION_EJERCICIO_LIST'),
        ('EjercicioCrear','PERMISSION_EJERCICIO_CREATE'),
        ('EjercicioModificar','PERMISSION_EJERCICIO_UPDATE'),
        ('EjercicioEliminar','PERMISSION_EJERCICIO_DELETE'),
        ('PagoListar','PERMISSION_PAGO_LIST'),
        ('PagoRegistrar','PERMISSION_PAGO_CREATE'),
        ('PagoReembolsar','PERMISSION_PAGO_REFUND'),
        ('PagoAdjuntarComprobante','PERMISSION_PAGO_ATTACH'),
        ('ReservaListar','PERMISSION_RESERVA_LIST'),
        ('ReservaCrear','PERMISSION_RESERVA_CREATE'),
        ('ReservaCancelar','PERMISSION_RESERVA_CANCEL'),
        ('IngresoListar','PERMISSION_INGRESO_LIST');
    -- 2. Insertar Patentes (Evitando duplicados por DataKey)
    INSERT INTO Patente (Id, Nombre, TipoAcceso, DataKey)
    SELECT NEWID(), Nombre, DataKey, DataKey
    FROM #PermisosTemporales
    WHERE DataKey NOT IN (SELECT DataKey FROM Patente);

    -- 3. Crear Familia 'Administrador' (si no existe)
    DECLARE @F_Admin UNIQUEIDENTIFIER;
    SELECT @F_Admin = Id FROM Familia WHERE Nombre = 'Administrador';

    IF @F_Admin IS NULL
    BEGIN
        SET @F_Admin = NEWID();
        INSERT INTO Familia (Id, Nombre) VALUES (@F_Admin, 'Administrador');
    END

    -- 4. Vincular todas las Patentes a la Familia Administrador
    -- Borramos vínculos previos para evitar errores de clave duplicada y asegurar que tenga TODO
    DELETE FROM FamiliaPatente WHERE IdFamilia = @F_Admin;

    INSERT INTO FamiliaPatente (IdFamilia, IdPatente)
    SELECT @F_Admin, Id FROM Patente;

    -- 5. Crear Usuario Admin (si no existe)
    DECLARE @U_Admin UNIQUEIDENTIFIER;
    SELECT @U_Admin = Id FROM Usuario WHERE NombreUsuario = 'admin';

    IF @U_Admin IS NULL
    BEGIN
        SET @U_Admin = NEWID();
        -- Password 'admin123' (SHA256)
        INSERT INTO Usuario (Id, NombreUsuario, Password, Estado, DigitoVerificador) 
        VALUES (@U_Admin, 'admin', '240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9', 1, '0');
    END

    -- 6. Vincular Usuario a Familia Administrador (si no tiene el rol)
    IF NOT EXISTS (SELECT 1 FROM UsuarioFamilia WHERE IdUsuario = @U_Admin AND IdFamilia = @F_Admin)
    BEGIN
        INSERT INTO UsuarioFamilia (IdUsuario, IdFamilia) VALUES (@U_Admin, @F_Admin);
    END

    -- Limpieza
    DROP TABLE #PermisosTemporales;

    COMMIT TRANSACTION;
    PRINT 'Script ejecutado con éxito: Permisos creados y Admin actualizado.';

END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    IF OBJECT_ID('tempdb..#PermisosTemporales') IS NOT NULL DROP TABLE #PermisosTemporales;
    
    DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
    PRINT 'Error detectado: ' + @ErrorMessage;
END CATCH