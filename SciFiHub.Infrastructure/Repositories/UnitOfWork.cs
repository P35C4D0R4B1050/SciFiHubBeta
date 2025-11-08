using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SciFiHub.Domain.Common;
using SciFiHub.Domain.Interfaces;
using SciFiHub.Infrastructure.Data;

namespace SciFiHub.Infrastructure.Repositories;

/// <summary>
/// Implementación del patrón Unit of Work para coordinar repositorios y transacciones
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly SciFiHubDbContext _context;
    private IDbContextTransaction? _transaction;
    private readonly Dictionary<Type, object> _repositories;

    // Repositorios específicos (lazy loading)
    private IUsuarioRepository? _usuarios;
    private ILibroRepository? _libros;
    private ICategoriaRepository? _categorias;
    private IVentaRepository? _ventas;
    private ICarritoCompraRepository? _carritos;

    public UnitOfWork(SciFiHubDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _repositories = new Dictionary<Type, object>();
    }

    public IUsuarioRepository Usuarios => _usuarios ??= new UsuarioRepository(_context);

    public ILibroRepository Libros => _libros ??= new LibroRepository(_context);

    public ICategoriaRepository Categorias => _categorias ??= new CategoriaRepository(_context);

    public IVentaRepository Ventas => _ventas ??= new VentaRepository(_context);

    public ICarritoCompraRepository Carritos => _carritos ??= new CarritoCompraRepository(_context);

    public IRepository<T> Repository<T>() where T : BaseEntity
    {
        var type = typeof(T);

        if (_repositories.ContainsKey(type))
        {
            return (IRepository<T>)_repositories[type];
        }

        var repository = new Repository<T>(_context);
        _repositories[type] = repository;

        return repository;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
    {
        return await SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            throw new InvalidOperationException("Ya existe una transacción activa");
        }

        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("No hay una transacción activa para confirmar");
        }

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            await _transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("No hay una transacción activa para revertir");
        }

        try
        {
            await _transaction.RollbackAsync(cancellationToken);
        }
        finally
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public bool HasActiveTransaction => _transaction != null;

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
