-- ============================================
-- SCRIPT DE CORRECCIÓN: Agregar Columnas Faltantes
-- ============================================
-- Fecha: Enero 2025
-- Propósito: Agregar columnas CreatedBy, UpdatedBy y DeletedAt a todas las tablas
-- ============================================

USE SciFiHubDB;
GO

PRINT '=== INICIANDO CORRECCIÓN DE COLUMNAS FALTANTES ===';
GO

-- ============================================
-- 1. TABLA USUARIOS
-- ============================================
PRINT 'Agregando columnas a Usuarios...';

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Usuarios') AND name = 'CreatedBy')
BEGIN
    ALTER TABLE Usuarios ADD CreatedBy UNIQUEIDENTIFIER NULL;
    PRINT '? Columna CreatedBy agregada a Usuarios';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Usuarios') AND name = 'UpdatedBy')
BEGIN
    ALTER TABLE Usuarios ADD UpdatedBy UNIQUEIDENTIFIER NULL;
    PRINT '? Columna UpdatedBy agregada a Usuarios';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Usuarios') AND name = 'DeletedAt')
BEGIN
    ALTER TABLE Usuarios ADD DeletedAt DATETIME2 NULL;
    PRINT '? Columna DeletedAt agregada a Usuarios';
END
GO

-- ============================================
-- 2. TABLA CATEGORIAS
-- ============================================
PRINT 'Agregando columnas a Categorias...';

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Categorias') AND name = 'CreatedBy')
BEGIN
    ALTER TABLE Categorias ADD CreatedBy UNIQUEIDENTIFIER NULL;
    PRINT '? Columna CreatedBy agregada a Categorias';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Categorias') AND name = 'UpdatedBy')
BEGIN
    ALTER TABLE Categorias ADD UpdatedBy UNIQUEIDENTIFIER NULL;
    PRINT '? Columna UpdatedBy agregada a Categorias';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Categorias') AND name = 'DeletedAt')
BEGIN
    ALTER TABLE Categorias ADD DeletedAt DATETIME2 NULL;
    PRINT '? Columna DeletedAt agregada a Categorias';
END
GO

-- ============================================
-- 3. TABLA LIBROS
-- ============================================
PRINT 'Agregando columnas a Libros...';

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Libros') AND name = 'CreatedBy')
BEGIN
    ALTER TABLE Libros ADD CreatedBy UNIQUEIDENTIFIER NULL;
    PRINT '? Columna CreatedBy agregada a Libros';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Libros') AND name = 'UpdatedBy')
BEGIN
    ALTER TABLE Libros ADD UpdatedBy UNIQUEIDENTIFIER NULL;
    PRINT '? Columna UpdatedBy agregada a Libros';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Libros') AND name = 'DeletedAt')
BEGIN
    ALTER TABLE Libros ADD DeletedAt DATETIME2 NULL;
    PRINT '? Columna DeletedAt agregada a Libros';
END
GO

-- ============================================
-- 4. TABLA VENTAS
-- ============================================
PRINT 'Agregando columnas a Ventas...';

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Ventas') AND name = 'CreatedBy')
BEGIN
    ALTER TABLE Ventas ADD CreatedBy UNIQUEIDENTIFIER NULL;
    PRINT '? Columna CreatedBy agregada a Ventas';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Ventas') AND name = 'UpdatedBy')
BEGIN
    ALTER TABLE Ventas ADD UpdatedBy UNIQUEIDENTIFIER NULL;
    PRINT '? Columna UpdatedBy agregada a Ventas';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Ventas') AND name = 'DeletedAt')
BEGIN
    ALTER TABLE Ventas ADD DeletedAt DATETIME2 NULL;
    PRINT '? Columna DeletedAt agregada a Ventas';
END
GO

-- ============================================
-- 5. TABLA DETALLESVENTA
-- ============================================
PRINT 'Agregando columnas a DetallesVenta...';

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('DetallesVenta') AND name = 'CreatedBy')
BEGIN
    ALTER TABLE DetallesVenta ADD CreatedBy UNIQUEIDENTIFIER NULL;
    PRINT '? Columna CreatedBy agregada a DetallesVenta';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('DetallesVenta') AND name = 'UpdatedBy')
