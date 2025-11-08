# =============================================
# SciFiHub Database - Script de Instalación Automatizada
# PowerShell Script
# Versión: 1.0
# =============================================

<#
.SYNOPSIS
    Instala la base de datos SciFiHub ejecutando todos los scripts SQL en orden.

.DESCRIPTION
    Este script ejecuta automáticamente todos los archivos SQL necesarios para crear
    la base de datos completa de SciFiHub, incluyendo tablas, procedimientos,
    funciones, vistas, triggers, índices y datos iniciales.

.PARAMETER ServerInstance
    Instancia de SQL Server (por defecto: localhost)

.PARAMETER DatabaseName
    Nombre de la base de datos (por defecto: SciFiHubDB)

.PARAMETER CreateLogFolder
    Crear carpeta de logs (por defecto: $true)

.EXAMPLE
    .\Install-SciFiHubDatabase.ps1
    .\Install-SciFiHubDatabase.ps1 -ServerInstance "localhost\SQLEXPRESS"
    .\Install-SciFiHubDatabase.ps1 -ServerInstance "." -DatabaseName "SciFiHubDB"

.NOTES
    Requiere SQL Server Management Studio o SQL Server con SQLCMD instalado.
    Asegúrese de tener permisos de administrador en SQL Server.
#>

param(
    [string]$ServerInstance = "localhost",
    [string]$DatabaseName = "SciFiHubDB",
    [bool]$CreateLogFolder = $true
)

# Configuración de colores
$ColorSuccess = "Green"
$ColorError = "Red"
$ColorWarning = "Yellow"
$ColorInfo = "Cyan"

# Banner
Clear-Host
Write-Host "========================================" -ForegroundColor $ColorInfo
Write-Host "   SCIFIHUB DATABASE - INSTALACIÓN     " -ForegroundColor $ColorInfo
Write-Host "========================================" -ForegroundColor $ColorInfo
Write-Host ""

# Validar que estamos en la carpeta correcta
$currentPath = Get-Location
if (-not (Test-Path "01_CreateDatabase_SciFiHub.sql")) {
    Write-Host "ERROR: No se encontraron los archivos SQL." -ForegroundColor $ColorError
    Write-Host "Asegúrese de ejecutar este script desde la carpeta Database." -ForegroundColor $ColorError
    exit 1
}

# Crear carpeta de logs si no existe
if ($CreateLogFolder) {
    $logFolder = Join-Path $currentPath "Logs"
    if (-not (Test-Path $logFolder)) {
        New-Item -ItemType Directory -Path $logFolder -Force | Out-Null
        Write-Host "? Carpeta de logs creada: $logFolder" -ForegroundColor $ColorSuccess
    }
}

# Verificar conectividad con SQL Server
Write-Host "Verificando conexión con SQL Server..." -ForegroundColor $ColorInfo
try {
    $testQuery = "SELECT @@VERSION"
    $result = sqlcmd -S $ServerInstance -Q $testQuery -h -1 2>&1
    if ($LASTEXITCODE -ne 0) {
        throw "No se pudo conectar a SQL Server"
    }
    Write-Host "? Conexión exitosa con: $ServerInstance" -ForegroundColor $ColorSuccess
    Write-Host ""
} catch {
    Write-Host "? ERROR: No se pudo conectar a SQL Server en $ServerInstance" -ForegroundColor $ColorError
    Write-Host "Verifique que SQL Server esté ejecutándose y que SQLCMD esté instalado." -ForegroundColor $ColorWarning
    exit 1
}

# Lista de scripts a ejecutar en orden
$scripts = @(
    @{Name="01_CreateDatabase_SciFiHub.sql"; Description="Creando base de datos"},
    @{Name="02_CreateTables_SciFiHub.sql"; Description="Creando tablas"},
    @{Name="03_CreateFunctions_SciFiHub.sql"; Description="Creando funciones"},
    @{Name="04_CreateSP_Usuarios_SciFiHub.sql"; Description="Creando SPs de Usuarios"},
    @{Name="05_CreateSP_Libros_SciFiHub.sql"; Description="Creando SPs de Libros"},
    @{Name="06_CreateSP_Ventas_SciFiHub.sql"; Description="Creando SPs de Ventas"},
    @{Name="07_CreateSP_Carrito_SciFiHub.sql"; Description="Creando SPs de Carrito"},
    @{Name="08_CreateSP_Reportes_SciFiHub.sql"; Description="Creando SPs de Reportes"},
    @{Name="09_CreateViews_SciFiHub.sql"; Description="Creando vistas"},
    @{Name="10_CreateTriggers_SciFiHub.sql"; Description="Creando triggers"},
    @{Name="11_CreateFullTextIndex_SciFiHub.sql"; Description="Configurando Full-Text Search"},
    @{Name="12_SeedData_SciFiHub.sql"; Description="Insertando datos iniciales"}
)

