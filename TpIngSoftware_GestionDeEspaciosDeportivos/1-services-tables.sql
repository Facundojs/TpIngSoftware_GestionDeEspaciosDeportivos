CREATE TABLE Bitacora (
        BitacoraID UNIQUEIDENTIFIER PRIMARY KEY,
        Timestamp DATETIME NOT NULL,
        LogLevel NVARCHAR(50),
        Message NVARCHAR(MAX),
        ExceptionDetails NVARCHAR(MAX)
 );

CREATE TABLE Familia (
    Id UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    Nombre VARCHAR(100) NOT NULL,
    CONSTRAINT PK_Familia PRIMARY KEY (Id)
);

CREATE TABLE Patente (
    Id UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    Nombre VARCHAR(100) NOT NULL,
    TipoAcceso VARCHAR(100) NOT NULL,
    DataKey VARCHAR(100) NOT NULL,
    CONSTRAINT PK_Patente PRIMARY KEY (Id)
);

CREATE TABLE Usuario (
    Id UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    NombreUsuario VARCHAR(150) NOT NULL,
    Password VARCHAR(150) NOT NULL,
    Estado BIT NOT NULL,
    DigitoVerificador VARCHAR(200) NOT NULL,
    CONSTRAINT PK_Usuario PRIMARY KEY (Id)
);

CREATE TABLE FamiliaPatente (
    IdFamilia UNIQUEIDENTIFIER NOT NULL,
    IdPatente UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT PK_FamiliaPatente PRIMARY KEY (IdFamilia, IdPatente),
    CONSTRAINT FK_FamiliaPatente_Familia FOREIGN KEY (IdFamilia) REFERENCES Familia(Id),
    CONSTRAINT FK_FamiliaPatente_Patente FOREIGN KEY (IdPatente) REFERENCES Patente(Id)
);

CREATE TABLE UsuarioFamilia (
    IdUsuario UNIQUEIDENTIFIER NOT NULL,
    IdFamilia UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT PK_UsuarioFamilia PRIMARY KEY (IdUsuario, IdFamilia),
    CONSTRAINT FK_UsuarioFamilia_Usuario FOREIGN KEY (IdUsuario) REFERENCES Usuario(Id),
    CONSTRAINT FK_UsuarioFamilia_Familia FOREIGN KEY (IdFamilia) REFERENCES Familia(Id)
);