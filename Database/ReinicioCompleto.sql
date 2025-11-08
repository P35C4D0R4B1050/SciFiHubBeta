-- =====================================================
-- SCRIPT DE REINICIO COMPLETO - SciFiHub
-- Limpia datos antiguos y crea datos frescos
-- =====================================================

USE SciFiHubDB;
GO

PRINT '========================================';
PRINT 'REINICIO COMPLETO - SciFiHub';
PRINT '========================================';
PRINT '';

-- ====================================
-- PASO 1: BACKUP COMPLETO
-- ====================================
PRINT '1. Creando backup completo...';

SELECT * INTO #Backup_Full_Usuarios FROM Usuarios;
SELECT * INTO #Backup_Full_Libros FROM Libros;
SELECT * INTO #Backup_Full_Categorias FROM Categorias;

PRINT '   ? Backup creado en tablas temporales';
PRINT '';

-- ====================================
-- PASO 2: LIMPIAR DATOS ANTIGUOS
-- ====================================
PRINT '2. Limpiando datos antiguos...';
PRINT '';

-- Eliminar en orden (respetando FK)
PRINT '   2.1. Limpiando DetallesCarrito...';
DELETE FROM DetallesCarrito;
PRINT '       ? ' + CAST(@@ROWCOUNT AS VARCHAR(10)) + ' registros eliminados';

PRINT '   2.2. Limpiando CarritoCompras...';
DELETE FROM CarritoCompras;
PRINT '       ? ' + CAST(@@ROWCOUNT AS VARCHAR(10)) + ' registros eliminados';

PRINT '   2.3. Limpiando DetallesVenta...';
DELETE FROM DetallesVenta;
PRINT '       ? ' + CAST(@@ROWCOUNT AS VARCHAR(10)) + ' registros eliminados';

PRINT '   2.4. Limpiando Ventas...';
DELETE FROM Ventas;
PRINT '       ? ' + CAST(@@ROWCOUNT AS VARCHAR(10)) + ' registros eliminados';

PRINT '   2.5. Limpiando AuditoriasInventario...';
DELETE FROM AuditoriasInventario;
PRINT '       ? ' + CAST(@@ROWCOUNT AS VARCHAR(10)) + ' registros eliminados';

PRINT '   2.6. Limpiando Libros...';
DELETE FROM Libros;
PRINT '       ? ' + CAST(@@ROWCOUNT AS VARCHAR(10)) + ' registros eliminados';

PRINT '   2.7. Limpiando Categorias...';
DELETE FROM Categorias;
PRINT '       ? ' + CAST(@@ROWCOUNT AS VARCHAR(10)) + ' registros eliminados';

PRINT '   2.8. Limpiando Usuarios...';
DELETE FROM Usuarios;
PRINT '       ? ' + CAST(@@ROWCOUNT AS VARCHAR(10)) + ' registros eliminados';

PRINT '';
PRINT '? Todas las tablas limpiadas';
PRINT '';

-- ====================================
-- PASO 3: INSERTAR USUARIOS FRESCOS
-- ====================================
PRINT '3. Creando usuarios frescos...';
PRINT '';

-- IMPORTANTE: Estos hashes son PLACEHOLDERS
-- Debes ejecutar POST /api/Test/fix-user-passwords después de este script
-- para generar hashes BCrypt válidos

-- Usuario Administrador
INSERT INTO Usuarios (Id, Username, Email, PasswordHash, NombreCompleto, Telefono, Rol, Estado, FechaRegistro, IsDeleted, CreatedAt)
VALUES (
    NEWID(),
    'admin',
    'admin@scifihub.com',
    'PLACEHOLDER_HASH', -- Se actualizará con el endpoint
    'Administrador del Sistema',
    '+51 999 888 777',
    'Administrador',
    'Activo',
    GETUTCDATE(),
    0,
    GETUTCDATE()
);
PRINT '   ? Usuario admin creado';

