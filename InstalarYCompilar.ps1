# Script de instalación y compilación automática
# SciFiHub - Correcciones Finales

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "SCIFIHUB - INSTALACIÓN Y COMPILACIÓN" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# 1. Verificar ubicación
$currentPath = Get-Location
Write-Host "?? Directorio actual: $currentPath" -ForegroundColor Yellow

if (!(Test-Path "SciFiHub.Web.csproj")) {
    Write-Host "? ERROR: No se encontró SciFiHub.Web.csproj" -ForegroundColor Red
    Write-Host "Por favor ejecuta este script desde: E:\Proyecto\SciFiHub\SciFiHub" -ForegroundColor Red
    Read-Host "Presiona Enter para salir"
    exit 1
}

Write-Host "? Ubicación correcta" -ForegroundColor Green
Write-Host ""

# 2. Instalar QuestPDF
Write-Host "?? Instalando paquete QuestPDF..." -ForegroundColor Yellow
dotnet add package QuestPDF

if ($LASTEXITCODE -ne 0) {
    Write-Host "? ERROR: No se pudo instalar QuestPDF" -ForegroundColor Red
    Read-Host "Presiona Enter para salir"
    exit 1
}

Write-Host "? QuestPDF instalado correctamente" -ForegroundColor Green
Write-Host ""

# 3. Limpiar proyecto
Write-Host "?? Limpiando proyecto..." -ForegroundColor Yellow
dotnet clean

Write-Host "? Proyecto limpiado" -ForegroundColor Green
Write-Host ""

# 4. Compilar
Write-Host "?? Compilando proyecto..." -ForegroundColor Yellow
dotnet build

if ($LASTEXITCODE -ne 0) {
    Write-Host "? ERROR: La compilación falló" -ForegroundColor Red
    Write-Host ""
    Write-Host "SOLUCIÓN:" -ForegroundColor Yellow
    Write-Host "1. Revisa los errores arriba" -ForegroundColor White
    Write-Host "2. Verifica que todos los archivos se crearon correctamente:" -ForegroundColor White
    Write-Host "   - Services/Interfaces/IPdfService.cs" -ForegroundColor White
    Write-Host "   - Services/PdfService.cs" -ForegroundColor White
    Write-Host "   - wwwroot/js/ventas.js" -ForegroundColor White
    Write-Host ""
    Read-Host "Presiona Enter para salir"
    exit 1
}

Write-Host "? Compilación exitosa" -ForegroundColor Green
Write-Host ""

# 5. Mostrar resumen
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "RESUMEN DE CAMBIOS APLICADOS" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "? Archivos Creados:" -ForegroundColor Green
Write-Host "   - Services/Interfaces/IPdfService.cs" -ForegroundColor White
Write-Host "   - Services/PdfService.cs" -ForegroundColor White
Write-Host "   - wwwroot/js/ventas.js" -ForegroundColor White
Write-Host ""

Write-Host "? Archivos Modificados:" -ForegroundColor Green
Write-Host "   - Program.cs (Registrado IPdfService)" -ForegroundColor White
Write-Host "   - Controllers/Api/VentasController.cs (Endpoints PDF y Cancelar)" -ForegroundColor White
Write-Host "   - Services/VentaService.cs (Mejoras en stock)" -ForegroundColor White
Write-Host ""

Write-Host "? Paquete Instalado:" -ForegroundColor Green
Write-Host "   - QuestPDF (Generación de PDF)" -ForegroundColor White
Write-Host ""

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "PRÓXIMOS PASOS" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "1. Modificar vistas (ver INSTRUCCIONES_EJECUTAR_AHORA.md)" -ForegroundColor Yellow
Write-Host "   - Views/Vendedor/Ventas.cshtml" -ForegroundColor White
Write-Host "   - Views/Vendedor/DetalleVenta.cshtml" -ForegroundColor White
Write-Host "   - Views/Cliente/DetalleCompra.cshtml" -ForegroundColor White
Write-Host ""

Write-Host "2. Ejecutar la aplicación:" -ForegroundColor Yellow
Write-Host "   dotnet run" -ForegroundColor White
Write-Host ""

Write-Host "3. Probar funcionalidades:" -ForegroundColor Yellow
Write-Host "   - Registrar venta (verificar stock)" -ForegroundColor White
Write-Host "   - Cancelar venta (stock se restaura)" -ForegroundColor White
Write-Host "   - Descargar PDF" -ForegroundColor White
Write-Host "   - Agregar al carrito (Cliente)" -ForegroundColor White
Write-Host ""

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "TODO LISTO! ??" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

Read-Host "Presiona Enter para finalizar"
