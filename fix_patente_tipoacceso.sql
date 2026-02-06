BEGIN TRANSACTION;

-- 1. Backup existing data
SELECT * INTO #PatenteBackup FROM Patente;

-- 2. Drop Foreign Key
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_FamiliaPatente_Patente]') AND parent_object_id = OBJECT_ID(N'[dbo].[FamiliaPatente]'))
    ALTER TABLE FamiliaPatente DROP CONSTRAINT FK_FamiliaPatente_Patente;

-- 3. Drop Table
IF OBJECT_ID('dbo.Patente', 'U') IS NOT NULL
    DROP TABLE Patente;

-- 4. Recreate Table with correct column type (NVARCHAR(100))
CREATE TABLE Patente (
    Id UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    Nombre VARCHAR(100) NOT NULL,
    TipoAcceso NVARCHAR(100) NOT NULL,
    DataKey VARCHAR(100) NOT NULL,
    CONSTRAINT PK_Patente PRIMARY KEY (Id)
);

-- 5. Restore Data
-- Converting the old INT values to string representation so we don't lose data.
-- Ideally these should be updated to their string key constants later.
INSERT INTO Patente (Id, Nombre, TipoAcceso, DataKey)
SELECT Id, Nombre, CAST(TipoAcceso AS NVARCHAR(100)), DataKey FROM #PatenteBackup;

-- 6. Restore Foreign Key
ALTER TABLE FamiliaPatente
ADD CONSTRAINT FK_FamiliaPatente_Patente FOREIGN KEY (IdPatente) REFERENCES Patente(Id);

-- 7. Clean up
DROP TABLE #PatenteBackup;

COMMIT TRANSACTION;
