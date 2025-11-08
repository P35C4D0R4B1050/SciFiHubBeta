# ?? GUÍA DE PRUEBAS - Verificación de Correcciones

## ?? ORDEN DE EJECUCIÓN

1. Ejecutar script SQL (5 min)
2. Reiniciar aplicación (2 min)
3. Pruebas como Administrador (15 min)
4. Pruebas como Vendedor (10 min)

**Tiempo Total Estimado:** 30-35 minutos

---

## ??? PASO 1: Ejecutar Script SQL (OPCIONAL pero RECOMENDADO)

### Objetivo:
Optimizar la base de datos para transacciones concurrentes

### Instrucciones:

1. **Abrir SQL Server Management Studio (SSMS)**
2. **Conectar a:** `localhost` (o tu servidor)
3. **Abrir archivo:** `Database/DiagnosticoTransacciones.sql`
4. **Ejecutar:** Presionar F5

### Resultado Esperado:

```
? READ_COMMITTED_SNAPSHOT: HABILITADO (CORRECTO)
   Permite transacciones concurrentes sin bloqueos de lectura

MODELO DE RECUPERACIÓN: SIMPLE
? Apropiado para desarrollo
```

### ? Si aparece error:

```sql
-- Ejecutar manualmente:
USE master;
GO

ALTER DATABASE SciFiHubDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
ALTER DATABASE SciFiHubDB SET READ_COMMITTED_SNAPSHOT ON;
ALTER DATABASE SciFiHubDB SET MULTI_USER;
GO
```

---

## ?? PASO 2: Reiniciar Aplicación

```bash
# Navegar al proyecto
cd E:\Proyecto\SciFiHub\SciFiHub

# Limpiar y compilar
dotnet clean
dotnet build

# Ejecutar
dotnet run
```

### Resultado Esperado:

```
? Build succeeded.
    0 Warning(s)
    0 Error(s)

info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7116
```

---

## ????? PASO 3: Pruebas como ADMINISTRADOR

### ?? TEST 1: Registrar Venta

**Objetivo:** Verificar que las ventas se registran sin error de transacciones

**Pasos:**

1. **Login:**
   - URL: `https://localhost:7116/Auth/Login`
   - Usuario: `admin` (o tu usuario admin)
   - Contraseña: `Admin123!`

2. **Ir a Nueva Venta:**
   - Menú superior ? "Ventas"
   - Click en "Nueva Venta"

3. **Seleccionar Cliente:**
   - Buscar cliente existente
   - O crear nuevo cliente rápido

4. **Agregar Libros:**
   - Click "Agregar Libro"
   - Seleccionar libro del catálogo
   - Cantidad: 2
   - Click "Agregar"
   - Repetir con 1-2 libros más

5. **Completar Datos:**
   - Dirección de Envío: "Av. Principal 123, Lima"
   - Método de Pago: "Efectivo"
   - Notas (opcional): "Prueba de venta"

6. **Registrar:**
   - Click "Registrar Venta"

**? RESULTADO ESPERADO:**
```
? Venta registrada exitosamente
Número de venta: V20250109-0001
```

**? ANTES:**
```
? Error al registrar venta:
The configured execution strategy 'SqlServerRetryingExecutionStrategy' 
does not support user-initiated transactions...
```

**?? Verificar:**
- Número de venta generado
- Totales correctos (Subtotal, IGV, Total)
- Stock de libros actualizado
- Mensaje de éxito verde

---

### ?? TEST 2: Eliminar Definitivamente Usuario

**Objetivo:** Verificar hard delete sin error de Context

**Pasos:**

1. **Ir a Usuarios:**
   - Menú superior ? "Admin" ? "Usuarios"

2. **Crear Usuario de Prueba (si no existe):**
   - Click "Crear Usuario"
   - Nombre: "Usuario Prueba Eliminar"
   - Email: "eliminar@test.com"
   - Username: "eliminar123"
   - Contraseña: "Test123!"
   - Rol: "Cliente"
   - Guardar

