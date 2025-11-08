using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SciFiHub.Domain.Enums;
using SciFiHub.Web.DTOs.Usuario;
using SciFiHub.Web.DTOs.Libro;
using SciFiHub.Web.DTOs.Venta;
using SciFiHub.Web.Services.Interfaces;

namespace SciFiHub.Web.Controllers;

[Authorize(Roles = "Admin,Administrador")]
public class AdminController : Controller
{
    private readonly IUsuarioService _usuarioService;
    private readonly ILibroService _libroService;
    private readonly IVentaService _ventaService;
    private readonly ILogger<AdminController> _logger;

    public AdminController(
        IUsuarioService usuarioService,
        ILibroService libroService,
        IVentaService ventaService,
        ILogger<AdminController> logger)
    {
        _usuarioService = usuarioService;
        _libroService = libroService;
        _ventaService = ventaService;
        _logger = logger;
    }

    // GET: /Admin (Dashboard principal)
    public IActionResult Index()
    {
        return RedirectToAction(nameof(Dashboard));
    }

    // GET: /Admin/Dashboard
    public async Task<IActionResult> Dashboard()
    {
        try
        {
            // Obtener estadísticas del mes actual
            var estadisticasResult = await _ventaService.ObtenerEstadisticasDelMesAsync();
            
            if (estadisticasResult.Success && estadisticasResult.Data != null)
            {
                ViewBag.Estadisticas = estadisticasResult.Data;
            }
            else
            {
                ViewBag.Estadisticas = new EstadisticasVentasDTO
                {
                    TotalVentas = 0,
                    CantidadVentas = 0,
                    PromedioVenta = 0,
                    CantidadClientes = 0
                };
            }

            // Obtener libros con stock bajo - TEMPORAL: obtener todos y filtrar en memoria
            var todosLibros = new LibrosFiltroDTO
            {
                PageNumber = 1,
                PageSize = 1000
            };

            var stockBajoResult = await _libroService.ObtenerLibrosPaginadosAsync(todosLibros);
            
            if (stockBajoResult.Success && stockBajoResult.Data != null)
            {
                var librosConStockBajo = stockBajoResult.Data.Items
                    .Where(l => l.Stock <= 9)
                    .OrderBy(l => l.Stock)
                    .Take(10)
                    .ToList();
                
                _logger.LogInformation("Libros con stock bajo encontrados: {Cantidad}", librosConStockBajo.Count);
                ViewBag.StockBajo = librosConStockBajo;
            }
            else
            {
                _logger.LogWarning("No se pudieron obtener libros para verificar stock bajo");
                ViewBag.StockBajo = Enumerable.Empty<LibroDTO>();
            }

            // Obtener top 5 libros más vendidos del mes
            var masVendidosResult = await _ventaService.ObtenerLibrosMasVendidosAsync(5, DateTime.UtcNow.AddMonths(-1));
            
            if (masVendidosResult.Success && masVendidosResult.Data != null)
            {
                ViewBag.MasVendidos = masVendidosResult.Data;
            }
            else
            {
                ViewBag.MasVendidos = Enumerable.Empty<LibroMasVendidoDTO>();
            }

            return View();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar Dashboard");
            TempData["Error"] = "Error al cargar el dashboard";
            return View();
        }
    }

