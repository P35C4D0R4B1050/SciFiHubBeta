using Microsoft.AspNetCore.Mvc;
using SciFiHub.Web.Services.Interfaces;

namespace SciFiHub.Web.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class BusquedaController : ControllerBase
{
    private readonly ILibroService _libroService;
    private readonly IUsuarioService _usuarioService;
    private readonly IVentaService _ventaService;

    public BusquedaController(
        ILibroService libroService, 
        IUsuarioService usuarioService,
        IVentaService ventaService)
    {
        _libroService = libroService;
        _usuarioService = usuarioService;
        _ventaService = ventaService;
    }

    [HttpGet("libros")]
    public async Task<IActionResult> BuscarLibros([FromQuery] string? query, [FromQuery] string? q)
    {
        string busqueda = query ?? q ?? "";
        
        if (string.IsNullOrWhiteSpace(busqueda) || busqueda.Length < 2)
            return Ok(new { items = Array.Empty<object>(), results = Array.Empty<object>() });

        var filtro = new DTOs.Libro.LibrosFiltroDTO
        {
            TextoBusqueda = busqueda,
            PageSize = 10,
            SoloDisponibles = false
        };

        var resultado = await _libroService.ObtenerLibrosAsync(filtro);
        
        if (!resultado.Success)
            return Ok(new { items = Array.Empty<object>(), results = Array.Empty<object>() });

        var datos = resultado.Data!.Items.Select(libro => new
        {
            id = libro.Id,
            titulo = libro.Titulo,
            autor = libro.Autor,
            precio = libro.Precio,
            precioOferta = libro.PrecioOferta,
            imagen = libro.ImagenPortada,
            categoria = libro.CategoriaNombre,
            disponible = libro.Disponible,
            stock = libro.Stock
        }).ToList();

        return Ok(new { items = datos, results = datos });
    }

    [HttpGet("usuarios")]
    public async Task<IActionResult> BuscarUsuarios([FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            return Ok(new { items = Array.Empty<object>() });

        var filtro = new DTOs.Usuario.UsuariosFiltroDTO
        {
            TextoBusqueda = query,
            PageSize = 10
        };

        var resultado = await _usuarioService.ObtenerUsuariosAsync(filtro);
        
        if (!resultado.Success)
            return Ok(new { items = Array.Empty<object>() });

        var items = resultado.Data!.Items.Select(usuario => new
        {
            id = usuario.Id,
            nombreCompleto = usuario.NombreCompleto,
            email = usuario.Email,
            rol = usuario.Rol.ToString(),
            estado = usuario.Estado.ToString()
        });

        return Ok(new { items });
    }

    [HttpGet("ventas")]
    public async Task<IActionResult> BuscarVentas([FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            return Ok(new { items = Array.Empty<object>() });

        var filtro = new DTOs.Venta.VentasFiltroDTO
        {
            TextoBusqueda = query,
            PageSize = 10
        };

        var resultado = await _ventaService.ObtenerVentasAsync(filtro);
        
        if (!resultado.Success)
            return Ok(new { items = Array.Empty<object>() });

        var items = resultado.Data!.Items.Select(venta => new
        {
            id = venta.Id,
            numeroVenta = venta.NumeroVenta,
            fecha = venta.FechaVenta.ToString("dd/MM/yyyy HH:mm"),
            cliente = venta.ClienteNombre,
            vendedor = venta.VendedorNombre,
            total = venta.Total,
            estado = venta.EstadoVenta.ToString()
        });

        return Ok(new { items });
    }
    
    [HttpGet("clientes")]
    public async Task<IActionResult> BuscarClientes([FromQuery] string q)
    {
        if (string.IsNullOrWhiteSpace(q) || q.Length < 2)
            return Ok(new { results = Array.Empty<object>() });

        var filtro = new DTOs.Usuario.UsuariosFiltroDTO
        {
            TextoBusqueda = q,
            Rol = Domain.Enums.RolUsuario.Cliente,
            Estado = Domain.Enums.EstadoUsuario.Activo,
            PageSize = 10
        };

        var resultado = await _usuarioService.ObtenerUsuariosAsync(filtro);
        
        if (!resultado.Success)
            return Ok(new { results = Array.Empty<object>() });

        var results = resultado.Data!.Items.Select(usuario => new
        {
            id = usuario.Id,
            nombre = usuario.NombreCompleto,
            email = usuario.Email,
            username = usuario.Username,
            telefono = usuario.Telefono
        });

        return Ok(new { results });
    }
}
