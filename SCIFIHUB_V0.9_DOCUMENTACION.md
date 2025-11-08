# ?? SCIFIHUB - VERSIÓN 0.9

## ?? INFORMACIÓN GENERAL

**Proyecto:** SciFiHub - Plataforma de E-commerce para Libros de Ciencia Ficción  
**Versión:** 0.9 (Beta)  
**Fecha de Release:** Enero 2025  
**Framework:** .NET 10.0  
**Tipo:** ASP.NET Core MVC / Razor Pages  
**Base de Datos:** SQL Server  
**Arquitectura:** Clean Architecture (3 Capas)  
**Repositorio:** https://github.com/P35C4D0R4B1050/SciFiHubBeta  

---

## ?? ESTADO DEL PROYECTO

### ? Proyecto Optimizado y Limpio

**Fecha de Limpieza:** Enero 2025  
**Archivos de Documentación Temporal Eliminados:** 158  
**Estado:** Proyecto limpio y listo para producción  

Este proyecto ha sido optimizado eliminando toda la documentación temporal, scripts de debugging, y archivos auxiliares generados durante el desarrollo. Solo se han preservado:

1. **Este documento** (`SCIFIHUB_V0.9_DOCUMENTACION.md`) - Documentación consolidada completa
2. **Database/README.md** - Documentación específica de la base de datos
3. **Archivos esenciales del proyecto** - Código fuente, configuración, y assets

**Resultado:** Repositorio limpio, organizado y profesional.

---

## ??? ARQUITECTURA DEL PROYECTO

### Estructura de Solución

```
SciFiHub.sln
?
??? SciFiHub.Domain/              (Capa de Dominio - Núcleo)
?   ??? Common/
?   ?   ??? BaseEntity.cs
?   ??? Entities/
?   ?   ??? Usuario.cs
?   ?   ??? Libro.cs
?   ?   ??? Categoria.cs
?   ?   ??? Venta.cs
?   ?   ??? DetalleVenta.cs
?   ?   ??? CarritoCompra.cs
?   ?   ??? DetalleCarrito.cs
?   ?   ??? AuditoriaInventario.cs
?   ??? Enums/
?   ?   ??? Enumerations.cs
?   ??? Interfaces/
?   ?   ??? IRepository.cs
?   ?   ??? IUnitOfWork.cs
?   ?   ??? IUsuarioRepository.cs
?   ?   ??? ILibroRepository.cs
?   ?   ??? ICategoriaRepository.cs
?   ?   ??? IVentaRepository.cs
?   ?   ??? ICarritoCompraRepository.cs
?   ??? ValueObjects/
?       ??? DireccionEnvio.cs
?
??? SciFiHub.Infrastructure/      (Capa de Infraestructura - Datos)
?   ??? Data/
?   ?   ??? SciFiHubDbContext.cs
?   ?   ??? Configurations/
?   ?       ??? UsuarioConfiguration.cs
?   ?       ??? LibroConfiguration.cs
?   ?       ??? CategoriaConfiguration.cs
?   ?       ??? VentaConfiguration.cs
?   ?       ??? DetalleVentaConfiguration.cs
?   ?       ??? CarritoCompraConfiguration.cs
?   ?       ??? DetalleCarritoConfiguration.cs
?   ?       ??? AuditoriaInventarioConfiguration.cs
?   ??? Repositories/
?       ??? Repository.cs (Genérico)
?       ??? UnitOfWork.cs
?       ??? UsuarioRepository.cs
?       ??? LibroRepository.cs
?       ??? CategoriaRepository.cs
?       ??? VentaRepository.cs
?       ??? CarritoCompraRepository.cs
?
??? SciFiHub.Web/                 (Capa de Presentación - UI)
    ??? Controllers/
    ?   ??? Common/
    ?   ?   ??? BaseController.cs
    ?   ??? HomeController.cs
    ?   ??? AuthController.cs
    ?   ??? CatalogoController.cs
    ?   ??? CarritoController.cs
    ?   ??? ClienteController.cs
    ?   ??? VendedorController.cs
    ?   ??? AdminController.cs
    ?   ??? Api/
    ?       ??? UsuariosController.cs
    ?       ??? LibrosController.cs
    ?       ??? VentasController.cs
    ?       ??? CarritoApiController.cs
    ?       ??? BusquedaController.cs
    ?       ??? UbicacionesController.cs
    ??? Services/
    ?   ??? Interfaces/
    ?   ?   ??? IAuthService.cs
    ?   ?   ??? IUsuarioService.cs
    ?   ?   ??? ILibroService.cs
    ?   ?   ??? IVentaService.cs
    ?   ?   ??? ICarritoService.cs
    ?   ?   ??? ICategoriaService.cs
    ?   ?   ??? IPdfService.cs
    ?   ??? AuthService.cs
    ?   ??? UsuarioService.cs
    ?   ??? LibroService.cs
    ?   ??? VentaService.cs
    ?   ??? CarritoService.cs
    ?   ??? CategoriaService.cs
    ?   ??? PdfService.cs
    ??? DTOs/
    ?   ??? Common/
    ?   ?   ??? Result.cs
    ?   ?   ??? PagedResult.cs
    ?   ??? Usuario/
    ?   ?   ??? UsuarioDTOs.cs
    ?   ??? Libro/
    ?   ?   ??? LibroDTOs.cs
    ?   ??? Venta/
    ?   ?   ??? VentaDTOs.cs
    ?   ??? Carrito/
    ?   ?   ??? CarritoDTOs.cs
    ?   ??? Categoria/
    ?       ??? CategoriaDTOs.cs
    ??? Validators/
    ?   ??? UsuarioValidators.cs
    ?   ??? LibroValidators.cs
    ??? Mappings/
    ?   ??? AutoMapperProfile.cs
    ??? Helpers/
    ?   ??? UbicacionPeruHelper.cs
    ??? Views/
    ??? wwwroot/
    ??? Data/
    ?   ??? DbInitializer.cs
    ??? Program.cs
```

