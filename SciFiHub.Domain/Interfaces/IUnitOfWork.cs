using SciFiHub.Domain.Common;

namespace SciFiHub.Domain.Interfaces;

/// <summary>
/// Patrón Unit of Work para gestionar transacciones y coordinar repositorios
/// </summary>
public interface IUnitOfWork : IDisposable
{
    // Repositorios específicos
    IUsuarioRepository Usuarios { get; }
    ILibroRepository Libros { get; }
    ICategoriaRepository Categorias { get; }
    IVentaRepository Ventas { get; }
    ICarritoCompraRepository Carritos { get; }
    
    // Factory de repositorio genérico (para entidades adicionales)
    IRepository<T> Repository<T>() where T : BaseEntity;
    
    // Operaciones de transacción
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<int> CommitAsync(CancellationToken cancellationToken = default);
    
    // Transacciones explícitas
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    
    // Estado
    bool HasActiveTransaction { get; }
}
