-- =============================================
-- SciFiHub Database - Data Seeding
-- SQL Server 2021+
-- Versión: 1.0
-- =============================================

USE SciFiHubDB;
GO

PRINT N'Iniciando inserción de datos iniciales...';
GO

-- =============================================
-- I. DATOS INICIALES (SEEDING)
-- =============================================

-- =============================================
-- 1. USUARIOS (Administrador, Vendedores, Clientes)
-- =============================================
PRINT N'Insertando usuarios...';

-- Nota: En producción, las contraseñas deben ser hasheadas con BCrypt o similar
-- Para este script usamos hashes simulados. Contraseña real: "Admin@2024"
DECLARE @AdminId UNIQUEIDENTIFIER = NEWID();
DECLARE @Vendedor1Id UNIQUEIDENTIFIER = NEWID();
DECLARE @Vendedor2Id UNIQUEIDENTIFIER = NEWID();
DECLARE @Cliente1Id UNIQUEIDENTIFIER = NEWID();
DECLARE @Cliente2Id UNIQUEIDENTIFIER = NEWID();
DECLARE @Cliente3Id UNIQUEIDENTIFIER = NEWID();

-- Usuario Administrador
INSERT INTO dbo.Usuarios (Id, NombreCompleto, Email, Username, PasswordHash, Rol, Estado, FechaRegistro, CreatedAt)
VALUES 
(@AdminId, 'Administrador Sistema', 'admin@scifihub.com', 'admin', '$2a$11$K8pZmNpRB.XNvJnEQz6yiO7JhZ5qL4mNH1pYz5xL8vK9nZ6qL4mNH', 'Administrador', 'Activo', GETUTCDATE(), GETUTCDATE());

-- Vendedores
INSERT INTO dbo.Usuarios (Id, NombreCompleto, Email, Username, PasswordHash, Rol, Estado, Telefono, FechaRegistro, CreatedAt)
VALUES 
(@Vendedor1Id, 'Carlos Mendoza', 'carlos.mendoza@scifihub.com', 'cmendoza', '$2a$11$V1nZ2qL4mNH1pYz5xL8vK9nZ6qL4mNH1pYz5xL8vK9nZ6qL4mNH', 'Vendedor', 'Activo', '987654321', GETUTCDATE(), GETUTCDATE()),
(@Vendedor2Id, 'María Torres', 'maria.torres@scifihub.com', 'mtorres', '$2a$11$W2oA3rM5nOI2qZa6yM9wL0oA3rM5nOI2qZa6yM9wL0oA3rM5nOI', 'Vendedor', 'Activo', '987654322', GETUTCDATE(), GETUTCDATE());

-- Clientes
INSERT INTO dbo.Usuarios (Id, NombreCompleto, Email, Username, PasswordHash, Rol, Estado, Telefono, FechaRegistro, CreatedAt)
VALUES 
(@Cliente1Id, 'Juan Pérez García', 'juan.perez@email.com', 'jperez', '$2a$11$C1lT3eN5tP3sQb7zN0xM1pB3sQb7zN0xM1pB3sQb7zN0xM1pB', 'Cliente', 'Activo', '998877665', GETUTCDATE(), GETUTCDATE()),
(@Cliente2Id, 'Ana Martínez López', 'ana.martinez@email.com', 'amartinez', '$2a$11$D2mU4fO6uQ4tRc8aO1yN2qC4tRc8aO1yN2qC4tRc8aO1yN2qC', 'Cliente', 'Activo', '998877666', GETUTCDATE(), GETUTCDATE()),
(@Cliente3Id, 'Roberto Silva Vargas', 'roberto.silva@email.com', 'rsilva', '$2a$11$E3nV5gP7vR5uSd9bP2zO3rD5uSd9bP2zO3rD5uSd9bP2zO3rD', 'Cliente', 'Activo', '998877667', GETUTCDATE(), GETUTCDATE());

PRINT N'Usuarios insertados: 1 Admin, 2 Vendedores, 3 Clientes';
GO

