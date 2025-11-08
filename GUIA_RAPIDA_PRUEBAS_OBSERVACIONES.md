# ?? GUÍA RÁPIDA DE PRUEBAS - OBSERVACIONES SUBSANADAS

## ? INICIO RÁPIDO

```powershell
cd E:\Proyecto\SciFiHub\SciFiHub
dotnet clean && dotnet build && dotnet run
```

**Abrir en navegador:** `https://localhost:7116`

---

## ?? PRUEBAS PASO A PASO

### ? PRUEBA 1: PDF de Boleta (Cliente)

**Tiempo estimado:** 2 minutos

1. Login como **Cliente**
2. Click en menú usuario ? **"Mis Compras"**
3. Click en **"Ver"** en cualquier compra
4. Scroll hacia abajo hasta sección **"Boleta de Venta"** (tarjeta verde)
5. Click en **"Ver/Descargar Boleta (PDF)"**

**? Resultado Esperado:**
- PDF se abre en nueva pestaña
- Nombre: `Boleta_VENTA-XXXXXX.pdf`
- Contenido completo de la venta

---

### ? PRUEBA 2: Agregar Producto Duplicado

**Tiempo estimado:** 1 minuto

1. Login como **Cliente**
2. Ir al **Catálogo**
3. Agregar un libro al carrito (ej: "Dune")
4. Intentar agregar el mismo libro nuevamente

**? Resultado Esperado:**
- Notificación azul (??) aparece arriba a la derecha
- Mensaje: "Este libro ya está en tu carrito..."
- No hay error en consola (F12)

---

### ? PRUEBA 3: Modificar Cantidad en Carrito

**Tiempo estimado:** 3 minutos

1. Login como **Cliente**
2. Agregar 2-3 libros al carrito
3. Click en **"Carrito"** en navbar
4. **Probar:**
   - Click en **"+"** ? cantidad aumenta
   - Click en **"-"** ? cantidad disminuye
   - **Editar input** manualmente (ej: escribir 5) ? cantidad se actualiza
   - **Stock máximo:** intentar cantidad mayor al stock ? mensaje de advertencia

**? Resultado Esperado:**
- Cantidad se actualiza en tiempo real
- Subtotal se recalcula automáticamente
- Contador del navbar se actualiza (número arriba del icono de carrito)

---

### ? PRUEBA 4: Eliminar Item del Carrito

**Tiempo estimado:** 2 minutos

1. Login como **Cliente**
2. Agregar 2-3 libros al carrito
3. Click en **"Carrito"** en navbar
4. Click en **botón rojo de papelera (???)** de un item
5. Confirmar eliminación

**? Resultado Esperado:**
- Item desaparece de la lista
- Contador del navbar se actualiza
- Total se recalcula

**Caso especial - Último item:**
6. Eliminar todos los items menos uno
7. Eliminar el último item

**? Resultado Esperado:**
- Se muestra vista "Tu carrito está vacío" con icono grande
- Contador del navbar muestra 0 (o se oculta)
- Botón "Ver Catálogo" aparece

---

### ? PRUEBA 5: Vaciar Carrito Completo

**Tiempo estimado:** 1 minuto

1. Login como **Cliente**
2. Agregar varios libros al carrito
3. Click en **"Carrito"** en navbar
4. Click en **"Vaciar Carrito"** (botón rojo debajo de totales)
5. Confirmar

**? Resultado Esperado:**
- Todos los items desaparecen
- Se muestra vista "Tu carrito está vacío"
- Contador del navbar muestra 0

---

### ? PRUEBA 6: Formulario de Dirección con Listas

**Tiempo estimado:** 3 minutos

1. Login como **Cliente**
2. Agregar libros al carrito
3. Click en **"Carrito"** ? **"Proceder al Checkout"**

**Probar el formulario:**

4. **Departamento:**
   - Click en select "Departamento/Región"
   - Seleccionar **"Ayacucho"**
   - ? Select "Provincia" se habilita automáticamente

5. **Provincia:**
   - Click en select "Provincia"
   - Ver que aparecen 11 provincias: Huamanga, Cangallo, etc.
   - Seleccionar **"Huamanga"**
   - ? Select "Distrito" se habilita automáticamente

6. **Distrito:**
   - Click en select "Distrito"
   - Ver que aparecen 3 distritos: Ayacucho (Cercado), Carmen Alto, San Juan Bautista
   - Seleccionar **"Ayacucho (Cercado)"**

7. **Completar otros campos:**
   - Dirección Exacta: `Av. 28 de Julio 456, Dpto 301`
   - Código Postal: `05000` (opcional)
   - Referencia: `Cerca al mercado central` (opcional)

8. **Método de Pago:**
   - Seleccionar **"Efectivo"**
   - ? Aparece mensaje azul "Pago en Efectivo: El pago se realizará al momento de la entrega"

