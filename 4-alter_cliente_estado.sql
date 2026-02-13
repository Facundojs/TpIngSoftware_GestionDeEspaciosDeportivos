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
    ADD Estado INT NOT NULL DEFAULT 0;
END
GO