---

## ?? PATRONES Y PRINCIPIOS IMPLEMENTADOS

### Clean Architecture
- ? **Domain Layer**: Entidades, interfaces, lógica de negocio pura
- ? **Infrastructure Layer**: Implementación de persistencia, repositorios
- ? **Presentation Layer**: UI, controllers, services de aplicación

### Design Patterns
- ? **Repository Pattern**: Abstracción del acceso a datos
- ? **Unit of Work Pattern**: Gestión de transacciones
- ? **Dependency Injection**: Inversión de control
- ? **DTO Pattern**: Transferencia de datos
- ? **Service Layer Pattern**: Lógica de aplicación
- ? **Validator Pattern**: Validación con FluentValidation
- ? **Mapper Pattern**: AutoMapper para transformación de objetos

### SOLID Principles
- ? **S**ingle Responsibility: Cada clase tiene una sola responsabilidad
- ? **O**pen/Closed: Abierto a extensión, cerrado a modificación
- ? **L**iskov Substitution: Interfaces correctamente implementadas
- ? **I**nterface Segregation: Interfaces específicas y segregadas
- ? **D**ependency Inversion: Dependencias hacia abstracciones

---

## ??? MODELO DE BASE DE DATOS

### Tablas Principales

#### Usuarios
```sql
- Id (int, PK, Identity)
- Email (nvarchar(100), unique)
- PasswordHash (nvarchar(MAX))
- Nombre (nvarchar(100))
- Apellido (nvarchar(100))
- Telefono (nvarchar(20), nullable)
- Direccion (nvarchar(200), nullable)
- Rol (nvarchar(20)) -- Administrador, Vendedor, Cliente
- FechaRegistro (datetime2)
- UltimoAcceso (datetime2, nullable)
- Activo (bit)
- CreatedAt (datetime2)
- UpdatedAt (datetime2, nullable)
```

#### Categorias
```sql
- Id (int, PK, Identity)
- Nombre (nvarchar(100), unique)
- Descripcion (nvarchar(500), nullable)
- Activo (bit)
- CreatedAt (datetime2)
- UpdatedAt (datetime2, nullable)
```

#### Libros
```sql
- Id (int, PK, Identity)
- Titulo (nvarchar(200))
- Autor (nvarchar(100))
- ISBN (nvarchar(13), unique)
- Editorial (nvarchar(100))
- AnoPublicacion (int, nullable)
- Descripcion (nvarchar(MAX), nullable)
- Precio (decimal(18,2))
- Stock (int)
- CategoriaId (int, FK -> Categorias)
- ImagenUrl (nvarchar(500), nullable)
- Destacado (bit)
- Novedad (bit)
- Oferta (bit)
- PrecioOferta (decimal(18,2), nullable)
- FechaOfertaInicio (datetime2, nullable)
- FechaOfertaFin (datetime2, nullable)
- Activo (bit)
- CreatedAt (datetime2)
- UpdatedAt (datetime2, nullable)
```

