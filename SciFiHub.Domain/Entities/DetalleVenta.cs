using SciFiHub.Domain.Common;

namespace SciFiHub.Domain.Entities;

/// <summary>
/// Entidad DetalleVenta - Líneas de detalle de cada venta
/// </summary>
public class DetalleVenta : BaseEntity
{
    public Guid VentaId { get; set; }
    public Guid LibroId { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Descuento { get; set; }
    public decimal Subtotal { get; set; }

    // Relaciones de navegación
    public virtual Venta Venta { get; set; } = null!;
    public virtual Libro Libro { get; set; } = null!;

    // Propiedades calculadas
    public decimal SubtotalSinDescuento => Cantidad * PrecioUnitario;

    // Métodos de dominio
    public void CalcularSubtotal()
    {
        Subtotal = (Cantidad * PrecioUnitario) - Descuento;
    }

    public void ActualizarCantidad(int nuevaCantidad)
    {
        if (nuevaCantidad <= 0)
            throw new ArgumentException("La cantidad debe ser mayor a cero", nameof(nuevaCantidad));

        Cantidad = nuevaCantidad;
        CalcularSubtotal();
        UpdatedAt = DateTime.UtcNow;
    }

    public void AplicarDescuento(decimal descuento)
    {
        if (descuento < 0)
            throw new ArgumentException("El descuento no puede ser negativo", nameof(descuento));

        if (descuento > SubtotalSinDescuento)
            throw new ArgumentException("El descuento no puede ser mayor al subtotal", nameof(descuento));

        Descuento = descuento;
        CalcularSubtotal();
        UpdatedAt = DateTime.UtcNow;
    }
}
