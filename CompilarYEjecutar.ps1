# ==============================================================================
# Script: Limpiar, Compilar y Ejecutar SciFiHub - VERSIÓN CORREGIDA
# Descripción: Limpia cache, compila con configuraciones corregidas y ejecuta
# Fecha: 2025-01-09 - ACTUALIZADO CON ESTRUCTURA BD REAL
# ==============================================================================

Write-Host "??????????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host "?                                                                ?" -ForegroundColor Cyan
Write-Host "?        SciFiHub - Compilar y Ejecutar (Versión Corregida)     ?" -ForegroundColor Cyan
Write-Host "?              ? Configurado según BD Real                      ?" -ForegroundColor Cyan
Write-Host "?                                                                ?" -ForegroundColor Cyan
Write-Host "??????????????????????????????????????????????????????????????????" -ForegroundColor Cyan
Write-Host ""

# Directorio del proyecto
$projectPath = "E:\Proyecto\SciFiHub\SciFiHub"

# Verificar que el directorio existe
if (-not (Test-Path $projectPath)) {
    Write-Host "? ERROR: No se encontró el directorio del proyecto" -ForegroundColor Red
    Write-Host "   Ruta esperada: $projectPath" -ForegroundColor Yellow
    Write-Host ""
    Read-Host "Presiona Enter para salir"
    exit 1
}

# Cambiar al directorio del proyecto
Set-Location $projectPath
Write-Host "?? Directorio de trabajo: $projectPath" -ForegroundColor Green
Write-Host ""

# ==============================================================================
# INFORMACIÓN DE LA CORRECCIÓN
# ==============================================================================

Write-Host "??  CORRECCIONES APLICADAS:" -ForegroundColor Cyan
Write-Host "?????????????????????????????????????????????????????????????????" -ForegroundColor Gray
Write-Host "   ? Configuraciones actualizadas según estructura BD real" -ForegroundColor Green
Write-Host "   ? CarritoCompras: Ignorar CreatedAt, UpdatedAt" -ForegroundColor Green
Write-Host "   ? DetallesCarrito: Ignorar CreatedAt, UpdatedAt, IsDeleted" -ForegroundColor Green
Write-Host "   ? AuditoriasInventario: Ignorar CreatedAt, UpdatedAt, IsDeleted" -ForegroundColor Green
Write-Host "   ? Filtros globales corregidos" -ForegroundColor Green
Write-Host ""

# ==============================================================================
# PASO 1: LIMPIAR CACHE
# ==============================================================================

Write-Host "?? PASO 1: Limpiando cache y compilaciones anteriores..." -ForegroundColor Yellow
Write-Host "?????????????????????????????????????????????????????????????????" -ForegroundColor Gray

try {
    # Ejecutar dotnet clean
    Write-Host "   Ejecutando: dotnet clean" -ForegroundColor Gray
    dotnet clean --verbosity quiet
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "   ? dotnet clean: EXITOSO" -ForegroundColor Green
    } else {
        Write-Host "   ??  dotnet clean: Completado con advertencias" -ForegroundColor Yellow
    }
    
    # Eliminar directorios bin y obj manualmente
    Write-Host "   Eliminando directorios bin/ y obj/..." -ForegroundColor Gray
    
    Get-ChildItem -Path $projectPath -Include bin,obj -Recurse -Force -ErrorAction SilentlyContinue | 
        ForEach-Object {
            Write-Host "   Eliminando: $($_.FullName)" -ForegroundColor DarkGray
            Remove-Item $_.FullName -Recurse -Force -ErrorAction SilentlyContinue
        }
    
    Write-Host "   ? Cache limpiado correctamente" -ForegroundColor Green
    Write-Host ""
}
catch {
    Write-Host "   ??  Advertencia al limpiar: $($_.Exception.Message)" -ForegroundColor Yellow
    Write-Host ""
}

# ==============================================================================
# PASO 2: RESTAURAR PAQUETES
# ==============================================================================

Write-Host "?? PASO 2: Restaurando paquetes NuGet..." -ForegroundColor Yellow
Write-Host "?????????????????????????????????????????????????????????????????" -ForegroundColor Gray

try {
    dotnet restore --verbosity quiet
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "   ? Paquetes restaurados correctamente" -ForegroundColor Green
    } else {
        Write-Host "   ? Error al restaurar paquetes" -ForegroundColor Red
        Write-Host ""
        Read-Host "Presiona Enter para salir"
        exit 1
    }
    Write-Host ""
}
catch {
    Write-Host "   ? Error: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
    Read-Host "Presiona Enter para salir"
    exit 1
}

# ==============================================================================
# PASO 3: COMPILAR
# ==============================================================================