#### Ventas
```sql
- Id (int, PK, Identity)
- NumeroVenta (nvarchar(20), unique)
- UsuarioId (int, FK -> Usuarios)
- VendedorId (int, FK -> Usuarios)
- FechaVenta (datetime2)
- SubTotal (decimal(18,2))
- IGV (decimal(18,2))
- Total (decimal(18,2))
- Estado (nvarchar(20)) -- Pendiente, Confirmada, Enviada, Entregada, Cancelada
- MetodoPago (nvarchar(50))
- DireccionEnvio (nvarchar(MAX), nullable)
- Observaciones (nvarchar(500), nullable)
- CreatedAt (datetime2)
- UpdatedAt (datetime2, nullable)
```

#### DetallesVenta
```sql
- Id (int, PK, Identity)
- VentaId (int, FK -> Ventas)
- LibroId (int, FK -> Libros)
- Cantidad (int)
- PrecioUnitario (decimal(18,2))
- SubTotal (decimal(18,2))
- CreatedAt (datetime2)
- UpdatedAt (datetime2, nullable)
```

#### CarritosCompra
```sql
- Id (int, PK, Identity)
- UsuarioId (int, FK -> Usuarios, unique)
- FechaCreacion (datetime2)
- FechaActualizacion (datetime2, nullable)
- CreatedAt (datetime2)
- UpdatedAt (datetime2, nullable)
```

#### DetallesCarrito
```sql
- Id (int, PK, Identity)
- CarritoId (int, FK -> CarritosCompra)
- LibroId (int, FK -> Libros)
- Cantidad (int)
- PrecioUnitario (decimal(18,2))
- CreatedAt (datetime2)
- UpdatedAt (datetime2, nullable)
```

#### AuditoriasInventario
```sql
- Id (int, PK, Identity)
- LibroId (int, FK -> Libros)
- TipoMovimiento (nvarchar(20)) -- Entrada, Salida, Ajuste
- CantidadAnterior (int)
- CantidadNueva (int)
- Diferencia (int)
- UsuarioId (int, FK -> Usuarios)
- Motivo (nvarchar(500), nullable)
- Fecha (datetime2)
- CreatedAt (datetime2)
- UpdatedAt (datetime2, nullable)
```

---

## ?? FUNCIONALIDADES IMPLEMENTADAS

### Módulo de Autenticación
- ? Registro de usuarios
- ? Login con email y contraseña (BCrypt)
- ? Cambio de contraseña
- ? Gestión de perfil de usuario
- ? Control de acceso por roles (Claims-based)
- ? Políticas de autorización
- ? Cookies de autenticación seguras

### Módulo de Catálogo (Público)
- ? Listado de libros con paginación
- ? Búsqueda por título, autor, ISBN
- ? Filtrado por categoría
- ? Vista de detalles de libro
- ? Sección de libros destacados
- ? Sección de novedades
- ? Sección de ofertas
- ? Sistema de búsqueda avanzada

### Módulo de Carrito de Compras
- ? Agregar productos al carrito
- ? Actualizar cantidades
- ? Eliminar productos del carrito
- ? Validación de stock en tiempo real
- ? Cálculo automático de totales
- ? Proceso de checkout
- ? Selección de dirección de envío
- ? Resumen de pedido
- ? Confirmación de compra

### Módulo de Cliente
- ? Mis compras (historial)
- ? Detalle de compra
- ? Estado de pedidos
- ? Ver boleta/factura (PDF)

### Módulo de Vendedor
- ? Dashboard de ventas
- ? Crear nueva venta
- ? Listado de ventas
- ? Detalle de venta
- ? Generar boleta/factura (PDF)
- ? Gestión de inventario
- ? Actualización de stock
- ? Auditoría de movimientos

### Módulo de Administrador
- ? Dashboard con estadísticas
- ? Gestión de usuarios (CRUD)
- ? Crear vendedores y administradores
- ? Activar/desactivar usuarios
- ? Gestión de inventario completo
- ? Ver todas las ventas del sistema
- ? Reportes de ventas por período
- ? Estadísticas en tiempo real

