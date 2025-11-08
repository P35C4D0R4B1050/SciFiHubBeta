# ? COMANDOS DIRECTOS - DEBUGGING CARRITO

## ?? EJECUTAR APLICACIÓN

```powershell
cd E:\Proyecto\SciFiHub\SciFiHub
dotnet run
```

---

## ?? ABRIR DEVTOOLS

**En Chrome/Edge:**
- Presiona `F12`
- O `Ctrl + Shift + I`
- O Click derecho ? Inspeccionar

**Ir a pestaña Console**

**Limpiar consola:**
- `Ctrl + L`
- O click en icono ?? (Clear console)

---

## ?? PLANTILLA PARA REPORTAR

Copiar y pegar esta plantilla, completar con los logs:

```
==============================================
REPORTE DE DEBUGGING - CARRITO
==============================================

OPERACIÓN PROBADA: [Botón +/- / Input / Eliminar / Vaciar]

----------------------------------------------
LOGS DE CONSOLA DEL NAVEGADOR:
----------------------------------------------
[Pegar aquí TODOS los logs]


----------------------------------------------
¿APARECIÓ ALGÚN LOG?
----------------------------------------------
[ ] SÍ - Ver arriba
[ ] NO - No apareció nada en consola


----------------------------------------------
¿QUÉ STATUS CODE APARECIÓ? (si hubo petición)
----------------------------------------------
[ ] 200 - OK
[ ] 400 - Bad Request
[ ] 401 - Unauthorized
[ ] 500 - Server Error
[ ] Otro: ___________


----------------------------------------------
¿FUNCIONÓ LA OPERACIÓN?
----------------------------------------------
[ ] SÍ - Todo correcto
[ ] NO - Ver logs arriba


----------------------------------------------
LOGS DEL SERVIDOR (si hubo error):
----------------------------------------------
[Pegar aquí logs del terminal donde corre dotnet run]


----------------------------------------------
SCREENSHOT (opcional):
----------------------------------------------
[Adjuntar captura de pantalla]


==============================================
```

---

## ?? PRUEBAS ESPECÍFICAS

### Prueba 1: Botón + (Incrementar)

**Acción:**
1. Ir a `/Carrito`
2. Click en botón **+** de un item
3. Copiar TODOS los logs

**Log esperado (si funciona):**
```
?? Cambiando cantidad: { libroId: "...", nuevaCantidad: X }
?? Enviando petición PUT a /api/CarritoApi/actualizar
?? Response status actualizar: 200
? Respuesta actualizar: { success: true, ... }
? Actualización exitosa, recargando carrito
```

---

### Prueba 2: Botón - (Decrementar)

**Acción:**
1. Click en botón **-** de un item
2. Copiar TODOS los logs

**Log esperado (si funciona):**
```
?? Cambiando cantidad: { libroId: "...", nuevaCantidad: X }
?? Enviando petición PUT a /api/CarritoApi/actualizar
?? Response status actualizar: 200
? Respuesta actualizar: { success: true, ... }
? Actualización exitosa, recargando carrito
```

---

### Prueba 3: Input Manual

**Acción:**
1. Click en input de cantidad
2. Escribir número (ej: 5)
3. Presionar Enter
4. Copiar TODOS los logs

**Log esperado (si funciona):**
```
?? Cambio manual de cantidad: { libroId: "...", valor: "5" }
?? Validando: { nuevaCantidad: 5, minimo: 1, maximo: 99 }
?? Cambiando cantidad: { libroId: "...", nuevaCantidad: 5 }
?? Enviando petición PUT a /api/CarritoApi/actualizar
?? Response status actualizar: 200
```

---

### Prueba 4: Eliminar Item

**Acción:**
1. Click en botón papelera ???
2. Confirmar eliminación
3. Copiar TODOS los logs

**Log esperado (si funciona):**
```
??? Eliminando del carrito: "..."
?? Enviando petición DELETE a /api/CarritoApi/eliminar/...
?? Response status eliminar: 200
? Respuesta eliminar: { "success": true, ... }
? Eliminación exitosa
```

---

### Prueba 5: Vaciar Carrito

**Acción:**
1. Click en botón "Vaciar Carrito"
2. Confirmar
3. Copiar TODOS los logs

