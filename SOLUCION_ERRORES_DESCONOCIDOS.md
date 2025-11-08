# ?? SOLUCIÓN - Errores "Error desconocido" en Ventas y Usuarios

## ?? DIAGNÓSTICO

### Problema Identificado
Las capturas muestran:
1. **Crear Venta**: "Error desconocido al registrar venta" - Status 400 (Bad Request)
2. **Crear Usuario**: "Error desconocido al guardar el usuario" - Status 400 (Bad Request)

### Causa Raíz
El problema es que el servidor está devolviendo **errores de validación detallados** (status 400), pero el frontend no los está extrayendo correctamente del response JSON.

En las capturas se ve en la consola:
```
One or more validation errors occurred
traceid: 00-117039cf19dd28af93fcd939d46b2d6a-3757451867cf344e-00
```

Esto indica que **ASP.NET Core está devolviendo un ProblemDetails** con errores de validación, pero el código JavaScript solo busca `data.error` genérico.

---

## ??? SOLUCIÓN

### 1. Mejorar el Manejo de Errores en JavaScript

El código actual en `Usuarios.cshtml`:
```javascript
if (response.ok && data.success) {
    alert(`? Usuario ${esEdicion ? 'actualizado' : 'creado'} exitosamente`);
    window.location.reload();
} else {
    alert('? Error: ' + (data.error || 'Error desconocido al guardar el usuario'));
}
```

**PROBLEMA**: Solo busca `data.error`, pero ASP.NET puede devolver errores en:
- `data.errors` (errores de validación por campo)
- `data.title` (título del error)
- `data.detail` (detalle del error)
- `data.error` (mensaje personalizado)

### 2. Función Mejorada para Extraer Errores

Agregar esta función al inicio del script:

```javascript
/**
 * Extrae mensajes de error detallados de respuestas del servidor
 */
function extraerMensajeError(data) {
    // 1. Errores de validación por campo (ModelState)
    if (data.errors) {
        const errores = Object.entries(data.errors)
            .map(([campo, mensajes]) => {
                const msgs = Array.isArray(mensajes) ? mensajes.join(', ') : mensajes;
                return `• ${campo}: ${msgs}`;
            })
            .join('\n');
        return `Errores de validación:\n${errores}`;
    }
    
    // 2. ProblemDetails de ASP.NET
    if (data.title) {
        let mensaje = data.title;
        if (data.detail) {
            mensaje += `\n${data.detail}`;
        }
        if (data.traceId) {
            mensaje += `\n\nTrace ID: ${data.traceId}`;
        }
        return mensaje;
    }
    
    // 3. Mensaje personalizado del controller
    if (data.error) {
        return data.error;
    }
    
    // 4. Fallback
    return 'Error desconocido. Por favor, contacte al administrador.';
}
```

### 3. Actualizar `guardarUsuario()` en Usuarios.cshtml

```javascript
async function guardarUsuario() {
    const form = document.getElementById('formUsuario');
    if (!form.checkValidity()) {
        form.reportValidity();
        return;
    }
    
    // Validar contraseñas si está creando
    if (!modoEdicion) {
        const password = document.getElementById('password').value;
        const confirmPassword = document.getElementById('confirmPassword').value;
        
        if (password !== confirmPassword) {
            alert('? Las contraseñas no coinciden');
            document.getElementById('confirmPassword').classList.add('is-invalid');
            return;
        }
    }
    
    const formData = new FormData(form);
    const usuarioId = document.getElementById('usuarioId').value;
    const esEdicion = usuarioId !== '';
    
    let usuario;
    if (esEdicion) {
        usuario = {
            id: usuarioId,
            nombreCompleto: formData.get('NombreCompleto'),
            email: formData.get('Email'),
            telefono: formData.get('Telefono') || null,
            rol: formData.get('Rol'),
            estado: formData.get('Estado')
        };
    } else {
        usuario = {
            nombreCompleto: formData.get('NombreCompleto'),
            email: formData.get('Email'),
            username: formData.get('Username'),
            password: formData.get('Password'),
            confirmPassword: formData.get('ConfirmPassword'),
            rol: formData.get('Rol'),
            telefono: formData.get('Telefono') || null
        };
    }
    
    console.log('?? Enviando usuario:', usuario);
    
    try {
        const url = esEdicion ? `/api/Usuarios/${usuarioId}` : '/api/Usuarios';
        const method = esEdicion ? 'PUT' : 'POST';
        
        const response = await fetch(url, {
            method: method,
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(usuario)
        });
        
        const data = await response.json();
        console.log('?? Respuesta del servidor:', data);
        
        if (response.ok && data.success) {
            alert(`? Usuario ${esEdicion ? 'actualizado' : 'creado'} exitosamente`);
            window.location.reload();
        } else {
            // ?? USAR FUNCIÓN MEJORADA
            const errorMsg = extraerMensajeError(data);
            console.error('? Error del servidor:', JSON.stringify(data, null, 2));
            alert('? Error al guardar usuario:\n\n' + errorMsg);
        }
    } catch (error) {
        console.error('? Error de red o parsing:', error);
        alert('? Error de conexión:\n\n' + error.message);
    }
}
```

