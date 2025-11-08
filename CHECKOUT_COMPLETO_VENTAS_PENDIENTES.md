# ? CHECKOUT COMPLETO - VENTAS PENDIENTES

## ?? IMPLEMENTACIÓN COMPLETA

### FLUJO DE CHECKOUT:

```
1. Cliente modifica cantidades (solo visual)
2. Cliente hace click en "Checkout"
   ? Cantidades temporales se guardan en sessionStorage
3. Cliente llena formulario de checkout
4. Cliente confirma pedido
   ? Cantidades temporales se envían al servidor
5. Servidor:
   ? Aplica cantidades temporales a BD
   ? Valida stock con cantidades actualizadas
   ? Crea venta con estado PENDIENTE
   ? NO descuenta stock (se hará cuando vendedor confirme)
   ? Limpia carrito (borra detalles)
   ? Marca carrito como convertido
6. Cliente ve confirmación: "Pedido pendiente de confirmación"
7. Venta aparece en "Mis Compras" con estado PENDIENTE
```

---

## ?? CAMBIOS REALIZADOS

### 1. **CarritoController.cs**

**ProcesarCheckout:**
```csharp
[HttpPost]
public async Task<IActionResult> ProcesarCheckout(
    [FromForm] CheckoutDTO checkoutDto, 
    [FromForm] string? cantidadesTemporales)
{
    // ? Pasar cantidades temporales al servicio
    checkoutDto.CantidadesTemporalesJson = cantidadesTemporales;
    
    var result = await _carritoService.ProcesarCheckoutAsync(
        CurrentUserId.Value, checkoutDto);
    
    // ...
}
```

---

### 2. **CheckoutDTO**

**Agregado:**
```csharp
public record CheckoutDTO
{
    // ...propiedades existentes...
    
    /// <summary>
    /// JSON con cantidades temporales del frontend
    /// </summary>
    public string? CantidadesTemporalesJson { get; set; }
}
```

---

### 3. **CarritoService.cs**

**ProcesarCheckoutAsync:**
```csharp
public async Task<Result<VentaDTO>> ProcesarCheckoutAsync(...)
{
    await _unitOfWork.BeginTransactionAsync();
    
    // ? 1. APLICAR CANTIDADES TEMPORALES del frontend
    if (!string.IsNullOrEmpty(checkoutDto.CantidadesTemporalesJson))
    {
        var cantidades = JsonSerializer.Deserialize<Dictionary<string, int>>(
            checkoutDto.CantidadesTemporalesJson);
        
        foreach (var (libroIdStr, cantidad) in cantidades)
        {
            await _unitOfWork.Carritos.ActualizarCantidadItemAsync(
                carrito.Id, libroId, cantidad);
        }
        await _unitOfWork.CommitAsync();
    }
    
    // Recargar carrito con cantidades actualizadas
    var carrito = await _unitOfWork.Carritos.GetCarritoConDetallesAsync(...);
    
    // ? 2. VALIDAR STOCK con cantidades actualizadas
    if (!await _unitOfWork.Carritos.ValidarStockCarritoAsync(...))
    {
        return Result.FailureResult("Stock insuficiente");
    }
    
    // ? 3. CREAR VENTA con estado PENDIENTE
    var venta = await RegistrarVentaDesdeCarritoAsync(...);
    
    // ? 4. LIMPIAR CARRITO (borrar detalles)
    await _unitOfWork.Carritos.LimpiarCarritoAsync(carrito.Id);
    await _unitOfWork.Carritos.MarcarComoConvertidoAsync(carrito.Id);
    
    await _unitOfWork.CommitTransactionAsync();
}
```

**RegistrarVentaDesdeCarritoAsync:**
```csharp
private async Task<Result<VentaDTO>> RegistrarVentaDesdeCarritoAsync(...)
{
    var venta = new Venta
    {
        NumeroVenta = await GenerarNumeroVentaAsync(),
        FechaVenta = DateTime.UtcNow,
        
        // ? ESTADO PENDIENTE (para que vendedor/admin confirme)
        EstadoVenta = EstadoVenta.Pendiente,
        
        // ... resto de propiedades
    };
    
    // ? NO DESCONTAR STOCK aún
    // Se descuenta cuando vendedor confirma la venta
    
    await _unitOfWork.Ventas.AddAsync(venta);
    await _unitOfWork.CommitAsync();
}
```

---

