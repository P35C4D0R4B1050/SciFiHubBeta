# ? 3 PASOS - DEBUGGING CARRITO

## ? CAMBIO APLICADO
Se agregó **logging exhaustivo** en el carrito para diagnosticar por qué los botones no funcionan.

---

## ?? PASO 1: EJECUTAR (30 segundos)

```powershell
cd E:\Proyecto\SciFiHub\SciFiHub
dotnet run
```

Abre: `https://localhost:7116`

---

## ?? PASO 2: PREPARAR DEBUGGING (10 segundos)

1. Presiona **F12**
2. Click en pestaña **Console**
3. Presiona **Ctrl + L** (limpiar)

---

## ?? PASO 3: PROBAR Y COPIAR LOGS (5 minutos)

### A. Login y Cargar Carrito
- Login como Cliente
- Agrega 2-3 libros al carrito
- Ve a `/Carrito`

### B. Probar CADA Botón y Copiar Logs

#### 1?? Botón + (Incrementar)
- Click en **+**
- **Copiar TODO lo de consola**

#### 2?? Botón - (Decrementar)
- Click en **-**
- **Copiar TODO lo de consola**

#### 3?? Input Manual
- Click en input, escribir número, Enter
- **Copiar TODO lo de consola**

#### 4?? Eliminar Item
- Click en papelera ???, confirmar
- **Copiar TODO lo de consola**

#### 5?? Vaciar Carrito
- Click en "Vaciar Carrito", confirmar
- **Copiar TODO lo de consola**

---

## ?? PLANTILLA PARA COMPARTIR

```
Operación: [+/-/Input/Eliminar/Vaciar]

LOGS:
[Pegar aquí TODO lo de consola]

¿Funcionó? [SÍ/NO]
```

---

## ? LOGS ESPERADOS (Si funciona)

### Botón +/-
```
?? Cambiando cantidad: { ... }
?? Response status: 200
? Actualización exitosa
```

### Eliminar
```
??? Eliminando del carrito: ...
?? Response status: 200
? Eliminación exitosa
```

### Vaciar
```
??? Intentando vaciar carrito
?? Response status: 200
? Carrito vaciado
```

---

## ? SI HAY PROBLEMA

### No aparecen logs
?? Hard refresh: `Ctrl + F5`

### Error 401
?? Re-login

### Error 400/500
?? Copiar logs y compartir

---

## ?? MÁS INFO

Ver archivo: **INDICE_DOCUMENTACION_DEBUGGING.md**

---

**¡Solo 3 pasos! Ejecuta, abre F12 y prueba.** ?
