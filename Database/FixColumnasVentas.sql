-- =============================================
-- SCRIPT: Agregar Columnas de BaseEntity a Todas las Tablas
-- Descripción: Agrega las columnas de auditoría faltantes
-- Fecha: 2025-01-09
-- =============================================

USE SciFiHubDB;
GO

PRINT '==============================================';
PRINT 'AGREGANDO COLUMNAS DE BASEENTITY';
PRINT '==============================================';
PRINT '';

-- =============================================
-- 1. TABLA VENTAS
-- =============================================
PRINT '1. Procesando tabla VENTAS...';

-- Verificar y agregar CreatedAt
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Ventas' AND COLUMN_NAME = 'CreatedAt')
BEGIN
    ALTER TABLE Ventas ADD CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE();
    PRINT '? Columna CreatedAt agregada a Ventas';
END
ELSE
    PRINT '?? Columna CreatedAt ya existe en Ventas';

-- Verificar y agregar UpdatedAt
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Ventas' AND COLUMN_NAME = 'UpdatedAt')
BEGIN
    ALTER TABLE Ventas ADD UpdatedAt DATETIME2 NULL;
    PRINT '? Columna UpdatedAt agregada a Ventas';
END
ELSE
    PRINT '?? Columna UpdatedAt ya existe en Ventas';

-- Verificar y agregar CreatedBy
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Ventas' AND COLUMN_NAME = 'CreatedBy')
BEGIN
    ALTER TABLE Ventas ADD CreatedBy UNIQUEIDENTIFIER NULL;
    PRINT '? Columna CreatedBy agregada a Ventas';
END
ELSE
    PRINT '?? Columna CreatedBy ya existe en Ventas';

-- Verificar y agregar UpdatedBy
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Ventas' AND COLUMN_NAME = 'UpdatedBy')
BEGIN
    ALTER TABLE Ventas ADD UpdatedBy UNIQUEIDENTIFIER NULL;
    PRINT '? Columna UpdatedBy agregada a Ventas';
END
ELSE
    PRINT '?? Columna UpdatedBy ya existe en Ventas';

-- Verificar y agregar IsDeleted
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Ventas' AND COLUMN_NAME = 'IsDeleted')
BEGIN
    ALTER TABLE Ventas ADD IsDeleted BIT NOT NULL DEFAULT 0;
    PRINT '? Columna IsDeleted agregada a Ventas';
END
ELSE
    PRINT '?? Columna IsDeleted ya existe en Ventas';

-- Verificar y agregar DeletedAt
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Ventas' AND COLUMN_NAME = 'DeletedAt')
BEGIN
    ALTER TABLE Ventas ADD DeletedAt DATETIME2 NULL;
    PRINT '? Columna DeletedAt agregada a Ventas';
END
ELSE
    PRINT '?? Columna DeletedAt ya existe en Ventas';

PRINT '';

-- =============================================
-- 2. TABLA DETALLESVENTAS
-- =============================================
PRINT '2. Procesando tabla DETALLESVENTAS...';

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'DetallesVenta' AND COLUMN_NAME = 'CreatedAt')
BEGIN
    ALTER TABLE DetallesVenta ADD CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE();
    PRINT '? Columna CreatedAt agregada a DetallesVenta';
END
ELSE
    PRINT '?? Columna CreatedAt ya existe en DetallesVenta';

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'DetallesVenta' AND COLUMN_NAME = 'UpdatedAt')
BEGIN
    ALTER TABLE DetallesVenta ADD UpdatedAt DATETIME2 NULL;
    PRINT '? Columna UpdatedAt agregada a DetallesVenta';
END
ELSE
    PRINT '?? Columna UpdatedAt ya existe en DetallesVenta';

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'DetallesVenta' AND COLUMN_NAME = 'CreatedBy')
BEGIN
    ALTER TABLE DetallesVenta ADD CreatedBy UNIQUEIDENTIFIER NULL;
    PRINT '? Columna CreatedBy agregada a DetallesVenta';
