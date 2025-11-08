-- =============================================
-- SciFiHub Database - Creación de Tablas
-- SQL Server 2021+
-- Versión: 1.0
-- =============================================

USE SciFiHubDB;
GO

PRINT N'Iniciando creación de tablas...';
GO

-- =============================================
-- B. TABLAS CON MEJORES PRÁCTICAS EMPRESARIALES
-- =============================================

-- Tabla: Usuarios
-- Descripción: Almacena todos los usuarios del sistema (Administradores, Vendedores, Clientes)
-- =============================================
CREATE TABLE dbo.Usuarios (
    Id UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    NombreCompleto NVARCHAR(200) NOT NULL,
    Email NVARCHAR(256) NOT NULL,
    Username NVARCHAR(100) NOT NULL,
    PasswordHash NVARCHAR(500) NOT NULL,
    Rol NVARCHAR(20) NOT NULL, -- Administrador, Vendedor, Cliente
    FechaRegistro DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    UltimoAcceso DATETIME2(7) NULL,
    Estado NVARCHAR(20) NOT NULL DEFAULT 'Activo', -- Activo, Inactivo
    Avatar NVARCHAR(500) NULL,
    Telefono NVARCHAR(20) NULL,
    -- Campos de auditoría
    CreatedAt DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2(7) NULL,
    CreatedBy UNIQUEIDENTIFIER NULL,
    UpdatedBy UNIQUEIDENTIFIER NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    DeletedAt DATETIME2(7) NULL,
    -- Constraints
    CONSTRAINT PK_Usuarios PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT UK_Usuarios_Email UNIQUE (Email),
    CONSTRAINT UK_Usuarios_Username UNIQUE (Username),
    CONSTRAINT CK_Usuarios_Rol CHECK (Rol IN ('Administrador', 'Vendedor', 'Cliente')),
    CONSTRAINT CK_Usuarios_Estado CHECK (Estado IN ('Activo', 'Inactivo')),
    CONSTRAINT CK_Usuarios_Email CHECK (Email LIKE '%_@__%.__%')
);
GO

-- Índices para Usuarios
CREATE NONCLUSTERED INDEX IX_Usuarios_Email ON dbo.Usuarios(Email) WHERE IsDeleted = 0;
CREATE NONCLUSTERED INDEX IX_Usuarios_Username ON dbo.Usuarios(Username) WHERE IsDeleted = 0;
CREATE NONCLUSTERED INDEX IX_Usuarios_Rol ON dbo.Usuarios(Rol) INCLUDE (NombreCompleto, Email) WHERE IsDeleted = 0;
CREATE NONCLUSTERED INDEX IX_Usuarios_Estado ON dbo.Usuarios(Estado, Rol) WHERE IsDeleted = 0;
GO

PRINT N'Tabla Usuarios creada exitosamente.';
GO

-- =============================================
-- Tabla: Categorias
-- Descripción: Clasificación jerárquica de géneros literarios
-- =============================================
CREATE TABLE dbo.Categorias (
    Id UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    Nombre NVARCHAR(100) NOT NULL,
    Descripcion NVARCHAR(500) NULL,
    CategoriaPadreId UNIQUEIDENTIFIER NULL, -- Para jerarquía
    Orden INT NOT NULL DEFAULT 0,
    Estado NVARCHAR(20) NOT NULL DEFAULT 'Activo',
    -- Campos de auditoría
    CreatedAt DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2(7) NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    -- Constraints
    CONSTRAINT PK_Categorias PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT UK_Categorias_Nombre UNIQUE (Nombre),
    CONSTRAINT FK_Categorias_CategoriaPadre FOREIGN KEY (CategoriaPadreId) REFERENCES dbo.Categorias(Id),
    CONSTRAINT CK_Categorias_Estado CHECK (Estado IN ('Activo', 'Inactivo'))
);
GO

CREATE NONCLUSTERED INDEX IX_Categorias_CategoriaPadreId ON dbo.Categorias(CategoriaPadreId) WHERE IsDeleted = 0;
CREATE NONCLUSTERED INDEX IX_Categorias_Estado ON dbo.Categorias(Estado, Orden) WHERE IsDeleted = 0;
GO

