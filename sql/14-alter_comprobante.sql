USE [IngSoftwareNegocio]
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Comprobante]') AND name = 'ReservaID')
BEGIN
    ALTER TABLE [dbo].[Comprobante] ADD [ReservaID] UNIQUEIDENTIFIER NULL;
    ALTER TABLE [dbo].[Comprobante] ADD CONSTRAINT FK_Comprobante_Reserva FOREIGN KEY (ReservaID) REFERENCES Reserva(Id);
END
GO

DECLARE @FkName NVARCHAR(200)
SELECT @FkName = fk.name
FROM sys.foreign_keys fk
JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
JOIN sys.columns c ON fkc.parent_column_id = c.column_id AND fkc.parent_object_id = c.object_id
WHERE fk.parent_object_id = OBJECT_ID('Comprobante') AND c.name = 'PagoID'

IF @FkName IS NOT NULL
    EXEC('ALTER TABLE Comprobante DROP CONSTRAINT ' + @FkName)
GO

ALTER TABLE [dbo].[Comprobante] ALTER COLUMN [PagoID] UNIQUEIDENTIFIER NULL;
GO

ALTER TABLE [dbo].[Comprobante] ADD CONSTRAINT FK_Comprobante_Pago FOREIGN KEY (PagoID) REFERENCES Pago(Id);
GO

ALTER TABLE [dbo].[Comprobante] ADD CONSTRAINT CK_Comprobante_Origen CHECK (
    PagoID IS NOT NULL OR ReservaID IS NOT NULL
);
GO