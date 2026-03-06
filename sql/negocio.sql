USE [master]
GO

-- Crear la base de datos si no existe
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'IngSoftwareNegocio')
BEGIN
    CREATE DATABASE [IngSoftwareNegocio];
END
GO

USE [IngSoftwareNegocio]
GO

-------------------------------------------------------
-- 1. DROP TABLES & VIEWS (Orden inverso por dependencias)
-------------------------------------------------------
IF OBJECT_ID('[dbo].[vw_Balance]', 'V') IS NOT NULL DROP VIEW [dbo].[vw_Balance];
IF OBJECT_ID('[dbo].[Comprobante]', 'U') IS NOT NULL DROP TABLE [dbo].[Comprobante];
IF OBJECT_ID('[dbo].[Movimiento]', 'U') IS NOT NULL DROP TABLE [dbo].[Movimiento];
IF OBJECT_ID('[dbo].[RutinaEjercicio]', 'U') IS NOT NULL DROP TABLE [dbo].[RutinaEjercicio];
IF OBJECT_ID('[dbo].[Rutina]', 'U') IS NOT NULL DROP TABLE [dbo].[Rutina];
IF OBJECT_ID('[dbo].[Pago]', 'U') IS NOT NULL DROP TABLE [dbo].[Pago];
IF OBJECT_ID('[dbo].[Reserva]', 'U') IS NOT NULL DROP TABLE [dbo].[Reserva];
IF OBJECT_ID('[dbo].[Agenda]', 'U') IS NOT NULL DROP TABLE [dbo].[Agenda];
IF OBJECT_ID('[dbo].[Espacio]', 'U') IS NOT NULL DROP TABLE [dbo].[Espacio];
IF OBJECT_ID('[dbo].[Ingreso]', 'U') IS NOT NULL DROP TABLE [dbo].[Ingreso];
IF OBJECT_ID('[dbo].[ClienteMembresia]', 'U') IS NOT NULL DROP TABLE [dbo].[ClienteMembresia];
IF OBJECT_ID('[dbo].[Cliente]', 'U') IS NOT NULL DROP TABLE [dbo].[Cliente];
IF OBJECT_ID('[dbo].[Membresia]', 'U') IS NOT NULL DROP TABLE [dbo].[Membresia];
IF OBJECT_ID('[dbo].[Ejercicio]', 'U') IS NOT NULL DROP TABLE [dbo].[Ejercicio];
IF OBJECT_ID('[dbo].[Administrador]', 'U') IS NOT NULL DROP TABLE [dbo].[Administrador];
IF OBJECT_ID('[dbo].[Operador]', 'U') IS NOT NULL DROP TABLE [dbo].[Operador];
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-------------------------------------------------------
-- 2. CREATE TABLES (Entidades Base)
-------------------------------------------------------

CREATE TABLE [dbo].[Ejercicio](
	[Id] [uniqueidentifier] NOT NULL DEFAULT (newid()),
	[Nombre] [nvarchar](255) NOT NULL UNIQUE,
    PRIMARY KEY ([Id])
);

CREATE TABLE [dbo].[Membresia](
	[Id] [uniqueidentifier] NOT NULL DEFAULT (newid()),
	[Codigo] [int] NOT NULL UNIQUE,
	[Nombre] [nvarchar](255) NOT NULL,
	[Precio] [decimal](18, 2) NOT NULL CHECK ([Precio] >= 0),
	[Regularidad] [int] NOT NULL CHECK ([Regularidad] > 0),
	[Activa] [bit] NOT NULL DEFAULT (1),
	[Detalle] [nvarchar](max) NULL,
    PRIMARY KEY ([Id])
);

CREATE TABLE [dbo].[Espacio](
	[Id] [uniqueidentifier] NOT NULL DEFAULT (newid()),
	[Nombre] [nvarchar](255) NOT NULL,
	[Descripcion] [nvarchar](max) NULL,
	[PrecioHora] [decimal](18, 2) NOT NULL CHECK ([PrecioHora] >= 0),
	[Estado] [nvarchar](50) NOT NULL DEFAULT ('Activo'),
    PRIMARY KEY ([Id])
);

