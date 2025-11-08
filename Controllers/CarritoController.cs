using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SciFiHub.Web.Controllers.Common;
using SciFiHub.Web.DTOs.Carrito;
using SciFiHub.Web.Services.Interfaces;

namespace SciFiHub.Web.Controllers;

/// <summary>
/// Controller para gestión del carrito de compras
/// </summary>
[Authorize(Roles = "Cliente")]
public class CarritoController : BaseController
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

    // GET: /Carrito
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        try
        {
            if (!CurrentUserId.HasValue)
            {
                return RedirectToAction("Login", "Auth");
            }

            var result = await _carritoService.ObtenerCarritoPorClienteAsync(CurrentUserId.Value);

            if (!result.Success)
            {
                AddErrorMessage(result.ErrorMessage ?? "Error al cargar el carrito");
                return View(new CarritoDTO 
                { 
                    Id = Guid.Empty, 
                    ClienteId = CurrentUserId.Value,
                    Items = new List<ItemCarritoDTO>() 
                });
            }

            return View(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar el carrito");
            AddErrorMessage("Error al cargar el carrito");
            return View(new CarritoDTO 
            { 
                Id = Guid.Empty, 
                ClienteId = CurrentUserId ?? Guid.Empty,
                Items = new List<ItemCarritoDTO>() 
            });
        }
    }

    // POST: /Carrito/AgregarItem
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AgregarItem([FromForm] AgregarAlCarritoDTO agregarDto)
    {
        try
        {
            if (!CurrentUserId.HasValue)
            {
                return JsonError("Debes iniciar sesión para agregar items al carrito");
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return JsonError("Datos inválidos", errors);
            }

            var result = await _carritoService.AgregarItemAsync(CurrentUserId.Value, agregarDto);

            if (!result.Success)
            {
                return JsonError(result.ErrorMessage ?? "Error al agregar item");
            }

            return JsonSuccess(new { cantidadItems = result.Data!.CantidadItems }, "Libro agregado al carrito correctamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al agregar item al carrito");
            return JsonError("Error al agregar item al carrito");
        }
    }

    // POST: /Carrito/ActualizarCantidad
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ActualizarCantidad([FromForm] ActualizarCantidadCarritoDTO actualizarDto)
    {
        try
        {
            if (!CurrentUserId.HasValue)
            {
                return JsonError("Debes iniciar sesión");
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return JsonError("Datos inválidos", errors);
            }

            var result = await _carritoService.ActualizarCantidadAsync(CurrentUserId.Value, actualizarDto);

            if (!result.Success)
            {
                return JsonError(result.ErrorMessage ?? "Error al actualizar cantidad");
            }

            return JsonSuccess(
                new 
                { 
                    subtotal = result.Data!.Subtotal,
                    igv = result.Data.IGV,
                    total = result.Data.Total,
                    cantidadItems = result.Data.CantidadItems
                }, 
                "Cantidad actualizada");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar cantidad");
            return JsonError("Error al actualizar cantidad");
        }
    }

    // POST: /Carrito/EliminarItem
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EliminarItem([FromForm] Guid libroId)
    {
        try
        {
            if (!CurrentUserId.HasValue)
            {
                return JsonError("Debes iniciar sesión");
            }

            var result = await _carritoService.EliminarItemAsync(CurrentUserId.Value, libroId);

            if (!result.Success)
            {
                return JsonError(result.ErrorMessage ?? "Error al eliminar item");
            }

            return JsonSuccess(
                new 
                { 
                    subtotal = result.Data!.Subtotal,
                    igv = result.Data.IGV,
                    total = result.Data.Total,
                    cantidadItems = result.Data.CantidadItems
                }, 
                "Libro eliminado del carrito");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar item del carrito");
            return JsonError("Error al eliminar item");
        }
    }

    // POST: /Carrito/Limpiar
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Limpiar()
    {
        try
        {
            if (!CurrentUserId.HasValue)
            {
                return RedirectToAction("Login", "Auth");
            }

            var result = await _carritoService.LimpiarCarritoAsync(CurrentUserId.Value);

            if (!result.Success)
            {
                AddErrorMessage(result.ErrorMessage ?? "Error al limpiar el carrito");
            }
            else
            {
                AddSuccessMessage("Carrito limpiado exitosamente");
            }

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al limpiar carrito");
            AddErrorMessage("Error al limpiar el carrito");
            return RedirectToAction(nameof(Index));
        }
    }

    // GET: /Carrito/ObtenerResumen (AJAX)
    [HttpGet]
    public async Task<IActionResult> ObtenerResumen()
    {
        try
        {
            if (!CurrentUserId.HasValue)
            {
                return JsonSuccess(new { cantidadItems = 0, total = 0 });
            }

            var result = await _carritoService.ObtenerResumenCarritoAsync(CurrentUserId.Value);

            if (!result.Success)
            {
                return JsonError(result.ErrorMessage ?? "Error al obtener resumen");
            }

            return JsonSuccess(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener resumen del carrito");
            return JsonError("Error al obtener resumen");
        }
    }

    // GET: /Carrito/Checkout
    [HttpGet]
    public async Task<IActionResult> Checkout()
    {
        try
        {
            if (!CurrentUserId.HasValue)
            {
                return RedirectToAction("Login", "Auth");
            }

            var result = await _carritoService.ObtenerCarritoPorClienteAsync(CurrentUserId.Value);

            if (!result.Success)
            {
                AddErrorMessage(result.ErrorMessage ?? "Error al cargar el carrito");
                return RedirectToAction(nameof(Index));
            }

            if (!result.Data!.TieneItems)
            {
                AddWarningMessage("Tu carrito está vacío");
                return RedirectToAction("Index", "Catalogo");
            }

            // Validar stock antes de proceder
            var stockValido = await _carritoService.ValidarStockCarritoAsync(CurrentUserId.Value);
            if (!stockValido.Success || !stockValido.Data)
            {
                AddErrorMessage("Algunos items no tienen stock suficiente. Por favor revisa tu carrito.");
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Carrito = result.Data;
            return View();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar checkout");
            AddErrorMessage("Error al cargar el checkout");
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: /Carrito/ProcesarCheckout
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ProcesarCheckout([FromForm] CheckoutDTO checkoutDto, [FromForm] string? cantidadesTemporales)
    {
        try
        {
            if (!CurrentUserId.HasValue)
            {
                _logger.LogWarning("? Usuario no autenticado en checkout");
                return RedirectToAction("Login", "Auth");
            }

            _logger.LogInformation("?? === INICIO CHECKOUT ===");
            _logger.LogInformation("?? Cliente: {ClienteId}", CurrentUserId.Value);
            _logger.LogInformation("?? Método de pago: {MetodoPago}", checkoutDto.MetodoPago);
            _logger.LogInformation("?? Cantidades temporales: {Cantidades}", cantidadesTemporales ?? "ninguna");
            _logger.LogInformation("?? DireccionEnvioJson: {DireccionJson}", checkoutDto.DireccionEnvioJson ?? "vacío");

            // ? DESERIALIZAR DIRECCIÓN DE ENVÍO
            if (!string.IsNullOrEmpty(checkoutDto.DireccionEnvioJson))
            {
                try
                {
                    var direccionDict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(checkoutDto.DireccionEnvioJson);
                    
                    if (direccionDict != null)
                    {
                        checkoutDto.DireccionEnvio = new DTOs.Venta.DireccionEnvioDTO
                        {
                            Calle = direccionDict.GetValueOrDefault("calle", ""),
                            Ciudad = direccionDict.GetValueOrDefault("ciudad", ""),
                            Departamento = direccionDict.GetValueOrDefault("departamento", ""),
                            Pais = direccionDict.GetValueOrDefault("pais", "Perú"),
                            CodigoPostal = direccionDict.GetValueOrDefault("codigoPostal", null),
                            Referencia = direccionDict.GetValueOrDefault("referencia", null)
                        };
                        
                        _logger.LogInformation("? Dirección deserializada: {Direccion}", 
                            $"{checkoutDto.DireccionEnvio.Calle}, {checkoutDto.DireccionEnvio.Ciudad}, {checkoutDto.DireccionEnvio.Departamento}");
                    }
                    else
                    {
                        _logger.LogWarning("?? direccionDict es null");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "? Error al deserializar dirección");
                    AddErrorMessage("Error al procesar la dirección de envío");
                    return RedirectToAction(nameof(Checkout));
                }
            }
            else
            {
                _logger.LogWarning("?? DireccionEnvioJson está vacío");
            }

            // Validar que se tenga dirección
            if (checkoutDto.DireccionEnvio == null)
            {
                _logger.LogWarning("? Dirección de envío no proporcionada");
                AddErrorMessage("Debes proporcionar una dirección de envío válida");
                return RedirectToAction(nameof(Checkout));
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("? ModelState inválido");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    _logger.LogWarning("  - {Error}", error.ErrorMessage);
                }
                
                var carritoResult = await _carritoService.ObtenerCarritoPorClienteAsync(CurrentUserId.Value);
                ViewBag.Carrito = carritoResult.Data;
                return View("Checkout", checkoutDto);
            }

            // ? Pasar cantidades temporales
            checkoutDto.CantidadesTemporalesJson = cantidadesTemporales;

            _logger.LogInformation("?? Llamando a ProcesarCheckoutAsync...");
            
            var result = await _carritoService.ProcesarCheckoutAsync(CurrentUserId.Value, checkoutDto);

            if (!result.Success)
            {
                _logger.LogError("? Error en checkout: {Error}", result.ErrorMessage);
                AddErrorMessage(result.ErrorMessage ?? "Error al procesar la compra");
                var carritoResult = await _carritoService.ObtenerCarritoPorClienteAsync(CurrentUserId.Value);
                ViewBag.Carrito = carritoResult.Data;
                return View("Checkout", checkoutDto);
            }

            _logger.LogInformation("? Checkout exitoso. Venta: {NumeroVenta}", result.Data!.NumeroVenta);
            _logger.LogInformation("?? === FIN CHECKOUT ===");

            AddSuccessMessage($"¡Compra realizada exitosamente! Número de pedido: {result.Data!.NumeroVenta}. Estado: Pendiente de confirmación.");
            return RedirectToAction(nameof(Confirmacion), new { ventaId = result.Data.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "? EXCEPCIÓN en ProcesarCheckout");
            _logger.LogError("Mensaje: {Message}", ex.Message);
            _logger.LogError("StackTrace: {StackTrace}", ex.StackTrace);
            
            AddErrorMessage("Error inesperado al procesar la compra. Por favor intenta nuevamente.");
            return RedirectToAction(nameof(Checkout));
        }
    }

    // GET: /Carrito/Confirmacion/{ventaId}
    [HttpGet]
    public async Task<IActionResult> Confirmacion(Guid ventaId)
    {
        try
        {
            AddSuccessMessage("¡Tu pedido se ha procesado correctamente! Está pendiente de confirmación por un vendedor.");
            return View(ventaId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al mostrar confirmación");
            return RedirectToAction("Index", "Home");
        }
    }
}
