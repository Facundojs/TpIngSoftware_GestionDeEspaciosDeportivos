USE [IngSoftwareNegocio]
GO

-- 6-alter_rutina_ejercicio.sql
-- Migration script to refactor Ejercicio and create RutinaEjercicio

BEGIN TRANSACTION;

-- 1. Create temporary table to store existing exercises linked to routines (if any)
-- Assuming we want to preserve data, though structure changes significantly (exercises become unique by name)
-- For simplicity in this migration, and since this is likely dev, we will drop and recreate to match new schema cleanly
-- But strict instruction says "alter table".

-- Rename old table to backup
EXEC sp_rename 'Ejercicio', 'Ejercicio_Old';

-- 2. Create new Ejercicio table (Strong Entity)
CREATE TABLE Ejercicio (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Nombre NVARCHAR(255) NOT NULL UNIQUE
);

-- 3. Create RutinaEjercicio table (Intermediate)
CREATE TABLE RutinaEjercicio (
    RutinaID UNIQUEIDENTIFIER NOT NULL,
    EjercicioID UNIQUEIDENTIFIER NOT NULL,
    Repeticiones INT NOT NULL CHECK (Repeticiones > 0),
    DiaSemana INT NOT NULL CHECK (DiaSemana BETWEEN 1 AND 7),
    Orden INT NOT NULL,
    PRIMARY KEY (RutinaID, EjercicioID),
    FOREIGN KEY (RutinaID) REFERENCES Rutina(Id) ON DELETE CASCADE,
    FOREIGN KEY (EjercicioID) REFERENCES Ejercicio(Id) ON DELETE CASCADE
);

-- 4. Migrate Data
-- Insert unique exercises from old table
INSERT INTO Ejercicio (Id, Nombre)
SELECT NEWID(), Nombre
FROM (SELECT DISTINCT Nombre FROM Ejercicio_Old) AS DistinctExercises;

-- Insert links into RutinaEjercicio
-- We need to join old table with new Ejercicio table by Name to get the new EjercicioID
INSERT INTO RutinaEjercicio (RutinaID, EjercicioID, Repeticiones, DiaSemana, Orden)
SELECT
    old.RutinaID,
    new.Id,
    old.Repeticiones,
    old.DiaSemana,
    old.Orden
FROM Ejercicio_Old old
JOIN Ejercicio new ON old.Nombre = new.Nombre;

-- 5. Clean up
DROP TABLE Ejercicio_Old;

COMMIT;
GO
