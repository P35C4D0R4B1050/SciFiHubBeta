-- =============================================
-- SciFiHub Database - Procedimientos de Reportes
-- SQL Server 2021+
-- Versión: 1.0
-- =============================================

USE SciFiHubDB;
GO

PRINT N'Iniciando creación de procedimientos almacenados de Reportes...';
GO

-- =============================================
-- C. PROCEDIMIENTOS ALMACENADOS - REPORTES Y ANALYTICS
-- =============================================

-- =============================================
-- SP: sp_Reporte_VentasPorPeriodo
-- Descripción: Reporte detallado de ventas en un período
-- =============================================
CREATE OR ALTER PROCEDURE dbo.sp_Reporte_VentasPorPeriodo
    @FechaInicio DATETIME2(7),
    @FechaFin DATETIME2(7)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        v.NumeroVenta,
        v.FechaVenta,
        c.NombreCompleto AS Cliente,
        vend.NombreCompleto AS Vendedor,
        v.Subtotal,
        v.Descuento,
        v.IGV,
        v.Total,
        v.EstadoVenta,
        v.MetodoPago,
        (SELECT COUNT(*) FROM dbo.DetallesVenta WHERE VentaId = v.Id) AS CantidadItems,
        (SELECT SUM(Cantidad) FROM dbo.DetallesVenta WHERE VentaId = v.Id) AS TotalUnidades
    FROM dbo.Ventas v
    INNER JOIN dbo.Usuarios c ON v.ClienteId = c.Id
    LEFT JOIN dbo.Usuarios vend ON v.VendedorId = vend.Id
    WHERE v.FechaVenta BETWEEN @FechaInicio AND @FechaFin
        AND v.IsDeleted = 0
    ORDER BY v.FechaVenta DESC;
    
    -- Resumen del período
    SELECT 
        COUNT(*) AS TotalVentas,
        SUM(v.Total) AS MontoTotal,
        SUM(v.Subtotal) AS SubtotalTotal,
        SUM(v.IGV) AS IGVTotal,
        SUM(v.Descuento) AS DescuentoTotal,
        AVG(v.Total) AS PromedioVenta,
        MIN(v.Total) AS VentaMinima,
        MAX(v.Total) AS VentaMaxima
    FROM dbo.Ventas v
    WHERE v.FechaVenta BETWEEN @FechaInicio AND @FechaFin
        AND v.IsDeleted = 0
        AND v.EstadoVenta = 'Completada';
END;
GO

PRINT N'SP sp_Reporte_VentasPorPeriodo creado exitosamente.';
GO

-- =============================================
-- SP: sp_Reporte_LibrosMasVendidos
-- Descripción: Top N libros más vendidos en un período
-- =============================================
CREATE OR ALTER PROCEDURE dbo.sp_Reporte_LibrosMasVendidos
    @Top INT = 20,
    @FechaInicio DATETIME2(7) = NULL,
    @FechaFin DATETIME2(7) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Si no se especifican fechas, usar últimos 30 días
    IF @FechaInicio IS NULL
        SET @FechaInicio = DATEADD(DAY, -30, GETUTCDATE());
    
    IF @FechaFin IS NULL
        SET @FechaFin = GETUTCDATE();
    
    SELECT TOP (@Top)
        l.Id,
        l.ISBN,
        l.Titulo,
        l.Autor,
        l.Editorial,
        c.Nombre AS Categoria,
        l.Precio,
        SUM(dv.Cantidad) AS TotalVendido,
        COUNT(DISTINCT v.Id) AS NumeroVentas,
        SUM(dv.Subtotal) AS IngresoTotal,
        l.Stock AS StockActual
    FROM dbo.Libros l
    LEFT JOIN dbo.Categorias c ON l.CategoriaId = c.Id
    INNER JOIN dbo.DetallesVenta dv ON l.Id = dv.LibroId
    INNER JOIN dbo.Ventas v ON dv.VentaId = v.Id
    WHERE v.FechaVenta BETWEEN @FechaInicio AND @FechaFin
        AND v.IsDeleted = 0
        AND dv.IsDeleted = 0
        AND v.EstadoVenta = 'Completada'
    GROUP BY l.Id, l.ISBN, l.Titulo, l.Autor, l.Editorial, c.Nombre, l.Precio, l.Stock
    ORDER BY TotalVendido DESC;
