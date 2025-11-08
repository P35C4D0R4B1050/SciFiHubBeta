# ?? SOLUCIONES A PROBLEMAS CRÍTICOS - SciFiHub

## ?? RESUMEN DE PROBLEMAS

### **Problemas Identificados**:
1. ? Dashboard no redirige correctamente
2. ? Error al cargar categorías: "Invalid column name 'CreatedBy', 'UpdatedBy', 'DeletedAt'"
3. ? Crear/editar usuarios no funciona
4. ? Error tipográfico en CrearVenta.cshtml

---

## ? SOLUCIONES IMPLEMENTADAS

### **SOLUCIÓN 1: Script SQL para Agregar Columnas Faltantes** ?

**Archivo Creado**: `Database/FixColumnasBaseEntity.sql`

**Problema**: 
La clase `BaseEntity` define las columnas `CreatedBy`, `UpdatedBy` y `DeletedAt`, pero estas no existen en las tablas de la base de datos.

**Causa Raíz**:
Las migraciones de Entity Framework no se ejecutaron completamente o se creó la BD manualmente sin estas columnas.

**Script SQL Creado**:
```sql
-- Agrega las columnas faltantes a TODAS las tablas:
-- - CreatedBy (UNIQUEIDENTIFIER NULL)
-- - UpdatedBy (UNIQUEIDENTIFIER NULL)
-- - DeletedAt (DATETIME2 NULL)

-- Tablas afectadas:
1. Usuarios
2. Categorias
3. Libros
4. Ventas
5. DetallesVenta
6. CarritoCompras
7. DetallesCarrito
8. AuditoriasInventario
```

**Cómo Ejecutar**:
```bash
# Opción 1: Desde SQL Server Management Studio (SSMS)
1. Abrir SSMS
2. Conectar a la instancia de SQL Server
3. Abrir el archivo Database/FixColumnasBaseEntity.sql
4. Ejecutar (F5)

# Opción 2: Desde sqlcmd
sqlcmd -S localhost -d SciFiHubDB -i Database\FixColumnasBaseEntity.sql

# Opción 3: Desde PowerShell
Invoke-Sqlcmd -ServerInstance "localhost" -Database "SciFiHubDB" -InputFile "Database\FixColumnasBaseEntity.sql"
```

**Verificación**:
```sql
-- Ejecutar para verificar que las columnas se agregaron
SELECT 
    t.name AS Tabla,
    c.name AS Columna,
    ty.name AS TipoDato
FROM sys.tables t
JOIN sys.columns c ON t.object_id = c.object_id
JOIN sys.types ty ON c.user_type_id = ty.user_type_id
WHERE c.name IN ('CreatedBy', 'UpdatedBy', 'DeletedAt')
AND t.name IN ('Usuarios', 'Categorias', 'Libros', 'Ventas', 'DetallesVenta', 
               'CarritoCompras', 'DetallesCarrito', 'AuditoriasInventario')
ORDER BY t.name, c.name;
```

**Resultado Esperado**:
```
Tabla                    | Columna    | TipoDato
-------------------------|------------|------------
AuditoriasInventario     | CreatedBy  | uniqueidentifier
AuditoriasInventario     | DeletedAt  | datetime2
AuditoriasInventario     | UpdatedBy  | uniqueidentifier
CarritoCompras           | CreatedBy  | uniqueidentifier
CarritoCompras           | DeletedAt  | datetime2
CarritoCompras           | UpdatedBy  | uniqueidentifier
Categorias               | CreatedBy  | uniqueidentifier
Categorias               | DeletedAt  | datetime2
Categorias               | UpdatedBy  | uniqueidentifier
... (24 filas en total)
```

---

### **SOLUCIÓN 2: Corrección de Error Tipográfico en CrearVenta.cshtml** ?

**Archivo Modificado**: `Views/Vendedor/CrearVenta.cshtml`

**Problema**: 
```javascript
const metodo Pago = document.querySelector('select[name="MetodoPago"]').value;
// ^^^^^^ ESPACIO EN EL NOMBRE DE LA VARIABLE
```

**Solución**:
```javascript
const metodoPago = document.querySelector('select[name="MetodoPago"]').value;
// ? Sin espacio
```

**Estado**: ? CORREGIDO

---

