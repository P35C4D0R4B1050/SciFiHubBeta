using SciFiHub.Domain.Common;
using SciFiHub.Domain.Enums;

namespace SciFiHub.Domain.Entities;

/// <summary>
/// Entidad Libro - Representa el catálogo de libros
/// </summary>
public class Libro : BaseEntity
{
    public string ISBN { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string Autor { get; set; } = string.Empty;
    public string? Editorial { get; set; }
    public Guid? CategoriaId { get; set; }
    public int? AnioPublicacion { get; set; }
    public string? Sinopsis { get; set; }
    public string? Descripcion { get; set; }
    public decimal Precio { get; set; }
    public decimal? PrecioOferta { get; set; }
    public int Stock { get; set; }
    public string? ImagenPortada { get; set; }
    public string? ImagenesSecundarias { get; set; } // JSON array de URLs
    public decimal? Peso { get; set; }
    public string? Dimensiones { get; set; }
    public int? NumeroPaginas { get; set; }
    public string Idioma { get; set; } = "Español";
    public EstadoLibro Estado { get; set; }
    public bool Destacado { get; set; }
    public DateTime FechaIngreso { get; set; }

    // Relaciones de navegación
    public virtual Categoria? Categoria { get; set; }
    public virtual ICollection<DetalleVenta> DetallesVenta { get; set; } = new List<DetalleVenta>();
    public virtual ICollection<DetalleCarrito> DetallesCarrito { get; set; } = new List<DetalleCarrito>();
    public virtual ICollection<AuditoriaInventario> AuditoriasInventario { get; set; } = new List<AuditoriaInventario>();

    // Propiedades calculadas
    public decimal PrecioFinal => PrecioOferta ?? Precio;
    public bool TieneOferta => PrecioOferta.HasValue && PrecioOferta < Precio;
    public int? PorcentajeDescuento => TieneOferta ? (int?)((Precio - PrecioOferta!.Value) / Precio * 100) : null;
    public bool EstaDisponible => Stock > 0 && Estado == EstadoLibro.Disponible && !IsDeleted;
    public string EstadoStock
    {
        get
        {
            if (Stock == 0) return "Agotado";
            if (Stock <= 5) return "Bajo";
            if (Stock <= 20) return "Medio";
            return "Disponible";
        }
    }

    // Métodos de dominio
    public void ActualizarStock(int cantidad, string motivo)
    {
        var nuevoStock = Stock + cantidad;
        if (nuevoStock < 0)
            throw new InvalidOperationException("El stock no puede ser negativo");

        Stock = nuevoStock;
        Estado = Stock == 0 ? EstadoLibro.Agotado : EstadoLibro.Disponible;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AplicarOferta(decimal precioOferta)
    {
        if (precioOferta >= Precio)
            throw new ArgumentException("El precio de oferta debe ser menor al precio normal");

        PrecioOferta = precioOferta;
        UpdatedAt = DateTime.UtcNow;
    }

    public void QuitarOferta()
    {
        PrecioOferta = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarcarComoDestacado()
    {
        Destacado = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void DesmarcarDestacado()
    {
        Destacado = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool VerificarDisponibilidad(int cantidadRequerida)
    {
        return Stock >= cantidadRequerida && EstaDisponible;
    }
}
