-- ============================================
-- SCRIPT DEFINITIVO: Agregar Columnas Faltantes
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
ELSE PRINT '??  Columna CreatedBy ya existe en Usuarios';

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Usuarios') AND name = 'UpdatedBy')
BEGIN
    ALTER TABLE Usuarios ADD UpdatedBy UNIQUEIDENTIFIER NULL;
    PRINT '? Columna UpdatedBy agregada a Usuarios';
END
ELSE PRINT '??  Columna UpdatedBy ya existe en Usuarios';

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Usuarios') AND name = 'DeletedAt')
BEGIN
    ALTER TABLE Usuarios ADD DeletedAt DATETIME2 NULL;
    PRINT '? Columna DeletedAt agregada a Usuarios';
END
ELSE PRINT '??  Columna DeletedAt ya existe en Usuarios';
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
ELSE PRINT '??  Columna CreatedBy ya existe en Categorias';

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Categorias') AND name = 'UpdatedBy')
BEGIN
    ALTER TABLE Categorias ADD UpdatedBy UNIQUEIDENTIFIER NULL;
    PRINT '? Columna UpdatedBy agregada a Categorias';
END
ELSE PRINT '??  Columna UpdatedBy ya existe en Categorias';

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Categorias') AND name = 'DeletedAt')
BEGIN
    ALTER TABLE Categorias ADD DeletedAt DATETIME2 NULL;
    PRINT '? Columna DeletedAt agregada a Categorias';
END
ELSE PRINT '??  Columna DeletedAt ya existe en Categorias';
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
ELSE PRINT '??  Columna CreatedBy ya existe en Libros';

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Libros') AND name = 'UpdatedBy')
BEGIN
    ALTER TABLE Libros ADD UpdatedBy UNIQUEIDENTIFIER NULL;
    PRINT '? Columna UpdatedBy agregada a Libros';
END
ELSE PRINT '??  Columna UpdatedBy ya existe en Libros';

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Libros') AND name = 'DeletedAt')
BEGIN
    ALTER TABLE Libros ADD DeletedAt DATETIME2 NULL;
    PRINT '? Columna DeletedAt agregada a Libros';
END
ELSE PRINT '??  Columna DeletedAt ya existe en Libros';
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
ELSE PRINT '??  Columna CreatedBy ya existe en Ventas';

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Ventas') AND name = 'UpdatedBy')
BEGIN
    ALTER TABLE Ventas ADD UpdatedBy UNIQUEIDENTIFIER NULL;
    PRINT '? Columna UpdatedBy agregada a Ventas';
END
ELSE PRINT '??  Columna UpdatedBy ya existe en Ventas';

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Ventas') AND name = 'DeletedAt')
BEGIN
    ALTER TABLE Ventas ADD DeletedAt DATETIME2 NULL;
    PRINT '? Columna DeletedAt agregada a Ventas';
END
ELSE PRINT '??  Columna DeletedAt ya existe en Ventas';
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
ELSE PRINT '??  Columna CreatedBy ya existe en DetallesVenta';

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('DetallesVenta') AND name = 'UpdatedBy')
BEGIN
    ALTER TABLE DetallesVenta ADD UpdatedBy UNIQUEIDENTIFIER NULL;
    PRINT '? Columna UpdatedBy agregada a DetallesVenta';
END
ELSE PRINT '??  Columna UpdatedBy ya existe en DetallesVenta';

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('DetallesVenta') AND name = 'DeletedAt')
BEGIN
    ALTER TABLE DetallesVenta ADD DeletedAt DATETIME2 NULL;
    PRINT '? Columna DeletedAt agregada a DetallesVenta';
END
ELSE PRINT '??  Columna DeletedAt ya existe en DetallesVenta';
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
ELSE PRINT '??  Columna CreatedBy ya existe en CarritoCompras';

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('CarritoCompras') AND name = 'UpdatedBy')
BEGIN
    ALTER TABLE CarritoCompras ADD UpdatedBy UNIQUEIDENTIFIER NULL;
    PRINT '? Columna UpdatedBy agregada a CarritoCompras';
END
ELSE PRINT '??  Columna UpdatedBy ya existe en CarritoCompras';

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('CarritoCompras') AND name = 'DeletedAt')
BEGIN
    ALTER TABLE CarritoCompras ADD DeletedAt DATETIME2 NULL;
    PRINT '? Columna DeletedAt agregada a CarritoCompras';
END
ELSE PRINT '??  Columna DeletedAt ya existe en CarritoCompras';
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
ELSE PRINT '??  Columna CreatedBy ya existe en DetallesCarrito';

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('DetallesCarrito') AND name = 'UpdatedBy')
BEGIN
    ALTER TABLE DetallesCarrito ADD UpdatedBy UNIQUEIDENTIFIER NULL;
    PRINT '? Columna UpdatedBy agregada a DetallesCarrito';
END
ELSE PRINT '??  Columna UpdatedBy ya existe en DetallesCarrito';

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('DetallesCarrito') AND name = 'DeletedAt')
BEGIN
    ALTER TABLE DetallesCarrito ADD DeletedAt DATETIME2 NULL;
    PRINT '? Columna DeletedAt agregada a DetallesCarrito';
END
ELSE PRINT '??  Columna DeletedAt ya existe en DetallesCarrito';
GO

-- ============================================
-- 8. TABLA AUDITORIA INVENTARIO (DETECTAR NOMBRE AUTOMÁTICAMENTE)
-- ============================================
PRINT '';
PRINT '=== DETECTANDO NOMBRE DE TABLA DE AUDITORÍA ===';

DECLARE @NombreTablaAuditoria NVARCHAR(128);

