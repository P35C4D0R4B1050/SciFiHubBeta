# ? ACCIÓN INMEDIATA - DEBUGGING CARRITO

## ?? CAMBIO APLICADO

Se agregó **logging exhaustivo** en `carrito-cliente.js` para diagnosticar por qué los botones +/-, eliminar y vaciar no funcionan.

---

## ?? EJECUTAR AHORA

```powershell
cd E:\Proyecto\SciFiHub\SciFiHub
dotnet run
```

---

## ?? PASOS PARA DIAGNOSTICAR

### 1. Abrir DevTools
- Presiona **F12**
- Ve a pestaña **Console**
- Limpia consola (Ctrl + L)

### 2. Ir al Carrito
- Login como Cliente
- Agrega productos al carrito
- Ve a `/Carrito`

### 3. Probar y Copiar Logs

#### A. Botones +/-
- Click en botón **+** o **-**
- **Copiar TODO lo que aparece en consola**

#### B. Eliminar Item
- Click en botón papelera ???
- Confirmar
- **Copiar TODO lo que aparece en consola**

#### C. Vaciar Carrito
- Click en "Vaciar Carrito"
- Confirmar
- **Copiar TODO lo que aparece en consola**

---

## ?? LOGS ESPERADOS (SI FUNCIONA)

### Cambiar Cantidad:
```
?? Cambiando cantidad: { libroId: "...", nuevaCantidad: 2 }
?? Enviando petición PUT a /api/CarritoApi/actualizar
?? Response status actualizar: 200
? Respuesta actualizar: { success: true, ... }
? Actualización exitosa, recargando carrito
```

### Eliminar Item:
```
??? Eliminando del carrito: "..."
?? Enviando petición DELETE a /api/CarritoApi/eliminar/...
?? Response status eliminar: 200
? Respuesta eliminar: { "success": true, ... }
? Eliminación exitosa
```

### Vaciar Carrito:
```
??? Intentando vaciar carrito
?? Enviando petición DELETE a /api/CarritoApi/vaciar
?? Response status vaciar: 200
? Respuesta vaciar: { "success": true, ... }
? Carrito vaciado exitosamente
```

---

## ? ERRORES POSIBLES

### Error 401:
```
?? Response status: 401
```
**Causa:** Usuario no autenticado  
**Solución:** Verificar login

### Error 400:
```
?? Response status: 400
? Error en respuesta: "Carrito no encontrado"
```
**Causa:** Carrito o LibroId inválido  
**Solución:** Verificar datos

### Error 500:
```
?? Response status: 500
```
**Causa:** Error en servidor  
**Solución:** Revisar logs del servidor (consola donde corre `dotnet run`)

### Sin Logs:
**Causa:** JavaScript no se carga o hay error antes  
**Solución:** 
1. Ver si hay errores rojos en consola
2. Verificar que existe `carrito-items` en HTML
3. Verificar que se carga `carrito-cliente.js`

---

## ?? QUÉ HACER DESPUÉS

1. **Ejecutar** `dotnet run`
2. **Abrir** F12 ? Console
3. **Probar** cada botón
4. **Copiar** TODOS los logs (incluso si hay muchos)
5. **Compartir** los logs completos

---

**Estado:** ? LISTO PARA DIAGNOSTICAR  
**Archivo:** `wwwroot/js/carrito-cliente.js`  
**Compilación:** ? EXITOSA

---

**Con estos logs sabremos EXACTAMENTE dónde está el problema.** ??
