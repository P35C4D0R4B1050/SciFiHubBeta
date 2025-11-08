# ?? GUÍA DE EJECUCIÓN RÁPIDA - Corrección de Errores Críticos

## ?? IMPORTANTE: LEER ANTES DE EJECUTAR

Esta guía corrige los siguientes errores:
1. ? "Invalid column name 'CreatedBy', 'UpdatedBy', 'DeletedAt'"
2. ? No se pueden crear/editar usuarios
3. ? Error al cargar categorías
4. ? Error tipográfico en CrearVenta.cshtml

---

## ?? CHECKLIST RÁPIDO

### ? PASO 1: DETENER LA APLICACIÓN (si está corriendo)
```bash
# Presionar Ctrl+C en la terminal donde corre la aplicación
```

---

### ? PASO 2: EJECUTAR SCRIPT SQL

#### **Opción A: Desde SQL Server Management Studio (SSMS)** ? RECOMENDADO
1. Abrir SSMS
2. Conectar a tu instancia de SQL Server (generalmente `localhost` o `.\SQLEXPRESS`)
3. Abrir el archivo:
   ```
   Database/FixColumnasBaseEntity.sql
   ```
4. Asegurarte de que la BD seleccionada es `SciFiHubDB`
5. Presionar **F5** o click en **Ejecutar**
6. Verificar en los mensajes que dice:
   ```
   ? Columna CreatedBy agregada a Usuarios
   ? Columna UpdatedBy agregada a Usuarios
   ? Columna DeletedAt agregada a Usuarios
   ... (y así para todas las tablas)
   === CORRECCIÓN COMPLETADA ===
   ```

#### **Opción B: Desde Terminal/CMD**
```bash
# Navegar a la carpeta del proyecto
cd E:\Proyecto\SciFiHub\SciFiHub

# Ejecutar el script
sqlcmd -S localhost -d SciFiHubDB -E -i Database\FixColumnasBaseEntity.sql

# O si usas SQL Server Authentication:
sqlcmd -S localhost -d SciFiHubDB -U sa -P tu_password -i Database\FixColumnasBaseEntity.sql
```

#### **Opción C: Desde PowerShell**
```powershell
# Navegar a la carpeta del proyecto
cd E:\Proyecto\SciFiHub\SciFiHub

# Ejecutar el script
Invoke-Sqlcmd -ServerInstance "localhost" -Database "SciFiHubDB" -InputFile "Database\FixColumnasBaseEntity.sql"
```

---

### ? PASO 3: VERIFICAR QUE EL SCRIPT SE EJECUTÓ CORRECTAMENTE

Ejecutar esta query en SSMS o tu herramienta SQL:

```sql
USE SciFiHubDB;
GO

-- Debe devolver 24 filas (3 columnas × 8 tablas)
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

**Resultado Esperado**: 24 filas

Si ves menos de 24 filas, algunas columnas no se agregaron. Ejecuta el script nuevamente.

---

### ? PASO 4: LIMPIAR Y RECOMPILAR LA APLICACIÓN

```bash
# Navegar a la carpeta del proyecto (si no estás ya ahí)
cd E:\Proyecto\SciFiHub\SciFiHub

# Limpiar solución
dotnet clean

# Recompilar
dotnet build

# Debe decir: Build succeeded. 0 Warning(s). 0 Error(s).
```

---

### ? PASO 5: EJECUTAR LA APLICACIÓN

```bash
dotnet run
```

Esperar a que aparezca:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:XXXX
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

---

### ? PASO 6: PROBAR FUNCIONALIDADES

#### **A. Probar Carga de Categorías**:
1. Abrir navegador: `https://localhost:XXXX` (usar el puerto que aparece)
2. Login como Administrador
3. Ir a: **Admin ? Inventario** (o Libros)
4. ? NO debe aparecer error "Invalid column name"
5. ? Debe cargar la lista de libros
6. Click en **"Agregar Libro"**
7. ? El select de Categoría debe tener opciones

#### **B. Probar Creación de Usuario**:
1. Ir a: **Admin ? Usuarios**
2. Click en **"Crear Usuario"**
3. Abrir **DevTools** (F12) ? Pestaña **Console**
4. Completar formulario:
   - **Nombre**: Test Usuario
   - **Email**: test@scifihub.com
   - **Username**: testuser
   - **Password**: Test123!
   - **Confirmar Password**: Test123!
   - **Rol**: Cliente
5. Click en **"Guardar Usuario"**
6. ? Verificar en Console:
   - Debe aparecer: `Enviando usuario:`
   - Debe aparecer: `Respuesta del servidor:`
   - Debe aparecer: `? Usuario creado exitosamente`
7. ? Debe aparecer el nuevo usuario en la tabla

