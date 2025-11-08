-- =============================================
-- SciFiHub Database - Procedimientos de Libros
-- SQL Server 2021+
-- Versión: 1.0
-- =============================================

USE SciFiHubDB;
GO

PRINT N'Iniciando creación de procedimientos almacenados de Libros...';
GO

-- =============================================
-- C. PROCEDIMIENTOS ALMACENADOS - GESTIÓN DE LIBROS
-- =============================================

-- =============================================
-- SP: sp_Libro_Insertar
-- Descripción: Inserta un nuevo libro en el catálogo
-- =============================================
CREATE OR ALTER PROCEDURE dbo.sp_Libro_Insertar
    @ISBN NVARCHAR(20),
    @Titulo NVARCHAR(300),
    @Autor NVARCHAR(200),
    @Editorial NVARCHAR(150) = NULL,
    @CategoriaId UNIQUEIDENTIFIER = NULL,
    @AnioPublicacion INT = NULL,
    @Sinopsis NVARCHAR(1000) = NULL,
    @Descripcion NVARCHAR(MAX) = NULL,
    @Precio DECIMAL(18, 2),
    @PrecioOferta DECIMAL(18, 2) = NULL,
    @Stock INT = 0,
    @ImagenPortada NVARCHAR(500) = NULL,
    @ImagenesSecundarias NVARCHAR(MAX) = NULL,
    @Peso DECIMAL(10, 2) = NULL,
    @Dimensiones NVARCHAR(50) = NULL,
    @NumeroPaginas INT = NULL,
    @Idioma NVARCHAR(50) = 'Español',
    @Destacado BIT = 0,
    @CreatedBy UNIQUEIDENTIFIER = NULL,
    @LibroId UNIQUEIDENTIFIER OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Validar que ISBN no exista
        IF EXISTS (SELECT 1 FROM dbo.Libros WHERE ISBN = @ISBN AND IsDeleted = 0)
        BEGIN
            RAISERROR('El ISBN ya está registrado.', 16, 1);
            RETURN -1;
        END
        
        -- Generar nuevo GUID para el libro
        SET @LibroId = NEWID();

        -- Insertar libro
        INSERT INTO dbo.Libros (
            Id, ISBN, Titulo, Autor, Editorial, CategoriaId, AnioPublicacion,
            Sinopsis, Descripcion, Precio, PrecioOferta, Stock,
            ImagenPortada, ImagenesSecundarias, Peso, Dimensiones,
            NumeroPaginas, Idioma, Destacado, Estado, CreatedBy, CreatedAt
        )
        VALUES (
            @LibroId, @ISBN, @Titulo, @Autor, @Editorial, @CategoriaId, @AnioPublicacion,
            @Sinopsis, @Descripcion, @Precio, @PrecioOferta, @Stock,
            @ImagenPortada, @ImagenesSecundarias, @Peso, @Dimensiones,
            @NumeroPaginas, @Idioma, @Destacado, 'Disponible', @CreatedBy, GETUTCDATE()
        );
        
        -- Registrar en auditoría de inventario
        IF @Stock > 0
        BEGIN
            INSERT INTO dbo.AuditoriaInventario (
                LibroId, TipoMovimiento, StockAnterior, Cantidad, StockNuevo,
                Motivo, UsuarioId, FechaMovimiento
            )
            VALUES (
                @LibroId, 'Ingreso', 0, @Stock, @Stock,
                'Ingreso inicial de libro', @CreatedBy, GETUTCDATE()
            );
        END
        
        COMMIT TRANSACTION;
        RETURN 0;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
        RETURN -1;
    END CATCH
END;
GO

PRINT N'SP sp_Libro_Insertar creado exitosamente.';
GO

