Write-Host "============================================" -ForegroundColor Cyan
Write-Host "INSTALANDO QUESTPDF Y COMPILANDO PROYECTO" -ForegroundColor Cyan
Write-Host "============================================" -ForegroundColor Cyan
Write-Host ""

# Verificar ubicación
if (!(Test-Path "SciFiHub.Web.csproj")) {
    Write-Host "? ERROR: Ejecuta este script desde la carpeta raíz del proyecto" -ForegroundColor Red
    Write-Host "   Carpeta actual: $PWD" -ForegroundColor Yellow
    Write-Host "   Carpeta correcta: E:\Proyecto\SciFiHub\SciFiHub" -ForegroundColor Yellow
    Read-Host "Presiona Enter para salir"
    exit 1
}

Write-Host "? Ubicación correcta" -ForegroundColor Green
Write-Host ""

# Instalar QuestPDF
Write-Host "?? Instalando QuestPDF..." -ForegroundColor Yellow
dotnet add package QuestPDF

if ($LASTEXITCODE -ne 0) {
    Write-Host "? ERROR al instalar QuestPDF" -ForegroundColor Red
    Read-Host "Presiona Enter para salir"
    exit 1
}

Write-Host "? QuestPDF instalado" -ForegroundColor Green
Write-Host ""

# Limpiar
Write-Host "?? Limpiando..." -ForegroundColor Yellow
dotnet clean | Out-Null

# Compilar
Write-Host "?? Compilando..." -ForegroundColor Yellow
dotnet build

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "============================================" -ForegroundColor Green
    Write-Host "? COMPILACIÓN EXITOSA" -ForegroundColor Green
    Write-Host "============================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "Próximo paso: dotnet run" -ForegroundColor Yellow
} else {
    Write-Host ""
    Write-Host "? ERROR EN LA COMPILACIÓN" -ForegroundColor Red
    Write-Host "Revisa los errores arriba" -ForegroundColor Yellow
}

Write-Host ""
Read-Host "Presiona Enter para salir"
