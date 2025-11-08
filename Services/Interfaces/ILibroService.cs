using SciFiHub.Web.DTOs.Common;
using SciFiHub.Web.DTOs.Libro;

namespace SciFiHub.Web.Services.Interfaces;

/// <summary>
/// Servicio para gestión de libros e inventario
/// </summary>
public interface ILibroService
{
    // ===================================
    // CRUD Básico
    // ===================================
    Task<Result<LibroDTO>> CrearLibroAsync(CrearLibroDTO crearLibroDto, CancellationToken cancellationToken = default);
    Task<Result<LibroDTO>> ActualizarLibroAsync(ActualizarLibroDTO actualizarLibroDto, CancellationToken cancellationToken = default);
    Task<Result> EliminarLibroAsync(Guid libroId, CancellationToken cancellationToken = default);
    Task<Result<LibroDTO>> ObtenerLibroPorIdAsync(Guid libroId, CancellationToken cancellationToken = default);

    // ===================================
    // Búsqueda y Filtrado
    // ===================================
    Task<Result<PagedResult<LibroCardDTO>>> ObtenerLibrosAsync(LibrosFiltroDTO filtro, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<LibroCardDTO>>> ObtenerLibrosPaginadosAsync(LibrosFiltroDTO filtro, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<LibroCardDTO>>> BuscarLibrosAsync(string textoBusqueda, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<LibroCardDTO>>> ObtenerPorCategoriaAsync(Guid categoriaId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<LibroCardDTO>>> ObtenerPorAutorAsync(string autor, CancellationToken cancellationToken = default);

    // ===================================
    // Libros Especiales
    // ===================================
    Task<Result<IEnumerable<LibroCardDTO>>> ObtenerDestacadosAsync(int cantidad = 10, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<LibroCardDTO>>> ObtenerNovedadesAsync(int cantidad = 10, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<LibroCardDTO>>> ObtenerMasVendidosAsync(int cantidad = 10, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<LibroCardDTO>>> ObtenerOfertasAsync(CancellationToken cancellationToken = default);

    // ===================================
    // Gestión de Inventario
    // ===================================
    Task<Result> ActualizarStockAsync(ActualizarStockDTO actualizarStockDto, CancellationToken cancellationToken = default);
    Task<Result<bool>> VerificarStockDisponibleAsync(Guid libroId, int cantidadRequerida, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<LibroDTO>>> ObtenerStockBajoAsync(int umbral = 10, CancellationToken cancellationToken = default);

    // ===================================
    // Validaciones
    // ===================================
    Task<Result<bool>> ISBNExisteAsync(string isbn, Guid? excludeId = null, CancellationToken cancellationToken = default);
    
    // ===================================
    // Utilidades
    // ===================================
    Task<Result<IEnumerable<string>>> ObtenerEditorialesAsync(CancellationToken cancellationToken = default);
}
