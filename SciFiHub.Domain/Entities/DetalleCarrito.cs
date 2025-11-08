using SciFiHub.Domain.Common;

namespace SciFiHub.Domain.Entities;

/// <summary>
/// Entidad DetalleCarrito - Items en cada carrito de compra
/// </summary>
public class DetalleCarrito : BaseEntity
{
    public Guid CarritoId { get; set; }
    public Guid LibroId { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public DateTime FechaAgregado { get; set; }

    // Relaciones de navegación
    public virtual CarritoCompra Carrito { get; set; } = null!;
    public virtual Libro Libro { get; set; } = null!;

    // Propiedades calculadas
    public decimal Subtotal => Cantidad * PrecioUnitario;

    // Métodos de dominio
    public void ActualizarCantidad(int nuevaCantidad)
    {
        if (nuevaCantidad <= 0)
            throw new ArgumentException("La cantidad debe ser mayor a cero", nameof(nuevaCantidad));

        Cantidad = nuevaCantidad;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ActualizarPrecio(decimal nuevoPrecio)
    {
        if (nuevoPrecio < 0)
            throw new ArgumentException("El precio no puede ser negativo", nameof(nuevoPrecio));

        PrecioUnitario = nuevoPrecio;
        UpdatedAt = DateTime.UtcNow;
    }
}
