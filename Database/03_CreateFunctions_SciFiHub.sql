-- =============================================
-- SciFiHub Database - Funciones Escalares y TVF
-- SQL Server 2021+
-- Versión: 1.0
-- =============================================

USE SciFiHubDB;
GO

PRINT N'Iniciando creación de funciones...';
GO

-- =============================================
-- D. FUNCIONES ESCALARES Y DE TABLA (TVF)
-- =============================================

-- =============================================
-- Función: fn_CalcularTotalVenta
-- Descripción: Calcula el total de una venta con IGV y descuentos
-- Parámetros: @Subtotal, @Descuento, @AplicarIGV (18%)
-- =============================================
CREATE OR ALTER FUNCTION dbo.fn_CalcularTotalVenta
(
    @Subtotal DECIMAL(18, 2),
    @Descuento DECIMAL(18, 2),
    @AplicarIGV BIT = 1
)
RETURNS DECIMAL(18, 2)
AS
BEGIN
    DECLARE @SubtotalConDescuento DECIMAL(18, 2);
    DECLARE @IGV DECIMAL(18, 2);
    DECLARE @Total DECIMAL(18, 2);
    
    SET @SubtotalConDescuento = @Subtotal - @Descuento;
    
    IF @AplicarIGV = 1
        SET @IGV = @SubtotalConDescuento * 0.18;
    ELSE
        SET @IGV = 0;
    
    SET @Total = @SubtotalConDescuento + @IGV;
    
    RETURN ISNULL(@Total, 0);
END;
GO

PRINT N'Función fn_CalcularTotalVenta creada exitosamente.';
GO

-- =============================================
-- Función: fn_VerificarStockDisponible
-- Descripción: Verifica si hay stock suficiente de un libro
-- =============================================
CREATE OR ALTER FUNCTION dbo.fn_VerificarStockDisponible
(
    @LibroId UNIQUEIDENTIFIER,
    @CantidadRequerida INT
)
RETURNS BIT
AS
BEGIN
    DECLARE @StockActual INT;
    DECLARE @Resultado BIT = 0;
    
    SELECT @StockActual = Stock
    FROM dbo.Libros
    WHERE Id = @LibroId AND IsDeleted = 0;
    
    IF @StockActual >= @CantidadRequerida
        SET @Resultado = 1;
    
    RETURN ISNULL(@Resultado, 0);
END;
GO

PRINT N'Función fn_VerificarStockDisponible creada exitosamente.';
GO

-- =============================================
-- Función: fn_ObtenerNumeroVentaSecuencial
-- Descripción: Genera número de venta con formato VYYYYMM-0000001
-- =============================================
CREATE OR ALTER FUNCTION dbo.fn_ObtenerNumeroVentaSecuencial()
RETURNS NVARCHAR(50)
AS
BEGIN
    DECLARE @NumeroVenta NVARCHAR(50);
    DECLARE @Anio NVARCHAR(4) = CAST(YEAR(GETDATE()) AS NVARCHAR(4));
    DECLARE @Mes NVARCHAR(2) = RIGHT('0' + CAST(MONTH(GETDATE()) AS NVARCHAR(2)), 2);
    DECLARE @Prefijo NVARCHAR(10) = 'V' + @Anio + @Mes + '-';
    DECLARE @UltimoNumero INT = 0;
    
    -- Obtener último número del mes
    SELECT TOP 1 @UltimoNumero = CAST(RIGHT(NumeroVenta, 7) AS INT)
    FROM dbo.Ventas
    WHERE NumeroVenta LIKE @Prefijo + '%'
    ORDER BY NumeroVenta DESC;
    
    -- Incrementar y formatear
    SET @NumeroVenta = @Prefijo + RIGHT('0000000' + CAST((@UltimoNumero + 1) AS NVARCHAR(7)), 7);
    
    RETURN @NumeroVenta;
END;
GO

PRINT N'Función fn_ObtenerNumeroVentaSecuencial creada exitosamente.';
GO

-- =============================================
-- Función: fn_CalcularDescuento
-- Descripción: Calcula descuento según reglas de negocio
-- =============================================
CREATE OR ALTER FUNCTION dbo.fn_CalcularDescuento
(
    @Subtotal DECIMAL(18, 2),
    @TipoCliente NVARCHAR(20)
)
RETURNS DECIMAL(18, 2)
AS
BEGIN
    DECLARE @Descuento DECIMAL(18, 2) = 0;
    
    -- Reglas de descuento por volumen
    IF @Subtotal >= 500
        SET @Descuento = @Subtotal * 0.10; -- 10% descuento
    ELSE IF @Subtotal >= 300
        SET @Descuento = @Subtotal * 0.05; -- 5% descuento
    
    -- Descuento adicional para clientes frecuentes
    IF @TipoCliente = 'Frecuente'
        SET @Descuento = @Descuento + (@Subtotal * 0.03); -- 3% adicional
    
    RETURN ISNULL(@Descuento, 0);
END;
GO

PRINT N'Función fn_CalcularDescuento creada exitosamente.';
GO

-- =============================================
-- Función: fn_ObtenerEstadoStock
-- Descripción: Retorna estado del stock según umbrales
-- =============================================
CREATE OR ALTER FUNCTION dbo.fn_ObtenerEstadoStock
(
    @Stock INT
)
RETURNS NVARCHAR(20)
AS
BEGIN
    DECLARE @Estado NVARCHAR(20);
    
    IF @Stock = 0
        SET @Estado = 'Agotado';
    ELSE IF @Stock <= 5
        SET @Estado = 'Bajo';
    ELSE IF @Stock <= 20
        SET @Estado = 'Medio';
    ELSE
        SET @Estado = 'Disponible';
    
    RETURN @Estado;