#### **C. Probar Registro Rápido en Ventas**:
1. Ir a: **Vendedor ? CrearVenta** (o Admin ? Ventas ? Nueva Venta)
2. Click en **"Nuevo Cliente"**
3. Completar:
   - **Nombre**: Cliente Test
   - **Email**: cliente@test.com
   - **Teléfono**: 999888777
4. Click en **"Registrar Cliente"**
5. ? Debe aparecer alert con credenciales
6. ? Cliente debe quedar seleccionado

---

## ?? SOLUCIÓN DE PROBLEMAS

### ? Error: "El script no se ejecuta"

**Causa**: No tienes permisos o la BD no existe

**Solución**:
```sql
-- Verificar que la BD existe
SELECT name FROM sys.databases WHERE name = 'SciFiHubDB';

-- Si no existe, crearla
CREATE DATABASE SciFiHubDB;
GO

-- Ejecutar migraciones
cd E:\Proyecto\SciFiHub\SciFiHub
dotnet ef database update
```

---

### ? Error: "Compilation failed"

**Causa**: Hay errores de sintaxis

**Solución**:
```bash
# Ver detalles del error
dotnet build --verbosity detailed

# Limpiar y recompilar
dotnet clean
dotnet build
```

---

### ? Error: "Connection refused"

**Causa**: SQL Server no está corriendo

**Solución**:
1. Abrir "Servicios" (services.msc)
2. Buscar "SQL Server (MSSQLSERVER)" o "SQL Server (SQLEXPRESS)"
3. Si está detenido, click derecho ? Iniciar
4. Intentar nuevamente

---

### ? Error: "Login failed for user"

**Causa**: Credenciales incorrectas en connection string

**Solución**:
1. Abrir `appsettings.json`
2. Verificar la connection string:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=SciFiHubDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```
3. Si usas usuario/contraseña:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=SciFiHubDB;User Id=sa;Password=tu_password;TrustServerCertificate=True;"
  }
}
```

---

## ? VERIFICACIÓN FINAL

Ejecutar esta query para confirmar que todo está bien:

```sql
USE SciFiHubDB;
GO

-- 1. Verificar columnas agregadas
PRINT '=== COLUMNAS AGREGADAS ===';
SELECT 
    t.name AS Tabla,
    COUNT(*) AS Total_Columnas_Agregadas
FROM sys.tables t
JOIN sys.columns c ON t.object_id = c.object_id
WHERE c.name IN ('CreatedBy', 'UpdatedBy', 'DeletedAt')
AND t.name IN ('Usuarios', 'Categorias', 'Libros', 'Ventas', 'DetallesVenta', 
               'CarritoCompras', 'DetallesCarrito', 'AuditoriasInventario')
GROUP BY t.name
ORDER BY t.name;

-- Debe mostrar:
-- AuditoriasInventario: 3
-- CarritoCompras: 3
-- Categorias: 3
-- DetallesCarrito: 3
-- DetallesVenta: 3
-- Libros: 3
-- Usuarios: 3
-- Ventas: 3

-- 2. Verificar categorías activas
PRINT '';
PRINT '=== CATEGORÍAS ACTIVAS ===';
SELECT Id, Nombre, Estado, IsDeleted
FROM Categorias
WHERE IsDeleted = 0 AND Estado = 'Activa';

-- Debe mostrar al menos 1 categoría

-- 3. Verificar usuarios
PRINT '';
PRINT '=== USUARIOS RECIENTES ===';
SELECT TOP 5
    NombreCompleto,
    Email,
    Username,
    Rol,
    Estado,
    CreatedAt
FROM Usuarios
WHERE IsDeleted = 0
ORDER BY CreatedAt DESC;

-- Debe mostrar tus usuarios
```

---

## ?? RESULTADO ESPERADO

Después de seguir todos los pasos:

? **Aplicación**:
- Compila sin errores
- Corre sin errores
- No hay logs de error en consola

? **Categorías**:
- Se cargan correctamente
- Aparecen en el select de Agregar Libro
- No hay error "Invalid column name"

? **Usuarios**:
- Se pueden crear
- Se pueden editar
- Se pueden eliminar
- Se guardan en la BD

? **Ventas**:
- Se puede crear venta
- Se puede registrar cliente rápido
- Se guardan correctamente

---

## ?? SI TODO FUNCIONA

¡Felicidades! ?? 

Las correcciones se aplicaron exitosamente.

Ahora puedes:
1. Continuar desarrollando
2. Agregar más funcionalidades
3. Probar el sistema completamente

---

## ?? SI ALGO NO FUNCIONA

1. **Copiar el mensaje de error exacto**
2. **Revisar los logs** en la consola de la aplicación
3. **Ejecutar las queries de diagnóstico** del documento `SOLUCION_PROBLEMAS_CRITICOS.md`
4. **Reportar el error** con el mensaje completo y los logs

---

**Tiempo Estimado**: 10-15 minutos  
**Dificultad**: Baja  
**Estado**: ? Instrucciones Completas y Verificadas