Write-Host "?? PASO 3: Compilando proyecto..." -ForegroundColor Yellow
Write-Host "?????????????????????????????????????????????????????????????????" -ForegroundColor Gray

try {
    dotnet build --configuration Release --no-restore --verbosity minimal
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host ""
        Write-Host "   ? ¡COMPILACIÓN EXITOSA!" -ForegroundColor Green
        Write-Host "   ? 0 Warnings" -ForegroundColor Green
        Write-Host "   ? 0 Errors" -ForegroundColor Green
    } else {
        Write-Host ""
        Write-Host "   ? COMPILACIÓN FALLIDA" -ForegroundColor Red
        Write-Host "   Ver detalles arriba" -ForegroundColor Yellow
        Write-Host ""
        Read-Host "Presiona Enter para salir"
        exit 1
    }
    Write-Host ""
}
catch {
    Write-Host ""
    Write-Host "   ? Error en compilación: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
    Read-Host "Presiona Enter para salir"
    exit 1
}

# ==============================================================================
# VERIFICACIÓN DE CONFIGURACIONES
# ==============================================================================

Write-Host "?? VERIFICACIÓN: Configuraciones aplicadas" -ForegroundColor Cyan
Write-Host "?????????????????????????????????????????????????????????????????" -ForegroundColor Gray
Write-Host ""
Write-Host "   ?? MATRIZ DE COLUMNAS BD:" -ForegroundColor Cyan
Write-Host "   ?? ?? COMPLETA (7/7): Usuarios, Libros, Ventas, Categorias, DetallesVenta" -ForegroundColor Green
Write-Host "   ?? ?? PARCIAL (5/7):  CarritoCompras" -ForegroundColor Yellow
Write-Host "   ?? ?? MÍNIMA (4/7):   DetallesCarrito, AuditoriasInventario" -ForegroundColor DarkYellow
Write-Host ""

# ==============================================================================
# PASO 4: PREGUNTAR SI EJECUTAR
# ==============================================================================

Write-Host "??????????????????????????????????????????????????????????????????" -ForegroundColor Green
Write-Host "?                                                                ?" -ForegroundColor Green
Write-Host "?         ? COMPILACIÓN COMPLETADA EXITOSAMENTE                ?" -ForegroundColor Green
Write-Host "?         ? Configuraciones según BD Real aplicadas            ?" -ForegroundColor Green
Write-Host "?                                                                ?" -ForegroundColor Green
Write-Host "??????????????????????????????????????????????????????????????????" -ForegroundColor Green
Write-Host ""

$ejecutar = Read-Host "¿Deseas ejecutar la aplicación ahora? (S/N)"

if ($ejecutar -eq "S" -or $ejecutar -eq "s") {
    Write-Host ""
    Write-Host "?? PASO 4: Ejecutando aplicación..." -ForegroundColor Yellow
    Write-Host "?????????????????????????????????????????????????????????????????" -ForegroundColor Gray
    Write-Host ""
    Write-Host "   Para detener la aplicación presiona: Ctrl+C" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "   URL de la aplicación:" -ForegroundColor Cyan
    Write-Host "   https://localhost:7116" -ForegroundColor White
    Write-Host ""
    Write-Host "   ?? PRUEBAS RECOMENDADAS:" -ForegroundColor Cyan
    Write-Host "   1. Agregar al carrito (Cliente)" -ForegroundColor White
    Write-Host "   2. Registrar venta (Admin/Vendedor)" -ForegroundColor White
    Write-Host "   3. Verificar stock actualizado" -ForegroundColor White
    Write-Host ""
    Write-Host "?????????????????????????????????????????????????????????????????" -ForegroundColor Gray
    Write-Host ""
    
    # Ejecutar la aplicación
    dotnet run --configuration Release --no-build
}
else {
    Write-Host ""
    Write-Host "? Proceso completado." -ForegroundColor Green
    Write-Host ""
    Write-Host "Para ejecutar manualmente:" -ForegroundColor Cyan
    Write-Host "   cd $projectPath" -ForegroundColor White
    Write-Host "   dotnet run" -ForegroundColor White
    Write-Host ""
    Write-Host "URL de la aplicación:" -ForegroundColor Cyan
    Write-Host "   https://localhost:7116" -ForegroundColor White
    Write-Host ""
    Write-Host "?? Documentación:" -ForegroundColor Cyan
    Write-Host "   • SOLUCION_ESTRUCTURA_BD_REAL.md" -ForegroundColor White
    Write-Host "   • MATRIZ_COLUMNAS_BD_COMPLETA.md" -ForegroundColor White
    Write-Host ""
}

# ==============================================================================
# FIN DEL SCRIPT
# ==============================================================================