-- Buscar el nombre exacto de la tabla
SELECT @NombreTablaAuditoria = name 
FROM sys.tables 
WHERE name LIKE '%Audit%Inventario%' OR name LIKE 'Auditoria%Inventario%';

IF @NombreTablaAuditoria IS NULL
BEGIN
    PRINT '??  No se encontró tabla de auditoría de inventario';
    PRINT '   Tablas disponibles:';
    SELECT name FROM sys.tables WHERE name LIKE '%Audit%' OR name LIKE '%Inventario%';
END
ELSE
BEGIN
    PRINT '? Tabla encontrada: ' + @NombreTablaAuditoria;
    PRINT 'Agregando columnas a ' + @NombreTablaAuditoria + '...';
    
    -- Agregar CreatedBy
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(@NombreTablaAuditoria) AND name = 'CreatedBy')
    BEGIN
        DECLARE @SqlCreatedBy NVARCHAR(MAX) = 'ALTER TABLE ' + QUOTENAME(@NombreTablaAuditoria) + ' ADD CreatedBy UNIQUEIDENTIFIER NULL';
        EXEC sp_executesql @SqlCreatedBy;
        PRINT '? Columna CreatedBy agregada a ' + @NombreTablaAuditoria;
    END
    ELSE
        PRINT '??  Columna CreatedBy ya existe en ' + @NombreTablaAuditoria;
    
    -- Agregar UpdatedBy
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(@NombreTablaAuditoria) AND name = 'UpdatedBy')
    BEGIN
        DECLARE @SqlUpdatedBy NVARCHAR(MAX) = 'ALTER TABLE ' + QUOTENAME(@NombreTablaAuditoria) + ' ADD UpdatedBy UNIQUEIDENTIFIER NULL';
        EXEC sp_executesql @SqlUpdatedBy;
        PRINT '? Columna UpdatedBy agregada a ' + @NombreTablaAuditoria;
    END
    ELSE
        PRINT '??  Columna UpdatedBy ya existe en ' + @NombreTablaAuditoria;
    
    -- Agregar DeletedAt
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(@NombreTablaAuditoria) AND name = 'DeletedAt')
    BEGIN
        DECLARE @SqlDeletedAt NVARCHAR(MAX) = 'ALTER TABLE ' + QUOTENAME(@NombreTablaAuditoria) + ' ADD DeletedAt DATETIME2 NULL';
        EXEC sp_executesql @SqlDeletedAt;
        PRINT '? Columna DeletedAt agregada a ' + @NombreTablaAuditoria;
    END
    ELSE
        PRINT '??  Columna DeletedAt ya existe en ' + @NombreTablaAuditoria;
END
GO

-- ============================================
-- VERIFICACIÓN FINAL
-- ============================================
PRINT '';
PRINT '=== VERIFICACIÓN DE COLUMNAS AGREGADAS ===';

-- Detectar nombre de tabla de auditoría para verificación
DECLARE @NombreTabla NVARCHAR(128);
SELECT @NombreTabla = name FROM sys.tables WHERE name LIKE '%Audit%Inventario%' OR name LIKE 'Auditoria%Inventario%';

-- Mostrar todas las columnas agregadas
SELECT 
    t.name AS Tabla,
    c.name AS Columna,
    ty.name AS TipoDato,
    CASE WHEN c.is_nullable = 1 THEN 'Sí' ELSE 'No' END AS PermiteNULL
FROM sys.tables t
JOIN sys.columns c ON t.object_id = c.object_id
JOIN sys.types ty ON c.user_type_id = ty.user_type_id
WHERE c.name IN ('CreatedBy', 'UpdatedBy', 'DeletedAt')
AND (
    t.name IN ('Usuarios', 'Categorias', 'Libros', 'Ventas', 'DetallesVenta', 'CarritoCompras', 'DetallesCarrito')
    OR t.name = @NombreTabla
)
ORDER BY t.name, c.name;

-- Contar total de columnas agregadas
PRINT '';
PRINT '=== RESUMEN ===';

SELECT 
    t.name AS Tabla,
    COUNT(*) AS Columnas_BaseEntity
FROM sys.tables t
JOIN sys.columns c ON t.object_id = c.object_id
WHERE c.name IN ('CreatedBy', 'UpdatedBy', 'DeletedAt')
AND (
    t.name IN ('Usuarios', 'Categorias', 'Libros', 'Ventas', 'DetallesVenta', 'CarritoCompras', 'DetallesCarrito')
    OR t.name = @NombreTabla
)
GROUP BY t.name
ORDER BY t.name;

-- Total general
DECLARE @TotalColumnas INT;
SELECT @TotalColumnas = COUNT(*)
FROM sys.tables t
JOIN sys.columns c ON t.object_id = c.object_id
WHERE c.name IN ('CreatedBy', 'UpdatedBy', 'DeletedAt')
AND (
    t.name IN ('Usuarios', 'Categorias', 'Libros', 'Ventas', 'DetallesVenta', 'CarritoCompras', 'DetallesCarrito')
    OR t.name = @NombreTabla
);

PRINT '';
PRINT 'Total de columnas agregadas: ' + CAST(@TotalColumnas AS VARCHAR(10)) + ' de 24 esperadas';

IF @TotalColumnas = 24
    PRINT '??? CORRECCIÓN COMPLETADA EXITOSAMENTE ???';
ELSE
    PRINT '??  ATENCIÓN: Faltan ' + CAST(24 - @TotalColumnas AS VARCHAR(10)) + ' columnas';

PRINT '';
PRINT '=== FIN DEL SCRIPT ===';
GO
