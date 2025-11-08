using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SciFiHub.Web.Controllers.Common;
using SciFiHub.Web.DTOs.Libro;
using SciFiHub.Web.Services.Interfaces;

namespace SciFiHub.Web.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Vendedor,Administrador")]
public class LibrosController : ControllerBase
{
    private readonly ILibroService _libroService;
    private readonly ICategoriaService _categoriaService;
    private readonly ILogger<LibrosController> _logger;

    public LibrosController(
        ILibroService libroService,
        ICategoriaService categoriaService,
        ILogger<LibrosController> logger)
    {
        _libroService = libroService;
        _categoriaService = categoriaService;
        _logger = logger;
    }

    // POST: api/Libros
    [HttpPost]
    public async Task<IActionResult> CrearLibro([FromBody] CrearLibroDTO model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { error = "Datos inválidos", errors = ModelState });
            }

            var result = await _libroService.CrearLibroAsync(model);

            if (!result.Success)
            {
                return BadRequest(new { error = result.ErrorMessage });
            }

            return Ok(new
            {
                success = true,
                message = "Libro creado exitosamente",
                data = result.Data
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear libro");
            return StatusCode(500, new { error = "Error al crear el libro" });
        }
    }

    // PUT: api/Libros/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> ActualizarLibro(Guid id, [FromBody] ActualizarLibroDTO model)
    {
        try
        {
            if (id != model.Id)
            {
                return BadRequest(new { error = "El ID no coincide" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new { error = "Datos inválidos", errors = ModelState });
            }

            var result = await _libroService.ActualizarLibroAsync(model);

            if (!result.Success)
            {
                return BadRequest(new { error = result.ErrorMessage });
            }

            return Ok(new
            {
                success = true,
                message = "Libro actualizado exitosamente",
                data = result.Data
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar libro {LibroId}", id);
            return StatusCode(500, new { error = "Error al actualizar el libro" });
        }
    }

    // DELETE: api/Libros/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> EliminarLibro(Guid id)
    {
        try
        {
            var result = await _libroService.EliminarLibroAsync(id);

            if (!result.Success)
            {
                return BadRequest(new { error = result.ErrorMessage });
            }

            return Ok(new
            {
                success = true,
                message = "Libro eliminado exitosamente"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar libro {LibroId}", id);
            return StatusCode(500, new { error = "Error al eliminar el libro" });
        }
    }

    // GET: api/Libros/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerLibro(Guid id)
    {
        try
        {
            var result = await _libroService.ObtenerLibroPorIdAsync(id);

            if (!result.Success)
            {
                return NotFound(new { error = "Libro no encontrado" });
            }

            return Ok(new
            {
                success = true,
                data = result.Data
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener libro {LibroId}", id);
            return StatusCode(500, new { error = "Error al obtener el libro" });
        }
    }

    // GET: api/Libros/categorias
    [HttpGet("categorias")]
    public async Task<IActionResult> ObtenerCategorias()
    {
        try
        {
            var result = await _categoriaService.ObtenerCategoriasActivasAsync();

            if (!result.Success)
            {
                return BadRequest(new { error = result.ErrorMessage });
            }

            var categorias = result.Data?.Select(c => new
            {
                id = c.Id,
                nombre = c.Nombre
            }) ?? Enumerable.Empty<object>();

            return Ok(new
            {
                success = true,
                data = categorias
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener categorías");
            return StatusCode(500, new { error = "Error al obtener categorías" });
        }
    }

    // PUT: api/Libros/{id}/stock
    [HttpPut("{id}/stock")]
    public async Task<IActionResult> ActualizarStock(Guid id, [FromBody] ActualizarStockDTO model)
    {
        try
        {
            if (id != model.LibroId)
            {
                return BadRequest(new { error = "El ID no coincide" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new { error = "Datos inválidos", errors = ModelState });
            }

            var result = await _libroService.ActualizarStockAsync(model);

            if (!result.Success)
            {
                return BadRequest(new { error = result.ErrorMessage });
            }

            return Ok(new
            {
                success = true,
                message = "Stock actualizado exitosamente"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar stock del libro {LibroId}", id);
            return StatusCode(500, new { error = "Error al actualizar el stock" });
        }
    }

    // GET: api/Libros/stock-bajo
    [HttpGet("stock-bajo")]
    public async Task<IActionResult> ObtenerStockBajo([FromQuery] int umbral = 10)
    {
        try
        {
            var result = await _libroService.ObtenerStockBajoAsync(umbral);

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
            _logger.LogError(ex, "Error al obtener libros con stock bajo");
            return StatusCode(500, new { error = "Error al obtener libros con stock bajo" });
        }
    }

    // GET: api/Libros/verificar-isbn/{isbn}
    [HttpGet("verificar-isbn/{isbn}")]
    public async Task<IActionResult> VerificarISBN(string isbn, [FromQuery] Guid? excludeId = null)
    {
        try
        {
            var result = await _libroService.ISBNExisteAsync(isbn, excludeId);

            if (!result.Success)
            {
                return BadRequest(new { error = result.ErrorMessage });
            }

            return Ok(new
            {
                success = true,
                existe = result.Data
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al verificar ISBN");
            return StatusCode(500, new { error = "Error al verificar ISBN" });
        }
    }

    // GET: api/Libros/editoriales
    [HttpGet("editoriales")]
    public async Task<IActionResult> ObtenerEditoriales()
    {
        try
        {
            var result = await _libroService.ObtenerEditorialesAsync();

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
            _logger.LogError(ex, "Error al obtener editoriales");
            return StatusCode(500, new { error = "Error al obtener editoriales" });
        }
    }
}