### 4. **Checkout.cshtml**

**Campo hidden agregado:**
```html
<form asp-action="ProcesarCheckout" method="post" id="form-checkout">
    <!-- ? Campo para cantidades temporales -->
    <input type="hidden" name="cantidadesTemporales" id="cantidadesTemporales" value="">
    
    <!-- ...resto del formulario... -->
</form>
```

**Script modificado:**
```javascript
$('#form-checkout').submit(function(e) {
    // ...validaciones...
    
    // ? RECUPERAR Y ENVIAR CANTIDADES TEMPORALES
    const cantidadesTemporalesStr = sessionStorage.getItem('cambiosTemporales');
    if (cantidadesTemporalesStr) {
        console.log('?? Enviando cantidades:', cantidadesTemporalesStr);
        $('#cantidadesTemporales').val(cantidadesTemporalesStr);
    } else {
        $('#cantidadesTemporales').val('{}');
    }
    
    // Limpiar sessionStorage después de enviar
    sessionStorage.removeItem('cambiosTemporales');
    
    this.submit();
});
```

---

### 5. **carrito-cliente.js**

**Guardar en sessionStorage:**
```javascript
function cambiarCantidad(libroId, nuevaCantidad) {
    // ...validaciones...
    
    // ? Guardar cambio temporal
    cambiosTemporales[libroId] = nuevaCantidad;
    
    // ? Persistir en sessionStorage para checkout
    sessionStorage.setItem('cambiosTemporales', JSON.stringify(cambiosTemporales));
    
    renderizarCarrito();
}

// ? Función para guardar antes de ir a checkout
function guardarCantidadesTemporales() {
    if (Object.keys(cambiosTemporales).length > 0) {
        sessionStorage.setItem('cambiosTemporales', JSON.stringify(cambiosTemporales));
        console.log('?? Guardadas para checkout:', cambiosTemporales);
    }
}
```

**Botón checkout:**
```html
<a href="/Carrito/Checkout" class="btn btn-success" 
   onclick="guardarCantidadesTemporales()">
    Checkout
</a>
```

---

## ?? ESTADOS DE LA VENTA

### Estado PENDIENTE (al crear desde checkout)
- ? Venta creada en BD
- ? Carrito limpiado
- ? Stock NO descontado
- ? Visible en "Mis Compras" (cliente)
- ? Visible en panel Vendedor/Admin
- ? Esperando confirmación

### Estado CONFIRMADA (cuando vendedor confirma)
- ? Stock descontado
- ? Cliente puede generar PDF
- ? Envío programado

---

## ?? DATOS QUE SE ENVÍAN

**Formato de cantidadesTemporales (JSON string):**
```json
{
  "f47ac10b-58cc-4372-a567-0e02b2c3d479": 3,
  "a47ac10b-58cc-4372-a567-0e02b2c3d480": 5
}
```

**Donde:**
- Key: LibroId (GUID como string)
- Value: Nueva cantidad

---

## ? RESULTADO FINAL

### Cliente puede:
1. ? Modificar cantidades (visual instantáneo)
2. ? Hacer checkout
3. ? Ver pedido en "Mis Compras" (estado: Pendiente)
4. ? Esperar confirmación de vendedor

### Vendedor/Admin puede:
1. ? Ver ventas pendientes
2. ? Confirmar venta (descontar stock)
3. ? Cancelar venta (no descontar stock)

### Sistema:
1. ? Carrito se limpia después del checkout
2. ? Venta queda registrada (estado: Pendiente)
3. ? Stock NO se descuenta hasta confirmación
4. ? Cliente puede seguir comprando (nuevo carrito)

---

## ?? EJECUTAR

```powershell
cd E:\Proyecto\SciFiHub\SciFiHub
dotnet run
```

---

## ?? PROBAR

1. Login como Cliente
2. Agregar libros al carrito
3. Modificar cantidades (+/-)
4. Click en "Checkout"
5. Llenar formulario
6. Confirmar pedido
7. Verificar:
   - ? Mensaje: "Pedido pendiente de confirmación"
   - ? Carrito vacío
   - ? Venta en "Mis Compras" (estado: Pendiente)
   - ? Stock NO descontado

---

**Estado:** ? IMPLEMENTACIÓN COMPLETA  
**Compilación:** ? EXITOSA  
**Fecha:** 2025-01-09

**¡Checkout funcional con ventas pendientes!** ???
