# ?? GUÍA DE PRUEBAS FUNCIONALES - SciFiHub

## ?? PREPARACIÓN

### **Paso 1: Ejecutar Script de Preparación**

```sql
-- Ejecutar en SSMS:
Database/PreparacionPruebas.sql
```

Este script:
- ? Verifica que existan categorías (si no, las crea)
- ? Verifica usuarios y libros
- ? Crea usuarios de prueba
- ? Verifica columnas BaseEntity

**Usuarios creados**:
- `admin.test` / `Test123!` (Administrador)
- `vendedor.test` / `Test123!` (Vendedor)

---

### **Paso 2: Iniciar Aplicación**

```bash
cd E:\Proyecto\SciFiHub\SciFiHub
dotnet run
```

Esperar mensaje:
```
Now listening on: https://localhost:XXXX
Application started. Press Ctrl+C to shut down.
```

---

## ?? PRUEBA 1: Carga de Categorías

### **Objetivo**: Verificar que las categorías se cargan correctamente en el modal de Agregar Libro

### **Pasos**:

1. **Abrir navegador** en `https://localhost:XXXX`

2. **Login como Vendedor**:
   - Username: `vendedor.test`
   - Password: `Test123!`

3. **Ir a Inventario**:
   - Click en menú "Inventario" o "Libros"

4. **Abrir DevTools** (F12):
   - Ir a pestaña "Console"

5. **Abrir modal Agregar Libro**:
   - Click en botón "Agregar Libro"

6. **Verificar en Console**:
   ```
   ?? Iniciando carga de categorías...
   ?? Datos recibidos del servidor: {success: true, data: Array(8)}
   ? 8 categorías cargadas exitosamente
   ```

7. **Verificar en el select**:
   - Debe mostrar:
     ```
     Seleccionar categoría...
     Ciencia Ficción
     Cyberpunk
     Distopía
     Fantasía
     Horror Cósmico
     Space Opera
     Steampunk
     Viajes en el Tiempo
     ```

### **Resultado Esperado**:
- ? NO hay error "Invalid column name"
- ? Categorías se muestran en el select
- ? Logs aparecen en consola

### **Si Falla**:
- Verificar que el script `PreparacionPruebas.sql` se ejecutó
- Verificar logs del servidor (terminal donde corre dotnet run)
- Ejecutar en SSMS:
  ```sql
  SELECT * FROM Categorias WHERE IsDeleted = 0;
  ```

---

## ?? PRUEBA 2: Crear Usuario (Admin)

### **Objetivo**: Verificar que se puede crear un usuario sin error

### **Pasos**:

1. **Logout** (si estás logueado como vendedor)

2. **Login como Admin**:
   - Username: `admin.test`
   - Password: `Test123!`

3. **Ir a Usuarios**:
   - Click en menú "Usuarios"

4. **Abrir DevTools** (F12):
   - Pestaña "Console"

5. **Click en "Crear Usuario"**

6. **Completar formulario**:
   - Nombre Completo: `Test Usuario Nuevo`
   - Email: `test.usuario@scifihub.com`
   - Username: `test.user`
   - Password: `Test123!`
   - Confirmar Password: `Test123!`
   - Rol: `Cliente`

7. **Click en "Guardar Usuario"**

8. **Verificar en Console**:
   ```javascript
   Enviando usuario: {nombreCompleto: "Test Usuario Nuevo", ...}
   Respuesta del servidor: {success: true, message: "Usuario creado exitosamente", ...}
   ```

9. **Verificar en la tabla**:
   - Debe aparecer el nuevo usuario

10. **Verificar en BD**:
    ```sql
    SELECT * FROM Usuarios 
    WHERE Email = 'test.usuario@scifihub.com';
    ```

### **Resultado Esperado**:
- ? NO aparece "Error: Error desconocido al guardar el usuario"
- ? Mensaje "? Usuario creado exitosamente"
- ? Usuario aparece en la tabla
- ? Usuario existe en BD

### **Si Falla**:
- Copiar el error exacto de Console
- Verificar logs del servidor
- Verificar que el email no esté duplicado

---

## ?? PRUEBA 3: Editar Usuario (Admin)

### **Objetivo**: Verificar que se puede editar un usuario sin error

