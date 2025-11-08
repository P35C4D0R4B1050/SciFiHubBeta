# ========================================
# SCRIPT DE VERIFICACIÓN POST-REESTRUCTURACIÓN
# SciFiHub - Validación de Arquitectura en Capas
# ========================================

Write-Host "?????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host "?  VERIFICACIÓN POST-REESTRUCTURACIÓN SCIFIHUB              ?" -ForegroundColor Cyan
Write-Host "?  Validación de Arquitectura en Capas                      ?" -ForegroundColor Cyan
Write-Host "?????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""

$errores = 0
$warnings = 0

# ========================================
# VERIFICACIÓN 1: ESTRUCTURA DE PROYECTOS
# ========================================
Write-Host "?? VERIFICACIÓN 1: Estructura de Proyectos" -ForegroundColor Cyan
Write-Host ""

# Verificar SciFiHub.Domain
Write-Host "   Verificando SciFiHub.Domain..." -ForegroundColor Yellow
$domainFiles = @(
    "..\SciFiHub.Domain\Common\BaseEntity.cs",
    "..\SciFiHub.Domain\Entities\Usuario.cs",
    "..\SciFiHub.Domain\Entities\Libro.cs",
    "..\SciFiHub.Domain\Entities\Categoria.cs",
    "..\SciFiHub.Domain\Entities\Venta.cs",
    "..\SciFiHub.Domain\Entities\CarritoCompra.cs",
    "..\SciFiHub.Domain\Enums\Enumerations.cs",
    "..\SciFiHub.Domain\Interfaces\IRepository.cs",
    "..\SciFiHub.Domain\Interfaces\IUnitOfWork.cs"
)

foreach ($file in $domainFiles) {
    if (Test-Path $file) {
        Write-Host "      ? $file" -ForegroundColor Green
    } else {
        Write-Host "      ? NO ENCONTRADO: $file" -ForegroundColor Red
        $errores++
    }
}

# Verificar SciFiHub.Infrastructure
Write-Host ""
Write-Host "   Verificando SciFiHub.Infrastructure..." -ForegroundColor Yellow
$infrastructureFiles = @(
    "..\SciFiHub.Infrastructure\Data\SciFiHubDbContext.cs",
    "..\SciFiHub.Infrastructure\Repositories\Repository.cs",
    "..\SciFiHub.Infrastructure\Repositories\UnitOfWork.cs",
    "..\SciFiHub.Infrastructure\Repositories\UsuarioRepository.cs",
    "..\SciFiHub.Infrastructure\Repositories\LibroRepository.cs"
)

foreach ($file in $infrastructureFiles) {
    if (Test-Path $file) {
        Write-Host "      ? $file" -ForegroundColor Green
    } else {
        Write-Host "      ? NO ENCONTRADO: $file" -ForegroundColor Red
        $errores++
    }
}

# Verificar que las carpetas viejas fueron eliminadas
Write-Host ""
Write-Host "   Verificando que carpetas antiguas fueron eliminadas..." -ForegroundColor Yellow
$oldFolders = @(
    "SciFiHub.Domain",
    "SciFiHub.Infrastructure"
)

foreach ($folder in $oldFolders) {
    if (Test-Path $folder) {
        Write-Host "      ??  CARPETA ANTIGUA AÚN EXISTE: $folder" -ForegroundColor Yellow
        $warnings++
    } else {
        Write-Host "      ? Carpeta antigua eliminada: $folder" -ForegroundColor Green
    }
}

Write-Host ""
if ($errores -eq 0) {
    Write-Host "? Verificación 1: PASADA" -ForegroundColor Green
} else {
    Write-Host "? Verificación 1: FALLIDA ($errores errores)" -ForegroundColor Red
}
Write-Host ""

# ========================================
# VERIFICACIÓN 2: REFERENCIAS DE PROYECTOS
# ========================================
Write-Host "?? VERIFICACIÓN 2: Referencias de Proyectos" -ForegroundColor Cyan
Write-Host ""

# Verificar SciFiHub.Web.csproj
Write-Host "   Verificando SciFiHub.Web.csproj..." -ForegroundColor Yellow
if (Test-Path "SciFiHub.Web.csproj") {
    $webCsproj = Get-Content "SciFiHub.Web.csproj" -Raw
    
    if ($webCsproj -match "SciFiHub\.Infrastructure\.csproj") {
        Write-Host "      ? Referencia a SciFiHub.Infrastructure encontrada" -ForegroundColor Green
    } else {
        Write-Host "      ? Referencia a SciFiHub.Infrastructure NO encontrada" -ForegroundColor Red
        $errores++
    }
    
    # Verificar que NO tenga BCrypt (debe estar solo en Infrastructure)
    if ($webCsproj -match "BCrypt\.Net-Next") {
        Write-Host "      ??  BCrypt.Net-Next debería estar solo en Infrastructure" -ForegroundColor Yellow
        $warnings++
    } else {
        Write-Host "      ? BCrypt.Net-Next no está en Web (correcto)" -ForegroundColor Green
    }
} else {
    Write-Host "      ? SciFiHub.Web.csproj NO encontrado" -ForegroundColor Red
    $errores++
}