-- =============================================
-- 2. CATEGORÍAS Y 3. LIBROS (EN UN SOLO BATCH)
-- IMPORTANTE: Sin GO entre ellos para que las variables persistan
-- =============================================
PRINT N'Insertando categorías y libros...';

-- Declarar variables para categorías
DECLARE @CatCienciaFiccion UNIQUEIDENTIFIER = NEWID();
DECLARE @CatFantasia UNIQUEIDENTIFIER = NEWID();
DECLARE @CatDistopia UNIQUEIDENTIFIER = NEWID();
DECLARE @CatCyberpunk UNIQUEIDENTIFIER = NEWID();
DECLARE @CatSpaceOpera UNIQUEIDENTIFIER = NEWID();
DECLARE @CatPostApocaliptico UNIQUEIDENTIFIER = NEWID();
DECLARE @CatClasicos UNIQUEIDENTIFIER = NEWID();

-- Insertar categorías
INSERT INTO dbo.Categorias (Id, Nombre, Descripcion, Orden, Estado, CreatedAt)
VALUES 
(@CatCienciaFiccion, 'Ciencia Ficción', 'Historias basadas en ciencia y tecnología', 1, 'Activo', GETUTCDATE()),
(@CatFantasia, 'Fantasía', 'Mundos imaginarios y magia', 2, 'Activo', GETUTCDATE()),
(@CatDistopia, 'Distopía', 'Sociedades futuras opresivas', 3, 'Activo', GETUTCDATE()),
(@CatCyberpunk, 'Cyberpunk', 'Futuros tecnológicos oscuros', 4, 'Activo', GETUTCDATE()),
(@CatSpaceOpera, 'Space Opera', 'Aventuras espaciales épicas', 5, 'Activo', GETUTCDATE()),
(@CatPostApocaliptico, 'Post-Apocalíptico', 'Mundos después del fin', 6, 'Activo', GETUTCDATE()),
(@CatClasicos, 'Clásicos', 'Obras clásicas de la literatura', 7, 'Activo', GETUTCDATE());

PRINT N'Categorías insertadas: 7 categorías';

-- Insertar libros (usando las variables de categorías del mismo batch)
INSERT INTO dbo.Libros (ISBN, Titulo, Autor, Editorial, CategoriaId, AnioPublicacion, Sinopsis, Descripcion, Precio, PrecioOferta, Stock, ImagenPortada, NumeroPaginas, Idioma, Destacado, Estado, CreatedBy, CreatedAt)
VALUES 
-- Ciencia Ficción
('978-0441013593', 'Dune', 'Frank Herbert', 'Ace Books', @CatCienciaFiccion, 1965, 
'Una épica historia de política, religión y poder en el desierto planeta Arrakis.', 
'Dune es considerada una de las novelas de ciencia ficción más importantes de todos los tiempos', 
89.90, 75.90, 25, '/images/libros/dune.jpg', 688, 'Español', 1, 'Disponible', NULL, GETUTCDATE()),

('978-0345404473', 'Fundación', 'Isaac Asimov', 'Del Rey', @CatSpaceOpera, 1951, 
'La caída de un imperio galáctico y el plan para preservar el conocimiento humano.', 
'Primera novela de la saga de la Fundación, donde el matemático Hari Seldon predice la caída del Imperio Galáctico', 
69.90, NULL, 30, '/images/libros/fundacion.jpg', 255, 'Español', 1, 'Disponible', NULL, GETUTCDATE()),

('978-0451524935', '1984', 'George Orwell', 'Signet Classics', @CatDistopia, 1949, 
'Una advertencia sobre el totalitarismo y la vigilancia estatal.', 
'En un futuro distópico, Winston Smith trabaja en el Ministerio de la Verdad reescribiendo la historia', 
55.90, 45.00, 40, '/images/libros/1984.jpg', 328, 'Español', 1, 'Disponible', NULL, GETUTCDATE()),

