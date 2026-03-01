USE IngSoftwareNegocio;
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[Permiso] WHERE Id = 'PERMISSION_INGRESO_LIST')
BEGIN
    INSERT INTO [dbo].[Permiso] (Id, Nombre, Permitido, EsFamilia)
    VALUES ('PERMISSION_INGRESO_LIST', 'Listar Ingresos', 1, 0);
END
GO

-- Assign to Admin Familia (Assuming Id = 'FAM_ADMIN')
IF NOT EXISTS (SELECT 1 FROM [dbo].[Permiso_Permiso] WHERE IdPadre = 'FAM_ADMIN' AND IdHijo = 'PERMISSION_INGRESO_LIST')
BEGIN
    INSERT INTO [dbo].[Permiso_Permiso] (IdPadre, IdHijo)
    VALUES ('FAM_ADMIN', 'PERMISSION_INGRESO_LIST');
END
GO
