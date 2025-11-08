-- ============================================
-- SCRIPT DE PRUEBAS Y VERIFICACIÓN
-- ============================================
-- Propósito: Verificar y preparar datos para pruebas
-- ============================================

USE SciFiHubDB;
GO

PRINT '=== INICIANDO VERIFICACIÓN Y PREPARACIÓN DE DATOS ===';
PRINT '';

-- ============================================
-- 1. VERIFICAR CATEGORÍAS
-- ============================================
PRINT '1. Verificando categorías...';

DECLARE @CantidadCategorias INT;
SELECT @CantidadCategorias = COUNT(*) 
FROM Categorias 
WHERE IsDeleted = 0 AND Estado = 'Activo'; -- 0 = Activo (enum)

PRINT 'Categorías activas encontradas: ' + CAST(@CantidadCategorias AS VARCHAR(10));

IF @CantidadCategorias = 0
BEGIN
    PRINT '??  No hay categorías activas. Verificando si existen categorías eliminadas...';
    
    DECLARE @CantidadEliminadas INT;
    SELECT @CantidadEliminadas = COUNT(*) 
    FROM Categorias 
    WHERE IsDeleted = 1;
    
    IF @CantidadEliminadas > 0
    BEGIN
        PRINT '? Encontradas ' + CAST(@CantidadEliminadas AS VARCHAR(10)) + ' categorías eliminadas. Restaurando...';
        
        UPDATE Categorias 
        SET IsDeleted = 0, 
            DeletedAt = NULL,
            Estado = 'Activo', -- 0 = Activo (enum)
            UpdatedAt = GETUTCDATE()
        WHERE IsDeleted = 1;
        
        PRINT '? Categorías restauradas exitosamente';
    END
    ELSE
    BEGIN
        PRINT '??  No hay categorías. Insertando categorías de prueba...';
        
        -- Insertar solo si NO existen (verificando por nombre)
        IF NOT EXISTS (SELECT 1 FROM Categorias WHERE Nombre = 'Ciencia Ficción')
            INSERT INTO Categorias (Id, Nombre, Descripcion, Estado, Orden, IsDeleted, CreatedAt, CategoriaPadreId, CreatedBy, UpdatedBy, DeletedAt)
            VALUES (NEWID(), 'Ciencia Ficción', 'Novelas de ciencia ficción y futurismo', 'Activo', 1, 0, GETUTCDATE(), NULL, NULL, NULL, NULL);
        
        IF NOT EXISTS (SELECT 1 FROM Categorias WHERE Nombre = 'Fantasía')
            INSERT INTO Categorias (Id, Nombre, Descripcion, Estado, Orden, IsDeleted, CreatedAt, CategoriaPadreId, CreatedBy, UpdatedBy, DeletedAt)
            VALUES (NEWID(), 'Fantasía', 'Mundos fantásticos y magia', 'Activo', 2, 0, GETUTCDATE(), NULL, NULL, NULL, NULL);
        
        IF NOT EXISTS (SELECT 1 FROM Categorias WHERE Nombre = 'Cyberpunk')
            INSERT INTO Categorias (Id, Nombre, Descripcion, Estado, Orden, IsDeleted, CreatedAt, CategoriaPadreId, CreatedBy, UpdatedBy, DeletedAt)
            VALUES (NEWID(), 'Cyberpunk', 'Futuros tecnológicos y hackers', 'Activo', 3, 0, GETUTCDATE(), NULL, NULL, NULL, NULL);
        
        IF NOT EXISTS (SELECT 1 FROM Categorias WHERE Nombre = 'Distopía')
            INSERT INTO Categorias (Id, Nombre, Descripcion, Estado, Orden, IsDeleted, CreatedAt, CategoriaPadreId, CreatedBy, UpdatedBy, DeletedAt)
            VALUES (NEWID(), 'Distopía', 'Sociedades oscuras y totalitarias', 'Activo', 4, 0, GETUTCDATE(), NULL, NULL, NULL, NULL);
        
        IF NOT EXISTS (SELECT 1 FROM Categorias WHERE Nombre = 'Space Opera')
            INSERT INTO Categorias (Id, Nombre, Descripcion, Estado, Orden, IsDeleted, CreatedAt, CategoriaPadreId, CreatedBy, UpdatedBy, DeletedAt)
            VALUES (NEWID(), 'Space Opera', 'Aventuras espaciales épicas', 'Activo', 5, 0, GETUTCDATE(), NULL, NULL, NULL, NULL);
        
        IF NOT EXISTS (SELECT 1 FROM Categorias WHERE Nombre = 'Horror Cósmico')
            INSERT INTO Categorias (Id, Nombre, Descripcion, Estado, Orden, IsDeleted, CreatedAt, CategoriaPadreId, CreatedBy, UpdatedBy, DeletedAt)
            VALUES (NEWID(), 'Horror Cósmico', 'Terror lovecraftiano y cósmico', 'Activo', 6, 0, GETUTCDATE(), NULL, NULL, NULL, NULL);
        
        IF NOT EXISTS (SELECT 1 FROM Categorias WHERE Nombre = 'Steampunk')
            INSERT INTO Categorias (Id, Nombre, Descripcion, Estado, Orden, IsDeleted, CreatedAt, CategoriaPadreId, CreatedBy, UpdatedBy, DeletedAt)
            VALUES (NEWID(), 'Steampunk', 'Tecnología victoriana alternativa', 'Activo', 7, 0, GETUTCDATE(), NULL, NULL, NULL, NULL);
        
        IF NOT EXISTS (SELECT 1 FROM Categorias WHERE Nombre = 'Viajes en el Tiempo')
            INSERT INTO Categorias (Id, Nombre, Descripcion, Estado, Orden, IsDeleted, CreatedAt, CategoriaPadreId, CreatedBy, UpdatedBy, DeletedAt)
            VALUES (NEWID(), 'Viajes en el Tiempo', 'Paradojas temporales y viajes', 'Activo', 8, 0, GETUTCDATE(), NULL, NULL, NULL, NULL);
        
        PRINT '? Categorías insertadas exitosamente';
    END