CREATE TABLE [dbo].[Cliente](
	[Id] [uniqueidentifier] NOT NULL DEFAULT (newid()),
	[Nombre] [nvarchar](100) NOT NULL,
	[Apellido] [nvarchar](100) NOT NULL,
	[DNI] [int] NOT NULL UNIQUE,
	[FechaNacimiento] [datetime] NOT NULL,
	[MembresiaID] [uniqueidentifier] NULL REFERENCES [dbo].[Membresia]([Id]),
	[Estado] [nvarchar](50) NOT NULL DEFAULT ('Activo'),
	[Email] [nvarchar](255) NULL,
	[CreatedAt] [datetime] NOT NULL DEFAULT (getdate()),
	[Razon] [nvarchar](max) NULL,
    PRIMARY KEY ([Id])
);

-------------------------------------------------------
-- 3. CREATE TABLES (Operativas y Relaciones)
-------------------------------------------------------

CREATE TABLE [dbo].[Agenda](
	[EspacioID] [uniqueidentifier] NOT NULL REFERENCES [dbo].[Espacio]([Id]) ON DELETE CASCADE,
	[HoraDesde] [time](7) NOT NULL,
	[HoraHasta] [time](7) NOT NULL,
	[DiaSemana] [int] NOT NULL DEFAULT (0) CHECK ([DiaSemana] >= 0 AND [DiaSemana] <= 6),
    CONSTRAINT [PK_Agenda] PRIMARY KEY ([EspacioID], [DiaSemana], [HoraDesde], [HoraHasta])
);

CREATE TABLE [dbo].[Reserva](
	[Id] [uniqueidentifier] NOT NULL DEFAULT (newid()),
	[ClienteID] [uniqueidentifier] NOT NULL REFERENCES [dbo].[Cliente]([Id]),
	[EspacioID] [uniqueidentifier] NOT NULL REFERENCES [dbo].[Espacio]([Id]),
	[Fecha] [date] NOT NULL,
	[FechaHora] [datetime] NOT NULL,
	[Duracion] [int] NOT NULL,
	[Adelanto] [decimal](18, 2) NOT NULL,
	[CodigoReserva] [nvarchar](50) NOT NULL UNIQUE,
	[Estado] [nvarchar](50) NOT NULL CHECK ([Estado] IN ('Cancelada', 'Pagada', 'Pendiente')),
    PRIMARY KEY ([Id])
);

CREATE TABLE [dbo].[Pago](
	[Id] [uniqueidentifier] NOT NULL DEFAULT (newid()),
	[Codigo] [int] IDENTITY(1,1) NOT NULL,
	[ClienteID] [uniqueidentifier] NOT NULL REFERENCES [dbo].[Cliente]([Id]),
	[Monto] [decimal](18, 2) NOT NULL CHECK ([Monto] >= 0),
	[Metodo] [nvarchar](50) NOT NULL,
	[Detalle] [nvarchar](max) NULL,
	[Fecha] [datetime] NOT NULL,
	[Estado] [nvarchar](50) NOT NULL CHECK ([Estado] IN ('Reembolsado', 'Abonado')),
	[MembresiaID] [uniqueidentifier] NULL REFERENCES [dbo].[Membresia]([Id]),
	[ReservaID] [uniqueidentifier] NULL REFERENCES [dbo].[Reserva]([Id]),
    PRIMARY KEY ([Id]),
    CONSTRAINT [CK_Pago_Origen] CHECK (NOT ([MembresiaID] IS NOT NULL AND [ReservaID] IS NOT NULL))
);

