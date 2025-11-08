-- =====================================================
-- SCRIPT DE DIAGNÓSTICO COMPLETO - SciFiHub
-- =====================================================

USE SciFiHubDB;
GO

PRINT '========================================';
PRINT 'DIAGNÓSTICO DE BASE DE DATOS - SciFiHub';
PRINT '========================================';
PRINT '';

-- 1. VERIFICAR LIBROS
PRINT '1. LIBROS:';
PRINT '--------------------';
SELECT 
    COUNT(*) AS TotalLibros,
    SUM(CASE WHEN IsDeleted = 0 THEN 1 ELSE 0 END) AS LibrosActivos,
    SUM(CASE WHEN IsDeleted = 1 THEN 1 ELSE 0 END) AS LibrosEliminados,
    SUM(CASE WHEN Stock > 0 THEN 1 ELSE 0 END) AS ConStock,
    SUM(CASE WHEN Destacado = 1 THEN 1 ELSE 0 END) AS Destacados,
    SUM(CASE WHEN PrecioOferta IS NOT NULL THEN 1 ELSE 0 END) AS ConOferta
FROM Libros;

PRINT '';
PRINT 'Detalles de libros:';
SELECT 
    Id,
    Titulo,
    Autor,
    Stock,
    Precio,
    PrecioOferta,
    Destacado,
    Estado,
    IsDeleted,
    CONVERT(VARCHAR, CreatedAt, 120) AS FechaCreacion
FROM Libros
ORDER BY CreatedAt DESC;

-- 2. VERIFICAR CATEGORÍAS
PRINT '';
PRINT '2. CATEGORÍAS:';
PRINT '--------------------';
SELECT 
    COUNT(*) AS TotalCategorias,
    SUM(CASE WHEN IsDeleted = 0 THEN 1 ELSE 0 END) AS CategoriasActivas,
    SUM(CASE WHEN IsDeleted = 1 THEN 1 ELSE 0 END) AS CategoriasEliminadas
FROM Categorias;

PRINT '';
PRINT 'Detalles de categorías:';
SELECT 
    Id,
    Nombre,
    Estado,
    IsDeleted,
    (SELECT COUNT(*) FROM Libros WHERE CategoriaId = c.Id AND IsDeleted = 0) AS CantidadLibros
FROM Categorias c
ORDER BY Nombre;

-- 3. VERIFICAR USUARIOS
PRINT '';
PRINT '3. USUARIOS:';
PRINT '--------------------';
SELECT 
    COUNT(*) AS TotalUsuarios,
    SUM(CASE WHEN Rol = 'Administrador' THEN 1 ELSE 0 END) AS Admins,
    SUM(CASE WHEN Rol = 'Vendedor' THEN 1 ELSE 0 END) AS Vendedores,
    SUM(CASE WHEN Rol = 'Cliente' THEN 1 ELSE 0 END) AS Clientes,
    SUM(CASE WHEN Estado = 'Activo' THEN 1 ELSE 0 END) AS UsuariosActivos
FROM Usuarios;

PRINT '';
PRINT 'Detalles de usuarios:';
SELECT 
    Id,
    Username,
    Email,
    Rol,
    Estado,
    IsDeleted,
    LEFT(PasswordHash, 30) + '...' AS PasswordHashPreview
FROM Usuarios
ORDER BY Rol, Username;

-- 4. VERIFICAR VENTAS Y CARRITOS
PRINT '';
PRINT '4. VENTAS Y CARRITOS:';
PRINT '--------------------';
SELECT 
    (SELECT COUNT(*) FROM Ventas WHERE IsDeleted = 0) AS TotalVentas,
    (SELECT COUNT(*) FROM CarritoCompras WHERE IsDeleted = 0) AS CarritosActivos,
    (SELECT SUM(Total) FROM Ventas WHERE IsDeleted = 0) AS TotalVendido;

-- 5. VERIFICAR ÍNDICES Y CONSTRAINTS
PRINT '';
PRINT '5. ÍNDICES Y CONSTRAINTS:';
PRINT '--------------------';
SELECT 
    OBJECT_NAME(object_id) AS Tabla,
    name AS Indice,
    type_desc AS TipoIndice
FROM sys.indexes
WHERE OBJECT_NAME(object_id) IN ('Libros', 'Categorias', 'Usuarios', 'Ventas')
ORDER BY Tabla, name;

-- 6. RECOMENDACIONES
PRINT '';
PRINT '========================================';
PRINT 'RECOMENDACIONES:';
PRINT '========================================';

IF EXISTS (SELECT 1 FROM Libros WHERE IsDeleted = 1)
BEGIN
    PRINT '??  HAY LIBROS MARCADOS COMO ELIMINADOS';
    PRINT '   Ejecutar: UPDATE Libros SET IsDeleted = 0;';
END

IF EXISTS (SELECT 1 FROM Categorias WHERE IsDeleted = 1)
BEGIN
    PRINT '??  HAY CATEGORÍAS MARCADAS COMO ELIMINADAS';
    PRINT '   Ejecutar: UPDATE Categorias SET IsDeleted = 0;';
END

IF NOT EXISTS (SELECT 1 FROM Libros WHERE Stock > 0 AND IsDeleted = 0)
BEGIN
    PRINT '??  NO HAY LIBROS CON STOCK DISPONIBLE';
    PRINT '   Verificar inventario';
END

IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE Rol = 'Administrador' AND Estado = 'Activo')
BEGIN
    PRINT '??  NO HAY ADMINISTRADORES ACTIVOS';
    PRINT '   Crear un usuario administrador';
END

PRINT '';
PRINT '? Diagnóstico completado';
PRINT '========================================';
GO
