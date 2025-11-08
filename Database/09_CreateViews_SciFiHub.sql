-- =============================================
-- SciFiHub Database - Vistas Optimizadas
-- SQL Server 2021+
-- Versión: 1.0
-- =============================================

USE SciFiHubDB;
GO

PRINT N'Iniciando creación de vistas...';
GO

-- =============================================
-- F. VISTAS OPTIMIZADAS PARA CONSULTAS FRECUENTES
-- =============================================

-- =============================================
-- Vista: vw_InventarioCompleto
-- Descripción: Inventario completo con información agregada
-- =============================================
CREATE OR ALTER VIEW dbo.vw_InventarioCompleto
AS
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
    dbo.fn_ObtenerEstadoStock(l.Stock) AS EstadoStock,
    l.Destacado,
    l.FechaIngreso,
    ISNULL((
        SELECT SUM(dv.Cantidad)
        FROM dbo.DetallesVenta dv
        INNER JOIN dbo.Ventas v ON dv.VentaId = v.Id
        WHERE dv.LibroId = l.Id 
            AND v.EstadoVenta = 'Completada'
            AND dv.IsDeleted = 0
            AND v.IsDeleted = 0
    ), 0) AS TotalVendido,
    (
        SELECT MAX(v.FechaVenta)
        FROM dbo.DetallesVenta dv
        INNER JOIN dbo.Ventas v ON dv.VentaId = v.Id
        WHERE dv.LibroId = l.Id 
            AND v.EstadoVenta = 'Completada'
            AND dv.IsDeleted = 0
            AND v.IsDeleted = 0
    ) AS UltimaVenta,
    l.ImagenPortada
FROM dbo.Libros l
LEFT JOIN dbo.Categorias c ON l.CategoriaId = c.Id
WHERE l.IsDeleted = 0;
GO

PRINT N'Vista vw_InventarioCompleto creada exitosamente.';
GO

-- =============================================
-- Vista: vw_VentasDetalladas
-- Descripción: Ventas con detalles completos para reportes
-- =============================================
CREATE OR ALTER VIEW dbo.vw_VentasDetalladas
AS
SELECT 
    v.Id AS VentaId,
    v.NumeroVenta,
    v.FechaVenta,
    c.Id AS ClienteId,
    c.NombreCompleto AS ClienteNombre,
    c.Email AS ClienteEmail,
    vend.Id AS VendedorId,
    vend.NombreCompleto AS VendedorNombre,
    dv.LibroId,
    l.Titulo AS LibroTitulo,
    l.Autor AS LibroAutor,
    l.ISBN,
    cat.Nombre AS Categoria,
    dv.Cantidad,
    dv.PrecioUnitario,
    dv.Descuento AS DescuentoDetalle,
    dv.Subtotal AS SubtotalDetalle,
    v.Subtotal AS SubtotalVenta,
    v.Descuento AS DescuentoVenta,
    v.IGV,
    v.Total,
    v.EstadoVenta,
    v.MetodoPago
FROM dbo.Ventas v
INNER JOIN dbo.Usuarios c ON v.ClienteId = c.Id
LEFT JOIN dbo.Usuarios vend ON v.VendedorId = vend.Id
INNER JOIN dbo.DetallesVenta dv ON v.Id = dv.VentaId
INNER JOIN dbo.Libros l ON dv.LibroId = l.Id
LEFT JOIN dbo.Categorias cat ON l.CategoriaId = cat.Id
WHERE v.IsDeleted = 0 AND dv.IsDeleted = 0;
GO

PRINT N'Vista vw_VentasDetalladas creada exitosamente.';
GO

-- =============================================
-- Vista: vw_ClientesActivos
-- Descripción: Clientes con compras recientes
-- =============================================
CREATE OR ALTER VIEW dbo.vw_ClientesActivos
AS
SELECT 
    u.Id,
    u.NombreCompleto,
    u.Email,
    u.Username,
    u.Telefono,
    u.FechaRegistro,
    COUNT(v.Id) AS TotalCompras,
    SUM(v.Total) AS MontoTotalGastado,
    MAX(v.FechaVenta) AS UltimaCompra,
    DATEDIFF(DAY, MAX(v.FechaVenta), GETUTCDATE()) AS DiasDesdeUltimaCompra
FROM dbo.Usuarios u
INNER JOIN dbo.Ventas v ON u.Id = v.ClienteId
WHERE u.Rol = 'Cliente'
    AND u.IsDeleted = 0
    AND v.IsDeleted = 0
    AND v.EstadoVenta = 'Completada'
    AND v.FechaVenta >= DATEADD(MONTH, -6, GETUTCDATE())
GROUP BY u.Id, u.NombreCompleto, u.Email, u.Username, u.Telefono, u.FechaRegistro;
GO

PRINT N'Vista vw_ClientesActivos creada exitosamente.';
GO