-- =============================================
-- SP: sp_Libro_Actualizar
-- Descripción: Actualiza datos de un libro
-- =============================================
CREATE OR ALTER PROCEDURE dbo.sp_Libro_Actualizar
    @LibroId UNIQUEIDENTIFIER,
    @Titulo NVARCHAR(300),
    @Autor NVARCHAR(200),
    @Editorial NVARCHAR(150) = NULL,
    @CategoriaId UNIQUEIDENTIFIER = NULL,
    @AnioPublicacion INT = NULL,
    @Sinopsis NVARCHAR(1000) = NULL,
    @Descripcion NVARCHAR(MAX) = NULL,
    @Precio DECIMAL(18, 2),
    @PrecioOferta DECIMAL(18, 2) = NULL,
    @ImagenPortada NVARCHAR(500) = NULL,
    @ImagenesSecundarias NVARCHAR(MAX) = NULL,
    @Peso DECIMAL(10, 2) = NULL,
    @Dimensiones NVARCHAR(50) = NULL,
    @NumeroPaginas INT = NULL,
    @Idioma NVARCHAR(50) = 'Español',
    @Destacado BIT = 0,
    @UpdatedBy UNIQUEIDENTIFIER = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Validar que el libro existe
        IF NOT EXISTS (SELECT 1 FROM dbo.Libros WHERE Id = @LibroId AND IsDeleted = 0)
        BEGIN
            RAISERROR('Libro no encontrado.', 16, 1);
            RETURN -1;
        END
        
        -- Actualizar libro
        UPDATE dbo.Libros
        SET Titulo = @Titulo,
            Autor = @Autor,
            Editorial = @Editorial,
            CategoriaId = @CategoriaId,
            AnioPublicacion = @AnioPublicacion,
            Sinopsis = @Sinopsis,
            Descripcion = @Descripcion,
            Precio = @Precio,
            PrecioOferta = @PrecioOferta,
            ImagenPortada = @ImagenPortada,
            ImagenesSecundarias = @ImagenesSecundarias,
            Peso = @Peso,
            Dimensiones = @Dimensiones,
            NumeroPaginas = @NumeroPaginas,
            Idioma = @Idioma,
            Destacado = @Destacado,
            UpdatedAt = GETUTCDATE(),
            UpdatedBy = @UpdatedBy
        WHERE Id = @LibroId;
        
        COMMIT TRANSACTION;
        RETURN 0;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
        RETURN -1;
    END CATCH
END;
GO

PRINT N'SP sp_Libro_Actualizar creado exitosamente.';
GO

-- =============================================
-- SP: sp_Libro_Eliminar
-- Descripción: Eliminación lógica de libro
-- =============================================
CREATE OR ALTER PROCEDURE dbo.sp_Libro_Eliminar
    @LibroId UNIQUEIDENTIFIER,
    @DeletedBy UNIQUEIDENTIFIER = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Validar que el libro existe
        IF NOT EXISTS (SELECT 1 FROM dbo.Libros WHERE Id = @LibroId AND IsDeleted = 0)
        BEGIN
            RAISERROR('Libro no encontrado.', 16, 1);
            RETURN -1;
        END
        
        -- Soft delete
        UPDATE dbo.Libros
        SET IsDeleted = 1,
            DeletedAt = GETUTCDATE(),
            UpdatedBy = @DeletedBy,
            Estado = 'Descontinuado'
        WHERE Id = @LibroId;
        
        COMMIT TRANSACTION;
        RETURN 0;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
        RETURN -1;
    END CATCH
END;
GO

PRINT N'SP sp_Libro_Eliminar creado exitosamente.';
GO