# Contador de progreso
$total = $scripts.Count
$current = 0
$failed = $false

Write-Host "Iniciando instalación de $DatabaseName..." -ForegroundColor $ColorInfo
Write-Host "Servidor: $ServerInstance" -ForegroundColor $ColorInfo
Write-Host "Total de scripts a ejecutar: $total" -ForegroundColor $ColorInfo
Write-Host ""

# Ejecutar cada script
foreach ($script in $scripts) {
    $current++
    $fileName = $script.Name
    $description = $script.Description
    
    Write-Host "[$current/$total] $description..." -ForegroundColor $ColorInfo
    Write-Host "        Archivo: $fileName" -ForegroundColor Gray
    
    # Verificar que el archivo existe
    if (-not (Test-Path $fileName)) {
        Write-Host "        ? ERROR: Archivo no encontrado: $fileName" -ForegroundColor $ColorError
        $failed = $true
        break
    }
    
    # Ejecutar el script
    $logFile = if ($CreateLogFolder) { Join-Path $logFolder "$fileName.log" } else { $null }
    
    try {
        if ($logFile) {
            sqlcmd -S $ServerInstance -i $fileName -o $logFile -I 2>&1 | Out-Null
        } else {
            sqlcmd -S $ServerInstance -i $fileName -I 2>&1 | Out-Null
        }
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "        ? Completado exitosamente" -ForegroundColor $ColorSuccess
            if ($logFile) {
                Write-Host "        Log: $logFile" -ForegroundColor Gray
            }
        } else {
            Write-Host "        ? ERROR al ejecutar el script" -ForegroundColor $ColorError
            if ($logFile) {
                Write-Host "        Ver detalles en: $logFile" -ForegroundColor $ColorWarning
            }
            $failed = $true
            break
        }
    } catch {
        Write-Host "        ? EXCEPCIÓN: $($_.Exception.Message)" -ForegroundColor $ColorError
        $failed = $true
        break
    }
    
    Write-Host ""
}

# Resumen final
Write-Host "========================================" -ForegroundColor $ColorInfo
if (-not $failed) {
    Write-Host "   INSTALACIÓN COMPLETADA EXITOSAMENTE" -ForegroundColor $ColorSuccess
    Write-Host "========================================" -ForegroundColor $ColorInfo
    Write-Host ""
    Write-Host "Base de datos: $DatabaseName" -ForegroundColor $ColorSuccess
    Write-Host "Servidor: $ServerInstance" -ForegroundColor $ColorSuccess
    Write-Host ""
    Write-Host "CREDENCIALES POR DEFECTO:" -ForegroundColor $ColorWarning
    Write-Host ""
    Write-Host "Administrador:" -ForegroundColor $ColorInfo
    Write-Host "  Usuario: admin" -ForegroundColor Gray
    Write-Host "  Email: admin@scifihub.com" -ForegroundColor Gray
    Write-Host "  Password: Admin@2024" -ForegroundColor Gray
    Write-Host ""
    Write-Host "Vendedores:" -ForegroundColor $ColorInfo
    Write-Host "  cmendoza / mtorres" -ForegroundColor Gray
    Write-Host "  Password: Vendedor@2024" -ForegroundColor Gray
    Write-Host ""
    Write-Host "Clientes de prueba:" -ForegroundColor $ColorInfo
    Write-Host "  jperez / amartinez / rsilva" -ForegroundColor Gray
    Write-Host "  Password: Cliente@2024" -ForegroundColor Gray
    Write-Host ""
    Write-Host "La base de datos está lista para usarse!" -ForegroundColor $ColorSuccess
    Write-Host ""
    
    exit 0
} else {
    Write-Host "   INSTALACIÓN FALLIDA" -ForegroundColor $ColorError
    Write-Host "========================================" -ForegroundColor $ColorInfo
    Write-Host ""
    Write-Host "La instalación no se completó correctamente." -ForegroundColor $ColorError
    if ($CreateLogFolder) {
        Write-Host "Revise los archivos de log en la carpeta Logs para más detalles." -ForegroundColor $ColorWarning
    }
    Write-Host ""
    
    exit 1
}