END;
GO

PRINT N'SP sp_Reporte_LibrosMasVendidos creado exitosamente.';
GO

-- =============================================
-- SP: sp_Reporte_ClientesFrecuentes
-- Descripción: Análisis RFM básico de clientes
-- =============================================
CREATE OR ALTER PROCEDURE dbo.sp_Reporte_ClientesFrecuentes
    @Top INT = 50
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT TOP (@Top)
        u.Id,
        u.NombreCompleto,
        u.Email,
        u.Telefono,
        COUNT(v.Id) AS TotalCompras,
        SUM(v.Total) AS MontoTotalGastado,
        AVG(v.Total) AS PromedioCompra,
        MAX(v.FechaVenta) AS UltimaCompra,
        DATEDIFF(DAY, MAX(v.FechaVenta), GETUTCDATE()) AS DiasDesdeUltimaCompra,
        CASE 
            WHEN DATEDIFF(DAY, MAX(v.FechaVenta), GETUTCDATE()) <= 30 THEN 'Activo'
            WHEN DATEDIFF(DAY, MAX(v.FechaVenta), GETUTCDATE()) <= 90 THEN 'Regular'
            ELSE 'Inactivo'
        END AS EstadoCliente
    FROM dbo.Usuarios u
    INNER JOIN dbo.Ventas v ON u.Id = v.ClienteId
    WHERE u.Rol = 'Cliente'
        AND u.IsDeleted = 0
        AND v.IsDeleted = 0
        AND v.EstadoVenta = 'Completada'
    GROUP BY u.Id, u.NombreCompleto, u.Email, u.Telefono
    ORDER BY MontoTotalGastado DESC;
END;
GO

PRINT N'SP sp_Reporte_ClientesFrecuentes creado exitosamente.';
GO

-- =============================================
-- SP: sp_Reporte_StockBajo
-- Descripción: Alerta de libros con stock bajo
-- =============================================
CREATE OR ALTER PROCEDURE dbo.sp_Reporte_StockBajo
    @UmbralStock INT = 10
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        l.Id,
        l.ISBN,
        l.Titulo,
        l.Autor,
        l.Editorial,
        c.Nombre AS Categoria,
        l.Stock,
        l.Precio,
        dbo.fn_ObtenerEstadoStock(l.Stock) AS EstadoStock,
        -- Calcular ventas de últimos 30 días para proyección
        ISNULL((
            SELECT SUM(dv.Cantidad)
            FROM dbo.DetallesVenta dv
            INNER JOIN dbo.Ventas v ON dv.VentaId = v.Id
            WHERE dv.LibroId = l.Id
                AND v.FechaVenta >= DATEADD(DAY, -30, GETUTCDATE())
                AND v.EstadoVenta = 'Completada'
        ), 0) AS VentasUltimos30Dias,
        -- Días aproximados de stock restante
        CASE 
            WHEN ISNULL((
                SELECT SUM(dv.Cantidad)
                FROM dbo.DetallesVenta dv
                INNER JOIN dbo.Ventas v ON dv.VentaId = v.Id
                WHERE dv.LibroId = l.Id
                    AND v.FechaVenta >= DATEADD(DAY, -30, GETUTCDATE())
                    AND v.EstadoVenta = 'Completada'
            ), 0) > 0 THEN 
                CAST((l.Stock * 30.0 / ISNULL((
                    SELECT SUM(dv.Cantidad)
                    FROM dbo.DetallesVenta dv
                    INNER JOIN dbo.Ventas v ON dv.VentaId = v.Id
                    WHERE dv.LibroId = l.Id
                        AND v.FechaVenta >= DATEADD(DAY, -30, GETUTCDATE())
                        AND v.EstadoVenta = 'Completada'
                ), 1)) AS INT)
            ELSE NULL
        END AS DiasStockEstimado
    FROM dbo.Libros l
    LEFT JOIN dbo.Categorias c ON l.CategoriaId = c.Id
    WHERE l.Stock <= @UmbralStock
        AND l.IsDeleted = 0
        AND l.Estado = 'Disponible'
    ORDER BY l.Stock ASC, DiasStockEstimado ASC;