-- Usuario Vendedor
INSERT INTO Usuarios (Id, Username, Email, PasswordHash, NombreCompleto, Telefono, Rol, Estado, FechaRegistro, IsDeleted, CreatedAt)
VALUES (
    NEWID(),
    'vendedor1',
    'vendedor1@scifihub.com',
    'PLACEHOLDER_HASH',
    'María Torres García',
    '+51 999 777 666',
    'Vendedor',
    'Activo',
    GETUTCDATE(),
    0,
    GETUTCDATE()
);
PRINT '   ? Usuario vendedor1 creado';

-- Usuarios Clientes
INSERT INTO Usuarios (Id, Username, Email, PasswordHash, NombreCompleto, Telefono, Rol, Estado, FechaRegistro, IsDeleted, CreatedAt)
VALUES 
(NEWID(), 'cliente1', 'cliente1@scifihub.com', 'PLACEHOLDER_HASH', 'Juan Pérez López', '+51 999 666 555', 'Cliente', 'Activo', GETUTCDATE(), 0, GETUTCDATE()),
(NEWID(), 'cliente2', 'cliente2@scifihub.com', 'PLACEHOLDER_HASH', 'Ana Martínez Ruiz', '+51 999 555 444', 'Cliente', 'Activo', GETUTCDATE(), 0, GETUTCDATE()),
(NEWID(), 'cliente3', 'cliente3@scifihub.com', 'PLACEHOLDER_HASH', 'Carlos García Soto', '+51 999 444 333', 'Cliente', 'Activo', GETUTCDATE(), 0, GETUTCDATE());

PRINT '   ? 3 clientes creados';
PRINT '';

-- ====================================
-- PASO 4: INSERTAR CATEGORÍAS
-- ====================================
PRINT '4. Creando categorías...';
PRINT '';

DECLARE @CatCienciaFiccion UNIQUEIDENTIFIER = NEWID();
DECLARE @CatFantasia UNIQUEIDENTIFIER = NEWID();
DECLARE @CatDistopia UNIQUEIDENTIFIER = NEWID();
DECLARE @CatCyberpunk UNIQUEIDENTIFIER = NEWID();
DECLARE @CatSpaceOpera UNIQUEIDENTIFIER = NEWID();

INSERT INTO Categorias (Id, Nombre, Descripcion, Estado, Orden, IsDeleted, CreatedAt)
VALUES 
(@CatCienciaFiccion, 'Ciencia Ficción', 'Novelas de ciencia ficción y futurismo', 'Activo', 1, 0, GETUTCDATE()),
(@CatFantasia, 'Fantasía', 'Mundos fantásticos y magia', 'Activo', 2, 0, GETUTCDATE()),
(@CatDistopia, 'Distopía', 'Futuros oscuros y sociedades totalitarias', 'Activo', 3, 0, GETUTCDATE()),
(@CatCyberpunk, 'Cyberpunk', 'Futuros tecnológicos y hackers', 'Activo', 4, 0, GETUTCDATE()),
(@CatSpaceOpera, 'Space Opera', 'Aventuras espaciales épicas', 'Activo', 5, 0, GETUTCDATE());

PRINT '   ? 5 categorías creadas';
PRINT '';

-- ====================================
-- PASO 5: INSERTAR LIBROS
-- ====================================
PRINT '5. Creando libros...';
PRINT '';

INSERT INTO Libros (Id, ISBN, Titulo, Autor, Editorial, CategoriaId, AnioPublicacion, NumeroPaginas, Idioma, Precio, PrecioOferta, Stock, ImagenPortada, Sinopsis, Descripcion, Destacado, Estado, FechaIngreso, IsDeleted, CreatedAt)
VALUES 
-- Ciencia Ficción
(NEWID(), '978-0-441-17271-9', 'Dune', 'Frank Herbert', 'Ace Books', @CatCienciaFiccion, 1965, 688, 'Español', 29.99, NULL, 50, 'https://m.media-amazon.com/images/I/81zN7udGRUL._SY466_.jpg', 'Una obra maestra de la ciencia ficción que narra la historia de Paul Atreides en el planeta desértico Arrakis.', 'Considerada una de las mejores novelas de ciencia ficción de todos los tiempos.', 1, 'Disponible', GETUTCDATE(), 0, GETUTCDATE()),