3. **Eliminar Usuario (Soft Delete):**
   - Buscar "Usuario Prueba Eliminar"
   - Click botón ??? (Eliminar)
   - Confirmar

4. **Ver Usuarios Eliminados:**
   - Activar toggle ?? "Ver usuarios eliminados (Papelera)"

5. **Eliminar Definitivamente:**
   - Buscar "Usuario Prueba Eliminar" en tabla de eliminados
   - Click "Eliminar Definitivo" (botón rojo ???)
   - **Primera Confirmación:** Aceptar
   - **Segunda Confirmación:** Escribir exactamente: "Usuario Prueba Eliminar"
   - Enter

**? RESULTADO ESPERADO:**
```
? Usuario eliminado definitivamente de la base de datos
```

**? ANTES:**
```
? Error al eliminar definitivamente: 
'SciFiHub.Infrastructure.Repositories.UnitOfWork' does not contain 
a definition for 'Context'
```

**?? Verificar:**
- Usuario desaparece de tabla de eliminados
- Usuario NO existe en BD (query SQL):
  ```sql
  SELECT * FROM Usuarios WITH (NOLOCK) 
  WHERE Email = 'eliminar@test.com';
  -- Debe retornar 0 filas
  ```

---

### ?? TEST 3: Restaurar Usuario

**Objetivo:** Verificar que usuario restaurado aparece en tabla principal

**Pasos:**

1. **Crear Usuario de Prueba:**
   - Click "Crear Usuario"
   - Nombre: "Usuario Prueba Restaurar"
   - Email: "restaurar@test.com"
   - Username: "restaurar123"
   - Contraseña: "Test123!"
   - Rol: "Cliente"
   - Guardar

2. **Eliminar Usuario:**
   - Buscar "Usuario Prueba Restaurar"
   - Click ??? (Eliminar)
   - Confirmar

3. **Ir a Papelera:**
   - Activar toggle ?? "Ver usuarios eliminados"

4. **Restaurar Usuario:**
   - Buscar "Usuario Prueba Restaurar" en tabla de eliminados
   - Click "Restaurar" (botón verde ??)
   - Confirmar

**? RESULTADO ESPERADO:**
```
? Usuario restaurado exitosamente
(Página se recarga automáticamente)
```

**DESPUÉS DE RECARGA:**
- ? Usuario aparece en tabla de usuarios activos
- ? Estado: Activo
- ? IsDeleted = 0 en BD

**? ANTES:**
```
? Usuario restaurado exitosamente
(Usuario NO aparece en tabla principal)
(Solo visible cambiando toggle manualmente)
```

**?? Verificar:**
- Usuario visible en tabla principal
- Toggle "Ver eliminados" está desactivado
- Usuario tiene estado "Activo"

---

## ????? PASO 4: Pruebas como VENDEDOR

### ?? TEST 4: Registrar Venta (Vendedor)

**Objetivo:** Verificar que vendedores también pueden registrar ventas sin errores

**Pasos:**

1. **Cerrar Sesión:**
   - Menú usuario ? "Cerrar Sesión"

2. **Login como Vendedor:**
   - Usuario: `vendedor1` (o crear vendedor)
   - Contraseña: `Vendedor123!`

3. **Ir a Nueva Venta:**
   - Menú "Inventario" ? "Nueva Venta"

4. **Repetir pasos de TEST 1:**
   - Seleccionar cliente
   - Agregar libros
   - Completar dirección y método de pago
   - Registrar venta

**? RESULTADO ESPERADO:**
```
? Venta registrada exitosamente
```

**?? Verificar:**
- Venta se registra correctamente
- VendedorId asignado al usuario logueado
- Stock actualizado

---

## ?? RESUMEN DE VERIFICACIÓN

### Checklist de Pruebas:

