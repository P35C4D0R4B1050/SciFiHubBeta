using Microsoft.EntityFrameworkCore;
using SciFiHub.Domain.Entities;
using SciFiHub.Domain.Enums;
using SciFiHub.Domain.Interfaces;
using SciFiHub.Infrastructure.Data;

namespace SciFiHub.Infrastructure.Repositories;

/// <summary>
/// Implementación del repositorio de Libro con métodos de búsqueda y gestión de inventario
/// </summary>
public class LibroRepository : Repository<Libro>, ILibroRepository
{
    public LibroRepository(SciFiHubDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Libro>> BuscarPorCriterioAsync(
        string? textoBusqueda,
        Guid? categoriaId = null,
        bool soloDisponibles = false,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Libro> query = _dbSet.AsNoTracking().Include(l => l.Categoria);

        if (!string.IsNullOrWhiteSpace(textoBusqueda))
        {
            query = query.Where(l =>
                l.Titulo.Contains(textoBusqueda) ||
                l.Autor.Contains(textoBusqueda) ||
                (l.Editorial != null && l.Editorial.Contains(textoBusqueda)) ||
                (l.Sinopsis != null && l.Sinopsis.Contains(textoBusqueda)));
        }

        if (categoriaId.HasValue)
        {
            query = query.Where(l => l.CategoriaId == categoriaId.Value);
        }

        if (soloDisponibles)
        {
            query = query.Where(l => l.Stock > 0 && l.Estado == EstadoLibro.Disponible);
        }

        return await query
            .OrderBy(l => l.Titulo)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Libro>> GetPorCategoriaAsync(
        Guid categoriaId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(l => l.Categoria)
            .Where(l => l.CategoriaId == categoriaId)
            .OrderBy(l => l.Titulo)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Libro>> GetPorAutorAsync(
        string autor,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(l => l.Categoria)
            .Where(l => l.Autor.Contains(autor))
            .OrderBy(l => l.Titulo)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Libro>> GetPorEditorialAsync(
        string editorial,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(l => l.Categoria)
            .Where(l => l.Editorial != null && l.Editorial.Contains(editorial))
            .OrderBy(l => l.Titulo)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Libro>> GetDestacadosAsync(
        int cantidad = 10,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(l => l.Categoria)
            .Where(l => l.Destacado && l.Stock > 0 && l.Estado == EstadoLibro.Disponible)
            .OrderByDescending(l => l.CreatedAt)
            .Take(cantidad)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Libro>> GetNovedadesAsync(
        int cantidad = 10,
        CancellationToken cancellationToken = default)
    {
        var fechaLimite = DateTime.UtcNow.AddMonths(-3); // Últimos 3 meses
        return await _dbSet
            .AsNoTracking()
            .Include(l => l.Categoria)
            .Where(l => l.CreatedAt >= fechaLimite && l.Stock > 0 && l.Estado == EstadoLibro.Disponible)
            .OrderByDescending(l => l.CreatedAt)
            .Take(cantidad)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Libro>> GetMasVendidosAsync(
        int cantidad = 10,
        DateTime? fechaDesde = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.DetallesVenta
            .AsNoTracking()
            .Include(dv => dv.Libro)
            .ThenInclude(l => l.Categoria)
            .Where(dv => !dv.Libro.IsDeleted);

        if (fechaDesde.HasValue)
        {
            query = query.Where(dv => dv.CreatedAt >= fechaDesde.Value);
        }

        return await query
            .GroupBy(dv => dv.Libro)
            .Select(g => new
            {
                Libro = g.Key,
                TotalVendido = g.Sum(dv => dv.Cantidad)
            })
            .OrderByDescending(x => x.TotalVendido)
            .Take(cantidad)
            .Select(x => x.Libro)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Libro>> GetOfertasAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(l => l.Categoria)
            .Where(l => l.PrecioOferta.HasValue &&
                       l.PrecioOferta < l.Precio &&
                       l.Stock > 0 &&
                       l.Estado == EstadoLibro.Disponible)
            .OrderBy(l => l.Titulo)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> VerificarStockAsync(
        Guid libroId,
        int cantidadRequerida,
        CancellationToken cancellationToken = default)
    {
        var libro = await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == libroId, cancellationToken);

        return libro != null && libro.Stock >= cantidadRequerida;
    }

    public async Task ActualizarStockAsync(
        Guid libroId,
        int cantidad,
        TipoMovimientoInventario tipoMovimiento,
        string? motivo = null,
        CancellationToken cancellationToken = default)
    {
        var libro = await _dbSet.FindAsync([libroId], cancellationToken);
        if (libro == null)
            throw new InvalidOperationException($"Libro con ID {libroId} no encontrado");

        var stockAnterior = libro.Stock;

        // Actualizar stock según tipo de movimiento
        libro.Stock = tipoMovimiento switch
        {
            TipoMovimientoInventario.Ingreso => libro.Stock + cantidad,
            TipoMovimientoInventario.Venta => libro.Stock - cantidad,
            TipoMovimientoInventario.Ajuste => cantidad, // Ajuste directo
            TipoMovimientoInventario.Devolucion => libro.Stock + cantidad,
            TipoMovimientoInventario.Anulacion => libro.Stock + cantidad,
            _ => libro.Stock
        };

        // Validar stock no negativo
        if (libro.Stock < 0)
            throw new InvalidOperationException("El stock no puede ser negativo");

        // Crear registro de auditoría
        var auditoria = new AuditoriaInventario
        {
            LibroId = libroId,
            TipoMovimiento = tipoMovimiento,
            StockAnterior = stockAnterior,
            Cantidad = cantidad,
            StockNuevo = libro.Stock,
            Motivo = motivo,
            FechaMovimiento = DateTime.UtcNow
        };

        await _context.AuditoriasInventario.AddAsync(auditoria, cancellationToken);
        _dbSet.Update(libro);
    }

    public async Task<IEnumerable<Libro>> GetStockBajoAsync(
        int umbral = 10,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(l => l.Categoria)
            .Where(l => l.Stock <= umbral && l.Estado == EstadoLibro.Disponible)
            .OrderBy(l => l.Stock)
            .ToListAsync(cancellationToken);
    }

    public async Task<Libro?> GetByISBNAsync(
        string isbn,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(l => l.Categoria)
            .FirstOrDefaultAsync(l => l.ISBN == isbn, cancellationToken);
    }

    public async Task<bool> ISBNExisteAsync(
        string isbn,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(l => l.ISBN == isbn);

        if (excludeId.HasValue)
        {
            query = query.Where(l => l.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<(IEnumerable<Libro> Items, int TotalCount)> GetPagedConFiltrosAsync(
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
        CancellationToken cancellationToken = default)
    {
        IQueryable<Libro> query = _dbSet.AsNoTracking().Include(l => l.Categoria);

        // Aplicar filtros
        if (!string.IsNullOrWhiteSpace(textoBusqueda))
        {
            query = query.Where(l =>
                l.Titulo.Contains(textoBusqueda) ||
                l.Autor.Contains(textoBusqueda) ||
                (l.Editorial != null && l.Editorial.Contains(textoBusqueda)));
        }

        if (categoriaId.HasValue)
        {
            query = query.Where(l => l.CategoriaId == categoriaId.Value);
        }

        if (precioMin.HasValue)
        {
            query = query.Where(l => l.Precio >= precioMin.Value);
        }

        if (precioMax.HasValue)
        {
            query = query.Where(l => l.Precio <= precioMax.Value);
        }

        if (destacado.HasValue)
        {
            query = query.Where(l => l.Destacado == destacado.Value);
        }

        if (soloDisponibles == true)
        {
            query = query.Where(l => l.Stock > 0 && l.Estado == EstadoLibro.Disponible);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        // Aplicar ordenamiento
        query = ordenarPor?.ToLower() switch
        {
            "precio" => ordenAscendente ? query.OrderBy(l => l.Precio) : query.OrderByDescending(l => l.Precio),
            "titulo" => ordenAscendente ? query.OrderBy(l => l.Titulo) : query.OrderByDescending(l => l.Titulo),
            "autor" => ordenAscendente ? query.OrderBy(l => l.Autor) : query.OrderByDescending(l => l.Autor),
            "fecha" => ordenAscendente ? query.OrderBy(l => l.CreatedAt) : query.OrderByDescending(l => l.CreatedAt),
            _ => query.OrderBy(l => l.Titulo)
        };

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}
