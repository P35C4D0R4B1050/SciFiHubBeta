# ?? GUÍA RÁPIDA - Probar las Correcciones

## ? YA ESTÁ TODO LISTO

La compilación fue **EXITOSA**. Todos los cambios están aplicados.

---

## ?? QUÉ SE CORRIGIÓ

1. **Usuarios**: Los errores ahora muestran mensajes detallados
2. **Ventas**: Los errores ahora muestran mensajes detallados
3. **Logging**: Se agregó logging detallado en Console con emojis
4. **Validaciones**: Validación de stock, cliente seleccionado, etc.

---

## ?? PASOS PARA PROBAR

### 1. Ejecutar la Aplicación

```bash
dotnet run
```

O desde Visual Studio: Presionar **F5**

### 2. Abrir el Navegador

Ir a: `https://localhost:XXXX` (donde XXXX es el puerto)

**IMPORTANTE**: Abrir DevTools inmediatamente:
- Presionar **F12**
- Ir a pestaña **Console**

---

## ?? PRUEBA 1: Crear Usuario (5 minutos)

### Pasos:
1. Login: `admin.test` / `Test123!`
2. Admin ? Usuarios
3. Click "Crear Usuario"
4. Completar formulario:
   - Nombre: `Prueba Error`
   - Email: `prueba@test.com`
   - Username: `prueba`
   - Password: `123` (solo 3 caracteres - provocará error)
   - Confirmar Password: `123`
   - Rol: Seleccionar cualquiera
5. Click "Guardar Usuario"

### Resultado Esperado:

**En Console** (DevTools):
```
?? Enviando usuario: {nombreCompleto: "Prueba Error", ...}
?? Respuesta del servidor: {...}
?? Status code: 400
? Error del servidor completo: {...}
```

**Alert en pantalla**:
```
? Error al guardar usuario:

Errores de validación:
• Password: The Password field must be at least 6 characters.
```

### ? Si Funciona Correctamente:
- Se muestra el error específico de que la contraseña debe tener al menos 6 caracteres
- Los logs en console son claros y detallados

### ? Si NO Funciona:
- Capturar screenshot de Console
- Capturar screenshot del Alert
- Compartir ambos

---

## ?? PRUEBA 2: Crear Usuario Exitoso (3 minutos)

### Pasos:
1. Mismos pasos que Prueba 1, pero:
   - Password: `Prueba123!` (válido)
   - Confirmar Password: `Prueba123!`
2. Click "Guardar Usuario"

### Resultado Esperado:

**En Console**:
```
?? Enviando usuario: {...}
?? Respuesta del servidor: {success: true, ...}
?? Status code: 200
```

**Alert**:
```
? Usuario creado exitosamente
```

**Acción**:
- Página se recarga
- El nuevo usuario aparece en la tabla

---

## ?? PRUEBA 3: Crear Venta sin Cliente (2 minutos)

### Pasos:
1. Login: `vendedor.test` / `Test123!`
2. Vendedor ? Nueva Venta
3. **SIN seleccionar cliente**, agregar un libro:
   - Buscar libro: escribir "dune" (o cualquier título)
   - Click en un libro de los resultados
4. Completar método de pago y dirección
5. Click "Registrar Venta"

### Resultado Esperado:

**Alert**:
```
? Debe seleccionar un cliente
```

El campo "Buscar Cliente" recibe el foco.

---

## ?? PRUEBA 4: Crear Venta Exitosa (10 minutos)

### Pasos:
1. Vendedor ? Nueva Venta
2. **Buscar Cliente**:
   - Escribir "test" o cualquier nombre
   - **Hacer CLICK** en un cliente de los resultados
   - Verificar que se seleccionó (el nombre aparece en el campo)
3. **Agregar Libros**:
   - Buscar libro: escribir un título
   - **Hacer CLICK** en un libro
   - Verificar que se agrega a la tabla
   - (Opcional) Agregar más libros
4. **Completar datos**:
   - Método de pago: Seleccionar uno
   - Dirección de envío: Escribir una dirección
5. Click "Registrar Venta"

### Resultado Esperado:

**En Console** (mucho detalle):
```
?? Iniciando registro de venta...
?? Cliente seleccionado: {id: "...", nombre: "..."}
?? Items: [{titulo: "...", cantidad: 1, ...}]
?? Datos de venta a enviar: {...}
?? Calculando totales...
? Totales calculados: {subtotal: XX, igv: YY, total: ZZ}
?? Enviando venta...
?? Respuesta recibida, status: 200
?? Datos de respuesta: {success: true, ...}
```

**Alert**:
```
? Venta registrada exitosamente

Número de Venta: V-20250115-0001
```

**Acción**:
- Redirige a `/Vendedor/Ventas`
- La nueva venta aparece en la lista

---

## ?? SI HAY ALGÚN PROBLEMA

### Si aparece "Error desconocido"

Significa que la corrección NO se aplicó correctamente.

**Verificar**:
1. ¿Está el archivo `wwwroot/js/crear-venta.js`?
2. ¿Se modificó `Views/Admin/Usuarios.cshtml`?
3. ¿Se actualizó `Views/Vendedor/CrearVenta.cshtml`?

### Si no hay logs en Console

**Verificar**:
- DevTools está abierto ANTES de hacer click en "Guardar" o "Registrar"
- Está en la pestaña **Console**, no en **Elements** o **Network**

### Si el error sigue siendo genérico

**Capturar**:
1. Screenshot completo de **Console** (F12 ? Console)
2. DevTools ? **Network** ? Buscar petición fallida (POST /api/...)
3. Click en ella ? Pestaña **Response** ? Copy
4. Compartir ambos

---

## ?? COMPARACIÓN ANTES/DESPUÉS

### ANTES ?

**Error genérico**:
```
Alert: ? Error desconocido al guardar el usuario
```

**Console vacío**:
```
(sin información)
```

### DESPUÉS ?

**Error específico**:
```
Alert: ? Error al guardar usuario:

Errores de validación:
• Username: The Username field is required.
• Password: The Password field must be at least 6 characters.
```

**Console detallado**:
```
?? Enviando usuario: {...}
?? Respuesta del servidor: {...}
?? Status code: 400
? Error del servidor completo: {
  "errors": {
    "Username": ["The Username field is required."],
    "Password": ["The Password field must be at least 6 characters."]
  },
  ...
}
```

---

## ? CHECKLIST FINAL

Después de probar todo:

- [ ] Crear usuario con error muestra mensaje específico
- [ ] Crear usuario exitoso funciona y recarga
- [ ] Crear venta sin cliente muestra error
- [ ] Crear venta con stock insuficiente previene agregar
- [ ] Crear venta exitosa redirige y aparece en la lista
- [ ] Los logs en Console son claros con emojis

Si TODOS los checks están ?, **¡la corrección fue exitosa!**

---

## ?? SIGUIENTE PASO

Si todo funciona:
- **Continuar con las pruebas funcionales** del resto de la aplicación
- **Documentar** cualquier otro problema que encuentres

Si algo NO funciona:
- **Compartir**:
  1. Screenshot de Console completo
  2. Screenshot del error
  3. Pasos exactos para reproducirlo

---

**Estado**: ? Listo para Probar  
**Compilación**: ? Exitosa  
**Tiempo Estimado de Pruebas**: 20-25 minutos  
**Próximo Paso**: `dotnet run` y empezar las pruebas