- [ ] ? Script SQL ejecutado (opcional)
- [ ] ? Aplicación reiniciada
- [ ] ? TEST 1: Registrar venta como Admin
- [ ] ? TEST 2: Eliminar definitivamente usuario
- [ ] ? TEST 3: Restaurar usuario
- [ ] ? TEST 4: Registrar venta como Vendedor

### Resultados Esperados:

| Test | Problema Anterior | Estado Actual |
|------|-------------------|---------------|
| Registrar Venta (Admin) | ? Error de transacciones | ? Funcional |
| Eliminar Definitivamente | ? Error de Context | ? Funcional |
| Restaurar Usuario | ? No aparece en tabla | ? Funcional |
| Registrar Venta (Vendedor) | ? Error de transacciones | ? Funcional |

---

## ?? TROUBLESHOOTING

### ? Si "Registrar Venta" sigue dando error:

**Verificar:**

1. **Logs de aplicación:**
   ```bash
   # En la consola donde corre dotnet run
   # Buscar líneas con "?" o "Error"
   ```

2. **DevTools:**
   - F12 ? Console
   - F12 ? Network ? POST `/api/Ventas`
   - Ver Response completa

3. **Base de Datos:**
   ```sql
   -- Verificar READ_COMMITTED_SNAPSHOT
   SELECT is_read_committed_snapshot_on 
   FROM sys.databases 
   WHERE name = 'SciFiHubDB';
   -- Debe retornar: 1
   ```

### ? Si "Eliminar Definitivamente" falla:

**Verificar:**

1. **Usuario está en papelera:**
   ```sql
   SELECT * FROM Usuarios WITH (NOLOCK) 
   WHERE Email = 'eliminar@test.com' AND IsDeleted = 1;
   -- Debe retornar 1 fila
   ```

2. **Confirmación correcta:**
   - Escribir EXACTAMENTE el nombre completo
   - Sin espacios extras

### ? Si "Restaurar" no muestra usuario:

**Verificar:**

1. **Usuario fue restaurado en BD:**
   ```sql
   SELECT IsDeleted FROM Usuarios WITH (NOLOCK) 
   WHERE Email = 'restaurar@test.com';
   -- Debe retornar: 0
   ```

2. **Página se recargó:**
   - Debe haber recarga automática
   - Si no, F5 manual

---

## ?? REPORTAR RESULTADOS

### ? Si TODO funciona:

**Marcar checklist completa:**
- [x] ? Todas las pruebas pasaron
- [x] ? Sin errores de transacciones
- [x] ? Hard delete funcional
- [x] ? Restaurar funcional

### ? Si HAY problemas:

**Reportar con:**

1. **Número de test que falló** (TEST 1, 2, 3, o 4)

2. **Mensaje de error exacto:**
   - Copiar mensaje completo
   - Screenshot si es posible

3. **Logs de consola:**
   ```bash
   # Copiar últimas 20 líneas de consola
   # Donde dice "Error" o "Exception"
   ```

4. **Query de verificación:**
   ```sql
   -- Para TEST 2:
   SELECT * FROM Usuarios WITH (NOLOCK) 
   WHERE Email = 'eliminar@test.com';
   
   -- Para TEST 3:
   SELECT * FROM Usuarios WITH (NOLOCK) 
   WHERE Email = 'restaurar@test.com';
   ```

---

## ?? CRITERIOS DE ÉXITO

### ? TODAS las pruebas deben pasar:

1. ? Ventas se registran sin error de transacciones
2. ? Usuarios se eliminan definitivamente de BD
3. ? Usuarios restaurados aparecen en tabla principal
4. ? Vendedores pueden registrar ventas

### ?? Si TODAS pasan:
**OBSERVACIONES SUBSANADAS EXITOSAMENTE** ??

---

**Versión:** 1.0  
**Fecha:** $(Get-Date -Format "dd/MM/yyyy HH:mm")  
**Tiempo Estimado:** 30-35 minutos
