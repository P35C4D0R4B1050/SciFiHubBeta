-- ============================================
-- SCRIPT DE VERIFICACIÓN Y CORRECCIÓN
-- Nombre de Tabla: AuditoriaInventario (SINGULAR)
-- ============================================

USE SciFiHubDB;
GO

PRINT '=== DIAGNÓSTICO DE TABLA AUDITORIAINVENTARIO ===';
GO

-- 1. Verificar si la tabla existe y cuál es su nombre exacto
PRINT 'Buscando tabla de auditoría...';
SELECT 
    name AS NombreTabla,
    create_date AS FechaCreacion
FROM sys.tables
WHERE name LIKE '%Audit%' OR name LIKE '%Inventario%';
GO

-- 2. Ver columnas actuales de la tabla
PRINT '';
PRINT 'Columnas actuales en la tabla de auditoría:';
SELECT 
    c.name AS Columna,
    t.name AS TipoDato,
    c.is_nullable AS PermiteNULL
FROM sys.tables tab
JOIN sys.columns c ON tab.object_id = c.object_id
JOIN sys.types t ON c.user_type_id = t.user_type_id
WHERE tab.name LIKE '%Audit%' OR tab.name LIKE '%Inventario%'
ORDER BY c.column_id;
GO

-- 3. Agregar columnas faltantes a AuditoriaInventario (SINGULAR - CORRECTO)
PRINT '';
PRINT 'Agregando columnas faltantes a AuditoriaInventario...';

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AuditoriaInventario') AND name = 'CreatedBy')
BEGIN
    ALTER TABLE AuditoriaInventario ADD CreatedBy UNIQUEIDENTIFIER NULL;
    PRINT '? Columna CreatedBy agregada a AuditoriaInventario';
END
ELSE
BEGIN
    PRINT '?? Columna CreatedBy ya existe en AuditoriaInventario';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AuditoriaInventario') AND name = 'UpdatedBy')
BEGIN
    ALTER TABLE AuditoriaInventario ADD UpdatedBy UNIQUEIDENTIFIER NULL;
    PRINT '? Columna UpdatedBy agregada a AuditoriaInventario';
END
ELSE
BEGIN
    PRINT '?? Columna UpdatedBy ya existe en AuditoriaInventario';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AuditoriaInventario') AND name = 'DeletedAt')
BEGIN
    ALTER TABLE AuditoriaInventario ADD DeletedAt DATETIME2 NULL;
    PRINT '? Columna DeletedAt agregada a AuditoriaInventario';
END
ELSE
BEGIN
    PRINT '?? Columna DeletedAt ya existe en AuditoriaInventario';
END
GO

-- 4. VERIFICACIÓN FINAL
PRINT '';
PRINT '=== VERIFICACIÓN FINAL ===';

-- Contar columnas agregadas por tabla
SELECT 
    t.name AS Tabla,
    COUNT(*) AS Columnas_BaseEntity
FROM sys.tables t
JOIN sys.columns c ON t.object_id = c.object_id
WHERE c.name IN ('CreatedBy', 'UpdatedBy', 'DeletedAt')
AND t.name IN ('Usuarios', 'Categorias', 'Libros', 'Ventas', 'DetallesVenta', 
               'CarritoCompras', 'DetallesCarrito', 'AuditoriaInventario')
GROUP BY t.name
ORDER BY t.name;

-- Resultado esperado: 8 tablas, cada una con 3 columnas

PRINT '';
PRINT '=== Total de columnas agregadas ===';

SELECT COUNT(*) AS Total_Columnas_Agregadas
FROM sys.tables t
JOIN sys.columns c ON t.object_id = c.object_id
WHERE c.name IN ('CreatedBy', 'UpdatedBy', 'DeletedAt')
AND t.name IN ('Usuarios', 'Categorias', 'Libros', 'Ventas', 'DetallesVenta', 
               'CarritoCompras', 'DetallesCarrito', 'AuditoriaInventario');

-- Debe ser 24 (8 tablas × 3 columnas)

PRINT '';
PRINT '=== CORRECCIÓN COMPLETADA ===';
GO
