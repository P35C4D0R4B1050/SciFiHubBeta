-- =============================================
-- SciFiHub Database - Procedimientos de Ventas
-- SQL Server 2021+
-- Versión: 1.0
-- =============================================

USE SciFiHubDB;
GO

PRINT N'Iniciando creación de procedimientos almacenados de Ventas...';
GO

-- =============================================
-- C. PROCEDIMIENTOS ALMACENADOS - GESTIÓN DE VENTAS
-- =============================================

-- =============================================
-- SP: sp_Venta_Registrar
-- Descripción: Registra una venta completa con sus detalles
-- Transacción completa: crear venta + insertar detalles + actualizar stock
-- =============================================
CREATE OR ALTER PROCEDURE dbo.sp_Venta_Registrar
    @ClienteId UNIQUEIDENTIFIER,
    @VendedorId UNIQUEIDENTIFIER = NULL,
    @Subtotal DECIMAL(18, 2),
    @Descuento DECIMAL(18, 2) = 0,
    @IGV DECIMAL(18, 2),
    @Total DECIMAL(18, 2),
    @MetodoPago NVARCHAR(30),
    @DireccionCalle NVARCHAR(200) = NULL,
    @DireccionCiudad NVARCHAR(100) = NULL,
    @DireccionDepartamento NVARCHAR(100) = NULL,
    @DireccionCodigoPostal NVARCHAR(10) = NULL,
    @DireccionPais NVARCHAR(50) = 'Perú',
    @DireccionReferencia NVARCHAR(300) = NULL,
    @NotasVenta NVARCHAR(1000) = NULL,
    @DetallesVentaJSON NVARCHAR(MAX), -- JSON: [{"LibroId":"guid","Cantidad":1,"PrecioUnitario":50.00,"Descuento":0}]
    @CreatedBy UNIQUEIDENTIFIER = NULL,
    @VentaId UNIQUEIDENTIFIER OUTPUT,
    @NumeroVenta NVARCHAR(50) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Generar número de venta
        SET @NumeroVenta = dbo.fn_ObtenerNumeroVentaSecuencial();
        
        -- Insertar venta
        INSERT INTO dbo.Ventas (
            NumeroVenta, ClienteId, VendedorId, FechaVenta, Subtotal, Descuento, IGV, Total,
            EstadoVenta, MetodoPago, DireccionCalle, DireccionCiudad, DireccionDepartamento,
            DireccionCodigoPostal, DireccionPais, DireccionReferencia, NotasVenta,
            CreatedBy, CreatedAt
        )
        VALUES (
            @NumeroVenta, @ClienteId, @VendedorId, GETUTCDATE(), @Subtotal, @Descuento, @IGV, @Total,
            'Completada', @MetodoPago, @DireccionCalle, @DireccionCiudad, @DireccionDepartamento,
            @DireccionCodigoPostal, @DireccionPais, @DireccionReferencia, @NotasVenta,
            @CreatedBy, GETUTCDATE()
        );
        
        -- Recuperar VentaId (si es UNIQUEIDENTIFIER generado por la tabla)
        SELECT @VentaId = Id
        FROM dbo.Ventas
        WHERE NumeroVenta = @NumeroVenta;
        
        -- Insertar detalles de venta desde JSON
        DECLARE @DetallesTable TABLE (
            LibroId UNIQUEIDENTIFIER,
            Cantidad INT,
            PrecioUnitario DECIMAL(18, 2),
            Descuento DECIMAL(18, 2),
            Subtotal DECIMAL(18, 2)
        );
        
        INSERT INTO @DetallesTable (LibroId, Cantidad, PrecioUnitario, Descuento, Subtotal)
        SELECT 
            LibroId,
            Cantidad,
            PrecioUnitario,
            Descuento,
            (Cantidad * PrecioUnitario) - Descuento AS Subtotal
        FROM OPENJSON(@DetallesVentaJSON)
        WITH (
            LibroId UNIQUEIDENTIFIER '$.LibroId',
            Cantidad INT '$.Cantidad',
            PrecioUnitario DECIMAL(18, 2) '$.PrecioUnitario',
            Descuento DECIMAL(18, 2) '$.Descuento'
        );
        
        -- Validar stock disponible
        DECLARE @LibroId UNIQUEIDENTIFIER;
        DECLARE @Cantidad INT;
        DECLARE @StockDisponible INT;
        DECLARE @Titulo NVARCHAR(300);
        
        DECLARE detalle_cursor CURSOR FOR
        SELECT LibroId, Cantidad FROM @DetallesTable;
        
        OPEN detalle_cursor;
        FETCH NEXT FROM detalle_cursor INTO @LibroId, @Cantidad;
        
        WHILE @@FETCH_STATUS = 0
        BEGIN
            SELECT @StockDisponible = Stock, @Titulo = Titulo
            FROM dbo.Libros
            WHERE Id = @LibroId AND IsDeleted = 0;
            
            IF @StockDisponible < @Cantidad
            BEGIN
                CLOSE detalle_cursor;
                DEALLOCATE detalle_cursor;
                RAISERROR('Stock insuficiente para el libro: %s', 16, 1, @Titulo);
                RETURN -1;
            END
            
            FETCH NEXT FROM detalle_cursor INTO @LibroId, @Cantidad;
        END
        
        CLOSE detalle_cursor;
        DEALLOCATE detalle_cursor;
        
        -- Insertar detalles
        INSERT INTO dbo.DetallesVenta (VentaId, LibroId, Cantidad, PrecioUnitario, Descuento, Subtotal, CreatedAt)
        SELECT @VentaId, LibroId, Cantidad, PrecioUnitario, Descuento, Subtotal, GETUTCDATE()
        FROM @DetallesTable;
        
        -- Actualizar stock
        DECLARE detalle_cursor2 CURSOR FOR
        SELECT LibroId, Cantidad FROM @DetallesTable;
        
        OPEN detalle_cursor2;
        FETCH NEXT FROM detalle_cursor2 INTO @LibroId, @Cantidad;
        
        WHILE @@FETCH_STATUS = 0
        BEGIN
            DECLARE @CantidadNeg INT;
            SET @CantidadNeg = -@Cantidad;

            EXEC dbo.sp_Libro_ActualizarStock  
                 @LibroId = @LibroId,
                 @Cantidad = @CantidadNeg,
                 @TipoMovimiento = 'Venta',
                 @Motivo = 'Venta completada',
                 @ReferenciaId = @VentaId,
                 @UsuarioId = @VendedorId;
            
            FETCH NEXT FROM detalle_cursor2 INTO @LibroId, @Cantidad;
        END
        
        CLOSE detalle_cursor2;
        DEALLOCATE detalle_cursor2;
        
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

