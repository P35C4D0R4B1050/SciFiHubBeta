# =============================================
# Script: Limpiar y Reconstruir Proyecto
# Descripción: Limpia cache de EF Core y recompila
# =============================================

Write-Host "=============================================="
Write-Host "LIMPIEZA COMPLETA DEL PROYECTO"
Write-Host "==============================================`n"

# Navegar al directorio del proyecto
$projectPath = "E:\Proyecto\SciFiHub\SciFiHub"
Set-Location $projectPath

Write-Host "1. Deteniendo aplicación si está corriendo..."
Get-Process -Name "dotnet" -ErrorAction SilentlyContinue | Stop-Process -Force
Write-Host "? Procesos detenidos`n"

Write-Host "2. Eliminando carpetas bin/ y obj/..."
Get-ChildItem -Path . -Recurse -Directory -Filter "bin" | Remove-Item -Recurse -Force -ErrorAction SilentlyContinue
Get-ChildItem -Path . -Recurse -Directory -Filter "obj" | Remove-Item -Recurse -Force -ErrorAction SilentlyContinue
Write-Host "? Carpetas eliminadas`n"

Write-Host "3. Ejecutando dotnet clean..."
dotnet clean --nologo
Write-Host "? Clean completado`n"

Write-Host "4. Restaurando paquetes..."
dotnet restore --nologo
Write-Host "? Restore completado`n"

Write-Host "5. Compilando proyecto..."
dotnet build --nologo
Write-Host "? Build completado`n"

Write-Host "=============================================="
Write-Host "LIMPIEZA COMPLETADA EXITOSAMENTE"
Write-Host "==============================================`n"

Write-Host "SIGUIENTE PASO:"
Write-Host "Ejecutar: dotnet run`n"

# Pausar para que el usuario vea el resultado
Read-Host "Presione Enter para continuar..."