(NEWID(), '978-0-345-39180-3', 'Fundación', 'Isaac Asimov', 'Bantam Spectra', @CatCienciaFiccion, 1951, 255, 'Español', 24.99, 19.99, 35, 'https://m.media-amazon.com/images/I/91dxLb7SGUL._SY466_.jpg', 'La saga que narra el declive del Imperio Galáctico y el nacimiento de la Fundación.', 'Primera parte de la legendaria trilogía de la Fundación.', 1, 'Disponible', GETUTCDATE(), 0, GETUTCDATE()),

(NEWID(), '978-0-316-01984-0', 'El Juego de Ender', 'Orson Scott Card', 'Tor Books', @CatCienciaFiccion, 1985, 324, 'Español', 26.99, NULL, 40, 'https://m.media-amazon.com/images/I/71vWYx9kBTL._SY466_.jpg', 'Ender Wiggin, un niño genio entrenado para salvar a la humanidad de una invasión alienígena.', 'Ganadora de los premios Hugo y Nébula.', 1, 'Disponible', GETUTCDATE(), 0, GETUTCDATE()),

-- Distopía
(NEWID(), '978-0-553-38034-1', '1984', 'George Orwell', 'Signet Classic', @CatDistopia, 1949, 328, 'Español', 19.99, NULL, 60, 'https://m.media-amazon.com/images/I/71rpa1-kyvL._SY466_.jpg', 'Una visión distópica de un futuro totalitario donde el Gran Hermano todo lo ve.', 'Un clásico que ha definido el género distópico.', 1, 'Disponible', GETUTCDATE(), 0, GETUTCDATE()),

(NEWID(), '978-0-544-33641-9', 'Un mundo feliz', 'Aldous Huxley', 'Harper Perennial', @CatDistopia, 1932, 268, 'Español', 18.99, 14.99, 45, 'https://m.media-amazon.com/images/I/81p563T+AFL._SY466_.jpg', 'Una sociedad aparentemente perfecta donde la felicidad se impone mediante drogas y condicionamiento.', 'Distopía clásica que cuestiona el precio de la felicidad artificial.', 1, 'Disponible', GETUTCDATE(), 0, GETUTCDATE()),

-- Cyberpunk
(NEWID(), '978-0-441-56959-4', 'Neuromante', 'William Gibson', 'Ace Books', @CatCyberpunk, 1984, 271, 'Español', 22.99, NULL, 28, 'https://m.media-amazon.com/images/I/51aKCYZVmCL._SY466_.jpg', 'La novela que definió el género cyberpunk. Case, un hacker caído en desgracia, acepta un último trabajo imposible.', 'Ganadora del premio Hugo, Nébula y Philip K. Dick.', 0, 'Disponible', GETUTCDATE(), 0, GETUTCDATE()),

(NEWID(), '978-0-575-09406-6', '¿Sueñan los androides con ovejas eléctricas?', 'Philip K. Dick', 'Gollancz', @CatCyberpunk, 1968, 210, 'Español', 21.99, 17.99, 32, 'https://m.media-amazon.com/images/I/71XhYv+HBSL._SY466_.jpg', 'En un futuro postapocalíptico, un cazarrecompensas debe identificar y eliminar androides fugitivos.', 'Base de la película Blade Runner.', 1, 'Disponible', GETUTCDATE(), 0, GETUTCDATE()),

-- Fantasía
(NEWID(), '978-0-345-41943-6', 'El Nombre del Viento', 'Patrick Rothfuss', 'DAW Books', @CatFantasia, 2007, 662, 'Español', 34.99, 27.99, 45, 'https://m.media-amazon.com/images/I/91b8oNwaQsL._SY466_.jpg', 'La historia de Kvothe, un héroe legendario que narra su propia historia desde su humilde posada.', 'Primera parte de la saga Crónica del Asesino de Reyes.', 1, 'Disponible', GETUTCDATE(), 0, GETUTCDATE()),

(NEWID(), '978-0-765-31178-5', 'El Imperio Final', 'Brandon Sanderson', 'Tor Books', @CatFantasia, 2006, 541, 'Español', 32.99, NULL, 38, 'https://m.media-amazon.com/images/I/81OgC2YKJKL._SY466_.jpg', 'En un mundo donde la ceniza cae del cielo, un grupo de rebeldes planea derrocar al Lord Legislador.', 'Primera novela de la saga Nacidos de la Bruma.', 1, 'Disponible', GETUTCDATE(), 0, GETUTCDATE()),