    // GET: /Admin/Inventario (Hereda de Vendedor)
    public async Task<IActionResult> Inventario()
    {
        try
        {
            var filtro = new LibrosFiltroDTO
            {
                PageNumber = 1,
                PageSize = 100
            };

            var result = await _libroService.ObtenerLibrosPaginadosAsync(filtro);

            if (!result.Success)
            {
                TempData["Error"] = "Error al cargar el inventario";
                return View("~/Views/Vendedor/Inventario.cshtml", Enumerable.Empty<DTOs.Libro.LibroCardDTO>());
            }

            return View("~/Views/Vendedor/Inventario.cshtml", result.Data?.Items ?? Enumerable.Empty<DTOs.Libro.LibroCardDTO>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar inventario");
            TempData["Error"] = "Error al cargar el inventario";
            return View("~/Views/Vendedor/Inventario.cshtml", Enumerable.Empty<DTOs.Libro.LibroCardDTO>());
        }
    }

    // GET: /Admin/Ventas (Hereda de Vendedor)
    public async Task<IActionResult> Ventas()
    {
        try
        {
            var filtro = new VentasFiltroDTO
            {
                PageNumber = 1,
                PageSize = 50
            };

            var result = await _ventaService.ObtenerVentasAsync(filtro);

            if (!result.Success)
            {
                TempData["Error"] = "Error al cargar ventas";
                return View("~/Views/Vendedor/Ventas.cshtml", Enumerable.Empty<DTOs.Venta.VentaDTO>());
            }

            return View("~/Views/Vendedor/Ventas.cshtml", result.Data?.Items ?? Enumerable.Empty<DTOs.Venta.VentaDTO>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar ventas");
            TempData["Error"] = "Error al cargar ventas";
            return View("~/Views/Vendedor/Ventas.cshtml", Enumerable.Empty<DTOs.Venta.VentaDTO>());
        }
    }

    // GET: /Admin/Usuarios
    public async Task<IActionResult> Usuarios()
    {
        try
        {
            var filtro = new UsuariosFiltroDTO
            {
                PageNumber = 1,
                PageSize = 100
            };

            var result = await _usuarioService.ObtenerUsuariosAsync(filtro);

            if (!result.Success)
            {
                TempData["Error"] = "Error al cargar usuarios";
                return View(Enumerable.Empty<DTOs.Usuario.UsuarioDTO>());
            }

            return View(result.Data?.Items ?? Enumerable.Empty<DTOs.Usuario.UsuarioDTO>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar usuarios");
            TempData["Error"] = "Error al cargar usuarios";
            return View(Enumerable.Empty<DTOs.Usuario.UsuarioDTO>());
        }
    }

    // GET: /Admin/CrearUsuario
    public IActionResult CrearUsuario()
    {
        return View();
    }

    // POST: /Admin/CrearUsuario
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CrearUsuario(DTOs.Usuario.CrearUsuarioDTO model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var result = await _usuarioService.CrearUsuarioAsync(model);

            if (!result.Success)
            {
                TempData["Error"] = result.ErrorMessage ?? "Error al crear usuario";
                return View(model);
            }

            TempData["Success"] = "Usuario creado exitosamente";
            return RedirectToAction(nameof(Usuarios));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear usuario");
            TempData["Error"] = "Error al crear el usuario";
            return View(model);
        }
    }

    // GET: /Admin/EditarUsuario/{id}
    public async Task<IActionResult> EditarUsuario(Guid id)
    {
        try
        {
            var result = await _usuarioService.ObtenerUsuarioPorIdAsync(id);

            if (!result.Success)
            {
                TempData["Error"] = "Usuario no encontrado";
                return RedirectToAction(nameof(Usuarios));
            }

            // Convertir a DTO de actualización
            var model = new DTOs.Usuario.ActualizarUsuarioDTO
            {
                Id = result.Data!.Id,
                NombreCompleto = result.Data.NombreCompleto,
                Email = result.Data.Email,
                Telefono = result.Data.Telefono,
                Estado = result.Data.Estado
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar usuario {UsuarioId}", id);
            TempData["Error"] = "Error al cargar el usuario";
            return RedirectToAction(nameof(Usuarios));
        }
    }

    // POST: /Admin/EditarUsuario
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditarUsuario(DTOs.Usuario.ActualizarUsuarioDTO model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var result = await _usuarioService.ActualizarUsuarioAsync(model);

            if (!result.Success)
            {
                TempData["Error"] = result.ErrorMessage ?? "Error al actualizar usuario";
                return View(model);
            }

            TempData["Success"] = "Usuario actualizado exitosamente";
            return RedirectToAction(nameof(Usuarios));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar usuario");
            TempData["Error"] = "Error al actualizar el usuario";
            return View(model);
        }
    }

    // POST: /Admin/EliminarUsuario/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EliminarUsuario(Guid id)
    {
        try
        {
            var result = await _usuarioService.EliminarUsuarioAsync(id);

            if (!result.Success)
            {
                TempData["Error"] = result.ErrorMessage ?? "Error al eliminar usuario";
            }
            else
            {
                TempData["Success"] = "Usuario eliminado exitosamente";
            }

            return RedirectToAction(nameof(Usuarios));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar usuario {UsuarioId}", id);
            TempData["Error"] = "Error al eliminar el usuario";
            return RedirectToAction(nameof(Usuarios));
        }
    }
}
