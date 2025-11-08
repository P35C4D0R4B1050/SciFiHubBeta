using SciFiHub.Web.DTOs.Common;
using SciFiHub.Web.DTOs.Carrito;
using SciFiHub.Web.DTOs.Venta;

namespace SciFiHub.Web.Services.Interfaces;

/// <summary>
/// Servicio para gestión del carrito de compras
/// </summary>
public interface ICarritoService
{
    // ===================================
    // Gestión del Carrito
    // ===================================
    
    /// <summary>
    /// Obtener carrito activo del cliente
    /// </summary>
    Task<Result<CarritoDTO>> ObtenerCarritoPorClienteAsync(Guid clienteId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Agregar item al carrito (crea carrito si no existe)
    /// </summary>
    Task<Result<CarritoDTO>> AgregarItemAsync(Guid clienteId, AgregarAlCarritoDTO agregarDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Actualizar cantidad de un item en el carrito
    /// </summary>
    Task<Result<CarritoDTO>> ActualizarCantidadAsync(Guid clienteId, ActualizarCantidadCarritoDTO actualizarDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Eliminar item del carrito
    /// </summary>
    Task<Result<CarritoDTO>> EliminarItemAsync(Guid clienteId, Guid libroId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Limpiar todo el carrito
    /// </summary>
    Task<Result> LimpiarCarritoAsync(Guid clienteId, CancellationToken cancellationToken = default);

    // ===================================
    // Validaciones y Cálculos
    // ===================================
    
    /// <summary>
    /// Validar que todos los items del carrito tengan stock suficiente
    /// </summary>
    Task<Result<bool>> ValidarStockCarritoAsync(Guid clienteId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtener resumen del carrito (para badge/contador)
    /// </summary>
    Task<Result<ResumenCarritoDTO>> ObtenerResumenCarritoAsync(Guid clienteId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Calcular total del carrito con impuestos
    /// </summary>
    Task<Result<decimal>> CalcularTotalCarritoAsync(Guid clienteId, CancellationToken cancellationToken = default);

    // ===================================
    // Checkout (Conversión a Venta)
    // ===================================
    
    /// <summary>
    /// Procesar checkout: convierte el carrito en una venta
    /// </summary>
    Task<Result<VentaDTO>> ProcesarCheckoutAsync(Guid clienteId, CheckoutDTO checkoutDto, Guid? vendedorId = null, CancellationToken cancellationToken = default);

    // ===================================
    // Gestión de Carritos Abandonados
    // ===================================
    
    /// <summary>
    /// Obtener carritos abandonados (para recuperación o limpieza)
    /// </summary>
    Task<Result<IEnumerable<CarritoDTO>>> ObtenerCarritosAbandonadosAsync(int diasInactividad = 30, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marcar carrito como abandonado
    /// </summary>
    Task<Result> MarcarComoAbandonadoAsync(Guid carritoId, CancellationToken cancellationToken = default);
}
