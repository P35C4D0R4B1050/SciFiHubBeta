using SciFiHub.Domain.Entities;

namespace SciFiHub.Domain.Interfaces;

/// <summary>
/// Repositorio para la entidad CarritoCompra con operaciones específicas
/// </summary>
public interface ICarritoCompraRepository : IRepository<CarritoCompra>
{
    // Obtener carrito del cliente
    Task<CarritoCompra?> GetCarritoActivoPorClienteAsync(
        Guid clienteId, 
        CancellationToken cancellationToken = default);
    
    Task<CarritoCompra?> GetCarritoConDetallesAsync(
        Guid carritoId, 
        CancellationToken cancellationToken = default);
    
    // Gestión de items del carrito
    Task<DetalleCarrito?> GetDetalleCarritoAsync(
        Guid carritoId, 
        Guid libroId, 
        CancellationToken cancellationToken = default);
    
    Task AgregarItemAsync(
        Guid carritoId, 
        Guid libroId, 
        int cantidad,
        decimal precioUnitario,
        CancellationToken cancellationToken = default);
    
    Task ActualizarCantidadItemAsync(
        Guid carritoId, 
        Guid libroId, 
        int nuevaCantidad,
        CancellationToken cancellationToken = default);
    
    Task EliminarItemAsync(
        Guid carritoId, 
        Guid libroId, 
        CancellationToken cancellationToken = default);
    
    Task LimpiarCarritoAsync(
        Guid carritoId, 
        CancellationToken cancellationToken = default);
    
    // Operaciones de carrito completo
    Task<decimal> CalcularTotalCarritoAsync(
        Guid carritoId, 
        CancellationToken cancellationToken = default);
    
    Task<int> ContarItemsCarritoAsync(
        Guid carritoId, 
        CancellationToken cancellationToken = default);
    
    Task<bool> ValidarStockCarritoAsync(
        Guid carritoId, 
        CancellationToken cancellationToken = default);
    
    // Conversión a venta
    Task MarcarComoConvertidoAsync(
        Guid carritoId, 
        CancellationToken cancellationToken = default);
    
    // Limpieza de carritos abandonados
    Task<IEnumerable<CarritoCompra>> GetCarritosAbandonadosAsync(
        int diasInactividad = 30,
        CancellationToken cancellationToken = default);
}
