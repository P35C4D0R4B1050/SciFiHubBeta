using SciFiHub.Domain.Entities;
using SciFiHub.Domain.Enums;

namespace SciFiHub.Domain.Interfaces;

/// <summary>
/// Repositorio para la entidad Libro con métodos específicos de negocio
/// </summary>
public interface ILibroRepository : IRepository<Libro>
{
    // Búsqueda y filtrado
    Task<IEnumerable<Libro>> BuscarPorCriterioAsync(
        string? textoBusqueda, 
        Guid? categoriaId = null,
        bool soloDisponibles = false,
        CancellationToken cancellationToken = default);
    
    Task<IEnumerable<Libro>> GetPorCategoriaAsync(
        Guid categoriaId, 
        CancellationToken cancellationToken = default);
    
    Task<IEnumerable<Libro>> GetPorAutorAsync(
        string autor, 
        CancellationToken cancellationToken = default);
    
    Task<IEnumerable<Libro>> GetPorEditorialAsync(
        string editorial, 
        CancellationToken cancellationToken = default);
    
    // Libros especiales
    Task<IEnumerable<Libro>> GetDestacadosAsync(
        int cantidad = 10, 
        CancellationToken cancellationToken = default);
    
    Task<IEnumerable<Libro>> GetNovedadesAsync(
        int cantidad = 10, 
        CancellationToken cancellationToken = default);
    
    Task<IEnumerable<Libro>> GetMasVendidosAsync(
        int cantidad = 10,
        DateTime? fechaDesde = null,
        CancellationToken cancellationToken = default);
    
    Task<IEnumerable<Libro>> GetOfertasAsync(
        CancellationToken cancellationToken = default);
    
    // Gestión de inventario
    Task<bool> VerificarStockAsync(
        Guid libroId, 
        int cantidadRequerida, 
        CancellationToken cancellationToken = default);
    
    Task ActualizarStockAsync(
        Guid libroId, 
        int cantidad, 
        TipoMovimientoInventario tipoMovimiento,
        string? motivo = null,
        CancellationToken cancellationToken = default);
    
    Task<IEnumerable<Libro>> GetStockBajoAsync(
        int umbral = 10, 
        CancellationToken cancellationToken = default);
    
    // Consultas específicas
    Task<Libro?> GetByISBNAsync(
        string isbn, 
        CancellationToken cancellationToken = default);
    
    Task<bool> ISBNExisteAsync(
        string isbn, 
        Guid? excludeId = null, 
        CancellationToken cancellationToken = default);
    
    Task<(IEnumerable<Libro> Items, int TotalCount)> GetPagedConFiltrosAsync(
        int pageNumber,
        int pageSize,
        string? textoBusqueda = null,
        Guid? categoriaId = null,
        decimal? precioMin = null,
        decimal? precioMax = null,
        bool? destacado = null,
        bool? soloDisponibles = null,
        string? ordenarPor = null,
        bool ordenAscendente = true,
        CancellationToken cancellationToken = default);
}