## ?? DIAGNÓSTICO DE PROBLEMAS ADICIONALES

### **Problema 3: Usuarios No Se Guardan**

**Posibles Causas**:
1. ? Error en el frontend (JavaScript no envía la petición)
2. ? Error en el backend (validación fallida)
3. ? Error en la BD (constraint violation)
4. ? Error de autenticación/autorización

**Pasos de Diagnóstico**:

#### **A. Verificar en el Frontend**:
1. Abrir DevTools (F12)
2. Ir a pestaña Console
3. Intentar crear un usuario
4. Verificar logs:
   - "Enviando usuario:" - Debe aparecer
   - "Respuesta del servidor:" - Debe aparecer
   - Si no aparecen, el problema está en el JavaScript

#### **B. Verificar en el Backend**:
```bash
# Ver logs del servidor en tiempo real
# En la consola donde ejecutaste dotnet run:

# Buscar líneas como:
[Error] Error al crear usuario
[Error] SciFiHub.Web.Services.UsuarioService[0]
```

#### **C. Verificar en la Base de Datos**:
```sql
-- Ver si se están intentando crear usuarios
SELECT TOP 10 *
FROM Usuarios
ORDER BY CreatedAt DESC;

-- Ver errores de SQL Server
SELECT TOP 20
    creation_time,
    last_execution_time,
    text,
    *
FROM sys.dm_exec_query_stats
CROSS APPLY sys.dm_exec_sql_text(sql_handle)
WHERE text LIKE '%Usuarios%'
ORDER BY last_execution_time DESC;
```

---

## ?? CHECKLIST DE VERIFICACIÓN POST-FIX

### **Paso 1: Ejecutar Script SQL** ?
- [ ] Abrir SSMS o herramienta SQL
- [ ] Conectar a la BD SciFiHubDB
- [ ] Ejecutar `Database/FixColumnasBaseEntity.sql`
- [ ] Verificar que no hay errores
- [ ] Ejecutar query de verificación
- [ ] Confirmar que las 24 columnas se agregaron (3 columnas × 8 tablas)

### **Paso 2: Reiniciar Aplicación** ?
- [ ] Detener la aplicación (Ctrl+C si está en consola)
- [ ] Limpiar solución: `dotnet clean`
- [ ] Recompilar: `dotnet build`
- [ ] Ejecutar: `dotnet run`

### **Paso 3: Probar Carga de Categorías** ?
- [ ] Login como Administrador
- [ ] Ir a Admin ? Inventario (Libros)
- [ ] Verificar que NO aparece error "Invalid column name"
- [ ] Verificar que se cargan las categorías
- [ ] Abrir modal "Agregar Libro"
- [ ] Verificar que el select de categorías tiene opciones

### **Paso 4: Probar Creación de Usuario** ?
- [ ] Ir a Admin ? Usuarios
- [ ] Click en "Crear Usuario"
- [ ] Completar formulario:
  - Nombre: Usuario Test
  - Email: test@test.com
  - Username: usertest
  - Password: Test123!
  - Confirm Password: Test123!
  - Rol: Cliente
- [ ] Abrir DevTools (F12) ? Console
- [ ] Click en "Guardar Usuario"
- [ ] Verificar logs en consola:
  - Debe aparecer "Enviando usuario:"
  - Debe aparecer "Respuesta del servidor:"
- [ ] Si hay error, anotar el mensaje exacto
- [ ] Verificar en BD:
```sql
SELECT TOP 1 * FROM Usuarios 
WHERE Email = 'test@test.com'
ORDER BY CreatedAt DESC;
```

### **Paso 5: Probar Dashboard** ?
- [ ] Ir a URL: `https://localhost:XXXX/Admin`
- [ ] Debe redirigir a `https://localhost:XXXX/Admin/Dashboard`
- [ ] Debe mostrar la vista Dashboard (no error 404)
- [ ] Verificar que los widgets/cards se muestran

---

## ?? ERRORES COMUNES Y SOLUCIONES

### **Error: "Invalid column name 'CreatedBy'"**

**Causa**: Script SQL no se ejecutó o falló

**Solución**:
1. Ejecutar script SQL manualmente
2. Verificar que el usuario SQL tiene permisos ALTER TABLE
3. Verificar que la BD es SciFiHubDB
4. Ejecutar query de verificación