9. **Confirmar:**
   - Marcar checkbox "Acepto los términos y condiciones"
   - Click en **"Confirmar Pedido"**
   - Confirmar en el diálogo

**? Resultado Esperado:**
- Pedido se procesa correctamente
- Redirige a página de confirmación
- Venta se registra en BD con dirección completa

---

### ? PRUEBA 7: Verificar Dirección en BD

**Tiempo estimado:** 1 minuto

Después de completar la Prueba 6, ejecutar en **SQL Server Management Studio**:

```sql
USE SciFiHubDB;

SELECT TOP 1
    NumeroVenta,
    DireccionDepartamento,  -- Debe ser "Ayacucho"
    DireccionPais,          -- Debe ser "Huamanga"
    DireccionCiudad,        -- Debe ser "Ayacucho (Cercado)"
    DireccionCalle,         -- Debe ser "Av. 28 de Julio 456, Dpto 301"
    DireccionCodigoPostal,  -- Debe ser "05000"
    DireccionReferencia     -- Debe ser "Cerca al mercado central"
FROM Ventas
ORDER BY FechaVenta DESC;
```

**? Resultado Esperado:**
- Todos los campos de dirección están llenos correctamente
- Mapeo correcto según lo especificado en observación 2.3

---

## ?? CHECKLIST DE VERIFICACIÓN

Marcar con ? cada prueba completada exitosamente:

- [ ] **Prueba 1:** Cliente puede descargar PDF de boleta
- [ ] **Prueba 2:** Mensaje amigable al agregar producto duplicado
- [ ] **Prueba 3:** Cantidad se puede modificar con +/- e input manual
- [ ] **Prueba 4:** Items se pueden eliminar individualmente
- [ ] **Prueba 5:** Carrito se puede vaciar completamente
- [ ] **Prueba 6:** Formulario de dirección con listas funciona correctamente
- [ ] **Prueba 7:** Dirección se guarda correctamente en BD

---

## ?? SOLUCIÓN DE PROBLEMAS

### Problema: "No se puede descargar PDF"

**Solución:**
1. Verificar que QuestPDF está instalado:
   ```powershell
   dotnet list package | findstr QuestPDF
   ```
   Debe mostrar: `QuestPDF 2025.7.4`

2. Si no está instalado:
   ```powershell
   dotnet add package QuestPDF --version 2025.7.4
   dotnet build
   ```

---

### Problema: "Listas de ubicaciones no cargan"

**Solución:**
1. Abrir DevTools (F12) ? Pestaña Console
2. Buscar errores en rojo
3. Verificar que API responde:
   - Abrir en navegador: `https://localhost:7116/api/Ubicaciones/departamentos`
   - Debe retornar JSON con Ayacucho

4. Si no responde, verificar que `UbicacionesController.cs` existe en `Controllers/Api/`

---

### Problema: "Contador del carrito no se actualiza"

**Solución:**
1. Abrir DevTools (F12) ? Pestaña Console
2. Verificar que aparece: "? Carrito de compras inicializado"
3. Verificar que aparece: "?? Contador actualizado: X"
4. Si no aparecen, verificar que `carrito-cliente.js` se carga:
   - DevTools ? Pestaña Network
   - Buscar `carrito-cliente.js`
   - Debe tener Status 200

---

### Problema: "No puedo modificar cantidad en carrito"

**Solución:**
1. Verificar en Console (F12) que no hay errores JavaScript
2. Verificar que botones +/- tienen atributo `onclick`
3. Verificar que input tiene atributo `onchange`
4. Hacer Ctrl+F5 para limpiar caché del navegador

---

## ?? TIEMPO TOTAL DE PRUEBAS

- **Prueba 1:** 2 min
- **Prueba 2:** 1 min
- **Prueba 3:** 3 min
- **Prueba 4:** 2 min
- **Prueba 5:** 1 min
- **Prueba 6:** 3 min
- **Prueba 7:** 1 min

**TOTAL:** ~13 minutos

---

## ?? CAPTURAS RECOMENDADAS

Si deseas documentar las pruebas, tomar capturas de:

1. **PDF de boleta** descargado (Cliente)
2. **Notificación azul** al agregar producto duplicado
3. **Carrito con cantidades modificables** (botones +/-, input)
4. **Vista "Carrito vacío"** después de vaciar
5. **Formulario de dirección completo** con listas desplegables llenas
6. **Query en BD** mostrando dirección guardada correctamente

---

## ?? TODAS LAS OBSERVACIONES COMPLETADAS

Si todas las pruebas pasan exitosamente (?), entonces:

**¡TODAS LAS OBSERVACIONES DEL CLIENTE HAN SIDO SUBSANADAS CORRECTAMENTE!**

Puedes proceder con:
- Despliegue a producción
- Notificar al cliente
- Cerrar tickets/issues relacionados

---

**Versión:** 1.0  
**Fecha:** 2025-01-09  
**Tiempo estimado total:** 13 minutos  
**Dificultad:** ????? (Fácil)
