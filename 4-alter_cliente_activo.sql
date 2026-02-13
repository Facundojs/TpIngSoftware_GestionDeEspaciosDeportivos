USE [IngSoftwareNegocio]
GO

IF NOT EXISTS (
  SELECT *
  FROM   sys.columns
  WHERE  object_id = OBJECT_ID(N'[dbo].[Cliente]')
         AND name = 'Activo'
)
BEGIN
    ALTER TABLE Cliente
    ADD Activo BIT NOT NULL DEFAULT 1;
END
GO
