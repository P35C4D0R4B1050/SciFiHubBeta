-- =============================================
-- SciFiHub Database - Procedimientos de Carrito
-- SQL Server 2021+
-- Versión: 1.0
-- =============================================

USE SciFiHubDB;
GO

PRINT N'Iniciando creación de procedimientos almacenados de Carrito...';
GO

-- =============================================
-- C. PROCEDIMIENTOS ALMACENADOS - GESTIÓN DE CARRITO
-- =============================================

-- =============================================
-- SP: sp_Carrito_AgregarItem
-- Descripción: Agrega un item al carrito (crea carrito si no existe)
-- =============================================
CREATE OR ALTER PROCEDURE dbo.sp_Carrito_AgregarItem
    @ClienteId UNIQUEIDENTIFIER,
    @LibroId UNIQUEIDENTIFIER,
    @Cantidad INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        DECLARE @CarritoId UNIQUEIDENTIFIER;
        DECLARE @Stock INT;
        DECLARE @Precio DECIMAL(18, 2);
        
        -- Verificar stock disponible
        SELECT @Stock = Stock, @Precio = ISNULL(PrecioOferta, Precio)
        FROM dbo.Libros
        WHERE Id = @LibroId AND IsDeleted = 0;
        
        IF @Stock < @Cantidad
        BEGIN
            RAISERROR('Stock insuficiente para este libro.', 16, 1);
            RETURN -1;
        END
        
        -- Obtener carrito activo
        SELECT @CarritoId = Id
        FROM dbo.CarritoCompras
        WHERE ClienteId = @ClienteId AND Estado = 'Activo';
        
        -- Si no existe, crear uno nuevo
        IF @CarritoId IS NULL
        BEGIN
            SET @CarritoId = NEWID();
            
            INSERT INTO dbo.CarritoCompras (Id, ClienteId, Estado, FechaCreacion)
            VALUES (@CarritoId, @ClienteId, 'Activo', GETUTCDATE());
        END
        
        -- Verificar si el libro ya está en el carrito
        IF EXISTS (SELECT 1 FROM dbo.DetallesCarrito WHERE CarritoId = @CarritoId AND LibroId = @LibroId)
        BEGIN
            -- Actualizar cantidad
            UPDATE dbo.DetallesCarrito
            SET Cantidad = Cantidad + @Cantidad,
                PrecioUnitario = @Precio
            WHERE CarritoId = @CarritoId AND LibroId = @LibroId;
        END
        ELSE
        BEGIN
            -- Insertar nuevo item
            INSERT INTO dbo.DetallesCarrito (CarritoId, LibroId, Cantidad, PrecioUnitario, FechaAgregado)
            VALUES (@CarritoId, @LibroId, @Cantidad, @Precio, GETUTCDATE());
        END
        
        -- Actualizar fecha del carrito
        UPDATE dbo.CarritoCompras
        SET FechaActualizacion = GETUTCDATE()
        WHERE Id = @CarritoId;
        
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

PRINT N'SP sp_Carrito_AgregarItem creado exitosamente.';
GO

-- =============================================
-- SP: sp_Carrito_ActualizarCantidad
-- Descripción: Actualiza la cantidad de un item en el carrito
-- =============================================
CREATE OR ALTER PROCEDURE dbo.sp_Carrito_ActualizarCantidad
    @DetalleCarritoId UNIQUEIDENTIFIER,
    @NuevaCantidad INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        DECLARE @LibroId UNIQUEIDENTIFIER;
        DECLARE @Stock INT;
        DECLARE @CarritoId UNIQUEIDENTIFIER;
        
        -- Obtener información del detalle
        SELECT @LibroId = LibroId, @CarritoId = CarritoId
        FROM dbo.DetallesCarrito
        WHERE Id = @DetalleCarritoId;
        
        IF @LibroId IS NULL
        BEGIN
            RAISERROR('Item no encontrado en el carrito.', 16, 1);
            RETURN -1;
        END
        
        -- Verificar stock disponible
        SELECT @Stock = Stock
        FROM dbo.Libros
        WHERE Id = @LibroId AND IsDeleted = 0;
        
        IF @Stock < @NuevaCantidad
        BEGIN
            RAISERROR('Stock insuficiente para la cantidad solicitada.', 16, 1);
            RETURN -1;
        END
        
        -- Actualizar cantidad
        UPDATE dbo.DetallesCarrito
        SET Cantidad = @NuevaCantidad
        WHERE Id = @DetalleCarritoId;
        
        -- Actualizar fecha del carrito
        UPDATE dbo.CarritoCompras
        SET FechaActualizacion = GETUTCDATE()
        WHERE Id = @CarritoId;
        
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

