using SciFiHub.Web.DTOs.Common;
using SciFiHub.Web.DTOs.Venta;

namespace SciFiHub.Web.Services.Interfaces;

/// <summary>
/// Servicio para gestión de ventas y facturación
/// </summary>
public interface IVentaService
{
    // ===================================
    // CRUD y Gestión de Ventas
    // ===================================
    
    /// <summary>
    /// Registrar una nueva venta con todos sus detalles
    /// </summary>
    Task<Result<VentaDTO>> RegistrarVentaAsync(CrearVentaDTO crearVentaDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtener venta por ID con todos sus detalles
    /// </summary>
    Task<Result<VentaDTO>> ObtenerVentaPorIdAsync(Guid ventaId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtener venta por número de venta
    /// </summary>
    Task<Result<VentaDTO>> ObtenerVentaPorNumeroAsync(string numeroVenta, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtener ventas con filtros
    /// </summary>
    Task<Result<PagedResult<VentaDTO>>> ObtenerVentasAsync(VentasFiltroDTO filtro, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtener ventas con filtros y paginación
    /// </summary>
    Task<Result<PagedResult<VentaDTO>>> ObtenerVentasPaginadasAsync(VentasFiltroDTO filtro, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtener ventas de un cliente específico
    /// </summary>
    Task<Result<IEnumerable<VentaDTO>>> ObtenerVentasPorClienteAsync(Guid clienteId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtener ventas de un vendedor específico
    /// </summary>
    Task<Result<IEnumerable<VentaDTO>>> ObtenerVentasPorVendedorAsync(Guid vendedorId, CancellationToken cancellationToken = default);

    // ===================================
    // Gestión de Estado de Ventas
    // ===================================
    
    /// <summary>
    /// Cambiar estado de una venta
    /// </summary>
    Task<Result> CambiarEstadoVentaAsync(Guid ventaId, Domain.Enums.EstadoVenta nuevoEstado, CancellationToken cancellationToken = default);

    /// <summary>
    /// Completar una venta (marcar como completada)
    /// </summary>
    Task<Result> CompletarVentaAsync(Guid ventaId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cancelar una venta y restaurar stock
    /// </summary>
    Task<Result> CancelarVentaAsync(CancelarVentaDTO cancelarVentaDto, CancellationToken cancellationToken = default);

    // ===================================
    // Reportes y Estadísticas
    // ===================================
    
    /// <summary>
    /// Obtener total de ventas por período
    /// </summary>
    Task<Result<decimal>> ObtenerTotalVentasPorPeriodoAsync(DateTime fechaInicio, DateTime fechaFin, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtener estadísticas de ventas del mes actual
    /// </summary>
    Task<Result<EstadisticasVentasDTO>> ObtenerEstadisticasDelMesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtener libros más vendidos
    /// </summary>
    Task<Result<IEnumerable<LibroMasVendidoDTO>>> ObtenerLibrosMasVendidosAsync(int cantidad = 10, DateTime? fechaDesde = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtener clientes frecuentes
    /// </summary>
    Task<Result<IEnumerable<ClienteFrecuenteDTO>>> ObtenerClientesFrecuentesAsync(int cantidad = 10, DateTime? fechaDesde = null, CancellationToken cancellationToken = default);
}

/// <summary>
/// DTO para estadísticas de ventas
/// </summary>
public record EstadisticasVentasDTO
{
    public decimal TotalVentas { get; init; }
    public int CantidadVentas { get; init; }
    public decimal PromedioVenta { get; init; }
    public int CantidadClientes { get; init; }
}

/// <summary>
/// DTO para libros más vendidos
/// </summary>
public record LibroMasVendidoDTO
{
    public Guid LibroId { get; init; }
    public string Titulo { get; init; } = string.Empty;
    public string Autor { get; init; } = string.Empty;
    public int CantidadVendida { get; init; }
}

/// <summary>
/// DTO para clientes frecuentes
/// </summary>
public record ClienteFrecuenteDTO
{
    public Guid ClienteId { get; init; }
    public string NombreCliente { get; init; } = string.Empty;
    public int CantidadCompras { get; init; }
    public decimal TotalGastado { get; init; }
}
