# ? GUÍA RÁPIDA DE COMANDOS - REESTRUCTURACIÓN

---

## ?? UN SOLO COMANDO (RECOMENDADO)

```powershell
cd E:\Proyecto\SciFiHub\SciFiHub\
.\ReestructurarTodoEnUno.ps1
```

? **Lo hace todo:** Mueve archivos, actualiza referencias, compila y verifica

---

## ?? EJECUCIÓN PASO A PASO

### 1. Preparación
```powershell
# Hacer backup
git add .
git commit -m "Backup antes de reestructuración"

# Navegar al directorio
cd E:\Proyecto\SciFiHub\SciFiHub\

# Cerrar Visual Studio
```

### 2. Reestructurar
```powershell
# Ejecutar script principal
.\ReestructurarProyecto.ps1
```

### 3. Compilar
```powershell
# Restaurar paquetes
dotnet restore

# Compilar proyecto
dotnet build
```

### 4. Verificar
```powershell
# Ejecutar verificación
.\VerificarReestructuracion.ps1
```

---

## ?? VERIFICACIÓN MANUAL

```powershell
# Ver estructura de Domain
ls ..\SciFiHub.Domain\

# Ver estructura de Infrastructure
ls ..\SciFiHub.Infrastructure\

# Verificar que carpetas viejas no existen
Test-Path SciFiHub.Domain  # Debe ser False
Test-Path SciFiHub.Infrastructure  # Debe ser False
```

---

## ?? COMANDOS DE COMPILACIÓN

```powershell
# Limpiar solución
dotnet clean

# Restaurar paquetes
dotnet restore

# Compilar
dotnet build

# Compilar con información detallada
dotnet build -v detailed

# Compilar sin caché
dotnet build --no-incremental
```

---

## ?? COMANDOS DE PRUEBA

```powershell
# Ejecutar aplicación
dotnet run

# Ejecutar en modo watch (recarga automática)
dotnet watch run

# Ejecutar con configuración específica
dotnet run --configuration Release
```

---

## ?? COMANDOS DE DIAGNÓSTICO

```powershell
# Ver proyectos en la solución
dotnet sln list

# Ver referencias de un proyecto
dotnet list ..\SciFiHub.Domain\SciFiHub.Domain.csproj reference
dotnet list ..\SciFiHub.Infrastructure\SciFiHub.Infrastructure.csproj reference
dotnet list SciFiHub.Web.csproj reference

# Ver paquetes instalados
dotnet list package

# Ver información del proyecto
dotnet --info
```

---

## ??? COMANDOS DE LIMPIEZA

```powershell
# Limpiar proyecto actual
dotnet clean

# Eliminar bin y obj
Remove-Item bin, obj -Recurse -Force -ErrorAction SilentlyContinue

# Limpiar todos los proyectos
cd E:\Proyecto\SciFiHub\
Get-ChildItem -Path . -Include bin,obj -Recurse | Remove-Item -Recurse -Force

# Limpiar caché de NuGet (si hay problemas)
dotnet nuget locals all --clear
```

---

## ?? COMANDOS DE REVERTIR

```powershell
# Revertir cambios con Git
git reset --hard HEAD

# Deshacer último commit (mantener cambios)
git reset --soft HEAD~1

# Deshacer último commit (descartar cambios)
git reset --hard HEAD~1

# Ver estado de Git
git status
git log --oneline
```

---

## ?? COMANDOS DE GESTIÓN DE PAQUETES

```powershell
# Restaurar paquetes
dotnet restore

# Agregar paquete
dotnet add package <PackageName>

# Actualizar paquete
dotnet add package <PackageName> --version <Version>

# Eliminar paquete
dotnet remove package <PackageName>

# Ver paquetes desactualizados
dotnet list package --outdated
```

---

## ?? COMANDOS DE REFERENCIAS

