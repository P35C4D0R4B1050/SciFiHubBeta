using SciFiHub.Domain.Common;
using SciFiHub.Domain.Enums;
using SciFiHub.Domain.ValueObjects;

namespace SciFiHub.Domain.Entities;

/// <summary>
/// Entidad Venta - Representa una venta realizada
/// </summary>
public class Venta : BaseEntity
{
    public string NumeroVenta { get; set; } = string.Empty;
    public Guid ClienteId { get; set; }
    public Guid? VendedorId { get; set; }
    public DateTime FechaVenta { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Descuento { get; set; }
    public decimal IGV { get; set; }
    public decimal Total { get; set; }
    public EstadoVenta EstadoVenta { get; set; }
    public MetodoPago MetodoPago { get; set; }
    
    // Dirección de envío (como columnas separadas en BD, pero Value Object en dominio)
    public string? DireccionCalle { get; set; }
    public string? DireccionCiudad { get; set; }
    public string? DireccionDepartamento { get; set; }
    public string? DireccionCodigoPostal { get; set; }
    public string? DireccionPais { get; set; }
    public string? DireccionReferencia { get; set; }
    
    public string? NotasVenta { get; set; }

    // Relaciones de navegación
    public virtual Usuario Cliente { get; set; } = null!;
    public virtual Usuario? Vendedor { get; set; }
    public virtual ICollection<DetalleVenta> Detalles { get; set; } = new List<DetalleVenta>();

    // Propiedad de conveniencia para dirección
    public DireccionEnvio? Direccion
    {
        get
        {
            if (string.IsNullOrWhiteSpace(DireccionCalle) ||
                string.IsNullOrWhiteSpace(DireccionCiudad) ||
                string.IsNullOrWhiteSpace(DireccionDepartamento) ||
                string.IsNullOrWhiteSpace(DireccionPais))
                return null;

            return new DireccionEnvio(
                DireccionCalle,
                DireccionCiudad,
                DireccionDepartamento,
                DireccionPais,
                DireccionCodigoPostal,
                DireccionReferencia
            );
        }
        set
        {
            if (value == null)
            {
                DireccionCalle = null;
                DireccionCiudad = null;
                DireccionDepartamento = null;
                DireccionCodigoPostal = null;
                DireccionPais = null;
                DireccionReferencia = null;
            }
            else
            {
                DireccionCalle = value.Calle;
                DireccionCiudad = value.Ciudad;
                DireccionDepartamento = value.Departamento;
                DireccionCodigoPostal = value.CodigoPostal;
                DireccionPais = value.Pais;
                DireccionReferencia = value.Referencia;
            }
        }
    }

    // Métodos de dominio
    public void CalcularTotales(decimal tasaIGV = 0.18m)
    {
        Subtotal = Detalles.Sum(d => d.Subtotal);
        IGV = (Subtotal - Descuento) * tasaIGV;
        Total = Subtotal - Descuento + IGV;
    }

    public void Completar()
    {
        if (EstadoVenta == EstadoVenta.Cancelada)
            throw new InvalidOperationException("No se puede completar una venta cancelada");

        EstadoVenta = EstadoVenta.Completada;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancelar(string motivo)
    {
        if (EstadoVenta == EstadoVenta.Completada)
            throw new InvalidOperationException("No se puede cancelar una venta completada sin procesar reembolso");

        EstadoVenta = EstadoVenta.Cancelada;
        NotasVenta = string.IsNullOrWhiteSpace(NotasVenta)
            ? $"Cancelada: {motivo}"
            : $"{NotasVenta}\nCancelada: {motivo}";
        UpdatedAt = DateTime.UtcNow;
    }

    public void ActualizarEstado(EstadoVenta nuevoEstado)
    {
        EstadoVenta = nuevoEstado;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool PuedeSerCancelada()
    {
        return EstadoVenta != EstadoVenta.Completada && 
               EstadoVenta != EstadoVenta.Cancelada;
    }

    public int CantidadTotalItems => Detalles.Sum(d => d.Cantidad);
}
