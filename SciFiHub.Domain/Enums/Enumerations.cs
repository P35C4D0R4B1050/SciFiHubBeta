namespace SciFiHub.Domain.Enums;

/// <summary>
/// Roles de usuario en el sistema
/// </summary>
public enum RolUsuario
{
    Administrador,
    Vendedor,
    Cliente
}

/// <summary>
/// Estados de usuario
/// </summary>
public enum EstadoUsuario
{
    Activo,
    Inactivo
}

/// <summary>
/// Estados de venta
/// </summary>
public enum EstadoVenta
{
    Pendiente,
    Procesando,
    Completada,
    Cancelada,
    Reembolsada
}

/// <summary>
/// Métodos de pago
/// </summary>
public enum MetodoPago
{
    Efectivo,
    TarjetaCredito,
    TarjetaDebito,
    Yape,
    Plin,
    Transferencia
}

/// <summary>
/// Estados de libro
/// </summary>
public enum EstadoLibro
{
    Disponible,
    Agotado,
    Descontinuado
}

/// <summary>
/// Estados de carrito
/// </summary>
public enum EstadoCarrito
{
    Activo,
    Convertido,
    Abandonado
}

/// <summary>
/// Tipos de movimiento de inventario
/// </summary>
public enum TipoMovimientoInventario
{
    Ingreso,
    Venta,
    Ajuste,
    Devolucion,
    Anulacion
}

/// <summary>
/// Estados de categoría
/// </summary>
public enum EstadoCategoria
{
    Activo,
    Inactivo
}
