-- =============================================
-- DIAGN?STICO COMPLETO - SciFiHub Database
-- Verifica estado actual de la BD y columnas
-- =============================================

USE SciFiHubDB;
GO

PRINT '==============================================';
PRINT 'DIAGN?STICO COMPLETO - SciFiHub Database';
PRINT '==============================================';
PRINT '';

-- =============================================
-- 1. VERIFICAR ESTRUCTURA DE TABLA LIBROS
-- =============================================
PRINT '1. ESTRUCTURA DE TABLA LIBROS:';
PRINT '----------------------------------------------';

SELECT 
    COLUMN_NAME AS Columna,
    DATA_TYPE AS TipoDato,
    CHARACTER_MAXIMUM_LENGTH AS TamañoMax,
    IS_NULLABLE AS Nullable
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Libros'
ORDER BY ORDINAL_POSITION;

PRINT '';

-- =============================================
-- 2. VERIFICAR QUE EXISTEN LAS COLUMNAS CORRECTAS
-- =============================================
PRINT '2. VERIFICACI?N DE COLUMNAS CR?TICAS:';
PRINT '----------------------------------------------';

DECLARE @HasAnioPublicacion BIT = 0;
DECLARE @HasNumeroPaginas BIT = 0;

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
           WHERE TABLE_NAME = 'Libros' AND COLUMN_NAME = 'AnioPublicacion')
    SET @HasAnioPublicacion = 1;

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
           WHERE TABLE_NAME = 'Libros' AND COLUMN_NAME = 'NumeroPaginas')
    SET @HasNumeroPaginas = 1;

SELECT 
    'AnioPublicacion' AS Columna,
    CASE WHEN @HasAnioPublicacion = 1 THEN '? EXISTE' ELSE '? NO EXISTE' END AS Estado

UNION ALL

SELECT 
    'NumeroPaginas' AS Columna,
    CASE WHEN @HasNumeroPaginas = 1 THEN '? EXISTE' ELSE '? NO EXISTE' END AS Estado;

PRINT '';

-- =============================================
-- 3. VERIFICAR ESTRUCTURA DE TABLA USUARIOS
-- =============================================
PRINT '3. ESTRUCTURA DE TABLA USUARIOS:';
PRINT '----------------------------------------------';

SELECT 
    COLUMN_NAME AS Columna,
    DATA_TYPE AS TipoDato,
    CHARACTER_MAXIMUM_LENGTH AS TamañoMax,
    IS_NULLABLE AS Nullable
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Usuarios'
ORDER BY ORDINAL_POSITION;

PRINT '';

-- =============================================
-- 4. VERIFICAR VALORES DE ROL Y ESTADO
-- =============================================
PRINT '4. VALORES ACTUALES DE ROL Y ESTADO:';
PRINT '----------------------------------------------';

SELECT 
    Rol,
    Estado,
    COUNT(*) AS Cantidad
FROM Usuarios
WHERE IsDeleted = 0
GROUP BY Rol, Estado
ORDER BY Rol, Estado;

PRINT '';

-- =============================================
-- 5. VERIFICAR ESTRUCTURA DE TABLA VENTAS
-- =============================================
PRINT '5. ESTRUCTURA DE TABLA VENTAS:';
PRINT '----------------------------------------------';

SELECT 
    COLUMN_NAME AS Columna,
    DATA_TYPE AS TipoDato,
    CHARACTER_MAXIMUM_LENGTH AS TamañoMax,
    IS_NULLABLE AS Nullable
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Ventas'
ORDER BY ORDINAL_POSITION;

PRINT '';

-- =============================================
-- 6. VERIFICAR CAMPOS DE DIRECCI?N EN VENTAS
-- =============================================
PRINT '6. VERIFICACI?N DE CAMPOS DE DIRECCI?N:';
PRINT '----------------------------------------------';

DECLARE @HasDireccionCalle BIT = 0;
DECLARE @HasDireccionCiudad BIT = 0;
DECLARE @HasDireccionDepartamento BIT = 0;
DECLARE @HasDireccionPais BIT = 0;

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
           WHERE TABLE_NAME = 'Ventas' AND COLUMN_NAME = 'DireccionCalle')
    SET @HasDireccionCalle = 1;

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
           WHERE TABLE_NAME = 'Ventas' AND COLUMN_NAME = 'DireccionCiudad')
    SET @HasDireccionCiudad = 1;

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
           WHERE TABLE_NAME = 'Ventas' AND COLUMN_NAME = 'DireccionDepartamento')
    SET @HasDireccionDepartamento = 1;

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
           WHERE TABLE_NAME = 'Ventas' AND COLUMN_NAME = 'DireccionPais')
    SET @HasDireccionPais = 1;

SELECT 
    'DireccionCalle' AS Campo,
    CASE WHEN @HasDireccionCalle = 1 THEN '? EXISTE' ELSE '? NO EXISTE' END AS Estado

UNION ALL

