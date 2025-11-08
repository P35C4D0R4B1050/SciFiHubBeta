-- =============================================
-- SciFiHub Database - Script Maestro de Ejecución
-- SQL Server 2021+
-- Versión: 1.0
-- Ejecuta todos los scripts en el orden correcto
-- =============================================

/*
INSTRUCCIONES DE USO:
1. Abrir SQL Server Management Studio (SSMS) 21
2. Conectarse a la instancia de SQL Server
3. Ejecutar este script maestro que llamará a todos los demás scripts en orden
4. Alternativamente, ejecutar cada script individualmente en el orden especificado

ORDEN DE EJECUCIÓN:
1. 01_CreateDatabase_SciFiHub.sql - Crear base de datos
2. 02_CreateTables_SciFiHub.sql - Crear todas las tablas
3. 03_CreateFunctions_SciFiHub.sql - Crear funciones escalares y TVF
4. 04_CreateSP_Usuarios_SciFiHub.sql - Crear SPs de Usuarios
5. 05_CreateSP_Libros_SciFiHub.sql - Crear SPs de Libros
6. 06_CreateSP_Ventas_SciFiHub.sql - Crear SPs de Ventas
7. 07_CreateSP_Carrito_SciFiHub.sql - Crear SPs de Carrito
8. 08_CreateSP_Reportes_SciFiHub.sql - Crear SPs de Reportes
9. 09_CreateViews_SciFiHub.sql - Crear Vistas
10. 10_CreateTriggers_SciFiHub.sql - Crear Triggers
11. 11_CreateFullTextIndex_SciFiHub.sql - Configurar Full-Text Search
12. 12_SeedData_SciFiHub.sql - Insertar datos iniciales

NOTAS IMPORTANTES:
- Asegúrese de que SQL Server tenga Full-Text Search instalado
- Ajuste las rutas de archivos de datos en el script 01 según su configuración
- Las contraseñas en el seeding son hasheadas (simuladas), en producción usar BCrypt
- Todos los scripts están diseñados para ser idempotentes cuando sea posible
*/

PRINT N'========================================';
PRINT N'SCIFIHUB DATABASE - INSTALACIÓN COMPLETA';
PRINT N'========================================';
PRINT N'';

-- Verificar versión de SQL Server
DECLARE @Version NVARCHAR(128) = CAST(SERVERPROPERTY('ProductVersion') AS NVARCHAR(128));
DECLARE @Edition NVARCHAR(128) = CAST(SERVERPROPERTY('Edition') AS NVARCHAR(128));

PRINT N'SQL Server Version: ' + @Version;
PRINT N'SQL Server Edition: ' + @Edition;
PRINT N'';

-- Configuración de opciones de ejecución
SET NOCOUNT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

PRINT N'========================================';
PRINT N'INSTRUCCIONES:';
PRINT N'Este script maestro debe ejecutarse en orden.';
PRINT N'Si prefiere control manual, ejecute cada archivo .sql individual.';
PRINT N'';
PRINT N'ARCHIVOS A EJECUTAR EN ORDEN:';
PRINT N'1. 01_CreateDatabase_SciFiHub.sql';
PRINT N'2. 02_CreateTables_SciFiHub.sql';
PRINT N'3. 03_CreateFunctions_SciFiHub.sql';
PRINT N'4. 04_CreateSP_Usuarios_SciFiHub.sql';
PRINT N'5. 05_CreateSP_Libros_SciFiHub.sql';
PRINT N'6. 06_CreateSP_Ventas_SciFiHub.sql';
PRINT N'7. 07_CreateSP_Carrito_SciFiHub.sql';
PRINT N'8. 08_CreateSP_Reportes_SciFiHub.sql';
PRINT N'9. 09_CreateViews_SciFiHub.sql';
PRINT N'10. 10_CreateTriggers_SciFiHub.sql';
PRINT N'11. 11_CreateFullTextIndex_SciFiHub.sql';
PRINT N'12. 12_SeedData_SciFiHub.sql';
PRINT N'========================================';
PRINT N'';

PRINT N'Para ejecutar todos los scripts automáticamente,';
PRINT N'use SQLCMD o ejecute cada archivo individualmente en SSMS.';
PRINT N'';
PRINT N'Ejemplo con SQLCMD:';
PRINT N'sqlcmd -S localhost -i 01_CreateDatabase_SciFiHub.sql';
PRINT N'sqlcmd -S localhost -i 02_CreateTables_SciFiHub.sql';
PRINT N'... y así sucesivamente';
PRINT N'';

PRINT N'========================================';
PRINT N'CREDENCIALES POR DEFECTO DESPUÉS DEL SEEDING:';
PRINT N'========================================';
PRINT N'Administrador:';
PRINT N'  Usuario: admin';
PRINT N'  Email: admin@scifihub.com';
PRINT N'  Password: Admin@2024';
PRINT N'';
PRINT N'Vendedores:';
PRINT N'  Usuario: cmendoza / mtorres';
PRINT N'  Emails: carlos.mendoza@scifihub.com / maria.torres@scifihub.com';
PRINT N'  Password: Vendedor@2024';
PRINT N'';
PRINT N'Clientes de prueba:';
PRINT N'  Usuarios: jperez / amartinez / rsilva';
PRINT N'  Password: Cliente@2024';
PRINT N'========================================';
GO

/*
PARA EJECUCIÓN AUTOMÁTICA CON SQLCMD (PowerShell):

$scripts = @(
    "01_CreateDatabase_SciFiHub.sql",
    "02_CreateTables_SciFiHub.sql",
    "03_CreateFunctions_SciFiHub.sql",
    "04_CreateSP_Usuarios_SciFiHub.sql",
    "05_CreateSP_Libros_SciFiHub.sql",
    "06_CreateSP_Ventas_SciFiHub.sql",
    "07_CreateSP_Carrito_SciFiHub.sql",
    "08_CreateSP_Reportes_SciFiHub.sql",
    "09_CreateViews_SciFiHub.sql",
    "10_CreateTriggers_SciFiHub.sql",
    "11_CreateFullTextIndex_SciFiHub.sql",
    "12_SeedData_SciFiHub.sql"
)

foreach ($script in $scripts) {
    Write-Host "Ejecutando $script..." -ForegroundColor Green
    sqlcmd -S localhost -i "Database\$script" -o "Logs\$script.log"
    if ($LASTEXITCODE -eq 0) {
        Write-Host "  ? Completado exitosamente" -ForegroundColor Green
    } else {
        Write-Host "  ? Error al ejecutar" -ForegroundColor Red
        break
    }
}
*/