```powershell
# Agregar referencia de proyecto
dotnet add reference <ProjectPath>

# Ejemplo: Web referencia Infrastructure
cd E:\Proyecto\SciFiHub\SciFiHub\
dotnet add reference ..\SciFiHub.Infrastructure\SciFiHub.Infrastructure.csproj

# Ejemplo: Infrastructure referencia Domain
cd ..\SciFiHub.Infrastructure\
dotnet add reference ..\SciFiHub.Domain\SciFiHub.Domain.csproj

# Ver referencias
dotnet list reference
```

---

## ?? COMANDOS DE SOLUCIÓN

```powershell
# Crear nueva solución
dotnet new sln -n SciFiHub

# Agregar proyectos a la solución
dotnet sln add SciFiHub.Domain\SciFiHub.Domain.csproj
dotnet sln add SciFiHub.Infrastructure\SciFiHub.Infrastructure.csproj
dotnet sln add SciFiHub.Web\SciFiHub.Web.csproj

# Compilar toda la solución
dotnet build SciFiHub.sln

# Limpiar toda la solución
dotnet clean SciFiHub.sln
```

---

## ?? COMANDOS DE VISUAL STUDIO

```powershell
# Abrir solución en Visual Studio
start SciFiHub.sln

# Abrir en VS Code
code .

# Compilar desde línea de comandos (si MSBuild está en PATH)
msbuild SciFiHub.sln /t:Build /p:Configuration=Release
```

---

## ?? COMANDOS DE BÚSQUEDA

```powershell
# Buscar archivos por nombre
Get-ChildItem -Path . -Recurse -Filter "*.cs"

# Buscar texto en archivos
Get-ChildItem -Path . -Recurse -Filter "*.cs" | Select-String "SciFiHub.Domain"

# Contar archivos por tipo
Get-ChildItem -Path . -Recurse | Group-Object Extension | Sort-Object Count -Descending
```

---

## ?? COMANDOS DE EMERGENCIA

```powershell
# Si todo falla: Limpiar y empezar de nuevo
dotnet clean
Remove-Item bin, obj -Recurse -Force -ErrorAction SilentlyContinue
dotnet nuget locals all --clear
dotnet restore
dotnet build

# Si Git está desincronizado
git fetch origin
git reset --hard origin/main

# Si Visual Studio no responde
taskkill /F /IM devenv.exe
```

---

## ? COMANDOS DE VALIDACIÓN

```powershell
# Verificar que proyectos existen
Test-Path ..\SciFiHub.Domain\SciFiHub.Domain.csproj
Test-Path ..\SciFiHub.Infrastructure\SciFiHub.Infrastructure.csproj
Test-Path SciFiHub.Web.csproj

# Verificar que archivos fueron movidos
Test-Path ..\SciFiHub.Domain\Entities\Usuario.cs
Test-Path ..\SciFiHub.Infrastructure\Data\SciFiHubDbContext.cs

# Verificar que carpetas viejas no existen
!(Test-Path SciFiHub.Domain)
!(Test-Path SciFiHub.Infrastructure)
```

---

## ?? COMANDOS DE ESTADÍSTICAS

```powershell
# Contar líneas de código
Get-ChildItem -Path . -Recurse -Filter "*.cs" | 
    Get-Content | 
    Measure-Object -Line

# Contar archivos por proyecto
Write-Host "Domain:"
(Get-ChildItem -Path ..\SciFiHub.Domain\ -Recurse -Filter "*.cs").Count
Write-Host "Infrastructure:"
(Get-ChildItem -Path ..\SciFiHub.Infrastructure\ -Recurse -Filter "*.cs").Count
Write-Host "Web:"
(Get-ChildItem -Path . -Recurse -Filter "*.cs").Count

# Ver tamaño de proyectos
Get-ChildItem -Path . -Recurse | 
    Measure-Object -Property Length -Sum | 
    Select-Object @{Name="TotalMB";Expression={[math]::Round($_.Sum / 1MB, 2)}}
```

---

## ?? COMANDOS DE CONFIGURACIÓN

```powershell
# Ver configuración de .NET
dotnet --info

# Ver SDKs instalados
dotnet --list-sdks

# Ver runtimes instalados
dotnet --list-runtimes

# Ver variables de entorno
Get-ChildItem Env: | Where-Object Name -like "*dotnet*"
```

