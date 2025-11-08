# ?? CHECKLIST - DEBUGGING CARRITO

## ? ESTADO ACTUAL

- [x] Código modificado con logging exhaustivo
- [x] Compilación exitosa
- [ ] Aplicación en ejecución
- [ ] Logs recopilados

---

## ?? PASOS A SEGUIR

### PASO 1: Ejecutar Aplicación
```powershell
cd E:\Proyecto\SciFiHub\SciFiHub
dotnet run
```
- [ ] Aplicación corriendo
- [ ] URL abierta: `https://localhost:7116`

---

### PASO 2: Preparar Debugging
- [ ] Presionar F12 (DevTools)
- [ ] Ir a pestaña Console
- [ ] Limpiar consola (Ctrl + L)

---

### PASO 3: Login y Cargar Carrito
- [ ] Login como Cliente
- [ ] Ir al Catálogo
- [ ] Agregar 2-3 libros al carrito
- [ ] Ir a `/Carrito`
- [ ] Ver que aparece log: `? Carrito de compras inicializado con logging exhaustivo`

---

### PASO 4: Probar Botón + (Incrementar)

**Acción:** Click en botón **+** de un item

**Logs esperados:**
```
?? Cambiando cantidad: { libroId: "...", nuevaCantidad: X }
?? Enviando petición PUT a /api/CarritoApi/actualizar
?? Response status actualizar: 200
? Respuesta actualizar: { success: true, ... }
```

**Checklist:**
- [ ] Botón responde al click
- [ ] Aparecen logs en consola
- [ ] Cantidad aumenta en pantalla
- [ ] No hay errores rojos

**Si hay error:**
- [ ] Copiar logs completos
- [ ] Copiar status code
- [ ] Copiar mensaje de error

---

### PASO 5: Probar Botón - (Decrementar)

**Acción:** Click en botón **-** de un item

**Logs esperados:**
```
?? Cambiando cantidad: { libroId: "...", nuevaCantidad: X }
?? Enviando petición PUT a /api/CarritoApi/actualizar
?? Response status actualizar: 200
? Respuesta actualizar: { success: true, ... }
```

**Checklist:**
- [ ] Botón responde al click
- [ ] Aparecen logs en consola
- [ ] Cantidad disminuye en pantalla
- [ ] No hay errores rojos

**Si hay error:**
- [ ] Copiar logs completos

---

### PASO 6: Probar Input Manual

**Acción:** Click en input de cantidad, escribir número (ej: 5), presionar Enter

**Logs esperados:**
```
?? Cambio manual de cantidad: { libroId: "...", valor: "5" }
?? Validando: { nuevaCantidad: 5, minimo: 1, maximo: 99 }
?? Cambiando cantidad: { libroId: "...", nuevaCantidad: 5 }
?? Enviando petición PUT a /api/CarritoApi/actualizar
?? Response status actualizar: 200
```

**Checklist:**
- [ ] Input responde al cambio
- [ ] Aparecen logs en consola
- [ ] Cantidad se actualiza en pantalla
- [ ] No hay errores rojos

**Pruebas adicionales:**
- [ ] Probar escribir 0 ? Debe mostrar advertencia
- [ ] Probar exceder stock ? Debe mostrar advertencia

---

### PASO 7: Probar Eliminar Item

**Acción:** Click en botón papelera ???, confirmar

**Logs esperados:**
```
??? Eliminando del carrito: "..."
?? Enviando petición DELETE a /api/CarritoApi/eliminar/...
?? Response status eliminar: 200
?? Response headers: ...
? Respuesta eliminar: { "success": true, ... }
? Eliminación exitosa
```

**Checklist:**
- [ ] Aparece confirmación
- [ ] Aparecen logs en consola
- [ ] Item desaparece de la lista
- [ ] Contador se actualiza
- [ ] No hay errores rojos

**Si hay error:**
- [ ] Copiar logs completos
- [ ] Copiar stack trace si existe

---

### PASO 8: Probar Vaciar Carrito

