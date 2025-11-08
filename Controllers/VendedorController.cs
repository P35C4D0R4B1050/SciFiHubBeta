using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SciFiHub.Web.Controllers.Common;
using SciFiHub.Web.Services.Interfaces;

namespace SciFiHub.Web.Controllers;

[Authorize(Roles = "Vendedor,Administrador")]
public class VendedorController : BaseController
{
    private readonly ILibroService _libroService;
    private readonly IVentaService _ventaService;
    private readonly ILogger<VendedorController> _logger;

    public VendedorController(
        ILibroService libroService,
        IVentaService ventaService,
        ILogger<VendedorController> logger)
    {
        _libroService = libroService;
        _ventaService = ventaService;
        _logger = logger;
    }

    // GET: /Vendedor (Dashboard principal)
    public IActionResult Index()
    {
        return RedirectToAction(nameof(Inventario));
    }

    // GET: /Vendedor/Inventario
    public async Task<IActionResult> Inventario()
    {
        try
        {
            var result = await _libroService.ObtenerLibrosPaginadosAsync(new DTOs.Libro.LibrosFiltroDTO
            {
                PageNumber = 1,
                PageSize = 50,
                SoloDisponibles = false
            });

            if (!result.Success)
            {
                AddErrorMessage("Error al cargar el inventario");
                return View(Enumerable.Empty<DTOs.Libro.LibroCardDTO>());
            }

            return View(result.Data?.Items ?? Enumerable.Empty<DTOs.Libro.LibroCardDTO>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar inventario");
            AddErrorMessage("Error al cargar el inventario");
            return View(Enumerable.Empty<DTOs.Libro.LibroCardDTO>());
        }
    }

    // GET: /Vendedor/Ventas
    public async Task<IActionResult> Ventas()
    {
        try
        {
            var result = await _ventaService.ObtenerVentasPaginadasAsync(new DTOs.Venta.VentasFiltroDTO
            {
                PageNumber = 1,
                PageSize = 50
            });

            if (!result.Success)
            {
                AddErrorMessage("Error al cargar ventas");
                return View(Enumerable.Empty<DTOs.Venta.VentaDTO>());
            }

            return View(result.Data?.Items ?? Enumerable.Empty<DTOs.Venta.VentaDTO>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar ventas");
            AddErrorMessage("Error al cargar ventas");
            return View(Enumerable.Empty<DTOs.Venta.VentaDTO>());
        }
    }

    // GET: /Vendedor/CrearVenta
    public IActionResult CrearVenta()
    {
        return View();
    }

    // GET: /Vendedor/DetalleVenta/{id}
    public async Task<IActionResult> DetalleVenta(Guid id)
    {
        try
        {
            var result = await _ventaService.ObtenerVentaPorIdAsync(id);

            if (!result.Success)
            {
                AddErrorMessage("Venta no encontrada");
                return RedirectToAction(nameof(Ventas));
            }

            return View(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar detalle de venta {VentaId}", id);
            AddErrorMessage("Error al cargar el detalle de la venta");
            return RedirectToAction(nameof(Ventas));
        }
    }
}