PRINT N'SP sp_Venta_Registrar creado exitosamente.';
GO

-- =============================================
-- SP: sp_Venta_ObtenerDetalle
-- Descripción: Obtiene detalle completo de una venta
-- =============================================
CREATE OR ALTER PROCEDURE dbo.sp_Venta_ObtenerDetalle
    @VentaId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Datos de la venta
    SELECT 
        v.Id,
        v.NumeroVenta,
        v.ClienteId,
        c.NombreCompleto AS ClienteNombre,
        c.Email AS ClienteEmail,
        c.Telefono AS ClienteTelefono,
        v.VendedorId,
        vend.NombreCompleto AS VendedorNombre,
        v.FechaVenta,
        v.Subtotal,
        v.Descuento,
        v.IGV,
        v.Total,
        v.EstadoVenta,
        v.MetodoPago,
        v.DireccionCalle,
        v.DireccionCiudad,
        v.DireccionDepartamento,
        v.DireccionCodigoPostal,
        v.DireccionPais,
        v.DireccionReferencia,
        v.NotasVenta
    FROM dbo.Ventas v
    INNER JOIN dbo.Usuarios c ON v.ClienteId = c.Id
    LEFT JOIN dbo.Usuarios vend ON v.VendedorId = vend.Id
    WHERE v.Id = @VentaId AND v.IsDeleted = 0;
    
    -- Detalles de la venta
    SELECT 
        dv.Id,
        dv.LibroId,
        l.Titulo,
        l.Autor,
        l.ISBN,
        l.ImagenPortada,
        dv.Cantidad,
        dv.PrecioUnitario,
        dv.Descuento,
        dv.Subtotal
    FROM dbo.DetallesVenta dv
    INNER JOIN dbo.Libros l ON dv.LibroId = l.Id
    WHERE dv.VentaId = @VentaId AND dv.IsDeleted = 0;
END;
GO

PRINT N'SP sp_Venta_ObtenerDetalle creado exitosamente.';
GO

