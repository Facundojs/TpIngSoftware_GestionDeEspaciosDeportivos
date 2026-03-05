USE [IngSoftwareNegocio]
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Cliente]') AND name = 'Email')
BEGIN
    ALTER TABLE Cliente ADD Email NVARCHAR(255) NULL;
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Cliente]') AND name = 'CreatedAt')
BEGIN
    ALTER TABLE Cliente ADD CreatedAt DATETIME NOT NULL DEFAULT GETDATE();
END
GO
