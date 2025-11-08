# ========================================
# SCRIPT DE REESTRUCTURACIÓN DEL PROYECTO
# SciFiHub - Migración a Arquitectura en Capas
# ========================================

Write-Host "?????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host "?  REESTRUCTURACIÓN DE PROYECTO SCIFIHUB                    ?" -ForegroundColor Cyan
Write-Host "?  Migración de Monolítico a Arquitectura en Capas         ?" -ForegroundColor Cyan
Write-Host "?????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""

# Verificar que estamos en el directorio correcto
$currentPath = Get-Location
Write-Host "?? Directorio actual: $currentPath" -ForegroundColor Yellow
Write-Host ""

# Confirmar antes de proceder
Write-Host "??  ADVERTENCIA: Este script va a mover archivos entre proyectos." -ForegroundColor Yellow
Write-Host "   Se recomienda tener un respaldo o commit antes de continuar." -ForegroundColor Yellow
Write-Host ""
$confirmation = Read-Host "¿Deseas continuar? (S/N)"
if ($confirmation -ne 'S' -and $confirmation -ne 's') {
    Write-Host "? Operación cancelada por el usuario." -ForegroundColor Red
    exit
}

Write-Host ""
Write-Host "?? Iniciando reestructuración..." -ForegroundColor Green
Write-Host ""

# ========================================
# FASE 1: MOVER ARCHIVOS DE DOMAIN
# ========================================
Write-Host "?? FASE 1: Moviendo archivos de SciFiHub.Domain..." -ForegroundColor Cyan
Write-Host ""

$domainFiles = @{
    # Entities
    "SciFiHub.Domain\Entities\AuditoriaInventario.cs" = "..\SciFiHub.Domain\Entities\AuditoriaInventario.cs"
    "SciFiHub.Domain\Entities\CarritoCompra.cs" = "..\SciFiHub.Domain\Entities\CarritoCompra.cs"
    "SciFiHub.Domain\Entities\Categoria.cs" = "..\SciFiHub.Domain\Entities\Categoria.cs"
    "SciFiHub.Domain\Entities\DetalleCarrito.cs" = "..\SciFiHub.Domain\Entities\DetalleCarrito.cs"
    "SciFiHub.Domain\Entities\DetalleVenta.cs" = "..\SciFiHub.Domain\Entities\DetalleVenta.cs"
    "SciFiHub.Domain\Entities\Libro.cs" = "..\SciFiHub.Domain\Entities\Libro.cs"
    "SciFiHub.Domain\Entities\Usuario.cs" = "..\SciFiHub.Domain\Entities\Usuario.cs"
    "SciFiHub.Domain\Entities\Venta.cs" = "..\SciFiHub.Domain\Entities\Venta.cs"
    
    # Common
    "SciFiHub.Domain\Common\BaseEntity.cs" = "..\SciFiHub.Domain\Common\BaseEntity.cs"
    
    # Enums
    "SciFiHub.Domain\Enums\Enumerations.cs" = "..\SciFiHub.Domain\Enums\Enumerations.cs"
    
    # ValueObjects
    "SciFiHub.Domain\ValueObjects\DireccionEnvio.cs" = "..\SciFiHub.Domain\ValueObjects\DireccionEnvio.cs"
    
    # Interfaces
    "SciFiHub.Domain\Interfaces\IRepository.cs" = "..\SciFiHub.Domain\Interfaces\IRepository.cs"
    "SciFiHub.Domain\Interfaces\IUnitOfWork.cs" = "..\SciFiHub.Domain\Interfaces\IUnitOfWork.cs"
    "SciFiHub.Domain\Interfaces\ICarritoCompraRepository.cs" = "..\SciFiHub.Domain\Interfaces\ICarritoCompraRepository.cs"
    "SciFiHub.Domain\Interfaces\ICategoriaRepository.cs" = "..\SciFiHub.Domain\Interfaces\ICategoriaRepository.cs"
    "SciFiHub.Domain\Interfaces\ILibroRepository.cs" = "..\SciFiHub.Domain\Interfaces\ILibroRepository.cs"
    "SciFiHub.Domain\Interfaces\IUsuarioRepository.cs" = "..\SciFiHub.Domain\Interfaces\IUsuarioRepository.cs"
    "SciFiHub.Domain\Interfaces\IVentaRepository.cs" = "..\SciFiHub.Domain\Interfaces\IVentaRepository.cs"
}

