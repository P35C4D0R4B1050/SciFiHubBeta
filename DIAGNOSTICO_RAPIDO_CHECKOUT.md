# ?? DIAGNÓSTICO RÁPIDO - CHECKOUT

## ?? INFORMACIÓN NECESARIA

Por favor, sigue estos pasos y copia los resultados:

---

### 1. VERIFICAR LOGS DEL NAVEGADOR

**Pasos:**
1. Abrir la página de Checkout
2. Presionar **F12**
3. Ir a pestaña **Console**
4. Limpiar consola (Ctrl+L)
5. Llenar el formulario completo
6. Click en "Confirmar Pedido"
7. Confirmar en el diálogo

**Copiar TODOS los logs que aparecen en Console:**
```
[PEGAR AQUÍ LOS LOGS]
```

---

### 2. VERIFICAR LOGS DEL SERVIDOR

En la consola donde corre `dotnet run`, copiar los logs que aparecen después de click en "Confirmar Pedido":

```
[PEGAR AQUÍ LOS LOGS DEL SERVIDOR]
```

---

### 3. VERIFICAR REQUEST EN NETWORK TAB

**Pasos:**
1. Con DevTools abierto (F12)
2. Ir a pestaña **Network**
3. Filtrar por "ProcesarCheckout"
4. Click derecho en la petición
5. **Copy** ? **Copy as cURL (bash)**

**Pegar aquí:**
```
[PEGAR AQUÍ EL cURL]
```

---

### 4. VER RESPUESTA DEL SERVIDOR

En la pestaña **Network**, hacer click en la petición "ProcesarCheckout":
- Ver la sección **Response**

**Pegar aquí:**
```
[PEGAR AQUÍ LA RESPUESTA]
```

---

### 5. VERIFICAR USUARIO VENDEDOR

Ejecutar en SQL Server Management Studio:

```sql
SELECT Id, Username, Rol, NombreCompleto, Email, Estado
FROM Usuarios
WHERE Username = 'vendedor' AND Rol = 'Vendedor';
```

**Resultado:**
```
[PEGAR AQUÍ EL RESULTADO]
```

**Si NO retorna nada**, ejecutar:

```sql
INSERT INTO Usuarios (
    Id, 
    NombreCompleto, 
    Email, 
    Username, 
    PasswordHash, 
    Rol, 
    FechaRegistro, 
    Estado, 
    CreatedAt
)
VALUES (
    NEWID(),
    'Vendedor Sistema',
    'vendedor@scifihub.com',
    'vendedor',
    'AQAAAAIAAYagAAAAEK1z2w3QmH8K5pZvB5nZ0A==',
    'Vendedor',
    GETDATE(),
    'Activo',
    GETDATE()
);
```

---

### 6. VERIFICAR CARRITO TIENE ITEMS

Ejecutar en SQL (reemplazar `TU_CLIENTE_ID` con el ID de tu usuario):

```sql
SELECT 
    c.Id as CarritoId,
    c.ClienteId,
    c.Estado as EstadoCarrito,
    COUNT(dc.Id) as CantidadDetalles
FROM CarritoCompras c
LEFT JOIN DetallesCarrito dc ON c.Id = dc.CarritoId
WHERE c.ClienteId = 'TU_CLIENTE_ID' AND c.Estado = 'Activo'
GROUP BY c.Id, c.ClienteId, c.Estado;
```

**Resultado:**
```
[PEGAR AQUÍ EL RESULTADO]
```

---

### 7. VERIFICAR SI SE CREÓ ALGUNA VENTA

```sql
SELECT TOP 5 *
FROM Ventas
ORDER BY FechaVenta DESC;
```

**Resultado:**
```
[PEGAR AQUÍ EL RESULTADO]
```

---

## ?? POSIBLES PROBLEMAS Y SOLUCIONES

### Problema A: "Formulario no se envía"

**Síntomas:**
- No aparecen logs en consola del navegador
- No aparece confirmación

**Solución:**
1. Verificar que todos los campos están llenos
2. Verificar que se aceptó términos y condiciones
3. Recargar página con Ctrl+F5

---

### Problema B: "Error 401 Unauthorized"

**Síntomas:**
- Response del servidor: 401
- No se procesa el checkout

**Solución:**
1. Cerrar sesión
2. Login nuevamente como Cliente
3. Intentar de nuevo

---

### Problema C: "Error 400 Bad Request"

**Síntomas:**
- Response del servidor: 400
- Mensaje de error en Response

**Solución:**
1. Ver el mensaje de error específico en Response
2. Verificar que DireccionEnvioJson no está vacío
3. Verificar que MetodoPago está seleccionado

---

### Problema D: "Error 500 Server Error"

**Síntomas:**
- Response del servidor: 500
- Error en logs del servidor

**Solución:**
1. Ver los logs del servidor (paso 2)
2. Buscar el mensaje de error específico
3. Verificar que existe usuario vendedor (paso 5)

---

### Problema E: "No aparecen logs en servidor"

**Síntomas:**
- No hay logs después de click en "Confirmar Pedido"
- Servidor no responde

**Solución:**
1. Verificar que `dotnet run` está corriendo
2. Recargar página
3. Intentar de nuevo

---

## ?? ENVIAR DIAGNÓSTICO

Una vez que tengas todos los datos arriba, compártelos para poder identificar exactamente el problema.

---

**Fecha:** 2025-01-09  
**Propósito:** Diagnóstico exhaustivo del problema de checkout
