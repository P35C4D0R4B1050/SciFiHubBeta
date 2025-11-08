using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SciFiHub.Web.DTOs.Carrito;
using SciFiHub.Web.Services.Interfaces;
using System.Security.Claims;

namespace SciFiHub.Web.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Cliente")]
public class CarritoApiController : ControllerBase
{
    private readonly ICarritoService _carritoService;
    private readonly ILogger<CarritoApiController> _logger;

    public CarritoApiController(
        ICarritoService carritoService,
        ILogger<CarritoApiController> logger)
    {
        _carritoService = carritoService;
        _logger = logger;
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }

    // POST: /api/CarritoApi/agregar
    [HttpPost("agregar")]
    public async Task<IActionResult> AgregarItem([FromBody] AgregarAlCarritoDTO dto)
    {
        try
        {
            var clienteId = GetCurrentUserId();
            
            if (clienteId == Guid.Empty)
            {
                return Unauthorized(new { success = false, error = "Usuario no autenticado" });
            }

            var result = await _carritoService.AgregarItemAsync(clienteId, dto);

            if (!result.Success)
            {
                return BadRequest(new { success = false, error = result.ErrorMessage });
            }

            return Ok(new { success = true, message = "Agregado", data = result.Data });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error agregar");
            return StatusCode(500, new { success = false, error = "Error" });
        }
    }

    // GET: /api/CarritoApi/obtener
    [HttpGet("obtener")]
    public async Task<IActionResult> ObtenerCarrito()
    {
        try
        {
            var clienteId = GetCurrentUserId();
            
            if (clienteId == Guid.Empty)
            {
                return Unauthorized(new { success = false, error = "No autenticado" });
            }

            var result = await _carritoService.ObtenerCarritoPorClienteAsync(clienteId);

            if (!result.Success)
            {
                return Ok(new { success = true, data = (object?)null });
            }

            return Ok(new { success = true, data = result.Data });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error obtener");
            return StatusCode(500, new { success = false, error = "Error" });
        }
    }

    // DELETE: /api/CarritoApi/eliminar/{libroId}
    [HttpDelete("eliminar/{libroId}")]
    public async Task<IActionResult> EliminarItem(Guid libroId)
    {
        try
        {
            var clienteId = GetCurrentUserId();
            
            if (clienteId == Guid.Empty)
            {
                return Unauthorized(new { success = false, error = "No autenticado" });
            }

            var result = await _carritoService.EliminarItemAsync(clienteId, libroId);

            if (!result.Success)
            {
                return BadRequest(new { success = false, error = result.ErrorMessage });
            }

            return Ok(new { success = true, message = "Eliminado", data = result.Data });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error eliminar");
            return StatusCode(500, new { success = false, error = "Error" });
        }
    }

    // DELETE: /api/CarritoApi/vaciar
    [HttpDelete("vaciar")]
    public async Task<IActionResult> VaciarCarrito()
    {
        try
        {
            var clienteId = GetCurrentUserId();
            
            if (clienteId == Guid.Empty)
            {
                return Unauthorized(new { success = false, error = "No autenticado" });
            }

            var result = await _carritoService.LimpiarCarritoAsync(clienteId);

            if (!result.Success)
            {
                return BadRequest(new { success = false, error = result.ErrorMessage });
            }

            return Ok(new { success = true, message = "Vaciado" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error vaciar");
            return StatusCode(500, new { success = false, error = "Error" });
        }
    }

    // GET: /api/CarritoApi/resumen
    [HttpGet("resumen")]
    public async Task<IActionResult> ObtenerResumen()
    {
        try
        {
            var clienteId = GetCurrentUserId();
            
            if (clienteId == Guid.Empty)
            {
                return Ok(new { success = true, data = new { cantidadItems = 0, total = 0 } });
            }

            var result = await _carritoService.ObtenerResumenCarritoAsync(clienteId);

            if (!result.Success)
            {
                return Ok(new { success = true, data = new { cantidadItems = 0, total = 0 } });
            }

            return Ok(new { success = true, data = result.Data });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resumen");
            return Ok(new { success = true, data = new { cantidadItems = 0, total = 0 } });
        }
    }
}