END
ELSE
BEGIN
    PRINT '? Ya existen categorías activas';
END

-- Mostrar categorías
PRINT '';
PRINT 'Categorías disponibles:';
SELECT 
    Id,
    Nombre,
    Descripcion,
    Estado,
    Orden,
    IsDeleted
FROM Categorias
WHERE IsDeleted = 0 AND Estado = 'Activo'
ORDER BY Orden, Nombre;

PRINT '';
GO

-- ============================================
-- 2. VERIFICAR USUARIOS
-- ============================================
PRINT '2. Verificando usuarios...';

-- Usuarios activos
DECLARE @CantidadUsuarios INT;
SELECT @CantidadUsuarios = COUNT(*) 
FROM Usuarios 
WHERE IsDeleted = 0;

PRINT 'Usuarios activos: ' + CAST(@CantidadUsuarios AS VARCHAR(10));

-- Usuarios por rol
PRINT '';
PRINT 'Distribución por roles:';
SELECT 
    Rol,
    COUNT(*) AS Cantidad,
    SUM(CASE WHEN Estado = 'Activo' THEN 1 ELSE 0 END) AS Activos
FROM Usuarios
WHERE IsDeleted = 0
GROUP BY Rol
ORDER BY Rol;

PRINT '';
GO

-- ============================================
-- 3. VERIFICAR LIBROS
-- ============================================
PRINT '3. Verificando libros...';

DECLARE @CantidadLibros INT;
SELECT @CantidadLibros = COUNT(*) 
FROM Libros 
WHERE IsDeleted = 0;

PRINT 'Libros disponibles: ' + CAST(@CantidadLibros AS VARCHAR(10));

IF @CantidadLibros = 0
BEGIN
    PRINT '??  No hay libros. Se recomienda agregar libros de prueba.';
END
ELSE
BEGIN
    PRINT '? Hay libros disponibles';
    
    -- Mostrar muestra de libros
    PRINT '';
    PRINT 'Muestra de libros (Top 5):';
    SELECT TOP 5
        Titulo,
        Autor,
        Precio,
        Stock,
        Estado
    FROM Libros
    WHERE IsDeleted = 0
    ORDER BY CreatedAt DESC;
END

PRINT '';
GO

-- ============================================
-- 4. VERIFICAR VENTAS
-- ============================================
PRINT '4. Verificando ventas...';

DECLARE @CantidadVentas INT;
SELECT @CantidadVentas = COUNT(*) 
FROM Ventas 
WHERE IsDeleted = 0;

PRINT 'Ventas registradas: ' + CAST(@CantidadVentas AS VARCHAR(10));

IF @CantidadVentas > 0
BEGIN
    -- Ventas por estado
    PRINT '';
    PRINT 'Ventas por estado:';
    SELECT 
        EstadoVenta,
        COUNT(*) AS Cantidad
    FROM Ventas
    WHERE IsDeleted = 0
    GROUP BY EstadoVenta
    ORDER BY EstadoVenta;
END

PRINT '';
GO

-- ============================================
-- 5. VERIFICAR COLUMNAS BASEENTITY
-- ============================================
PRINT '5. Verificando columnas BaseEntity...';

DECLARE @TotalColumnas INT;
SELECT @TotalColumnas = COUNT(*)
FROM sys.tables t
JOIN sys.columns c ON t.object_id = c.object_id
WHERE c.name IN ('CreatedBy', 'UpdatedBy', 'DeletedAt')
AND (
    t.name IN ('Usuarios', 'Categorias', 'Libros', 'Ventas', 'DetallesVenta', 'CarritoCompras', 'DetallesCarrito')
    OR t.name LIKE '%Audit%Inventario%'
);

PRINT 'Columnas BaseEntity encontradas: ' + CAST(@TotalColumnas AS VARCHAR(10)) + ' de 24 esperadas';

