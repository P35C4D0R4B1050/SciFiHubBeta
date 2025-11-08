# ?? SOLUCIÓN DEFINITIVA - Error de Columnas Persiste

## ?? PROBLEMA

El error persiste **AÚN DESPUÉS** de ejecutar el script SQL y verificar que las columnas existen.

**Causa:**
- ? Columnas **SÍ EXISTEN** en la base de datos (verificado con query)
- ? EF Core tiene **CACHÉ** del modelo anterior
- ? Archivos compilados (bin/obj) tienen **metadata antigua**

---

## ? SOLUCIÓN EN 5 PASOS (10 MINUTOS)

### ?? PASO 1: Verificar Columnas en BD

**Ejecutar:** `Database/VerificacionColumnas.sql` en SSMS

**Resultado esperado:**
```
? VENTAS: Todas las columnas existen (7/7)
? DETALLESVENTA: Todas las columnas existen (7/7)
? Test de inserción: EXITOSO
```

**Si falla:** Volver a ejecutar `Database/FixColumnasVentas.sql`

---

### ?? PASO 2: Detener Aplicación

```bash
# Presionar Ctrl+C en la terminal donde corre dotnet run
# O cerrar completamente Visual Studio
```

---

### ?? PASO 3: Limpiar Cache (CRÍTICO)

**Opción A - Usando PowerShell (RECOMENDADO):**

```powershell
# Ejecutar en PowerShell como Administrador
cd E:\Proyecto\SciFiHub\SciFiHub
.\LimpiarProyecto.ps1
```

**Opción B - Manual:**

```bash
cd E:\Proyecto\SciFiHub\SciFiHub

# 1. Eliminar carpetas bin/ y obj/ MANUALMENTE
# Ir a cada proyecto y borrar:
#   - SciFiHub\bin\
#   - SciFiHub\obj\
#   - SciFiHub.Infrastructure\bin\
#   - SciFiHub.Infrastructure\obj\
#   - SciFiHub.Domain\bin\
#   - SciFiHub.Domain\obj\

# 2. Ejecutar clean
dotnet clean

# 3. Restaurar
dotnet restore
```

---

### ?? PASO 4: Recompilar

```bash
cd E:\Proyecto\SciFiHub\SciFiHub
dotnet build
```

**Resultado esperado:**
```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

---

### ?? PASO 5: Ejecutar y Probar

```bash
dotnet run
```

**Luego:**
1. Login como Admin/Vendedor
2. Ir a "Nueva Venta"
3. Agregar libros
4. Registrar venta

**? Resultado esperado:**
```
? Venta registrada exitosamente
Número de venta: V20250109-0001
```

---

## ?? VERIFICACIÓN ADICIONAL

### Si PERSISTE el error después de los 5 pasos:

**Ejecutar en SQL Server:**

```sql
-- Verificar que las columnas REALMENTE existen
USE SciFiHubDB;

SELECT 
    TABLE_NAME,
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME IN ('Ventas', 'DetallesVenta')
AND COLUMN_NAME IN ('Id', 'CreatedAt', 'UpdatedAt', 'IsDeleted')
ORDER BY TABLE_NAME, COLUMN_NAME;

-- Debe retornar 8 filas (4 columnas × 2 tablas)
```

**Verificar en logs de aplicación:**

```bash
# Buscar en la consola:
>> Error: Invalid column name 'CreatedAt'

# Si aparece, significa que:
# - La conexión apunta a BD incorrecta
# - O el cache no se limpió correctamente
```

---

## ?? SOLUCIÓN ALTERNATIVA (Si nada funciona)

### Recrear el DbContext:

**Eliminar caché de EF Core:**

```bash
# PowerShell
Remove-Item -Recurse -Force "$env:USERPROFILE\.dotnet\store\*efcore*"
Remove-Item -Recurse -Force "$env:LOCALAPPDATA\NuGet\Cache\*efcore*"
```

**Luego:**

```bash
cd E:\Proyecto\SciFiHub\SciFiHub
dotnet clean
dotnet restore --force-evaluate
dotnet build
dotnet run
```

---

## ?? DIAGNÓSTICO

### ¿Por qué pasa esto?

**EF Core mantiene caché en múltiples lugares:**

| Ubicación | Qué Guarda | Solución |
|-----------|------------|----------|
| **bin/** | Ensamblados compilados | `dotnet clean` |
| **obj/** | Archivos intermedios | Eliminar manualmente |
| **NuGet Cache** | Paquetes | `dotnet restore --force-evaluate` |
| **EF Core Model Cache** | Modelo de BD | Recompilar desde cero |

**Cuando agregas columnas a BD:**
1. ? BD se actualiza inmediatamente
2. ? EF Core sigue usando modelo viejo de caché
3. ? Intenta insertar en columnas "que no existen" (según caché)
4. ? SQL Server dice: "Esa columna SÍ existe, pero tu INSERT es incorrecto"

---

## ? CHECKLIST DE VERIFICACIÓN

- [ ] ? Script `VerificacionColumnas.sql` ejecutado
- [ ] ? Muestra "7/7" para Ventas y DetallesVenta
- [ ] ? Test de inserción exitoso
- [ ] ? Aplicación detenida completamente
- [ ] ? Carpetas bin/ y obj/ eliminadas
- [ ] ? `dotnet clean` ejecutado
- [ ] ? `dotnet restore` ejecutado
- [ ] ? `dotnet build` exitoso (0 errores)
- [ ] ? `dotnet run` iniciado
- [ ] ? Venta registrada sin errores

---

## ?? SIGUIENTE PASO CRÍTICO

**DESPUÉS de limpiar y recompilar:**

```bash
# 1. Ejecutar
dotnet run

# 2. Esperar mensaje
info: Now listening on: https://localhost:7116

# 3. Ver logs en consola
# Buscar líneas que digan:
>> Iniciando registro de venta...
>> Cliente seleccionado: [...]
>> Enviando venta...

# 4. Si aparece:
>> ? Venta registrada exitosamente
# ENTONCES: ? PROBLEMA RESUELTO

# 5. Si aparece:
>> ? Invalid column name 'CreatedAt'
# ENTONCES: Ejecutar "SOLUCIÓN ALTERNATIVA" arriba
```

---

## ?? ÚLTIMA OPCIÓN

**Si NADA de lo anterior funciona:**

**Verificar conexión a BD:**

```json
// appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=SciFiHubDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }
}
```

**Ejecutar query directa:**

```sql
-- En SSMS, cambiar a SciFiHubDB
USE SciFiHubDB;
SELECT @@SERVERNAME AS Servidor, DB_NAME() AS BaseDatos;

-- Debe retornar:
-- Servidor: TU_SERVIDOR
-- BaseDatos: SciFiHubDB
```

---

**Versión:** 1.0  
**Fecha:** 2025-01-09  
**Estado:** ?? CRÍTICO - Requiere limpieza de caché  
**Tiempo Estimado:** 10 minutos
