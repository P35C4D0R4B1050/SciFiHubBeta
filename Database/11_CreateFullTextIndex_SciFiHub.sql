-- =============================================
-- SciFiHub Database - Búsqueda Optimizada (Sin Full-Text)
-- SQL Server 2021+
-- Versión: 1.0
-- =============================================

USE SciFiHubDB;
GO

PRINT N'Iniciando configuración de búsqueda optimizada...';
GO

-- =============================================
-- NOTA: Este script no usa Full-Text Search
-- Implementa búsqueda tradicional optimizada con LIKE
-- Para bases de datos pequeñas/medianas es suficiente
-- =============================================

PRINT N'Full-Text Search no está instalado.';
PRINT N'Usando búsqueda tradicional optimizada con índices.';
GO

-- =============================================
-- Crear índices adicionales para optimizar búsquedas con LIKE
-- =============================================

-- Índice compuesto para búsqueda combinada
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Libros_Busqueda_Compuesta' AND object_id = OBJECT_ID('dbo.Libros'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Libros_Busqueda_Compuesta 
    ON dbo.Libros(Titulo, Autor, Editorial)
    INCLUDE (ISBN, CategoriaId, Precio, PrecioOferta, Stock, ImagenPortada, Sinopsis, Destacado, Estado)
    WHERE IsDeleted = 0;
    
    PRINT N'Índice de búsqueda compuesta creado exitosamente.';
END
ELSE
BEGIN
    PRINT N'Índice de búsqueda compuesta ya existe.';
END
GO

-- =============================================
-- SP: sp_Libro_BuscarPorCriterios (Búsqueda Tradicional)
-- Descripción: Búsqueda avanzada usando LIKE optimizado
-- =============================================
CREATE OR ALTER PROCEDURE dbo.sp_Libro_BuscarPorCriterios
    @BusquedaTexto NVARCHAR(500) = NULL,
    @CategoriaId UNIQUEIDENTIFIER = NULL,
    @PrecioMin DECIMAL(18, 2) = NULL,
    @PrecioMax DECIMAL(18, 2) = NULL,
    @SoloDisponibles BIT = 1,
    @PageNumber INT = 1,
    @PageSize INT = 12,
    @TotalRegistros INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
    DECLARE @BusquedaPatron NVARCHAR(502);
    
    -- Si hay búsqueda de texto, preparar patrón
    IF @BusquedaTexto IS NOT NULL AND LEN(@BusquedaTexto) > 0
    BEGIN
        -- Limpiar y preparar patrón de búsqueda
        SET @BusquedaPatron = '%' + LTRIM(RTRIM(@BusquedaTexto)) + '%';
        
        -- Obtener total de registros
        SELECT @TotalRegistros = COUNT(*)
        FROM dbo.Libros l
        WHERE (
                l.Titulo LIKE @BusquedaPatron 
                OR l.Autor LIKE @BusquedaPatron
                OR l.ISBN LIKE @BusquedaPatron
                OR l.Editorial LIKE @BusquedaPatron
                OR l.Sinopsis LIKE @BusquedaPatron
            )
            AND l.IsDeleted = 0
            AND (@CategoriaId IS NULL OR l.CategoriaId = @CategoriaId)
            AND (@PrecioMin IS NULL OR l.Precio >= @PrecioMin)
            AND (@PrecioMax IS NULL OR l.Precio <= @PrecioMax)
            AND (@SoloDisponibles = 0 OR l.Stock > 0)
            AND l.Estado = 'Disponible';
        
        -- Retornar resultados paginados
        SELECT 
            l.Id,
            l.ISBN,
            l.Titulo,
            l.Autor,
            l.Editorial,
            l.CategoriaId,
            c.Nombre AS CategoriaNombre,
            l.Sinopsis,
            l.Precio,
            l.PrecioOferta,
            l.Stock,
            l.ImagenPortada,
            l.Destacado,
            dbo.fn_ObtenerEstadoStock(l.Stock) AS EstadoStock,
            -- Scoring simple para relevancia (opcional)
            CASE 
                WHEN l.Titulo LIKE @BusquedaPatron THEN 3
                WHEN l.Autor LIKE @BusquedaPatron THEN 2
                WHEN l.ISBN LIKE @BusquedaPatron THEN 4
                ELSE 1
            END AS Relevancia
        FROM dbo.Libros l
        LEFT JOIN dbo.Categorias c ON l.CategoriaId = c.Id
        WHERE (
                l.Titulo LIKE @BusquedaPatron 
                OR l.Autor LIKE @BusquedaPatron
                OR l.ISBN LIKE @BusquedaPatron
                OR l.Editorial LIKE @BusquedaPatron
                OR l.Sinopsis LIKE @BusquedaPatron
            )
            AND l.IsDeleted = 0
            AND (@CategoriaId IS NULL OR l.CategoriaId = @CategoriaId)
            AND (@PrecioMin IS NULL OR l.Precio >= @PrecioMin)
            AND (@PrecioMax IS NULL OR l.Precio <= @PrecioMax)
            AND (@SoloDisponibles = 0 OR l.Stock > 0)
            AND l.Estado = 'Disponible'
        ORDER BY 
            -- Ordenar por relevancia primero, luego por destacados
            CASE 
                WHEN l.Titulo LIKE @BusquedaPatron THEN 0
                WHEN l.Autor LIKE @BusquedaPatron THEN 1
                WHEN l.ISBN LIKE @BusquedaPatron THEN 2
                ELSE 3
            END ASC,
            l.Destacado DESC, 
            l.FechaIngreso DESC
        OFFSET @Offset ROWS
        FETCH NEXT @PageSize ROWS ONLY;
    END
    ELSE
    BEGIN
        -- Sin búsqueda de texto, usar listado estándar
        EXEC dbo.sp_Libro_ListarPaginado
            @PageNumber = @PageNumber,
            @PageSize = @PageSize,
            @CategoriaId = @CategoriaId,
            @PrecioMin = @PrecioMin,
            @PrecioMax = @PrecioMax,
            @SoloDisponibles = @SoloDisponibles,
            @TotalRegistros = @TotalRegistros OUTPUT;
    END
