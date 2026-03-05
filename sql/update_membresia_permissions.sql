USE IngSoftwareBase;
GO

-- 1. Insertar Patentes si no existen
IF NOT EXISTS (SELECT 1 FROM Patente WHERE PatenteId = 'PERMISSION_MEMBRESIA_LIST')
BEGIN
    INSERT INTO Patente (PatenteId, Descripcion) VALUES ('PERMISSION_MEMBRESIA_LIST', 'Listar Membresías');
END

IF NOT EXISTS (SELECT 1 FROM Patente WHERE PatenteId = 'PERMISSION_MEMBRESIA_CREATE')
BEGIN
    INSERT INTO Patente (PatenteId, Descripcion) VALUES ('PERMISSION_MEMBRESIA_CREATE', 'Crear Membresías');
END

IF NOT EXISTS (SELECT 1 FROM Patente WHERE PatenteId = 'PERMISSION_MEMBRESIA_UPDATE')
BEGIN
    INSERT INTO Patente (PatenteId, Descripcion) VALUES ('PERMISSION_MEMBRESIA_UPDATE', 'Modificar Membresías');
END

IF NOT EXISTS (SELECT 1 FROM Patente WHERE PatenteId = 'PERMISSION_MEMBRESIA_DISABLE')
BEGIN
    INSERT INTO Patente (PatenteId, Descripcion) VALUES ('PERMISSION_MEMBRESIA_DISABLE', 'Deshabilitar Membresías');
END

-- 2. Asignar Patentes a la Familia 'Administrador'
DECLARE @AdminFamiliaId UNIQUEIDENTIFIER;
SELECT @AdminFamiliaId = Id FROM Familia WHERE Nombre = 'Administrador';

IF @AdminFamiliaId IS NOT NULL
BEGIN
    -- Listar
    IF NOT EXISTS (SELECT 1 FROM FamiliaPatente WHERE FamiliaId = @AdminFamiliaId AND PatenteId = 'PERMISSION_MEMBRESIA_LIST')
    BEGIN
        INSERT INTO FamiliaPatente (FamiliaId, PatenteId) VALUES (@AdminFamiliaId, 'PERMISSION_MEMBRESIA_LIST');
    END

    -- Crear
    IF NOT EXISTS (SELECT 1 FROM FamiliaPatente WHERE FamiliaId = @AdminFamiliaId AND PatenteId = 'PERMISSION_MEMBRESIA_CREATE')
    BEGIN
        INSERT INTO FamiliaPatente (FamiliaId, PatenteId) VALUES (@AdminFamiliaId, 'PERMISSION_MEMBRESIA_CREATE');
    END

    -- Modificar
    IF NOT EXISTS (SELECT 1 FROM FamiliaPatente WHERE FamiliaId = @AdminFamiliaId AND PatenteId = 'PERMISSION_MEMBRESIA_UPDATE')
    BEGIN
        INSERT INTO FamiliaPatente (FamiliaId, PatenteId) VALUES (@AdminFamiliaId, 'PERMISSION_MEMBRESIA_UPDATE');
    END

    -- Deshabilitar
    IF NOT EXISTS (SELECT 1 FROM FamiliaPatente WHERE FamiliaId = @AdminFamiliaId AND PatenteId = 'PERMISSION_MEMBRESIA_DISABLE')
    BEGIN
        INSERT INTO FamiliaPatente (FamiliaId, PatenteId) VALUES (@AdminFamiliaId, 'PERMISSION_MEMBRESIA_DISABLE');
    END
END
ELSE
BEGIN
    PRINT 'Familia Administrador no encontrada.';
END
GO
