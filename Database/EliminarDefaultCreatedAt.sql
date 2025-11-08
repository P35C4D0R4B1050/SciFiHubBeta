-- =============================================
-- SCRIPT: Eliminar DEFAULT de CreatedAt
-- Descripción: Quita los constraints DEFAULT para que EF Core maneje CreatedAt
-- =============================================

USE SciFiHubDB;
GO

PRINT '==============================================';
PRINT 'ELIMINANDO CONSTRAINTS DEFAULT DE CREATEDDAT';
PRINT '==============================================';
PRINT '';

-- =============================================
-- 1. ENCONTRAR Y ELIMINAR CONSTRAINT DE VENTAS
-- =============================================
PRINT '1. Procesando tabla VENTAS...';

DECLARE @ConstraintNameVentas NVARCHAR(200);

-- Buscar el nombre del constraint
SELECT @ConstraintNameVentas = dc.name
FROM sys.default_constraints dc
INNER JOIN sys.columns c ON dc.parent_column_id = c.column_id AND dc.parent_object_id = c.object_id
WHERE OBJECT_NAME(dc.parent_object_id) = 'Ventas'
AND c.name = 'CreatedAt';

IF @ConstraintNameVentas IS NOT NULL
BEGIN
    DECLARE @SqlVentas NVARCHAR(500) = 'ALTER TABLE Ventas DROP CONSTRAINT ' + @ConstraintNameVentas;
    EXEC sp_executesql @SqlVentas;
    PRINT '? Constraint eliminado de Ventas: ' + @ConstraintNameVentas;
END
ELSE
BEGIN
    PRINT '?? No se encontró constraint DEFAULT en Ventas.CreatedAt';
END

PRINT '';

-- =============================================
-- 2. ENCONTRAR Y ELIMINAR CONSTRAINT DE DETALLESVENTA
-- =============================================
PRINT '2. Procesando tabla DETALLESVENTA...';

DECLARE @ConstraintNameDetalles NVARCHAR(200);

-- Buscar el nombre del constraint
SELECT @ConstraintNameDetalles = dc.name
FROM sys.default_constraints dc
INNER JOIN sys.columns c ON dc.parent_column_id = c.column_id AND dc.parent_object_id = c.object_id
WHERE OBJECT_NAME(dc.parent_object_id) = 'DetallesVenta'
AND c.name = 'CreatedAt';

IF @ConstraintNameDetalles IS NOT NULL
BEGIN
    DECLARE @SqlDetalles NVARCHAR(500) = 'ALTER TABLE DetallesVenta DROP CONSTRAINT ' + @ConstraintNameDetalles;
    EXEC sp_executesql @SqlDetalles;
    PRINT '? Constraint eliminado de DetallesVenta: ' + @ConstraintNameDetalles;
END
ELSE
BEGIN
    PRINT '?? No se encontró constraint DEFAULT en DetallesVenta.CreatedAt';
END

PRINT '';

-- =============================================
-- 3. VERIFICAR QUE SE ELIMINARON
-- =============================================
PRINT '3. Verificando que se eliminaron los constraints...';
PRINT '';

SELECT 
    OBJECT_NAME(dc.parent_object_id) AS Tabla,
    c.name AS Columna,
    dc.name AS ConstraintDefault,
    dc.definition AS ValorDefault
FROM sys.default_constraints dc
INNER JOIN sys.columns c ON dc.parent_column_id = c.column_id AND dc.parent_object_id = c.object_id
WHERE OBJECT_NAME(dc.parent_object_id) IN ('Ventas', 'DetallesVenta')
AND c.name = 'CreatedAt';

-- Si no retorna filas, significa que se eliminaron correctamente

PRINT '';
PRINT '==============================================';
PRINT 'VERIFICACIÓN FINAL';
PRINT '==============================================';

DECLARE @RemainingConstraints INT;
SELECT @RemainingConstraints = COUNT(*)
FROM sys.default_constraints dc
INNER JOIN sys.columns c ON dc.parent_column_id = c.column_id AND dc.parent_object_id = c.object_id
WHERE OBJECT_NAME(dc.parent_object_id) IN ('Ventas', 'DetallesVenta')
AND c.name = 'CreatedAt';

IF @RemainingConstraints = 0
BEGIN
    PRINT '? ÉXITO: Todos los constraints DEFAULT eliminados';
    PRINT '';
    PRINT 'SIGUIENTE PASO:';
    PRINT '1. Reiniciar aplicación: dotnet clean && dotnet build && dotnet run';
    PRINT '2. Probar registro de venta';
END
ELSE
BEGIN
    PRINT '? ERROR: Aún quedan ' + CAST(@RemainingConstraints AS VARCHAR) + ' constraint(s)';
    PRINT 'Por favor, ejecute el script nuevamente';
END

PRINT '';
PRINT '==============================================';
GO