foreach ($source in $domainFiles.Keys) {
    $destination = $domainFiles[$source]
    
    if (Test-Path $source) {
        # Crear directorio de destino si no existe
        $destDir = Split-Path $destination -Parent
        if (!(Test-Path $destDir)) {
            New-Item -ItemType Directory -Path $destDir -Force | Out-Null
            Write-Host "   ?? Creado directorio: $destDir" -ForegroundColor Gray
        }
        
        # Mover archivo
        Move-Item -Path $source -Destination $destination -Force
        Write-Host "   ? Movido: $source" -ForegroundColor Green
        Write-Host "      ? $destination" -ForegroundColor Gray
    } else {
        Write-Host "   ??  No encontrado: $source" -ForegroundColor Yellow
    }
}

Write-Host ""
Write-Host "? Fase 1 completada - Domain files moved" -ForegroundColor Green
Write-Host ""

# ========================================
# FASE 2: MOVER ARCHIVOS DE INFRASTRUCTURE
# ========================================
Write-Host "?? FASE 2: Moviendo archivos de SciFiHub.Infrastructure..." -ForegroundColor Cyan
Write-Host ""

$infrastructureFiles = @{
    # Data Configurations
    "SciFiHub.Infrastructure\Data\Configurations\AuditoriaInventarioConfiguration.cs" = "..\SciFiHub.Infrastructure\Data\Configurations\AuditoriaInventarioConfiguration.cs"
    "SciFiHub.Infrastructure\Data\Configurations\CarritoCompraConfiguration.cs" = "..\SciFiHub.Infrastructure\Data\Configurations\CarritoCompraConfiguration.cs"
    "SciFiHub.Infrastructure\Data\Configurations\CategoriaConfiguration.cs" = "..\SciFiHub.Infrastructure\Data\Configurations\CategoriaConfiguration.cs"
    "SciFiHub.Infrastructure\Data\Configurations\DetalleCarritoConfiguration.cs" = "..\SciFiHub.Infrastructure\Data\Configurations\DetalleCarritoConfiguration.cs"
    "SciFiHub.Infrastructure\Data\Configurations\DetalleVentaConfiguration.cs" = "..\SciFiHub.Infrastructure\Data\Configurations\DetalleVentaConfiguration.cs"
    "SciFiHub.Infrastructure\Data\Configurations\LibroConfiguration.cs" = "..\SciFiHub.Infrastructure\Data\Configurations\LibroConfiguration.cs"
    "SciFiHub.Infrastructure\Data\Configurations\UsuarioConfiguration.cs" = "..\SciFiHub.Infrastructure\Data\Configurations\UsuarioConfiguration.cs"
    "SciFiHub.Infrastructure\Data\Configurations\VentaConfiguration.cs" = "..\SciFiHub.Infrastructure\Data\Configurations\VentaConfiguration.cs"
    
    # DbContext
    "SciFiHub.Infrastructure\Data\SciFiHubDbContext.cs" = "..\SciFiHub.Infrastructure\Data\SciFiHubDbContext.cs"
    
    # Repositories
    "SciFiHub.Infrastructure\Repositories\Repository.cs" = "..\SciFiHub.Infrastructure\Repositories\Repository.cs"
    "SciFiHub.Infrastructure\Repositories\UnitOfWork.cs" = "..\SciFiHub.Infrastructure\Repositories\UnitOfWork.cs"
    "SciFiHub.Infrastructure\Repositories\CarritoCompraRepository.cs" = "..\SciFiHub.Infrastructure\Repositories\CarritoCompraRepository.cs"
    "SciFiHub.Infrastructure\Repositories\CategoriaRepository.cs" = "..\SciFiHub.Infrastructure\Repositories\CategoriaRepository.cs"
    "SciFiHub.Infrastructure\Repositories\LibroRepository.cs" = "..\SciFiHub.Infrastructure\Repositories\LibroRepository.cs"
    "SciFiHub.Infrastructure\Repositories\UsuarioRepository.cs" = "..\SciFiHub.Infrastructure\Repositories\UsuarioRepository.cs"
    "SciFiHub.Infrastructure\Repositories\VentaRepository.cs" = "..\SciFiHub.Infrastructure\Repositories\VentaRepository.cs"
}

