USE [IngSoftwareNegocio]
GO

-- Drop existing constraint
ALTER TABLE Pago DROP CONSTRAINT [CK__Pago__Monto__2B3F6F97]; -- Note: The name might vary, I'll use a more generic approach if needed, but usually it follows this pattern. 
-- Actually, let's just drop and recreate it if we can find the name, or use a script that finds it.

DECLARE @ConstraintName nvarchar(200)
SELECT @ConstraintName = Name FROM sys.check_constraints WHERE parent_object_id = OBJECT_ID('Pago') AND definition LIKE '%Monto%'
IF @ConstraintName IS NOT NULL
    EXEC('ALTER TABLE Pago DROP CONSTRAINT ' + @ConstraintName)

-- Add new constraint allowing 0
ALTER TABLE Pago ADD CONSTRAINT CK_Pago_Monto_NonNegative CHECK (Monto >= 0);
GO
