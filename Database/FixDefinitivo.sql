PRAGMA foreign_keys = OFF;

-- =====================================================
-- SCRIPT DE CORRECCIÓN DEFINITIVA - SciFiHub
-- Forzar IsDeleted = 0 en TODOS los registros
-- =====================================================

USE SciFiHubDB;
GO

PRINT '========================================';
PRINT 'CORRECCIÓN DEFINITIVA - SciFiHub';
PRINT '========================================';
PRINT '';

-- ====================================
-- PASO 1: BACKUP DE SEGURIDAD
-- ====================================
PRINT '1. Creando respaldo de seguridad...';

-- Crear tablas temporales de respaldo
SELECT * INTO #Backup_Libros FROM Libros;
SELECT * INTO #Backup_Categorias FROM Categorias;
SELECT * INTO #Backup_Usuarios FROM Usuarios;

PRINT '   ? Respaldo creado';
PRINT '';

-- ====================================
-- PASO 2: ACTUALIZAR IsDeleted = 0
-- ====================================
PRINT '2. Actualizando IsDeleted en todas las tablas...';
PRINT '';

-- LIBROS
PRINT '   2.1. Corrigiendo LIBROS...';
UPDATE Libros
SET IsDeleted = 0, 
    UpdatedAt = GETUTCDATE()
WHERE IsDeleted IS NULL OR IsDeleted = 1;

DECLARE @LibrosActualizados INT = @@ROWCOUNT;
PRINT '       ? Libros actualizados: ' + CAST(@LibrosActualizados AS VARCHAR(10));

-- CATEGORIAS
PRINT '   2.2. Corrigiendo CATEGORÍAS...';
UPDATE Categorias
SET IsDeleted = 0, 
    UpdatedAt = GETUTCDATE()
WHERE IsDeleted IS NULL OR IsDeleted = 1;

DECLARE @CategoriasActualizadas INT = @@ROWCOUNT;
PRINT '       ? Categorías actualizadas: ' + CAST(@CategoriasActualizadas AS VARCHAR(10));

-- USUARIOS
PRINT '   2.3. Corrigiendo USUARIOS...';
UPDATE Usuarios
SET IsDeleted = 0
WHERE IsDeleted IS NULL OR IsDeleted = 1;

DECLARE @UsuariosActualizados INT = @@ROWCOUNT;
PRINT '       ? Usuarios actualizados: ' + CAST(@UsuariosActualizados AS VARCHAR(10));

-- VENTAS
PRINT '   2.4. Corrigiendo VENTAS...';
UPDATE Ventas
SET IsDeleted = 0
WHERE IsDeleted IS NULL OR IsDeleted = 1;

PRINT '       ? Ventas actualizadas: ' + CAST(@@ROWCOUNT AS VARCHAR(10));

-- CARRITOS
PRINT '   2.5. Corrigiendo CARRITOS...';
IF COL_LENGTH('dbo.CarritoCompras', 'IsDeleted') IS NOT NULL
BEGIN
    UPDATE CarritoCompras
    SET IsDeleted = 0
    WHERE IsDeleted IS NULL OR IsDeleted = 1;
    PRINT '       ? Carritos actualizados: ' + CAST(@@ROWCOUNT AS VARCHAR(10));
END
ELSE
BEGIN
    PRINT '       ??  CarritoCompras no tiene columna IsDeleted (skip)';
END

PRINT '';

-- ====================================
-- PASO 3: VERIFICACIÓN DETALLADA
-- ====================================
PRINT '========================================';
PRINT '3. VERIFICACIÓN POST-CORRECCIÓN';
PRINT '========================================';
PRINT '';

-- Verificar LIBROS
PRINT 'LIBROS:';
SELECT 
    COUNT(*) AS Total,
    SUM(CASE WHEN IsDeleted = 0 THEN 1 ELSE 0 END) AS Activos,
    SUM(CASE WHEN IsDeleted = 1 THEN 1 ELSE 0 END) AS Eliminados,
    SUM(CASE WHEN IsDeleted IS NULL THEN 1 ELSE 0 END) AS Nulos,
    SUM(CASE WHEN Stock > 0 THEN 1 ELSE 0 END) AS ConStock,
    SUM(CASE WHEN Estado = 'Disponible' THEN 1 ELSE 0 END) AS Disponibles
FROM Libros;

PRINT '';
PRINT 'CATEGORÍAS:';
SELECT 
    COUNT(*) AS Total,
    SUM(CASE WHEN IsDeleted = 0 THEN 1 ELSE 0 END) AS Activas,
    SUM(CASE WHEN IsDeleted = 1 THEN 1 ELSE 0 END) AS Eliminadas,
    SUM(CASE WHEN Estado = 'Activo' THEN 1 ELSE 0 END) AS EstadoActivo
FROM Categorias;

PRINT '';
PRINT 'USUARIOS:';
SELECT 
    COUNT(*) AS Total,
    SUM(CASE WHEN IsDeleted = 0 THEN 1 ELSE 0 END) AS Activos,
    SUM(CASE WHEN Estado = 'Activo' THEN 1 ELSE 0 END) AS EstadoActivo,
    SUM(CASE WHEN Rol = 'Administrador' THEN 1 ELSE 0 END) AS Admins,
    SUM(CASE WHEN Rol = 'Cliente' THEN 1 ELSE 0 END) AS Clientes