-- =============================================
-- SP: sp_Venta_ListarPorCliente
-- Descripción: Lista las ventas de un cliente
-- =============================================
CREATE OR ALTER PROCEDURE dbo.sp_Venta_ListarPorCliente
    @ClienteId UNIQUEIDENTIFIER,
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @TotalRegistros INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
    
    -- Total de registros
    SELECT @TotalRegistros = COUNT(*)
    FROM dbo.Ventas
    WHERE ClienteId = @ClienteId AND IsDeleted = 0;
    
    -- Ventas del cliente
    SELECT 
        v.Id,
        v.NumeroVenta,
        v.FechaVenta,
        v.Total,
        v.EstadoVenta,
        v.MetodoPago,
        (SELECT COUNT(*) FROM dbo.DetallesVenta WHERE VentaId = v.Id) AS CantidadItems
    FROM dbo.Ventas v
    WHERE v.ClienteId = @ClienteId AND v.IsDeleted = 0
    ORDER BY v.FechaVenta DESC
    OFFSET @Offset ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END;
GO

PRINT N'SP sp_Venta_ListarPorCliente creado exitosamente.';
GO

-- =============================================
-- SP: sp_Venta_ListarPaginado
-- Descripción: Lista todas las ventas con filtros y paginación
-- =============================================
CREATE OR ALTER PROCEDURE dbo.sp_Venta_ListarPaginado
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @FechaInicio DATETIME2(7) = NULL,
    @FechaFin DATETIME2(7) = NULL,
    @ClienteId UNIQUEIDENTIFIER = NULL,
    @VendedorId UNIQUEIDENTIFIER = NULL,
    @EstadoVenta NVARCHAR(20) = NULL,
    @MetodoPago NVARCHAR(30) = NULL,
    @NumeroVenta NVARCHAR(50) = NULL,
    @Ordenamiento NVARCHAR(50) = 'FechaVenta DESC',
    @TotalRegistros INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
    
    -- Total de registros
    SELECT @TotalRegistros = COUNT(*)
    FROM dbo.Ventas v
    WHERE v.IsDeleted = 0
        AND (@FechaInicio IS NULL OR v.FechaVenta >= @FechaInicio)
        AND (@FechaFin IS NULL OR v.FechaVenta <= @FechaFin)
        AND (@ClienteId IS NULL OR v.ClienteId = @ClienteId)
        AND (@VendedorId IS NULL OR v.VendedorId = @VendedorId)
        AND (@EstadoVenta IS NULL OR v.EstadoVenta = @EstadoVenta)
        AND (@MetodoPago IS NULL OR v.MetodoPago = @MetodoPago)
        AND (@NumeroVenta IS NULL OR v.NumeroVenta LIKE '%' + @NumeroVenta + '%');
    
    -- Ventas paginadas
    SELECT 
        v.Id,
        v.NumeroVenta,
        v.ClienteId,
        c.NombreCompleto AS ClienteNombre,
        v.VendedorId,
        vend.NombreCompleto AS VendedorNombre,
        v.FechaVenta,
        v.Subtotal,
        v.Descuento,
        v.Total,
        v.EstadoVenta,
        v.MetodoPago,
        (SELECT COUNT(*) FROM dbo.DetallesVenta WHERE VentaId = v.Id) AS CantidadItems
    FROM dbo.Ventas v
    INNER JOIN dbo.Usuarios c ON v.ClienteId = c.Id
    LEFT JOIN dbo.Usuarios vend ON v.VendedorId = vend.Id
    WHERE v.IsDeleted = 0
        AND (@FechaInicio IS NULL OR v.FechaVenta >= @FechaInicio)
        AND (@FechaFin IS NULL OR v.FechaVenta <= @FechaFin)
        AND (@ClienteId IS NULL OR v.ClienteId = @ClienteId)
        AND (@VendedorId IS NULL OR v.VendedorId = @VendedorId)
        AND (@EstadoVenta IS NULL OR v.EstadoVenta = @EstadoVenta)
        AND (@MetodoPago IS NULL OR v.MetodoPago = @MetodoPago)
        AND (@NumeroVenta IS NULL OR v.NumeroVenta LIKE '%' + @NumeroVenta + '%')
    ORDER BY 
        CASE WHEN @Ordenamiento = 'FechaVenta ASC' THEN v.FechaVenta END ASC,
        CASE WHEN @Ordenamiento = 'FechaVenta DESC' THEN v.FechaVenta END DESC,
        CASE WHEN @Ordenamiento = 'Total ASC' THEN v.Total END ASC,
        CASE WHEN @Ordenamiento = 'Total DESC' THEN v.Total END DESC,
        CASE WHEN @Ordenamiento = 'NumeroVenta ASC' THEN v.NumeroVenta END ASC,
        CASE WHEN @Ordenamiento = 'NumeroVenta DESC' THEN v.NumeroVenta END DESC
    OFFSET @Offset ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END;