PRINT N'SP sp_Carrito_ActualizarCantidad creado exitosamente.';
GO

-- =============================================
-- SP: sp_Carrito_EliminarItem
-- Descripción: Elimina un item del carrito
-- =============================================
CREATE OR ALTER PROCEDURE dbo.sp_Carrito_EliminarItem
    @DetalleCarritoId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        DECLARE @CarritoId UNIQUEIDENTIFIER;
        
        SELECT @CarritoId = CarritoId
        FROM dbo.DetallesCarrito
        WHERE Id = @DetalleCarritoId;
        
        -- Eliminar item
        DELETE FROM dbo.DetallesCarrito
        WHERE Id = @DetalleCarritoId;
        
        -- Actualizar fecha del carrito
        UPDATE dbo.CarritoCompras
        SET FechaActualizacion = GETUTCDATE()
        WHERE Id = @CarritoId;
        
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

PRINT N'SP sp_Carrito_EliminarItem creado exitosamente.';
GO

-- =============================================
-- SP: sp_Carrito_ObtenerPorCliente
-- Descripción: Obtiene el carrito activo de un cliente con sus items
-- =============================================
CREATE OR ALTER PROCEDURE dbo.sp_Carrito_ObtenerPorCliente
    @ClienteId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @CarritoId UNIQUEIDENTIFIER;
    
    -- Obtener carrito activo
    SELECT @CarritoId = Id
    FROM dbo.CarritoCompras
    WHERE ClienteId = @ClienteId AND Estado = 'Activo';
    
    IF @CarritoId IS NULL
    BEGIN
        -- Retornar vacío
        SELECT NULL AS Id, @ClienteId AS ClienteId, 'Activo' AS Estado, GETUTCDATE() AS FechaCreacion;
        SELECT NULL AS Id; -- Detalles vacíos
        RETURN;
    END
    
    -- Información del carrito
    SELECT 
        Id,
        ClienteId,
        FechaCreacion,
        FechaActualizacion,
        Estado
    FROM dbo.CarritoCompras
    WHERE Id = @CarritoId;
    
    -- Detalles del carrito
    SELECT 
        dc.Id,
        dc.CarritoId,
        dc.LibroId,
        l.Titulo,
        l.Autor,
        l.ISBN,
        l.ImagenPortada,
        l.Stock,
        dc.Cantidad,
        dc.PrecioUnitario,
        (dc.Cantidad * dc.PrecioUnitario) AS Subtotal,
        CASE 
            WHEN l.Stock >= dc.Cantidad THEN 1
            ELSE 0
        END AS StockDisponible
    FROM dbo.DetallesCarrito dc
    INNER JOIN dbo.Libros l ON dc.LibroId = l.Id
    WHERE dc.CarritoId = @CarritoId
    ORDER BY dc.FechaAgregado DESC;
END;
GO

PRINT N'SP sp_Carrito_ObtenerPorCliente creado exitosamente.';
GO

-- =============================================
-- SP: sp_Carrito_Limpiar
-- Descripción: Limpia todos los items del carrito de un cliente
-- =============================================
CREATE OR ALTER PROCEDURE dbo.sp_Carrito_Limpiar
    @ClienteId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        DECLARE @CarritoId UNIQUEIDENTIFIER;
        
        SELECT @CarritoId = Id
        FROM dbo.CarritoCompras
        WHERE ClienteId = @ClienteId AND Estado = 'Activo';
        
        IF @CarritoId IS NOT NULL
        BEGIN
            -- Eliminar todos los detalles
            DELETE FROM dbo.DetallesCarrito
            WHERE CarritoId = @CarritoId;
            
            -- Actualizar estado del carrito
            UPDATE dbo.CarritoCompras
            SET Estado = 'Abandonado',
                FechaActualizacion = GETUTCDATE()
            WHERE Id = @CarritoId;
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

