CREATE TABLE ClienteMembresia (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    ClienteID UNIQUEIDENTIFIER NOT NULL,
    MembresiaID UNIQUEIDENTIFIER NOT NULL,
    FechaAsignacion DATETIME NOT NULL,
    ProximaFechaPago DATETIME NULL,
    FechaBaja DATETIME NULL,
    CONSTRAINT FK_ClienteMembresia_Cliente FOREIGN KEY (ClienteID) REFERENCES Cliente(Id),
    CONSTRAINT FK_ClienteMembresia_Membresia FOREIGN KEY (MembresiaID) REFERENCES Membresia(Id)
);
