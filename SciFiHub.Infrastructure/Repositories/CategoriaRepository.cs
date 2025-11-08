using Microsoft.EntityFrameworkCore;
using SciFiHub.Domain.Entities;
using SciFiHub.Domain.Interfaces;
using SciFiHub.Infrastructure.Data;

namespace SciFiHub.Infrastructure.Repositories;

/// <summary>
/// Implementación del repositorio de Categoria con soporte para jerarquía
/// </summary>
public class CategoriaRepository : Repository<Categoria>, ICategoriaRepository
{
    public CategoriaRepository(SciFiHubDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Categoria>> GetCategoriasPrincipalesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(c => c.CategoriaPadreId == null)
            .OrderBy(c => c.Orden)
            .ThenBy(c => c.Nombre)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Categoria>> GetCategoriasActivasAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(c => c.Estado == "Activo")
            .OrderBy(c => c.Orden)
            .ThenBy(c => c.Nombre)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Categoria>> GetSubCategoriasAsync(
        Guid categoriaPadreId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(c => c.CategoriaPadreId == categoriaPadreId)
            .OrderBy(c => c.Orden)
            .ThenBy(c => c.Nombre)
            .ToListAsync(cancellationToken);
    }

    public async Task<Categoria?> GetConSubCategoriasAsync(
        Guid categoriaId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(c => c.SubCategorias)
            .FirstOrDefaultAsync(c => c.Id == categoriaId, cancellationToken);
    }

    public async Task<Categoria?> GetConLibrosAsync(
        Guid categoriaId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(c => c.Libros)
            .FirstOrDefaultAsync(c => c.Id == categoriaId, cancellationToken);
    }

    public async Task<bool> TieneSubCategoriasAsync(
        Guid categoriaId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(c => c.CategoriaPadreId == categoriaId, cancellationToken);
    }

    public async Task<bool> TieneLibrosAsync(
        Guid categoriaId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Libros.AnyAsync(l => l.CategoriaId == categoriaId, cancellationToken);
    }

    public async Task<bool> NombreExisteAsync(
        string nombre,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(c => c.Nombre == nombre);

        if (excludeId.HasValue)
        {
            query = query.Where(c => c.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<IEnumerable<Categoria>> GetOrdenadaPorOrdenAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .OrderBy(c => c.Orden)
            .ThenBy(c => c.Nombre)
            .ToListAsync(cancellationToken);
    }
}
