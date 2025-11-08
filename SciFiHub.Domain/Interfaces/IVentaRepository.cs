using SciFiHub.Domain.Entities;
using SciFiHub.Domain.Enums;

namespace SciFiHub.Domain.Interfaces;

/// <summary>
/// Repositorio para la entidad Venta con métodos de consultas y reportes
/// </summary>
public interface IVentaRepository : IRepository<Venta>
{
    // Consultas de ventas
    Task<Venta?> GetConDetallesAsync(
        Guid ventaId,
        CancellationToken cancellationToken = default);

    Task<Venta?> GetPorNumeroVentaAsync(
        string numeroVenta,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<Venta>> GetPorClienteAsync(
        Guid clienteId,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<Venta>> GetPorVendedorAsync(
        Guid vendedorId,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<Venta>> GetPorRangoFechaAsync(
        DateTime fechaInicio,
        DateTime fechaFin,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<Venta>> GetPorEstadoAsync(
        EstadoVenta estado,
        CancellationToken cancellationToken = default);

    // Estadísticas y reportes
    Task<decimal> GetTotalVentasPorPeriodoAsync(
        DateTime fechaInicio,
        DateTime fechaFin,
        CancellationToken cancellationToken = default);

    Task<decimal> GetTotalVentasDelMesAsync(
        int año,
        int mes,
        CancellationToken cancellationToken = default);

    Task<int> GetCantidadVentasPorPeriodoAsync(
        DateTime fechaInicio,
        DateTime fechaFin,
        EstadoVenta? estado = null,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<(Guid LibroId, string Titulo, int Cantidad)>> GetLibrosMasVendidosAsync(
        int cantidad = 10,
        DateTime? fechaDesde = null,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<(Guid ClienteId, string NombreCliente, int CantidadCompras, decimal TotalGastado)>> 
        GetClientesFrecuentesAsync(
            int cantidad = 10,
            DateTime? fechaDesde = null,
            CancellationToken cancellationToken = default);

    // Paginación con filtros
    Task<(IEnumerable<Venta> Items, int TotalCount)> GetPagedConFiltrosAsync(
        int pageNumber,
        int pageSize,
        Guid? clienteId = null,
        Guid? vendedorId = null,
        DateTime? fechaInicio = null,
        DateTime? fechaFin = null,
        EstadoVenta? estado = null,
        MetodoPago? metodoPago = null,
        string? numeroVenta = null,
        CancellationToken cancellationToken = default);
}