FROM Usuarios;

-- ====================================
-- PASO 4: LISTAR DATOS PARA CONFIRMAR
-- ====================================
PRINT '';
PRINT '========================================';
PRINT '4. LISTADO DE REGISTROS ACTIVOS';
PRINT '========================================';
PRINT '';

PRINT 'TOP 10 LIBROS:';
SELECT TOP 10
    Id,
    Titulo,
    Autor,
    Stock,
    Precio,
    Estado,
    IsDeleted,
    CONVERT(VARCHAR, CreatedAt, 120) AS FechaCreacion
FROM Libros
WHERE IsDeleted = 0
ORDER BY CreatedAt DESC;

PRINT '';
PRINT 'CATEGORÍAS:';
SELECT 
    Id,
    Nombre,
    Estado,
    IsDeleted,
    (SELECT COUNT(*) FROM Libros WHERE CategoriaId = c.Id AND IsDeleted = 0) AS LibrosEnCategoria
FROM Categorias c
WHERE IsDeleted = 0
ORDER BY Nombre;

PRINT '';
PRINT 'USUARIOS (sin passwords):';
SELECT 
    Id,
    Username,
    Email,
    Rol,
    Estado,
    IsDeleted
FROM Usuarios
WHERE IsDeleted = 0
ORDER BY Rol, Username;

-- ====================================
-- PASO 5: VALIDACIONES CRÍTICAS
-- ====================================
PRINT '';
PRINT '========================================';
PRINT '5. VALIDACIONES CRÍTICAS';
PRINT '========================================';

-- Validar que NO haya IsDeleted = NULL
IF EXISTS (SELECT 1 FROM Libros WHERE IsDeleted IS NULL)
BEGIN
    PRINT '? ERROR: Hay libros con IsDeleted = NULL';
END
ELSE
BEGIN
    PRINT '? Todos los libros tienen IsDeleted definido';
END

-- Validar que NO haya IsDeleted = 1
IF EXISTS (SELECT 1 FROM Libros WHERE IsDeleted = 1)
BEGIN
    PRINT '? ERROR: Hay libros con IsDeleted = 1';
END
ELSE
BEGIN
    PRINT '? Ningún libro está marcado como eliminado';
END

-- Validar que haya libros disponibles
IF EXISTS (SELECT 1 FROM Libros WHERE IsDeleted = 0 AND Stock > 0 AND Estado = 'Disponible')
BEGIN
    DECLARE @LibrosDisponibles INT;
    SELECT @LibrosDisponibles = COUNT(*) 
    FROM Libros 
    WHERE IsDeleted = 0 AND Stock > 0 AND Estado = 'Disponible';
    
    PRINT '? Hay ' + CAST(@LibrosDisponibles AS VARCHAR(10)) + ' libros disponibles para venta';
END
ELSE
BEGIN
    PRINT '? ERROR: No hay libros disponibles para venta';
END

-- Validar categorías
IF EXISTS (SELECT 1 FROM Categorias WHERE IsDeleted = 0 AND Estado = 'Activo')
BEGIN
    DECLARE @CategoriasActivas INT;
    SELECT @CategoriasActivas = COUNT(*) 
    FROM Categorias 
    WHERE IsDeleted = 0 AND Estado = 'Activo';
    
    PRINT '? Hay ' + CAST(@CategoriasActivas AS VARCHAR(10)) + ' categorías activas';
END
ELSE
BEGIN
    PRINT '? ERROR: No hay categorías activas';
END

-- Validar usuarios admin
IF EXISTS (SELECT 1 FROM Usuarios WHERE IsDeleted = 0 AND Rol = 'Administrador' AND Estado = 'Activo')
BEGIN
    PRINT '? Hay administradores activos';
END
ELSE
BEGIN
    PRINT '? ERROR: No hay administradores activos';
END

-- ====================================
-- PASO 6: INSTRUCCIONES FINALES
-- ====================================
PRINT '';
PRINT '========================================';
PRINT '? CORRECCIÓN COMPLETADA EXITOSAMENTE';
PRINT '========================================';
PRINT '';
PRINT 'PRÓXIMOS PASOS:';
PRINT '1. DETENER la aplicación si está corriendo';
PRINT '2. Ejecutar: dotnet run --project SciFiHub.Web.csproj';
PRINT '3. Ejecutar: POST http://localhost:5189/api/Test/fix-user-passwords';
PRINT '4. Acceder a: http://localhost:5189/Catalogo';
PRINT '5. Probar login con: admin / Admin123!';
PRINT '';
PRINT 'Si el catálogo SIGUE VACÍO:';
PRINT '- Verificar logs de la aplicación en la consola';
PRINT '- Ejecutar: GET http://localhost:5189/api/Test/database-status';
PRINT '- Compartir el resultado con el desarrollador';
PRINT '========================================';

-- Limpiar tablas temporales
DROP TABLE #Backup_Libros;
DROP TABLE #Backup_Categorias;
DROP TABLE #Backup_Usuarios;

GO