GO

PRINT N'SP sp_Venta_ListarPaginado creado exitosamente.';
GO

-- =============================================
-- SP: sp_Venta_CancelarVenta
-- Descripción: Cancela una venta y restaura el stock
-- =============================================
CREATE OR ALTER PROCEDURE dbo.sp_Venta_CancelarVenta
    @VentaId UNIQUEIDENTIFIER,
    @Motivo NVARCHAR(500) = NULL,
    @UpdatedBy UNIQUEIDENTIFIER = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Validar que la venta existe y puede cancelarse
        DECLARE @EstadoActual NVARCHAR(20);
        
        SELECT @EstadoActual = EstadoVenta
        FROM dbo.Ventas
        WHERE Id = @VentaId AND IsDeleted = 0;
        
        IF @EstadoActual IS NULL
        BEGIN
            RAISERROR('Venta no encontrada.', 16, 1);
            RETURN -1;
        END
        
        IF @EstadoActual = 'Cancelada'
        BEGIN
            RAISERROR('La venta ya está cancelada.', 16, 1);
            RETURN -1;
        END
        
        -- Restaurar stock de los libros vendidos
        DECLARE @LibroId UNIQUEIDENTIFIER;
        DECLARE @Cantidad INT;
        
        DECLARE detalle_cursor CURSOR FOR
        SELECT LibroId, Cantidad 
        FROM dbo.DetallesVenta 
        WHERE VentaId = @VentaId AND IsDeleted = 0;
        
        OPEN detalle_cursor;
        FETCH NEXT FROM detalle_cursor INTO @LibroId, @Cantidad;
        
        WHILE @@FETCH_STATUS = 0
        BEGIN
            EXEC dbo.sp_Libro_ActualizarStock 
                @LibroId = @LibroId,
                @Cantidad = @Cantidad, -- Positivo para incrementar
                @TipoMovimiento = 'Anulacion',
                @Motivo = @Motivo,
                @ReferenciaId = @VentaId,
                @UsuarioId = @UpdatedBy;
            
            FETCH NEXT FROM detalle_cursor INTO @LibroId, @Cantidad;
        END
        
        CLOSE detalle_cursor;
        DEALLOCATE detalle_cursor;
        
        -- Actualizar estado de la venta
        UPDATE dbo.Ventas
        SET EstadoVenta = 'Cancelada',
            NotasVenta = ISNULL(NotasVenta + CHAR(13) + CHAR(10), '') + 'Cancelada: ' + ISNULL(@Motivo, 'Sin motivo especificado'),
            UpdatedAt = GETUTCDATE(),
            UpdatedBy = @UpdatedBy
        WHERE Id = @VentaId;
        
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

PRINT N'SP sp_Venta_CancelarVenta creado exitosamente.';
GO

-- =============================================
-- SP: sp_Venta_ActualizarEstado
-- Descripción: Actualiza el estado de una venta
-- =============================================
CREATE OR ALTER PROCEDURE dbo.sp_Venta_ActualizarEstado
    @VentaId UNIQUEIDENTIFIER,
    @NuevoEstado NVARCHAR(20),
    @UpdatedBy UNIQUEIDENTIFIER = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Validar que la venta existe
        IF NOT EXISTS (SELECT 1 FROM dbo.Ventas WHERE Id = @VentaId AND IsDeleted = 0)
        BEGIN
            RAISERROR('Venta no encontrada.', 16, 1);
            RETURN -1;
        END
        
        -- Actualizar estado
        UPDATE dbo.Ventas
        SET EstadoVenta = @NuevoEstado,
            UpdatedAt = GETUTCDATE(),
            UpdatedBy = @UpdatedBy
        WHERE Id = @VentaId;
        
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

PRINT N'SP sp_Venta_ActualizarEstado creado exitosamente.';
GO

-- =============================================
-- SP: sp_Venta_ObtenerComprobante
-- Descripción: Obtiene datos para generar comprobante de venta
-- =============================================
CREATE OR ALTER PROCEDURE dbo.sp_Venta_ObtenerComprobante
    @VentaId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Usar el procedimiento de detalle que ya retorna todo
    EXEC dbo.sp_Venta_ObtenerDetalle @VentaId;
END;
GO

PRINT N'SP sp_Venta_ObtenerComprobante creado exitosamente.';
GO

PRINT N'Todos los procedimientos de Ventas creados exitosamente.';
GO
