using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using SciFiHub.Domain.Enums;

namespace SciFiHub.Web.DTOs.Venta;

/// <summary>
/// DTO para crear una nueva venta
/// </summary>
public record CrearVentaDTO
{
    [Required(ErrorMessage = "El cliente es requerido")]
    public Guid ClienteId { get; init; }

    public Guid? VendedorId { get; init; }

    [Required(ErrorMessage = "El método de pago es requerido")]
    [JsonPropertyName("metodoPago")]
    public string? MetodoPagoString { get; set; }
    
    // Propiedad calculada que convierte el string a enum
    [JsonIgnore]
    public MetodoPago MetodoPago
    {
        get => string.IsNullOrEmpty(MetodoPagoString)
            ? MetodoPago.Efectivo
            : Enum.TryParse<MetodoPago>(MetodoPagoString, true, out var result)
                ? result
                : MetodoPago.Efectivo;
        set => MetodoPagoString = value.ToString();
    }

    [Required(ErrorMessage = "La dirección de envío es requerida")]
    [JsonPropertyName("direccionEnvio")]
    public string DireccionEnvioString { get; init; } = string.Empty;

    [Required(ErrorMessage = "Los detalles de venta son requeridos")]
    [MinLength(1, ErrorMessage = "Debe haber al menos un item en la venta")]
    public List<DetalleVentaDTO> Detalles { get; init; } = new();

    public string? NotasVenta { get; init; }
}

/// <summary>
/// DTO para detalle de venta individual
/// </summary>
public record DetalleVentaDTO
{
    [Required(ErrorMessage = "El libro es requerido")]
    public Guid LibroId { get; init; }

    [Required(ErrorMessage = "La cantidad es requerida")]
    [Range(1, 999, ErrorMessage = "La cantidad debe estar entre 1 y 999")]
    public int Cantidad { get; init; }

    [Required(ErrorMessage = "El precio unitario es requerido")]
    [Range(0.01, 999999.99, ErrorMessage = "El precio debe ser mayor a 0")]
    public decimal PrecioUnitario { get; init; }

    [Range(0, 100, ErrorMessage = "El descuento debe estar entre 0 y 100")]
    public decimal Descuento { get; init; } = 0;
}

/// <summary>
/// DTO para dirección de envío
/// </summary>
public record DireccionEnvioDTO
{
    [Required(ErrorMessage = "La calle es requerida")]
    [StringLength(200, ErrorMessage = "La calle no puede exceder 200 caracteres")]
    public string Calle { get; init; } = string.Empty;

    [Required(ErrorMessage = "La ciudad es requerida")]
    [StringLength(100, ErrorMessage = "La ciudad no puede exceder 100 caracteres")]
    public string Ciudad { get; init; } = string.Empty;

    [Required(ErrorMessage = "El departamento es requerido")]
    [StringLength(100, ErrorMessage = "El departamento no puede exceder 100 caracteres")]
    public string Departamento { get; init; } = string.Empty;

    [Required(ErrorMessage = "El país es requerido")]
    [StringLength(100, ErrorMessage = "El país no puede exceder 100 caracteres")]
    public string Pais { get; init; } = string.Empty;

    [StringLength(20, ErrorMessage = "El código postal no puede exceder 20 caracteres")]
    public string? CodigoPostal { get; init; }

    [StringLength(200, ErrorMessage = "La referencia no puede exceder 200 caracteres")]
    public string? Referencia { get; init; }
}

/// <summary>
/// DTO para mostrar información de venta
/// </summary>
public record VentaDTO
{
    public Guid Id { get; init; }
    public string NumeroVenta { get; init; } = string.Empty;
    public Guid ClienteId { get; init; }
    public string ClienteNombre { get; init; } = string.Empty;
    public Guid? VendedorId { get; init; }
    public string? VendedorNombre { get; init; }
    public DateTime FechaVenta { get; init; }
    public decimal Subtotal { get; init; }
    public decimal Descuento { get; init; }
    public decimal IGV { get; init; }
    public decimal Total { get; init; }
    public EstadoVenta EstadoVenta { get; init; }
    public MetodoPago MetodoPago { get; init; }
    public DireccionEnvioDTO? DireccionEnvio { get; init; }
    public string? NotasVenta { get; init; }
    public List<DetalleVentaMostrarDTO> Detalles { get; init; } = new();
    public int CantidadItems => Detalles.Sum(d => d.Cantidad);
    public string EstadoNombre => EstadoVenta.ToString();
    public string MetodoPagoNombre => MetodoPago.ToString();
}

/// <summary>
/// DTO para mostrar detalle de venta
/// </summary>
public record DetalleVentaMostrarDTO
{
    public Guid LibroId { get; init; }
    public string LibroISBN { get; init; } = string.Empty;
    public string LibroTitulo { get; init; } = string.Empty;
    public string LibroAutor { get; init; } = string.Empty;
    public string? LibroImagen { get; init; }
    public int Cantidad { get; init; }
    public decimal PrecioUnitario { get; init; }
    public decimal Descuento { get; init; }
    public decimal Subtotal { get; init; }
}

/// <summary>
/// DTO para filtros de búsqueda de ventas
/// </summary>
public record VentasFiltroDTO
{
    public string? TextoBusqueda { get; init; }
    public Guid? ClienteId { get; init; }
    public Guid? VendedorId { get; init; }
    public DateTime? FechaInicio { get; init; }
    public DateTime? FechaFin { get; init; }
    public EstadoVenta? Estado { get; init; }
    public MetodoPago? MetodoPago { get; init; }
    public string? NumeroVenta { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

/// <summary>
/// DTO para cancelar venta
/// </summary>
public record CancelarVentaDTO
{
    [Required(ErrorMessage = "El ID de la venta es requerido")]
    public Guid VentaId { get; init; }

    [Required(ErrorMessage = "El motivo de cancelación es requerido")]
    [StringLength(500, ErrorMessage = "El motivo no puede exceder 500 caracteres")]
    public string Motivo { get; init; } = string.Empty;
}
