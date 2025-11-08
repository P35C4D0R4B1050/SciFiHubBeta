using Microsoft.EntityFrameworkCore;
using SciFiHub.Domain.Entities;
using SciFiHub.Domain.Enums;
using SciFiHub.Domain.Interfaces;
using SciFiHub.Infrastructure.Data;

namespace SciFiHub.Infrastructure.Repositories;

/// <summary>
/// Implementación del repositorio de Venta con consultas y reportes
/// </summary>
public class VentaRepository : Repository<Venta>, IVentaRepository
{
    public VentaRepository(SciFiHubDbContext context) : base(context)
    {
    }

    public async Task<Venta?> GetConDetallesAsync(
        Guid ventaId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(v => v.Cliente)
            .Include(v => v.Vendedor)
            .Include(v => v.Detalles)
                .ThenInclude(dv => dv.Libro)
            .FirstOrDefaultAsync(v => v.Id == ventaId, cancellationToken);
    }

    public async Task<Venta?> GetPorNumeroVentaAsync(
        string numeroVenta,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(v => v.Cliente)
            .Include(v => v.Vendedor)
            .Include(v => v.Detalles)
                .ThenInclude(dv => dv.Libro)
            .FirstOrDefaultAsync(v => v.NumeroVenta == numeroVenta, cancellationToken);
    }