END;
GO

PRINT N'SP sp_Libro_BuscarPorCriterios creado exitosamente.';
GO

-- =============================================
-- SP: sp_Libro_BusquedaRapida (Búsqueda rápida para autocompletado)
-- Descripción: Búsqueda rápida en Título y Autor para autocomplete
-- =============================================
CREATE OR ALTER PROCEDURE dbo.sp_Libro_BusquedaRapida
    @Termino NVARCHAR(100),
    @Top INT = 10
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Patron NVARCHAR(102) = '%' + @Termino + '%';
    
    SELECT TOP (@Top)
        l.Id,
        l.ISBN,
        l.Titulo,
        l.Autor,
        l.Precio,
        l.PrecioOferta,
        l.Stock,
        l.ImagenPortada,
        c.Nombre AS CategoriaNombre,
        -- Prioridad de coincidencia
        CASE 
            WHEN l.Titulo LIKE @Termino + '%' THEN 1  -- Empieza con el término
            WHEN l.Autor LIKE @Termino + '%' THEN 2
            WHEN l.Titulo LIKE @Patron THEN 3         -- Contiene el término
            WHEN l.Autor LIKE @Patron THEN 4
            ELSE 5
        END AS Prioridad
    FROM dbo.Libros l
    LEFT JOIN dbo.Categorias c ON l.CategoriaId = c.Id
    WHERE (l.Titulo LIKE @Patron OR l.Autor LIKE @Patron OR l.ISBN LIKE @Patron)
        AND l.IsDeleted = 0
        AND l.Estado = 'Disponible'
        AND l.Stock > 0
    ORDER BY Prioridad ASC, l.Destacado DESC, l.FechaIngreso DESC;
END;
GO

PRINT N'SP sp_Libro_BusquedaRapida creado exitosamente.';
GO