('978-0060850524', 'Un Mundo Feliz', 'Aldous Huxley', 'Harper Perennial', @CatDistopia, 1932, 
'Una sociedad futurista donde la felicidad es obligatoria.', 
'En un Estado Mundial donde los seres humanos son creados en laboratorios', 
59.90, NULL, 35, '/images/libros/mundo-feliz.jpg', 311, 'Español', 1, 'Disponible', NULL, GETUTCDATE()),

('978-0441569595', 'Neuromante', 'William Gibson', 'Ace Books', @CatCyberpunk, 1984, 
'La novela que definió el género cyberpunk.', 
'Case, un hacker caído en desgracia, recibe una última oportunidad para un trabajo imposible en el ciberespacio', 
79.90, 65.00, 20, '/images/libros/neuromante.jpg', 271, 'Español', 1, 'Disponible', NULL, GETUTCDATE()),

('978-0553380958', 'Snow Crash', 'Neal Stephenson', 'Bantam Books', @CatCyberpunk, 1992, 
'Realidad virtual, corporaciones y virus informáticos.', 
'En un futuro cercano, Hiro Protagonist es repartidor de pizza y guerrero samurái en el Metaverso', 
72.90, NULL, 18, '/images/libros/snow-crash.jpg', 559, 'Español', 0, 'Disponible', NULL, GETUTCDATE()),

('978-0553293357', 'El Fin de la Eternidad', 'Isaac Asimov', 'Bantam Books', @CatCienciaFiccion, 1955, 
'Viajes en el tiempo y las consecuencias de alterar la historia.', 
'Los Eternos pueden viajar a través del tiempo y realizar Cambios para mejorar la Realidad', 
64.90, 52.00, 22, '/images/libros/fin-eternidad.jpg', 233, 'Español', 0, 'Disponible', NULL, GETUTCDATE()),

('978-0553283686', 'Hyperion', 'Dan Simmons', 'Bantam Books', @CatSpaceOpera, 1989, 
'Una peregrinación hacia un destino incierto en un mundo lejano.', 
'Siete peregrinos viajan a Hyperion para encontrarse con el Alcaudón, una criatura misteriosa', 
84.90, NULL, 15, '/images/libros/hyperion.jpg', 482, 'Español', 1, 'Disponible', NULL, GETUTCDATE()),

('978-0307743657', 'La Guerra de los Mundos', 'H.G. Wells', 'Penguin Classics', @CatClasicos, 1898, 
'La invasión marciana que cambió la ciencia ficción.', 
'Una de las primeras historias de invasión alienígena, donde los marcianos aterrizan en Inglaterra', 
49.90, 39.90, 45, '/images/libros/guerra-mundos.jpg', 192, 'Español', 0, 'Disponible', NULL, GETUTCDATE()),

('978-1451673319', 'Fahrenheit 451', 'Ray Bradbury', 'Simon & Schuster', @CatDistopia, 1953, 
'Una sociedad donde los libros están prohibidos.', 
'Guy Montag es un bombero cuyo trabajo es quemar libros', 
58.90, NULL, 38, '/images/libros/fahrenheit-451.jpg', 249, 'Español', 1, 'Disponible', NULL, GETUTCDATE()),

-- Fantasía
('978-0261102385', 'El Señor de los Anillos: La Comunidad del Anillo', 'J.R.R. Tolkien', 'Harper Collins', @CatFantasia, 1954, 
'El inicio de la épica aventura en la Tierra Media.', 
'Frodo Bolsón hereda un anillo mágico que debe ser destruido', 
95.90, 79.90, 50, '/images/libros/comunidad-anillo.jpg', 423, 'Español', 1, 'Disponible', NULL, GETUTCDATE()),

('978-0553103540', 'Juego de Tronos', 'George R.R. Martin', 'Bantam Books', @CatFantasia, 1996, 
'Intrigas políticas y batallas épicas por el Trono de Hierro.', 
'Primera novela de Canción de Hielo y Fuego, donde varias casas nobles luchan por el control de los Siete Reinos', 
92.90, NULL, 42, '/images/libros/juego-tronos.jpg', 694, 'Español', 1, 'Disponible', NULL, GETUTCDATE()),

