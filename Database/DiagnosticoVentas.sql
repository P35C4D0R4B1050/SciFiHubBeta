-- ========================================
-- DIAGNÓSTICO COMPLETO - Tabla Ventas
-- ========================================

USE SciFiHubDB;
GO

PRINT '========================================';
PRINT 'DIAGNÓSTICO DE TABLA VENTAS';
PRINT '========================================';
PRINT '';

-- 1. Verificar si la tabla existe
PRINT '1. VERIFICACIÓN DE EXISTENCIA:';
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Ventas')
    PRINT '   ? Tabla Ventas EXISTE';
ELSE
BEGIN
    PRINT '   ? Tabla Ventas NO EXISTE';
    RETURN;
END
PRINT '';

-- 2. Listar TODAS las columnas con tipo de dato
PRINT '2. COLUMNAS EXISTENTES EN VENTAS:';
PRINT '   Columna                      | Tipo            | NULL  | Default';
PRINT '   ----------------------------|-----------------|-------|------------------';

SELECT 
    '   ' + COLUMN_NAME AS Columna,
    '| ' + DATA_TYPE + 
    CASE 
        WHEN CHARACTER_MAXIMUM_LENGTH IS NOT NULL 
        THEN '(' + CAST(CHARACTER_MAXIMUM_LENGTH AS VARCHAR) + ')'
        WHEN NUMERIC_PRECISION IS NOT NULL 
        THEN '(' + CAST(NUMERIC_PRECISION AS VARCHAR) + ',' + CAST(NUMERIC_SCALE AS VARCHAR) + ')'
        ELSE ''
    END AS Tipo,
    '| ' + IS_NULLABLE AS Nullable,
    '| ' + ISNULL(COLUMN_DEFAULT, 'NULL') AS DefaultValue
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Ventas'
ORDER BY ORDINAL_POSITION;
PRINT '';

-- 3. Verificar columnas de BaseEntity
PRINT '3. VERIFICACIÓN DE COLUMNAS BASEENTITY:';

DECLARE @TieneId BIT = 0;
DECLARE @TieneCreatedAt BIT = 0;
DECLARE @TieneUpdatedAt BIT = 0;
DECLARE @TieneIsDeleted BIT = 0;
DECLARE @TieneCreatedBy BIT = 0;
DECLARE @TieneUpdatedBy BIT = 0;
DECLARE @TieneDeletedAt BIT = 0;

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Ventas' AND COLUMN_NAME = 'Id')
    SET @TieneId = 1;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Ventas' AND COLUMN_NAME = 'CreatedAt')
    SET @TieneCreatedAt = 1;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Ventas' AND COLUMN_NAME = 'UpdatedAt')
    SET @TieneUpdatedAt = 1;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Ventas' AND COLUMN_NAME = 'IsDeleted')
    SET @TieneIsDeleted = 1;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Ventas' AND COLUMN_NAME = 'CreatedBy')
    SET @TieneCreatedBy = 1;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Ventas' AND COLUMN_NAME = 'UpdatedBy')
    SET @TieneUpdatedBy = 1;
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Ventas' AND COLUMN_NAME = 'DeletedAt')
    SET @TieneDeletedAt = 1;

PRINT '   Id:         ' + CASE WHEN @TieneId = 1 THEN '? EXISTE' ELSE '? NO EXISTE' END;
PRINT '   CreatedAt:  ' + CASE WHEN @TieneCreatedAt = 1 THEN '? EXISTE' ELSE '? NO EXISTE' END;
PRINT '   UpdatedAt:  ' + CASE WHEN @TieneUpdatedAt = 1 THEN '? EXISTE' ELSE '? NO EXISTE' END;
PRINT '   IsDeleted:  ' + CASE WHEN @TieneIsDeleted = 1 THEN '? EXISTE' ELSE '? NO EXISTE' END;
PRINT '   CreatedBy:  ' + CASE WHEN @TieneCreatedBy = 1 THEN '? EXISTE' ELSE '? NO EXISTE' END;
PRINT '   UpdatedBy:  ' + CASE WHEN @TieneUpdatedBy = 1 THEN '? EXISTE' ELSE '? NO EXISTE' END;
PRINT '   DeletedAt:  ' + CASE WHEN @TieneDeletedAt = 1 THEN '? EXISTE' ELSE '? NO EXISTE' END;
PRINT '';