-- Space Opera
(NEWID(), '978-0-316-33836-3', 'Leviathan Wakes', 'James S.A. Corey', 'Orbit', @CatSpaceOpera, 2011, 561, 'Español', 28.99, NULL, 30, 'https://m.media-amazon.com/images/I/91aJJKGJOYL._SY466_.jpg', 'Una detective busca a una chica desaparecida mientras el sistema solar se acerca a la guerra.', 'Primera novela de The Expanse.', 1, 'Disponible', GETUTCDATE(), 0, GETUTCDATE());

PRINT '   ? 10 libros creados';
PRINT '';

-- ====================================
-- PASO 6: VERIFICACIÓN
-- ====================================
PRINT '========================================';
PRINT '6. VERIFICACIÓN DE DATOS CREADOS';
PRINT '========================================';
PRINT '';

SELECT 
    'Usuarios' AS Tabla,
    COUNT(*) AS Total,
    SUM(CASE WHEN Rol = 'Administrador' THEN 1 ELSE 0 END) AS Admins,
    SUM(CASE WHEN Rol = 'Vendedor' THEN 1 ELSE 0 END) AS Vendedores,
    SUM(CASE WHEN Rol = 'Cliente' THEN 1 ELSE 0 END) AS Clientes
FROM Usuarios
UNION ALL
SELECT 
    'Categorías' AS Tabla,
    COUNT(*) AS Total,
    NULL, NULL
FROM Categorias
UNION ALL
SELECT 
    'Libros' AS Tabla,
    COUNT(*) AS Total,
    SUM(CASE WHEN IsDeleted = 0 THEN 1 ELSE 0 END) AS Activos,
    SUM(CASE WHEN Stock > 0 THEN 1 ELSE 0 END) AS ConStock
FROM Libros;

-- Mostrar usuarios creados
PRINT '';
PRINT 'USUARIOS CREADOS:';
SELECT Username, Email, Rol, Estado, IsDeleted
FROM Usuarios
ORDER BY Rol, Username;

-- Mostrar categorías
PRINT '';
PRINT 'CATEGORÍAS CREADAS:';
SELECT Id, Nombre, Estado, IsDeleted
FROM Categorias
ORDER BY Orden;

-- Mostrar libros
PRINT '';
PRINT 'LIBROS CREADOS:';
SELECT Titulo, Autor, Stock, Precio, PrecioOferta, Destacado, Estado, IsDeleted
FROM Libros
ORDER BY Titulo;

-- ====================================
-- PASO 7: INSTRUCCIONES FINALES
-- ====================================
PRINT '';
PRINT '========================================';
PRINT '? REINICIO COMPLETADO EXITOSAMENTE';
PRINT '========================================';
PRINT '';
PRINT '??  IMPORTANTE: Los usuarios tienen hashes PLACEHOLDER';
PRINT '';
PRINT 'PRÓXIMOS PASOS OBLIGATORIOS:';
PRINT '1. Ejecutar la aplicación: dotnet run --project SciFiHub.Web.csproj';
PRINT '2. EJECUTAR INMEDIATAMENTE: POST http://localhost:5189/api/Test/fix-user-passwords';
PRINT '   (Esto generará hashes BCrypt válidos)';
PRINT '3. Verificar: GET http://localhost:5189/api/Test/diagnose-catalog';
PRINT '4. Probar login: admin / Admin123!';
PRINT '5. Acceder a: http://localhost:5189/Catalogo';
PRINT '';
PRINT 'CREDENCIALES POST-FIX:';
PRINT '  Admin:     admin / Admin123!';
PRINT '  Vendedor:  vendedor1 / Vendedor123!';
PRINT '  Clientes:  cliente1 / Cliente123!';
PRINT '========================================';

-- Limpiar backups temporales
DROP TABLE #Backup_Full_Usuarios;
DROP TABLE #Backup_Full_Libros;
DROP TABLE #Backup_Full_Categorias;

GO
