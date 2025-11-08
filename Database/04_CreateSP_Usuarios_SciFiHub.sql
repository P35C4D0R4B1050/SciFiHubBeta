-- =============================================
-- SciFiHub Database - Procedimientos de Usuarios
-- SQL Server 2021+
-- Versión: 1.0
-- =============================================

USE SciFiHubDB;
GO

PRINT N'Iniciando creación de procedimientos almacenados de Usuarios...';
GO

-- =============================================
-- C. PROCEDIMIENTOS ALMACENADOS - GESTIÓN DE USUARIOS
-- =============================================

-- =============================================
-- SP: sp_Usuario_Insertar
-- Descripción: Inserta un nuevo usuario
-- =============================================
CREATE OR ALTER PROCEDURE dbo.sp_Usuario_Insertar
    @NombreCompleto NVARCHAR(200),
    @Email NVARCHAR(256),
    @Username NVARCHAR(100),
    @PasswordHash NVARCHAR(500),
    @Rol NVARCHAR(20),
    @Telefono NVARCHAR(20) = NULL,
    @Avatar NVARCHAR(500) = NULL,
    @CreatedBy UNIQUEIDENTIFIER = NULL,
    @UsuarioId UNIQUEIDENTIFIER OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Validar que email no exista
        IF EXISTS (SELECT 1 FROM dbo.Usuarios WHERE Email = @Email AND IsDeleted = 0)
        BEGIN
            RAISERROR('El email ya está registrado.', 16, 1);
            RETURN -1;
        END
        
        -- Validar que username no exista
        IF EXISTS (SELECT 1 FROM dbo.Usuarios WHERE Username = @Username AND IsDeleted = 0)
        BEGIN
            RAISERROR('El nombre de usuario ya está registrado.', 16, 1);
            RETURN -1;
        END
        
        -- Generar nuevo GUID para el usuario
        SET @UsuarioId = NEWID();

        -- Insertar usuario
        INSERT INTO dbo.Usuarios (
            Id, NombreCompleto, Email, Username, PasswordHash, Rol,
            Telefono, Avatar, Estado, CreatedBy, CreatedAt
        )
        VALUES (
            @UsuarioId, @NombreCompleto, @Email, @Username, @PasswordHash, @Rol,
            @Telefono, @Avatar, 'Activo', @CreatedBy, GETUTCDATE()
        );
        
        COMMIT TRANSACTION;
        RETURN 0;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
        RETURN -1;
    END CATCH
END;
GO

PRINT N'SP sp_Usuario_Insertar creado exitosamente.';
GO

-- =============================================
-- SP: sp_Usuario_Actualizar
-- Descripción: Actualiza datos de un usuario
-- =============================================
CREATE OR ALTER PROCEDURE dbo.sp_Usuario_Actualizar
    @UsuarioId UNIQUEIDENTIFIER,
    @NombreCompleto NVARCHAR(200),
    @Email NVARCHAR(256),
    @Telefono NVARCHAR(20) = NULL,
    @Avatar NVARCHAR(500) = NULL,
    @UpdatedBy UNIQUEIDENTIFIER = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Validar que el usuario existe
        IF NOT EXISTS (SELECT 1 FROM dbo.Usuarios WHERE Id = @UsuarioId AND IsDeleted = 0)
        BEGIN
            RAISERROR('Usuario no encontrado.', 16, 1);
            RETURN -1;
        END
        
        -- Validar que email no esté en uso por otro usuario
        IF EXISTS (SELECT 1 FROM dbo.Usuarios WHERE Email = @Email AND Id <> @UsuarioId AND IsDeleted = 0)
        BEGIN
            RAISERROR('El email ya está registrado por otro usuario.', 16, 1);
            RETURN -1;
        END
        
        -- Actualizar usuario
        UPDATE dbo.Usuarios
        SET NombreCompleto = @NombreCompleto,
            Email = @Email,
            Telefono = @Telefono,
            Avatar = @Avatar,
            UpdatedAt = GETUTCDATE(),
            UpdatedBy = @UpdatedBy
        WHERE Id = @UsuarioId;
        
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

PRINT N'SP sp_Usuario_Actualizar creado exitosamente.';
GO