-- =============================================
-- Vista: vw_DashboardMetricas
-- Descripción: Métricas pre-calculadas para dashboard
-- =============================================
CREATE OR ALTER VIEW dbo.vw_DashboardMetricas
AS
SELECT 
    (SELECT COUNT(*) FROM dbo.Libros WHERE IsDeleted = 0) AS TotalLibros,
    (SELECT COUNT(*) FROM dbo.Libros WHERE IsDeleted = 0 AND Stock > 0) AS LibrosDisponibles,
    (SELECT COUNT(*) FROM dbo.Libros WHERE IsDeleted = 0 AND Stock = 0) AS LibrosAgotados,
    (SELECT COUNT(*) FROM dbo.Libros WHERE IsDeleted = 0 AND Stock <= 5 AND Stock > 0) AS LibrosStockBajo,
    (SELECT COUNT(*) FROM dbo.Usuarios WHERE IsDeleted = 0 AND Rol = 'Cliente') AS TotalClientes,
    (SELECT COUNT(*) FROM dbo.Usuarios WHERE IsDeleted = 0 AND Rol = 'Vendedor') AS TotalVendedores,
    (SELECT COUNT(*) FROM dbo.Ventas WHERE IsDeleted = 0 AND CAST(FechaVenta AS DATE) = CAST(GETUTCDATE() AS DATE)) AS VentasHoy,
    (SELECT ISNULL(SUM(Total), 0) FROM dbo.Ventas WHERE IsDeleted = 0 AND CAST(FechaVenta AS DATE) = CAST(GETUTCDATE() AS DATE) AND EstadoVenta = 'Completada') AS MontoVentasHoy,
    (SELECT COUNT(*) FROM dbo.Ventas WHERE IsDeleted = 0 AND FechaVenta >= DATEADD(DAY, -7, GETUTCDATE())) AS VentasSemana,
    (SELECT ISNULL(SUM(Total), 0) FROM dbo.Ventas WHERE IsDeleted = 0 AND FechaVenta >= DATEADD(DAY, -7, GETUTCDATE()) AND EstadoVenta = 'Completada') AS MontoVentasSemana,
    (SELECT COUNT(*) FROM dbo.Ventas WHERE IsDeleted = 0 AND MONTH(FechaVenta) = MONTH(GETUTCDATE()) AND YEAR(FechaVenta) = YEAR(GETUTCDATE())) AS VentasMes,
    (SELECT ISNULL(SUM(Total), 0) FROM dbo.Ventas WHERE IsDeleted = 0 AND MONTH(FechaVenta) = MONTH(GETUTCDATE()) AND YEAR(FechaVenta) = YEAR(GETUTCDATE()) AND EstadoVenta = 'Completada') AS MontoVentasMes;
GO

PRINT N'Vista vw_DashboardMetricas creada exitosamente.';
GO

-- =============================================
-- Vista: vw_CatalogoPublico
-- Descripción: Catálogo optimizado para clientes
-- =============================================
CREATE OR ALTER VIEW dbo.vw_CatalogoPublico
AS
SELECT 
    l.Id,
    l.ISBN,
    l.Titulo,
    l.Autor,
    l.Editorial,
    c.Nombre AS Categoria,
    l.Precio,
    l.PrecioOferta,
    ISNULL(l.PrecioOferta, l.Precio) AS PrecioFinal,
    CASE WHEN l.PrecioOferta IS NOT NULL THEN 
        CAST(((l.Precio - l.PrecioOferta) / l.Precio * 100) AS INT)
    ELSE 0 END AS PorcentajeDescuento,
    l.Stock,
    CASE 
        WHEN l.Stock > 0 THEN 1
        ELSE 0
    END AS Disponible,
    l.ImagenPortada,
    l.Sinopsis,
    l.AnioPublicacion,
    l.NumeroPaginas,
    l.Idioma,
    l.Destacado,
    l.FechaIngreso
FROM dbo.Libros l
LEFT JOIN dbo.Categorias c ON l.CategoriaId = c.Id
WHERE l.IsDeleted = 0 
    AND l.Estado = 'Disponible';
GO

PRINT N'Vista vw_CatalogoPublico creada exitosamente.';
GO

-- =============================================
-- Vista: vw_LibrosDestacados
-- Descripción: Libros destacados con información completa
-- =============================================
CREATE OR ALTER VIEW dbo.vw_LibrosDestacados
AS
SELECT 
    l.Id,
    l.ISBN,
    l.Titulo,
    l.Autor,
    l.Editorial,
    c.Nombre AS Categoria,
    l.Precio,
    l.PrecioOferta,
    ISNULL(l.PrecioOferta, l.Precio) AS PrecioFinal,
    l.Stock,
    l.ImagenPortada,
    l.Sinopsis,
    l.Descripcion,
    l.AnioPublicacion,
    l.FechaIngreso,
    ISNULL((
        SELECT SUM(dv.Cantidad)
        FROM dbo.DetallesVenta dv
        INNER JOIN dbo.Ventas v ON dv.VentaId = v.Id
        WHERE dv.LibroId = l.Id 
            AND v.EstadoVenta = 'Completada'
            AND v.FechaVenta >= DATEADD(MONTH, -3, GETUTCDATE())
    ), 0) AS VentasRecientes
FROM dbo.Libros l
LEFT JOIN dbo.Categorias c ON l.CategoriaId = c.Id
WHERE l.IsDeleted = 0 
    AND l.Destacado = 1
    AND l.Estado = 'Disponible';
GO

PRINT N'Vista vw_LibrosDestacados creada exitosamente.';
GO

-- =============================================
-- Vista: vw_HistorialComprasCliente
-- Descripción: Historial de compras por cliente
-- =============================================
CREATE OR ALTER VIEW dbo.vw_HistorialComprasCliente
AS
SELECT 
    v.ClienteId,
    v.Id AS VentaId,
    v.NumeroVenta,
    v.FechaVenta,
    v.Total,
    v.EstadoVenta,
    v.MetodoPago,
    dv.LibroId,
    l.Titulo AS LibroTitulo,
    l.Autor AS LibroAutor,
    l.ImagenPortada,
    dv.Cantidad,
    dv.PrecioUnitario,
    dv.Subtotal
FROM dbo.Ventas v
INNER JOIN dbo.DetallesVenta dv ON v.Id = dv.VentaId
INNER JOIN dbo.Libros l ON dv.LibroId = l.Id
WHERE v.IsDeleted = 0 AND dv.IsDeleted = 0;
GO

PRINT N'Vista vw_HistorialComprasCliente creada exitosamente.';
GO

PRINT N'Todas las vistas creadas exitosamente.';
GO
