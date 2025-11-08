# ?? GUÍA DE REESTRUCTURACIÓN DEL PROYECTO SCIFIHUB
## Migración de Arquitectura Monolítica a Arquitectura en Capas (MVC)

---

## ?? OBJETIVO

Transformar la estructura actual del proyecto de **monolítica** a una verdadera **arquitectura en capas (MVC)** distribuyendo correctamente los componentes en sus respectivos proyectos.

---

## ?? ESTADO ACTUAL vs ESTADO OBJETIVO

### ? ESTADO ACTUAL (Monolítico)
```
SciFiHub\
??? SciFiHub.Web.csproj
??? SciFiHub.Domain\          ?? CARPETA (debería ser proyecto)
?   ??? Entities\
?   ??? Interfaces\
?   ??? Enums\
?   ??? ValueObjects\
??? SciFiHub.Infrastructure\  ?? CARPETA (debería ser proyecto)
?   ??? Data\
?   ??? Repositories\
??? Controllers\
??? Views\
??? Services\
??? ...

SciFiHub.Domain\              ? PROYECTO (pero vacío)
??? SciFiHub.Domain.csproj

SciFiHub.Infrastructure\      ? PROYECTO (pero vacío)
??? SciFiHub.Infrastructure.csproj
```

### ? ESTADO OBJETIVO (Arquitectura en Capas)
```
SciFiHub.Domain\              ? PROYECTO CON CÓDIGO
??? SciFiHub.Domain.csproj
??? Entities\
??? Interfaces\
??? Enums\
??? ValueObjects\
??? Common\

SciFiHub.Infrastructure\      ? PROYECTO CON CÓDIGO
??? SciFiHub.Infrastructure.csproj
??? Data\
?   ??? SciFiHubDbContext.cs
?   ??? Configurations\
??? Repositories\

SciFiHub.Web\                 ? PROYECTO WEB
??? SciFiHub.Web.csproj
??? Controllers\
??? Views\
??? Services\
??? DTOs\
??? Validators\
??? ...
```

---

## ?? PASOS DE EJECUCIÓN

### 1?? PREPARACIÓN

**a) Hacer backup del proyecto:**
```powershell
# Crear una copia de seguridad
git add .
git commit -m "Backup antes de reestructuración"
```

**b) Cerrar Visual Studio** (importante para evitar conflictos)

### 2?? EJECUTAR EL SCRIPT DE REESTRUCTURACIÓN

```powershell
# Navegar al directorio del proyecto Web
cd E:\Proyecto\SciFiHub\SciFiHub\

# Ejecutar el script
.\ReestructurarProyecto.ps1
```

El script realizará automáticamente:
- ? Mover archivos de Domain a `../SciFiHub.Domain/`
- ? Mover archivos de Infrastructure a `../SciFiHub.Infrastructure/`
- ? Actualizar referencias de proyectos
- ? Limpiar carpetas vacías
- ? Limpiar archivos temporales

### 3?? RESTAURAR Y COMPILAR

```powershell
# Restaurar paquetes NuGet
dotnet restore

# Compilar la solución
dotnet build
```

### 4?? VERIFICAR EN VISUAL STUDIO

1. Abrir Visual Studio
2. Abrir la solución `SciFiHub.sln`
3. Verificar que los proyectos tengan archivos:
   - **SciFiHub.Domain** ? debe contener Entities, Interfaces, etc.
   - **SciFiHub.Infrastructure** ? debe contener Data, Repositories, etc.
   - **SciFiHub.Web** ? debe contener Controllers, Views, Services, etc.

---

## ?? ARQUITECTURA DE REFERENCIA DEL PROYECTO

### ? SciFiHub.Domain (Capa de Dominio)
**Propósito:** Contiene las entidades del negocio y las interfaces

```
SciFiHub.Domain\
??? Common\
?   ??? BaseEntity.cs
??? Entities\
?   ??? AuditoriaInventario.cs
?   ??? CarritoCompra.cs
?   ??? Categoria.cs
?   ??? DetalleCarrito.cs
?   ??? DetalleVenta.cs
?   ??? Libro.cs
?   ??? Usuario.cs
?   ??? Venta.cs
??? Enums\
?   ??? Enumerations.cs
??? Interfaces\
?   ??? ICarritoCompraRepository.cs
?   ??? ICategoriaRepository.cs
?   ??? ILibroRepository.cs
?   ??? IRepository.cs
?   ??? IUnitOfWork.cs
?   ??? IUsuarioRepository.cs
?   ??? IVentaRepository.cs
??? ValueObjects\
    ??? DireccionEnvio.cs
```