### APIs REST Implementadas
- ? API de Usuarios (`/api/usuarios`)
- ? API de Libros (`/api/libros`)
- ? API de Ventas (`/api/ventas`)
- ? API de Carrito (`/api/carrito`)
- ? API de Búsqueda (`/api/busqueda`)
- ? API de Ubicaciones (`/api/ubicaciones`)

---

## ?? DEPENDENCIAS NUGET

### SciFiHub.Domain
- Sin dependencias externas (solo .NET 10)

### SciFiHub.Infrastructure
```xml
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="9.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="9.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="9.0.0" />
<PackageReference Include="Dapper" Version="2.1.66" />
<PackageReference Include="BCrypt.Net-Next" Version="4.0.3" />
```

### SciFiHub.Web
```xml
<PackageReference Include="AutoMapper.Extensions.Microsoft.DependencyInjection" Version="12.0.1" />
<PackageReference Include="FluentValidation.AspNetCore" Version="11.3.1" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="9.0.10" />
<PackageReference Include="QuestPDF" Version="2025.7.4" />
```

---

## ?? CONFIGURACIÓN

### appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=SciFiHubDB;Trusted_Connection=true;TrustServerCertificate=true;MultipleActiveResultSets=true"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### appsettings.Development.json
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  }
}
```

---

## ?? INSTALACIÓN Y EJECUCIÓN

### Prerequisitos
- .NET 10 SDK
- SQL Server o SQL Server LocalDB
- Visual Studio 2022 (v17.13+) o VS Code

### Pasos de Instalación

1. **Clonar el repositorio**
   ```bash
   git clone https://github.com/P35C4D0R4B1050/SciFiHubBeta.git
   cd SciFiHubBeta
   ```

2. **Configurar la cadena de conexión**
   - Editar `appsettings.json` con tu servidor SQL Server

3. **Restaurar paquetes**
   ```bash
   dotnet restore SciFiHub.sln
   ```

4. **Aplicar migraciones**
   ```bash
   dotnet ef database update --project SciFiHub.Web.csproj
   ```

5. **Ejecutar la aplicación**
   ```bash
   dotnet run --project SciFiHub.Web.csproj
   ```

6. **Acceder a la aplicación**
   - URL: `https://localhost:5001` o `http://localhost:5000`

### Datos de Prueba

Al ejecutar la aplicación en modo Development, se crean automáticamente:

**Usuarios de Prueba:**
- **Administrador:**
  - Email: `admin@scifihub.com`
  - Contraseña: `Admin@123`
  
- **Vendedor:**
  - Email: `vendedor@scifihub.com`
  - Contraseña: `Vendedor@123`
  
- **Cliente:**
  - Email: `cliente@scifihub.com`
  - Contraseña: `Cliente@123`

**Catálogo:**
- 50+ libros de ciencia ficción
- 5 categorías principales
- Libros con ofertas, novedades y destacados

---

## ?? SEGURIDAD

### Autenticación y Autorización
- ? **Cookie Authentication** con `HttpOnly` y `Secure` flags
- ? **BCrypt** para hash de contraseñas (factor 12)
- ? **Claims-based Authorization** con políticas por rol
- ? **Anti-forgery tokens** en formularios
- ? **HTTPS** obligatorio en producción
- ? **SameSite** cookies configuradas

### Validación de Datos
- ? **FluentValidation** para validación del lado del servidor
- ? Validación de entrada en todos los endpoints
- ? Prevención de SQL Injection (Entity Framework)
- ? Sanitización de HTML
- ? Validación de tipos de datos

### Control de Acceso
- ? Políticas de autorización por rol:
  - `AdminOnly`: Solo administradores
  - `AdminOrVendedor`: Administradores y vendedores
  - `ClienteRegistrado`: Cualquier usuario autenticado
  - `AccesoVentas`: Gestión de ventas
  - `GestionUsuarios`: Solo administradores
  - `GestionCatalogo`: Administradores y vendedores

---

## ?? TESTING (Pendiente para v1.0)

### Tests Unitarios (Planeado)
- Domain Layer tests
- Repository tests con InMemory DB
- Service Layer tests con mocks

### Tests de Integración (Planeado)
- API endpoints tests
- Database integration tests
- Authentication flow tests

### Tests E2E (Planeado)
- User journey tests
- Purchase flow tests
- Admin operations tests

---

## ?? RENDIMIENTO

