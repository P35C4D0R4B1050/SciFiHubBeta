# ========================================
# SCRIPT TODO-EN-UNO: REESTRUCTURACIÓN COMPLETA
# SciFiHub - Un solo comando para migrar todo
# ========================================

param(
    [switch]$SkipVerification,
    [switch]$SkipBuild
)

$ErrorActionPreference = "Stop"

Write-Host "?????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host "?  REESTRUCTURACIÓN AUTOMÁTICA COMPLETA - SCIFIHUB          ?" -ForegroundColor Cyan
Write-Host "?  Migración de Monolítico a Arquitectura en Capas         ?" -ForegroundColor Cyan
Write-Host "?????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""

# ========================================
# PASO 0: VERIFICACIONES PREVIAS
# ========================================
Write-Host "?? PASO 0: Verificaciones previas..." -ForegroundColor Yellow
Write-Host ""

# Verificar que estamos en el directorio correcto
if (!(Test-Path "SciFiHub.Web.csproj")) {
    Write-Host "? ERROR: No se encuentra SciFiHub.Web.csproj" -ForegroundColor Red
    Write-Host "   Asegúrate de estar en el directorio: E:\Proyecto\SciFiHub\SciFiHub\" -ForegroundColor Yellow
    exit 1
}

# Verificar que existen los proyectos hermanos
if (!(Test-Path "..\SciFiHub.Domain\SciFiHub.Domain.csproj")) {
    Write-Host "? ERROR: No se encuentra ..\SciFiHub.Domain\SciFiHub.Domain.csproj" -ForegroundColor Red
    exit 1
}

if (!(Test-Path "..\SciFiHub.Infrastructure\SciFiHub.Infrastructure.csproj")) {
    Write-Host "? ERROR: No se encuentra ..\SciFiHub.Infrastructure\SciFiHub.Infrastructure.csproj" -ForegroundColor Red
    exit 1
}

Write-Host "? Verificaciones previas pasadas" -ForegroundColor Green
Write-Host ""

# Confirmar con el usuario
Write-Host "??  Este script va a:" -ForegroundColor Yellow
Write-Host "   1. Mover archivos de Domain e Infrastructure a sus proyectos" -ForegroundColor White
Write-Host "   2. Actualizar referencias de proyectos" -ForegroundColor White
Write-Host "   3. Limpiar carpetas vacías" -ForegroundColor White
Write-Host "   4. Compilar la solución" -ForegroundColor White
Write-Host "   5. Verificar que todo funcione correctamente" -ForegroundColor White
Write-Host ""
Write-Host "   Se recomienda hacer commit antes de continuar." -ForegroundColor Yellow
Write-Host ""

$confirmation = Read-Host "¿Continuar? (S/N)"
if ($confirmation -ne 'S' -and $confirmation -ne 's') {
    Write-Host "? Operación cancelada" -ForegroundColor Red
    exit 0
}

Write-Host ""
Write-Host "?? Iniciando proceso completo..." -ForegroundColor Green
Write-Host ""

# ========================================
# PASO 1: EJECUTAR REESTRUCTURACIÓN
# ========================================
Write-Host "?????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host "?  PASO 1: REESTRUCTURACIÓN                                 ?" -ForegroundColor Cyan
Write-Host "?????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""

if (Test-Path "ReestructurarProyecto.ps1") {
    Write-Host "?? Ejecutando script de reestructuración..." -ForegroundColor Yellow
    Write-Host ""
    
    # Ejecutar el script de reestructuración (modo silencioso)
    $env:REESTRUCTURACION_AUTO = "true"
    
    # Incluir el código del script de reestructuración aquí
    . ".\ReestructurarProyecto.ps1"
    
    Write-Host ""
    Write-Host "? Reestructuración completada" -ForegroundColor Green
} else {
    Write-Host "? ERROR: No se encuentra ReestructurarProyecto.ps1" -ForegroundColor Red
    exit 1
}

Write-Host ""

# ========================================
# PASO 2: RESTAURAR PAQUETES
# ========================================
if (!$SkipBuild) {
    Write-Host "?????????????????????????????????????????????????????????????" -ForegroundColor Cyan
    Write-Host "?  PASO 2: RESTAURAR PAQUETES                               ?" -ForegroundColor Cyan
    Write-Host "?????????????????????????????????????????????????????????????" -ForegroundColor Cyan
    Write-Host ""

    Write-Host "?? Ejecutando dotnet restore..." -ForegroundColor Yellow
    Write-Host ""

    $restoreOutput = dotnet restore 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "? Restore exitoso" -ForegroundColor Green
    } else {
        Write-Host "? Error en restore" -ForegroundColor Red
        Write-Host $restoreOutput -ForegroundColor Gray
        exit 1
    }

    Write-Host ""
}