**Log esperado (si funciona):**
```
??? Intentando vaciar carrito
?? Enviando petición DELETE a /api/CarritoApi/vaciar
?? Response status vaciar: 200
? Respuesta vaciar: { "success": true, ... }
? Carrito vaciado exitosamente
```

---

## ?? COMANDOS DE EMERGENCIA

### Hard Refresh (limpiar cache)
```
Ctrl + F5
```
O
```
Ctrl + Shift + R
```

### Ver peticiones HTTP en Network
1. F12 ? Pestaña **Network**
2. Filtrar: **Fetch/XHR**
3. Reproducir el error
4. Click en la petición fallida
5. Ver: Headers, Payload, Response

### Copiar petición como cURL
1. Network tab
2. Click derecho en petición
3. Copy ? Copy as cURL
4. Pegar en terminal para probar

### Ver si JavaScript se cargó
1. F12 ? Sources
2. Buscar: `carrito-cliente.js`
3. Verificar que existe y no tiene errores de sintaxis

---

## ?? INTERPRETACIÓN RÁPIDA

| Log visible | Interpretación | Acción |
|------------|----------------|---------|
| `? Carrito de compras inicializado` | JavaScript cargado OK | Continuar probando |
| `?? Cambiando cantidad` | Función llamada OK | Ver siguiente log |
| `?? Response status: 200` | Servidor respondió OK | Debe funcionar |
| `?? Response status: 401` | No autenticado | Re-login |
| `?? Response status: 400` | Error de datos | Ver mensaje error |
| `?? Response status: 500` | Error servidor | Ver logs servidor |
| (Ningún log) | JavaScript NO cargado | Hard refresh |

---

## ?? ATAJOS DE TECLADO

### DevTools
- `F12` - Abrir/Cerrar
- `Ctrl + Shift + C` - Inspector de elementos
- `Ctrl + Shift + J` - Console directa

### Console
- `Ctrl + L` - Limpiar
- `Ctrl + K` - Limpiar (alternativo)
- `clear()` - Comando para limpiar

### Página
- `F5` - Refresh normal
- `Ctrl + F5` - Hard refresh (limpiar cache)
- `Ctrl + Shift + R` - Hard refresh (alternativo)

---

## ?? INFORMACIÓN A RECOPILAR

**Cuando algo NO funciona, recopilar:**

1. **Logs de consola completos** (Ctrl+A, Ctrl+C)
2. **Status code** de la petición HTTP
3. **Mensaje de error** exacto
4. **Logs del servidor** (del terminal donde corre dotnet run)
5. **Screenshot** (opcional pero útil)

---

## ? COMANDOS ÚTILES EN CONSOLA DEL NAVEGADOR

### Verificar si funciones existen
```javascript
console.log(typeof window.cambiarCantidad); // "function" = OK
console.log(typeof window.eliminarDelCarrito); // "function" = OK
console.log(typeof window.vaciarCarrito); // "function" = OK
```

### Verificar variable carritoData
```javascript
console.log(carritoData); // Ver contenido
```

### Verificar flag actualizandoCarrito
```javascript
console.log(actualizandoCarrito); // false = OK, puede hacer operaciones
```

### Probar función manualmente
```javascript
// Probar cambiar cantidad
await cambiarCantidad('GUID-DEL-LIBRO', 5);

// Probar eliminar
await eliminarDelCarrito('GUID-DEL-LIBRO');

// Probar vaciar
await vaciarCarrito();
```

---

## ?? ERRORES COMUNES Y SOLUCIÓN INMEDIATA

### "actualizandoCarrito is not defined"
**Solución:** Recargar página (F5)

### "Cannot read property 'items' of null"
**Solución:** Recargar carrito, verificar que tiene items

### "Failed to fetch"
**Solución:** Verificar que servidor está corriendo

### "401 Unauthorized"
**Solución:** Cerrar sesión y volver a hacer login

### No aparece ningún log
**Solución:** 
1. Hard refresh (Ctrl+F5)
2. Ver si hay error rojo en consola
3. Verificar que se carga carrito-cliente.js (pestaña Network)

---

**Estado:** ? LISTO  
**Fecha:** 2025-01-09  

**¡Copia y pega estos comandos para debugging rápido!** ?
