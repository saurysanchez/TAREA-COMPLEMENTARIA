CREATE DATABASE ClinicaDB;
GO

USE ClinicaDB;
GO

CREATE TABLE Pacientes (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Cedula VARCHAR(50) NOT NULL UNIQUE,
    Nombre VARCHAR(100) NOT NULL,
    Telefono VARCHAR(50) NOT NULL,
    Edad INT NOT NULL
);
GO

INSERT INTO Pacientes (Cedula, Nombre, Telefono, Edad) VALUES
('001-1234567-8', 'Ana Rosario', '809-555-1111', 30),
('002-9876543-2', 'Carlos Mejía', '829-555-2222', 45),
('003-4567890-1', 'Elena Pimentel', '849-555-3333', 28);
GO