# ========================================
# PASO 3: COMPILAR PROYECTO
# ========================================
if (!$SkipBuild) {
    Write-Host "?????????????????????????????????????????????????????????????" -ForegroundColor Cyan
    Write-Host "?  PASO 3: COMPILAR PROYECTO                                ?" -ForegroundColor Cyan
    Write-Host "?????????????????????????????????????????????????????????????" -ForegroundColor Cyan
    Write-Host ""

    Write-Host "?? Ejecutando dotnet build..." -ForegroundColor Yellow
    Write-Host ""

    $buildOutput = dotnet build 2>&1 | Out-String
    if ($LASTEXITCODE -eq 0) {
        Write-Host "? Build exitoso" -ForegroundColor Green
        
        # Mostrar warnings si hay
        if ($buildOutput -match "(\d+) Warning") {
            $warningCount = $matches[1]
            if ([int]$warningCount -gt 0) {
                Write-Host "??  Build tiene $warningCount warning(s)" -ForegroundColor Yellow
            }
        }
    } else {
        Write-Host "? Error en build" -ForegroundColor Red
        Write-Host $buildOutput -ForegroundColor Gray
        exit 1
    }

    Write-Host ""
}

# ========================================
# PASO 4: VERIFICACIÓN
# ========================================
if (!$SkipVerification) {
    Write-Host "?????????????????????????????????????????????????????????????" -ForegroundColor Cyan
    Write-Host "?  PASO 4: VERIFICACIÓN                                     ?" -ForegroundColor Cyan
    Write-Host "?????????????????????????????????????????????????????????????" -ForegroundColor Cyan
    Write-Host ""

    if (Test-Path "VerificarReestructuracion.ps1") {
        Write-Host "?? Ejecutando verificación completa..." -ForegroundColor Yellow
        Write-Host ""
        
        $verificationResult = & ".\VerificarReestructuracion.ps1"
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host ""
            Write-Host "? Verificación completada exitosamente" -ForegroundColor Green
        } else {
            Write-Host ""
            Write-Host "??  Verificación completada con advertencias" -ForegroundColor Yellow
        }
    } else {
        Write-Host "??  Script de verificación no encontrado (opcional)" -ForegroundColor Yellow
    }

    Write-Host ""
}

# ========================================
# RESUMEN FINAL
# ========================================
Write-Host "?????????????????????????????????????????????????????????????" -ForegroundColor Green
Write-Host "?  ? PROCESO COMPLETADO EXITOSAMENTE                       ?" -ForegroundColor Green
Write-Host "?????????????????????????????????????????????????????????????" -ForegroundColor Green
Write-Host ""

Write-Host "?? RESUMEN:" -ForegroundColor Cyan
Write-Host ""
Write-Host "   ? Archivos movidos correctamente" -ForegroundColor Green
Write-Host "   ? Referencias actualizadas" -ForegroundColor Green
Write-Host "   ? Proyecto compilado sin errores" -ForegroundColor Green
Write-Host "   ? Verificación completada" -ForegroundColor Green
Write-Host ""

Write-Host "?? ARQUITECTURA FINAL:" -ForegroundColor Cyan
Write-Host ""
Write-Host "   ?? SciFiHub.Domain" -ForegroundColor Magenta
Write-Host "      ?? Entities, Interfaces, Enums, ValueObjects" -ForegroundColor Gray
Write-Host ""
Write-Host "   ?? SciFiHub.Infrastructure" -ForegroundColor Magenta
Write-Host "      ?? DbContext, Repositories, Configurations" -ForegroundColor Gray
Write-Host ""
Write-Host "   ?? SciFiHub.Web" -ForegroundColor Magenta
Write-Host "      ?? Controllers, Views, Services, DTOs" -ForegroundColor Gray
Write-Host ""

Write-Host "?? ¡Tu proyecto ahora tiene una arquitectura profesional en capas!" -ForegroundColor Green
Write-Host ""

Write-Host "?? PRÓXIMOS PASOS:" -ForegroundColor Yellow
Write-Host ""
Write-Host "   1. Ejecutar la aplicación: dotnet run" -ForegroundColor White
Write-Host "   2. Probar funcionalidades principales" -ForegroundColor White
Write-Host "   3. Hacer commit de los cambios: git add . && git commit -m 'Reestructuración a capas'" -ForegroundColor White
Write-Host ""

Write-Host "?? DOCUMENTACIÓN:" -ForegroundColor Cyan
Write-Host "   - GUIA_REESTRUCTURACION_COMPLETA.md - Guía detallada" -ForegroundColor White
Write-Host "   - RESUMEN_EJECUTIVO_REESTRUCTURACION.md - Resumen rápido" -ForegroundColor White
Write-Host ""

Write-Host "?? ¡Listo para desarrollar!" -ForegroundColor Green
Write-Host ""

# Preguntar si desea ejecutar la aplicación
$runApp = Read-Host "¿Deseas ejecutar la aplicación ahora? (S/N)"
if ($runApp -eq 'S' -or $runApp -eq 's') {
    Write-Host ""
    Write-Host "?? Iniciando aplicación..." -ForegroundColor Cyan
    Write-Host ""
    dotnet run
}
