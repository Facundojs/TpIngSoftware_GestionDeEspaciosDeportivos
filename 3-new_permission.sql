-- Script to add the new BitacoraVer permission
-- Run this script in your database to add the new permission.

IF NOT EXISTS (SELECT 1 FROM Patente WHERE DataKey = 'PERMISSION_LOG_VIEW')
BEGIN
    INSERT INTO Patente (Id, Nombre, TipoAcceso, DataKey)
    VALUES (NEWID(), 'BitacoraVer', 'PERMISSION_LOG_VIEW', 'PERMISSION_LOG_VIEW');

    PRINT 'Permission PERMISSION_LOG_VIEW added successfully.';
END
ELSE
BEGIN
    PRINT 'Permission PERMISSION_LOG_VIEW already exists.';
END