**Dependencias:** Ninguna (es la capa más interna)

---

### ? SciFiHub.Infrastructure (Capa de Infraestructura)
**Propósito:** Implementa acceso a datos y repositorios

```
SciFiHub.Infrastructure\
??? Data\
?   ??? SciFiHubDbContext.cs
?   ??? Configurations\
?       ??? AuditoriaInventarioConfiguration.cs
?       ??? CarritoCompraConfiguration.cs
?       ??? CategoriaConfiguration.cs
?       ??? DetalleCarritoConfiguration.cs
?       ??? DetalleVentaConfiguration.cs
?       ??? LibroConfiguration.cs
?       ??? UsuarioConfiguration.cs
?       ??? VentaConfiguration.cs
??? Repositories\
    ??? CarritoCompraRepository.cs
    ??? CategoriaRepository.cs
    ??? LibroRepository.cs
    ??? Repository.cs
    ??? UnitOfWork.cs
    ??? UsuarioRepository.cs
    ??? VentaRepository.cs
```

**Dependencias:**
- SciFiHub.Domain (referencia de proyecto)
- Microsoft.EntityFrameworkCore
- Microsoft.EntityFrameworkCore.SqlServer
- Dapper
- BCrypt.Net-Next

---

### ? SciFiHub.Web (Capa de Presentación)
**Propósito:** Interfaz de usuario, Controllers, Views, DTOs

```
SciFiHub.Web\
??? Controllers\
?   ??? AdminController.cs
?   ??? AuthController.cs
?   ??? CarritoController.cs
?   ??? CatalogoController.cs
?   ??? ClienteController.cs
?   ??? HomeController.cs
?   ??? VendedorController.cs
?   ??? Api\
?   ?   ??? BusquedaController.cs
?   ?   ??? CarritoApiController.cs
?   ?   ??? LibrosController.cs
?   ?   ??? UbicacionesController.cs
?   ?   ??? UsuariosController.cs
?   ?   ??? VentasController.cs
?   ??? Common\
?       ??? BaseController.cs
??? Views\
??? Services\
?   ??? Interfaces\
?   ??? Implementations\
??? DTOs\
??? Validators\
??? Mappings\
??? Helpers\
??? wwwroot\
```

**Dependencias:**
- SciFiHub.Infrastructure (referencia de proyecto)
- AutoMapper
- FluentValidation
- QuestPDF

---

## ?? REFERENCIAS DE PROYECTOS

### SciFiHub.Domain.csproj
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
</Project>
```

### SciFiHub.Infrastructure.csproj
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="9.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="9.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="9.0.0">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="Dapper" Version="2.1.66" />
    <PackageReference Include="BCrypt.Net-Next" Version="4.0.3" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\SciFiHub.Domain\SciFiHub.Domain.csproj" />
  </ItemGroup>
</Project>
```

### SciFiHub.Web.csproj
```xml
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
```

---

## ?? POSIBLES PROBLEMAS Y SOLUCIONES

### ? Error: "The type or namespace name 'X' could not be found"

**Causa:** Referencias de using statements incorrectas

**Solución:** 
Los namespaces ahora son diferentes. Los archivos en los proyectos Domain e Infrastructure usarán:
- `SciFiHub.Domain.Entities`
- `SciFiHub.Domain.Interfaces`
- `SciFiHub.Infrastructure.Data`
- `SciFiHub.Infrastructure.Repositories`

El script ya moverá los archivos con sus namespaces correctos, pero si tienes problemas:

```powershell
# Limpiar y recompilar
dotnet clean
dotnet restore
dotnet build
```

---

### ? Error: "The name 'DbContext' does not exist in the current context"

**Causa:** Faltan using statements en Infrastructure

**Solución:**
Asegúrate de que los archivos en `SciFiHub.Infrastructure` tengan:
```csharp
using Microsoft.EntityFrameworkCore;
using SciFiHub.Domain.Entities;
using SciFiHub.Domain.Interfaces;
```

---

### ? Error: "Could not load file or assembly 'SciFiHub.Domain'"

**Causa:** Referencias de proyectos no actualizadas