### 4. Actualizar Código de Ventas en CrearVenta.cshtml

En la función que registra la venta (aproximadamente línea 280-320):

```javascript
async function registrarVenta(event) {
    event.preventDefault();
    
    if (!clienteSeleccionado || !clienteSeleccionado.id) {
        alert('? Debe seleccionar un cliente');
        return;
    }
    
    if (itemsVenta.length === 0) {
        alert('? Debe agregar al menos un libro a la venta');
        return;
    }
    
    const formData = new FormData(document.getElementById('formNuevaVenta'));
    
    const venta = {
        clienteId: clienteSeleccionado.id,
        metodoPago: formData.get('MetodoPago'),
        direccionEnvio: formData.get('DireccionEnvio'),
        detalles: itemsVenta.map(item => ({
            libroId: item.id,
            cantidad: item.cantidad,
            precioUnitario: item.precio,
            descuento: 0
        }))
    };
    
    console.log('?? Iniciando registro de venta...');
    console.log('?? Calculando totales...');
    
    try {
        // Primero calcular totales
        const calcResponse = await fetch('/api/Ventas/calcular-totales', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ items: venta.detalles })
        });
        
        const calcData = await calcResponse.json();
        console.log('? Totales calculados:', calcData);
        
        // Ahora registrar la venta
        console.log('?? Preparando venta para enviar...', venta);
        console.log('?? Enviando venta...');
        
        const response = await fetch('/api/Ventas', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(venta)
        });
        
        const data = await response.json();
        console.log('?? Respuesta recibida, status:', response.status);
        console.log('?? Datos de respuesta:', data);
        
        if (response.ok && data.success) {
            alert(`? Venta registrada exitosamente\n\nNúmero de Venta: ${data.numeroVenta}`);
            window.location.href = '/Vendedor/Ventas';
        } else {
            // ?? USAR FUNCIÓN MEJORADA
            const errorMsg = extraerMensajeError(data);
            console.error('? Error del servidor:', JSON.stringify(data, null, 2));
            alert('? Error al registrar venta:\n\n' + errorMsg);
        }
    } catch (error) {
        console.error('? Error inesperado:', error);
        alert('? Error de conexión:\n\n' + error.message);
    }
}
```

---

## ?? CÓMO DEPURAR EL PROBLEMA ACTUAL

### Paso 1: Abrir DevTools
1. Presionar `F12` en el navegador
2. Ir a la pestaña **Console**
3. Ir a la pestaña **Network**

### Paso 2: Intentar Crear Usuario
1. Ir a Admin ? Usuarios
2. Click "Crear Usuario"
3. Completar el formulario
4. Click "Guardar Usuario"

### Paso 3: Ver en Console
Buscar estos logs:
```
?? Enviando usuario: {objeto con datos}
?? Respuesta del servidor: {objeto con respuesta}
```

### Paso 4: Ver en Network
1. Buscar la petición `POST /api/Usuarios`
2. Click en ella
3. Ver pestaña **Response**
4. Ver el JSON completo que devuelve el servidor