-- =============================================
-- SP: sp_Usuario_Eliminar
-- Descripción: Eliminación lógica de usuario
-- =============================================
CREATE OR ALTER PROCEDURE dbo.sp_Usuario_Eliminar
    @UsuarioId UNIQUEIDENTIFIER,
    @DeletedBy UNIQUEIDENTIFIER = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Validar que el usuario existe
        IF NOT EXISTS (SELECT 1 FROM dbo.Usuarios WHERE Id = @UsuarioId AND IsDeleted = 0)
        BEGIN
            RAISERROR('Usuario no encontrado.', 16, 1);
            RETURN -1;
        END
        
        -- Soft delete
        UPDATE dbo.Usuarios
        SET IsDeleted = 1,
            DeletedAt = GETUTCDATE(),
            UpdatedBy = @DeletedBy,
            Estado = 'Inactivo'
        WHERE Id = @UsuarioId;
        
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

PRINT N'SP sp_Usuario_Eliminar creado exitosamente.';
GO

-- =============================================
-- SP: sp_Usuario_ListarPaginado
-- Descripción: Lista usuarios con paginación y filtros
-- =============================================
CREATE OR ALTER PROCEDURE dbo.sp_Usuario_ListarPaginado
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @Filtro NVARCHAR(200) = NULL,
    @Rol NVARCHAR(20) = NULL,
    @Estado NVARCHAR(20) = NULL,
    @Ordenamiento NVARCHAR(50) = 'FechaRegistro DESC',
    @TotalRegistros INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Calcular offset
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
    
    -- Obtener total de registros
    SELECT @TotalRegistros = COUNT(*)
    FROM dbo.Usuarios
    WHERE IsDeleted = 0
        AND (@Filtro IS NULL OR 
             NombreCompleto LIKE '%' + @Filtro + '%' OR 
             Email LIKE '%' + @Filtro + '%' OR 
             Username LIKE '%' + @Filtro + '%')
        AND (@Rol IS NULL OR Rol = @Rol)
        AND (@Estado IS NULL OR Estado = @Estado);
    
    -- Retornar registros paginados
    SELECT 
        Id,
        NombreCompleto,
        Email,
        Username,
        Rol,
        FechaRegistro,
        UltimoAcceso,
        Estado,
        Avatar,
        Telefono,
        CreatedAt,
        UpdatedAt
    FROM dbo.Usuarios
    WHERE IsDeleted = 0
        AND (@Filtro IS NULL OR 
             NombreCompleto LIKE '%' + @Filtro + '%' OR 
             Email LIKE '%' + @Filtro + '%' OR 
             Username LIKE '%' + @Filtro + '%')
        AND (@Rol IS NULL OR Rol = @Rol)
        AND (@Estado IS NULL OR Estado = @Estado)
    ORDER BY 
        CASE WHEN @Ordenamiento = 'NombreCompleto ASC' THEN NombreCompleto END ASC,
        CASE WHEN @Ordenamiento = 'NombreCompleto DESC' THEN NombreCompleto END DESC,
        CASE WHEN @Ordenamiento = 'Email ASC' THEN Email END ASC,
        CASE WHEN @Ordenamiento = 'Email DESC' THEN Email END DESC,
        CASE WHEN @Ordenamiento = 'FechaRegistro ASC' THEN FechaRegistro END ASC,
        CASE WHEN @Ordenamiento = 'FechaRegistro DESC' THEN FechaRegistro END DESC
    OFFSET @Offset ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END;
GO

PRINT N'SP sp_Usuario_ListarPaginado creado exitosamente.';
GO

-- =============================================
-- SP: sp_Usuario_Autenticar
-- Descripción: Autenticación de usuario
-- =============================================
CREATE OR ALTER PROCEDURE dbo.sp_Usuario_Autenticar
    @Username NVARCHAR(100),
    @PasswordHash NVARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @UsuarioId UNIQUEIDENTIFIER;
    DECLARE @StoredPasswordHash NVARCHAR(500);
    DECLARE @Estado NVARCHAR(20);
    
    -- Obtener datos del usuario
    SELECT 
        @UsuarioId = Id,
        @StoredPasswordHash = PasswordHash,
        @Estado = Estado
    FROM dbo.Usuarios
    WHERE (Username = @Username OR Email = @Username)
        AND IsDeleted = 0;
    
    -- Validar existencia del usuario
    IF @UsuarioId IS NULL
    BEGIN
        -- Retornar vacío para no revelar si el usuario existe
        SELECT NULL AS Id, 'Usuario o contraseña incorrectos' AS Mensaje;
        RETURN -1;
    END
    
    -- Validar estado del usuario
    IF @Estado = 'Inactivo'
    BEGIN
        SELECT NULL AS Id, 'Usuario inactivo. Contacte al administrador.' AS Mensaje;
        RETURN -1;
    END
    
    -- Validar contraseña
    IF @StoredPasswordHash <> @PasswordHash
    BEGIN
        SELECT NULL AS Id, 'Usuario o contraseña incorrectos' AS Mensaje;
        RETURN -1;
    END
    
    -- Actualizar último acceso
    UPDATE dbo.Usuarios
    SET UltimoAcceso = GETUTCDATE()
    WHERE Id = @UsuarioId;
    
    -- Retornar datos del usuario autenticado
    SELECT 
        Id,
        NombreCompleto,
        Email,
        Username,
        Rol,
        Avatar,
        Telefono,
        'Autenticación exitosa' AS Mensaje
    FROM dbo.Usuarios
    WHERE Id = @UsuarioId;
    
    RETURN 0;