foreach ($source in $infrastructureFiles.Keys) {
    $destination = $infrastructureFiles[$source]
    
    if (Test-Path $source) {
        # Crear directorio de destino si no existe
        $destDir = Split-Path $destination -Parent
        if (!(Test-Path $destDir)) {
            New-Item -ItemType Directory -Path $destDir -Force | Out-Null
            Write-Host "   ?? Creado directorio: $destDir" -ForegroundColor Gray
        }
        
        # Mover archivo
        Move-Item -Path $source -Destination $destination -Force
        Write-Host "   ? Movido: $source" -ForegroundColor Green
        Write-Host "      ? $destination" -ForegroundColor Gray
    } else {
        Write-Host "   ??  No encontrado: $source" -ForegroundColor Yellow
    }
}

Write-Host ""
Write-Host "? Fase 2 completada - Infrastructure files moved" -ForegroundColor Green
Write-Host ""

# ========================================
# FASE 3: LIMPIAR CARPETAS VACÍAS
# ========================================
Write-Host "?? FASE 3: Limpiando carpetas vacías..." -ForegroundColor Cyan
Write-Host ""

$foldersToRemove = @(
    "SciFiHub.Domain\Entities",
    "SciFiHub.Domain\Common",
    "SciFiHub.Domain\Enums",
    "SciFiHub.Domain\ValueObjects",
    "SciFiHub.Domain\Interfaces",
    "SciFiHub.Domain",
    "SciFiHub.Infrastructure\Data\Configurations",
    "SciFiHub.Infrastructure\Data",
    "SciFiHub.Infrastructure\Repositories",
    "SciFiHub.Infrastructure"
)

foreach ($folder in $foldersToRemove) {
    if (Test-Path $folder) {
        $items = Get-ChildItem -Path $folder -Recurse
        if ($items.Count -eq 0) {
            Remove-Item -Path $folder -Force -Recurse
            Write-Host "   ???  Eliminada carpeta vacía: $folder" -ForegroundColor Gray
        } else {
            Write-Host "   ??  Carpeta no vacía (conservada): $folder" -ForegroundColor Yellow
        }
    }
}

Write-Host ""
Write-Host "? Fase 3 completada - Cleanup done" -ForegroundColor Green
Write-Host ""

# ========================================
# FASE 4: ACTUALIZAR REFERENCIAS DE PROYECTOS
# ========================================
Write-Host "?? FASE 4: Actualizando referencias de proyectos..." -ForegroundColor Cyan
Write-Host ""

# Actualizar SciFiHub.Web.csproj
Write-Host "   ?? Actualizando SciFiHub.Web.csproj..." -ForegroundColor Yellow

$webCsprojContent = @"
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="AutoMapper.Extensions.Microsoft.DependencyInjection" Version="12.0.1" />
    <PackageReference Include="FluentValidation.AspNetCore" Version="11.3.1" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="9.0.10">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
    <PackageReference Include="QuestPDF" Version="2025.7.4" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\SciFiHub.Infrastructure\SciFiHub.Infrastructure.csproj" />
  </ItemGroup>

</Project>
"@

Set-Content -Path "SciFiHub.Web.csproj" -Value $webCsprojContent -Force
Write-Host "   ? SciFiHub.Web.csproj actualizado" -ForegroundColor Green