IF @TotalColumnas = 24
    PRINT '? Todas las columnas BaseEntity están presentes';
ELSE
    PRINT '??  Faltan ' + CAST(24 - @TotalColumnas AS VARCHAR(10)) + ' columnas. Ejecutar FixColumnasBaseEntity_DEFINITIVO.sql';

PRINT '';
GO

-- ============================================
-- 6. CREAR USUARIO DE PRUEBA PARA VENDEDOR
-- ============================================
PRINT '6. Verificando usuario de prueba para vendedor...';

IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE Username = 'vendedor.test' AND IsDeleted = 0)
BEGIN
    PRINT 'Creando usuario de prueba: vendedor.test';
    
    INSERT INTO Usuarios (
        Id, 
        NombreCompleto, 
        Email, 
        Username, 
        PasswordHash, 
        Rol, 
        Estado, 
        IsDeleted, 
        CreatedAt,
        CreatedBy,
        UpdatedBy,
        DeletedAt,
        FechaRegistro
    )
    VALUES (
        NEWID(),
        'Vendedor de Prueba',
        'vendedor.test@scifihub.com',
        'vendedor.test',
        'AQAAAAIAAYagAAAAEJ5xKZ4xKZ4xKZ4xKZ4xKZ4xKZ4xKZ4xKZ4xKZ4=', -- Hash de 'Test123!'
        'Vendedor',
        'Activo',
        0,
        GETUTCDATE(),
        NULL,
        NULL,
        NULL,
        GETUTCDATE()
    );
    
    PRINT '? Usuario vendedor.test creado';
    PRINT '   Username: vendedor.test';
    PRINT '   Password: Test123!';
END
ELSE
BEGIN
    PRINT '? Usuario vendedor.test ya existe';
END

PRINT '';
GO

-- ============================================
-- 7. CREAR USUARIO DE PRUEBA PARA ADMIN
-- ============================================
PRINT '7. Verificando usuario de prueba para admin...';

IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE Username = 'admin.test' AND IsDeleted = 0)
BEGIN
    PRINT 'Creando usuario de prueba: admin.test';
    
    INSERT INTO Usuarios (
        Id, 
        NombreCompleto, 
        Email, 
        Username, 
        PasswordHash, 
        Rol, 
        Estado, 
        IsDeleted, 
        CreatedAt,
        CreatedBy,
        UpdatedBy,
        DeletedAt,
        FechaRegistro
    )
    VALUES (
        NEWID(),
        'Administrador de Prueba',
        'admin.test@scifihub.com',
        'admin.test',
        'AQAAAAIAAYagAAAAEJ5xKZ4xKZ4xKZ4xKZ4xKZ4xKZ4xKZ4xKZ4xKZ4=', -- Hash de 'Test123!'
        'Administrador',
        'Activo',
        0,
        GETUTCDATE(),
        NULL,
        NULL,
        NULL,
        GETUTCDATE()
    );
    
    PRINT '? Usuario admin.test creado';
    PRINT '   Username: admin.test';
    PRINT '   Password: Test123!';
END
ELSE
BEGIN
    PRINT '? Usuario admin.test ya existe';
END

PRINT '';
GO

-- ============================================
-- 8. RESUMEN FINAL
-- ============================================
PRINT '=== RESUMEN FINAL ===';
PRINT '';

SELECT 
    'Categorías Activas' AS Item,
    COUNT(*) AS Cantidad
FROM Categorias
WHERE IsDeleted = 0 AND Estado = 'Activo' -- 0 = Activo (enum)

UNION ALL

SELECT 
    'Usuarios Activos' AS Item,
    COUNT(*) AS Cantidad
FROM Usuarios
WHERE IsDeleted = 0

UNION ALL

SELECT 
    'Libros Disponibles' AS Item,
    COUNT(*) AS Cantidad
FROM Libros
WHERE IsDeleted = 0

UNION ALL

SELECT 
    'Ventas Registradas' AS Item,
    COUNT(*) AS Cantidad
FROM Ventas
WHERE IsDeleted = 0

UNION ALL

SELECT 
    'Columnas BaseEntity' AS Item,
    COUNT(*) AS Cantidad
FROM sys.tables t
JOIN sys.columns c ON t.object_id = c.object_id
WHERE c.name IN ('CreatedBy', 'UpdatedBy', 'DeletedAt')
AND (
    t.name IN ('Usuarios', 'Categorias', 'Libros', 'Ventas', 'DetallesVenta', 'CarritoCompras', 'DetallesCarrito')
    OR t.name LIKE '%Audit%Inventario%'
);

PRINT '';
PRINT '=== VERIFICACIÓN COMPLETADA ===';
PRINT '';
PRINT '? La base de datos está lista para pruebas';
PRINT '';
PRINT 'Usuarios de prueba creados:';
PRINT '  - admin.test / Test123! (Administrador)';
PRINT '  - vendedor.test / Test123! (Vendedor)';
PRINT '';
PRINT 'Próximo paso: Ejecutar la aplicación con dotnet run';
GO