PRINT N'Tabla Categorias creada exitosamente.';
GO

-- =============================================
-- Tabla: Libros
-- Descripción: Catálogo completo de libros
-- =============================================
CREATE TABLE dbo.Libros (
    Id UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    ISBN NVARCHAR(20) NOT NULL,
    Titulo NVARCHAR(300) NOT NULL,
    Autor NVARCHAR(200) NOT NULL,
    Editorial NVARCHAR(150) NULL,
    CategoriaId UNIQUEIDENTIFIER NULL,
    AnioPublicacion INT NULL,
    Sinopsis NVARCHAR(1000) NULL,
    Descripcion NVARCHAR(MAX) NULL,
    Precio DECIMAL(18, 2) NOT NULL,
    PrecioOferta DECIMAL(18, 2) NULL,
    Stock INT NOT NULL DEFAULT 0,
    ImagenPortada NVARCHAR(500) NULL,
    ImagenesSecundarias NVARCHAR(MAX) NULL, -- JSON array de URLs
    Peso DECIMAL(10, 2) NULL, -- en kilogramos
    Dimensiones NVARCHAR(50) NULL, -- Formato: "Alto x Ancho x Largo cm"
    NumeroPaginas INT NULL,
    Idioma NVARCHAR(50) NULL DEFAULT 'Español',
    Estado NVARCHAR(20) NOT NULL DEFAULT 'Disponible', -- Disponible, Agotado, Descontinuado
    Destacado BIT NOT NULL DEFAULT 0,
    -- Campos de auditoría
    FechaIngreso DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    CreatedAt DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2(7) NULL,
    CreatedBy UNIQUEIDENTIFIER NULL,
    UpdatedBy UNIQUEIDENTIFIER NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    DeletedAt DATETIME2(7) NULL,
    -- Constraints
    CONSTRAINT PK_Libros PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT UK_Libros_ISBN UNIQUE (ISBN),
    CONSTRAINT FK_Libros_Categoria FOREIGN KEY (CategoriaId) REFERENCES dbo.Categorias(Id),
    CONSTRAINT CK_Libros_Precio CHECK (Precio >= 0),
    CONSTRAINT CK_Libros_PrecioOferta CHECK (PrecioOferta IS NULL OR PrecioOferta >= 0),
    CONSTRAINT CK_Libros_Stock CHECK (Stock >= 0),
    CONSTRAINT CK_Libros_Estado CHECK (Estado IN ('Disponible', 'Agotado', 'Descontinuado')),
    CONSTRAINT CK_Libros_AnioPublicacion CHECK (AnioPublicacion IS NULL OR (AnioPublicacion >= 1800 AND AnioPublicacion <= YEAR(GETDATE()) + 1))
);
GO

-- Índices estratégicos para Libros
CREATE NONCLUSTERED INDEX IX_Libros_ISBN ON dbo.Libros(ISBN) WHERE IsDeleted = 0;
CREATE NONCLUSTERED INDEX IX_Libros_Titulo ON dbo.Libros(Titulo) WHERE IsDeleted = 0;
CREATE NONCLUSTERED INDEX IX_Libros_Autor ON dbo.Libros(Autor) INCLUDE (Titulo, Precio) WHERE IsDeleted = 0;
CREATE NONCLUSTERED INDEX IX_Libros_CategoriaId ON dbo.Libros(CategoriaId) INCLUDE (Titulo, Autor, Precio, Stock) WHERE IsDeleted = 0;
CREATE NONCLUSTERED INDEX IX_Libros_Precio ON dbo.Libros(Precio) INCLUDE (Titulo, Autor, Stock) WHERE IsDeleted = 0;
CREATE NONCLUSTERED INDEX IX_Libros_Stock ON dbo.Libros(Stock) WHERE Stock > 0 AND IsDeleted = 0;
CREATE NONCLUSTERED INDEX IX_Libros_Destacado ON dbo.Libros(Destacado) INCLUDE (Titulo, Autor, Precio, ImagenPortada) WHERE Destacado = 1 AND IsDeleted = 0;
CREATE NONCLUSTERED INDEX IX_Libros_FechaIngreso ON dbo.Libros(FechaIngreso DESC) INCLUDE (Titulo, Autor, Precio, ImagenPortada) WHERE IsDeleted = 0;
CREATE NONCLUSTERED INDEX IX_Libros_Estado_Stock ON dbo.Libros(Estado, Stock) INCLUDE (Titulo, Precio) WHERE IsDeleted = 0;
GO

