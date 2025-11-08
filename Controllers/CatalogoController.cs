using Microsoft.AspNetCore.Mvc;
using SciFiHub.Web.Controllers.Common;
using SciFiHub.Web.DTOs.Libro;
using SciFiHub.Web.Services.Interfaces;

namespace SciFiHub.Web.Controllers;

/// <summary>
/// Controller para el catálogo público de libros
/// </summary>
public class CatalogoController : BaseController
{
    private readonly ILibroService _libroService;
    private readonly ICategoriaService _categoriaService;
    private readonly ILogger<CatalogoController> _logger;

    public CatalogoController(
        ILibroService libroService,
        ICategoriaService categoriaService,
        ILogger<CatalogoController> logger)
    {
        _libroService = libroService;
        _categoriaService = categoriaService;
        _logger = logger;
    }

    // GET: /Catalogo
    public async Task<IActionResult> Index(LibrosFiltroDTO? filtro)
    {
        try
        {
            filtro ??= new LibrosFiltroDTO { PageNumber = 1, PageSize = 12, SoloDisponibles = true };

            var result = await _libroService.ObtenerLibrosPaginadosAsync(filtro);

            if (!result.Success)
            {
                AddErrorMessage(result.ErrorMessage ?? "Error al cargar el catálogo");
                return View(new DTOs.Common.PagedResult<LibroCardDTO>
                {
                    Items = Enumerable.Empty<LibroCardDTO>(),
                    PageNumber = 1,
                    PageSize = 12,
                    TotalCount = 0
                });
            }

            // Cargar categorías para el filtro
            var categoriasResult = await _categoriaService.ObtenerCategoriasActivasAsync();
            ViewBag.Categorias = categoriasResult.Success ? categoriasResult.Data : Enumerable.Empty<DTOs.Categoria.CategoriaDTO>();

            ViewBag.FiltroActual = filtro;

            return View(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar el catálogo");
            AddErrorMessage("Error al cargar el catálogo");
            return View();
        }
    }

    // GET: /Catalogo/Detalle/{id}
    public async Task<IActionResult> Detalle(Guid id)
    {
        try
        {
            var result = await _libroService.ObtenerLibroPorIdAsync(id);

            if (!result.Success)
            {
                AddErrorMessage("Libro no encontrado");
                return RedirectToAction(nameof(Index));
            }

            return View(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar detalle del libro {LibroId}", id);
            AddErrorMessage("Error al cargar el detalle del libro");
            return RedirectToAction(nameof(Index));
        }
    }

    // GET: /Catalogo/Buscar?q={texto}
    public async Task<IActionResult> Buscar(string? q)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                return RedirectToAction(nameof(Index));
            }

            var result = await _libroService.BuscarLibrosAsync(q);

            if (!result.Success)
            {
                AddErrorMessage(result.ErrorMessage ?? "Error en la búsqueda");
                return RedirectToAction(nameof(Index));
            }

            ViewBag.TerminoBusqueda = q;

            return View(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en búsqueda: {Termino}", q);
            AddErrorMessage("Error al realizar la búsqueda");
            return RedirectToAction(nameof(Index));
        }
    }

    // GET: /Catalogo/Categoria/{id}
    public async Task<IActionResult> Categoria(Guid id)
    {
        try
        {
            var categoriaResult = await _categoriaService.ObtenerCategoriaPorIdAsync(id);

            if (!categoriaResult.Success)
            {
                AddErrorMessage("Categoría no encontrada");
                return RedirectToAction(nameof(Index));
            }

            var librosResult = await _libroService.ObtenerPorCategoriaAsync(id);

            ViewBag.Categoria = categoriaResult.Data;

            if (!librosResult.Success)
            {
                return View(Enumerable.Empty<LibroCardDTO>());
            }

            return View(librosResult.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar libros de categoría {CategoriaId}", id);
            AddErrorMessage("Error al cargar la categoría");
            return RedirectToAction(nameof(Index));
        }
    }

    // GET: /Catalogo/Ofertas
    public async Task<IActionResult> Ofertas()
    {
        try
        {
            var result = await _libroService.ObtenerOfertasAsync();

            if (!result.Success)
            {
                AddErrorMessage(result.ErrorMessage ?? "Error al cargar ofertas");
                return View(Enumerable.Empty<LibroCardDTO>());
            }

            return View(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar ofertas");
            AddErrorMessage("Error al cargar ofertas");
            return View();
        }
    }

    // GET: /Catalogo/Destacados
    public async Task<IActionResult> Destacados()
    {
        try
        {
            var result = await _libroService.ObtenerDestacadosAsync(20);

            if (!result.Success)
            {
                AddErrorMessage(result.ErrorMessage ?? "Error al cargar destacados");
                return View(Enumerable.Empty<LibroCardDTO>());
            }

            return View(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar destacados");
            AddErrorMessage("Error al cargar destacados");
            return View();
        }
    }

    // GET: /Catalogo/Novedades
    public async Task<IActionResult> Novedades()
    {
        try
        {
            var result = await _libroService.ObtenerNovedadesAsync(20);

            if (!result.Success)
            {
                AddErrorMessage(result.ErrorMessage ?? "Error al cargar novedades");
                return View(Enumerable.Empty<LibroCardDTO>());
            }

            return View(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar novedades");
            AddErrorMessage("Error al cargar novedades");
            return View();
        }
    }
}
