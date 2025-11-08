using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SciFiHub.Web.DTOs.Venta;
using SciFiHub.Web.Services.Interfaces;
using System.Security.Claims;

namespace SciFiHub.Web.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Vendedor,Administrador")]
public class VentasController : ControllerBase
{
    private readonly IVentaService _ventaService;
    private readonly IPdfService _pdfService;
    private readonly ILogger<VentasController> _logger;

    public VentasController(
        IVentaService ventaService,
        IPdfService pdfService,
        ILogger<VentasController> logger)
    {
        _ventaService = ventaService;
        _pdfService = pdfService;
        _logger = logger;
    }

    // POST: api/Ventas
    [HttpPost]
    public async Task<IActionResult> CrearVenta([FromBody] CrearVentaDTO model)
    {
        try
        {
            _logger.LogInformation("Iniciando creación de venta");
            _logger.LogInformation("Modelo recibido: {@Model}", model);
            
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                    
                _logger.LogWarning("ModelState inválido: {Errors}", string.Join(", ", errors));
                
                return BadRequest(new { 
                    success = false,
                    error = "Datos inválidos", 
                    errors = errors 
                });
            }

            // Obtener ID del vendedor desde el token
            var vendedorIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(vendedorIdClaim) || !Guid.TryParse(vendedorIdClaim, out var vendedorId))
            {
                _logger.LogWarning("Usuario no autenticado o claim inválido");
                return Unauthorized(new { 
                    success = false,
                    error = "Usuario no autenticado" 
                });
            }

            _logger.LogInformation("Vendedor ID: {VendedorId}", vendedorId);

            // Asignar vendedor al DTO
            var ventaConVendedor = model with { VendedorId = vendedorId };

            _logger.LogInformation("Llamando a RegistrarVentaAsync");
            var result = await _ventaService.RegistrarVentaAsync(ventaConVendedor);

            if (!result.Success)
            {
                _logger.LogWarning("Error al registrar venta: {Error}", result.ErrorMessage);
                return BadRequest(new { 
                    success = false,
                    error = result.ErrorMessage ?? "Error desconocido al registrar la venta"
                });
            }

            _logger.LogInformation("Venta registrada exitosamente: {NumeroVenta}", result.Data?.NumeroVenta);

            return Ok(new
            {
                success = true,
                message = "Venta registrada exitosamente",
                data = result.Data,
                numeroVenta = result.Data?.NumeroVenta
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al crear venta");
            return StatusCode(500, new { 
                success = false,
                error = $"Error al registrar la venta: {ex.Message}",
                details = ex.ToString()
            });
        }
    }