SELECT 
    'DireccionCiudad' AS Campo,
    CASE WHEN @HasDireccionCiudad = 1 THEN '? EXISTE' ELSE '? NO EXISTE' END AS Estado

UNION ALL

SELECT 
    'DireccionDepartamento' AS Campo,
    CASE WHEN @HasDireccionDepartamento = 1 THEN '? EXISTE' ELSE '? NO EXISTE' END AS Estado

UNION ALL

SELECT 
    'DireccionPais' AS Campo,
    CASE WHEN @HasDireccionPais = 1 THEN '? EXISTE' ELSE '? NO EXISTE' END AS Estado;

PRINT '';

-- =============================================
-- 7. VERIFICAR CONSTRAINTS DE TABLAS
-- =============================================
PRINT '7. CONSTRAINTS ACTIVOS:';
PRINT '----------------------------------------------';

SELECT 
    t.name AS Tabla,
    c.name AS NombreConstraint,
    c.type_desc AS Tipo
FROM sys.check_constraints c
INNER JOIN sys.tables t ON c.parent_object_id = t.object_id
WHERE t.name IN ('Usuarios', 'Libros', 'Ventas')
ORDER BY t.name, c.name;

PRINT '';

-- =============================================
-- 8. VERIFICAR DATOS DE PRUEBA
-- =============================================
PRINT '8. DATOS DE PRUEBA EXISTENTES:';
PRINT '----------------------------------------------';

SELECT 
    'Categorías' AS Tabla,
    COUNT(*) AS Cantidad
FROM Categorias
WHERE IsDeleted = 0

UNION ALL

SELECT 
    'Libros' AS Tabla,
    COUNT(*) AS Cantidad
FROM Libros
WHERE IsDeleted = 0

UNION ALL

SELECT 
    'Usuarios' AS Tabla,
    COUNT(*) AS Cantidad
FROM Usuarios
WHERE IsDeleted = 0

UNION ALL

SELECT 
    'Ventas' AS Tabla,
    COUNT(*) AS Cantidad
FROM Ventas
WHERE IsDeleted = 0;

PRINT '';

-- =============================================
-- 9. VERIFICAR LIBROS CON DATOS COMPLETOS
-- =============================================
PRINT '9. LIBROS CON DATOS COMPLETOS:';
PRINT '----------------------------------------------';

SELECT 
    COUNT(*) AS TotalLibros,
    SUM(CASE WHEN AnioPublicacion IS NOT NULL THEN 1 ELSE 0 END) AS ConAnio,
    SUM(CASE WHEN NumeroPaginas IS NOT NULL THEN 1 ELSE 0 END) AS ConPaginas,
    SUM(CASE WHEN AnioPublicacion IS NULL THEN 1 ELSE 0 END) AS SinAnio,
    SUM(CASE WHEN NumeroPaginas IS NULL THEN 1 ELSE 0 END) AS SinPaginas
FROM Libros
WHERE IsDeleted = 0;

PRINT '';

-- =============================================
-- 10. VERIFICAR EJEMPLO DE LIBROS
-- =============================================
PRINT '10. EJEMPLO DE LIBROS (Primeros 5):';
PRINT '----------------------------------------------';

SELECT TOP 5
    ISBN,
    Titulo,
    AnioPublicacion,
    NumeroPaginas,
    Precio,
    Stock
FROM Libros
WHERE IsDeleted = 0
ORDER BY FechaIngreso DESC;

PRINT '';

-- =============================================
-- RESUMEN FINAL
-- =============================================
PRINT '==============================================';
PRINT 'RESUMEN DEL DIAGN?STICO:';
PRINT '==============================================';
PRINT '';

-- Verificar estado general
IF @HasAnioPublicacion = 1 AND @HasNumeroPaginas = 1
BEGIN
    PRINT '? ESTRUCTURA DE LIBROS: CORRECTA';
    PRINT '  - Columna AnioPublicacion: EXISTE';
    PRINT '  - Columna NumeroPaginas: EXISTE';
END
ELSE
BEGIN
    PRINT '? ESTRUCTURA DE LIBROS: INCOMPLETA';
    IF @HasAnioPublicacion = 0
        PRINT '  - Columna AnioPublicacion: FALTA';
    IF @HasNumeroPaginas = 0
        PRINT '  - Columna NumeroPaginas: FALTA';
END

PRINT '';

IF @HasDireccionCalle = 1 AND @HasDireccionCiudad = 1 AND @HasDireccionDepartamento = 1
BEGIN
    PRINT '? ESTRUCTURA DE VENTAS: CORRECTA';
    PRINT '  - Campos de dirección: EXISTEN';
END
ELSE
BEGIN
    PRINT '? ESTRUCTURA DE VENTAS: INCOMPLETA';
    PRINT '  - Algunos campos de dirección faltan';
END

PRINT '';
PRINT '==============================================';
PRINT 'FIN DEL DIAGN?STICO';
PRINT '==============================================';
GO
