USE [IngSoftwareNegocio]
GO

IF NOT EXISTS (
  SELECT *
  FROM   sys.columns
  WHERE  object_id = OBJECT_ID(N'[dbo].[Cliente]')
         AND name = 'Estado'
)
BEGIN
    ALTER TABLE Cliente
    ADD Estado NVARCHAR(50) NOT NULL DEFAULT 'Activo';
END
GO
