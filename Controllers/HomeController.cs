using Microsoft.AspNetCore.Mvc;
using SciFiHub.Web.Controllers.Common;
using SciFiHub.Web.Services.Interfaces;

namespace SciFiHub.Web.Controllers;

/// <summary>
/// Controller principal de la página de inicio
/// </summary>
public class HomeController : BaseController
{
    private readonly ILibroService _libroService;
    private readonly ICategoriaService _categoriaService;
    private readonly ILogger<HomeController> _logger;

    public HomeController(
        ILibroService libroService,
        ICategoriaService categoriaService,
        ILogger<HomeController> logger)
    {
        _libroService = libroService;
        _categoriaService = categoriaService;
        _logger = logger;
    }

    // GET: /
    public async Task<IActionResult> Index()
    {
        try
        {
            // Cargar datos para la página principal
            var destacadosTask = _libroService.ObtenerDestacadosAsync(8);
            var novedadesTask = _libroService.ObtenerNovedadesAsync(8);
            var ofertasTask = _libroService.ObtenerOfertasAsync();
            var categoriasTask = _categoriaService.ObtenerCategoriasActivasAsync();

            await Task.WhenAll(destacadosTask, novedadesTask, ofertasTask, categoriasTask);

            ViewBag.Destacados = destacadosTask.Result.Success ? destacadosTask.Result.Data : Enumerable.Empty<DTOs.Libro.LibroCardDTO>();
            ViewBag.Novedades = novedadesTask.Result.Success ? novedadesTask.Result.Data : Enumerable.Empty<DTOs.Libro.LibroCardDTO>();
            ViewBag.Ofertas = ofertasTask.Result.Success ? ofertasTask.Result.Data?.Take(4) : Enumerable.Empty<DTOs.Libro.LibroCardDTO>();
            ViewBag.Categorias = categoriasTask.Result.Success ? categoriasTask.Result.Data : Enumerable.Empty<DTOs.Categoria.CategoriaDTO>();

            return View();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar página principal");
            return View();
        }
    }

    // GET: /Home/About
    public IActionResult About()
    {
        return View();
    }

    // GET: /Home/Contact
    public IActionResult Contact()
    {
        return View();
    }

    // GET: /Home/Privacy
    public IActionResult Privacy()
    {
        return View();
    }

    // GET: /Home/Error
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View();
    }
}