# Verificar SciFiHub.Infrastructure.csproj
Write-Host ""
Write-Host "   Verificando SciFiHub.Infrastructure.csproj..." -ForegroundColor Yellow
if (Test-Path "..\SciFiHub.Infrastructure\SciFiHub.Infrastructure.csproj") {
    $infraCsproj = Get-Content "..\SciFiHub.Infrastructure\SciFiHub.Infrastructure.csproj" -Raw
    
    if ($infraCsproj -match "SciFiHub\.Domain\.csproj") {
        Write-Host "      ? Referencia a SciFiHub.Domain encontrada" -ForegroundColor Green
    } else {
        Write-Host "      ? Referencia a SciFiHub.Domain NO encontrada" -ForegroundColor Red
        $errores++
    }
    
    if ($infraCsproj -match "EntityFrameworkCore") {
        Write-Host "      ? Entity Framework Core encontrado" -ForegroundColor Green
    } else {
        Write-Host "      ? Entity Framework Core NO encontrado" -ForegroundColor Red
        $errores++
    }
} else {
    Write-Host "      ? SciFiHub.Infrastructure.csproj NO encontrado" -ForegroundColor Red
    $errores++
}

Write-Host ""
if ($errores -eq 0) {
    Write-Host "? Verificación 2: PASADA" -ForegroundColor Green
} else {
    Write-Host "? Verificación 2: FALLIDA" -ForegroundColor Red
}
Write-Host ""

# ========================================
# VERIFICACIÓN 3: COMPILACIÓN
# ========================================
Write-Host "?? VERIFICACIÓN 3: Compilación del Proyecto" -ForegroundColor Cyan
Write-Host ""

Write-Host "   Ejecutando 'dotnet restore'..." -ForegroundColor Yellow
$restoreOutput = dotnet restore 2>&1
if ($LASTEXITCODE -eq 0) {
    Write-Host "      ? Restore exitoso" -ForegroundColor Green
} else {
    Write-Host "      ? Restore falló" -ForegroundColor Red
    Write-Host $restoreOutput -ForegroundColor Gray
    $errores++
}

Write-Host ""
Write-Host "   Ejecutando 'dotnet build'..." -ForegroundColor Yellow
$buildOutput = dotnet build 2>&1 | Out-String
if ($LASTEXITCODE -eq 0) {
    Write-Host "      ? Build exitoso" -ForegroundColor Green
    
    # Verificar warnings
    if ($buildOutput -match "(\d+) Warning") {
        $warningCount = $matches[1]
        if ([int]$warningCount -gt 0) {
            Write-Host "      ??  Build tiene $warningCount warning(s)" -ForegroundColor Yellow
            $warnings += [int]$warningCount
        }
    }
} else {
    Write-Host "      ? Build falló" -ForegroundColor Red
    Write-Host $buildOutput -ForegroundColor Gray
    $errores++
}

Write-Host ""
if ($errores -eq 0) {
    Write-Host "? Verificación 3: PASADA" -ForegroundColor Green
} else {
    Write-Host "? Verificación 3: FALLIDA" -ForegroundColor Red
}
Write-Host ""

# ========================================
# VERIFICACIÓN 4: NAMESPACES
# ========================================
Write-Host "?? VERIFICACIÓN 4: Namespaces Correctos" -ForegroundColor Cyan
Write-Host ""

# Verificar namespace en Domain
Write-Host "   Verificando namespaces en Domain..." -ForegroundColor Yellow
$sampleDomainFile = "..\SciFiHub.Domain\Entities\Usuario.cs"
if (Test-Path $sampleDomainFile) {
    $content = Get-Content $sampleDomainFile -Raw
    if ($content -match "namespace\s+SciFiHub\.Domain\.Entities") {
        Write-Host "      ? Namespace correcto en Domain" -ForegroundColor Green
    } else {
        Write-Host "      ??  Namespace podría ser incorrecto en Domain" -ForegroundColor Yellow
        $warnings++
    }
}

