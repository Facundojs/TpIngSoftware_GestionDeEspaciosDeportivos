USE [IngSoftwareNegocio]
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Cliente]') AND name = N'Razon')
BEGIN
    ALTER TABLE [dbo].[Cliente] ADD [Razon] NVARCHAR(MAX) NULL;
END
GO