('978-0316015844', 'El Nombre del Viento', 'Patrick Rothfuss', 'DAW Books', @CatFantasia, 2007, 
'La historia de Kvothe, un legendario mago y asesino.', 
'Kvothe narra su propia historia: desde su infancia en una tropa de actores', 
86.90, 72.00, 28, '/images/libros/nombre-viento.jpg', 662, 'Español', 1, 'Disponible', NULL, GETUTCDATE()),

('978-0765311788', 'El Imperio Final', 'Brandon Sanderson', 'Tor Books', @CatFantasia, 2006, 
'Un mundo cubierto de ceniza donde la magia proviene de metales.', 
'Primera novela de Nacidos de la Bruma, donde un grupo de rebeldes planea derrocar al Lord Legislador', 
78.90, NULL, 33, '/images/libros/imperio-final.jpg', 541, 'Español', 0, 'Disponible', NULL, GETUTCDATE()),

-- Post-Apocalíptico y otros
('978-0307743664', 'La Carretera', 'Cormac McCarthy', 'Vintage', @CatPostApocaliptico, 2006, 
'Un padre y su hijo viajan por un mundo devastado.', 
'En un futuro post-apocalíptico, un hombre y su hijo caminan hacia la costa en busca de supervivencia', 
62.90, 49.90, 26, '/images/libros/la-carretera.jpg', 287, 'Español', 1, 'Disponible', NULL, GETUTCDATE()),

('978-0316055437', 'Soy Leyenda', 'Richard Matheson', 'Orbit', @CatPostApocaliptico, 1954, 
'El último hombre en la Tierra, rodeado de vampiros.', 
'Robert Neville es el único superviviente de una plaga que ha convertido al resto de la humanidad en vampiros', 
52.90, NULL, 30, '/images/libros/soy-leyenda.jpg', 317, 'Español', 0, 'Disponible', NULL, GETUTCDATE()),

('978-0441172710', 'Enders Game', 'Orson Scott Card', 'Tor Books', @CatCienciaFiccion, 1985, 
'Un niño genio entrenado para salvar la humanidad.', 
'Ender Wiggin es reclutado en una escuela militar espacial', 
67.90, 55.00, 35, '/images/libros/enders-game.jpg', 324, 'Español', 1, 'Disponible', NULL, GETUTCDATE()),

('978-0765326355', 'Marte Rojo', 'Kim Stanley Robinson', 'Tor Books', @CatCienciaFiccion, 1992, 
'La colonización y terraformación de Marte.', 
'Primera novela de la trilogía marciana', 
88.90, NULL, 20, '/images/libros/marte-rojo.jpg', 572, 'Español', 0, 'Disponible', NULL, GETUTCDATE()),

('978-0316346627', 'La Mano Izquierda de la Oscuridad', 'Ursula K. Le Guin', 'Ace Books', @CatCienciaFiccion, 1969, 
'Una exploración de género y cultura en un mundo alienígena.', 
'Un enviado terrestre en el planeta Gethen', 
71.90, 59.90, 24, '/images/libros/mano-izquierda.jpg', 304, 'Español', 0, 'Disponible', NULL, GETUTCDATE()),

('978-0441007462', 'Forastero en Tierra Extraña', 'Robert A. Heinlein', 'Ace Books', @CatCienciaFiccion, 1961, 
'Un humano criado en Marte regresa a la Tierra.', 
'Valentine Michael Smith, criado por marcianos', 
73.90, NULL, 19, '/images/libros/forastero.jpg', 525, 'Español', 0, 'Disponible', NULL, GETUTCDATE()),

-- Libros adicionales (sin campo Descripcion para hacer el INSERT más corto)
('978-0345391803', 'El Juego de Ender', 'Orson Scott Card', 'Del Rey', @CatCienciaFiccion, 1985, 
'La continuación de la historia de Ender Wiggin.', 
'', 65.90, NULL, 28, '/images/libros/juego-ender.jpg', 368, 'Español', 0, 'Disponible', NULL, GETUTCDATE()),