    public async Task<IEnumerable<Venta>> GetPorClienteAsync(
        Guid clienteId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(v => v.Cliente)
            .Include(v => v.Vendedor)
            .Where(v => v.ClienteId == clienteId)
            .OrderByDescending(v => v.FechaVenta)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Venta>> GetPorVendedorAsync(
        Guid vendedorId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(v => v.Cliente)
            .Include(v => v.Vendedor)
            .Where(v => v.VendedorId == vendedorId)
            .OrderByDescending(v => v.FechaVenta)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Venta>> GetPorRangoFechaAsync(
        DateTime fechaInicio,
        DateTime fechaFin,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(v => v.Cliente)
            .Include(v => v.Vendedor)
            .Where(v => v.FechaVenta >= fechaInicio && v.FechaVenta <= fechaFin)
            .OrderByDescending(v => v.FechaVenta)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Venta>> GetPorEstadoAsync(
        EstadoVenta estado,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(v => v.Cliente)
            .Include(v => v.Vendedor)
            .Where(v => v.EstadoVenta == estado)
            .OrderByDescending(v => v.FechaVenta)
            .ToListAsync(cancellationToken);
    }

    public async Task<decimal> GetTotalVentasPorPeriodoAsync(
        DateTime fechaInicio,
        DateTime fechaFin,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(v => v.FechaVenta >= fechaInicio &&
                       v.FechaVenta <= fechaFin &&
                       (v.EstadoVenta == EstadoVenta.Pendiente || 
                        v.EstadoVenta == EstadoVenta.Procesando || 
                        v.EstadoVenta == EstadoVenta.Completada))
            .SumAsync(v => v.Total, cancellationToken);
    }

    public async Task<decimal> GetTotalVentasDelMesAsync(
        int año,
        int mes,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(v => v.FechaVenta.Year == año &&
                       v.FechaVenta.Month == mes &&
                       (v.EstadoVenta == EstadoVenta.Pendiente || 
                        v.EstadoVenta == EstadoVenta.Procesando || 
                        v.EstadoVenta == EstadoVenta.Completada))
            .SumAsync(v => v.Total, cancellationToken);
    }

    public async Task<int> GetCantidadVentasPorPeriodoAsync(
        DateTime fechaInicio,
        DateTime fechaFin,
        EstadoVenta? estado = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .AsNoTracking()
            .Where(v => v.FechaVenta >= fechaInicio && v.FechaVenta <= fechaFin);

        if (estado.HasValue)
        {
            query = query.Where(v => v.EstadoVenta == estado.Value);
        }
        else
        {
            // Si no se especifica estado, solo contar Pendiente, Procesando y Completada (excluir Canceladas y Reembolsadas)
            query = query.Where(v => 
                v.EstadoVenta == EstadoVenta.Pendiente || 
                v.EstadoVenta == EstadoVenta.Procesando || 
                v.EstadoVenta == EstadoVenta.Completada);
        }

        return await query.CountAsync(cancellationToken);
    }

    public async Task<IEnumerable<(Guid LibroId, string Titulo, int Cantidad)>> GetLibrosMasVendidosAsync(
        int cantidad = 10,
        DateTime? fechaDesde = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.DetallesVenta
            .AsNoTracking()
            .Include(dv => dv.Libro)
            .Include(dv => dv.Venta)
            .Where(dv => dv.Venta.EstadoVenta == EstadoVenta.Completada);

        if (fechaDesde.HasValue)
        {
            query = query.Where(dv => dv.Venta.FechaVenta >= fechaDesde.Value);
        }

        return await query
            .GroupBy(dv => new { dv.LibroId, dv.Libro.Titulo })
            .Select(g => new
            {
                LibroId = g.Key.LibroId,
                Titulo = g.Key.Titulo,
                CantidadVendida = g.Sum(dv => dv.Cantidad)
            })
            .OrderByDescending(x => x.CantidadVendida)
            .Take(cantidad)
            .Select(x => ValueTuple.Create(x.LibroId, x.Titulo, x.CantidadVendida))
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<(Guid ClienteId, string NombreCliente, int CantidadCompras, decimal TotalGastado)>> 
        GetClientesFrecuentesAsync(
            int cantidad = 10,
            DateTime? fechaDesde = null,
            CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .AsNoTracking()
            .Include(v => v.Cliente)
            .Where(v => v.EstadoVenta == EstadoVenta.Completada);

        if (fechaDesde.HasValue)
        {
            query = query.Where(v => v.FechaVenta >= fechaDesde.Value);
        }

        return await query
            .GroupBy(v => new { v.ClienteId, v.Cliente.NombreCompleto })
            .Select(g => new
            {
                ClienteId = g.Key.ClienteId,
                NombreCliente = g.Key.NombreCompleto,
                CantidadCompras = g.Count(),
                TotalGastado = g.Sum(v => v.Total)
            })
            .OrderByDescending(x => x.TotalGastado)
            .Take(cantidad)
            .Select(x => ValueTuple.Create(x.ClienteId, x.NombreCliente, x.CantidadCompras, x.TotalGastado))
            .ToListAsync(cancellationToken);
    }

    public async Task<(IEnumerable<Venta> Items, int TotalCount)> GetPagedConFiltrosAsync(
        int pageNumber,
        int pageSize,
        Guid? clienteId = null,
        Guid? vendedorId = null,
        DateTime? fechaInicio = null,
        DateTime? fechaFin = null,
        EstadoVenta? estado = null,
        MetodoPago? metodoPago = null,
        string? numeroVenta = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Venta> query = _dbSet
            .AsNoTracking()
            .Include(v => v.Cliente)
            .Include(v => v.Vendedor);

        // Aplicar filtros
        if (clienteId.HasValue)
        {
            query = query.Where(v => v.ClienteId == clienteId.Value);
        }

        if (vendedorId.HasValue)
        {
            query = query.Where(v => v.VendedorId == vendedorId.Value);
        }

        if (fechaInicio.HasValue)
        {
            query = query.Where(v => v.FechaVenta >= fechaInicio.Value);
        }

        if (fechaFin.HasValue)
        {
            query = query.Where(v => v.FechaVenta <= fechaFin.Value);
        }

        if (estado.HasValue)
        {
            query = query.Where(v => v.EstadoVenta == estado.Value);
        }

        if (metodoPago.HasValue)
        {
            query = query.Where(v => v.MetodoPago == metodoPago.Value);
        }

        if (!string.IsNullOrWhiteSpace(numeroVenta))
        {
            query = query.Where(v => v.NumeroVenta.Contains(numeroVenta));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(v => v.FechaVenta)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}
