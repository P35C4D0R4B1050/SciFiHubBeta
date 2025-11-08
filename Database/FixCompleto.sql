-- =====================================================
-- SCRIPT DE CORRECCIÓN FINAL - SciFiHub
-- =====================================================

USE SciFiHubDB;
GO

PRINT '========================================';
PRINT 'CORRECCIÓN DE BASE DE DATOS - SciFiHub';
PRINT '========================================';
PRINT '';

-- 1. CORREGIR IsDeleted en Libros
PRINT '1. Corrigiendo IsDeleted en Libros...';
UPDATE Libros
SET IsDeleted = 0, UpdatedAt = GETUTCDATE()
WHERE IsDeleted = 1 OR IsDeleted IS NULL;

PRINT '   ? Libros actualizados: ' + CAST(@@ROWCOUNT AS VARCHAR(10));

-- 2. CORREGIR IsDeleted en Categorias
PRINT '2. Corrigiendo IsDeleted en Categorías...';
UPDATE Categorias
SET IsDeleted = 0, UpdatedAt = GETUTCDATE()
WHERE IsDeleted = 1 OR IsDeleted IS NULL;

PRINT '   ? Categorías actualizadas: ' + CAST(@@ROWCOUNT AS VARCHAR(10));

-- 3. CORREGIR IsDeleted en Usuarios
PRINT '3. Corrigiendo IsDeleted en Usuarios...';
UPDATE Usuarios
SET IsDeleted = 0
WHERE IsDeleted = 1 OR IsDeleted IS NULL;

PRINT '   ? Usuarios actualizados: ' + CAST(@@ROWCOUNT AS VARCHAR(10));

-- 4. ACTUALIZAR HASHES DE CONTRASEÑAS
PRINT '';
PRINT '4. Actualizando hashes de contraseñas...';
PRINT '   IMPORTANTE: Después ejecutar endpoint POST /api/Test/fix-user-passwords';

-- 5. VERIFICAR RESULTADOS
PRINT '';
PRINT '========================================';
PRINT 'VERIFICACIÓN POST-CORRECCIÓN:';
PRINT '========================================';

SELECT 
    'Libros' AS Tabla,
    COUNT(*) AS Total,
    SUM(CASE WHEN IsDeleted = 0 THEN 1 ELSE 0 END) AS Activos,
    SUM(CASE WHEN Stock > 0 THEN 1 ELSE 0 END) AS ConStock
FROM Libros
UNION ALL
SELECT 
    'Categorias' AS Tabla,
    COUNT(*) AS Total,
    SUM(CASE WHEN IsDeleted = 0 THEN 1 ELSE 0 END) AS Activos,
    NULL AS ConStock
FROM Categorias
UNION ALL
SELECT 
    'Usuarios' AS Tabla,
    COUNT(*) AS Total,
    SUM(CASE WHEN IsDeleted = 0 THEN 1 ELSE 0 END) AS Activos,
    NULL AS ConStock
FROM Usuarios;

-- 6. MOSTRAR LIBROS PARA CONFIRMAR
PRINT '';
PRINT 'LIBROS DISPONIBLES:';
SELECT 
    Id,
    Titulo,
    Autor,
    Stock,
    Precio,
    IsDeleted,
    Estado
FROM Libros
WHERE IsDeleted = 0
ORDER BY Titulo;

PRINT '';
PRINT '? Corrección completada';
PRINT '========================================';
PRINT '';
PRINT 'PRÓXIMOS PASOS:';
PRINT '1. Ejecutar la aplicación: dotnet run';
PRINT '2. Ejecutar: POST http://localhost:5189/api/Test/fix-user-passwords';
PRINT '3. Probar login con: admin / Admin123!';
PRINT '4. Acceder a: http://localhost:5189/Catalogo';
PRINT '========================================';

GO
