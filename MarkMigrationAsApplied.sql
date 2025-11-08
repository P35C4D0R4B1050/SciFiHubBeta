-- Script para marcar la migración InitialCreate como aplicada
-- Ejecutar esto en SQL Server Management Studio o sqlcmd
-- conectado a la base de datos SciFiHubDB

USE SciFiHubDB;
GO

-- Verificar si la tabla __EFMigrationsHistory existe
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = '__EFMigrationsHistory')
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
    PRINT 'Tabla __EFMigrationsHistory creada exitosamente';
END
ELSE
BEGIN
    PRINT 'Tabla __EFMigrationsHistory ya existe';
END
GO

-- Insertar el registro de la migración InitialCreate
-- IMPORTANTE: Cambia el MigrationId por el que aparece en tu carpeta Migrations
IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20251106235339_InitialCreate')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES ('20251106235339_InitialCreate', '9.0.10');
    
    PRINT 'Migración InitialCreate marcada como aplicada exitosamente';
END
ELSE
BEGIN
    PRINT 'La migración InitialCreate ya está registrada';
END
GO

-- Verificar el resultado
SELECT * FROM [__EFMigrationsHistory];
GO