('978-0441783588', 'Crónicas Marcianas', 'Ray Bradbury', 'Doubleday', @CatCienciaFiccion, 1950, 
'Relatos sobre la colonización de Marte.', 
'', 57.90, NULL, 32, '/images/libros/cronicas-marcianas.jpg', 182, 'Español', 1, 'Disponible', NULL, GETUTCDATE()),

('978-0441013594', 'Los Propios Dioses', 'Isaac Asimov', 'Doubleday', @CatCienciaFiccion, 1972, 
'Energía gratuita desde un universo paralelo.', 
'', 68.90, NULL, 21, '/images/libros/propios-dioses.jpg', 288, 'Español', 0, 'Disponible', NULL, GETUTCDATE()),

('978-0345353801', 'La Guerra Interminable', 'Joe Haldeman', 'St. Martins Press', @CatCienciaFiccion, 1974, 
'Una guerra que dura milenios por la dilatación temporal.', 
'', 63.90, NULL, 17, '/images/libros/guerra-interminable.jpg', 278, 'Español', 0, 'Disponible', NULL, GETUTCDATE()),

('978-0765311764', 'Elantris', 'Brandon Sanderson', 'Tor Books', @CatFantasia, 2005, 
'Una ciudad mágica caída en desgracia.', 
'', 75.90, NULL, 25, '/images/libros/elantris.jpg', 496, 'Español', 0, 'Disponible', NULL, GETUTCDATE()),

('978-0061233845', 'American Gods', 'Neil Gaiman', 'William Morrow', @CatFantasia, 2001, 
'Dioses antiguos vs dioses modernos en América.', 
'', 81.90, NULL, 30, '/images/libros/american-gods.jpg', 635, 'Español', 1, 'Disponible', NULL, GETUTCDATE()),

('978-0765342522', 'El Camino de los Reyes', 'Brandon Sanderson', 'Tor Books', @CatFantasia, 2010, 
'Primera novela del Archivo de las Tormentas.', 
'', 98.90, NULL, 22, '/images/libros/camino-reyes.jpg', 1007, 'Español', 1, 'Disponible', NULL, GETUTCDATE()),

('978-0385333849', 'Contacto', 'Carl Sagan', 'Simon & Schuster', @CatCienciaFiccion, 1985, 
'Primer contacto con inteligencia extraterrestre.', 
'', 69.90, NULL, 27, '/images/libros/contacto.jpg', 352, 'Español', 0, 'Disponible', NULL, GETUTCDATE()),

('978-0553803716', 'I Robot', 'Isaac Asimov', 'Spectra', @CatCienciaFiccion, 1950, 
'Relatos sobre robots y las Tres Leyes de la Robótica.', 
'', 54.90, NULL, 36, '/images/libros/yo-robot.jpg', 224, 'Español', 0, 'Disponible', NULL, GETUTCDATE()),

('978-0441172720', 'Solaris', 'Stanislaw Lem', 'Faber & Faber', @CatCienciaFiccion, 1961, 
'Un océano pensante en un planeta lejano.', 
'', 66.90, NULL, 16, '/images/libros/solaris.jpg', 204, 'Español', 0, 'Disponible', NULL, GETUTCDATE());

PRINT N'Libros insertados: 30 títulos reconocidos';
GO

PRINT N'========================================';
PRINT N'DATOS INICIALES INSERTADOS EXITOSAMENTE';
PRINT N'========================================';
PRINT N'Usuarios:';
PRINT N'  - Admin: admin@scifihub.com / Admin@2024';
PRINT N'  - Vendedor 1: carlos.mendoza@scifihub.com / Vendedor@2024';
PRINT N'  - Vendedor 2: maria.torres@scifihub.com / Vendedor@2024';
PRINT N'  - Cliente 1: juan.perez@email.com / Cliente@2024';
PRINT N'  - Cliente 2: ana.martinez@email.com / Cliente@2024';
PRINT N'  - Cliente 3: roberto.silva@email.com / Cliente@2024';
PRINT N'';
PRINT N'Categorías: 7 categorías de géneros';
PRINT N'Libros: 30 títulos en catálogo';
PRINT N'========================================';
GO