END;
GO

PRINT N'SP sp_Usuario_Autenticar creado exitosamente.';
GO

-- =============================================
-- SP: sp_Usuario_CambiarEstado
-- Descripción: Cambiar estado de un usuario (Activo/Inactivo)
-- =============================================
CREATE OR ALTER PROCEDURE dbo.sp_Usuario_CambiarEstado
    @UsuarioId UNIQUEIDENTIFIER,
    @NuevoEstado NVARCHAR(20),
    @UpdatedBy UNIQUEIDENTIFIER = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Validar que el usuario existe
        IF NOT EXISTS (SELECT 1 FROM dbo.Usuarios WHERE Id = @UsuarioId AND IsDeleted = 0)
        BEGIN
            RAISERROR('Usuario no encontrado.', 16, 1);
            RETURN -1;
        END
        
        -- Actualizar estado
        UPDATE dbo.Usuarios
        SET Estado = @NuevoEstado,
            UpdatedAt = GETUTCDATE(),
            UpdatedBy = @UpdatedBy
        WHERE Id = @UsuarioId;
        
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

PRINT N'SP sp_Usuario_CambiarEstado creado exitosamente.';
GO

-- =============================================
-- SP: sp_Usuario_ObtenerPorEmail
-- Descripción: Obtiene un usuario por su email
-- =============================================
CREATE OR ALTER PROCEDURE dbo.sp_Usuario_ObtenerPorEmail
    @Email NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        NombreCompleto,
        Email,
        Username,
        Rol,
        FechaRegistro,
        UltimoAcceso,
        Estado,
        Avatar,
        Telefono
    FROM dbo.Usuarios
    WHERE Email = @Email AND IsDeleted = 0;
END;
GO

PRINT N'SP sp_Usuario_ObtenerPorEmail creado exitosamente.';
GO

-- =============================================
-- SP: sp_Usuario_ObtenerPorUsername
-- Descripción: Obtiene un usuario por su username
-- =============================================
CREATE OR ALTER PROCEDURE dbo.sp_Usuario_ObtenerPorUsername
    @Username NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        NombreCompleto,
        Email,
        Username,
        Rol,
        FechaRegistro,
        UltimoAcceso,
        Estado,
        Avatar,
        Telefono
    FROM dbo.Usuarios
    WHERE Username = @Username AND IsDeleted = 0;
END;
GO

PRINT N'SP sp_Usuario_ObtenerPorUsername creado exitosamente.';
GO

-- =============================================
-- SP: sp_Usuario_CambiarPassword
-- Descripción: Cambiar contraseña de usuario
-- =============================================
CREATE OR ALTER PROCEDURE dbo.sp_Usuario_CambiarPassword
    @UsuarioId UNIQUEIDENTIFIER,
    @PasswordHashActual NVARCHAR(500),
    @NuevoPasswordHash NVARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        DECLARE @StoredPasswordHash NVARCHAR(500);
        
        -- Obtener contraseña actual
        SELECT @StoredPasswordHash = PasswordHash
        FROM dbo.Usuarios
        WHERE Id = @UsuarioId AND IsDeleted = 0;
        
        -- Validar que el usuario existe
        IF @StoredPasswordHash IS NULL
        BEGIN
            RAISERROR('Usuario no encontrado.', 16, 1);
            RETURN -1;
        END
        
        -- Validar contraseña actual
        IF @StoredPasswordHash <> @PasswordHashActual
        BEGIN
            RAISERROR('La contraseña actual es incorrecta.', 16, 1);
            RETURN -1;
        END
        
        -- Actualizar contraseña
        UPDATE dbo.Usuarios
        SET PasswordHash = @NuevoPasswordHash,
            UpdatedAt = GETUTCDATE()
        WHERE Id = @UsuarioId;
        
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

PRINT N'SP sp_Usuario_CambiarPassword creado exitosamente.';
GO

PRINT N'Todos los procedimientos de Usuarios creados exitosamente.';
GO