### **Pasos**:

1. **Estando en Admin ? Usuarios**

2. **Buscar el usuario creado**: `Test Usuario Nuevo`

3. **Click en botón "Editar" (icono lápiz)**

4. **Modificar datos**:
   - Teléfono: `999888777`
   - Estado: Mantener en Activo

5. **Click en "Guardar Usuario"**

6. **Verificar en Console**:
   ```javascript
   Enviando usuario: {id: "...", nombreCompleto: "Test Usuario Nuevo", ...}
   Respuesta del servidor: {success: true, message: "Usuario actualizado exitosamente", ...}
   ```

7. **Verificar en BD**:
   ```sql
   SELECT Telefono, UpdatedAt 
   FROM Usuarios 
   WHERE Email = 'test.usuario@scifihub.com';
   ```

### **Resultado Esperado**:
- ? NO aparece "Error: Error desconocido al guardar el usuario"
- ? Mensaje "? Usuario actualizado exitosamente"
- ? Teléfono se guardó correctamente
- ? UpdatedAt se actualizó

---

## ?? PRUEBA 4: Registro Rápido de Cliente (Vendedor)

### **Objetivo**: Verificar que se puede registrar un cliente desde CrearVenta

### **Pasos**:

1. **Logout y Login como Vendedor**:
   - Username: `vendedor.test`
   - Password: `Test123!`

2. **Ir a Ventas ? Nueva Venta** (o CrearVenta)

3. **En sección Cliente, click en "Nuevo Cliente"**

4. **Completar formulario**:
   - Nombre Completo: `Cliente Rápido Test`
   - Email: `cliente.rapido@test.com`
   - Teléfono: `987654321`

5. **Click en "Registrar Cliente"**

6. **Aparecerá un Alert con credenciales**:
   ```
   ? Cliente registrado exitosamente

   ?? Email: cliente.rapido@test.com
   ?? Usuario: cliente.rapido_20250128XXXXXX
   ?? Contraseña temporal: XXXXXXXXXX

   ?? IMPORTANTE: Anote estas credenciales y entréguelas al cliente.
   ```

7. **ANOTAR las credenciales**

8. **Cerrar el alert**

9. **Verificar que el cliente quedó seleccionado**:
   - El input de Cliente debe mostrar: "Cliente Rápido Test"

10. **Verificar en BD**:
    ```sql
    SELECT * FROM Usuarios 
    WHERE Email = 'cliente.rapido@test.com';
    ```

### **Resultado Esperado**:
- ? Modal se cierra automáticamente
- ? Alert muestra credenciales
- ? Cliente queda seleccionado
- ? Usuario existe en BD con Rol = 'Cliente'

---

## ?? PRUEBA 5: Usuarios Eliminados - Soft Delete

### **Objetivo**: Verificar que funciona el soft delete

### **Pasos**:

1. **Login como Admin**

2. **Ir a Admin ? Usuarios**

3. **Buscar "Test Usuario Nuevo"**

4. **Click en botón "Eliminar" (icono basura roja)**

5. **Confirmar eliminación**

6. **Verificar que desaparece de la tabla**

7. **Verificar en BD**:
   ```sql
   SELECT IsDeleted, DeletedAt 
   FROM Usuarios 
   WHERE Email = 'test.usuario@scifihub.com';
   ```

### **Resultado Esperado**:
- ? Usuario desaparece de tabla activos
- ? IsDeleted = 1 en BD
- ? DeletedAt tiene fecha

---

## ?? PRUEBA 6: Usuarios Eliminados - Vista Papelera

### **Objetivo**: Verificar que funciona la vista de papelera

### **Pasos**:

1. **Estando en Admin ? Usuarios**

2. **Activar toggle** "Ver usuarios eliminados (Papelera)"

3. **Verificar que aparece el usuario eliminado**:
   - Debe mostrar: "Test Usuario Nuevo"
   - Con fecha de eliminación

4. **Verificar botones**:
   - Botón "Restaurar" (verde)
   - Botón "Eliminar Definitivo" (rojo)

### **Resultado Esperado**:
- ? Tabla cambia a vista de eliminados
- ? Aparece el usuario eliminado
- ? Muestra fecha de eliminación
- ? Tiene 2 botones de acción

