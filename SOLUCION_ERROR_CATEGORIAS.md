# ? SOLUCIÓN RÁPIDA - Error de Categorías Duplicadas

## ?? PROBLEMA

Error al ejecutar `PreparacionPruebas.sql`:
```
Violation of UNIQUE KEY constraint 'UK_Categorias_Nombre'. 
Cannot insert duplicate key in object 'dbo.Categorias'. 
The duplicate key value is (Ciencia Ficción).
```

**Causa**: Las categorías ya existen en la BD (posiblemente eliminadas con soft delete).

---

## ? SOLUCIÓN 1: Ejecutar Script de Limpieza (30 segundos)

### **Paso 1: Ejecutar script de limpieza**

```sql
-- En SSMS, ejecutar:
Database/LimpiezaCategorias.sql
```

**Este script**:
- ? Restaura categorías eliminadas (IsDeleted = 1 ? 0)
- ? Activa categorías inactivas
- ? Crea solo las categorías que faltan
- ? Verifica duplicados
- ? Muestra resumen

### **Paso 2: Re-ejecutar PreparacionPruebas**

```sql
-- Ahora ejecutar:
Database/PreparacionPruebas.sql
```

**Resultado esperado**:
```
? Ya existen categorías activas
Categorías disponibles:
(Listado de 8 categorías)
```

---

## ? SOLUCIÓN 2: SQL Directo (10 segundos)

Si solo quieres restaurar categorías eliminadas:

```sql
USE SciFiHubDB;
GO

-- Restaurar categorías eliminadas
UPDATE Categorias 
SET IsDeleted = 0, 
    DeletedAt = NULL,
    Estado = 'Activa',
    UpdatedAt = GETUTCDATE()
WHERE IsDeleted = 1;

-- Verificar
SELECT COUNT(*) AS Categorias_Activas
FROM Categorias 
WHERE IsDeleted = 0 AND Estado = 'Activa';
```

---

## ? SOLUCIÓN 3: Verificar Estado Actual

Ver qué categorías existen:

```sql
USE SciFiHubDB;
GO

-- Ver TODAS las categorías (incluso eliminadas)
SELECT 
    Nombre,
    Estado,
    IsDeleted,
    CASE 
        WHEN IsDeleted = 1 THEN '? Eliminada'
        WHEN Estado = 'Activa' THEN '? Activa'
        WHEN Estado = 'Inactiva' THEN '?? Inactiva'
        ELSE Estado
    END AS EstadoActual
FROM Categorias
ORDER BY IsDeleted, Nombre;
```

**Interpretación**:
- Si `IsDeleted = 1`: Categoría en papelera ? Ejecutar Solución 2
- Si `Estado = 'Inactiva'`: Categoría desactivada ? Activar
- Si `IsDeleted = 0` y `Estado = 'Activa'`: Todo OK ? No hacer nada

---

## ?? SI LAS SOLUCIONES ANTERIORES NO FUNCIONAN

### **Opción A: Eliminar y Recrear Categorías** (CUIDADO)

?? **ADVERTENCIA**: Esto eliminará TODAS las categorías y sus relaciones.

```sql
USE SciFiHubDB;
GO

-- Solo ejecutar si NO hay libros asociados
DELETE FROM Categorias;

-- Luego ejecutar:
Database/PreparacionPruebas.sql
```

### **Opción B: Ver Constraint Exacto**

```sql
-- Ver el constraint que está fallando
SELECT 
    i.name AS IndiceNombre,
    COL_NAME(ic.object_id, ic.column_id) AS ColumnaNombre
FROM sys.indexes i
INNER JOIN sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
WHERE i.object_id = OBJECT_ID('Categorias')
AND i.name = 'UK_Categorias_Nombre';
```

---

## ?? VERIFICACIÓN POST-FIX

Después de aplicar cualquier solución:

```sql
-- 1. Contar categorías activas
SELECT COUNT(*) AS Total_Activas 
FROM Categorias 
WHERE IsDeleted = 0 AND Estado = 'Activa';
-- Debe ser >= 8

-- 2. Ver listado
SELECT Nombre, Estado, IsDeleted 
FROM Categorias 
WHERE IsDeleted = 0 
ORDER BY Nombre;
-- Debe mostrar al menos 8 categorías

-- 3. Verificar duplicados
SELECT Nombre, COUNT(*) AS Cantidad
FROM Categorias
WHERE IsDeleted = 0
GROUP BY Nombre
HAVING COUNT(*) > 1;
-- NO debe devolver ninguna fila
```

---

## ?? DESPUÉS DE CORREGIR

### **Paso 1: Ejecutar PreparacionPruebas.sql**

Debe completarse sin errores:
```
? Ya existen categorías activas
? Usuario vendedor.test ya existe
? Usuario admin.test ya existe
??? VERIFICACIÓN COMPLETADA ???
```

### **Paso 2: Iniciar Aplicación**

```bash
dotnet clean
dotnet build
dotnet run
```

### **Paso 3: Probar Categorías**

1. Login con `vendedor.test` / `Test123!`
2. Ir a Inventario
3. Click en "Agregar Libro"
4. Verificar que el select de Categoría tiene opciones

---

## ?? ARCHIVOS

| Archivo | Propósito | Cuándo Usar |
|---------|-----------|-------------|
| `Database/LimpiezaCategorias.sql` | Limpia y restaura categorías | ? **USAR PRIMERO** |
| `Database/PreparacionPruebas.sql` | Preparación completa | Después de limpieza |

---

## ?? PREVENCIÓN FUTURA

El script `PreparacionPruebas.sql` ya fue corregido para:
- ? Verificar existencia antes de insertar
- ? Restaurar categorías eliminadas
- ? No intentar insertar duplicados

**Ahora es seguro ejecutarlo múltiples veces.**

---

**Tiempo Estimado**: 30 segundos  
**Dificultad**: Baja  
**Estado**: ? Scripts Corregidos y Listos
