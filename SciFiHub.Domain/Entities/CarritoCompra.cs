using SciFiHub.Domain.Common;
using SciFiHub.Domain.Enums;

namespace SciFiHub.Domain.Entities;

/// <summary>
/// Entidad CarritoCompra - Carritos de compra persistentes
/// </summary>
public class CarritoCompra : BaseEntity
{
    public Guid ClienteId { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaActualizacion { get; set; }
    public EstadoCarrito Estado { get; set; }

    // Relaciones de navegación
    public virtual Usuario Cliente { get; set; } = null!;
    public virtual ICollection<DetalleCarrito> Detalles { get; set; } = new List<DetalleCarrito>();

    // Propiedades calculadas
    public decimal SubtotalCarrito => Detalles.Sum(d => d.PrecioUnitario * d.Cantidad);
    public int CantidadTotalItems => Detalles.Sum(d => d.Cantidad);
    public bool EstaVacio => !Detalles.Any();

    // Métodos de dominio
    public void AgregarItem(Libro libro, int cantidad)
    {
        if (!libro.VerificarDisponibilidad(cantidad))
            throw new InvalidOperationException($"No hay suficiente stock disponible para {libro.Titulo}");

        var detalleExistente = Detalles.FirstOrDefault(d => d.LibroId == libro.Id);

        if (detalleExistente != null)
        {
            detalleExistente.ActualizarCantidad(detalleExistente.Cantidad + cantidad);
        }
        else
        {
            var nuevoDetalle = new DetalleCarrito
            {
                Id = Guid.NewGuid(),
                CarritoId = Id,
                LibroId = libro.Id,
                Cantidad = cantidad,
                PrecioUnitario = libro.PrecioFinal,
                FechaAgregado = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };
            Detalles.Add(nuevoDetalle);
        }

        FechaActualizacion = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoverItem(Guid libroId)
    {
        var detalle = Detalles.FirstOrDefault(d => d.LibroId == libroId);
        if (detalle != null)
        {
            Detalles.Remove(detalle);
            FechaActualizacion = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void ActualizarCantidadItem(Guid libroId, int nuevaCantidad)
    {
        var detalle = Detalles.FirstOrDefault(d => d.LibroId == libroId);
        if (detalle == null)
            throw new InvalidOperationException("El item no existe en el carrito");

        if (nuevaCantidad <= 0)
        {
            RemoverItem(libroId);
        }
        else
        {
            detalle.ActualizarCantidad(nuevaCantidad);
            FechaActualizacion = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void Vaciar()
    {
        Detalles.Clear();
        FechaActualizacion = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarcarComoConvertido()
    {
        Estado = EstadoCarrito.Convertido;
        FechaActualizacion = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarcarComoAbandonado()
    {
        Estado = EstadoCarrito.Abandonado;
        FechaActualizacion = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool VerificarDisponibilidadItems()
    {
        return Detalles.All(d => d.Libro.VerificarDisponibilidad(d.Cantidad));
    }
}