---

## ?? COMANDOS DE RENDIMIENTO

```powershell
# Compilar con información de tiempo
dotnet build /p:ShowTimings=True

# Compilar en paralelo
dotnet build -m

# Compilar con caché deshabilitado
dotnet build --no-incremental

# Medir tiempo de build
Measure-Command { dotnet build }
```

---

## ?? ATAJOS DE TECLADO (PowerShell)

```powershell
# Historial de comandos
Get-History

# Buscar en historial
Ctrl + R

# Limpiar pantalla
cls  # o Clear-Host

# Completar automáticamente
Tab

# Ver ayuda de comando
Get-Help <comando> -Full
```

---

## ?? COMANDOS DE DOCUMENTACIÓN

```powershell
# Ver archivos de documentación
ls *.md

# Abrir documentación en navegador
start README_REESTRUCTURACION.md

# Ver contenido de documentación
Get-Content GUIA_REESTRUCTURACION_COMPLETA.md
```

---

## ?? FLUJO COMPLETO EN UN SOLO BLOQUE

```powershell
# 1. Preparación
cd E:\Proyecto\SciFiHub\SciFiHub\
git add .
git commit -m "Backup antes de reestructuración"

# 2. Reestructuración automática
.\ReestructurarTodoEnUno.ps1

# 3. Verificar resultado
dotnet build
.\VerificarReestructuracion.ps1

# 4. Ejecutar aplicación
dotnet run

# 5. Commit final
git add .
git commit -m "Reestructuración a arquitectura en capas completada"
```

---

## ?? CHECKLIST DE COMANDOS ESENCIALES

```powershell
# ? Estos son los únicos comandos que realmente necesitas:

# 1. Navegar
cd E:\Proyecto\SciFiHub\SciFiHub\

# 2. Ejecutar reestructuración
.\ReestructurarTodoEnUno.ps1

# 3. Compilar (si no lo hizo el script)
dotnet build

# 4. Verificar (opcional pero recomendado)
.\VerificarReestructuracion.ps1

# 5. Ejecutar (para probar)
dotnet run

# ¡Listo! ??
```

---

## ?? TIPS DE POWERSHELL

```powershell
# Alias útiles
Set-Alias -Name build -Value "dotnet build"
Set-Alias -Name run -Value "dotnet run"
Set-Alias -Name clean -Value "dotnet clean"

# Usar después:
build    # en lugar de: dotnet build
run      # en lugar de: dotnet run
clean    # en lugar de: dotnet clean

# Crear función personalizada
function reb { dotnet clean; dotnet build }

# Usar:
reb      # limpia y compila
```

---

## ?? COMANDOS ÚTILES DE GIT

```powershell
# Ver cambios
git status
git diff

# Ver historial
git log --oneline --graph --decorate --all

# Crear rama para reestructuración
git checkout -b feature/arquitectura-capas

# Hacer commit de cambios
git add .
git commit -m "Reestructuración a arquitectura en capas"

# Mergear a main
git checkout main
git merge feature/arquitectura-capas
```

---

## ?? COMANDOS DE AYUDA

```powershell
# Ver ayuda de dotnet
dotnet --help

# Ver ayuda de comando específico
dotnet build --help
dotnet run --help
dotnet clean --help

# Ver ayuda de PowerShell
Get-Help Get-ChildItem
Get-Help Remove-Item -Examples
```

---

## ? RESULTADO ESPERADO

Después de ejecutar todos los comandos correctamente:

```
? Archivos movidos
? Referencias actualizadas
? Compilación exitosa
? Verificación pasada
? Aplicación funcional
```

---

**?? Comando rápido para ejecutar TODO:**

```powershell
cd E:\Proyecto\SciFiHub\SciFiHub\ ; .\ReestructurarTodoEnUno.ps1
```

---

*Guía Rápida de Comandos - Reestructuración SciFiHub v1.0*