-- =============================================
-- SP: sp_Libro_ListarPaginado
-- Descripción: Lista libros con paginación y filtros avanzados
-- =============================================
CREATE OR ALTER PROCEDURE dbo.sp_Libro_ListarPaginado
    @PageNumber INT = 1,
    @PageSize INT = 12,
    @Filtro NVARCHAR(200) = NULL,
    @CategoriaId UNIQUEIDENTIFIER = NULL,
    @Autor NVARCHAR(200) = NULL,
    @Editorial NVARCHAR(150) = NULL,
    @PrecioMin DECIMAL(18, 2) = NULL,
    @PrecioMax DECIMAL(18, 2) = NULL,
    @SoloDisponibles BIT = 0,
    @SoloDestacados BIT = 0,
    @Ordenamiento NVARCHAR(50) = 'FechaIngreso DESC',
    @TotalRegistros INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Calcular offset
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
    
    -- Obtener total de registros
    SELECT @TotalRegistros = COUNT(*)
    FROM dbo.Libros l
    WHERE l.IsDeleted = 0
        AND (@Filtro IS NULL OR 
             l.Titulo LIKE '%' + @Filtro + '%' OR 
             l.Autor LIKE '%' + @Filtro + '%' OR
             l.ISBN LIKE '%' + @Filtro + '%')
        AND (@CategoriaId IS NULL OR l.CategoriaId = @CategoriaId)
        AND (@Autor IS NULL OR l.Autor LIKE '%' + @Autor + '%')
        AND (@Editorial IS NULL OR l.Editorial LIKE '%' + @Editorial + '%')
        AND (@PrecioMin IS NULL OR l.Precio >= @PrecioMin)
        AND (@PrecioMax IS NULL OR l.Precio <= @PrecioMax)
        AND (@SoloDisponibles = 0 OR l.Stock > 0)
        AND (@SoloDestacados = 0 OR l.Destacado = 1);
    
    -- Retornar registros paginados
    SELECT 
        l.Id,
        l.ISBN,
        l.Titulo,
        l.Autor,
        l.Editorial,
        l.CategoriaId,
        c.Nombre AS CategoriaNombre,
        l.AnioPublicacion,
        l.Sinopsis,
        l.Precio,
        l.PrecioOferta,
        l.Stock,
        l.ImagenPortada,
        l.Destacado,
        l.Estado,
        l.FechaIngreso,
        dbo.fn_ObtenerEstadoStock(l.Stock) AS EstadoStock
    FROM dbo.Libros l
    LEFT JOIN dbo.Categorias c ON l.CategoriaId = c.Id
    WHERE l.IsDeleted = 0
        AND (@Filtro IS NULL OR 
             l.Titulo LIKE '%' + @Filtro + '%' OR 
             l.Autor LIKE '%' + @Filtro + '%' OR
             l.ISBN LIKE '%' + @Filtro + '%')
        AND (@CategoriaId IS NULL OR l.CategoriaId = @CategoriaId)
        AND (@Autor IS NULL OR l.Autor LIKE '%' + @Autor + '%')
        AND (@Editorial IS NULL OR l.Editorial LIKE '%' + @Editorial + '%')
        AND (@PrecioMin IS NULL OR l.Precio >= @PrecioMin)
        AND (@PrecioMax IS NULL OR l.Precio <= @PrecioMax)
        AND (@SoloDisponibles = 0 OR l.Stock > 0)
        AND (@SoloDestacados = 0 OR l.Destacado = 1)
    ORDER BY 
        CASE WHEN @Ordenamiento = 'Titulo ASC' THEN l.Titulo END ASC,
        CASE WHEN @Ordenamiento = 'Titulo DESC' THEN l.Titulo END DESC,
        CASE WHEN @Ordenamiento = 'Autor ASC' THEN l.Autor END ASC,
        CASE WHEN @Ordenamiento = 'Autor DESC' THEN l.Autor END DESC,
        CASE WHEN @Ordenamiento = 'Precio ASC' THEN l.Precio END ASC,
        CASE WHEN @Ordenamiento = 'Precio DESC' THEN l.Precio END DESC,
        CASE WHEN @Ordenamiento = 'FechaIngreso ASC' THEN l.FechaIngreso END ASC,
        CASE WHEN @Ordenamiento = 'FechaIngreso DESC' THEN l.FechaIngreso END DESC
    OFFSET @Offset ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END;
GO

PRINT N'SP sp_Libro_ListarPaginado creado exitosamente.';
GO

