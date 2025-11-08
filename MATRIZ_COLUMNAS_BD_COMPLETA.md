# ?? MATRIZ COMPLETA - Columnas BaseEntity por Tabla

## ?? RESUMEN EJECUTIVO

**Fecha de Verificación:** 2025-01-09  
**Método:** Verificación manual en base de datos  
**Total de Tablas:** 8

---

## ?? MATRIZ COMPLETA

| Tabla | Id | CreatedAt | UpdatedAt | IsDeleted | CreatedBy | UpdatedBy | DeletedAt | Total | Categoría |
|-------|----|-----------|-----------|-----------|-----------|-----------|-----------| ------|-----------|
| **Usuarios** | ? | ? | ? | ? | ? | ? | ? | **7/7** | ?? COMPLETA |
| **Libros** | ? | ? | ? | ? | ? | ? | ? | **7/7** | ?? COMPLETA |
| **Ventas** | ? | ? | ? | ? | ? | ? | ? | **7/7** | ?? COMPLETA |
| **Categorias** | ? | ? | ? | ? | ? | ? | ? | **7/7** | ?? COMPLETA |
| **DetallesVenta** | ? | ? | ? | ? | ? | ? | ? | **7/7** | ?? COMPLETA |
| **CarritoCompras** | ? | ? | ? | ? | ? | ? | ? | **5/7** | ?? PARCIAL |
| **DetallesCarrito** | ? | ? | ? | ? | ? | ? | ? | **4/7** | ?? MÍNIMA |
| **AuditoriasInventario** | ? | ? | ? | ? | ? | ? | ? | **4/7** | ?? MÍNIMA |

**Leyenda:**
- ? = Columna existe en BD
- ? = Columna NO existe en BD
- ?? = Auditoría completa (7/7)
- ?? = Auditoría parcial (5-6/7)
- ?? = Auditoría mínima (3-4/7)

---

## ?? ESTADÍSTICAS

### Por Categoría
- ?? **COMPLETA (7/7):** 5 tablas (62.5%)
  - Usuarios, Libros, Ventas, Categorias, DetallesVenta

- ?? **PARCIAL (5-6/7):** 1 tabla (12.5%)
  - CarritoCompras

- ?? **MÍNIMA (3-4/7):** 2 tablas (25%)
  - DetallesCarrito, AuditoriasInventario

---

### Por Columna

| Columna | Presente en | Porcentaje |
|---------|-------------|------------|
| **Id** | 8/8 tablas | 100% |
| **CreatedBy** | 8/8 tablas | 100% |
| **UpdatedBy** | 8/8 tablas | 100% |
| **DeletedAt** | 8/8 tablas | 100% |
| **CreatedAt** | 5/8 tablas | 62.5% |
| **UpdatedAt** | 5/8 tablas | 62.5% |
| **IsDeleted** | 6/8 tablas | 75% |

**Análisis:**
- ? **Columnas universales:** Id, CreatedBy, UpdatedBy, DeletedAt (todas las tablas)
- ?? **Columnas variables:** CreatedAt, UpdatedAt (solo tablas principales)
- ?? **Columna especial:** IsDeleted (6 de 8 tablas)

---

## ?? ANÁLISIS POR GRUPO

### ?? Grupo 1: Auditoría Completa (5 tablas)

**Tablas:** Usuarios, Libros, Ventas, Categorias, DetallesVenta

**Características:**
- Tienen TODAS las columnas de BaseEntity
- Son tablas principales o de negocio crítico
- Requieren auditoría completa de cambios
- Soportan soft delete

**Configuración EF Core:**
```csharp
// Mapear TODAS las propiedades
builder.Property(x => x.CreatedAt).IsRequired();
builder.Property(x => x.UpdatedAt).IsRequired(false);
builder.Property(x => x.IsDeleted).IsRequired().HasDefaultValue(false);
builder.Property(x => x.CreatedBy).IsRequired(false);
builder.Property(x => x.UpdatedBy).IsRequired(false);
builder.Property(x => x.DeletedAt).IsRequired(false);

// Aplicar filtro global
modelBuilder.Entity<T>().HasQueryFilter(x => !x.IsDeleted);
```

---

### ?? Grupo 2: Auditoría Parcial (1 tabla)

**Tabla:** CarritoCompras

**Características:**
- NO tiene CreatedAt ni UpdatedAt
- SÍ tiene IsDeleted (soft delete)
- SÍ tiene CreatedBy, UpdatedBy, DeletedAt
- Usa campos propios: FechaCreacion, FechaActualizacion

**Configuración EF Core:**
```csharp
// IGNORAR propiedades que NO existen
builder.Ignore(c => c.CreatedAt);
builder.Ignore(c => c.UpdatedAt);

// MAPEAR propiedades que SÍ existen
builder.Property(c => c.IsDeleted).IsRequired().HasDefaultValue(false);
builder.Property(c => c.CreatedBy).IsRequired(false);
builder.Property(c => c.UpdatedBy).IsRequired(false);
builder.Property(c => c.DeletedAt).IsRequired(false);

// Aplicar filtro global (tiene IsDeleted)
modelBuilder.Entity<CarritoCompra>().HasQueryFilter(c => !c.IsDeleted);
```

---

### ?? Grupo 3: Auditoría Mínima (2 tablas)

**Tablas:** DetallesCarrito, AuditoriasInventario

**Características:**
- NO tienen CreatedAt, UpdatedAt, IsDeleted
- SÍ tienen CreatedBy, UpdatedBy, DeletedAt
- Son tablas de detalle o históricas
- NO soportan soft delete

