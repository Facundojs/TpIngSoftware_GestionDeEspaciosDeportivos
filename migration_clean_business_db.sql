-- Migration to clean up business database (IngSoftwareNegocio)
-- Remove Operador and Administrador tables as roles are now handled by permissions in the Base DB.

USE IngSoftwareNegocio;
GO

IF OBJECT_ID('dbo.Operador', 'U') IS NOT NULL
DROP TABLE dbo.Operador;
GO

IF OBJECT_ID('dbo.Administrador', 'U') IS NOT NULL
DROP TABLE dbo.Administrador;
GO