-- =============================================
-- SP: sp_Libro_ActualizarStock
-- Descripción: Actualiza stock de un libro con auditoría
-- =============================================
CREATE OR ALTER PROCEDURE dbo.sp_Libro_ActualizarStock
    @LibroId UNIQUEIDENTIFIER,
    @Cantidad INT, -- Positivo para incrementar, negativo para decrementar
    @TipoMovimiento NVARCHAR(50) = 'Ajuste',
    @Motivo NVARCHAR(500) = NULL,
    @ReferenciaId UNIQUEIDENTIFIER = NULL,
    @UsuarioId UNIQUEIDENTIFIER = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        DECLARE @StockAnterior INT;
        DECLARE @StockNuevo INT;
        
        -- Obtener stock actual
        SELECT @StockAnterior = Stock
        FROM dbo.Libros
        WHERE Id = @LibroId AND IsDeleted = 0;
        
        -- Validar que el libro existe
        IF @StockAnterior IS NULL
        BEGIN
            RAISERROR('Libro no encontrado.', 16, 1);
            RETURN -1;
        END
        
        -- Calcular nuevo stock
        SET @StockNuevo = @StockAnterior + @Cantidad;
        
        -- Validar que el stock no sea negativo
        IF @StockNuevo < 0
        BEGIN
            RAISERROR('Stock insuficiente para realizar la operación.', 16, 1);
            RETURN -1;
        END
        
        -- Actualizar stock
        UPDATE dbo.Libros
        SET Stock = @StockNuevo,
            UpdatedAt = GETUTCDATE(),
            UpdatedBy = @UsuarioId,
            Estado = CASE 
                WHEN @StockNuevo = 0 THEN 'Agotado'
                ELSE 'Disponible'
            END
        WHERE Id = @LibroId;
        
        -- Registrar en auditoría
        INSERT INTO dbo.AuditoriaInventario (
            LibroId, TipoMovimiento, StockAnterior, Cantidad, StockNuevo,
            Motivo, ReferenciaId, UsuarioId, FechaMovimiento
        )
        VALUES (
            @LibroId, @TipoMovimiento, @StockAnterior, @Cantidad, @StockNuevo,
            @Motivo, @ReferenciaId, @UsuarioId, GETUTCDATE()
        );
        
        COMMIT TRANSACTION;
        RETURN 0;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
        RETURN -1;
    END CATCH
END;
GO

PRINT N'SP sp_Libro_ActualizarStock creado exitosamente.';
GO

-- =============================================
-- SP: sp_Libro_ObtenerDestacados
-- Descripción: Obtiene los libros destacados
-- =============================================
CREATE OR ALTER PROCEDURE dbo.sp_Libro_ObtenerDestacados
    @Top INT = 10
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT TOP (@Top)
        l.Id,
        l.ISBN,
        l.Titulo,
        l.Autor,
        l.Editorial,
        c.Nombre AS CategoriaNombre,
        l.Precio,
        l.PrecioOferta,
        l.Stock,
        l.ImagenPortada,
        l.Sinopsis,
        dbo.fn_ObtenerEstadoStock(l.Stock) AS EstadoStock
    FROM dbo.Libros l
    LEFT JOIN dbo.Categorias c ON l.CategoriaId = c.Id
    WHERE l.Destacado = 1
        AND l.IsDeleted = 0
        AND l.Estado = 'Disponible'
    ORDER BY l.FechaIngreso DESC;
END;
GO

PRINT N'SP sp_Libro_ObtenerDestacados creado exitosamente.';
GO

-- =============================================
-- SP: sp_Libro_ObtenerNovedades
-- Descripción: Obtiene los libros más recientes
-- =============================================
CREATE OR ALTER PROCEDURE dbo.sp_Libro_ObtenerNovedades
    @Top INT = 10
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT TOP (@Top)
        l.Id,
        l.ISBN,
        l.Titulo,
        l.Autor,
        l.Editorial,
        c.Nombre AS CategoriaNombre,
        l.Precio,
        l.PrecioOferta,
        l.Stock,
        l.ImagenPortada,
        l.Sinopsis,
        l.FechaIngreso,
        dbo.fn_ObtenerEstadoStock(l.Stock) AS EstadoStock
    FROM dbo.Libros l
    LEFT JOIN dbo.Categorias c ON l.CategoriaId = c.Id
    WHERE l.IsDeleted = 0
        AND l.Estado = 'Disponible'
    ORDER BY l.FechaIngreso DESC;
