using SciFiHub.Domain.Entities;

namespace SciFiHub.Domain.Interfaces;

/// <summary>
/// Repositorio para la entidad Categoria con soporte de jerarquía
/// </summary>
public interface ICategoriaRepository : IRepository<Categoria>
{
    // Categorías principales
    Task<IEnumerable<Categoria>> GetCategoriasPrincipalesAsync(
        CancellationToken cancellationToken = default);
    
    Task<IEnumerable<Categoria>> GetCategoriasActivasAsync(
        CancellationToken cancellationToken = default);
    
    // Jerarquía
    Task<IEnumerable<Categoria>> GetSubCategoriasAsync(
        Guid categoriaPadreId, 
        CancellationToken cancellationToken = default);
    
    Task<Categoria?> GetConSubCategoriasAsync(
        Guid categoriaId, 
        CancellationToken cancellationToken = default);
    
    Task<Categoria?> GetConLibrosAsync(
        Guid categoriaId, 
        CancellationToken cancellationToken = default);
    
    // Validaciones
    Task<bool> TieneSubCategoriasAsync(
        Guid categoriaId, 
        CancellationToken cancellationToken = default);
    
    Task<bool> TieneLibrosAsync(
        Guid categoriaId, 
        CancellationToken cancellationToken = default);
    
    Task<bool> NombreExisteAsync(
        string nombre, 
        Guid? excludeId = null,
        CancellationToken cancellationToken = default);
    
    // Ordenamiento
    Task<IEnumerable<Categoria>> GetOrdenadaPorOrdenAsync(
        CancellationToken cancellationToken = default);
}