BEGIN
    ALTER TABLE DetallesVenta ADD UpdatedBy UNIQUEIDENTIFIER NULL;
    PRINT '? Columna UpdatedBy agregada a DetallesVenta';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('DetallesVenta') AND name = 'DeletedAt')
BEGIN
    ALTER TABLE DetallesVenta ADD DeletedAt DATETIME2 NULL;
    PRINT '? Columna DeletedAt agregada a DetallesVenta';
END
GO

-- ============================================
-- 6. TABLA CARRITOCOMPRAS
-- ============================================
PRINT 'Agregando columnas a CarritoCompras...';

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('CarritoCompras') AND name = 'CreatedBy')
BEGIN
    ALTER TABLE CarritoCompras ADD CreatedBy UNIQUEIDENTIFIER NULL;
    PRINT '? Columna CreatedBy agregada a CarritoCompras';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('CarritoCompras') AND name = 'UpdatedBy')
BEGIN
    ALTER TABLE CarritoCompras ADD UpdatedBy UNIQUEIDENTIFIER NULL;
    PRINT '? Columna UpdatedBy agregada a CarritoCompras';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('CarritoCompras') AND name = 'DeletedAt')
BEGIN
    ALTER TABLE CarritoCompras ADD DeletedAt DATETIME2 NULL;
    PRINT '? Columna DeletedAt agregada a CarritoCompras';
END
GO

-- ============================================
-- 7. TABLA DETALLESCARRITO
-- ============================================
PRINT 'Agregando columnas a DetallesCarrito...';

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('DetallesCarrito') AND name = 'CreatedBy')
BEGIN
    ALTER TABLE DetallesCarrito ADD CreatedBy UNIQUEIDENTIFIER NULL;
    PRINT '? Columna CreatedBy agregada a DetallesCarrito';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('DetallesCarrito') AND name = 'UpdatedBy')
BEGIN
    ALTER TABLE DetallesCarrito ADD UpdatedBy UNIQUEIDENTIFIER NULL;
    PRINT '? Columna UpdatedBy agregada a DetallesCarrito';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('DetallesCarrito') AND name = 'DeletedAt')
BEGIN
    ALTER TABLE DetallesCarrito ADD DeletedAt DATETIME2 NULL;
    PRINT '? Columna DeletedAt agregada a DetallesCarrito';
END
GO

-- ============================================
-- 8. TABLA AUDITORIAINVENTARIO
-- ============================================
PRINT 'Agregando columnas a AuditoriaInventario...';

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AuditoriaInventario') AND name = 'CreatedBy')
BEGIN
    ALTER TABLE AuditoriaInventario ADD CreatedBy UNIQUEIDENTIFIER NULL;
    PRINT '? Columna CreatedBy agregada a AuditoriaInventario';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AuditoriaInventario') AND name = 'UpdatedBy')
BEGIN
    ALTER TABLE AuditoriaInventario ADD UpdatedBy UNIQUEIDENTIFIER NULL;
    PRINT '? Columna UpdatedBy agregada a AuditoriaInventario';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AuditoriaInventario') AND name = 'DeletedAt')
BEGIN
    ALTER TABLE AuditoriaInventario ADD DeletedAt DATETIME2 NULL;
    PRINT '? Columna DeletedAt agregada a AuditoriaInventario';
END
GO

-- ============================================
-- VERIFICACIÓN FINAL
-- ============================================
PRINT '';
PRINT '=== VERIFICACIÓN DE COLUMNAS ===';

SELECT 
    t.name AS Tabla,
    c.name AS Columna,
    ty.name AS TipoDato
FROM sys.tables t
JOIN sys.columns c ON t.object_id = c.object_id
JOIN sys.types ty ON c.user_type_id = ty.user_type_id
WHERE c.name IN ('CreatedBy', 'UpdatedBy', 'DeletedAt')
AND t.name IN ('Usuarios', 'Categorias', 'Libros', 'Ventas', 'DetallesVenta', 
               'CarritoCompras', 'DetallesCarrito', 'AuditoriaInventario')
ORDER BY t.name, c.name;

PRINT '';
PRINT '=== CORRECCIÓN COMPLETADA ===';
GO
