# ?? IMPLEMENTACIÓN COMPLETA - Correcciones Pendientes

## ?? ARCHIVOS A CREAR/MODIFICAR

Total de archivos: **8**
- **Crear:** 5 archivos nuevos
- **Modificar:** 3 archivos existentes

---

## ?? CORRECCIÓN 3: Cliente - Mis Compras

### Archivo 1: `Controllers/ClienteController.cs` (CREAR)

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SciFiHub.Web.Controllers.Common;
using SciFiHub.Web.DTOs.Venta;
using SciFiHub.Web.Services.Interfaces;

namespace SciFiHub.Web.Controllers;

[Authorize(Roles = "Cliente")]
public class ClienteController : BaseController
{
    private readonly IVentaService _ventaService;
    private readonly ILogger<ClienteController> _logger;

    public ClienteController(
        IVentaService ventaService,
        ILogger<ClienteController> logger)
    {
        _ventaService = ventaService;
        _logger = logger;
    }

    // GET: /Cliente/MisCompras
    public async Task<IActionResult> MisCompras()
    {
        try
        {
            var clienteId = GetCurrentUserId();
            var result = await _ventaService.ObtenerVentasPorClienteAsync(clienteId);

            if (!result.Success)
            {
                AddErrorMessage("Error al cargar tus compras");
                return View(Enumerable.Empty<VentaDTO>());
            }

            return View(result.Data ?? Enumerable.Empty<VentaDTO>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar compras del cliente");
            AddErrorMessage("Error al cargar tus compras");
            return View(Enumerable.Empty<VentaDTO>());
        }
    }

    // GET: /Cliente/DetalleCompra/{id}
    public async Task<IActionResult> DetalleCompra(Guid id)
    {
        try
        {
            var clienteId = GetCurrentUserId();
            var result = await _ventaService.ObtenerVentaPorIdAsync(id);

            if (!result.Success || result.Data == null)
            {
                AddErrorMessage("Compra no encontrada");
                return RedirectToAction(nameof(MisCompras));
            }

            // Verificar que la venta pertenece al cliente
            if (result.Data.ClienteId != clienteId)
            {
                AddErrorMessage("No tienes permiso para ver esta compra");
                return RedirectToAction(nameof(MisCompras));
            }

            return View(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar detalle de compra {VentaId}", id);
            AddErrorMessage("Error al cargar el detalle de la compra");
            return RedirectToAction(nameof(MisCompras));
        }
    }
}
```

---

### Archivo 2: `Views/Cliente/MisCompras.cshtml` (CREAR)

```cshtml
@model IEnumerable<SciFiHub.Web.DTOs.Venta.VentaDTO>
@{
    ViewData["Title"] = "Mis Compras";
}

<div class="container mt-4">
    <div class="d-flex justify-content-between align-items-center mb-4">
        <h2><i class="bi bi-bag-check"></i> Mis Compras</h2>
        <a href="@Url.Action("Index", "Catalogo")" class="btn btn-primary">
            <i class="bi bi-arrow-left"></i> Volver al Catálogo
        </a>
    </div>

    @if (!Model.Any())
    {
        <div class="alert alert-info">
            <i class="bi bi-info-circle"></i>
            Aún no has realizado ninguna compra.
            <a href="@Url.Action("Index", "Catalogo")" class="alert-link">¡Explora nuestro catálogo!</a>
        </div>
    }
    else
    {
        <div class="table-responsive">
            <table class="table table-hover">
                <thead class="table-dark">
                    <tr>
                        <th>Número</th>
                        <th>Fecha</th>
                        <th>Items</th>
                        <th>Total</th>
                        <th>Estado</th>
                        <th>Acciones</th>
                    </tr>
                </thead>
                <tbody>
                    @foreach (var venta in Model)
                    {
                        <tr>
                            <td><strong>@venta.NumeroVenta</strong></td>
                            <td>@venta.FechaVenta.ToString("dd/MM/yyyy HH:mm")</td>
                            <td>@venta.Detalles.Sum(d => d.Cantidad) items</td>
                            <td><strong>S/ @venta.Total.ToString("N2")</strong></td>
                            <td>
                                @switch (venta.EstadoVenta)
                                {
                                    case "Pendiente":
                                        <span class="badge bg-warning text-dark">Pendiente</span>
                                        break;
                                    case "Completada":
                                        <span class="badge bg-success">Completada</span>
                                        break;
                                    case "Cancelada":
                                        <span class="badge bg-danger">Cancelada</span>
                                        break;
                                    default:
                                        <span class="badge bg-secondary">@venta.EstadoVenta</span>
                                        break;
                                }
                            </td>
                            <td>
                                <a href="@Url.Action("DetalleCompra", new { id = venta.Id })" 
                                   class="btn btn-sm btn-info" 
                                   title="Ver Detalle">
                                    <i class="bi bi-eye"></i> Ver
                                </a>
                            </td>
                        </tr>
                    }
                </tbody>
            </table>
        </div>

        <div class="row mt-4">
            <div class="col-md-6">
                <div class="card">
                    <div class="card-body">
                        <h5 class="card-title">Resumen de Compras</h5>
                        <p><strong>Total de compras:</strong> @Model.Count()</p>
                        <p><strong>Monto total invertido:</strong> S/ @Model.Sum(v => v.Total).ToString("N2")</p>
                        <p><strong>Compras completadas:</strong> @Model.Count(v => v.EstadoVenta == "Completada")</p>
                    </div>
                </div>
            </div>
        </div>
    }
</div>
```

---

### Archivo 3: `Views/Cliente/DetalleCompra.cshtml` (CREAR)

```cshtml
@model SciFiHub.Web.DTOs.Venta.VentaDTO
@{
    ViewData["Title"] = "Detalle de Compra";
}

<div class="container mt-4">
    <div class="d-flex justify-content-between align-items-center mb-4">
        <h2><i class="bi bi-receipt"></i> Detalle de Compra</h2>
        <a href="@Url.Action("MisCompras")" class="btn btn-secondary">
            <i class="bi bi-arrow-left"></i> Volver a Mis Compras
        </a>
    </div>

    <div class="card mb-4">
        <div class="card-header bg-primary text-white">
            <h5 class="mb-0">Información de la Compra</h5>
        </div>
        <div class="card-body">
            <div class="row">
                <div class="col-md-6">
                    <p><strong>Número de Compra:</strong> @Model.NumeroVenta</p>
                    <p><strong>Fecha:</strong> @Model.FechaVenta.ToString("dd/MM/yyyy HH:mm")</p>
                    <p><strong>Estado:</strong>
                        @switch (Model.EstadoVenta)
                        {
                            case "Pendiente":
                                <span class="badge bg-warning text-dark">Pendiente</span>
                                break;
                            case "Completada":
                                <span class="badge bg-success">Completada</span>
                                break;
                            case "Cancelada":
                                <span class="badge bg-danger">Cancelada</span>
                                break;
                            default:
                                <span class="badge bg-secondary">@Model.EstadoVenta</span>
                                break;
                        }
                    </p>
                </div>
                <div class="col-md-6">
                    <p><strong>Método de Pago:</strong> @Model.MetodoPago</p>
                    @if (!string.IsNullOrEmpty(Model.DireccionEnvio))
                    {
                        <p><strong>Dirección de Envío:</strong> @Model.DireccionEnvio</p>
                    }
                </div>
            </div>
        </div>
    </div>

    <div class="card mb-4">
        <div class="card-header">
            <h5 class="mb-0">Items Comprados</h5>
        </div>
        <div class="card-body">
            <div class="table-responsive">
                <table class="table">
                    <thead>
                        <tr>
                            <th>Libro</th>
                            <th>Cantidad</th>
                            <th>Precio Unit.</th>
                            <th>Descuento</th>
                            <th>Subtotal</th>
                        </tr>
                    </thead>
                    <tbody>
                        @foreach (var detalle in Model.Detalles)
                        {
                            <tr>
                                <td>@detalle.LibroTitulo</td>
                                <td>@detalle.Cantidad</td>
                                <td>S/ @detalle.PrecioUnitario.ToString("N2")</td>
                                <td>S/ @detalle.Descuento.ToString("N2")</td>
                                <td><strong>S/ @detalle.Subtotal.ToString("N2")</strong></td>
                            </tr>
                        }
                    </tbody>
                    <tfoot>
                        <tr>
                            <th colspan="4" class="text-end">Subtotal:</th>
                            <th>S/ @Model.Subtotal.ToString("N2")</th>
                        </tr>
                        <tr>
                            <th colspan="4" class="text-end">Descuento:</th>
                            <th>S/ @Model.Descuento.ToString("N2")</th>
                        </tr>
                        <tr>
                            <th colspan="4" class="text-end">IGV (18%):</th>
                            <th>S/ @Model.IGV.ToString("N2")</th>
                        </tr>
                        <tr class="table-primary">
                            <th colspan="4" class="text-end">TOTAL:</th>
                            <th>S/ @Model.Total.ToString("N2")</th>
                        </tr>
                    </tfoot>
                </table>
            </div>
        </div>
    </div>

    @if (!string.IsNullOrEmpty(Model.NotasVenta))
    {
        <div class="alert alert-info">
            <strong>Notas:</strong> @Model.NotasVenta
        </div>
    }
</div>
```

---

## ?? CORRECCIÓN 4: Carrito de Compras

### Archivo 4: `Controllers/Api/CarritoController.cs` (CREAR)

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SciFiHub.Web.DTOs.Carrito;
using SciFiHub.Web.Services.Interfaces;
using System.Security.Claims;

namespace SciFiHub.Web.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Cliente")]
public class CarritoController : ControllerBase
{
    private readonly ICarritoService _carritoService;
    private readonly ILogger<CarritoController> _logger;

    public CarritoController(
        ICarritoService carritoService,
        ILogger<CarritoController> logger)
    {
        _carritoService = carritoService;
        _logger = logger;
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }

    // POST: /api/Carrito/agregar
    [HttpPost("agregar")]
    public async Task<IActionResult> AgregarItem([FromBody] AgregarItemCarritoDTO dto)
    {
        try
        {
            var clienteId = GetCurrentUserId();
            var result = await _carritoService.AgregarItemAsync(clienteId, dto.LibroId, dto.Cantidad);

            if (!result.Success)
            {
                return BadRequest(new { success = false, error = result.ErrorMessage });
            }

            return Ok(new { success = true, message = "Libro agregado al carrito" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al agregar item al carrito");
            return StatusCode(500, new { success = false, error = "Error al agregar al carrito" });
        }
    }

    // GET: /api/Carrito/obtener
    [HttpGet("obtener")]
    public async Task<IActionResult> ObtenerCarrito()
    {
        try
        {
            var clienteId = GetCurrentUserId();
            var result = await _carritoService.ObtenerCarritoActivoAsync(clienteId);

            if (!result.Success)
            {
                return Ok(new { success = false, data = (object?)null });
            }

            return Ok(new { success = true, data = result.Data });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener carrito");
            return StatusCode(500, new { success = false, error = "Error al obtener carrito" });
        }
    }

    // DELETE: /api/Carrito/eliminar/{libroId}
    [HttpDelete("eliminar/{libroId}")]
    public async Task<IActionResult> EliminarItem(Guid libroId)
    {
        try
        {
            var clienteId = GetCurrentUserId();
            var result = await _carritoService.EliminarItemAsync(clienteId, libroId);

            if (!result.Success)
            {
                return BadRequest(new { success = false, error = result.ErrorMessage });
            }

            return Ok(new { success = true, message = "Item eliminado del carrito" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar item");
            return StatusCode(500, new { success = false, error = "Error al eliminar item" });
        }
    }

    // PUT: /api/Carrito/actualizar
    [HttpPut("actualizar")]
    public async Task<IActionResult> ActualizarCantidad([FromBody] ActualizarCantidadDTO dto)
    {
        try
        {
            var clienteId = GetCurrentUserId();
            var result = await _carritoService.ActualizarCantidadAsync(clienteId, dto.LibroId, dto.Cantidad);

            if (!result.Success)
            {
                return BadRequest(new { success = false, error = result.ErrorMessage });
            }

            return Ok(new { success = true, message = "Cantidad actualizada" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar cantidad");
            return StatusCode(500, new { success = false, error = "Error al actualizar cantidad" });
        }
    }

    // POST: /api/Carrito/checkout
    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout([FromBody] CheckoutDTO dto)
    {
        try
        {
            var clienteId = GetCurrentUserId();
            var result = await _carritoService.ConvertirCarritoAVentaAsync(clienteId, dto);

            if (!result.Success)
            {
                return BadRequest(new { success = false, error = result.ErrorMessage });
            }

            return Ok(new { success = true, data = result.Data, message = "Compra realizada exitosamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en checkout");
            return StatusCode(500, new { success = false, error = "Error al procesar la compra" });
        }
    }
}
```

---

### Archivo 5: `wwwroot/js/carrito-cliente.js` (CREAR)

```javascript
// Funciones del carrito de compras para clientes

let carritoItems = [];

// Cargar carrito al iniciar
document.addEventListener('DOMContentLoaded', () => {
    cargarCarrito();
    actualizarContadorCarrito();
});

// Agregar libro al carrito
async function agregarAlCarrito(libroId) {
    try {
        const response = await fetch('/api/Carrito/agregar', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                libroId: libroId,
                cantidad: 1
            })
        });

        const data = await response.json();

        if (data.success) {
            mostrarNotificacion('? Libro agregado al carrito', 'success');
            await actualizarContadorCarrito();
        } else {
            mostrarNotificacion('? ' + data.error, 'error');
        }
    } catch (error) {
        console.error('Error al agregar al carrito:', error);
        mostrarNotificacion('? Error al agregar al carrito', 'error');
    }
}

// Cargar carrito del servidor
async function cargarCarrito() {
    try {
        const response = await fetch('/api/Carrito/obtener');
        const data = await response.json();

        if (data.success && data.data) {
            carritoItems = data.data.items || [];
            renderizarCarrito();
        }
    } catch (error) {
        console.error('Error al cargar carrito:', error);
    }
}

// Actualizar contador de items en el navbar
async function actualizarContadorCarrito() {
    try {
        const response = await fetch('/api/Carrito/obtener');
        const data = await response.json();

        const contador = document.getElementById('carrito-contador');
        if (contador && data.success && data.data) {
            const totalItems = data.data.items.reduce((sum, item) => sum + item.cantidad, 0);
            contador.textContent = totalItems;
            contador.style.display = totalItems > 0 ? 'inline' : 'none';
        }
    } catch (error) {
        console.error('Error al actualizar contador:', error);
    }
}

// Eliminar item del carrito
async function eliminarDelCarrito(libroId) {
    try {
        const response = await fetch(`/api/Carrito/eliminar/${libroId}`, {
            method: 'DELETE'
        });

        const data = await response.json();

        if (data.success) {
            mostrarNotificacion('Item eliminado', 'success');
            await cargarCarrito();
            await actualizarContadorCarrito();
        } else {
            mostrarNotificacion('Error: ' + data.error, 'error');
        }
    } catch (error) {
        console.error('Error al eliminar:', error);
    }
}

// Actualizar cantidad
async function actualizarCantidad(libroId, cantidad) {
    try {
        const response = await fetch('/api/Carrito/actualizar', {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                libroId: libroId,
                cantidad: cantidad
            })
        });

        const data = await response.json();

        if (data.success) {
            await cargarCarrito();
            await actualizarContadorCarrito();
        } else {
            mostrarNotificacion('Error: ' + data.error, 'error');
        }
    } catch (error) {
        console.error('Error al actualizar cantidad:', error);
    }
}

// Realizar checkout
async function realizarCheckout() {
    const direccion = document.getElementById('direccion').value;
    const metodoPago = document.getElementById('metodoPago').value;

    if (!direccion || !metodoPago) {
        mostrarNotificacion('Por favor complete todos los campos', 'warning');
        return;
    }

    try {
        const response = await fetch('/api/Carrito/checkout', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                direccionEnvio: direccion,
                metodoPago: metodoPago
            })
        });

        const data = await response.json();

        if (data.success) {
            mostrarNotificacion('? Compra realizada exitosamente', 'success');
            setTimeout(() => {
                window.location.href = '/Cliente/DetalleCompra/' + data.data.id;
            }, 1500);
        } else {
            mostrarNotificacion('? ' + data.error, 'error');
        }
    } catch (error) {
        console.error('Error en checkout:', error);
        mostrarNotificacion('? Error al procesar la compra', 'error');
    }
}

// Renderizar carrito en la vista
function renderizarCarrito() {
    const container = document.getElementById('carrito-items');
    if (!container) return;

    if (carritoItems.length === 0) {
        container.innerHTML = `
            <div class="alert alert-info">
                <i class="bi bi-cart-x"></i> Tu carrito está vacío.
                <a href="/Catalogo" class="alert-link">¡Explora nuestro catálogo!</a>
            </div>
        `;
        return;
    }

    let html = '<div class="list-group">';
    let total = 0;

    carritoItems.forEach(item => {
        total += item.subtotal;
        html += `
            <div class="list-group-item">
                <div class="row align-items-center">
                    <div class="col-md-6">
                        <h6>${item.libroTitulo}</h6>
                        <small>Precio: S/ ${item.precioUnitario.toFixed(2)}</small>
                    </div>
                    <div class="col-md-3">
                        <div class="input-group">
                            <button class="btn btn-sm btn-outline-secondary" 
                                    onclick="actualizarCantidad('${item.libroId}', ${item.cantidad - 1})">-</button>
                            <input type="number" class="form-control" value="${item.cantidad}" readonly>
                            <button class="btn btn-sm btn-outline-secondary" 
                                    onclick="actualizarCantidad('${item.libroId}', ${item.cantidad + 1})">+</button>
                        </div>
                    </div>
                    <div class="col-md-2">
                        <strong>S/ ${item.subtotal.toFixed(2)}</strong>
                    </div>
                    <div class="col-md-1">
                        <button class="btn btn-sm btn-danger" 
                                onclick="eliminarDelCarrito('${item.libroId}')">
                            <i class="bi bi-trash"></i>
                        </button>
                    </div>
                </div>
            </div>
        `;
    });

    html += '</div>';
    html += `
        <div class="mt-3">
            <h4>Total: S/ ${total.toFixed(2)}</h4>
        </div>
    `;

    container.innerHTML = html;
}

// Mostrar notificaciones
function mostrarNotificacion(mensaje, tipo) {
    // Implementar sistema de notificaciones (toast, alert, etc.)
    alert(mensaje);
}
```

---

## ?? CORRECCIÓN 5: Checkbox "Solo disponibles"

### Modificación en: `wwwroot/js/catalogo.js` (SI EXISTE) o crear nuevo

```javascript
// Script para el catálogo de libros

document.addEventListener('DOMContentLoaded', () => {
    const checkbox = document.getElementById('soloDisponibles');
    
    if (checkbox) {
        checkbox.addEventListener('change', function() {
            // Obtener parámetros actuales de la URL
            const params = new URLSearchParams(window.location.search);
            
            // Actualizar el parámetro soloDisponibles
            if (this.checked) {
                params.set('soloDisponibles', 'true');
            } else {
                params.delete('soloDisponibles');
            }
            
            // Recargar página con nuevos parámetros
            window.location.search = params.toString();
        });
    }
});
```

---

## ?? MODIFICACIONES ADICIONALES

### Modificar `Views/Shared/_Layout.cshtml`

Agregar contador de carrito en el navbar:

```cshtml
@if (User.IsInRole("Cliente"))
{
    <li class="nav-item">
        <a class="nav-link" href="@Url.Action("Index", "Carrito")">
            <i class="bi bi-cart"></i> Carrito
            <span id="carrito-contador" class="badge bg-danger" style="display: none;">0</span>
        </a>
    </li>
    <li class="nav-item">
        <a class="nav-link" href="@Url.Action("MisCompras", "Cliente")">
            <i class="bi bi-bag-check"></i> Mis Compras
        </a>
    </li>
}
```

Agregar script del carrito:

```cshtml
@if (User.IsInRole("Cliente"))
{
    <script src="~/js/carrito-cliente.js"></script>
}
```

---

### Modificar `Views/Catalogo/Detalle.cshtml`

Agregar botón "Agregar al Carrito":

```cshtml
@if (User.IsInRole("Cliente"))
{
    <button class="btn btn-success btn-lg" 
            onclick="agregarAlCarrito('@Model.Id')">
        <i class="bi bi-cart-plus"></i> Agregar al Carrito
    </button>
}
```

---

## ? RESUMEN DE IMPLEMENTACIÓN

### Archivos a Crear:
1. `Controllers/ClienteController.cs`
2. `Views/Cliente/MisCompras.cshtml`
3. `Views/Cliente/DetalleCompra.cshtml`
4. `Controllers/Api/CarritoController.cs`
5. `wwwroot/js/carrito-cliente.js`

### Archivos a Modificar:
1. `Views/Shared/_Layout.cshtml` (agregar links y scripts)
2. `Views/Catalogo/Detalle.cshtml` (agregar botón carrito)
3. `wwwroot/js/catalogo.js` (crear o modificar para checkbox)

**Total:** 8 archivos

---

## ?? ORDEN DE IMPLEMENTACIÓN RECOMENDADO

1. ? Crear `ClienteController.cs`
2. ? Crear vistas de MisCompras
3. ? Probar navegación del cliente
4. ? Crear `CarritoController` (API)
5. ? Crear `carrito-cliente.js`
6. ? Modificar `_Layout.cshtml`
7. ? Modificar `Detalle.cshtml`
8. ? Probar flujo completo de carrito

**Tiempo Estimado:** 2-3 horas

---

**Estado:** ?? PENDIENTE DE IMPLEMENTACIÓN  
**Prioridad:** ?? ALTA
