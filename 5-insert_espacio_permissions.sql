USE IngSoftwareBase;
GO

-- 1. Declare variables
DECLARE @AdminFamiliaId UNIQUEIDENTIFIER;
SELECT @AdminFamiliaId = Id FROM Familia WHERE Nombre = 'Administrador';

IF @AdminFamiliaId IS NULL
BEGIN
    PRINT 'Familia Administrador no encontrada. Creando...';
    SET @AdminFamiliaId = NEWID();
    INSERT INTO Familia (Id, Nombre) VALUES (@AdminFamiliaId, 'Administrador');
END

-- 2. Define permissions to add
DECLARE @Permissions TABLE (
    Nombre NVARCHAR(100),
    KeyStr NVARCHAR(100)
);

INSERT INTO @Permissions (Nombre, KeyStr) VALUES
('EspacioListar', 'PERMISSION_ESPACIO_LIST'),
('EspacioCrear', 'PERMISSION_ESPACIO_CREATE'),
('EspacioModificar', 'PERMISSION_ESPACIO_UPDATE'),
('EspacioEliminar', 'PERMISSION_ESPACIO_DELETE');

-- 3. Insert permissions and assign to Admin
DECLARE @Name NVARCHAR(100);
DECLARE @Key NVARCHAR(100);
DECLARE @PatenteId UNIQUEIDENTIFIER;

DECLARE perm_cursor CURSOR FOR SELECT Nombre, KeyStr FROM @Permissions;

OPEN perm_cursor;
FETCH NEXT FROM perm_cursor INTO @Name, @Key;

WHILE @@FETCH_STATUS = 0
BEGIN
    -- Check if permission exists
    SELECT @PatenteId = Id FROM Patente WHERE DataKey = @Key;

    IF @PatenteId IS NULL
    BEGIN
        SET @PatenteId = NEWID();
        INSERT INTO Patente (Id, Nombre, TipoAcceso, DataKey)
        VALUES (@PatenteId, @Name, @Key, @Key);
        PRINT 'Permission ' + @Key + ' created.';
    END
    ELSE
    BEGIN
        PRINT 'Permission ' + @Key + ' already exists.';
    END

    -- Assign to Admin Family if not already assigned
    IF NOT EXISTS (SELECT 1 FROM FamiliaPatente WHERE IdFamilia = @AdminFamiliaId AND IdPatente = @PatenteId)
    BEGIN
        INSERT INTO FamiliaPatente (IdFamilia, IdPatente) VALUES (@AdminFamiliaId, @PatenteId);
        PRINT 'Permission ' + @Key + ' assigned to Administrador.';
    END

    FETCH NEXT FROM perm_cursor INTO @Name, @Key;
END

CLOSE perm_cursor;
DEALLOCATE perm_cursor;
GO
