using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SciFiHub.Domain.Entities;
using SciFiHub.Domain.Enums;
using SciFiHub.Domain.Interfaces;
using SciFiHub.Web.DTOs.Common;
using SciFiHub.Web.DTOs.Venta;
using SciFiHub.Web.Services.Interfaces;

namespace SciFiHub.Web.Services;

/// <summary>
/// Implementación del servicio de gestión de ventas
/// </summary>
public class VentaService : IVentaService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<VentaService> _logger;
    private readonly IConfiguration _configuration;

    public VentaService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<VentaService> logger,
        IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<Result<VentaDTO>> RegistrarVentaAsync(CrearVentaDTO crearVentaDto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("?? Iniciando registro de venta");
            
            // Validar que el cliente existe
            if (!await _unitOfWork.Usuarios.ExistsAsync(crearVentaDto.ClienteId, cancellationToken))
            {
                return Result<VentaDTO>.FailureResult("El cliente no existe");
            }

            // Validar vendedor si se proporciona
            if (crearVentaDto.VendedorId.HasValue &&
                !await _unitOfWork.Usuarios.ExistsAsync(crearVentaDto.VendedorId.Value, cancellationToken))
            {
                return Result<VentaDTO>.FailureResult("El vendedor no existe");
            }

            // Validar stock para todos los items
            foreach (var detalle in crearVentaDto.Detalles)
            {
                if (!await _unitOfWork.Libros.VerificarStockAsync(detalle.LibroId, detalle.Cantidad, cancellationToken))
                {
                    return Result<VentaDTO>.FailureResult($"Stock insuficiente para uno de los libros");
                }
            }

            // Preparar datos
            var ventaId = Guid.NewGuid();
            var numeroVenta = await GenerarNumeroVentaAsync(cancellationToken);
            var fechaVenta = DateTime.UtcNow;
            
            // Calcular totales
            decimal subtotal = 0;
            foreach (var detalleDto in crearVentaDto.Detalles)
            {
                var subtotalDetalle = (detalleDto.PrecioUnitario * detalleDto.Cantidad) - detalleDto.Descuento;
                subtotal += subtotalDetalle;
            }
            
            decimal descuento = 0;
            decimal igv = (subtotal - descuento) * 0.18m;
            decimal total = subtotal - descuento + igv;

            // Parsear dirección
            string direccionCompleta = crearVentaDto.DireccionEnvioString ?? "";

            // Obtener conexión directa
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync(cancellationToken);
            
            using var transaction = connection.BeginTransaction();
            
            try
            {
                // ============================================
                // INSERTAR VENTA - SOLO COLUMNAS QUE EXISTEN
                // ============================================
                var sqlVenta = @"
                    INSERT INTO Ventas (
                        Id, NumeroVenta, ClienteId, VendedorId, FechaVenta,
                        Subtotal, Descuento, IGV, Total,
                        EstadoVenta, MetodoPago,
                        DireccionCalle,
                        NotasVenta,
                        CreatedAt, IsDeleted
                    ) VALUES (
                        @Id, @NumeroVenta, @ClienteId, @VendedorId, @FechaVenta,
                        @Subtotal, @Descuento, @IGV, @Total,
                        @EstadoVenta, @MetodoPago,
                        @DireccionCalle,
                        @NotasVenta,
                        @CreatedAt, @IsDeleted
                    )";

                using (var cmd = new SqlCommand(sqlVenta, connection, transaction))
                {
                    cmd.Parameters.AddWithValue("@Id", ventaId);
                    cmd.Parameters.AddWithValue("@NumeroVenta", numeroVenta);
                    cmd.Parameters.AddWithValue("@ClienteId", crearVentaDto.ClienteId);
                    cmd.Parameters.AddWithValue("@VendedorId", crearVentaDto.VendedorId.HasValue ? crearVentaDto.VendedorId.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@FechaVenta", fechaVenta);
                    cmd.Parameters.AddWithValue("@Subtotal", subtotal);
                    cmd.Parameters.AddWithValue("@Descuento", descuento);
                    cmd.Parameters.AddWithValue("@IGV", igv);
                    cmd.Parameters.AddWithValue("@Total", total);
                    cmd.Parameters.AddWithValue("@EstadoVenta", "Pendiente");
                    cmd.Parameters.AddWithValue("@MetodoPago", crearVentaDto.MetodoPago.ToString());
                    cmd.Parameters.AddWithValue("@DireccionCalle", direccionCompleta);
                    cmd.Parameters.AddWithValue("@NotasVenta", crearVentaDto.NotasVenta ?? string.Empty);
                    cmd.Parameters.AddWithValue("@CreatedAt", fechaVenta);
                    cmd.Parameters.AddWithValue("@IsDeleted", false);

                    await cmd.ExecuteNonQueryAsync(cancellationToken);
                }

                _logger.LogInformation("? Venta insertada: {VentaId}", ventaId);

                // ============================================
                // INSERTAR DETALLES DE VENTA
                // ============================================
                foreach (var detalleDto in crearVentaDto.Detalles)
                {
                    var detalleId = Guid.NewGuid();
                    var subtotalDetalle = (detalleDto.PrecioUnitario * detalleDto.Cantidad) - detalleDto.Descuento;

                    var sqlDetalle = @"
                        INSERT INTO DetallesVenta (
                            Id, VentaId, LibroId, Cantidad,
                            PrecioUnitario, Descuento, Subtotal,
                            CreatedAt, IsDeleted
                        ) VALUES (
                            @Id, @VentaId, @LibroId, @Cantidad,
                            @PrecioUnitario, @Descuento, @Subtotal,
                            @CreatedAt, @IsDeleted
                        )";

                    using (var cmd = new SqlCommand(sqlDetalle, connection, transaction))
                    {
                        cmd.Parameters.AddWithValue("@Id", detalleId);
                        cmd.Parameters.AddWithValue("@VentaId", ventaId);
                        cmd.Parameters.AddWithValue("@LibroId", detalleDto.LibroId);
                        cmd.Parameters.AddWithValue("@Cantidad", detalleDto.Cantidad);
                        cmd.Parameters.AddWithValue("@PrecioUnitario", detalleDto.PrecioUnitario);
                        cmd.Parameters.AddWithValue("@Descuento", detalleDto.Descuento);
                        cmd.Parameters.AddWithValue("@Subtotal", subtotalDetalle);
                        cmd.Parameters.AddWithValue("@CreatedAt", fechaVenta);
                        cmd.Parameters.AddWithValue("@IsDeleted", false);

                        await cmd.ExecuteNonQueryAsync(cancellationToken);
                    }

                    // Obtener stock actual
                    int stockActual = 0;
                    var sqlGetStock = "SELECT Stock FROM Libros WHERE Id = @LibroId";
                    using (var cmd = new SqlCommand(sqlGetStock, connection, transaction))
                    {
                        cmd.Parameters.AddWithValue("@LibroId", detalleDto.LibroId);
                        stockActual = (int)await cmd.ExecuteScalarAsync(cancellationToken);
                    }

                    // Actualizar stock
                    var sqlUpdateStock = @"
                        UPDATE Libros 
                        SET Stock = Stock - @Cantidad,
                            UpdatedAt = @UpdatedAt
                        WHERE Id = @LibroId";

                    using (var cmd = new SqlCommand(sqlUpdateStock, connection, transaction))
                    {
                        cmd.Parameters.AddWithValue("@Cantidad", detalleDto.Cantidad);
                        cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);
                        cmd.Parameters.AddWithValue("@LibroId", detalleDto.LibroId);
                        await cmd.ExecuteNonQueryAsync(cancellationToken);
                    }

                    var nuevoStock = stockActual - detalleDto.Cantidad;

                    // ? CORREGIDO: Tabla en SINGULAR
                    var sqlAuditoria = @"
                        INSERT INTO AuditoriaInventario (
                            Id, LibroId, TipoMovimiento, StockAnterior, Cantidad, StockNuevo,
                            Motivo, ReferenciaId, UsuarioId, FechaMovimiento
                        ) VALUES (
                            @Id, @LibroId, @TipoMovimiento, @StockAnterior, @Cantidad, @StockNuevo,
                            @Motivo, @ReferenciaId, @UsuarioId, @FechaMovimiento
                        )";

                    using (var cmd = new SqlCommand(sqlAuditoria, connection, transaction))
                    {
                        cmd.Parameters.AddWithValue("@Id", Guid.NewGuid());
                        cmd.Parameters.AddWithValue("@LibroId", detalleDto.LibroId);
                        cmd.Parameters.AddWithValue("@TipoMovimiento", "Venta");
                        cmd.Parameters.AddWithValue("@StockAnterior", stockActual);
                        cmd.Parameters.AddWithValue("@Cantidad", -detalleDto.Cantidad);
                        cmd.Parameters.AddWithValue("@StockNuevo", nuevoStock);
                        cmd.Parameters.AddWithValue("@Motivo", $"Venta {numeroVenta}");
                        cmd.Parameters.AddWithValue("@ReferenciaId", ventaId);
                        cmd.Parameters.AddWithValue("@UsuarioId", crearVentaDto.VendedorId.HasValue ? crearVentaDto.VendedorId.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@FechaMovimiento", DateTime.UtcNow);

                        await cmd.ExecuteNonQueryAsync(cancellationToken);
                    }
                }

                // Commit transacción
                transaction.Commit();
                
                _logger.LogInformation("? Venta {NumeroVenta} registrada exitosamente", numeroVenta);
            }
            catch (Exception exTrans)
            {
                transaction.Rollback();
                _logger.LogError(exTrans, "? Error en transacción de venta");
                throw;
            }

            // Retornar datos básicos
            return Result<VentaDTO>.SuccessResult(new VentaDTO
            {
                Id = ventaId,
                NumeroVenta = numeroVenta,
                ClienteId = crearVentaDto.ClienteId,
                VendedorId = crearVentaDto.VendedorId,
                FechaVenta = fechaVenta,
                Subtotal = subtotal,
                Descuento = descuento,
                IGV = igv,
                Total = total,
                EstadoVenta = EstadoVenta.Pendiente,
                MetodoPago = crearVentaDto.MetodoPago,
                Detalles = new List<DetalleVentaMostrarDTO>()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "? Error en RegistrarVentaAsync");
            _logger.LogError("InnerException: {Inner}", ex.InnerException?.Message);
            
            var errorMessage = ex.InnerException?.Message ?? ex.Message;
            return Result<VentaDTO>.FailureResult($"Error al registrar la venta: {errorMessage}");
        }
    }

    public async Task<Result<VentaDTO>> ObtenerVentaPorIdAsync(Guid ventaId, CancellationToken cancellationToken = default)
    {
        try
        {
            var venta = await _unitOfWork.Ventas.GetConDetallesAsync(ventaId, cancellationToken);

            if (venta == null)
            {
                return Result<VentaDTO>.FailureResult("Venta no encontrada");
            }

            var ventaDto = _mapper.Map<VentaDTO>(venta);
            return Result<VentaDTO>.SuccessResult(ventaDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en ObtenerVentaPorIdAsync");
            return Result<VentaDTO>.FailureResult("Error al obtener la venta");
        }
    }

    public async Task<Result<VentaDTO>> ObtenerVentaPorNumeroAsync(string numeroVenta, CancellationToken cancellationToken = default)
    {
        try
        {
            var venta = await _unitOfWork.Ventas.GetPorNumeroVentaAsync(numeroVenta, cancellationToken);

            if (venta == null)
            {
                return Result<VentaDTO>.FailureResult("Venta no encontrada");
            }

            var ventaDto = _mapper.Map<VentaDTO>(venta);
            return Result<VentaDTO>.SuccessResult(ventaDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en ObtenerVentaPorNumeroAsync");
            return Result<VentaDTO>.FailureResult("Error al obtener la venta");
        }
    }

    public async Task<Result<PagedResult<VentaDTO>>> ObtenerVentasAsync(VentasFiltroDTO filtro, CancellationToken cancellationToken = default)
    {
        return await ObtenerVentasPaginadasAsync(filtro, cancellationToken);
    }

    public async Task<Result<PagedResult<VentaDTO>>> ObtenerVentasPaginadasAsync(VentasFiltroDTO filtro, CancellationToken cancellationToken = default)
    {
        try
        {
            var (ventas, totalCount) = await _unitOfWork.Ventas.GetPagedConFiltrosAsync(
                filtro.PageNumber,
                filtro.PageSize,
                filtro.ClienteId,
                filtro.VendedorId,
                filtro.FechaInicio,
                filtro.FechaFin,
                filtro.Estado,
                filtro.MetodoPago,
                filtro.NumeroVenta,
                cancellationToken);

            var ventasDto = _mapper.Map<IEnumerable<VentaDTO>>(ventas);

            var result = new PagedResult<VentaDTO>
            {
                Items = ventasDto,
                PageNumber = filtro.PageNumber,
                PageSize = filtro.PageSize,
                TotalCount = totalCount
            };

            return Result<PagedResult<VentaDTO>>.SuccessResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en ObtenerVentasPaginadasAsync");
            return Result<PagedResult<VentaDTO>>.FailureResult("Error al obtener ventas");
        }
    }

    public async Task<Result<IEnumerable<VentaDTO>>> ObtenerVentasPorClienteAsync(Guid clienteId, CancellationToken cancellationToken = default)
    {
        try
        {
            var ventas = await _unitOfWork.Ventas.GetPorClienteAsync(clienteId, cancellationToken);
            var ventasDto = _mapper.Map<IEnumerable<VentaDTO>>(ventas);

            return Result<IEnumerable<VentaDTO>>.SuccessResult(ventasDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en ObtenerVentasPorClienteAsync");
            return Result<IEnumerable<VentaDTO>>.FailureResult("Error al obtener ventas del cliente");
        }
    }

    public async Task<Result<IEnumerable<VentaDTO>>> ObtenerVentasPorVendedorAsync(Guid vendedorId, CancellationToken cancellationToken = default)
    {
        try
        {
            var ventas = await _unitOfWork.Ventas.GetPorVendedorAsync(vendedorId, cancellationToken);
            var ventasDto = _mapper.Map<IEnumerable<VentaDTO>>(ventas);

            return Result<IEnumerable<VentaDTO>>.SuccessResult(ventasDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en ObtenerVentasPorVendedorAsync");
            return Result<IEnumerable<VentaDTO>>.FailureResult("Error al obtener ventas del vendedor");
        }
    }

    public async Task<Result> CambiarEstadoVentaAsync(Guid ventaId, EstadoVenta nuevoEstado, CancellationToken cancellationToken = default)
    {
        try
        {
            var venta = await _unitOfWork.Ventas.GetByIdAsync(ventaId, cancellationToken);

            if (venta == null)
            {
                return Result.FailureResult("Venta no encontrada");
            }

            venta.ActualizarEstado(nuevoEstado);
            await _unitOfWork.Ventas.UpdateAsync(venta, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            _logger.LogInformation("Estado de venta {NumeroVenta} cambiado a {Estado}", venta.NumeroVenta, nuevoEstado);

            return Result.SuccessResult();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en CambiarEstadoVentaAsync");
            return Result.FailureResult("Error al cambiar el estado de la venta");
        }
    }

    public async Task<Result> CompletarVentaAsync(Guid ventaId, CancellationToken cancellationToken = default)
    {
        try
        {
            var venta = await _unitOfWork.Ventas.GetByIdAsync(ventaId, cancellationToken);

            if (venta == null)
            {
                return Result.FailureResult("Venta no encontrada");
            }

            venta.Completar();
            await _unitOfWork.Ventas.UpdateAsync(venta, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            _logger.LogInformation("Venta {NumeroVenta} completada", venta.NumeroVenta);

            return Result.SuccessResult();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en CompletarVentaAsync");
            return Result.FailureResult(ex.Message);
        }
    }

    public async Task<Result> CancelarVentaAsync(CancelarVentaDTO cancelarVentaDto, CancellationToken cancellationToken = default)
    {
        try
        {
            // ? Usar el repositorio con AsNoTracking para evitar conflictos de tracking
            var venta = await _unitOfWork.Ventas.Query()
                .AsNoTracking()
                .Include(v => v.Detalles)
                .FirstOrDefaultAsync(v => v.Id == cancelarVentaDto.VentaId, cancellationToken);

            if (venta == null)
            {
                return Result.FailureResult("Venta no encontrada");
            }

            if (!venta.PuedeSerCancelada())
            {
                return Result.FailureResult("La venta no puede ser cancelada en su estado actual");
            }

            // Usar SQL directo para evitar problemas de tracking
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync(cancellationToken);
            
            using var transaction = connection.BeginTransaction();
            
            try
            {
                // Restaurar stock de todos los items
                foreach (var detalle in venta.Detalles)
                {
                    // Obtener stock actual
                    int stockActual = 0;
                    var sqlGetStock = "SELECT Stock FROM Libros WHERE Id = @LibroId";
                    using (var cmd = new SqlCommand(sqlGetStock, connection, transaction))
                    {
                        cmd.Parameters.AddWithValue("@LibroId", detalle.LibroId);
                        stockActual = (int)await cmd.ExecuteScalarAsync(cancellationToken);
                    }

                    // Actualizar stock (devolver la cantidad)
                    var sqlUpdateStock = @"
                        UPDATE Libros 
                        SET Stock = Stock + @Cantidad,
                            UpdatedAt = @UpdatedAt
                        WHERE Id = @LibroId";

                    using (var cmd = new SqlCommand(sqlUpdateStock, connection, transaction))
                    {
                        cmd.Parameters.AddWithValue("@Cantidad", detalle.Cantidad);
                        cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);
                        cmd.Parameters.AddWithValue("@LibroId", detalle.LibroId);
                        await cmd.ExecuteNonQueryAsync(cancellationToken);
                    }

                    var nuevoStock = stockActual + detalle.Cantidad;

                    // Registrar auditoría
                    var sqlAuditoria = @"
                        INSERT INTO AuditoriaInventario (
                            Id, LibroId, TipoMovimiento, StockAnterior, Cantidad, StockNuevo,
                            Motivo, ReferenciaId, UsuarioId, FechaMovimiento
                        ) VALUES (
                            @Id, @LibroId, @TipoMovimiento, @StockAnterior, @Cantidad, @StockNuevo,
                            @Motivo, @ReferenciaId, @UsuarioId, @FechaMovimiento
                        )";

                    using (var cmd = new SqlCommand(sqlAuditoria, connection, transaction))
                    {
                        cmd.Parameters.AddWithValue("@Id", Guid.NewGuid());
                        cmd.Parameters.AddWithValue("@LibroId", detalle.LibroId);
                        cmd.Parameters.AddWithValue("@TipoMovimiento", "Anulacion");
                        cmd.Parameters.AddWithValue("@StockAnterior", stockActual);
                        cmd.Parameters.AddWithValue("@Cantidad", detalle.Cantidad); // Positivo (devuelto)
                        cmd.Parameters.AddWithValue("@StockNuevo", nuevoStock);
                        cmd.Parameters.AddWithValue("@Motivo", $"Cancelación de venta {venta.NumeroVenta}: {cancelarVentaDto.Motivo}");
                        cmd.Parameters.AddWithValue("@ReferenciaId", venta.Id);
                        cmd.Parameters.AddWithValue("@UsuarioId", DBNull.Value);
                        cmd.Parameters.AddWithValue("@FechaMovimiento", DateTime.UtcNow);

                        await cmd.ExecuteNonQueryAsync(cancellationToken);
                    }
                }

                // Actualizar estado de venta
                var sqlUpdateVenta = @"
                    UPDATE Ventas 
                    SET EstadoVenta = @EstadoVenta,
                        NotasVenta = @NotasVenta,
                        UpdatedAt = @UpdatedAt
                    WHERE Id = @VentaId";

                using (var cmd = new SqlCommand(sqlUpdateVenta, connection, transaction))
                {
                    cmd.Parameters.AddWithValue("@EstadoVenta", "Cancelada");
                    cmd.Parameters.AddWithValue("@NotasVenta", $"CANCELADA: {cancelarVentaDto.Motivo}");
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);
                    cmd.Parameters.AddWithValue("@VentaId", venta.Id);
                    await cmd.ExecuteNonQueryAsync(cancellationToken);
                }

                transaction.Commit();
                
                _logger.LogInformation("Venta {NumeroVenta} cancelada", venta.NumeroVenta);

                return Result.SuccessResult();
            }
            catch (Exception exTrans)
            {
                transaction.Rollback();
                _logger.LogError(exTrans, "? Error al cancelar venta");
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en CancelarVentaAsync");
            return Result.FailureResult(ex.Message);
        }
    }

    public async Task<Result<decimal>> ObtenerTotalVentasPorPeriodoAsync(DateTime fechaInicio, DateTime fechaFin, CancellationToken cancellationToken = default)
    {
        try
        {
            var total = await _unitOfWork.Ventas.GetTotalVentasPorPeriodoAsync(fechaInicio, fechaFin, cancellationToken);
            return Result<decimal>.SuccessResult(total);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en ObtenerTotalVentasPorPeriodoAsync");
            return Result<decimal>.FailureResult("Error al obtener total de ventas");
        }
    }

    public async Task<Result<EstadisticasVentasDTO>> ObtenerEstadisticasDelMesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var ahora = DateTime.UtcNow;
            var primerDia = new DateTime(ahora.Year, ahora.Month, 1);
            var ultimoDia = primerDia.AddMonths(1).AddDays(-1);

            var totalVentas = await _unitOfWork.Ventas.GetTotalVentasPorPeriodoAsync(primerDia, ultimoDia, cancellationToken);
            var cantidadVentas = await _unitOfWork.Ventas.GetCantidadVentasPorPeriodoAsync(primerDia, ultimoDia, EstadoVenta.Completada, cancellationToken);

            var estadisticas = new EstadisticasVentasDTO
            {
                TotalVentas = totalVentas,
                CantidadVentas = cantidadVentas,
                PromedioVenta = cantidadVentas > 0 ? totalVentas / cantidadVentas : 0,
                CantidadClientes = 0
            };

            return Result<EstadisticasVentasDTO>.SuccessResult(estadisticas);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en ObtenerEstadisticasDelMesAsync");
            return Result<EstadisticasVentasDTO>.FailureResult("Error al obtener estadísticas");
        }
    }

    public async Task<Result<IEnumerable<LibroMasVendidoDTO>>> ObtenerLibrosMasVendidosAsync(int cantidad = 10, DateTime? fechaDesde = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var libros = await _unitOfWork.Ventas.GetLibrosMasVendidosAsync(cantidad, fechaDesde, cancellationToken);

            var librosDto = libros.Select(l => new LibroMasVendidoDTO
            {
                LibroId = l.LibroId,
                Titulo = l.Titulo,
                Autor = "",
                CantidadVendida = l.Cantidad
            });

            return Result<IEnumerable<LibroMasVendidoDTO>>.SuccessResult(librosDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en ObtenerLibrosMasVendidosAsync");
            return Result<IEnumerable<LibroMasVendidoDTO>>.FailureResult("Error al obtener libros más vendidos");
        }
    }

    public async Task<Result<IEnumerable<ClienteFrecuenteDTO>>> ObtenerClientesFrecuentesAsync(int cantidad = 10, DateTime? fechaDesde = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var clientes = await _unitOfWork.Ventas.GetClientesFrecuentesAsync(cantidad, fechaDesde, cancellationToken);

            var clientesDto = clientes.Select(c => new ClienteFrecuenteDTO
            {
                ClienteId = c.ClienteId,
                NombreCliente = c.NombreCliente,
                CantidadCompras = c.CantidadCompras,
                TotalGastado = c.TotalGastado
            });

            return Result<IEnumerable<ClienteFrecuenteDTO>>.SuccessResult(clientesDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en ObtenerClientesFrecuentesAsync");
            return Result<IEnumerable<ClienteFrecuenteDTO>>.FailureResult("Error al obtener clientes frecuentes");
        }
    }

    private async Task<string> GenerarNumeroVentaAsync(CancellationToken cancellationToken)
    {
        var fecha = DateTime.UtcNow;
        var prefijo = $"V{fecha:yyyyMMdd}";
        var contador = await _unitOfWork.Ventas.Query()
            .Where(v => v.NumeroVenta.StartsWith(prefijo))
            .CountAsync(cancellationToken);

        return $"{prefijo}-{(contador + 1):D4}";
    }
}
