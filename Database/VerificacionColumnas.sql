-- =============================================
-- SCRIPT: Verificación Completa de Columnas
-- Descripción: Verifica que todas las columnas existen
-- =============================================

USE SciFiHubDB;
GO

PRINT '==============================================';
PRINT 'VERIFICACIÓN DE COLUMNAS DE BASEENTITY';
PRINT '==============================================';
PRINT '';

-- =============================================
-- 1. VERIFICAR VENTAS
-- =============================================
PRINT '1. TABLA VENTAS:';
PRINT '----------------------------------------------';

SELECT 
    COLUMN_NAME AS Columna,
    DATA_TYPE AS TipoDato,
    IS_NULLABLE AS Nullable,
    COLUMN_DEFAULT AS ValorPorDefecto
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Ventas'
AND COLUMN_NAME IN ('Id', 'CreatedAt', 'UpdatedAt', 'CreatedBy', 'UpdatedBy', 'IsDeleted', 'DeletedAt')
ORDER BY 
    CASE COLUMN_NAME
        WHEN 'Id' THEN 1
        WHEN 'CreatedAt' THEN 2
        WHEN 'UpdatedAt' THEN 3
        WHEN 'CreatedBy' THEN 4
        WHEN 'UpdatedBy' THEN 5
        WHEN 'IsDeleted' THEN 6
        WHEN 'DeletedAt' THEN 7
    END;

PRINT '';

-- Contar columnas
DECLARE @VentasCount INT;
SELECT @VentasCount = COUNT(*)
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Ventas'
AND COLUMN_NAME IN ('Id', 'CreatedAt', 'UpdatedAt', 'CreatedBy', 'UpdatedBy', 'IsDeleted', 'DeletedAt');

IF @VentasCount = 7
    PRINT '? VENTAS: Todas las columnas existen (7/7)';
ELSE
    PRINT '? VENTAS: Faltan columnas (' + CAST(@VentasCount AS VARCHAR) + '/7)';

PRINT '';

-- =============================================
-- 2. VERIFICAR DETALLESVENTA
-- =============================================
PRINT '2. TABLA DETALLESVENTA:';
PRINT '----------------------------------------------';

SELECT 
    COLUMN_NAME AS Columna,
    DATA_TYPE AS TipoDato,
    IS_NULLABLE AS Nullable,
    COLUMN_DEFAULT AS ValorPorDefecto
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'DetallesVenta'
AND COLUMN_NAME IN ('Id', 'CreatedAt', 'UpdatedAt', 'CreatedBy', 'UpdatedBy', 'IsDeleted', 'DeletedAt')
ORDER BY 
    CASE COLUMN_NAME
        WHEN 'Id' THEN 1
        WHEN 'CreatedAt' THEN 2
        WHEN 'UpdatedAt' THEN 3
        WHEN 'CreatedBy' THEN 4
        WHEN 'UpdatedBy' THEN 5
        WHEN 'IsDeleted' THEN 6
        WHEN 'DeletedAt' THEN 7
    END;

PRINT '';

DECLARE @DetallesCount INT;
SELECT @DetallesCount = COUNT(*)
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'DetallesVenta'
AND COLUMN_NAME IN ('Id', 'CreatedAt', 'UpdatedAt', 'CreatedBy', 'UpdatedBy', 'IsDeleted', 'DeletedAt');

IF @DetallesCount = 7
    PRINT '? DETALLESVENTA: Todas las columnas existen (7/7)';
ELSE
    PRINT '? DETALLESVENTA: Faltan columnas (' + CAST(@DetallesCount AS VARCHAR) + '/7)';

PRINT '';

-- =============================================
-- 3. TEST DE INSERCIÓN
-- =============================================
PRINT '3. TEST DE INSERCIÓN:';
PRINT '----------------------------------------------';

BEGIN TRY
    -- Crear una venta de prueba
    DECLARE @TestVentaId UNIQUEIDENTIFIER = NEWID();
    DECLARE @TestClienteId UNIQUEIDENTIFIER;
    
    -- Obtener primer cliente activo
    SELECT TOP 1 @TestClienteId = Id 
    FROM Usuarios 
    WHERE Rol = 'Cliente' AND IsDeleted = 0;
    
    IF @TestClienteId IS NOT NULL
    BEGIN
        -- Intentar insertar venta de prueba
        INSERT INTO Ventas (
            Id, 
            NumeroVenta, 
            ClienteId, 
            FechaVenta, 
            Subtotal, 
            Descuento, 
            IGV, 
            Total, 
            EstadoVenta, 
            MetodoPago,
            CreatedAt,
            IsDeleted
        )
        VALUES (
            @TestVentaId,
            'TEST-' + CONVERT(VARCHAR(50), @TestVentaId),
            @TestClienteId,
            GETUTCDATE(),
            100.00,
            0.00,
            18.00,
            118.00,
            'Pendiente',
            'Efectivo',
            GETUTCDATE(),
            0
        );
        
        PRINT '? Test de inserción: EXITOSO';
        
        -- Eliminar venta de prueba
        DELETE FROM Ventas WHERE Id = @TestVentaId;
        PRINT '? Venta de prueba eliminada';
    END
    ELSE
    BEGIN
        PRINT '?? No se encontró cliente para test';
    END
END TRY
BEGIN CATCH
    PRINT '? Test de inserción: FALLÓ';
    PRINT 'Error: ' + ERROR_MESSAGE();
END CATCH

PRINT '';

-- =============================================
-- 4. RESUMEN FINAL
-- =============================================
PRINT '==============================================';
PRINT 'RESUMEN:';
PRINT '==============================================';

IF @VentasCount = 7 AND @DetallesCount = 7
BEGIN
    PRINT '? TODAS LAS COLUMNAS EXISTEN CORRECTAMENTE';
    PRINT '';
    PRINT 'SIGUIENTE PASO:';
    PRINT '1. Cerrar completamente Visual Studio';
    PRINT '2. Eliminar carpetas bin/ y obj/';
    PRINT '3. dotnet clean';
    PRINT '4. dotnet build';
    PRINT '5. dotnet run';
END
ELSE
BEGIN
    PRINT '? FALTAN COLUMNAS EN LA BASE DE DATOS';
    PRINT '';
    PRINT 'ACCIÓN REQUERIDA:';
    PRINT '1. Re-ejecutar: Database/FixColumnasVentas.sql';
    PRINT '2. Verificar conexión a base de datos correcta';
END

PRINT '';
PRINT '==============================================';
GO