END;
GO

PRINT N'Función fn_ObtenerEstadoStock creada exitosamente.';
GO

-- =============================================
-- Función TVF: fn_LibrosPorGenero
-- Descripción: Retorna libros de una categoría específica
-- =============================================
CREATE OR ALTER FUNCTION dbo.fn_LibrosPorGenero
(
    @CategoriaId UNIQUEIDENTIFIER
)
RETURNS TABLE
AS
RETURN
(
    SELECT 
        l.Id,
        l.ISBN,
        l.Titulo,
        l.Autor,
        l.Editorial,
        c.Nombre AS Categoria,
        l.Precio,
        l.PrecioOferta,
        l.Stock,
        l.ImagenPortada,
        l.Destacado,
        dbo.fn_ObtenerEstadoStock(l.Stock) AS EstadoStock
    FROM dbo.Libros l
    INNER JOIN dbo.Categorias c ON l.CategoriaId = c.Id
    WHERE l.CategoriaId = @CategoriaId
        AND l.IsDeleted = 0
        AND l.Estado = 'Disponible'
);
GO

PRINT N'Función fn_LibrosPorGenero creada exitosamente.';
GO

-- =============================================
-- Función TVF: fn_VentasDelMes
-- Descripción: Retorna ventas de un mes específico
-- =============================================
CREATE OR ALTER FUNCTION dbo.fn_VentasDelMes
(
    @Mes INT,
    @Anio INT
)
RETURNS TABLE
AS
RETURN
(
    SELECT 
        v.Id,
        v.NumeroVenta,
        v.ClienteId,
        u.NombreCompleto AS Cliente,
        v.VendedorId,
        vend.NombreCompleto AS Vendedor,
        v.FechaVenta,
        v.Subtotal,
        v.Descuento,
        v.IGV,
        v.Total,
        v.EstadoVenta,
        v.MetodoPago
    FROM dbo.Ventas v
    INNER JOIN dbo.Usuarios u ON v.ClienteId = u.Id
    LEFT JOIN dbo.Usuarios vend ON v.VendedorId = vend.Id
    WHERE MONTH(v.FechaVenta) = @Mes
        AND YEAR(v.FechaVenta) = @Anio
        AND v.IsDeleted = 0
);
GO

PRINT N'Función fn_VentasDelMes creada exitosamente.';
GO

-- =============================================
-- Función TVF: fn_ClientesActivosEnPeriodo
-- Descripción: Clientes con compras en un período
-- =============================================
CREATE OR ALTER FUNCTION dbo.fn_ClientesActivosEnPeriodo
(
    @FechaInicio DATETIME2(7),
    @FechaFin DATETIME2(7)
)
RETURNS TABLE
AS
RETURN
(
    SELECT 
        u.Id,
        u.NombreCompleto,
        u.Email,
        u.Telefono,
        COUNT(v.Id) AS TotalCompras,
        SUM(v.Total) AS TotalGastado,
        MAX(v.FechaVenta) AS UltimaCompra
    FROM dbo.Usuarios u
    INNER JOIN dbo.Ventas v ON u.Id = v.ClienteId
    WHERE u.Rol = 'Cliente'
        AND u.IsDeleted = 0
        AND v.IsDeleted = 0
        AND v.FechaVenta BETWEEN @FechaInicio AND @FechaFin
        AND v.EstadoVenta = 'Completada'
    GROUP BY u.Id, u.NombreCompleto, u.Email, u.Telefono
);
GO

PRINT N'Función fn_ClientesActivosEnPeriodo creada exitosamente.';
GO

-- =============================================
-- Función TVF: fn_HistorialPreciosLibro
-- Descripción: Historial de precios de un libro (simulado con auditoría)
-- =============================================
CREATE OR ALTER FUNCTION dbo.fn_HistorialPreciosLibro
(
    @LibroId UNIQUEIDENTIFIER
)
RETURNS TABLE
AS
RETURN
(
    SELECT 
        l.Id AS LibroId,
        l.Titulo,
        l.Precio AS PrecioActual,
        l.PrecioOferta,
        l.UpdatedAt AS FechaActualizacion,
        'Actual' AS TipoPrecio
    FROM dbo.Libros l
    WHERE l.Id = @LibroId AND l.IsDeleted = 0
);
GO

PRINT N'Función fn_HistorialPreciosLibro creada exitosamente.';
GO

-- =============================================
-- Función TVF: fn_DetallesVentaFormateados
-- Descripción: Detalles de venta con información completa del libro
-- =============================================
CREATE OR ALTER FUNCTION dbo.fn_DetallesVentaFormateados
(
    @VentaId UNIQUEIDENTIFIER
)
RETURNS TABLE
AS
RETURN
(
    SELECT 
        dv.Id,
        dv.VentaId,
        l.Id AS LibroId,
        l.Titulo,
        l.Autor,
        l.ISBN,
        dv.Cantidad,
        dv.PrecioUnitario,
        dv.Descuento,
        dv.Subtotal,
        (dv.Cantidad * dv.PrecioUnitario) AS SubtotalSinDescuento,
        l.ImagenPortada
    FROM dbo.DetallesVenta dv
    INNER JOIN dbo.Libros l ON dv.LibroId = l.Id
    WHERE dv.VentaId = @VentaId AND dv.IsDeleted = 0
);
GO

PRINT N'Función fn_DetallesVentaFormateados creada exitosamente.';
GO

PRINT N'Todas las funciones creadas exitosamente.';
GO
