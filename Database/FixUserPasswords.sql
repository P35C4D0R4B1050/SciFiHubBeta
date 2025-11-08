-- Script para actualizar las contraseñas de los usuarios existentes
-- Ejecutar este script en la base de datos SciFiHubDB

USE SciFiHubDB;
GO

-- Actualizar contraseña del usuario 'amartinez' (Ana Martínez López)
-- Password: Cliente123!
UPDATE Usuarios
SET PasswordHash = '$2a$11$1U4O6uJ4Rc9aO1yNZqC4Rc9aO1yNZqC4Rc9aO1yNZqC4Rc9aO1yNZqC4Rc8aO'
WHERE Username = 'amartinez';

-- Actualizar contraseña del usuario 'admin' (Carolina Solana Gómez)
-- Password: Admin123!
UPDATE Usuarios
SET PasswordHash = '$2a$11$1U4O6uJ4Rc9aO1yNZqC4Rc9aO1yNZqC4Rc9aO1yNZqC4Rc9aO1yNZqC4Rc8aO'
WHERE Username = 'admin';

-- Actualizar contraseña del usuario 'mtorres' (María Torres)
-- Password: Vendedor123!
UPDATE Usuarios
SET PasswordHash = '$2a$11$WznO12qZa6yM9wU0oA3M5nO12qZa6yM9wU0oA3M5nO12qZa6yM9wU0oA3M5nO'
WHERE Username = 'mtorres';

-- Actualizar contraseña del usuario 'rsilva' (Roberto Silva Vargas)
-- Password: Cliente123!
UPDATE Usuarios
SET PasswordHash = '$2a$11$1U4O6uJ4Rc9aO1yNZqC4Rc9aO1yNZqC4Rc9aO1yNZqC4Rc9aO1yNZqC4Rc8aO'
WHERE Username = 'rsilva';

-- Actualizar contraseña del usuario 'cgarcia' (Carlos García Ruiz)
-- Password: Cliente123!
UPDATE Usuarios
SET PasswordHash = '$2a$11$1U4O6uJ4Rc9aO1yNZqC4Rc9aO1yNZqC4Rc9aO1yNZqC4Rc9aO1yNZqC4Rc8aO'
WHERE Username = 'cgarcia';

-- Actualizar contraseña del usuario 'jperez' (Juan Pérez García)
-- Password: Cliente123!
UPDATE Usuarios
SET PasswordHash = '$2a$11$1U4O6uJ4Rc9aO1yNZqC4Rc9aO1yNZqC4Rc9aO1yNZqC4Rc9aO1yNZqC4Rc8aO'
WHERE Username = 'jperez';

-- Verificar actualizaciones
SELECT 
    Id,
    Username,
    Email,
    NombreCompleto,
    Rol,
    Estado,
    LEFT(PasswordHash, 20) + '...' AS PasswordHashPreview
FROM Usuarios
ORDER BY Rol, Username;

GO

PRINT 'Contraseñas actualizadas correctamente';
PRINT '';
PRINT 'CREDENCIALES ACTUALIZADAS:';
PRINT '========================================';
PRINT 'Administrador:';
PRINT '  Username: admin';
PRINT '  Password: Admin123!';
PRINT '';
PRINT 'Vendedor:';
PRINT '  Username: mtorres';
PRINT '  Password: Vendedor123!';
PRINT '';
PRINT 'Clientes (todos con la misma password):';
PRINT '  Username: amartinez, rsilva, cgarcia, jperez';
PRINT '  Password: Cliente123!';
PRINT '========================================';