**Solución:**
```powershell
# En Visual Studio: clic derecho en solución ? "Restore NuGet Packages"
# O desde terminal:
dotnet restore

# Limpiar y recompilar
dotnet clean
dotnet build --no-incremental
```

---

## ? VERIFICACIÓN FINAL

### Checklist de Verificación:

- [ ] ? El proyecto `SciFiHub.Domain` contiene carpetas: Entities, Interfaces, Enums, ValueObjects, Common
- [ ] ? El proyecto `SciFiHub.Infrastructure` contiene carpetas: Data, Repositories
- [ ] ? El proyecto `SciFiHub.Web` ya NO tiene las carpetas: SciFiHub.Domain, SciFiHub.Infrastructure
- [ ] ? `dotnet build` se ejecuta sin errores
- [ ] ? Visual Studio muestra las referencias correctas entre proyectos:
  - SciFiHub.Infrastructure ? referencia a ? SciFiHub.Domain
  - SciFiHub.Web ? referencia a ? SciFiHub.Infrastructure
- [ ] ? Los namespaces en los archivos reflejan su ubicación real:
  - Archivos en Domain usan `namespace SciFiHub.Domain.*`
  - Archivos en Infrastructure usan `namespace SciFiHub.Infrastructure.*`
  - Archivos en Web usan `namespace SciFiHub.Web.*`

---

## ?? BENEFICIOS DE LA NUEVA ARQUITECTURA

### 1?? **Separación de Responsabilidades**
- Domain: Lógica de negocio pura
- Infrastructure: Acceso a datos
- Web: Presentación e interacción

### 2?? **Mejor Mantenibilidad**
- Cambios en la base de datos solo afectan Infrastructure
- Cambios en reglas de negocio solo afectan Domain
- Cambios en UI solo afectan Web

### 3?? **Testabilidad**
- Puedes testear Domain sin necesidad de base de datos
- Puedes testear Web sin necesidad de infraestructura real

### 4?? **Reusabilidad**
- Domain puede ser referenciado por otros proyectos (APIs, Consola, etc.)
- Infrastructure puede ser reemplazado sin tocar Domain

### 5?? **Escalabilidad**
- Puedes agregar nuevos proyectos (SciFiHub.Api, SciFiHub.Tests, etc.)
- La arquitectura está lista para crecer

---

## ?? DIAGRAMA DE DEPENDENCIAS

```
???????????????????????????????????????????????????
?           SciFiHub.Web (Presentación)           ?
?  Controllers, Views, Services, DTOs, Validators ?
???????????????????????????????????????????????????
                     ? depende de
                     ?
???????????????????????????????????????????????????
?      SciFiHub.Infrastructure (Datos)            ?
?    DbContext, Repositories, Configurations      ?
???????????????????????????????????????????????????
                     ? depende de
                     ?
???????????????????????????????????????????????????
?       SciFiHub.Domain (Dominio/Negocio)         ?
?  Entities, Interfaces, Enums, ValueObjects      ?
???????????????????????????????????????????????????
          ? No depende de nadie
```

---

## ?? RESUMEN EJECUTIVO

| Aspecto | Antes | Después |
|---------|-------|---------|
| **Arquitectura** | Monolítica | En Capas (MVC) |
| **Proyectos activos** | 1 (SciFiHub.Web) | 3 (Domain, Infrastructure, Web) |
| **Separación de responsabilidades** | ? Todo mezclado | ? Bien separado |
| **Mantenibilidad** | ?? Baja | ?? Alta |
| **Testabilidad** | ?? Difícil | ?? Fácil |
| **Escalabilidad** | ?? Limitada | ?? Excelente |

---

## ?? SOPORTE

Si encuentras problemas durante la reestructuración:

1. **Revisa los logs** del script de PowerShell
2. **Verifica las referencias** en Visual Studio
3. **Ejecuta** `dotnet clean` y `dotnet build`
4. **Revisa** que los namespaces sean correctos
5. **Consulta** este documento para soluciones comunes

---

## ? ESTADO FINAL ESPERADO

```powershell
# Este comando debe ejecutarse sin errores:
dotnet build

# Salida esperada:
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

---

**?? ¡Listo! Tu proyecto ahora tiene una arquitectura profesional en capas.**

---

*Documento generado como parte de la reestructuración del proyecto SciFiHub*  
*Fecha: 2025*  
*Versión: 1.0*