CREATE TABLE [dbo].[Movimiento](
	[Id] [uniqueidentifier] NOT NULL DEFAULT (newid()),
	[ClienteID] [uniqueidentifier] NOT NULL REFERENCES [dbo].[Cliente]([Id]),
	[Tipo] [nvarchar](50) NOT NULL,
	[Monto] [decimal](18, 2) NOT NULL,
	[Descripcion] [nvarchar](max) NULL,
	[Fecha] [datetime] NOT NULL,
	[PagoID] [uniqueidentifier] NULL REFERENCES [dbo].[Pago]([Id]),
    PRIMARY KEY ([Id])
);

CREATE TABLE [dbo].[Comprobante](
	[Id] [uniqueidentifier] NOT NULL DEFAULT (newid()),
	[PagoID] [uniqueidentifier] NULL REFERENCES [dbo].[Pago]([Id]),
	[NombreArchivo] [nvarchar](255) NOT NULL,
	[RutaArchivo] [nvarchar](max) NOT NULL,
	[FechaSubida] [datetime] NOT NULL,
	[ReservaID] [uniqueidentifier] NULL REFERENCES [dbo].[Reserva]([Id]),
    PRIMARY KEY ([Id]),
    CONSTRAINT [CK_Comprobante_Origen] CHECK ([PagoID] IS NOT NULL OR [ReservaID] IS NOT NULL)
);

CREATE TABLE [dbo].[Rutina](
	[Id] [uniqueidentifier] NOT NULL DEFAULT (newid()),
	[ClienteID] [uniqueidentifier] NOT NULL REFERENCES [dbo].[Cliente]([Id]) ON DELETE CASCADE,
	[Desde] [datetime] NOT NULL,
	[Hasta] [datetime] NULL,
	[Detalle] [nvarchar](max) NULL,
    PRIMARY KEY ([Id])
);

CREATE TABLE [dbo].[RutinaEjercicio](
	[RutinaID] [uniqueidentifier] NOT NULL REFERENCES [dbo].[Rutina]([Id]) ON DELETE CASCADE,
	[EjercicioID] [uniqueidentifier] NOT NULL REFERENCES [dbo].[Ejercicio]([Id]) ON DELETE CASCADE,
	[Repeticiones] [int] NOT NULL CHECK ([Repeticiones] > 0),
	[DiaSemana] [int] NOT NULL CHECK ([DiaSemana] >= 1 AND [DiaSemana] <= 7),
	[Orden] [int] NOT NULL,
    PRIMARY KEY ([RutinaID], [EjercicioID])
);

CREATE TABLE [dbo].[Ingreso](
	[Id] [uniqueidentifier] NOT NULL DEFAULT (newid()),
	[ClienteID] [uniqueidentifier] NULL REFERENCES [dbo].[Cliente]([Id]),
	[FechaHora] [datetime] NOT NULL,
    PRIMARY KEY ([Id])
);

CREATE TABLE [dbo].[ClienteMembresia](
	[Id] [uniqueidentifier] NOT NULL DEFAULT (newid()),
	[ClienteID] [uniqueidentifier] NOT NULL REFERENCES [dbo].[Cliente]([Id]),
	[MembresiaID] [uniqueidentifier] NOT NULL REFERENCES [dbo].[Membresia]([Id]),
	[FechaAsignacion] [datetime] NOT NULL,
	[ProximaFechaPago] [datetime] NULL,
	[FechaBaja] [datetime] NULL,
    PRIMARY KEY ([Id])
);

CREATE TABLE [dbo].[Administrador](
	[Id] [uniqueidentifier] NOT NULL PRIMARY KEY,
	[Email] [nvarchar](255) NOT NULL
);

CREATE TABLE [dbo].[Operador](
	[Id] [uniqueidentifier] NOT NULL PRIMARY KEY,
	[Email] [nvarchar](255) NOT NULL,
	[FechaIngreso] [datetime] NOT NULL
);
GO

-------------------------------------------------------
-- 4. VIEWS
-------------------------------------------------------
CREATE VIEW [dbo].[vw_Balance] AS
SELECT
    ClienteID,
    SUM(Monto) AS Saldo,
    MAX(Fecha) AS UltimaActualizacion
FROM Movimiento
GROUP BY ClienteID;
GO