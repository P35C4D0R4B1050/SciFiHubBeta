# ?? GUÍA RÁPIDA - Ejecutar y Probar Correcciones

## ?? TIEMPO TOTAL: 10-15 minutos

---

## ?? PASO 1: COMPILAR (2 min)

```powershell
# Navegar al proyecto
cd E:\Proyecto\SciFiHub\SciFiHub

# Limpiar y compilar
dotnet clean
dotnet build
```

**? Resultado Esperado:**
```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

**? Si falla:**
- Ver errores específicos
- Consultar `SOLUCION_ERRORES_CARRITO_VENTAS.md` ? TROUBLESHOOTING

---

## ?? PASO 2: EJECUTAR APLICACIÓN (1 min)

```powershell
dotnet run
```

**? Esperar mensaje:**
```
info: Now listening on: https://localhost:7116
```

**Abrir navegador:**
```
https://localhost:7116
```

---

## ?? PASO 3: PRUEBA CARRITO - CLIENTE (5 min)

### 3.1 Login como Cliente

**Opción A - Usuario Existente:**
```
Usuario: cliente1
Password: Cliente123!
```

**Opción B - Crear Nuevo Cliente:**
1. Click "Registrarse"
2. Completar formulario
3. Rol: Cliente
4. Iniciar sesión

---

### 3.2 Agregar al Carrito desde Catálogo

1. **Ir a "Catálogo"** (menú superior)
2. **Seleccionar un libro** (ej: "Frankenstein")
3. **Cambiar cantidad** a 3
4. **Click "?? Agregar al Carrito"**

**? ÉXITO:**
```
? Producto agregado al carrito exitosamente
? Contador actualizado en navbar
? Sin errores en consola (F12)
```

**? FALLO:**
```
? Mensaje de error rojo
? Ver F12 ? Console para detalles
? Ver F12 ? Network ? POST /api/CarritoApi/...
```

---

### 3.3 Agregar desde Vista de Detalle

1. **Click en un libro** para ver detalle
2. **Cambiar cantidad** a 2
3. **Click "?? Agregar al Carrito"**

**? ÉXITO:**
```
? Producto agregado
? Cantidad sumada si ya existía
```

---

### 3.4 Ver Carrito

1. **Click en icono de carrito** (navbar)
2. **Verificar:**
   - Libros agregados visibles
   - Cantidades correctas
   - Subtotales correctos
   - Total correcto

---

## ?? PASO 4: PRUEBA VENTAS - ADMIN/VENDEDOR (5 min)

### 4.1 Cerrar Sesión y Login como Admin

```
Usuario: admin
Password: Admin123!
```

---

### 4.2 Registrar Nueva Venta

1. **Ir a "Inventario"** ? **"Nueva Venta"**
2. **Seleccionar Cliente:**
   - Buscar "cliente" en el campo
   - Seleccionar cliente del dropdown

3. **Agregar Libros:**
   - Buscar por título, autor o ISBN
   - Ej: Buscar "Frankenstein"
   - Click en resultado
   - Cambiar cantidad a 2
   - Click "Agregar a la lista"

4. **Completar Dirección:**
   ```
   Dirección: Av. Prueba 123
   Ciudad: Lima
   Departamento: Lima
   Código Postal: 15001
   País: Perú
   Referencia: Cerca al parque
   ```

5. **Método de Pago:**
   - Seleccionar: "Efectivo"

6. **Click "? Registrar Venta"**

---

### 4.3 Verificar Éxito

**? RESULTADO ESPERADO:**

1. **Mensaje de éxito:**
```
? Venta registrada exitosamente
  Número de venta: V20250109-0001
```

2. **Redirección automática** a lista de ventas

3. **Ver venta en tabla:**
   - Número de venta
   - Cliente correcto
   - Total correcto
   - Estado: "Pendiente"

4. **Verificar stock actualizado:**
   - Ir a "Inventario" ? "Libros"
   - Buscar libro vendido
   - Stock debe haber disminuido

---

**? SI FALLA:**

Ver logs en consola (F12) y terminal del servidor:

```
Ejemplo de error:
POST /api/Ventas 400 (Bad Request)

En consola del servidor:
Error: Invalid column name 'CreatedAt'
```

**Solución:**
1. Verificar que `dotnet clean` se ejecutó
2. Reiniciar aplicación
3. Ver `SOLUCION_ERRORES_CARRITO_VENTAS.md` ? TROUBLESHOOTING

---

## ?? PASO 5: PRUEBA ADICIONAL - Vendedor (Opcional, 3 min)

### 5.1 Login como Vendedor

**Crear vendedor si no existe:**
1. Login como Admin
2. Ir a "Usuarios"
3. Click "Nuevo Usuario"
4. Completar:
   ```
   Nombre: Juan Vendedor
   Email: vendedor@test.com
   Username: vendedor1
   Password: Vendedor123!
   Rol: Vendedor
   ```
5. Guardar

**Login como vendedor:**
```
Usuario: vendedor1
Password: Vendedor123!
```

### 5.2 Registrar Venta

- Repetir pasos 4.2 a 4.3
- Verificar que funciona igual

---

## ? CHECKLIST FINAL

### Funcionalidades Probadas:

- [ ] ? Compilación exitosa
- [ ] ?? Agregar al carrito desde catálogo
- [ ] ?? Agregar al carrito desde detalle
- [ ] ?? Ver carrito con productos
- [ ] ?? Registrar venta como Admin
- [ ] ?? Stock actualizado después de venta
- [ ] ?? Registrar venta como Vendedor (opcional)

---

## ?? RESULTADO ESPERADO

```
??????????????????????????????????????????????????
?                                                ?
?   TODAS LAS PRUEBAS PASARON                   ?
?                                                ?
?   ? Carrito: Funcional                       ?
?   ? Agregar productos: Funcional             ?
?   ? Registrar ventas: Funcional              ?
?   ? Stock: Se actualiza correctamente        ?
?                                                ?
?   ?? SISTEMA OPERATIVO AL 100%                ?
?                                                ?
??????????????????????????????????????????????????
```

---

## ?? AYUDA RÁPIDA

### Error en Compilación
? Ver `SOLUCION_ERRORES_CARRITO_VENTAS.md` ? TROUBLESHOOTING

### Error al Agregar al Carrito
1. F12 ? Console (ver mensaje de error)
2. F12 ? Network ? POST request (ver respuesta)
3. Terminal servidor (ver logs backend)

### Error al Registrar Venta
1. F12 ? Console
2. F12 ? Network ? POST /api/Ventas
3. Ver response body completo

---

## ?? INFORMACIÓN DE CONTACTO

**Documentación Completa:**
- `SOLUCION_ERRORES_CARRITO_VENTAS.md`

**Archivos Modificados:**
- `SciFiHub.Infrastructure/Data/Configurations/CarritoCompraConfiguration.cs`
- `SciFiHub.Infrastructure/Data/Configurations/DetalleCarritoConfiguration.cs`
- `SciFiHub.Infrastructure/Data/Configurations/AuditoriaInventarioConfiguration.cs`
- `SciFiHub.Infrastructure/Data/Configurations/CategoriaConfiguration.cs`
- `SciFiHub.Infrastructure/Data/Configurations/DetalleVentaConfiguration.cs`
- `SciFiHub.Infrastructure/Data/SciFiHubDbContext.cs`

---

**Última Actualización:** 2025-01-09  
**Versión:** 1.0  
**Estado:** ? Listo para Pruebas