PRINT N'SP sp_Carrito_Limpiar creado exitosamente.';
GO

-- =============================================
-- SP: sp_Carrito_ConvertirAVenta
-- Descripción: Convierte el carrito en una venta (checkout process)
-- =============================================
CREATE OR ALTER PROCEDURE dbo.sp_Carrito_ConvertirAVenta
    @ClienteId UNIQUEIDENTIFIER,
    @VendedorId UNIQUEIDENTIFIER = NULL,
    @MetodoPago NVARCHAR(30),
    @DireccionCalle NVARCHAR(200) = NULL,
    @DireccionCiudad NVARCHAR(100) = NULL,
    @DireccionDepartamento NVARCHAR(100) = NULL,
    @DireccionCodigoPostal NVARCHAR(10) = NULL,
    @DireccionPais NVARCHAR(50) = 'Perú',
    @DireccionReferencia NVARCHAR(300) = NULL,
    @VentaId UNIQUEIDENTIFIER OUTPUT,
    @NumeroVenta NVARCHAR(50) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        DECLARE @CarritoId UNIQUEIDENTIFIER;
        DECLARE @Subtotal DECIMAL(18, 2) = 0;
        DECLARE @Descuento DECIMAL(18, 2) = 0;
        DECLARE @IGV DECIMAL(18, 2) = 0;
        DECLARE @Total DECIMAL(18, 2) = 0;
        DECLARE @DetallesJSON NVARCHAR(MAX);
        
        -- Obtener carrito activo
        SELECT @CarritoId = Id
        FROM dbo.CarritoCompras
        WHERE ClienteId = @ClienteId AND Estado = 'Activo';
        
        IF @CarritoId IS NULL
        BEGIN
            RAISERROR('No hay carrito activo para este cliente.', 16, 1);
            RETURN -1;
        END
        
        -- Validar que hay items en el carrito
        IF NOT EXISTS (SELECT 1 FROM dbo.DetallesCarrito WHERE CarritoId = @CarritoId)
        BEGIN
            RAISERROR('El carrito está vacío.', 16, 1);
            RETURN -1;
        END
        
        -- Calcular totales
        SELECT @Subtotal = SUM(Cantidad * PrecioUnitario)
        FROM dbo.DetallesCarrito
        WHERE CarritoId = @CarritoId;
        
        SET @IGV = @Subtotal * 0.18;
        SET @Total = @Subtotal + @IGV;
        
        -- Convertir detalles del carrito a JSON para sp_Venta_Registrar
        SELECT @DetallesJSON = (
            SELECT 
                LibroId,
                Cantidad,
                PrecioUnitario,
                0 AS Descuento
            FROM dbo.DetallesCarrito
            WHERE CarritoId = @CarritoId
            FOR JSON PATH
        );
        
        -- Registrar venta
        EXEC dbo.sp_Venta_Registrar
            @ClienteId = @ClienteId,
            @VendedorId = @VendedorId,
            @Subtotal = @Subtotal,
            @Descuento = @Descuento,
            @IGV = @IGV,
            @Total = @Total,
            @MetodoPago = @MetodoPago,
            @DireccionCalle = @DireccionCalle,
            @DireccionCiudad = @DireccionCiudad,
            @DireccionDepartamento = @DireccionDepartamento,
            @DireccionCodigoPostal = @DireccionCodigoPostal,
            @DireccionPais = @DireccionPais,
            @DireccionReferencia = @DireccionReferencia,
            @DetallesVentaJSON = @DetallesJSON,
            @CreatedBy = @ClienteId,
            @VentaId = @VentaId OUTPUT,
            @NumeroVenta = @NumeroVenta OUTPUT;
        
        -- Marcar carrito como convertido
        UPDATE dbo.CarritoCompras
        SET Estado = 'Convertido',
            FechaActualizacion = GETUTCDATE()
        WHERE Id = @CarritoId;
        
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

PRINT N'SP sp_Carrito_ConvertirAVenta creado exitosamente.';
GO

PRINT N'Todos los procedimientos de Carrito creados exitosamente.';
GO