### Optimizaciones Implementadas
- ? **Paginación** en listados de libros y ventas
- ? **Eager Loading** con `Include()` para evitar N+1 queries
- ? **Dapper** para queries de solo lectura optimizadas
- ? **AutoMapper** para mapeo eficiente de DTOs
- ? **Async/Await** en todas las operaciones de I/O
- ? **Connection Pooling** con Entity Framework Core
- ? **Índices** en columnas frecuentemente consultadas

### Métricas Esperadas
- Tiempo de respuesta promedio: < 200ms
- Consultas de catálogo: < 100ms
- Operaciones de escritura: < 500ms
- Generación de PDF: < 2s

---

## ?? PROBLEMAS CONOCIDOS Y SOLUCIONES

### 1. Error de Migración Inicial
**Problema:** Migraciones aplicadas en modo incorrecto  
**Solución:** Ejecutar scripts SQL directamente para crear base de datos

### 2. Stock Negativo en Carrito
**Problema:** Validación de stock no sincronizada  
**Solución:** Validación en tiempo real antes de confirmar compra

### 3. Sesión de Carrito Expira
**Problema:** Carrito se pierde al cerrar navegador  
**Solución:** Carrito persistido en BD para usuarios autenticados

### 4. Generación de PDF Lenta
**Problema:** PDFs complejos tardan en generar  
**Solución:** Simplificación de layout y generación asíncrona

---

## ??? ROADMAP v1.0

### Features Planeadas
- [ ] Sistema de reseñas y calificaciones
- [ ] Wishlist / Lista de deseos
- [ ] Sistema de cupones y descuentos
- [ ] Notificaciones por email
- [ ] Integración con pasarelas de pago
- [ ] Dashboard de analytics avanzado
- [ ] Exportación de reportes (Excel)
- [ ] Sistema de devoluciones
- [ ] Chat de soporte
- [ ] Recomendaciones personalizadas

### Mejoras Técnicas
- [ ] Tests unitarios completos
- [ ] Tests de integración
- [ ] CI/CD con GitHub Actions
- [ ] Docker containerization
- [ ] Logging centralizado (Serilog)
- [ ] Caché con Redis
- [ ] Health checks
- [ ] API versioning
- [ ] Swagger/OpenAPI documentation
- [ ] Rate limiting

---

## ?? CHANGELOG v0.9

### Agregado
- ? Arquitectura en 3 capas (Clean Architecture)
- ? Sistema completo de autenticación y autorización
- ? Gestión de usuarios con roles
- ? Catálogo de productos con búsqueda y filtros
- ? Carrito de compras funcional
- ? Proceso de checkout completo
- ? Módulo de ventas para vendedores
- ? Dashboard administrativo
- ? Generación de boletas en PDF
- ? Auditoría de inventario
- ? APIs REST para operaciones principales
- ? Validación de datos con FluentValidation
- ? Mapeo automático con AutoMapper

### Corregido
- ? Validación de stock en tiempo real
- ? Cálculo correcto de IGV (18%)
- ? Generación de número de venta único
- ? Formato de boleta en una sola página
- ? Estado de ventas en dashboard
- ? Permisos de acceso por rol
- ? Validación de formularios

### Mejorado
- ? Rendimiento de consultas con Dapper
- ? UI/UX responsive
- ? Mensajes de error descriptivos
- ? Experiencia de usuario en checkout
- ? Dashboard con estadísticas en tiempo real

---

## ?? EQUIPO DE DESARROLLO

**Desarrollador Principal:** P35C4D0R4B1050  
**GitHub:** https://github.com/P35C4D0R4B1050  
**Repositorio:** https://github.com/P35C4D0R4B1050/SciFiHubBeta  

---

## ?? LICENCIA

Proyecto educativo - Todos los derechos reservados © 2025

---

## ?? SOPORTE

Para reportar bugs o solicitar features:
- GitHub Issues: https://github.com/P35C4D0R4B1050/SciFiHubBeta/issues

---

## ?? AGRADECIMIENTOS

- Comunidad de .NET
- Entity Framework Core Team
- AutoMapper Contributors
- QuestPDF Team
- FluentValidation Team

---

**Fecha de Release:** Enero 2025  
**Versión:** 0.9 Beta  
**Estado:** En desarrollo activo  
**Próxima Versión:** 1.0 (Q2 2025)

---

*SciFiHub - Tu librería de ciencia ficción en línea* ????
