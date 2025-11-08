using Microsoft.EntityFrameworkCore;
using SciFiHub.Domain.Entities;
using SciFiHub.Domain.Enums;
using SciFiHub.Domain.Interfaces;
using SciFiHub.Infrastructure.Data;

namespace SciFiHub.Infrastructure.Repositories;

/// <summary>
/// Implementación del repositorio de CarritoCompra
/// </summary>
public class CarritoCompraRepository : Repository<CarritoCompra>, ICarritoCompraRepository
{
    public CarritoCompraRepository(SciFiHubDbContext context) : base(context)
    {
    }

    public async Task<CarritoCompra?> GetCarritoActivoPorClienteAsync(
        Guid clienteId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.Detalles)
                .ThenInclude(d => d.Libro)
            .FirstOrDefaultAsync(c => c.ClienteId == clienteId &&
                                     c.Estado == EstadoCarrito.Activo,
                                cancellationToken);
    }

    public async Task<CarritoCompra?> GetCarritoConDetallesAsync(
        Guid carritoId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(c => c.Cliente)
            .Include(c => c.Detalles)
                .ThenInclude(d => d.Libro)
                    .ThenInclude(l => l.Categoria)
            .FirstOrDefaultAsync(c => c.Id == carritoId, cancellationToken);
    }

    public async Task<DetalleCarrito?> GetDetalleCarritoAsync(
        Guid carritoId,
        Guid libroId,
        CancellationToken cancellationToken = default)
    {
        return await _context.DetallesCarrito
            .FirstOrDefaultAsync(d => d.CarritoId == carritoId && d.LibroId == libroId, cancellationToken);
    }

    public async Task AgregarItemAsync(
        Guid carritoId,
        Guid libroId,
        int cantidad,
        decimal precioUnitario,
        CancellationToken cancellationToken = default)
    {
        // Verificar si el item ya existe
        var detalleExistente = await GetDetalleCarritoAsync(carritoId, libroId, cancellationToken);

        if (detalleExistente != null)
        {
            // Si ya existe, lanzar excepción
            throw new InvalidOperationException("Este libro ya está en tu carrito. Puedes modificar la cantidad desde el carrito.");
        }

        // Si no existe, crear nuevo detalle
        var nuevoDetalle = new DetalleCarrito
        {
            CarritoId = carritoId,
            LibroId = libroId,
            Cantidad = cantidad,
            PrecioUnitario = precioUnitario,
            FechaAgregado = DateTime.UtcNow
        };
        await _context.DetallesCarrito.AddAsync(nuevoDetalle, cancellationToken);

        // Actualizar fecha del carrito
        var carrito = await _dbSet.FindAsync([carritoId], cancellationToken);
        if (carrito != null)
        {
            carrito.FechaActualizacion = DateTime.UtcNow;
        }
    }

    public async Task ActualizarCantidadItemAsync(
        Guid carritoId,
        Guid libroId,
        int nuevaCantidad,
        CancellationToken cancellationToken = default)
    {
        // ? NO SE USA - Los cambios son solo visuales
        var detalle = await GetDetalleCarritoAsync(carritoId, libroId, cancellationToken);

        if (detalle == null)
            throw new InvalidOperationException("Item no existe");

        if (nuevaCantidad <= 0)
        {
            _context.DetallesCarrito.Remove(detalle);
        }
        else
        {
            detalle.Cantidad = nuevaCantidad;
            _context.DetallesCarrito.Update(detalle);
        }

        // Actualizar fecha del carrito
        var carrito = await _dbSet.FindAsync([carritoId], cancellationToken);
        if (carrito != null)
        {
            carrito.FechaActualizacion = DateTime.UtcNow;
        }
    }

    public async Task EliminarItemAsync(
        Guid carritoId,
        Guid libroId,
        CancellationToken cancellationToken = default)
    {
        // ? SQL DIRECTO - Más rápido
        await _context.Database.ExecuteSqlRawAsync(
            "DELETE FROM DetallesCarrito WHERE CarritoId = {0} AND LibroId = {1}",
            carritoId, libroId);

        // Actualizar fecha del carrito
        var carrito = await _dbSet.FindAsync([carritoId], cancellationToken);
        if (carrito != null)
        {
            carrito.FechaActualizacion = DateTime.UtcNow;
            _context.Update(carrito);
        }
    }

    public async Task LimpiarCarritoAsync(
        Guid carritoId,
        CancellationToken cancellationToken = default)
    {
        // ? OPTIMIZADO: SQL directo en una sola operación
        await _context.Database.ExecuteSqlRawAsync(
            "DELETE FROM DetallesCarrito WHERE CarritoId = {0}; UPDATE CarritoCompras SET FechaActualizacion = GETDATE() WHERE Id = {0};",
            carritoId);
    }

    public async Task<decimal> CalcularTotalCarritoAsync(
        Guid carritoId,
        CancellationToken cancellationToken = default)
    {
        var detalles = await _context.DetallesCarrito
            .Where(d => d.CarritoId == carritoId)
            .ToListAsync(cancellationToken);

        return detalles.Sum(d => d.Subtotal);
    }

    public async Task<int> ContarItemsCarritoAsync(
        Guid carritoId,
        CancellationToken cancellationToken = default)
    {
        return await _context.DetallesCarrito
            .Where(d => d.CarritoId == carritoId)
            .SumAsync(d => d.Cantidad, cancellationToken);
    }

    public async Task<bool> ValidarStockCarritoAsync(
        Guid carritoId,
        CancellationToken cancellationToken = default)
    {
        var detalles = await _context.DetallesCarrito
            .Include(d => d.Libro)
            .Where(d => d.CarritoId == carritoId)
            .ToListAsync(cancellationToken);

        return detalles.All(d => d.Libro.Stock >= d.Cantidad);
    }

    public async Task MarcarComoConvertidoAsync(
        Guid carritoId,
        CancellationToken cancellationToken = default)
    {
        var carrito = await _dbSet.FindAsync([carritoId], cancellationToken);

        if (carrito != null)
        {
            carrito.Estado = EstadoCarrito.Convertido;
            carrito.FechaActualizacion = DateTime.UtcNow;
            _dbSet.Update(carrito);
        }
    }

    public async Task<IEnumerable<CarritoCompra>> GetCarritosAbandonadosAsync(
        int diasInactividad = 30,
        CancellationToken cancellationToken = default)
    {
        var fechaLimite = DateTime.UtcNow.AddDays(-diasInactividad);

        return await _dbSet
            .AsNoTracking()
            .Include(c => c.Cliente)
            .Include(c => c.Detalles)
            .Where(c => c.Estado == EstadoCarrito.Activo &&
                       c.FechaActualizacion < fechaLimite)
            .ToListAsync(cancellationToken);
    }
}