PRINT N'Tabla Libros creada exitosamente.';
GO

-- =============================================
-- Tabla: Ventas
-- Descripción: Registro de todas las ventas realizadas
-- =============================================
CREATE TABLE dbo.Ventas (
    Id UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    NumeroVenta NVARCHAR(50) NOT NULL,
    ClienteId UNIQUEIDENTIFIER NOT NULL,
    VendedorId UNIQUEIDENTIFIER NULL, -- NULL para ventas web
    FechaVenta DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    Subtotal DECIMAL(18, 2) NOT NULL,
    Descuento DECIMAL(18, 2) NOT NULL DEFAULT 0,
    IGV DECIMAL(18, 2) NOT NULL DEFAULT 0, -- 18% en Perú
    Total DECIMAL(18, 2) NOT NULL,
    EstadoVenta NVARCHAR(20) NOT NULL DEFAULT 'Pendiente', -- Pendiente, Procesando, Completada, Cancelada, Reembolsada
    MetodoPago NVARCHAR(30) NOT NULL, -- Efectivo, TarjetaCredito, TarjetaDebito, Yape, Plin, Transferencia
    -- Dirección de envío (Value Object como columnas separadas)
    DireccionCalle NVARCHAR(200) NULL,
    DireccionCiudad NVARCHAR(100) NULL,
    DireccionDepartamento NVARCHAR(100) NULL,
    DireccionCodigoPostal NVARCHAR(10) NULL,
    DireccionPais NVARCHAR(50) NULL DEFAULT 'Perú',
    DireccionReferencia NVARCHAR(300) NULL,
    NotasVenta NVARCHAR(1000) NULL,
    -- Campos de auditoría
    CreatedAt DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2(7) NULL,
    CreatedBy UNIQUEIDENTIFIER NULL,
    UpdatedBy UNIQUEIDENTIFIER NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    DeletedAt DATETIME2(7) NULL,
    -- Constraints
    CONSTRAINT PK_Ventas PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT UK_Ventas_NumeroVenta UNIQUE (NumeroVenta),
    CONSTRAINT FK_Ventas_Cliente FOREIGN KEY (ClienteId) REFERENCES dbo.Usuarios(Id),
    CONSTRAINT FK_Ventas_Vendedor FOREIGN KEY (VendedorId) REFERENCES dbo.Usuarios(Id),
    CONSTRAINT CK_Ventas_Subtotal CHECK (Subtotal >= 0),
    CONSTRAINT CK_Ventas_Descuento CHECK (Descuento >= 0),
    CONSTRAINT CK_Ventas_Total CHECK (Total >= 0),
    CONSTRAINT CK_Ventas_EstadoVenta CHECK (EstadoVenta IN ('Pendiente', 'Procesando', 'Completada', 'Cancelada', 'Reembolsada')),
    CONSTRAINT CK_Ventas_MetodoPago CHECK (MetodoPago IN ('Efectivo', 'TarjetaCredito', 'TarjetaDebito', 'Yape', 'Plin', 'Transferencia'))
);
GO

