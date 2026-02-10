-- Entities Creation Script

-- 1. Membresia
CREATE TABLE Membresia (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Codigo INT NOT NULL UNIQUE,
    Nombre NVARCHAR(255) NOT NULL,
    Precio DECIMAL(18,2) NOT NULL CHECK (Precio >= 0),
    Regularidad INT NOT NULL CHECK (Regularidad > 0),
    Activa BIT NOT NULL DEFAULT 1,
    Detalle NVARCHAR(MAX)
);

-- 2. Espacio
CREATE TABLE Espacio (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Nombre NVARCHAR(255) NOT NULL,
    Descripcion NVARCHAR(MAX),
    PrecioHora DECIMAL(18,2) NOT NULL CHECK (PrecioHora >= 0)
);

-- 3. Cliente (Inherits from Usuario)
CREATE TABLE Cliente (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    Apellido NVARCHAR(100) NOT NULL,
    DNI INT NOT NULL UNIQUE,
    FechaNacimiento DATETIME NOT NULL,
    MembresiaID UNIQUEIDENTIFIER NULL,
    FOREIGN KEY (Id) REFERENCES Usuario(Id) ON DELETE CASCADE,
    FOREIGN KEY (MembresiaID) REFERENCES Membresia(Id)
);

-- 4. Operador (Inherits from Usuario)
CREATE TABLE Operador (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Email NVARCHAR(255) NOT NULL,
    FechaIngreso DATETIME NOT NULL,
    FOREIGN KEY (Id) REFERENCES Usuario(Id) ON DELETE CASCADE
);

-- 5. Administrador (Inherits from Usuario)
CREATE TABLE Administrador (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Email NVARCHAR(255) NOT NULL,
    FOREIGN KEY (Id) REFERENCES Usuario(Id) ON DELETE CASCADE
);

-- 6. Agenda
CREATE TABLE Agenda (
    EspacioID UNIQUEIDENTIFIER NOT NULL,
    HoraDesde TIME NOT NULL,
    HoraHasta TIME NOT NULL,
    PRIMARY KEY (EspacioID, HoraDesde, HoraHasta),
    FOREIGN KEY (EspacioID) REFERENCES Espacio(Id) ON DELETE CASCADE,
    CHECK (HoraDesde < HoraHasta)
);

-- 7. Reserva
CREATE TABLE Reserva (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ClienteID UNIQUEIDENTIFIER NOT NULL,
    EspacioID UNIQUEIDENTIFIER NOT NULL,
    Fecha DATE NOT NULL,
    FechaHora DATETIME NOT NULL,
    Duracion INT NOT NULL,
    Adelanto DECIMAL(18,2) NOT NULL,
    CodigoReserva NVARCHAR(50) NOT NULL UNIQUE,
    Estado NVARCHAR(50) NOT NULL CHECK (Estado IN ('Pendiente', 'Pagada', 'Cancelada')),
    FOREIGN KEY (ClienteID) REFERENCES Cliente(Id),
    FOREIGN KEY (EspacioID) REFERENCES Espacio(Id)
);

CREATE INDEX IX_Reserva_Fecha ON Reserva(Fecha);

-- 8. Pago
CREATE TABLE Pago (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Codigo INT IDENTITY(1,1) NOT NULL,
    ClienteID UNIQUEIDENTIFIER NOT NULL,
    Monto DECIMAL(18,2) NOT NULL CHECK (Monto > 0),
    Metodo NVARCHAR(50) NOT NULL,
    Detalle NVARCHAR(MAX),
    Fecha DATETIME NOT NULL,
    Estado NVARCHAR(50) NOT NULL CHECK (Estado IN ('Abonado', 'Reembolsado')),
    MembresiaID UNIQUEIDENTIFIER NULL,
    ReservaID UNIQUEIDENTIFIER NULL,
    FOREIGN KEY (ClienteID) REFERENCES Cliente(Id),
    FOREIGN KEY (MembresiaID) REFERENCES Membresia(Id),
    FOREIGN KEY (ReservaID) REFERENCES Reserva(Id),
    CONSTRAINT CK_Pago_Origen CHECK (
        (MembresiaID IS NOT NULL AND ReservaID IS NULL) OR
        (MembresiaID IS NULL AND ReservaID IS NOT NULL)
    )
);

-- 9. Comprobante
CREATE TABLE Comprobante (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PagoID UNIQUEIDENTIFIER NOT NULL,
    NombreArchivo NVARCHAR(255) NOT NULL,
    RutaArchivo NVARCHAR(MAX) NOT NULL,
    FechaSubida DATETIME NOT NULL,
    FOREIGN KEY (PagoID) REFERENCES Pago(Id) ON DELETE CASCADE
);

-- 10. Movimiento
CREATE TABLE Movimiento (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ClienteID UNIQUEIDENTIFIER NOT NULL,
    Tipo NVARCHAR(50) NOT NULL,
    Monto DECIMAL(18,2) NOT NULL,
    Descripcion NVARCHAR(MAX),
    Fecha DATETIME NOT NULL,
    PagoID UNIQUEIDENTIFIER NULL,
    FOREIGN KEY (ClienteID) REFERENCES Cliente(Id),
    FOREIGN KEY (PagoID) REFERENCES Pago(Id)
);

CREATE INDEX IX_Movimiento_ClienteID ON Movimiento(ClienteID);
CREATE INDEX IX_Movimiento_Fecha ON Movimiento(Fecha);

-- 11. Rutina
CREATE TABLE Rutina (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ClienteID UNIQUEIDENTIFIER NOT NULL,
    Desde DATETIME NOT NULL,
    Hasta DATETIME NULL,
    Detalle NVARCHAR(MAX),
    FOREIGN KEY (ClienteID) REFERENCES Cliente(Id) ON DELETE CASCADE
);

-- 12. Ejercicio
CREATE TABLE Ejercicio (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    RutinaID UNIQUEIDENTIFIER NOT NULL,
    Nombre NVARCHAR(255) NOT NULL,
    Repeticiones INT NOT NULL CHECK (Repeticiones > 0),
    DiaSemana INT NOT NULL CHECK (DiaSemana BETWEEN 1 AND 7),
    Orden INT NOT NULL,
    FOREIGN KEY (RutinaID) REFERENCES Rutina(Id) ON DELETE CASCADE
);

-- 13. Vista vw_Balance
GO

CREATE VIEW vw_Balance AS
SELECT
    ClienteID,
    SUM(Monto) AS Saldo,
    MAX(Fecha) AS UltimaActualizacion
FROM Movimiento
GROUP BY ClienteID;
GO
