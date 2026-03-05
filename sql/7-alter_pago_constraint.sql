USE [IngSoftwareNegocio]
GO

IF EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_Pago_Origen')
BEGIN
    ALTER TABLE Pago DROP CONSTRAINT CK_Pago_Origen;
END
GO

ALTER TABLE Pago
ADD CONSTRAINT CK_Pago_Origen CHECK (
    NOT (MembresiaID IS NOT NULL AND ReservaID IS NOT NULL)
);
GO