-- =============================================
-- SP: sp_Libro_BusquedaAvanzada (Búsqueda con múltiples términos)
-- Descripción: Búsqueda que divide el texto en palabras y busca cada una
-- =============================================
CREATE OR ALTER PROCEDURE dbo.sp_Libro_BusquedaAvanzada
    @BusquedaTexto NVARCHAR(500),
    @CategoriaId UNIQUEIDENTIFIER = NULL,
    @SoloDisponibles BIT = 1,
    @PageNumber INT = 1,
    @PageSize INT = 12,
    @TotalRegistros INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
    
    -- Tabla temporal para almacenar palabras de búsqueda
    DECLARE @Palabras TABLE (Palabra NVARCHAR(100));
    
    -- Dividir el texto de búsqueda en palabras
    DECLARE @Palabra NVARCHAR(100);
    DECLARE @Posicion INT;
    DECLARE @TextoTemp NVARCHAR(500) = LTRIM(RTRIM(@BusquedaTexto)) + ' ';
    
    WHILE CHARINDEX(' ', @TextoTemp) > 0
    BEGIN
        SET @Posicion = CHARINDEX(' ', @TextoTemp);
        SET @Palabra = LTRIM(RTRIM(LEFT(@TextoTemp, @Posicion - 1)));
        
        IF LEN(@Palabra) > 2  -- Ignorar palabras muy cortas
        BEGIN
            INSERT INTO @Palabras (Palabra) VALUES (@Palabra);
        END
        
        SET @TextoTemp = LTRIM(RTRIM(SUBSTRING(@TextoTemp, @Posicion + 1, LEN(@TextoTemp))));
    END
    
    -- Contar total de registros
    SELECT @TotalRegistros = COUNT(DISTINCT l.Id)
    FROM dbo.Libros l
    CROSS APPLY @Palabras p
    WHERE (
            l.Titulo LIKE '%' + p.Palabra + '%'
            OR l.Autor LIKE '%' + p.Palabra + '%'
            OR l.Sinopsis LIKE '%' + p.Palabra + '%'
        )
        AND l.IsDeleted = 0
        AND (@CategoriaId IS NULL OR l.CategoriaId = @CategoriaId)
        AND (@SoloDisponibles = 0 OR l.Stock > 0)
        AND l.Estado = 'Disponible';
    
    -- Retornar resultados con scoring
    SELECT 
        l.Id,
        l.ISBN,
        l.Titulo,
        l.Autor,
        l.Editorial,
        l.CategoriaId,
        c.Nombre AS CategoriaNombre,
        l.Sinopsis,
        l.Precio,
        l.PrecioOferta,
        l.Stock,
        l.ImagenPortada,
        l.Destacado,
        dbo.fn_ObtenerEstadoStock(l.Stock) AS EstadoStock,
        -- Calcular score basado en coincidencias
        COUNT(DISTINCT p.Palabra) AS NumeroCoincidencias
    FROM dbo.Libros l
    LEFT JOIN dbo.Categorias c ON l.CategoriaId = c.Id
    CROSS APPLY @Palabras p
    WHERE (
            l.Titulo LIKE '%' + p.Palabra + '%'
            OR l.Autor LIKE '%' + p.Palabra + '%'
            OR l.Sinopsis LIKE '%' + p.Palabra + '%'
        )
        AND l.IsDeleted = 0
        AND (@CategoriaId IS NULL OR l.CategoriaId = @CategoriaId)
        AND (@SoloDisponibles = 0 OR l.Stock > 0)
        AND l.Estado = 'Disponible'
    GROUP BY 
        l.Id, l.ISBN, l.Titulo, l.Autor, l.Editorial, l.CategoriaId,
        c.Nombre, l.Sinopsis, l.Precio, l.PrecioOferta, l.Stock,
        l.ImagenPortada, l.Destacado, l.FechaIngreso
    ORDER BY 
        NumeroCoincidencias DESC,  -- Más coincidencias primero
        l.Destacado DESC,
        l.FechaIngreso DESC
    OFFSET @Offset ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END;
GO

PRINT N'SP sp_Libro_BusquedaAvanzada creado exitosamente.';
GO

PRINT N'========================================';
PRINT N'Búsqueda optimizada configurada exitosamente.';
PRINT N'========================================';
PRINT N'';
PRINT N'Se crearon los siguientes procedimientos:';
PRINT N'1. sp_Libro_BuscarPorCriterios - Búsqueda general con filtros';
PRINT N'2. sp_Libro_BusquedaRapida - Para autocompletado';
PRINT N'3. sp_Libro_BusquedaAvanzada - Búsqueda multi-palabra';
PRINT N'';
PRINT N'Índices optimizados creados para mejorar performance de LIKE.';
PRINT N'========================================';
GO
