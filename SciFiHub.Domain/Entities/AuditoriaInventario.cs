using SciFiHub.Domain.Common;
using SciFiHub.Domain.Enums;

namespace SciFiHub.Domain.Entities;

/// <summary>
/// Entidad AuditoriaInventario - Tracking de cambios en inventario
/// </summary>
public class AuditoriaInventario : BaseEntity
{
    public Guid LibroId { get; set; }
    public TipoMovimientoInventario TipoMovimiento { get; set; }
    public int StockAnterior { get; set; }
    public int Cantidad { get; set; }
    public int StockNuevo { get; set; }
    public string? Motivo { get; set; }
    public Guid? ReferenciaId { get; set; }
    public Guid? UsuarioId { get; set; }
    public DateTime FechaMovimiento { get; set; }

    // Relaciones de navegación
    public virtual Libro Libro { get; set; } = null!;
    public virtual Usuario? Usuario { get; set; }

    // Métodos de dominio
    public static AuditoriaInventario CrearRegistro(
        Libro libro,
        TipoMovimientoInventario tipoMovimiento,
        int cantidad,
        string motivo,
        Guid? referenciaId = null,
        Guid? usuarioId = null)
    {
        var stockAnterior = libro.Stock;
        var stockNuevo = stockAnterior + cantidad;

        return new AuditoriaInventario
        {
            Id = Guid.NewGuid(),
            LibroId = libro.Id,
            TipoMovimiento = tipoMovimiento,
            StockAnterior = stockAnterior,
            Cantidad = cantidad,
            StockNuevo = stockNuevo,
            Motivo = motivo,
            ReferenciaId = referenciaId,
            UsuarioId = usuarioId,
            FechaMovimiento = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };
    }
}
