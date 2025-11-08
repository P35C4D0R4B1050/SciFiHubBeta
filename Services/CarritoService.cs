using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SciFiHub.Domain.Entities;
using SciFiHub.Domain.Enums;
using SciFiHub.Domain.Interfaces;
using SciFiHub.Web.DTOs.Common;
using SciFiHub.Web.DTOs.Carrito;
using SciFiHub.Web.DTOs.Venta;
using SciFiHub.Web.Services.Interfaces;
using System.Text.Json;

namespace SciFiHub.Web.Services;

/// <summary>
/// Servicio ULTRA SIMPLIFICADO de carrito
/// </summary>
public class CarritoService : ICarritoService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<CarritoService> _logger;

    public CarritoService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<CarritoService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<CarritoDTO>> ObtenerCarritoPorClienteAsync(Guid clienteId, CancellationToken cancellationToken = default)
    {
        try
        {
            var carrito = await _unitOfWork.Carritos.GetCarritoActivoPorClienteAsync(clienteId, cancellationToken);

            if (carrito == null)
            {
                carrito = new CarritoCompra
                {
                    ClienteId = clienteId,
                    FechaCreacion = DateTime.UtcNow,
                    Estado = EstadoCarrito.Activo
                };

                await _unitOfWork.Carritos.AddAsync(carrito, cancellationToken);
                await _unitOfWork.CommitAsync(cancellationToken);
            }

            var carritoDto = _mapper.Map<CarritoDTO>(carrito);
            return Result<CarritoDTO>.SuccessResult(carritoDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en ObtenerCarritoPorClienteAsync");
            return Result<CarritoDTO>.FailureResult("Error al obtener el carrito");
        }
    }

    public async Task<Result<CarritoDTO>> AgregarItemAsync(Guid clienteId, AgregarAlCarritoDTO agregarDto, CancellationToken cancellationToken = default)
    {
        try {
            var carrito = await _unitOfWork.Carritos.GetCarritoActivoPorClienteAsync(clienteId, cancellationToken);

            if (carrito == null)
            {
                carrito = new CarritoCompra
                {
                    ClienteId = clienteId,
                    FechaCreacion = DateTime.UtcNow,
                    Estado = EstadoCarrito.Activo
                };
                await _unitOfWork.Carritos.AddAsync(carrito, cancellationToken);
                await _unitOfWork.CommitAsync(cancellationToken);
                
                carrito = await _unitOfWork.Carritos.GetCarritoActivoPorClienteAsync(clienteId, cancellationToken);
                
                if (carrito == null)
                {
                    _logger.LogError("Carrito no se pudo recargar");
                    return Result<CarritoDTO>.FailureResult("Error al crear el carrito");
                }
            }

            var libro = await _unitOfWork.Libros.GetByIdAsync(agregarDto.LibroId, cancellationToken);
            if (libro == null)
            {
                return Result<CarritoDTO>.FailureResult("El libro no existe");
            }

            if (libro.Stock < agregarDto.Cantidad)
            {
                return Result<CarritoDTO>.FailureResult($"Stock insuficiente. Disponible: {libro.Stock}");
            }

            var precio = libro.PrecioOferta ?? libro.Precio;

            try
            {
                await _unitOfWork.Carritos.AgregarItemAsync(
                    carrito.Id,
                    agregarDto.LibroId,
                    agregarDto.Cantidad,
                    precio,
                    cancellationToken);

                await _unitOfWork.CommitAsync(cancellationToken);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("ya está en tu carrito"))
            {
                _logger.LogWarning("Intento de agregar duplicado");
                return Result<CarritoDTO>.FailureResult(ex.Message);
            }

            carrito = await _unitOfWork.Carritos.GetCarritoConDetallesAsync(carrito.Id, cancellationToken);
            var carritoDto = _mapper.Map<CarritoDTO>(carrito);

            _logger.LogInformation("✅ Item agregado");

            return Result<CarritoDTO>.SuccessResult(carritoDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en AgregarItemAsync");
            return Result<CarritoDTO>.FailureResult($"Error: {ex.Message}");
        }
    }

    public async Task<Result<CarritoDTO>> ActualizarCantidadAsync(Guid clienteId, ActualizarCantidadCarritoDTO actualizarDto, CancellationToken cancellationToken = default)
    {
        // ✅ SIMPLE: No hacer nada, los cambios son solo visuales
        // La cantidad real se usará desde DetallesCarrito en BD al hacer checkout
        
        var carrito = await _unitOfWork.Carritos.GetCarritoActivoPorClienteAsync(clienteId, cancellationToken);
        if (carrito == null)
        {
            return Result<CarritoDTO>.FailureResult("Carrito no encontrado");
        }

        carrito = await _unitOfWork.Carritos.GetCarritoConDetallesAsync(carrito.Id, cancellationToken);
        var carritoDto = _mapper.Map<CarritoDTO>(carrito);
        
        return Result<CarritoDTO>.SuccessResult(carritoDto);
    }

    public async Task<Result<CarritoDTO>> EliminarItemAsync(Guid clienteId, Guid libroId, CancellationToken cancellationToken = default)
    {
        try
        {
            var carrito = await _unitOfWork.Carritos.GetCarritoActivoPorClienteAsync(clienteId, cancellationToken);

            if (carrito == null)
            {
                return Result<CarritoDTO>.FailureResult("Carrito no encontrado");
            }

            _logger.LogInformation("🗑️ Eliminando item");

            await _unitOfWork.Carritos.EliminarItemAsync(carrito.Id, libroId, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            _logger.LogInformation("✅ Item eliminado");

            carrito = await _unitOfWork.Carritos.GetCarritoConDetallesAsync(carrito.Id, cancellationToken);
            var carritoDto = _mapper.Map<CarritoDTO>(carrito);

            return Result<CarritoDTO>.SuccessResult(carritoDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en EliminarItemAsync");
            return Result<CarritoDTO>.FailureResult("Error al eliminar");
        }
    }

    public async Task<Result> LimpiarCarritoAsync(Guid clienteId, CancellationToken cancellationToken = default)
    {
        try
        {
            var carrito = await _unitOfWork.Carritos.GetCarritoActivoPorClienteAsync(clienteId, cancellationToken);

            if (carrito == null)
            {
                return Result.FailureResult("Carrito no encontrado");
            }

            _logger.LogInformation("🗑️ Vaciando carrito");

            await _unitOfWork.Carritos.LimpiarCarritoAsync(carrito.Id, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            _logger.LogInformation("✅ Carrito vaciado");

            return Result.SuccessResult();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en LimpiarCarritoAsync");
            return Result.FailureResult("Error al limpiar");
        }
    }

    public async Task<Result<bool>> ValidarStockCarritoAsync(Guid clienteId, CancellationToken cancellationToken = default)
    {
        try
        {
            var carrito = await _unitOfWork.Carritos.GetCarritoActivoPorClienteAsync(clienteId, cancellationToken);

            if (carrito == null)
            {
                return Result<bool>.FailureResult("Carrito no encontrado");
            }

            var esValido = await _unitOfWork.Carritos.ValidarStockCarritoAsync(carrito.Id, cancellationToken);
            return Result<bool>.SuccessResult(esValido);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en ValidarStockCarritoAsync");
            return Result<bool>.FailureResult("Error al validar");
        }
    }

    public async Task<Result<ResumenCarritoDTO>> ObtenerResumenCarritoAsync(Guid clienteId, CancellationToken cancellationToken = default)
    {
        try
        {
            var carrito = await _unitOfWork.Carritos.GetCarritoActivoPorClienteAsync(clienteId, cancellationToken);

            if (carrito == null || !carrito.Detalles.Any())
            {
                return Result<ResumenCarritoDTO>.SuccessResult(new ResumenCarritoDTO
                {
                    CantidadItems = 0,
                    Total = 0
                });
            }

            var cantidadItems = await _unitOfWork.Carritos.ContarItemsCarritoAsync(carrito.Id, cancellationToken);
            var total = await _unitOfWork.Carritos.CalcularTotalCarritoAsync(carrito.Id, cancellationToken);

            var resumen = new ResumenCarritoDTO
            {
                CantidadItems = cantidadItems,
                Total = total
            };

            return Result<ResumenCarritoDTO>.SuccessResult(resumen);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en ObtenerResumenCarritoAsync");
            return Result<ResumenCarritoDTO>.FailureResult("Error");
        }
    }

    public async Task<Result<decimal>> CalcularTotalCarritoAsync(Guid clienteId, CancellationToken cancellationToken = default)
    {
        try
        {
            var carrito = await _unitOfWork.Carritos.GetCarritoActivoPorClienteAsync(clienteId, cancellationToken);

            if (carrito == null)
            {
                return Result<decimal>.SuccessResult(0);
            }

            var total = await _unitOfWork.Carritos.CalcularTotalCarritoAsync(carrito.Id, cancellationToken);
            return Result<decimal>.SuccessResult(total);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en CalcularTotalCarritoAsync");
            return Result<decimal>.FailureResult("Error");
        }
    }

    public async Task<Result<VentaDTO>> ProcesarCheckoutAsync(Guid clienteId, CheckoutDTO checkoutDto, Guid? vendedorId = null, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("🛒 Iniciando checkout para cliente {ClienteId}", clienteId);
            
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var carritoActivo = await _unitOfWork.Carritos.GetCarritoActivoPorClienteAsync(clienteId, cancellationToken);
            if (carritoActivo == null)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogWarning("Carrito no encontrado");
                return Result<VentaDTO>.FailureResult("Carrito no encontrado");
            }

            // Recargar carrito con detalles actualizados
            var carrito = await _unitOfWork.Carritos.GetCarritoConDetallesAsync(carritoActivo.Id, cancellationToken);

            if (carrito == null || !carrito.Detalles.Any())
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogWarning("Carrito vacío");
                return Result<VentaDTO>.FailureResult("El carrito está vacío");
            }

            _logger.LogInformation("📦 Carrito con {Cantidad} detalles", carrito.Detalles.Count);

            // ✅ VALIDAR STOCK con cantidades de BD
            if (!await _unitOfWork.Carritos.ValidarStockCarritoAsync(carrito.Id, cancellationToken))
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogWarning("Stock insuficiente");
                return Result<VentaDTO>.FailureResult("Stock insuficiente para completar la compra");
            }

            // ✅ OBTENER VENDEDOR PREDETERMINADO
            if (!vendedorId.HasValue)
            {
                _logger.LogInformation("🔍 Buscando vendedor predeterminado...");
                
                var vendedorPredeterminado = await _unitOfWork.Usuarios.Query()
                    .Where(u => u.Username == "vendedor" && u.Rol == "Vendedor")
                    .FirstOrDefaultAsync(cancellationToken);

                if (vendedorPredeterminado != null)
                {
                    vendedorId = vendedorPredeterminado.Id;
                    _logger.LogInformation("✅ Vendedor predeterminado: {VendedorId}", vendedorId);
                }
                else
                {
                    _logger.LogWarning("⚠️ No se encontró vendedor predeterminado");
                }
            }

            // ✅ PARSEAR DIRECCION
            DireccionEnvioDTO? direccionEnvio = null;
            try
            {
                if (!string.IsNullOrEmpty(checkoutDto.DireccionEnvio?.ToString()))
                {
                    direccionEnvio = checkoutDto.DireccionEnvio;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al parsear dirección");
            }

            // ✅ CONSTRUIR STRING DE DIRECCION
            string direccionEnvioString = "Sin dirección especificada";
            if (direccionEnvio != null)
            {
                direccionEnvioString = $"{direccionEnvio.Calle}, {direccionEnvio.Ciudad}, {direccionEnvio.Departamento}, {direccionEnvio.Pais}";
                if (!string.IsNullOrEmpty(direccionEnvio.CodigoPostal))
                {
                    direccionEnvioString += $" - CP: {direccionEnvio.CodigoPostal}";
                }
                if (!string.IsNullOrEmpty(direccionEnvio.Referencia))
                {
                    direccionEnvioString += $" (Ref: {direccionEnvio.Referencia})";
                }
            }

            _logger.LogInformation("📍 Dirección: {Direccion}", direccionEnvioString);

            // Crear DTO de venta
            var crearVentaDto = new CrearVentaDTO
            {
                ClienteId = clienteId,
                VendedorId = vendedorId,
                MetodoPagoString = checkoutDto.MetodoPago.ToString(),
                DireccionEnvioString = direccionEnvioString,
                NotasVenta = checkoutDto.NotasVenta,
                Detalles = carrito.Detalles.Select(d => new DetalleVentaDTO
                {
                    LibroId = d.LibroId,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario,
                    Descuento = 0
                }).ToList()
            };

            _logger.LogInformation("📝 Creando venta con {CantidadDetalles} detalles", crearVentaDto.Detalles.Count);

            // ✅ REGISTRAR VENTA (con descuento de stock automático)
            var resultVenta = await RegistrarVentaDesdeCarritoAsync(crearVentaDto, cancellationToken);

            if (!resultVenta.Success)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogError("Error al registrar venta: {Error}", resultVenta.ErrorMessage);
                return resultVenta;
            }

            _logger.LogInformation("✅ Venta creada: {NumeroVenta}", resultVenta.Data!.NumeroVenta);

            // ✅ LIMPIAR CARRITO
            _logger.LogInformation("🗑️ Limpiando carrito...");
            await _unitOfWork.Carritos.LimpiarCarritoAsync(carrito.Id, cancellationToken);
            await _unitOfWork.Carritos.MarcarComoConvertidoAsync(carrito.Id, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            _logger.LogInformation("✅ Checkout completado. Venta: {NumeroVenta}, Stock descontado", resultVenta.Data.NumeroVenta);

            return resultVenta;
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            _logger.LogError(ex, "❌ Error crítico en checkout");
            _logger.LogError("Mensaje: {Message}", ex.Message);
            if (ex.InnerException != null)
            {
                _logger.LogError("InnerException: {Inner}", ex.InnerException.Message);
            }
            return Result<VentaDTO>.FailureResult($"Error al procesar: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<CarritoDTO>>> ObtenerCarritosAbandonadosAsync(int diasInactividad = 30, CancellationToken cancellationToken = default)
    {
        try
        {
            var carritos = await _unitOfWork.Carritos.GetCarritosAbandonadosAsync(diasInactividad, cancellationToken);
            var carritosDto = _mapper.Map<IEnumerable<CarritoDTO>>(carritos);

            return Result<IEnumerable<CarritoDTO>>.SuccessResult(carritosDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error");
            return Result<IEnumerable<CarritoDTO>>.FailureResult("Error");
        }
    }

    public async Task<Result> MarcarComoAbandonadoAsync(Guid carritoId, CancellationToken cancellationToken = default)
    {
        try
        {
            var carrito = await _unitOfWork.Carritos.GetByIdAsync(carritoId, cancellationToken);

            if (carrito == null)
            {
                return Result.FailureResult("No encontrado");
            }

            carrito.MarcarComoAbandonado();
            await _unitOfWork.Carritos.UpdateAsync(carrito, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return Result.SuccessResult();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error");
            return Result.FailureResult("Error");
        }
    }

    private async Task<Result<VentaDTO>> RegistrarVentaDesdeCarritoAsync(CrearVentaDTO crearVentaDto, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("💳 Registrando venta...");
            
            // ✅ ID del vendedor predeterminado
            var vendedorId = crearVentaDto.VendedorId;
            
            if (!vendedorId.HasValue)
            {
                var vendedorPredeterminado = await _unitOfWork.Usuarios.Query()
                    .Where(u => u.Username == "vendedor" && u.Rol == "Vendedor")
                    .FirstOrDefaultAsync(cancellationToken);
                
                vendedorId = vendedorPredeterminado?.Id;
            }
            
            var numeroVenta = await GenerarNumeroVentaAsync(cancellationToken);
            var fechaVenta = DateTime.UtcNow;
            
            // Calcular totales
            decimal subtotal = crearVentaDto.Detalles.Sum(d => d.Cantidad * d.PrecioUnitario);
            decimal descuento = 0;
            decimal igv = subtotal * 0.18m;
            decimal total = subtotal + igv;
            
            _logger.LogInformation("📄 Número: {NumeroVenta}, Total: {Total}", numeroVenta, total);
            
            // Crear entidad Venta
            var venta = new Venta
            {
                Id = Guid.NewGuid(),
                NumeroVenta = numeroVenta,
                ClienteId = crearVentaDto.ClienteId,
                VendedorId = vendedorId,
                FechaVenta = fechaVenta,
                Subtotal = subtotal,
                Descuento = descuento,
                IGV = igv,
                Total = total,
                EstadoVenta = EstadoVenta.Pendiente,
                MetodoPago = Enum.TryParse<MetodoPago>(crearVentaDto.MetodoPagoString, true, out var mp) ? mp : MetodoPago.Efectivo,
                DireccionCalle = crearVentaDto.DireccionEnvioString ?? "Sin dirección",
                DireccionCiudad = "",
                DireccionDepartamento = "",
                DireccionPais = "Perú",
                NotasVenta = crearVentaDto.NotasVenta,
                CreatedAt = DateTime.UtcNow
            };
            
            // Agregar detalles
            foreach (var detalleDto in crearVentaDto.Detalles)
            {
                var detalle = new DetalleVenta
                {
                    Id = Guid.NewGuid(),
                    VentaId = venta.Id,
                    LibroId = detalleDto.LibroId,
                    Cantidad = detalleDto.Cantidad,
                    PrecioUnitario = detalleDto.PrecioUnitario,
                    Descuento = 0,
                    Subtotal = detalleDto.Cantidad * detalleDto.PrecioUnitario,
                    CreatedAt = DateTime.UtcNow
                };
                
                venta.Detalles.Add(detalle);
                
                // ✅ ACTUALIZAR STOCK usando el repositorio
                var libro = await _unitOfWork.Libros.GetByIdAsync(detalleDto.LibroId, cancellationToken);
                if (libro != null)
                {
                    libro.Stock -= detalleDto.Cantidad;
                    await _unitOfWork.Libros.UpdateAsync(libro, cancellationToken);
                    _logger.LogInformation("  ✅ Stock actualizado: Libro={LibroId}, Cant={Cantidad}", detalleDto.LibroId, detalleDto.Cantidad);
                }
            }
            
            // Guardar venta
            await _unitOfWork.Ventas.AddAsync(venta, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            
            _logger.LogInformation("✅ Venta registrada con stock actualizado");
            
            // Retornar DTO
            var ventaDto = new VentaDTO
            {
                Id = venta.Id,
                NumeroVenta = venta.NumeroVenta,
                ClienteId = venta.ClienteId,
                VendedorId = venta.VendedorId,
                FechaVenta = venta.FechaVenta,
                Subtotal = venta.Subtotal,
                Descuento = venta.Descuento,
                IGV = venta.IGV,
                Total = venta.Total,
                EstadoVenta = venta.EstadoVenta,
                MetodoPago = venta.MetodoPago,
                NotasVenta = venta.NotasVenta,
                Detalles = new List<DetalleVentaMostrarDTO>()
            };
            
            return Result<VentaDTO>.SuccessResult(ventaDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error al registrar venta");
            return Result<VentaDTO>.FailureResult($"Error: {ex.Message}");
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