-- Índices para Ventas
CREATE NONCLUSTERED INDEX IX_Ventas_NumeroVenta ON dbo.Ventas(NumeroVenta) WHERE IsDeleted = 0;
CREATE NONCLUSTERED INDEX IX_Ventas_ClienteId ON dbo.Ventas(ClienteId) INCLUDE (NumeroVenta, FechaVenta, Total, EstadoVenta) WHERE IsDeleted = 0;
CREATE NONCLUSTERED INDEX IX_Ventas_VendedorId ON dbo.Ventas(VendedorId) INCLUDE (NumeroVenta, FechaVenta, Total) WHERE IsDeleted = 0 AND VendedorId IS NOT NULL;
CREATE NONCLUSTERED INDEX IX_Ventas_FechaVenta ON dbo.Ventas(FechaVenta DESC) INCLUDE (NumeroVenta, Total, EstadoVenta) WHERE IsDeleted = 0;
CREATE NONCLUSTERED INDEX IX_Ventas_EstadoVenta ON dbo.Ventas(EstadoVenta, FechaVenta DESC) WHERE IsDeleted = 0;
CREATE NONCLUSTERED INDEX IX_Ventas_MetodoPago ON dbo.Ventas(MetodoPago) INCLUDE (Total, FechaVenta) WHERE IsDeleted = 0;
GO

PRINT N'Tabla Ventas creada exitosamente.';
GO

-- =============================================
-- Tabla: DetallesVenta
-- Descripción: Líneas de detalle de cada venta
-- =============================================
CREATE TABLE dbo.DetallesVenta (
    Id UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    VentaId UNIQUEIDENTIFIER NOT NULL,
    LibroId UNIQUEIDENTIFIER NOT NULL,
    Cantidad INT NOT NULL,
    PrecioUnitario DECIMAL(18, 2) NOT NULL,
    Descuento DECIMAL(18, 2) NOT NULL DEFAULT 0,
    Subtotal DECIMAL(18, 2) NOT NULL,
    -- Campos de auditoría
    CreatedAt DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2(7) NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    -- Constraints
    CONSTRAINT PK_DetallesVenta PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT FK_DetallesVenta_Venta FOREIGN KEY (VentaId) REFERENCES dbo.Ventas(Id) ON DELETE CASCADE,
    CONSTRAINT FK_DetallesVenta_Libro FOREIGN KEY (LibroId) REFERENCES dbo.Libros(Id),
    CONSTRAINT CK_DetallesVenta_Cantidad CHECK (Cantidad > 0),
    CONSTRAINT CK_DetallesVenta_PrecioUnitario CHECK (PrecioUnitario >= 0),
    CONSTRAINT CK_DetallesVenta_Descuento CHECK (Descuento >= 0),
    CONSTRAINT CK_DetallesVenta_Subtotal CHECK (Subtotal >= 0)
);
GO

-- Índices para DetallesVenta
CREATE NONCLUSTERED INDEX IX_DetallesVenta_VentaId ON dbo.DetallesVenta(VentaId) INCLUDE (LibroId, Cantidad, PrecioUnitario, Subtotal);
CREATE NONCLUSTERED INDEX IX_DetallesVenta_LibroId ON dbo.DetallesVenta(LibroId) INCLUDE (Cantidad, PrecioUnitario);
GO

PRINT N'Tabla DetallesVenta creada exitosamente.';
GO

-- =============================================
-- Tabla: CarritoCompras
-- Descripción: Carritos de compra persistentes para clientes
-- =============================================
CREATE TABLE dbo.CarritoCompras (
    Id UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    ClienteId UNIQUEIDENTIFIER NOT NULL,
    FechaCreacion DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    FechaActualizacion DATETIME2(7) NULL,
    Estado NVARCHAR(20) NOT NULL DEFAULT 'Activo', -- Activo, Convertido, Abandonado
    -- Constraints
    CONSTRAINT PK_CarritoCompras PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT FK_CarritoCompras_Cliente FOREIGN KEY (ClienteId) REFERENCES dbo.Usuarios(Id),
    CONSTRAINT CK_CarritoCompras_Estado CHECK (Estado IN ('Activo', 'Convertido', 'Abandonado'))
);
GO