Write-Host ""
Write-Host "? Fase 4 completada - Project references updated" -ForegroundColor Green
Write-Host ""

# ========================================
# FASE 5: LIMPIAR SOLUCIÓN
# ========================================
Write-Host "?? FASE 5: Limpiando solución..." -ForegroundColor Cyan
Write-Host ""

# Limpiar bin y obj del proyecto Web
Write-Host "   ???  Limpiando bin y obj..." -ForegroundColor Yellow
if (Test-Path "bin") { Remove-Item -Path "bin" -Recurse -Force }
if (Test-Path "obj") { Remove-Item -Path "obj" -Recurse -Force }
Write-Host "   ? Archivos temporales eliminados" -ForegroundColor Green

Write-Host ""
Write-Host "? Fase 5 completada - Solution cleaned" -ForegroundColor Green
Write-Host ""

# ========================================
# RESUMEN FINAL
# ========================================
Write-Host "?????????????????????????????????????????????????????????????" -ForegroundColor Green
Write-Host "?  ? REESTRUCTURACIÓN COMPLETADA EXITOSAMENTE              ?" -ForegroundColor Green
Write-Host "?????????????????????????????????????????????????????????????" -ForegroundColor Green
Write-Host ""
Write-Host "?? RESUMEN DE CAMBIOS:" -ForegroundColor Cyan
Write-Host ""
Write-Host "   ? Archivos Domain movidos a: ..\SciFiHub.Domain\" -ForegroundColor Green
Write-Host "   ? Archivos Infrastructure movidos a: ..\SciFiHub.Infrastructure\" -ForegroundColor Green
Write-Host "   ? Referencias de proyectos actualizadas" -ForegroundColor Green
Write-Host "   ? Carpetas vacías eliminadas" -ForegroundColor Green
Write-Host "   ? Solución limpiada" -ForegroundColor Green
Write-Host ""
Write-Host "??  PRÓXIMOS PASOS:" -ForegroundColor Yellow
Write-Host ""
Write-Host "   1??  Ejecutar: dotnet restore" -ForegroundColor White
Write-Host "   2??  Ejecutar: dotnet build" -ForegroundColor White
Write-Host "   3??  Verificar que no haya errores de compilación" -ForegroundColor White
Write-Host "   4??  Ejecutar las pruebas de la aplicación" -ForegroundColor White
Write-Host ""
Write-Host "?? ARQUITECTURA RESULTANTE:" -ForegroundColor Cyan
Write-Host ""
Write-Host "   ?? SciFiHub.Domain (Capa de Dominio)" -ForegroundColor Magenta
Write-Host "      ?? Entities, Enums, Interfaces, ValueObjects" -ForegroundColor Gray
Write-Host ""
Write-Host "   ?? SciFiHub.Infrastructure (Capa de Infraestructura)" -ForegroundColor Magenta
Write-Host "      ?? DbContext, Repositories, Configurations" -ForegroundColor Gray
Write-Host ""
Write-Host "   ?? SciFiHub.Web (Capa de Presentación)" -ForegroundColor Magenta
Write-Host "      ?? Controllers, Views, Services, DTOs" -ForegroundColor Gray
Write-Host ""
Write-Host "?? TIP: Si encuentras errores de compilación, ejecuta 'dotnet clean' seguido de 'dotnet build'" -ForegroundColor Yellow
Write-Host ""

# Preguntar si desea compilar ahora
$compile = Read-Host "¿Deseas compilar el proyecto ahora? (S/N)"
if ($compile -eq 'S' -or $compile -eq 's') {
    Write-Host ""
    Write-Host "?? Compilando proyecto..." -ForegroundColor Cyan
    Write-Host ""
    
    dotnet restore
    Write-Host ""
    dotnet build
    
    Write-Host ""
    Write-Host "? Compilación completada. Revisa los resultados arriba." -ForegroundColor Green
}

Write-Host ""
Write-Host "?? ¡Script completado! La arquitectura ha sido reestructurada correctamente." -ForegroundColor Green
Write-Host ""
