USE [IngSoftwareBase]
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FamiliaFamilia]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[FamiliaFamilia] (
        [IdFamiliaPadre] UNIQUEIDENTIFIER NOT NULL,
        [IdFamiliaHija]  UNIQUEIDENTIFIER NOT NULL,
        CONSTRAINT [PK_FamiliaFamilia] PRIMARY KEY ([IdFamiliaPadre], [IdFamiliaHija]),
        CONSTRAINT [FK_FamiliaFamilia_Padre] FOREIGN KEY ([IdFamiliaPadre]) REFERENCES [dbo].[Familia]([Id]),
        CONSTRAINT [FK_FamiliaFamilia_Hija]  FOREIGN KEY ([IdFamiliaHija])  REFERENCES [dbo].[Familia]([Id]),
        CONSTRAINT [CK_FamiliaFamilia_NoSelfRef] CHECK ([IdFamiliaPadre] <> [IdFamiliaHija])
    )
END
GO
