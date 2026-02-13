USE IngSoftwareBase;
GO

-- 1. Definir variables para los nuevos permisos
DECLARE @IdList UNIQUEIDENTIFIER, @IdCreate UNIQUEIDENTIFIER, 
        @IdUpdate UNIQUEIDENTIFIER, @IdDisable UNIQUEIDENTIFIER;

-- 2. Insertar Patentes solo si no existen (buscando por DataKey)
-- Usamos una tabla temporal o variables para manejar los IDs nuevos
IF NOT EXISTS (SELECT 1 FROM Patente WHERE DataKey = 'PERMISSION_MEMBRESIA_LIST')
BEGIN
    SET @IdList = NEWID();
    INSERT INTO Patente (Id, Nombre, TipoAcceso, DataKey) 
    VALUES (@IdList, 'MembresiaListar', 'PERMISSION_MEMBRESIA_LIST', 'PERMISSION_MEMBRESIA_LIST');
END
ELSE SELECT @IdList = Id FROM Patente WHERE DataKey = 'PERMISSION_MEMBRESIA_LIST';

IF NOT EXISTS (SELECT 1 FROM Patente WHERE DataKey = 'PERMISSION_MEMBRESIA_CREATE')
BEGIN
    SET @IdCreate = NEWID();
    INSERT INTO Patente (Id, Nombre, TipoAcceso, DataKey) 
    VALUES (@IdCreate, 'MembresiaCrear', 'PERMISSION_MEMBRESIA_CREATE', 'PERMISSION_MEMBRESIA_CREATE');
END
ELSE SELECT @IdCreate = Id FROM Patente WHERE DataKey = 'PERMISSION_MEMBRESIA_CREATE';

IF NOT EXISTS (SELECT 1 FROM Patente WHERE DataKey = 'PERMISSION_MEMBRESIA_UPDATE')
BEGIN
    SET @IdUpdate = NEWID();
    INSERT INTO Patente (Id, Nombre, TipoAcceso, DataKey) 
    VALUES (@IdUpdate, 'MembresiaModificar', 'PERMISSION_MEMBRESIA_UPDATE', 'PERMISSION_MEMBRESIA_UPDATE');
END
ELSE SELECT @IdUpdate = Id FROM Patente WHERE DataKey = 'PERMISSION_MEMBRESIA_UPDATE';

IF NOT EXISTS (SELECT 1 FROM Patente WHERE DataKey = 'PERMISSION_MEMBRESIA_DISABLE')
BEGIN
    SET @IdDisable = NEWID();
    INSERT INTO Patente (Id, Nombre, TipoAcceso, DataKey) 
    VALUES (@IdDisable, 'MembresiaDeshabilitar', 'PERMISSION_MEMBRESIA_DISABLE', 'PERMISSION_MEMBRESIA_DISABLE');
END
ELSE SELECT @IdDisable = Id FROM Patente WHERE DataKey = 'PERMISSION_MEMBRESIA_DISABLE';

-- 3. Asignar Patentes a la Familia 'Administrador'
DECLARE @AdminFamiliaId UNIQUEIDENTIFIER;
SELECT @AdminFamiliaId = Id FROM Familia WHERE Nombre = 'Administrador';

IF @AdminFamiliaId IS NOT NULL
BEGIN
    -- Listar
    IF NOT EXISTS (SELECT 1 FROM FamiliaPatente WHERE IdFamilia = @AdminFamiliaId AND IdPatente = @IdList)
        INSERT INTO FamiliaPatente (IdFamilia, IdPatente) VALUES (@AdminFamiliaId, @IdList);

    -- Crear
    IF NOT EXISTS (SELECT 1 FROM FamiliaPatente WHERE IdFamilia = @AdminFamiliaId AND IdPatente = @IdCreate)
        INSERT INTO FamiliaPatente (IdFamilia, IdPatente) VALUES (@AdminFamiliaId, @IdCreate);

    -- Modificar
    IF NOT EXISTS (SELECT 1 FROM FamiliaPatente WHERE IdFamilia = @AdminFamiliaId AND IdPatente = @IdUpdate)
        INSERT INTO FamiliaPatente (IdFamilia, IdPatente) VALUES (@AdminFamiliaId, @IdUpdate);

    -- Deshabilitar
    IF NOT EXISTS (SELECT 1 FROM FamiliaPatente WHERE IdFamilia = @AdminFamiliaId AND IdPatente = @IdDisable)
        INSERT INTO FamiliaPatente (IdFamilia, IdPatente) VALUES (@AdminFamiliaId, @IdDisable);

    PRINT 'Permisos de Membresía asignados al Administrador correctamente.';
END
ELSE
BEGIN
    PRINT 'ERROR: Familia Administrador no encontrada.';
END
GO