CREATE UNIQUE NONCLUSTERED INDEX IX_CarritoCompras_ClienteId_Activo ON dbo.CarritoCompras(ClienteId) WHERE Estado = 'Activo';
CREATE NONCLUSTERED INDEX IX_CarritoCompras_Estado ON dbo.CarritoCompras(Estado, FechaActualizacion);
GO

PRINT N'Tabla CarritoCompras creada exitosamente.';
GO

-- =============================================
-- Tabla: DetallesCarrito
-- Descripción: Items en cada carrito de compra
-- =============================================
CREATE TABLE dbo.DetallesCarrito (
    Id UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    CarritoId UNIQUEIDENTIFIER NOT NULL,
    LibroId UNIQUEIDENTIFIER NOT NULL,
    Cantidad INT NOT NULL,
    PrecioUnitario DECIMAL(18, 2) NOT NULL,
    FechaAgregado DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    -- Constraints
    CONSTRAINT PK_DetallesCarrito PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT FK_DetallesCarrito_Carrito FOREIGN KEY (CarritoId) REFERENCES dbo.CarritoCompras(Id) ON DELETE CASCADE,
    CONSTRAINT FK_DetallesCarrito_Libro FOREIGN KEY (LibroId) REFERENCES dbo.Libros(Id),
    CONSTRAINT CK_DetallesCarrito_Cantidad CHECK (Cantidad > 0),
    CONSTRAINT CK_DetallesCarrito_PrecioUnitario CHECK (PrecioUnitario >= 0),
    CONSTRAINT UK_DetallesCarrito_CarritoLibro UNIQUE (CarritoId, LibroId)
);
GO

CREATE NONCLUSTERED INDEX IX_DetallesCarrito_CarritoId ON dbo.DetallesCarrito(CarritoId) INCLUDE (LibroId, Cantidad, PrecioUnitario);
CREATE NONCLUSTERED INDEX IX_DetallesCarrito_LibroId ON dbo.DetallesCarrito(LibroId);
GO

PRINT N'Tabla DetallesCarrito creada exitosamente.';
GO

-- =============================================
-- Tabla: AuditoriaInventario
-- Descripción: Tracking de cambios en inventario de libros
-- =============================================
CREATE TABLE dbo.AuditoriaInventario (
    Id UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    LibroId UNIQUEIDENTIFIER NOT NULL,
    TipoMovimiento NVARCHAR(50) NOT NULL, -- Ingreso, Venta, Ajuste, Devolucion, Anulacion
    StockAnterior INT NOT NULL,
    Cantidad INT NOT NULL, -- Positivo para ingreso, negativo para salida
    StockNuevo INT NOT NULL,
    Motivo NVARCHAR(500) NULL,
    ReferenciaId UNIQUEIDENTIFIER NULL, -- Id de Venta, Compra, etc.
    UsuarioId UNIQUEIDENTIFIER NULL,
    FechaMovimiento DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    -- Constraints
    CONSTRAINT PK_AuditoriaInventario PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT FK_AuditoriaInventario_Libro FOREIGN KEY (LibroId) REFERENCES dbo.Libros(Id),
    CONSTRAINT FK_AuditoriaInventario_Usuario FOREIGN KEY (UsuarioId) REFERENCES dbo.Usuarios(Id),
    CONSTRAINT CK_AuditoriaInventario_TipoMovimiento CHECK (TipoMovimiento IN ('Ingreso', 'Venta', 'Ajuste', 'Devolucion', 'Anulacion'))
);
GO

CREATE NONCLUSTERED INDEX IX_AuditoriaInventario_LibroId ON dbo.AuditoriaInventario(LibroId, FechaMovimiento DESC);
CREATE NONCLUSTERED INDEX IX_AuditoriaInventario_FechaMovimiento ON dbo.AuditoriaInventario(FechaMovimiento DESC);
CREATE NONCLUSTERED INDEX IX_AuditoriaInventario_TipoMovimiento ON dbo.AuditoriaInventario(TipoMovimiento, FechaMovimiento DESC);
GO

PRINT N'Tabla AuditoriaInventario creada exitosamente.';
GO

PRINT N'Todas las tablas creadas exitosamente.';
GO