END
ELSE
    PRINT '?? Columna CreatedBy ya existe en DetallesVenta';

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'DetallesVenta' AND COLUMN_NAME = 'UpdatedBy')
BEGIN
    ALTER TABLE DetallesVenta ADD UpdatedBy UNIQUEIDENTIFIER NULL;
    PRINT '? Columna UpdatedBy agregada a DetallesVenta';
END
ELSE
    PRINT '?? Columna UpdatedBy ya existe en DetallesVenta';

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'DetallesVenta' AND COLUMN_NAME = 'IsDeleted')
BEGIN
    ALTER TABLE DetallesVenta ADD IsDeleted BIT NOT NULL DEFAULT 0;
    PRINT '? Columna IsDeleted agregada a DetallesVenta';
END
ELSE
    PRINT '?? Columna IsDeleted ya existe en DetallesVenta';

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'DetallesVenta' AND COLUMN_NAME = 'DeletedAt')
BEGIN
    ALTER TABLE DetallesVenta ADD DeletedAt DATETIME2 NULL;
    PRINT '? Columna DeletedAt agregada a DetallesVenta';
END
ELSE
    PRINT '?? Columna DeletedAt ya existe en DetallesVenta';

PRINT '';

-- =============================================
-- 3. VERIFICAR TODAS LAS TABLAS
-- =============================================
PRINT '3. Verificando estado de todas las tablas...';
PRINT '';

SELECT 
    t.name AS Tabla,
    SUM(CASE WHEN c.name = 'CreatedAt' THEN 1 ELSE 0 END) AS CreatedAt,
    SUM(CASE WHEN c.name = 'UpdatedAt' THEN 1 ELSE 0 END) AS UpdatedAt,
    SUM(CASE WHEN c.name = 'CreatedBy' THEN 1 ELSE 0 END) AS CreatedBy,
    SUM(CASE WHEN c.name = 'UpdatedBy' THEN 1 ELSE 0 END) AS UpdatedBy,
    SUM(CASE WHEN c.name = 'IsDeleted' THEN 1 ELSE 0 END) AS IsDeleted,
    SUM(CASE WHEN c.name = 'DeletedAt' THEN 1 ELSE 0 END) AS DeletedAt,
    CASE 
        WHEN SUM(CASE WHEN c.name IN ('CreatedAt', 'UpdatedAt', 'CreatedBy', 'UpdatedBy', 'IsDeleted', 'DeletedAt') THEN 1 ELSE 0 END) = 6 
        THEN '? COMPLETO'
        ELSE '? INCOMPLETO'
    END AS Estado
FROM sys.tables t
LEFT JOIN sys.columns c ON t.object_id = c.object_id 
    AND c.name IN ('CreatedAt', 'UpdatedAt', 'CreatedBy', 'UpdatedBy', 'IsDeleted', 'DeletedAt')
WHERE t.name IN ('Usuarios', 'Categorias', 'Libros', 'Ventas', 'DetallesVenta', 
                 'CarritoCompras', 'DetallesCarrito', 'AuditoriasInventario')
GROUP BY t.name
ORDER BY t.name;

PRINT '';
PRINT '==============================================';
PRINT 'SCRIPT COMPLETADO';
PRINT '==============================================';

-- =============================================
-- 4. ACTUALIZAR VALORES POR DEFECTO
-- =============================================
PRINT '';
PRINT '4. Actualizando valores por defecto...';

-- Actualizar Ventas existentes
UPDATE Ventas 
SET CreatedAt = ISNULL(CreatedAt, FechaVenta),
    IsDeleted = ISNULL(IsDeleted, 0)
WHERE CreatedAt IS NULL OR IsDeleted IS NULL;

PRINT '? Valores por defecto actualizados en Ventas';

-- Actualizar DetallesVenta existentes
UPDATE DetallesVenta 
SET CreatedAt = ISNULL(CreatedAt, GETUTCDATE()),
    IsDeleted = ISNULL(IsDeleted, 0)
WHERE CreatedAt IS NULL OR IsDeleted IS NULL;

PRINT '? Valores por defecto actualizados en DetallesVenta';

PRINT '';
PRINT '==============================================';
PRINT '? TODAS LAS COLUMNAS AGREGADAS EXITOSAMENTE';
PRINT '==============================================';
GO