**Acción:** Click en "Vaciar Carrito", confirmar

**Logs esperados:**
```
??? Intentando vaciar carrito
?? Enviando petición DELETE a /api/CarritoApi/vaciar
?? Response status vaciar: 200
? Respuesta vaciar: { "success": true, ... }
? Carrito vaciado exitosamente
```

**Checklist:**
- [ ] Aparece confirmación
- [ ] Aparecen logs en consola
- [ ] Todos los items desaparecen
- [ ] Se muestra mensaje "Carrito vacío"
- [ ] Contador muestra 0
- [ ] No hay errores rojos

**Si hay error:**
- [ ] Copiar logs completos

---

## ?? RECOPILACIÓN DE INFORMACIÓN

### Si TODO funciona:
? Marcar todas las casillas como completadas  
? Celebrar ??

### Si algo NO funciona:

#### Información a recopilar:

**1. Logs de Consola del Navegador:**
```
[Pegar aquí TODOS los logs que aparecen en consola]
```

**2. Logs del Servidor (donde corre dotnet run):**
```
[Pegar aquí los logs del servidor si hay errores]
```

**3. Request/Response (DevTools ? Network):**
- [ ] Ir a pestaña Network
- [ ] Reproducir el error
- [ ] Click derecho en la petición fallida
- [ ] Copy ? Copy as cURL
```
[Pegar aquí el cURL]
```

**4. Screenshot del Error:**
- [ ] Captura de pantalla de la consola
- [ ] Captura de pantalla de Network tab

---

## ?? POSIBLES ERRORES Y SOLUCIONES

### ? Error: "actualizandoCarrito is not defined"
**Causa:** Variable global no inicializada  
**Solución:** Recargar página (F5)

### ? Error: "Cannot read property 'items' of null"
**Causa:** carritoData no cargado  
**Solución:** Verificar que se llama `cargarCarrito()` al inicio

### ? Error 401: Unauthorized
**Causa:** Usuario no autenticado  
**Solución:** 
1. Cerrar sesión
2. Login nuevamente
3. Probar de nuevo

### ? Error 400: Bad Request
**Causa:** Datos inválidos (LibroId, cantidad, etc.)  
**Solución:** Verificar que LibroId es un GUID válido

### ? Error 500: Server Error
**Causa:** Error en backend  
**Solución:** 
1. Ver logs del servidor
2. Verificar que la BD está accesible
3. Verificar que el servicio CarritoService funciona

### ? No aparecen logs
**Causa:** JavaScript no se carga o hay error previo  
**Solución:**
1. Ver si hay errores rojos en consola ANTES de probar
2. Verificar que se carga `carrito-cliente.js`
3. Verificar que existe elemento `carrito-items`
4. Recargar página con Ctrl+F5 (hard refresh)

---

## ?? MATRIZ DE DIAGNÓSTICO

| Síntoma | Causa Probable | Solución |
|---------|----------------|----------|
| Botón no responde | Event listener no configurado | Verificar que función está en `window` |
| Logs no aparecen | JavaScript no cargado | Hard refresh (Ctrl+F5) |
| Error 401 | No autenticado | Re-login |
| Error 400 | Datos inválidos | Verificar LibroId/Cantidad |
| Error 500 | Error en servidor | Ver logs del servidor |
| Cantidad no cambia | PUT no funciona | Verificar endpoint API |
| Item no se elimina | DELETE no funciona | Verificar endpoint API |

---

## ?? RESULTADO ESPERADO

Al finalizar este checklist:

- ? Sabrás exactamente qué funciona y qué no
- ? Tendrás logs detallados de cada operación
- ? Podrás identificar el problema exacto
- ? Tendrás información para solucionarlo

---

**Fecha:** 2025-01-09  
**Estado:** ? LISTO PARA DEBUGGING SISTEMÁTICO  
**Duración estimada:** 10-15 minutos

---

**¡Sigue este checklist paso a paso y encontraremos el problema!** ??