    // GET: api/Ventas/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerVenta(Guid id)
    {
        try
        {
            var result = await _ventaService.ObtenerVentaPorIdAsync(id);

            if (!result.Success)
            {
                return NotFound(new { error = "Venta no encontrada" });
            }

            return Ok(new
            {
                success = true,
                data = result.Data
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener venta {VentaId}", id);
            return StatusCode(500, new { error = "Error al obtener la venta" });
        }
    }

    // POST: api/Ventas/{id}/cancelar
    [HttpPost("{id}/cancelar")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> CancelarVenta(Guid id, [FromBody] CancelarVentaDTO model)
    {
        try
        {
            if (id != model.VentaId)
            {
                return BadRequest(new { error = "El ID no coincide" });
            }

            var result = await _ventaService.CancelarVentaAsync(model);

            if (!result.Success)
            {
                return BadRequest(new { error = result.ErrorMessage });
            }

            return Ok(new
            {
                success = true,
                message = "Venta cancelada exitosamente"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cancelar venta {VentaId}", id);
            return StatusCode(500, new { error = "Error al cancelar la venta" });
        }
    }

    // POST: api/Ventas/{id}/completar
    [HttpPost("{id}/completar")]
    public async Task<IActionResult> CompletarVenta(Guid id)
    {
        try
        {
            var result = await _ventaService.CompletarVentaAsync(id);

            if (!result.Success)
            {
                return BadRequest(new { error = result.ErrorMessage });
            }

            return Ok(new
            {
                success = true,
                message = "Venta completada exitosamente"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al completar venta {VentaId}", id);
            return StatusCode(500, new { error = "Error al completar la venta" });
        }
    }

    // PUT: api/Ventas/{id}/estado
    [HttpPut("{id}/estado")]
    public async Task<IActionResult> CambiarEstado(Guid id, [FromBody] CambiarEstadoVentaRequest request)
    {
        try
        {
            var result = await _ventaService.CambiarEstadoVentaAsync(id, request.Estado);

            if (!result.Success)
            {
                return BadRequest(new { error = result.ErrorMessage });
            }

            return Ok(new
            {
                success = true,
                message = "Estado de la venta actualizado exitosamente"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cambiar estado de venta {VentaId}", id);
            return StatusCode(500, new { error = "Error al cambiar el estado de la venta" });
        }
    }

    // GET: api/Ventas/estadisticas-mes
    [HttpGet("estadisticas-mes")]
    [Authorize(Roles = "Administrador,Admin")]
    public async Task<IActionResult> ObtenerEstadisticasMes()
    {
        try
        {
            // Obtener primer y último día del mes actual
            var ahora = DateTime.UtcNow;
            var primerDia = new DateTime(ahora.Year, ahora.Month, 1);
            var ultimoDia = primerDia.AddMonths(1).AddDays(-1);
            
            // Obtener todas las ventas del mes
            var filtro = new VentasFiltroDTO
            {
                PageNumber = 1,
                PageSize = 1000, // Suficiente para un mes
                FechaInicio = primerDia,
                FechaFin = ultimoDia,
                Estado = null // Todas las ventas
            };
            
            var result = await _ventaService.ObtenerVentasAsync(filtro);
            
            if (!result.Success)
            {
                return Ok(new { 
                    success = true, 
                    data = new { items = new List<VentaDTO>(), totalCount = 0 } 
                });
            }
            
            return Ok(new
            {
                success = true,
                data = result.Data
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener estadísticas del mes");
            return Ok(new { 
                success = true, 
                data = new { items = new List<VentaDTO>(), totalCount = 0 } 
            });
        }
    }

    // GET: api/Ventas/estadisticas
    [HttpGet("estadisticas")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> ObtenerEstadisticas()
    {
        try
        {
            var result = await _ventaService.ObtenerEstadisticasDelMesAsync();

            if (!result.Success)
            {
                return BadRequest(new { error = result.ErrorMessage });
            }

            return Ok(new
            {
                success = true,
                data = result.Data
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener estadísticas");
            return StatusCode(500, new { error = "Error al obtener estadísticas" });
        }
    }

    // GET: api/Ventas/libros-mas-vendidos
    [HttpGet("libros-mas-vendidos")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> ObtenerLibrosMasVendidos([FromQuery] int cantidad = 10)
    {
        try
        {
            var result = await _ventaService.ObtenerLibrosMasVendidosAsync(cantidad);

            if (!result.Success)
            {
                return BadRequest(new { error = result.ErrorMessage });
            }

            return Ok(new
            {
                success = true,
                data = result.Data
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener libros más vendidos");
            return StatusCode(500, new { error = "Error al obtener libros más vendidos" });
        }
    }

    // POST: api/Ventas/calcular-totales
    [HttpPost("calcular-totales")]
    public IActionResult CalcularTotales([FromBody] CalcularTotalesRequest request)
    {
        try
        {
            if (request.Items == null || !request.Items.Any())
            {
                return BadRequest(new { error = "Debe incluir al menos un item" });
            }

            decimal subtotal = 0;
            var itemsCalculados = new List<object>();

            foreach (var item in request.Items)
            {
                var precioConDescuento = item.PrecioUnitario - item.Descuento;
                var subtotalItem = precioConDescuento * item.Cantidad;
                subtotal += subtotalItem;

                itemsCalculados.Add(new
                {
                    libroId = item.LibroId,
                    cantidad = item.Cantidad,
                    precioUnitario = item.PrecioUnitario,
                    descuento = item.Descuento,
                    subtotal = subtotalItem
                });
            }

            // IGV 18%
            decimal igv = subtotal * 0.18m;
            decimal total = subtotal + igv;

            return Ok(new
            {
                success = true,
                data = new
                {
                    items = itemsCalculados,
                    subtotal = Math.Round(subtotal, 2),
                    igv = Math.Round(igv, 2),
                    total = Math.Round(total, 2)
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al calcular totales");
            return StatusCode(500, new { error = "Error al calcular totales" });
        }
    }

    // POST: api/Ventas/cancelar
    [HttpPost("cancelar")]
    public async Task<IActionResult> CancelarVentaV2([FromBody] CancelarVentaDTO dto)
    {
        try
        {
            _logger.LogInformation("Cancelando venta {VentaId}", dto.VentaId);
            
            var result = await _ventaService.CancelarVentaAsync(dto);
            
            if (!result.Success)
            {
                return BadRequest(new { success = false, error = result.ErrorMessage });
            }
            
            return Ok(new { success = true, message = "Venta cancelada exitosamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cancelar venta");
            return StatusCode(500, new { success = false, error = "Error al cancelar la venta" });
        }
    }

    // GET: api/Ventas/{id}/pdf
    [HttpGet("{id}/pdf")]
    [AllowAnonymous]
    public async Task<IActionResult> DescargarPDF(Guid id)
    {
        try
        {
            _logger.LogInformation("Generando PDF para venta {VentaId}", id);
            
            var result = await _ventaService.ObtenerVentaPorIdAsync(id);
            
            if (!result.Success || result.Data == null)
            {
                return NotFound(new { success = false, error = "Venta no encontrada" });
            }
            
            var pdf = _pdfService.GenerarBoletaPDF(result.Data);
            
            return File(pdf, "application/pdf", $"Boleta-{result.Data.NumeroVenta}.pdf");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar PDF");
            return StatusCode(500, new { success = false, error = "Error al generar PDF" });
        }
    }
}

public record CalcularTotalesRequest
{
    public List<ItemCalculoDTO> Items { get; init; } = new();
}

public record ItemCalculoDTO
{
    public Guid LibroId { get; init; }
    public int Cantidad { get; init; }
    public decimal PrecioUnitario { get; init; }
    public decimal Descuento { get; init; }
}

public record CambiarEstadoVentaRequest
{
    public SciFiHub.Domain.Enums.EstadoVenta Estado { get; init; }
}
