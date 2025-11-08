# ?? ERROR CRÍTICO - Columnas de BaseEntity Faltantes en Ventas

## ?? PROBLEMA IDENTIFICADO

**Error al registrar venta:**
```
Invalid column name 'CreatedAt'.
Invalid column name 'IsDeleted'.
Invalid column name 'UpdatedAt'.
```

**Causa:**
- Las entidades `Venta` y `DetalleVenta` heredan de `BaseEntity`
- `BaseEntity` define 6 columnas de auditoría
- Estas columnas **NO EXISTEN** en la tabla `Ventas` en SQL Server
- EF Core intenta insertar datos en columnas inexistentes ? **Error**

---

## ? SOLUCIÓN RÁPIDA (5 MINUTOS)

### 1?? Ejecutar Script SQL

**Archivo:** `Database/FixColumnasVentas.sql`

**Pasos:**
1. Abrir **SQL Server Management Studio (SSMS)**
2. Conectarse a `localhost` (o tu servidor)
3. Abrir archivo: `Database/FixColumnasVentas.sql`
4. **Ejecutar (F5)**

**Resultado esperado:**
```
? Columna CreatedAt agregada a Ventas
? Columna UpdatedAt agregada a Ventas
? Columna CreatedBy agregada a Ventas
? Columna UpdatedBy agregada a Ventas
? Columna IsDeleted agregada a Ventas
? Columna DeletedAt agregada a Ventas
? Todas las columnas agregadas exitosamente
```

---

### 2?? Reiniciar Aplicación

```bash
# Detener la aplicación (Ctrl+C)
cd E:\Proyecto\SciFiHub\SciFiHub
dotnet clean
dotnet build
dotnet run
```

---

### 3?? Probar Registro de Venta

1. **Login como Administrador o Vendedor**
2. **Ir a "Nueva Venta"**
3. **Seleccionar cliente**
4. **Agregar libros al carrito**
5. **Completar dirección de envío**
6. **Registrar venta**

**? Resultado esperado:**
```
? Venta registrada exitosamente
Número de venta: V20250109-0001
```

**? Antes:**
```
? Error al registrar venta: Invalid column name 'CreatedAt'...
```

---

## ?? COLUMNAS AGREGADAS

### Tabla: `Ventas`

| Columna | Tipo | Nullable | Default | Descripción |
|---------|------|----------|---------|-------------|
| `CreatedAt` | `DATETIME2` | NO | `GETUTCDATE()` | Fecha de creación |
| `UpdatedAt` | `DATETIME2` | SÍ | `NULL` | Última actualización |
| `CreatedBy` | `UNIQUEIDENTIFIER` | SÍ | `NULL` | Usuario que creó |
| `UpdatedBy` | `UNIQUEIDENTIFIER` | SÍ | `NULL` | Usuario que actualizó |
| `IsDeleted` | `BIT` | NO | `0` | Soft delete flag |
| `DeletedAt` | `DATETIME2` | SÍ | `NULL` | Fecha de eliminación |

### Tabla: `DetallesVenta`

Las mismas 6 columnas que `Ventas`.

---

## ?? VERIFICACIÓN

### Verificar en SQL Server:

```sql
-- Ver columnas agregadas
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Ventas'
AND COLUMN_NAME IN ('CreatedAt', 'UpdatedAt', 'CreatedBy', 'UpdatedBy', 'IsDeleted', 'DeletedAt')
ORDER BY COLUMN_NAME;

-- Debe retornar 6 filas
```

### Verificar en Aplicación:

```bash
# Ver logs de la aplicación
# Buscar:
>> Iniciando registro de venta...
>> Cliente seleccionado: [...]
>> Enviando venta...
>> ? Venta registrada exitosamente  # <- Debe aparecer esto
```

---

## ?? ¿POR QUÉ PASÓ ESTO?

**Problema de sincronización entre Código y Base de Datos:**

| Componente | Estado |
|------------|--------|
| **Entidades** (Código) | ? Heredan de `BaseEntity` |
| **EF Core Configuration** | ? Configurado en `BaseEntityConfiguration` |
| **Migraciones** | ? NO ejecutadas o incompletas |
| **Base de Datos** | ? Columnas NO existen |

**Resultado:** EF Core intenta insertar datos en columnas que no existen físicamente.

---

## ?? NOTAS TÉCNICAS

### BaseEntity (Código):

```csharp
public abstract class BaseEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }          // ? Faltaba en BD
    public DateTime? UpdatedAt { get; set; }         // ? Faltaba en BD
    public Guid? CreatedBy { get; set; }             // ? Faltaba en BD
    public Guid? UpdatedBy { get; set; }             // ? Faltaba en BD
    public bool IsDeleted { get; set; }              // ? Faltaba en BD
    public DateTime? DeletedAt { get; set; }         // ? Faltaba en BD
}
```

### Otras Tablas Afectadas:

El script verifica y corrige **todas** las tablas:
- ? Usuarios (probablemente ya tenía las columnas)
- ? Categorias
- ? Libros
- ? Ventas (corregido)
- ? DetallesVenta (corregido)
- ? CarritoCompras
- ? DetallesCarrito
- ? AuditoriasInventario

---

## ? CHECKLIST DE VERIFICACIÓN

- [ ] ? Script SQL ejecutado sin errores
- [ ] ? Mensaje "? Columna CreatedAt agregada a Ventas" visible
- [ ] ? Mensaje "? Columna IsDeleted agregada a Ventas" visible
- [ ] ? Query de verificación retorna 6 filas
- [ ] ? Aplicación reiniciada
- [ ] ? Login como Admin/Vendedor exitoso
- [ ] ? Registro de venta completado sin errores

---

## ?? PRÓXIMOS PASOS

### Si el script se ejecuta correctamente:

1. ? Reiniciar aplicación
2. ? Probar registro de venta
3. ? Verificar que se guarda en BD:
   ```sql
   SELECT TOP 1 * FROM Ventas ORDER BY CreatedAt DESC;
   ```

### Si persiste el error:

1. ? Verificar que la base de datos es `SciFiHubDB`
2. ? Verificar que el script se ejecutó completamente
3. ? Ver logs de aplicación para otros errores
4. ? Ejecutar query de verificación

---

## ?? SOPORTE

**Logs a revisar:**
- SQL Server: Mensajes del script
- Aplicación: Console output
- DevTools: Network ? POST `/api/Ventas` ? Response

**Comandos útiles:**
```bash
# Ver logs detallados
dotnet run --verbosity detailed

# Verificar conexión a BD
sqlcmd -S localhost -d SciFiHubDB -E -Q "SELECT @@VERSION"
```

---

**Versión:** 1.0  
**Fecha:** 2025-01-09  
**Estado:** ?? LISTO PARA EJECUTAR  
**Tiempo Estimado:** 5 minutos
