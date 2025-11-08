using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace SciFiHub.Web.DTOs.Carrito;

/// <summary>
/// DTO para agregar item al carrito
/// </summary>
public record AgregarAlCarritoDTO
{
    [Required(ErrorMessage = "El ID del libro es requerido")]
    public Guid LibroId { get; init; }

    [Required(ErrorMessage = "La cantidad es requerida")]
    [Range(1, 99, ErrorMessage = "La cantidad debe estar entre 1 y 99")]
    public int Cantidad { get; init; }
}

/// <summary>
/// DTO para actualizar cantidad de item en carrito
/// </summary>
public record ActualizarCantidadCarritoDTO
{
    [Required(ErrorMessage = "El ID del libro es requerido")]
    public Guid LibroId { get; init; }

    [Required(ErrorMessage = "La nueva cantidad es requerida")]
    [Range(1, 99, ErrorMessage = "La cantidad debe estar entre 1 y 99")]
    public int NuevaCantidad { get; init; }
}

/// <summary>
/// DTO para mostrar el carrito completo
/// </summary>
public record CarritoDTO
{
    public Guid Id { get; init; }
    public Guid ClienteId { get; init; }
    public List<ItemCarritoDTO> Items { get; init; } = new();
    public decimal Subtotal => Items.Sum(i => i.Subtotal);
    public decimal IGV => Subtotal * 0.18m;
    public decimal Total => Subtotal + IGV;
    public int CantidadItems => Items.Sum(i => i.Cantidad);
    public bool TieneItems => Items.Any();
}

/// <summary>
/// DTO para item individual del carrito
/// </summary>
public record ItemCarritoDTO
{
    public Guid LibroId { get; init; }
    public string Titulo { get; init; } = string.Empty;
    public string Autor { get; init; } = string.Empty;
    public string? ImagenPortada { get; init; }
    public decimal PrecioUnitario { get; init; }
    public int Cantidad { get; init; }
    public decimal Subtotal => PrecioUnitario * Cantidad;
    public int StockDisponible { get; init; }
    public bool TieneStockSuficiente => StockDisponible >= Cantidad;
}

/// <summary>
/// DTO para el resumen del carrito (usado en badge/contador)
/// </summary>
public record ResumenCarritoDTO
{
    public int CantidadItems { get; init; }
    public decimal Total { get; init; }
}

/// <summary>
/// DTO para procesar checkout (convertir carrito a venta)
/// </summary>
public class CheckoutDTO
{
    [Required(ErrorMessage = "El método de pago es requerido")]
    public Domain.Enums.MetodoPago MetodoPago { get; set; }

    /// <summary>
    /// Dirección de envío como JSON string que será deserializado
    /// </summary>
    public string DireccionEnvioJson { get; set; } = string.Empty;

    /// <summary>
    /// Dirección de envío deserializada (no se envía desde el form, se construye en el controller)
    /// </summary>
    public Venta.DireccionEnvioDTO? DireccionEnvio { get; set; }

    public string? NotasVenta { get; set; }

    /// <summary>
    /// JSON con las cantidades temporales del frontend (formato: {"libroId": cantidad})
    /// </summary>
    public string? CantidadesTemporalesJson { get; set; }
}