-- 4. Verificar columnas de dirección
PRINT '4. VERIFICACIÓN DE COLUMNAS DE DIRECCIÓN:';

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Ventas' AND COLUMN_NAME = 'DireccionCalle')
    PRINT '   DireccionCalle:         ? EXISTE';
ELSE
    PRINT '   DireccionCalle:         ? NO EXISTE';

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Ventas' AND COLUMN_NAME = 'DireccionCiudad')
    PRINT '   DireccionCiudad:        ? EXISTE';
ELSE
    PRINT '   DireccionCiudad:        ? NO EXISTE';

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Ventas' AND COLUMN_NAME = 'DireccionDepartamento')
    PRINT '   DireccionDepartamento:  ? EXISTE';
ELSE
    PRINT '   DireccionDepartamento:  ? NO EXISTE';

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Ventas' AND COLUMN_NAME = 'DireccionPais')
    PRINT '   DireccionPais:          ? EXISTE';
ELSE
    PRINT '   DireccionPais:          ? NO EXISTE';

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Ventas' AND COLUMN_NAME = 'DireccionCodigoPostal')
    PRINT '   DireccionCodigoPostal:  ? EXISTE';
ELSE
    PRINT '   DireccionCodigoPostal:  ? NO EXISTE';

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Ventas' AND COLUMN_NAME = 'DireccionReferencia')
    PRINT '   DireccionReferencia:    ? EXISTE';
ELSE
    PRINT '   DireccionReferencia:    ? NO EXISTE';
PRINT '';

-- 5. Verificar constraints
PRINT '5. CONSTRAINTS EN VENTAS:';
SELECT 
    '   ' + CONSTRAINT_NAME + ' (' + CONSTRAINT_TYPE + ')' AS [Constraint]
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
WHERE TABLE_NAME = 'Ventas';
PRINT '';

-- 6. Verificar triggers
PRINT '6. TRIGGERS EN VENTAS:';
IF EXISTS (SELECT * FROM sys.triggers WHERE parent_id = OBJECT_ID(N'Ventas'))
BEGIN
    SELECT '   ' + name + ' (ENABLED: ' + CASE WHEN is_disabled = 0 THEN 'YES' ELSE 'NO' END + ')' AS [Trigger]
    FROM sys.triggers 
    WHERE parent_id = OBJECT_ID(N'Ventas');
END
ELSE
    PRINT '   (Sin triggers)';
PRINT '';

-- 7. Contar registros existentes
PRINT '7. REGISTROS EXISTENTES:';
DECLARE @CantidadVentas INT;
SELECT @CantidadVentas = COUNT(*) FROM Ventas;
PRINT '   Total de ventas: ' + CAST(@CantidadVentas AS VARCHAR);
PRINT '';

-- 8. Generar INSERT de ejemplo
PRINT '8. EJEMPLO DE INSERT CORRECTO:';
PRINT '   (Basado en columnas existentes)';
PRINT '';

DECLARE @ColumnList NVARCHAR(MAX) = '';
SELECT @ColumnList = @ColumnList + COLUMN_NAME + ', '
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Ventas'
    AND COLUMN_NAME NOT IN ('Id') -- Id se genera automáticamente
ORDER BY ORDINAL_POSITION;

-- Remover última coma
SET @ColumnList = LEFT(@ColumnList, LEN(@ColumnList) - 1);

PRINT '   INSERT INTO Ventas (';
PRINT '       ' + @ColumnList;
PRINT '   ) VALUES (';
PRINT '       -- valores según tipo de dato';
PRINT '   )';
PRINT '';

PRINT '========================================';
PRINT 'FIN DEL DIAGNÓSTICO';
PRINT '========================================';