END;
GO

PRINT N'SP sp_Reporte_StockBajo creado exitosamente.';
GO

-- =============================================
-- SP: sp_Reporte_VentasPorCategoria
-- Descripción: Análisis de ventas por categoría de libros
-- =============================================
CREATE OR ALTER PROCEDURE dbo.sp_Reporte_VentasPorCategoria
    @FechaInicio DATETIME2(7) = NULL,
    @FechaFin DATETIME2(7) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    IF @FechaInicio IS NULL
        SET @FechaInicio = DATEADD(DAY, -30, GETUTCDATE());
    
    IF @FechaFin IS NULL
        SET @FechaFin = GETUTCDATE();
    
    SELECT 
        c.Id AS CategoriaId,
        c.Nombre AS Categoria,
        COUNT(DISTINCT l.Id) AS TotalLibrosVendidos,
        SUM(dv.Cantidad) AS UnidadesVendidas,
        SUM(dv.Subtotal) AS IngresoTotal,
        AVG(dv.PrecioUnitario) AS PrecioPromedio,
        COUNT(DISTINCT v.Id) AS NumeroVentas
    FROM dbo.Categorias c
    INNER JOIN dbo.Libros l ON c.Id = l.CategoriaId
    INNER JOIN dbo.DetallesVenta dv ON l.Id = dv.LibroId
    INNER JOIN dbo.Ventas v ON dv.VentaId = v.Id
    WHERE v.FechaVenta BETWEEN @FechaInicio AND @FechaFin
        AND v.EstadoVenta = 'Completada'
        AND v.IsDeleted = 0
        AND dv.IsDeleted = 0
    GROUP BY c.Id, c.Nombre
    ORDER BY IngresoTotal DESC;
END;
GO

PRINT N'SP sp_Reporte_VentasPorCategoria creado exitosamente.';
GO

-- =============================================
-- SP: sp_Reporte_VentasPorMetodoPago
-- Descripción: Análisis de ventas por método de pago
-- =============================================
CREATE OR ALTER PROCEDURE dbo.sp_Reporte_VentasPorMetodoPago
    @FechaInicio DATETIME2(7) = NULL,
    @FechaFin DATETIME2(7) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    IF @FechaInicio IS NULL
        SET @FechaInicio = DATEADD(MONTH, -1, GETUTCDATE());
    
    IF @FechaFin IS NULL
        SET @FechaFin = GETUTCDATE();
    
    SELECT 
        MetodoPago,
        COUNT(*) AS NumeroVentas,
        SUM(Total) AS MontoTotal,
        AVG(Total) AS PromedioVenta,
        CAST(COUNT(*) * 100.0 / (SELECT COUNT(*) FROM dbo.Ventas 
            WHERE FechaVenta BETWEEN @FechaInicio AND @FechaFin 
            AND EstadoVenta = 'Completada' AND IsDeleted = 0) AS DECIMAL(5,2)) AS PorcentajeVentas
    FROM dbo.Ventas
    WHERE FechaVenta BETWEEN @FechaInicio AND @FechaFin
        AND EstadoVenta = 'Completada'
        AND IsDeleted = 0
    GROUP BY MetodoPago
    ORDER BY MontoTotal DESC;
END;
GO

PRINT N'SP sp_Reporte_VentasPorMetodoPago creado exitosamente.';
GO