END;
GO

PRINT N'SP sp_Libro_ObtenerNovedades creado exitosamente.';
GO

-- =============================================
-- SP: sp_Libro_ObtenerMasVendidos
-- Descripción: Obtiene los libros más vendidos
-- =============================================
CREATE OR ALTER PROCEDURE dbo.sp_Libro_ObtenerMasVendidos
    @Top INT = 10,
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
        c.Nombre AS CategoriaNombre,
        l.Precio,
        l.PrecioOferta,
        l.Stock,
        l.ImagenPortada,
        SUM(dv.Cantidad) AS TotalVendido,
        COUNT(DISTINCT v.Id) AS NumeroVentas,
        dbo.fn_ObtenerEstadoStock(l.Stock) AS EstadoStock
    FROM dbo.Libros l
    LEFT JOIN dbo.Categorias c ON l.CategoriaId = c.Id
    INNER JOIN dbo.DetallesVenta dv ON l.Id = dv.LibroId
    INNER JOIN dbo.Ventas v ON dv.VentaId = v.Id
    WHERE l.IsDeleted = 0
        AND dv.IsDeleted = 0
        AND v.IsDeleted = 0
        AND v.EstadoVenta = 'Completada'
        AND v.FechaVenta BETWEEN @FechaInicio AND @FechaFin
    GROUP BY l.Id, l.ISBN, l.Titulo, l.Autor, l.Editorial, c.Nombre,
             l.Precio, l.PrecioOferta, l.Stock, l.ImagenPortada
    ORDER BY TotalVendido DESC;
END;
GO

PRINT N'SP sp_Libro_ObtenerMasVendidos creado exitosamente.';
GO

-- =============================================
-- SP: sp_Libro_VerificarDisponibilidad
-- Descripción: Verifica disponibilidad de stock de un libro
-- =============================================
CREATE OR ALTER PROCEDURE dbo.sp_Libro_VerificarDisponibilidad
    @LibroId UNIQUEIDENTIFIER,
    @CantidadRequerida INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Stock INT;
    DECLARE @Disponible BIT = 0;
    
    SELECT @Stock = Stock
    FROM dbo.Libros
    WHERE Id = @LibroId AND IsDeleted = 0;
    
    IF @Stock >= @CantidadRequerida
        SET @Disponible = 1;
    
    SELECT 
        @LibroId AS LibroId,
        @Stock AS StockActual,
        @CantidadRequerida AS CantidadRequerida,
        @Disponible AS Disponible,
        CASE 
            WHEN @Disponible = 1 THEN 'Stock disponible'
            ELSE 'Stock insuficiente'
        END AS Mensaje;
END;
GO

PRINT N'SP sp_Libro_VerificarDisponibilidad creado exitosamente.';
GO

-- =============================================
-- SP: sp_Libro_ObtenerPorISBN
-- Descripción: Obtiene un libro por su ISBN
-- =============================================
CREATE OR ALTER PROCEDURE dbo.sp_Libro_ObtenerPorISBN
    @ISBN NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        l.Id,
        l.ISBN,
        l.Titulo,
        l.Autor,
        l.Editorial,
        l.CategoriaId,
        c.Nombre AS CategoriaNombre,
        l.AnioPublicacion,
        l.Sinopsis,
        l.Descripcion,
        l.Precio,
        l.PrecioOferta,
        l.Stock,
        l.ImagenPortada,
        l.ImagenesSecundarias,
        l.Peso,
        l.Dimensiones,
        l.NumeroPaginas,
        l.Idioma,
        l.Destacado,
        l.Estado,
        l.FechaIngreso,
        dbo.fn_ObtenerEstadoStock(l.Stock) AS EstadoStock
    FROM dbo.Libros l
    LEFT JOIN dbo.Categorias c ON l.CategoriaId = c.Id
    WHERE l.ISBN = @ISBN AND l.IsDeleted = 0;
END;
GO

PRINT N'SP sp_Libro_ObtenerPorISBN creado exitosamente.';
GO

PRINT N'Todos los procedimientos de Libros creados exitosamente.';
GO