**Configuración EF Core:**
```csharp
// IGNORAR propiedades que NO existen
builder.Ignore(x => x.CreatedAt);
builder.Ignore(x => x.UpdatedAt);
builder.Ignore(x => x.IsDeleted);

// MAPEAR propiedades que SÍ existen
builder.Property(x => x.CreatedBy).IsRequired(false);
builder.Property(x => x.UpdatedBy).IsRequired(false);
builder.Property(x => x.DeletedAt).IsRequired(false);

// NO aplicar filtro global (no tienen IsDeleted)
```

---

## ?? PATRONES IDENTIFICADOS

### Patrón 1: Tablas Maestras
**Tablas:** Usuarios, Libros, Categorias

**Patrón:**
- Auditoría completa (7/7)
- Soft delete habilitado
- Son entidades principales del dominio

---

### Patrón 2: Tablas de Transacciones
**Tablas:** Ventas, DetallesVenta

**Patrón:**
- Auditoría completa (7/7)
- Soft delete habilitado
- Relacionadas con operaciones de negocio

---

### Patrón 3: Tablas de Sesión/Temporal
**Tabla:** CarritoCompras

**Patrón:**
- Auditoría parcial (5/7)
- Soft delete habilitado
- Datos temporales con ciclo de vida corto
- Usan campos propios de fecha (FechaCreacion, FechaActualizacion)

---

### Patrón 4: Tablas de Detalle/Historial
**Tablas:** DetallesCarrito, AuditoriasInventario

**Patrón:**
- Auditoría mínima (4/7)
- NO tienen soft delete
- Son dependientes de tablas maestras
- Eliminación en cascada o por fecha

---

## ?? DECISIONES DE DISEÑO (Inferidas)

### ¿Por qué CarritoCompras NO tiene CreatedAt/UpdatedAt?

**Razón probable:**
- Ya tiene `FechaCreacion` y `FechaActualizacion` (campos específicos)
- Evitar redundancia de información
- Diseño previo al patrón BaseEntity

**Implicación:**
- Usar campos propios en lógica de negocio
- Ignorar CreatedAt/UpdatedAt en EF Core

---

### ¿Por qué DetallesCarrito NO tiene IsDeleted?

**Razón probable:**
- Son datos transitorios (carrito activo)
- Se eliminan físicamente cuando carrito se convierte o abandona
- NO requieren historial de eliminación

**Implicación:**
- NO aplicar filtro global de soft delete
- Eliminación física directa

---

### ¿Por qué AuditoriasInventario NO tiene CreatedAt/UpdatedAt/IsDeleted?

**Razón probable:**
- Tiene `FechaMovimiento` (campo específico de auditoría)
- Es tabla de SOLO INSERCIÓN (no se actualiza)
- NO se elimina (auditoría permanente)

**Implicación:**
- Usar FechaMovimiento en lugar de CreatedAt
- Sin updates ni deletes (tabla append-only)

---

## ?? CHECKLIST DE CONFIGURACIÓN

### ? Tablas COMPLETAS (5)
- [x] Usuarios ? Configuración completa
- [x] Libros ? Configuración completa
- [x] Ventas ? Configuración completa
- [x] Categorias ? Configuración completa
- [x] DetallesVenta ? Configuración completa

### ? Tablas PARCIALES (1)
- [x] CarritoCompras ? Ignorar CreatedAt, UpdatedAt

### ? Tablas MÍNIMAS (2)
- [x] DetallesCarrito ? Ignorar CreatedAt, UpdatedAt, IsDeleted
- [x] AuditoriasInventario ? Ignorar CreatedAt, UpdatedAt, IsDeleted

---

## ?? MIGRACIÓN FUTURA (Opcional)

### Opción 1: Normalizar Completamente
```sql
-- Agregar columnas faltantes
ALTER TABLE CarritoCompras ADD CreatedAt DATETIME2(7);
ALTER TABLE CarritoCompras ADD UpdatedAt DATETIME2(7);

ALTER TABLE DetallesCarrito ADD CreatedAt DATETIME2(7);
ALTER TABLE DetallesCarrito ADD UpdatedAt DATETIME2(7);
ALTER TABLE DetallesCarrito ADD IsDeleted BIT DEFAULT 0;

ALTER TABLE AuditoriasInventario ADD CreatedAt DATETIME2(7);
ALTER TABLE AuditoriasInventario ADD UpdatedAt DATETIME2(7);
ALTER TABLE AuditoriasInventario ADD IsDeleted BIT DEFAULT 0;
```

**Ventajas:**
- Consistencia total
- Simplifica configuraciones EF Core
- Patrón uniforme

**Desventajas:**
- Redundancia (ej: CarritoCompras.FechaCreacion vs CreatedAt)
- Complejidad en migración de datos existentes
- Espacio adicional en disco

---

### Opción 2: Mantener Diseño Actual (Recomendado)
```
? Mantener estructura actual
? Configurar EF Core según cada caso
? Documentar decisiones de diseño
```

**Ventajas:**
- Sin cambios en BD
- Diseño optimizado por caso de uso
- Sin redundancia

**Desventajas:**
- Configuraciones más complejas
- Requiere documentación clara

---

## ?? RESULTADO

```
??????????????????????????????????????????????????????????????????
?                                                                ?
?          ?? MATRIZ COMPLETA VERIFICADA                        ?
?                                                                ?
?  ? 5 tablas con auditoría COMPLETA                           ?
?  ? 1 tabla con auditoría PARCIAL                             ?
?  ? 2 tablas con auditoría MÍNIMA                             ?
?                                                                ?
?  ?? Configuraciones EF Core actualizadas                      ?
?  ?? Patrones identificados y documentados                     ?
?  ? Compilación exitosa                                       ?
?                                                                ?
??????????????????????????????????????????????????????????????????
```

---

**Fecha:** 2025-01-09  
**Método de Verificación:** Manual en Base de Datos  
**Estado:** ? Documentado y Configurado