---

## ?? PRUEBA 7: Restaurar Usuario

### **Objetivo**: Verificar que se puede restaurar un usuario eliminado

### **Pasos**:

1. **Con el toggle activado** (ver usuarios eliminados)

2. **Click en "Restaurar"** del usuario "Test Usuario Nuevo"

3. **Confirmar restauración**

4. **Desactivar el toggle**

5. **Verificar que el usuario vuelve a la tabla de activos**

6. **Verificar en BD**:
   ```sql
   SELECT IsDeleted, DeletedAt 
   FROM Usuarios 
   WHERE Email = 'test.usuario@scifihub.com';
   ```

### **Resultado Esperado**:
- ? Usuario desaparece de papelera
- ? Usuario aparece en tabla activos
- ? IsDeleted = 0 en BD
- ? DeletedAt = NULL

---

## ?? PRUEBA 8: Eliminar Definitivo

### **Objetivo**: Verificar la eliminación permanente con doble confirmación

### **Pasos**:

1. **Eliminar nuevamente "Test Usuario Nuevo"** (soft delete)

2. **Activar toggle** "Ver usuarios eliminados"

3. **Click en "Eliminar Definitivo"**

4. **Primera confirmación** (Alert):
   ```
   ?? ADVERTENCIA ??

   ¿Está ABSOLUTAMENTE SEGURO de eliminar DEFINITIVAMENTE al usuario "Test Usuario Nuevo"?

   Esta acción NO SE PUEDE DESHACER.
   El usuario será eliminado permanentemente de la base de datos.
   ```
   - Click en "Aceptar"

5. **Segunda confirmación** (Prompt):
   ```
   Para confirmar, escriba el nombre completo del usuario:
   "Test Usuario Nuevo"
   ```
   - Escribir exactamente: `Test Usuario Nuevo`
   - Click en "Aceptar"

6. **Verificar mensaje**:
   ```
   ? Usuario eliminado definitivamente de la base de datos
   ```

7. **Verificar que desaparece de la papelera**

8. **Verificar en BD**:
   ```sql
   SELECT * FROM Usuarios 
   WHERE Email = 'test.usuario@scifihub.com';
   -- NO debe devolver ningún registro
   ```

### **Resultado Esperado**:
- ? Doble confirmación funciona
- ? Usuario desaparece de papelera
- ? Usuario NO existe en BD
- ? Acción irreversible completada

---

## ?? CHECKLIST DE PRUEBAS

| # | Prueba | Estado | Observaciones |
|---|--------|--------|---------------|
| 1 | Carga de Categorías | ? | |
| 2 | Crear Usuario | ? | |
| 3 | Editar Usuario | ? | |
| 4 | Registro Rápido Cliente | ? | |
| 5 | Soft Delete | ? | |
| 6 | Vista Papelera | ? | |
| 7 | Restaurar Usuario | ? | |
| 8 | Eliminar Definitivo | ? | |

---

## ?? SI ENCUENTRA ERRORES

### **Error en Categorías**:
```sql
-- Verificar categorías
SELECT * FROM Categorias WHERE IsDeleted = 0;

-- Si no hay, ejecutar:
Database/PreparacionPruebas.sql
```

### **Error al Crear/Editar Usuario**:
1. Abrir DevTools ? Console
2. Copiar el error exacto
3. Ver logs del servidor
4. Reportar con mensaje completo

### **Error 500 Internal Server Error**:
1. Ver terminal donde corre `dotnet run`
2. Buscar líneas con `[Error]`
3. Copiar stack trace completo

---

## ? RESULTADO ESPERADO

Después de completar todas las pruebas:

- ? Categorías se cargan sin error
- ? Usuarios se pueden crear sin "Error desconocido"
- ? Usuarios se pueden editar sin error
- ? Registro rápido funciona y genera credenciales
- ? Soft delete funciona correctamente
- ? Vista de papelera muestra usuarios eliminados
- ? Restaurar funciona correctamente
- ? Eliminar definitivo tiene doble confirmación y funciona

---

**Tiempo Estimado**: 30-40 minutos  
**Dificultad**: Media  
**Estado**: Listo para ejecutar