**Ejemplo de lo que deberías ver si hay errores de validación:**
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Rol": ["The Rol field is required."],
    "Password": ["The Password field is required."]
  },
  "traceId": "00-..."
}
```

### Paso 5: Copiar el JSON Completo
1. Click derecho en el Response
2. "Copy Response"
3. Pegarlo aquí para análisis

---

## ?? CAUSAS COMUNES Y SOLUCIONES

### Error 1: "Rol field is required"
**Causa**: El select de rol envía el texto ("Administrador") pero el backend espera el enum (0, 1, 2)

**Solución**: Cambiar el HTML del select:
```html
<select class="form-select" name="Rol" id="rol" required>
    <option value="">Seleccionar rol...</option>
    <option value="0">??? Administrador</option>
    <option value="1">?? Vendedor</option>
    <option value="2">?? Cliente</option>
</select>
```

### Error 2: "Password field is required" al editar
**Causa**: El campo Password sigue siendo requerido en modo edición

**Solución**: Ya está implementado en el código - se oculta `seccionCredenciales` en modo edición

### Error 3: "ClienteId is required" en ventas
**Causa**: No se está seleccionando correctamente el cliente

**Verificar**:
```javascript
// Antes de enviar
console.log('Cliente seleccionado:', clienteSeleccionado);
console.log('Cliente ID:', clienteSeleccionado?.id);
```

### Error 4: "Stock insuficiente"
**Causa**: El libro seleccionado no tiene stock

**Solución**: Agregar validación antes de agregar al carrito:
```javascript
function agregarItem(libro) {
    if (libro.stock <= 0) {
        alert(`? El libro "${libro.titulo}" no tiene stock disponible`);
        return;
    }
    // ... resto del código
}
```

---

## ?? CHECKLIST DE IMPLEMENTACIÓN

### Archivos a Modificar

#### 1. Views/Admin/Usuarios.cshtml ?
- [ ] Agregar función `extraerMensajeError()`
- [ ] Actualizar `guardarUsuario()` para usar la función
- [ ] Cambiar valores del select Rol a números (0, 1, 2)
- [ ] Agregar más logging en console

#### 2. Views/Vendedor/CrearVenta.cshtml ?
- [ ] Agregar función `extraerMensajeError()`
- [ ] Actualizar función de registro de venta
- [ ] Agregar validación de stock antes de agregar items
- [ ] Agregar logging detallado

#### 3. Opcional: wwwroot/js/site.js
Si muchas vistas comparten código:
- [ ] Mover `extraerMensajeError()` a site.js
- [ ] Usarla globalmente desde todas las vistas

---

## ?? PRUEBAS

### Test 1: Crear Usuario con Error de Validación
1. Admin ? Usuarios ? Crear Usuario
2. Dejar campos vacíos
3. Click "Guardar"
4. **Esperado**: Mensaje detallado con los campos faltantes

### Test 2: Crear Usuario Exitoso
1. Completar todos los campos correctamente
2. Click "Guardar"
3. **Esperado**: "? Usuario creado exitosamente"

### Test 3: Crear Venta sin Cliente
1. Vendedor ? Nueva Venta
2. Agregar libros sin seleccionar cliente
3. Click "Registrar Venta"
4. **Esperado**: "? Debe seleccionar un cliente"

### Test 4: Crear Venta Exitosa
1. Seleccionar cliente
2. Agregar libros
3. Completar método de pago y dirección
4. Click "Registrar Venta"
5. **Esperado**: "? Venta registrada exitosamente" con número de venta

---

## ?? PRÓXIMOS PASOS

1. **Implementar** la función `extraerMensajeError()` en ambos archivos
2. **Probar** crear usuario y venta
3. **Copiar** el JSON completo del error si persiste
4. **Analizar** el JSON para determinar el problema exacto
5. **Ajustar** el código según el error específico

---

## ?? SOPORTE

Si después de implementar esto el problema persiste:

1. Abrir DevTools ? Console
2. Intentar la operación
3. Copiar TODOS los logs que aparecen
4. Ir a DevTools ? Network
5. Buscar la petición fallida (POST /api/Usuarios o POST /api/Ventas)
6. Click derecho ? Copy ? Copy response
7. Pegar aquí para análisis detallado

---

**Versión**: 1.0  
**Fecha**: Enero 2025  
**Estado**: ?? Pendiente de Implementación