# Verificar namespace en Infrastructure
Write-Host "   Verificando namespaces en Infrastructure..." -ForegroundColor Yellow
$sampleInfraFile = "..\SciFiHub.Infrastructure\Data\SciFiHubDbContext.cs"
if (Test-Path $sampleInfraFile) {
    $content = Get-Content $sampleInfraFile -Raw
    if ($content -match "namespace\s+SciFiHub\.Infrastructure\.Data") {
        Write-Host "      ? Namespace correcto en Infrastructure" -ForegroundColor Green
    } else {
        Write-Host "      ??  Namespace podría ser incorrecto en Infrastructure" -ForegroundColor Yellow
        $warnings++
    }
}

Write-Host ""
Write-Host "? Verificación 4: COMPLETADA" -ForegroundColor Green
Write-Host ""

# ========================================
# VERIFICACIÓN 5: ESTRUCTURA DE DEPENDENCIAS
# ========================================
Write-Host "?? VERIFICACIÓN 5: Estructura de Dependencias" -ForegroundColor Cyan
Write-Host ""

Write-Host "   Verificando jerarquía de dependencias..." -ForegroundColor Yellow
Write-Host ""
Write-Host "      SciFiHub.Web" -ForegroundColor White
Write-Host "          ? depende de" -ForegroundColor Gray
Write-Host "      SciFiHub.Infrastructure" -ForegroundColor White
Write-Host "          ? depende de" -ForegroundColor Gray
Write-Host "      SciFiHub.Domain" -ForegroundColor White
Write-Host "          ? sin dependencias" -ForegroundColor Gray
Write-Host ""

# Verificar que Domain NO tenga referencias a otros proyectos
$domainCsproj = Get-Content "..\SciFiHub.Domain\SciFiHub.Domain.csproj" -Raw
if ($domainCsproj -match "ProjectReference") {
    Write-Host "      ??  Domain tiene referencias a otros proyectos (debería estar limpio)" -ForegroundColor Yellow
    $warnings++
} else {
    Write-Host "      ? Domain no tiene referencias (correcto)" -ForegroundColor Green
}

Write-Host ""
Write-Host "? Verificación 5: COMPLETADA" -ForegroundColor Green
Write-Host ""

# ========================================
# RESUMEN FINAL
# ========================================
Write-Host "?????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host "?  RESUMEN DE VERIFICACIÓN                                  ?" -ForegroundColor Cyan
Write-Host "?????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""

Write-Host "?? RESULTADOS:" -ForegroundColor White
Write-Host ""
Write-Host "   Errores:   $errores" -ForegroundColor $(if ($errores -eq 0) { "Green" } else { "Red" })
Write-Host "   Warnings:  $warnings" -ForegroundColor $(if ($warnings -eq 0) { "Green" } else { "Yellow" })
Write-Host ""

if ($errores -eq 0 -and $warnings -eq 0) {
    Write-Host "?????????????????????????????????????????????????????????????" -ForegroundColor Green
    Write-Host "?  ? REESTRUCTURACIÓN EXITOSA                              ?" -ForegroundColor Green
    Write-Host "?  ¡La arquitectura en capas está correctamente            ?" -ForegroundColor Green
    Write-Host "?  implementada!                                            ?" -ForegroundColor Green
    Write-Host "?????????????????????????????????????????????????????????????" -ForegroundColor Green
    Write-Host ""
    Write-Host "?? ¡Todo listo! Puedes ejecutar la aplicación." -ForegroundColor Green
} elseif ($errores -eq 0) {
    Write-Host "?????????????????????????????????????????????????????????????" -ForegroundColor Yellow
    Write-Host "?  ??  REESTRUCTURACIÓN COMPLETADA CON WARNINGS             ?" -ForegroundColor Yellow
    Write-Host "?  La arquitectura funciona pero hay algunas advertencias  ?" -ForegroundColor Yellow
    Write-Host "?????????????????????????????????????????????????????????????" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "?? Revisa los warnings arriba para optimizar el proyecto." -ForegroundColor Yellow
} else {
    Write-Host "?????????????????????????????????????????????????????????????" -ForegroundColor Red
    Write-Host "?  ? REESTRUCTURACIÓN INCOMPLETA                           ?" -ForegroundColor Red
    Write-Host "?  Hay errores que deben corregirse                        ?" -ForegroundColor Red
    Write-Host "?????????????????????????????????????????????????????????????" -ForegroundColor Red
    Write-Host ""
    Write-Host "?? Revisa los errores arriba y ejecuta el script de reestructuración nuevamente." -ForegroundColor Red
}

Write-Host ""
Write-Host "?? Para más información, consulta: GUIA_REESTRUCTURACION_COMPLETA.md" -ForegroundColor Cyan
Write-Host ""

# Retornar código de salida
exit $errores
