-- =============================================
-- SciFiHub Database - Triggers
-- SQL Server 2021+
-- Versión: 1.0
-- =============================================

USE SciFiHubDB;
GO

PRINT N'Iniciando creación de triggers...';
GO

-- =============================================
-- E. TRIGGERS INTELIGENTES Y EFICIENTES
-- =============================================

-- =============================================
-- Trigger: trg_Libros_ActualizarFechaModificacion
-- Descripción: Actualiza automáticamente UpdatedAt al modificar un libro
-- =============================================
CREATE OR ALTER TRIGGER dbo.trg_Libros_ActualizarFechaModificacion
ON dbo.Libros
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE l
    SET UpdatedAt = GETUTCDATE()
    FROM dbo.Libros l
    INNER JOIN inserted i ON l.Id = i.Id;
END;
GO

PRINT N'Trigger trg_Libros_ActualizarFechaModificacion creado exitosamente.';
GO

-- =============================================
-- Trigger: trg_Usuarios_ActualizarFechaModificacion
-- Descripción: Actualiza automáticamente UpdatedAt al modificar un usuario
-- =============================================
CREATE OR ALTER TRIGGER dbo.trg_Usuarios_ActualizarFechaModificacion
ON dbo.Usuarios
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE u
    SET UpdatedAt = GETUTCDATE()
    FROM dbo.Usuarios u
    INNER JOIN inserted i ON u.Id = i.Id;
END;
GO

PRINT N'Trigger trg_Usuarios_ActualizarFechaModificacion creado exitosamente.';
GO

-- =============================================
-- Trigger: trg_Ventas_ActualizarFechaModificacion
-- Descripción: Actualiza automáticamente UpdatedAt al modificar una venta
-- =============================================
CREATE OR ALTER TRIGGER dbo.trg_Ventas_ActualizarFechaModificacion
ON dbo.Ventas
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE v
    SET UpdatedAt = GETUTCDATE()
    FROM dbo.Ventas v
    INNER JOIN inserted i ON v.Id = i.Id;
END;
GO

PRINT N'Trigger trg_Ventas_ActualizarFechaModificacion creado exitosamente.';
GO

-- =============================================
-- Trigger: trg_CarritoCompras_ActualizarFecha
-- Descripción: Actualiza fecha de modificación del carrito
-- =============================================
CREATE OR ALTER TRIGGER dbo.trg_CarritoCompras_ActualizarFecha
ON dbo.DetallesCarrito
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Actualizar desde inserted (INSERT, UPDATE)
    UPDATE c
    SET FechaActualizacion = GETUTCDATE()
    FROM dbo.CarritoCompras c
    INNER JOIN inserted i ON c.Id = i.CarritoId;
    
    -- Actualizar desde deleted (DELETE)
    UPDATE c
    SET FechaActualizacion = GETUTCDATE()
    FROM dbo.CarritoCompras c
    INNER JOIN deleted d ON c.Id = d.CarritoId
    WHERE NOT EXISTS (SELECT 1 FROM inserted WHERE CarritoId = d.CarritoId);
END;
GO

PRINT N'Trigger trg_CarritoCompras_ActualizarFecha creado exitosamente.';
GO

PRINT N'Todos los triggers creados exitosamente.';
GO

PRINT N'========================================';
PRINT N'NOTA: Los siguientes triggers opcionales no se implementaron para dar más control a los SPs:';
PRINT N'  - trg_DetalleVenta_ActualizarStock: Stock se actualiza en sp_Venta_Registrar';
PRINT N'  - trg_Venta_GenerarNumero: Número se genera en fn_ObtenerNumeroVentaSecuencial';
PRINT N'  - trg_Libro_ValidarStock: Validación se hace en sp_Libro_ActualizarStock';
PRINT N'  - trg_Carrito_ValidarStock: Validación se hace en sp_Carrito_AgregarItem';
PRINT N'Esto permite mejor control de errores y transacciones.';
PRINT N'========================================';
GO