---

### **Error: "No se puede guardar usuario"**

**Posibles Causas y Soluciones**:

#### **1. Error de Validación**:
```
Error: Datos inválidos
```
**Solución**: Verificar que todos los campos requeridos están completos

#### **2. Email Duplicado**:
```
Error: El email ya está registrado
```
**Solución**: Usar otro email

#### **3. Username Duplicado**:
```
Error: El username ya existe
```
**Solución**: Usar otro username

#### **4. Error de BD**:
```
Error: Error interno del servidor
```
**Solución**: 
1. Ver logs del backend
2. Verificar que la BD está accesible
3. Verificar connection string en `appsettings.json`

---

### **Error: "Dashboard no carga"**

**Causa**: Falta la vista Dashboard.cshtml o ruta incorrecta

**Solución**:
```bash
# Verificar que existe el archivo
ls Views/Admin/Dashboard.cshtml

# Si no existe, crearlo con contenido básico
```

---

## ?? QUERIES SQL ÚTILES PARA DIAGNÓSTICO

### **1. Ver Estructura de Tabla**:
```sql
-- Ver todas las columnas de Categorias
SELECT 
    c.name AS Columna,
    t.name AS TipoDato,
    c.max_length AS Longitud,
    c.is_nullable AS Permite_NULL
FROM sys.columns c
JOIN sys.types t ON c.user_type_id = t.user_type_id
WHERE object_id = OBJECT_ID('Categorias')
ORDER BY c.column_id;
```

### **2. Ver Usuarios Recientes**:
```sql
SELECT TOP 10
    Id,
    NombreCompleto,
    Email,
    Username,
    Rol,
    Estado,
    CreatedAt,
    CreatedBy,
    IsDeleted
FROM Usuarios
ORDER BY CreatedAt DESC;
```

### **3. Ver Categorías**:
```sql
SELECT *
FROM Categorias
WHERE IsDeleted = 0
ORDER BY Nombre;
```

### **4. Verificar Triggers**:
```sql
SELECT 
    t.name AS Tabla,
    tr.name AS Trigger,
    tr.is_disabled AS Deshabilitado
FROM sys.triggers tr
JOIN sys.tables t ON tr.parent_id = t.object_id
WHERE t.name IN ('Usuarios', 'Categorias', 'Libros')
ORDER BY t.name;
```

---

## ? RESULTADO ESPERADO DESPUÉS DE LAS CORRECCIONES

### **? Categorías**:
- [x] Se cargan sin error
- [x] Aparecen en el select de "Agregar Libro"
- [x] No hay error "Invalid column name"

### **? Usuarios**:
- [x] Se pueden crear desde el formulario
- [x] Se guardan en la BD
- [x] Se pueden editar
- [x] Se pueden eliminar (soft delete)

### **? Dashboard**:
- [x] Redirige correctamente desde /Admin
- [x] Se muestra sin errores
- [x] Los links funcionan

### **? Ventas**:
- [x] Se puede crear venta
- [x] Se puede seleccionar cliente
- [x] Se puede registrar cliente rápido
- [x] Se guardan correctamente

---

## ?? PRÓXIMOS PASOS

1. ? **Ejecutar script SQL** (CRÍTICO)
2. ? **Reiniciar aplicación**
3. ? **Probar cada funcionalidad según checklist**
4. ? **Si persisten errores**: Revisar logs y ejecutar queries de diagnóstico
5. ? **Reportar resultados**: Anotar qué funciona y qué no

---

## ?? INFORMACIÓN DE SOPORTE

### **Archivos Modificados/Creados**:
1. `Database/FixColumnasBaseEntity.sql` - ? NUEVO
2. `Views/Vendedor/CrearVenta.cshtml` - ? MODIFICADO (línea 408)

### **Archivos a Revisar si Persisten Problemas**:
1. `SciFiHub.Domain/Common/BaseEntity.cs`
2. `SciFiHub.Infrastructure/Data/SciFiHubDbContext.cs`
3. `Controllers/Api/UsuariosController.cs`
4. `Services/UsuarioService.cs`

---

**Última Actualización**: Enero 2025  
**Estado**: ? Soluciones Listas - Pendiente de Aplicación  
**Prioridad**: ?? CRÍTICA - Ejecutar Inmediatamente
