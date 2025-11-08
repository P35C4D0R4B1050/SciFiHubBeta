# SciFiHub Database - SQL Server 2021+

## ?? Descripción General

Base de datos empresarial completa para SciFiHub, un sistema e-commerce de compra/venta de libros diseñado para una librería pequeña/mediana. Implementa arquitectura robusta con mejores prácticas de SQL Server 2021+.

## ?? Características Principales

### ? Arquitectura Empresarial
- **Base de datos optimizada** con collation Modern_Spanish_CI_AS
- **Isolation levels** configurados (SNAPSHOT_ISOLATION, READ_COMMITTED_SNAPSHOT)
- **Recovery model** SIMPLE (ajustable a FULL para producción)
- **Índices estratégicos** (clustered, non-clustered, filtered, full-text)

### ? Estructura de Tablas
- **7 tablas principales** con diseño normalizado
- **Claves primarias GUID** con NEWSEQUENTIALID() para mejor performance
- **Soft delete** implementado (campo IsDeleted)
- **Auditoría automática** (CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
- **Constraints completos** (CHECK, DEFAULT, UNIQUE, FOREIGN KEY)

### ? Procedimientos Almacenados (40+)
- **Gestión de Usuarios** (8 SPs)
- **Gestión de Libros** (10 SPs)
- **Gestión de Ventas** (7 SPs)
- **Gestión de Carrito** (6 SPs)
- **Reportes y Analytics** (9 SPs)
- Uso de CTEs, window functions, transacciones, manejo de errores

### ? Funciones
- **5 funciones escalares** (cálculos, validaciones)
- **5 funciones de tabla (TVF)** (consultas complejas)

### ? Vistas Optimizadas (7)
- Vista de inventario completo
- Vista de ventas detalladas
- Vista de dashboard con métricas
- Vista de catálogo público
- Vista de clientes activos

### ? Triggers Inteligentes
- Actualización automática de fechas de modificación
- Triggers no invasivos (control en SPs)

### ? Full-Text Search
- Catálogo full-text configurado
- Índice full-text en tabla Libros (Titulo, Autor, Sinopsis, Descripción)
- SP de búsqueda avanzada

### ? Datos Iniciales (Seeding)
- 1 Administrador
- 2 Vendedores
- 3 Clientes de prueba
- 7 Categorías de géneros
- 30 Libros reconocidos de ciencia ficción y fantasía

## ?? Estructura de Archivos

```
Database/
??? 00_MasterScript_SciFiHub.sql           # Script maestro con instrucciones
??? 01_CreateDatabase_SciFiHub.sql         # Creación de BD con configuraciones
??? 02_CreateTables_SciFiHub.sql           # Todas las tablas con constraints
??? 03_CreateFunctions_SciFiHub.sql        # Funciones escalares y TVF
??? 04_CreateSP_Usuarios_SciFiHub.sql      # SPs de gestión de usuarios
??? 05_CreateSP_Libros_SciFiHub.sql        # SPs de gestión de libros
??? 06_CreateSP_Ventas_SciFiHub.sql        # SPs de gestión de ventas
??? 07_CreateSP_Carrito_SciFiHub.sql       # SPs de gestión de carrito
??? 08_CreateSP_Reportes_SciFiHub.sql      # SPs de reportes y analytics
??? 09_CreateViews_SciFiHub.sql            # Vistas optimizadas
??? 10_CreateTriggers_SciFiHub.sql         # Triggers
??? 11_CreateFullTextIndex_SciFiHub.sql    # Configuración Full-Text
??? 12_SeedData_SciFiHub.sql               # Datos iniciales
```

## ?? Instalación

### Opción 1: Ejecución Manual en SSMS

1. Abrir **SQL Server Management Studio (SSMS) 21**
2. Conectarse a la instancia de SQL Server
3. Ejecutar cada script en orden:

```sql
-- 1. Crear base de datos
:r 01_CreateDatabase_SciFiHub.sql

-- 2. Crear tablas
:r 02_CreateTables_SciFiHub.sql

-- 3. Crear funciones
:r 03_CreateFunctions_SciFiHub.sql

-- ... y así sucesivamente hasta el script 12
```

### Opción 2: Ejecución con SQLCMD (PowerShell)

```powershell
# Navegar a la carpeta Database
cd Database

# Ejecutar todos los scripts en orden
$scripts = @(
    "01_CreateDatabase_SciFiHub.sql",
    "02_CreateTables_SciFiHub.sql",
    "03_CreateFunctions_SciFiHub.sql",
    "04_CreateSP_Usuarios_SciFiHub.sql",
    "05_CreateSP_Libros_SciFiHub.sql",
    "06_CreateSP_Ventas_SciFiHub.sql",
    "07_CreateSP_Carrito_SciFiHub.sql",
    "08_CreateSP_Reportes_SciFiHub.sql",
    "09_CreateViews_SciFiHub.sql",
    "10_CreateTriggers_SciFiHub.sql",
    "11_CreateFullTextIndex_SciFiHub.sql",
    "12_SeedData_SciFiHub.sql"
)

foreach ($script in $scripts) {
    Write-Host "Ejecutando $script..." -ForegroundColor Green
    sqlcmd -S localhost -i $script
}
```

### Opción 3: Script Automatizado (Batch)

```batch
@echo off
echo Instalando SciFiHub Database...

sqlcmd -S localhost -i 01_CreateDatabase_SciFiHub.sql
sqlcmd -S localhost -i 02_CreateTables_SciFiHub.sql
sqlcmd -S localhost -i 03_CreateFunctions_SciFiHub.sql
sqlcmd -S localhost -i 04_CreateSP_Usuarios_SciFiHub.sql
sqlcmd -S localhost -i 05_CreateSP_Libros_SciFiHub.sql
sqlcmd -S localhost -i 06_CreateSP_Ventas_SciFiHub.sql
sqlcmd -S localhost -i 07_CreateSP_Carrito_SciFiHub.sql
sqlcmd -S localhost -i 08_CreateSP_Reportes_SciFiHub.sql
sqlcmd -S localhost -i 09_CreateViews_SciFiHub.sql
sqlcmd -S localhost -i 10_CreateTriggers_SciFiHub.sql
sqlcmd -S localhost -i 11_CreateFullTextIndex_SciFiHub.sql
sqlcmd -S localhost -i 12_SeedData_SciFiHub.sql

echo.
echo Instalación completada!
pause
```

## ?? Credenciales por Defecto

### Administrador
- **Usuario:** admin
- **Email:** admin@scifihub.com
- **Password:** Admin@2024

### Vendedores
| Usuario | Email | Password |
|---------|-------|----------|
| cmendoza | carlos.mendoza@scifihub.com | Vendedor@2024 |
| mtorres | maria.torres@scifihub.com | Vendedor@2024 |

### Clientes de Prueba
| Usuario | Email | Password |
|---------|-------|----------|
| jperez | juan.perez@email.com | Cliente@2024 |
| amartinez | ana.martinez@email.com | Cliente@2024 |
| rsilva | roberto.silva@email.com | Cliente@2024 |

**?? IMPORTANTE:** Las contraseñas mostradas son para desarrollo. En el script SQL están hasheadas (simuladas). En producción, usar BCrypt, Argon2 o PBKDF2 desde la aplicación .NET.

## ?? Modelo de Datos

### Entidades Principales

#### Usuarios
- **Roles:** Administrador, Vendedor, Cliente
- **Campos:** Id, NombreCompleto, Email, Username, PasswordHash, Rol, Estado
- **Auditoría:** CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, IsDeleted

#### Libros
- **Campos:** ISBN (único), Titulo, Autor, Editorial, Categoria, Precio, Stock
- **Extras:** ImagenPortada, Sinopsis, Descripción, Destacado, PrecioOferta
- **Relaciones:** Categoría, DetallesVenta, DetallesCarrito

#### Ventas
- **Campos:** NumeroVenta (autogenerado), Cliente, Vendedor, FechaVenta, Total
- **Estados:** Pendiente, Procesando, Completada, Cancelada, Reembolsada
- **Métodos de Pago:** Efectivo, TarjetaCredito, Yape, Plin, Transferencia
- **Dirección de Envío:** Value Object embebido

#### DetallesVenta
- Relación muchos-a-muchos entre Venta y Libro
- Almacena: Cantidad, PrecioUnitario, Descuento, Subtotal

#### CarritoCompras y DetallesCarrito
- Persistencia del carrito para clientes registrados
- Estados: Activo, Convertido, Abandonado

#### AuditoriaInventario
- Tracking completo de todos los cambios en stock
- Tipos: Ingreso, Venta, Ajuste, Devolucion, Anulacion

#### Categorias
- Clasificación de géneros literarios
- Soporte para jerarquía (CategoriaPadreId)

## ?? Procedimientos Almacenados Principales

### Usuarios
```sql
-- Autenticar usuario
EXEC sp_Usuario_Autenticar @Username = 'admin', @PasswordHash = 'hash_aqui'

-- Listar con paginación
EXEC sp_Usuario_ListarPaginado @PageNumber = 1, @PageSize = 10, @Rol = 'Cliente'

-- Insertar usuario
DECLARE @UsuarioId UNIQUEIDENTIFIER;
EXEC sp_Usuario_Insertar 
    @NombreCompleto = 'Nuevo Cliente',
    @Email = 'nuevo@email.com',
    @Username = 'nuevouser',
    @PasswordHash = 'hash_aqui',
    @Rol = 'Cliente',
    @UsuarioId = @UsuarioId OUTPUT
```

### Libros
```sql
-- Buscar libros (Full-Text)
DECLARE @TotalRegistros INT;
EXEC sp_Libro_BuscarPorCriterios 
    @BusquedaTexto = 'Dune arena',
    @SoloDisponibles = 1,
    @PageNumber = 1,
    @PageSize = 12,
    @TotalRegistros = @TotalRegistros OUTPUT

-- Obtener destacados
EXEC sp_Libro_ObtenerDestacados @Top = 10

-- Obtener más vendidos
EXEC sp_Libro_ObtenerMasVendidos @Top = 20, @FechaInicio = '2024-01-01'

-- Actualizar stock
EXEC sp_Libro_ActualizarStock 
    @LibroId = 'guid_aqui',
    @Cantidad = 50,
    @TipoMovimiento = 'Ingreso',
    @Motivo = 'Compra de inventario'
```

### Ventas
```sql
-- Registrar venta completa
DECLARE @VentaId UNIQUEIDENTIFIER, @NumeroVenta NVARCHAR(50);
DECLARE @DetallesJSON NVARCHAR(MAX) = N'[
    {"LibroId":"guid1","Cantidad":2,"PrecioUnitario":89.90,"Descuento":0},
    {"LibroId":"guid2","Cantidad":1,"PrecioUnitario":69.90,"Descuento":0}
]';

EXEC sp_Venta_Registrar
    @ClienteId = 'guid_cliente',
    @VendedorId = 'guid_vendedor',
    @Subtotal = 249.70,
    @Descuento = 0,
    @IGV = 44.95,
    @Total = 294.65,
    @MetodoPago = 'TarjetaCredito',
    @DireccionCalle = 'Av. Ejemplo 123',
    @DireccionCiudad = 'Lima',
    @DireccionDepartamento = 'Lima',
    @DireccionPais = 'Perú',
    @DetallesVentaJSON = @DetallesJSON,
    @VentaId = @VentaId OUTPUT,
    @NumeroVenta = @NumeroVenta OUTPUT

-- Obtener detalle de venta
EXEC sp_Venta_ObtenerDetalle @VentaId = 'guid_venta'

-- Cancelar venta (restaura stock)
EXEC sp_Venta_CancelarVenta 
    @VentaId = 'guid_venta',
    @Motivo = 'Cliente solicitó cancelación'
```

### Carrito
```sql
-- Agregar al carrito
EXEC sp_Carrito_AgregarItem 
    @ClienteId = 'guid_cliente',
    @LibroId = 'guid_libro',
    @Cantidad = 2

-- Obtener carrito del cliente
EXEC sp_Carrito_ObtenerPorCliente @ClienteId = 'guid_cliente'

-- Checkout (convertir carrito a venta)
DECLARE @VentaId UNIQUEIDENTIFIER, @NumeroVenta NVARCHAR(50);
EXEC sp_Carrito_ConvertirAVenta
    @ClienteId = 'guid_cliente',
    @MetodoPago = 'Yape',
    @DireccionCalle = 'Jr. Lima 456',
    @DireccionCiudad = 'Arequipa',
    @VentaId = @VentaId OUTPUT,
    @NumeroVenta = @NumeroVenta OUTPUT
```

### Reportes
```sql
-- Dashboard general
EXEC sp_Dashboard_EstadisticasGenerales

-- Ventas por período
EXEC sp_Reporte_VentasPorPeriodo 
    @FechaInicio = '2024-01-01',
    @FechaFin = '2024-12-31'

-- Stock bajo
EXEC sp_Reporte_StockBajo @UmbralStock = 10

-- Clientes frecuentes
EXEC sp_Reporte_ClientesFrecuentes @Top = 50

-- Tendencia de ventas (últimos 12 meses)
EXEC sp_Reporte_TendenciaVentas @NumeroMeses = 12
```

## ?? Vistas Principales

```sql
-- Inventario completo con estadísticas
SELECT * FROM vw_InventarioCompleto WHERE Stock <= 10

-- Dashboard con métricas
SELECT * FROM vw_DashboardMetricas

-- Catálogo público (para e-commerce)
SELECT * FROM vw_CatalogoPublico WHERE Disponible = 1 ORDER BY Destacado DESC

-- Ventas detalladas (para reportes)
SELECT * FROM vw_VentasDetalladas 
WHERE FechaVenta >= '2024-01-01' AND EstadoVenta = 'Completada'
```

## ?? Configuración Avanzada

### Ajustar Rutas de Archivos de Datos

Editar en `01_CreateDatabase_SciFiHub.sql`:

```sql
CREATE DATABASE SciFiHubDB
ON PRIMARY 
(
    NAME = N'SciFiHubDB_Data',
    FILENAME = N'C:\TU_RUTA\SciFiHubDB_Data.mdf',  -- Cambiar aquí
    SIZE = 100MB,
    FILEGROWTH = 50MB
)
LOG ON 
(
    NAME = N'SciFiHubDB_Log',
    FILENAME = N'C:\TU_RUTA\SciFiHubDB_Log.ldf',  -- Cambiar aquí
    SIZE = 50MB,
    FILEGROWTH = 25MB
)
```

### Cambiar a Recovery Model FULL (Producción)

```sql
USE master;
ALTER DATABASE SciFiHubDB SET RECOVERY FULL;
```

### Configurar Backups Automáticos

```sql
-- Backup completo diario
BACKUP DATABASE SciFiHubDB 
TO DISK = 'C:\Backups\SciFiHubDB_Full.bak'
WITH FORMAT, INIT, NAME = 'SciFiHubDB-Full Database Backup';

-- Backup de log cada hora
BACKUP LOG SciFiHubDB 
TO DISK = 'C:\Backups\SciFiHubDB_Log.trn'
WITH FORMAT, INIT, NAME = 'SciFiHubDB-Transaction Log Backup';
```

## ??? Seguridad

### Roles de Base de Datos (Pendiente de implementación)

El script incluye preparación para:
- `db_SciFiHub_Admin`: Acceso completo
- `db_SciFiHub_Vendedor`: Solo módulos de venta e inventario
- `db_SciFiHub_Cliente`: Solo lectura de catálogo y sus compras

### Hashing de Contraseñas

**?? CRÍTICO:** Las contraseñas en el seeding son simuladas. En la aplicación .NET:

```csharp
// Usar BCrypt.Net-Next
using BCrypt.Net;

// Al registrar
string hashedPassword = BCrypt.HashPassword(plainPassword);

// Al autenticar
bool isValid = BCrypt.Verify(plainPassword, hashedPassword);
```

## ?? Notas Técnicas

### Consideraciones de Performance

1. **Índices:** Todos los campos de búsqueda frecuente tienen índices
2. **Paginación:** Implementada en BD con OFFSET/FETCH para eficiencia
3. **Full-Text:** Para búsquedas de texto libre en catálogo
4. **Soft Delete:** Mejora performance vs DELETE físico
5. **GUID Secuencial:** NEWSEQUENTIALID() reduce fragmentación

### Escalabilidad Futura

- **Particionamiento:** Tabla Ventas lista para particionamiento por fecha
- **Temporal Tables:** Preparada para auditoría histórica
- **Caché:** Vistas optimizadas para caché en aplicación
- **Read Replicas:** Estructura lista para replicación

### Transacciones y Concurrencia

- **Isolation Levels:** SNAPSHOT_ISOLATION habilitado
- **Locks:** Minimizados con diseño apropiado
- **Deadlocks:** Orden consistente de acceso a tablas en SPs

## ?? Troubleshooting

### Error: "Full-Text Search no está instalado"

```sql
-- Verificar si está instalado
SELECT SERVERPROPERTY('IsFullTextInstalled')
-- Resultado debe ser 1

-- Si no está, instalar Full-Text desde SQL Server Setup
```

### Error: "No se puede crear el archivo de datos"

Cambiar rutas en `01_CreateDatabase_SciFiHub.sql` a una ubicación con permisos de escritura.

### Error: "Función no existe"

Asegurarse de ejecutar los scripts en orden. Las funciones deben crearse antes que los SPs que las usan.

## ?? Documentación Adicional

- [SQL Server 2021 Documentation](https://docs.microsoft.com/sql/sql-server/)
- [T-SQL Reference](https://docs.microsoft.com/sql/t-sql/language-reference)
- [Best Practices for Database Design](https://docs.microsoft.com/sql/relational-databases/best-practices)

## ?? Contribución

Este script es parte del proyecto SciFiHub. Para contribuir:

1. Seguir convenciones de nomenclatura establecidas
2. Documentar todos los SPs y funciones
3. Incluir manejo de errores en transacciones
4. Actualizar este README con cambios significativos

## ?? Licencia

Proyecto educativo - SciFiHub 2024

---

**Desarrollado para:** .NET 10 + SQL Server 2021+  
**Arquitectura:** Clean Architecture (Domain, Infrastructure, Web)  
**Versión:** 1.0.0  
**Última actualización:** 2024