-- =============================================
-- SP: sp_Reporte_RendimientoVendedores
-- Descripción: Análisis de rendimiento de vendedores
-- =============================================
CREATE OR ALTER PROCEDURE dbo.sp_Reporte_RendimientoVendedores
    @FechaInicio DATETIME2(7) = NULL,
    @FechaFin DATETIME2(7) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    IF @FechaInicio IS NULL
        SET @FechaInicio = DATEADD(MONTH, -1, GETUTCDATE());
    
    IF @FechaFin IS NULL
        SET @FechaFin = GETUTCDATE();
    
    SELECT 
        u.Id AS VendedorId,
        u.NombreCompleto AS Vendedor,
        COUNT(v.Id) AS TotalVentas,
        SUM(v.Total) AS MontoTotalVendido,
        AVG(v.Total) AS PromedioVenta,
        SUM(dv.Cantidad) AS TotalUnidadesVendidas,
        MIN(v.FechaVenta) AS PrimeraVenta,
        MAX(v.FechaVenta) AS UltimaVenta
    FROM dbo.Usuarios u
    LEFT JOIN dbo.Ventas v ON u.Id = v.VendedorId
    LEFT JOIN (
        SELECT VentaId, SUM(Cantidad) AS Cantidad
        FROM dbo.DetallesVenta
        WHERE IsDeleted = 0
        GROUP BY VentaId
    ) dv ON v.Id = dv.VentaId
    WHERE u.Rol = 'Vendedor'
        AND u.IsDeleted = 0
        AND (v.FechaVenta IS NULL OR v.FechaVenta BETWEEN @FechaInicio AND @FechaFin)
        AND (v.IsDeleted = 0 OR v.IsDeleted IS NULL)
        AND (v.EstadoVenta = 'Completada' OR v.EstadoVenta IS NULL)
    GROUP BY u.Id, u.NombreCompleto
    ORDER BY MontoTotalVendido DESC;
END;
GO

PRINT N'SP sp_Reporte_RendimientoVendedores creado exitosamente.';
GO

-- =============================================
-- SP: sp_Dashboard_EstadisticasGenerales
-- Descripción: Estadísticas generales para dashboard administrativo
-- =============================================
CREATE OR ALTER PROCEDURE dbo.sp_Dashboard_EstadisticasGenerales
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Hoy DATETIME2(7) = CAST(GETUTCDATE() AS DATE);
    DECLARE @InicioSemana DATETIME2(7) = DATEADD(DAY, 1-DATEPART(WEEKDAY, @Hoy), @Hoy);
    DECLARE @InicioMes DATETIME2(7) = DATEFROMPARTS(YEAR(@Hoy), MONTH(@Hoy), 1);
    
    -- Ventas del día, semana y mes
    SELECT 
        'VentasPeriodo' AS TipoResultado,
        ISNULL(SUM(CASE WHEN v.FechaVenta >= @Hoy THEN v.Total ELSE 0 END), 0) AS VentasHoy,
        ISNULL(SUM(CASE WHEN v.FechaVenta >= @InicioSemana THEN v.Total ELSE 0 END), 0) AS VentasSemana,
        ISNULL(SUM(CASE WHEN v.FechaVenta >= @InicioMes THEN v.Total ELSE 0 END), 0) AS VentasMes,
        ISNULL(COUNT(CASE WHEN v.FechaVenta >= @Hoy THEN 1 END), 0) AS NumeroVentasHoy,
        ISNULL(COUNT(CASE WHEN v.FechaVenta >= @InicioSemana THEN 1 END), 0) AS NumeroVentasSemana,
        ISNULL(COUNT(CASE WHEN v.FechaVenta >= @InicioMes THEN 1 END), 0) AS NumeroVentasMes
    FROM dbo.Ventas v
    WHERE v.EstadoVenta = 'Completada' AND v.IsDeleted = 0;
    
    -- Estadísticas de inventario
    SELECT 
        'EstadisticasInventario' AS TipoResultado,
        COUNT(*) AS TotalLibros,
        SUM(Stock) AS TotalUnidadesStock,
        SUM(CASE WHEN Stock = 0 THEN 1 ELSE 0 END) AS LibrosAgotados,
        SUM(CASE WHEN Stock <= 5 AND Stock > 0 THEN 1 ELSE 0 END) AS LibrosStockBajo,
        SUM(Precio * Stock) AS ValorInventario
    FROM dbo.Libros
    WHERE IsDeleted = 0;
    
    -- Estadísticas de clientes
    SELECT 
        'EstadisticasClientes' AS TipoResultado,
        COUNT(DISTINCT CASE WHEN u.Rol = 'Cliente' THEN u.Id END) AS TotalClientes,
        COUNT(DISTINCT CASE WHEN u.Rol = 'Cliente' AND u.Estado = 'Activo' THEN u.Id END) AS ClientesActivos,
        COUNT(DISTINCT CASE WHEN v.FechaVenta >= DATEADD(DAY, -30, @Hoy) THEN v.ClienteId END) AS ClientesActivosUltimos30Dias
    FROM dbo.Usuarios u
    LEFT JOIN dbo.Ventas v ON u.Id = v.ClienteId AND v.IsDeleted = 0
    WHERE u.IsDeleted = 0;
    
    -- Top 5 libros más vendidos del mes
    SELECT TOP 5
        'TopLibrosMes' AS TipoResultado,
        l.Titulo,
        l.Autor,
        SUM(dv.Cantidad) AS UnidadesVendidas
    FROM dbo.Libros l
    INNER JOIN dbo.DetallesVenta dv ON l.Id = dv.LibroId
    INNER JOIN dbo.Ventas v ON dv.VentaId = v.Id
    WHERE v.FechaVenta >= @InicioMes
        AND v.EstadoVenta = 'Completada'
        AND v.IsDeleted = 0
        AND dv.IsDeleted = 0
    GROUP BY l.Titulo, l.Autor
    ORDER BY UnidadesVendidas DESC;
