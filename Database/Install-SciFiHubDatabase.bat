@echo off
REM =============================================
REM SciFiHub Database - Script de Instalación
REM Batch Script para Windows
REM Versión: 1.0
REM =============================================

setlocal enabledelayedexpansion

REM Configuración
set SERVER=localhost
set DATABASE=SciFiHubDB

REM Banner
cls
echo ========================================
echo    SCIFIHUB DATABASE - INSTALACION
echo ========================================
echo.

REM Verificar que estamos en la carpeta correcta
if not exist "01_CreateDatabase_SciFiHub.sql" (
    echo ERROR: No se encontraron los archivos SQL.
    echo Asegurese de ejecutar este script desde la carpeta Database.
    pause
    exit /b 1
)

echo Servidor SQL Server: %SERVER%
echo Base de datos: %DATABASE%
echo.
echo Presione cualquier tecla para continuar o Ctrl+C para cancelar...
pause >nul

echo.
echo Iniciando instalacion...
echo.

REM Crear carpeta de logs
if not exist "Logs" mkdir Logs

REM Variables de contador
set TOTAL=12
set CURRENT=0
set FAILED=0

REM Script 1
set /a CURRENT+=1
echo [%CURRENT%/%TOTAL%] Creando base de datos...
sqlcmd -S %SERVER% -i 01_CreateDatabase_SciFiHub.sql -o Logs\01_CreateDatabase_SciFiHub.log
if errorlevel 1 (
    echo   ERROR al ejecutar script 1
    set FAILED=1
    goto :error
)
echo   Completado exitosamente
echo.

REM Script 2
set /a CURRENT+=1
echo [%CURRENT%/%TOTAL%] Creando tablas...
sqlcmd -S %SERVER% -i 02_CreateTables_SciFiHub.sql -o Logs\02_CreateTables_SciFiHub.log
if errorlevel 1 (
    echo   ERROR al ejecutar script 2
    set FAILED=1
    goto :error
)
echo   Completado exitosamente
echo.

REM Script 3
set /a CURRENT+=1
echo [%CURRENT%/%TOTAL%] Creando funciones...
sqlcmd -S %SERVER% -i 03_CreateFunctions_SciFiHub.sql -o Logs\03_CreateFunctions_SciFiHub.log
if errorlevel 1 (
    echo   ERROR al ejecutar script 3
    set FAILED=1
    goto :error
)
echo   Completado exitosamente
echo.

REM Script 4
set /a CURRENT+=1
echo [%CURRENT%/%TOTAL%] Creando SPs de Usuarios...
sqlcmd -S %SERVER% -i 04_CreateSP_Usuarios_SciFiHub.sql -o Logs\04_CreateSP_Usuarios_SciFiHub.log
if errorlevel 1 (
    echo   ERROR al ejecutar script 4
    set FAILED=1
    goto :error
)
echo   Completado exitosamente
echo.

REM Script 5
set /a CURRENT+=1
echo [%CURRENT%/%TOTAL%] Creando SPs de Libros...
sqlcmd -S %SERVER% -i 05_CreateSP_Libros_SciFiHub.sql -o Logs\05_CreateSP_Libros_SciFiHub.log
if errorlevel 1 (
    echo   ERROR al ejecutar script 5
    set FAILED=1
    goto :error
)
echo   Completado exitosamente
echo.

REM Script 6
set /a CURRENT+=1
echo [%CURRENT%/%TOTAL%] Creando SPs de Ventas...
sqlcmd -S %SERVER% -i 06_CreateSP_Ventas_SciFiHub.sql -o Logs\06_CreateSP_Ventas_SciFiHub.log
if errorlevel 1 (
    echo   ERROR al ejecutar script 6
    set FAILED=1
    goto :error
)
echo   Completado exitosamente
echo.

REM Script 7
set /a CURRENT+=1
echo [%CURRENT%/%TOTAL%] Creando SPs de Carrito...
sqlcmd -S %SERVER% -i 07_CreateSP_Carrito_SciFiHub.sql -o Logs\07_CreateSP_Carrito_SciFiHub.log
if errorlevel 1 (
    echo   ERROR al ejecutar script 7
    set FAILED=1
    goto :error
)
echo   Completado exitosamente
echo.

REM Script 8
set /a CURRENT+=1
echo [%CURRENT%/%TOTAL%] Creando SPs de Reportes...
sqlcmd -S %SERVER% -i 08_CreateSP_Reportes_SciFiHub.sql -o Logs\08_CreateSP_Reportes_SciFiHub.log
if errorlevel 1 (
    echo   ERROR al ejecutar script 8
    set FAILED=1
    goto :error
)
echo   Completado exitosamente
echo.

REM Script 9
set /a CURRENT+=1
echo [%CURRENT%/%TOTAL%] Creando vistas...
sqlcmd -S %SERVER% -i 09_CreateViews_SciFiHub.sql -o Logs\09_CreateViews_SciFiHub.log
if errorlevel 1 (
    echo   ERROR al ejecutar script 9
    set FAILED=1
    goto :error
)
echo   Completado exitosamente
echo.

REM Script 10
set /a CURRENT+=1
echo [%CURRENT%/%TOTAL%] Creando triggers...
sqlcmd -S %SERVER% -i 10_CreateTriggers_SciFiHub.sql -o Logs\10_CreateTriggers_SciFiHub.log
if errorlevel 1 (
    echo   ERROR al ejecutar script 10
    set FAILED=1
    goto :error
)
echo   Completado exitosamente
echo.

REM Script 11
set /a CURRENT+=1
echo [%CURRENT%/%TOTAL%] Configurando Full-Text Search...
sqlcmd -S %SERVER% -i 11_CreateFullTextIndex_SciFiHub.sql -o Logs\11_CreateFullTextIndex_SciFiHub.log
if errorlevel 1 (
    echo   ERROR al ejecutar script 11
    set FAILED=1
    goto :error
)
echo   Completado exitosamente
echo.

REM Script 12
set /a CURRENT+=1
echo [%CURRENT%/%TOTAL%] Insertando datos iniciales...
sqlcmd -S %SERVER% -i 12_SeedData_SciFiHub.sql -o Logs\12_SeedData_SciFiHub.log
if errorlevel 1 (
    echo   ERROR al ejecutar script 12
    set FAILED=1
    goto :error
)
echo   Completado exitosamente
echo.

REM Éxito
:success
echo ========================================
echo    INSTALACION COMPLETADA EXITOSAMENTE
echo ========================================
echo.
echo Base de datos: %DATABASE%
echo Servidor: %SERVER%
echo.
echo CREDENCIALES POR DEFECTO:
echo.
echo Administrador:
echo   Usuario: admin
echo   Email: admin@scifihub.com
echo   Password: Admin@2024
echo.
echo Vendedores:
echo   cmendoza / mtorres
echo   Password: Vendedor@2024
echo.
echo Clientes de prueba:
echo   jperez / amartinez / rsilva
echo   Password: Cliente@2024
echo.
echo La base de datos esta lista para usarse!
echo.
pause
exit /b 0

REM Error
:error
echo ========================================
echo    INSTALACION FALLIDA
echo ========================================
echo.
echo La instalacion no se completo correctamente.
echo Revise los archivos de log en la carpeta Logs para mas detalles.
echo.
pause
exit /b 1
