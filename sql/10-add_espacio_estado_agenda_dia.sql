ALTER TABLE Espacio ADD Estado NVARCHAR(50) NOT NULL DEFAULT 'Activo';
GO

DECLARE @ConstraintName nvarchar(200)
SELECT @ConstraintName = Name FROM SYS.KEY_CONSTRAINTS
WHERE type = 'PK' AND OBJECT_NAME(parent_object_id) = OBJECT_ID('Agenda')

IF @ConstraintName IS NOT NULL
BEGIN
    EXEC('ALTER TABLE Agenda DROP CONSTRAINT ' + @ConstraintName)
END
GO

ALTER TABLE Agenda ADD DiaSemana INT NOT NULL DEFAULT 0 CHECK (DiaSemana BETWEEN 0 AND 6);
GO

ALTER TABLE Agenda ADD CONSTRAINT PK_Agenda PRIMARY KEY (EspacioID, DiaSemana, HoraDesde, HoraHasta);
GO