END;
GO

PRINT N'SP sp_Dashboard_EstadisticasGenerales creado exitosamente.';
GO

-- =============================================
-- SP: sp_Reporte_TendenciaVentas
-- Descripción: Tendencia de ventas mensual/trimestral
-- =============================================
CREATE OR ALTER PROCEDURE dbo.sp_Reporte_TendenciaVentas
    @NumeroMeses INT = 12
AS
BEGIN
    SET NOCOUNT ON;
    
    WITH Meses AS (
        SELECT 
            DATEADD(MONTH, -n, DATEFROMPARTS(YEAR(GETUTCDATE()), MONTH(GETUTCDATE()), 1)) AS FechaMes,
            n
        FROM (
            SELECT ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) - 1 AS n
            FROM sys.objects
        ) t
        WHERE n < @NumeroMeses
    )
    SELECT 
        m.FechaMes,
        DATENAME(MONTH, m.FechaMes) AS NombreMes,
        YEAR(m.FechaMes) AS Anio,
        MONTH(m.FechaMes) AS Mes,
        ISNULL(COUNT(v.Id), 0) AS NumeroVentas,
        ISNULL(SUM(v.Total), 0) AS MontoTotal,
        ISNULL(AVG(v.Total), 0) AS PromedioVenta,
        ISNULL(SUM(dv.TotalUnidades), 0) AS UnidadesVendidas
    FROM Meses m
    LEFT JOIN dbo.Ventas v ON 
        YEAR(v.FechaVenta) = YEAR(m.FechaMes) AND 
        MONTH(v.FechaVenta) = MONTH(m.FechaMes) AND
        v.EstadoVenta = 'Completada' AND
        v.IsDeleted = 0
    LEFT JOIN (
        SELECT VentaId, SUM(Cantidad) AS TotalUnidades
        FROM dbo.DetallesVenta
        WHERE IsDeleted = 0
        GROUP BY VentaId
    ) dv ON v.Id = dv.VentaId
    GROUP BY m.FechaMes
    ORDER BY m.FechaMes DESC;
END;
GO

PRINT N'SP sp_Reporte_TendenciaVentas creado exitosamente.';
GO

PRINT N'Todos los procedimientos de Reportes creados exitosamente.';
GO
