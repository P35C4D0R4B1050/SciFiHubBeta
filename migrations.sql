IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251106235339_InitialCreate'
)
BEGIN
    CREATE TABLE [dbo].[Categorias] (
        [Id] uniqueidentifier NOT NULL,
        [Nombre] nvarchar(100) NOT NULL,
        [Descripcion] nvarchar(500) NULL,
        [CategoriaPadreId] uniqueidentifier NULL,
        [Orden] int NOT NULL,
        [Estado] nvarchar(20) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedBy] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        CONSTRAINT [PK_Categorias] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Categorias_Categorias_CategoriaPadreId] FOREIGN KEY ([CategoriaPadreId]) REFERENCES [dbo].[Categorias] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251106235339_InitialCreate'
)
BEGIN
    CREATE TABLE [dbo].[Usuarios] (
        [Id] uniqueidentifier NOT NULL,
        [NombreCompleto] nvarchar(200) NOT NULL,
        [Email] nvarchar(256) NOT NULL,
        [Username] nvarchar(100) NOT NULL,
        [PasswordHash] nvarchar(500) NOT NULL,
        [Rol] nvarchar(20) NOT NULL,
        [FechaRegistro] datetime2 NOT NULL,
        [UltimoAcceso] datetime2 NULL,
        [Estado] nvarchar(20) NOT NULL,
        [Avatar] nvarchar(500) NULL,
        [Telefono] nvarchar(20) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedBy] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        CONSTRAINT [PK_Usuarios] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251106235339_InitialCreate'
)
BEGIN
    CREATE TABLE [dbo].[Libros] (
        [Id] uniqueidentifier NOT NULL,
        [ISBN] nvarchar(20) NOT NULL,
        [Titulo] nvarchar(300) NOT NULL,
        [Autor] nvarchar(200) NOT NULL,
        [Editorial] nvarchar(150) NULL,
        [CategoriaId] uniqueidentifier NULL,
        [AnioPublicacion] int NULL,
        [Sinopsis] nvarchar(1000) NULL,
        [Descripcion] NVARCHAR(MAX) NULL,
        [Precio] DECIMAL(18,2) NOT NULL,
        [PrecioOferta] DECIMAL(18,2) NULL,
        [Stock] int NOT NULL,
        [ImagenPortada] nvarchar(500) NULL,
        [ImagenesSecundarias] NVARCHAR(MAX) NULL,
        [Peso] DECIMAL(10,2) NULL,
        [Dimensiones] nvarchar(50) NULL,
        [NumeroPaginas] int NULL,
        [Idioma] nvarchar(50) NOT NULL DEFAULT N'Español',
        [Estado] nvarchar(20) NOT NULL,
        [Destacado] bit NOT NULL,
        [FechaIngreso] datetime2 NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedBy] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        CONSTRAINT [PK_Libros] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Libros_Categorias_CategoriaId] FOREIGN KEY ([CategoriaId]) REFERENCES [dbo].[Categorias] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251106235339_InitialCreate'
)
BEGIN
    CREATE TABLE [dbo].[CarritoCompras] (
        [Id] uniqueidentifier NOT NULL,
        [ClienteId] uniqueidentifier NOT NULL,
        [FechaCreacion] datetime2 NOT NULL,
        [FechaActualizacion] datetime2 NULL,
        [Estado] nvarchar(20) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedBy] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        CONSTRAINT [PK_CarritoCompras] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_CarritoCompras_Usuarios_ClienteId] FOREIGN KEY ([ClienteId]) REFERENCES [dbo].[Usuarios] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251106235339_InitialCreate'
)
BEGIN
    CREATE TABLE [dbo].[Ventas] (
        [Id] uniqueidentifier NOT NULL,
        [NumeroVenta] nvarchar(50) NOT NULL,
        [ClienteId] uniqueidentifier NOT NULL,
        [VendedorId] uniqueidentifier NULL,
        [FechaVenta] datetime2 NOT NULL,
        [Subtotal] DECIMAL(18,2) NOT NULL,
        [Descuento] DECIMAL(18,2) NOT NULL,
        [IGV] DECIMAL(18,2) NOT NULL,
        [Total] DECIMAL(18,2) NOT NULL,
        [EstadoVenta] nvarchar(20) NOT NULL,
        [MetodoPago] nvarchar(30) NOT NULL,
        [DireccionCalle] nvarchar(200) NULL,
        [DireccionCiudad] nvarchar(100) NULL,
        [DireccionDepartamento] nvarchar(100) NULL,
        [DireccionCodigoPostal] nvarchar(10) NULL,
        [DireccionPais] nvarchar(50) NULL,
        [DireccionReferencia] nvarchar(300) NULL,
        [NotasVenta] nvarchar(1000) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedBy] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        CONSTRAINT [PK_Ventas] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Ventas_Usuarios_ClienteId] FOREIGN KEY ([ClienteId]) REFERENCES [dbo].[Usuarios] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Ventas_Usuarios_VendedorId] FOREIGN KEY ([VendedorId]) REFERENCES [dbo].[Usuarios] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251106235339_InitialCreate'
)
BEGIN
    CREATE TABLE [dbo].[AuditoriaInventario] (
        [Id] uniqueidentifier NOT NULL,
        [LibroId] uniqueidentifier NOT NULL,
        [TipoMovimiento] nvarchar(50) NOT NULL,
        [StockAnterior] int NOT NULL,
        [Cantidad] int NOT NULL,
        [StockNuevo] int NOT NULL,
        [Motivo] nvarchar(500) NULL,
        [ReferenciaId] uniqueidentifier NULL,
        [UsuarioId] uniqueidentifier NULL,
        [FechaMovimiento] datetime2 NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedBy] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        CONSTRAINT [PK_AuditoriaInventario] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AuditoriaInventario_Libros_LibroId] FOREIGN KEY ([LibroId]) REFERENCES [dbo].[Libros] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_AuditoriaInventario_Usuarios_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [dbo].[Usuarios] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251106235339_InitialCreate'
)
BEGIN
    CREATE TABLE [dbo].[DetallesCarrito] (
        [Id] uniqueidentifier NOT NULL,
        [CarritoId] uniqueidentifier NOT NULL,
        [LibroId] uniqueidentifier NOT NULL,
        [Cantidad] int NOT NULL,
        [PrecioUnitario] DECIMAL(18,2) NOT NULL,
        [FechaAgregado] datetime2 NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedBy] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        CONSTRAINT [PK_DetallesCarrito] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_DetallesCarrito_CarritoCompras_CarritoId] FOREIGN KEY ([CarritoId]) REFERENCES [dbo].[CarritoCompras] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_DetallesCarrito_Libros_LibroId] FOREIGN KEY ([LibroId]) REFERENCES [dbo].[Libros] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251106235339_InitialCreate'
)
BEGIN
    CREATE TABLE [dbo].[DetallesVenta] (
        [Id] uniqueidentifier NOT NULL,
        [VentaId] uniqueidentifier NOT NULL,
        [LibroId] uniqueidentifier NOT NULL,
        [Cantidad] int NOT NULL,
        [PrecioUnitario] DECIMAL(18,2) NOT NULL,
        [Descuento] DECIMAL(18,2) NOT NULL,
        [Subtotal] DECIMAL(18,2) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [CreatedBy] uniqueidentifier NULL,
        [UpdatedBy] uniqueidentifier NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAt] datetime2 NULL,
        CONSTRAINT [PK_DetallesVenta] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_DetallesVenta_Libros_LibroId] FOREIGN KEY ([LibroId]) REFERENCES [dbo].[Libros] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_DetallesVenta_Ventas_VentaId] FOREIGN KEY ([VentaId]) REFERENCES [dbo].[Ventas] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251106235339_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AuditoriaInventario_FechaMovimiento] ON [dbo].[AuditoriaInventario] ([FechaMovimiento]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251106235339_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AuditoriaInventario_LibroId_Fecha] ON [dbo].[AuditoriaInventario] ([LibroId], [FechaMovimiento]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251106235339_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AuditoriaInventario_TipoMovimiento] ON [dbo].[AuditoriaInventario] ([TipoMovimiento]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251106235339_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AuditoriaInventario_UsuarioId] ON [dbo].[AuditoriaInventario] ([UsuarioId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251106235339_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_CarritoCompras_ClienteId_Activo] ON [dbo].[CarritoCompras] ([ClienteId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251106235339_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_CarritoCompras_Estado] ON [dbo].[CarritoCompras] ([Estado]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251106235339_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Categorias_CategoriaPadreId] ON [dbo].[Categorias] ([CategoriaPadreId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251106235339_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [UK_Categorias_Nombre] ON [dbo].[Categorias] ([Nombre]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251106235339_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_DetallesCarrito_CarritoId] ON [dbo].[DetallesCarrito] ([CarritoId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251106235339_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_DetallesCarrito_LibroId] ON [dbo].[DetallesCarrito] ([LibroId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251106235339_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [UK_DetallesCarrito_CarritoLibro] ON [dbo].[DetallesCarrito] ([CarritoId], [LibroId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251106235339_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_DetallesVenta_LibroId] ON [dbo].[DetallesVenta] ([LibroId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251106235339_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_DetallesVenta_VentaId] ON [dbo].[DetallesVenta] ([VentaId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251106235339_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Libros_Autor] ON [dbo].[Libros] ([Autor]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251106235339_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Libros_CategoriaId] ON [dbo].[Libros] ([CategoriaId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251106235339_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Libros_Destacado] ON [dbo].[Libros] ([Destacado]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251106235339_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Libros_ISBN] ON [dbo].[Libros] ([ISBN]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251106235339_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Libros_Titulo] ON [dbo].[Libros] ([Titulo]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251106235339_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Usuarios_Email] ON [dbo].[Usuarios] ([Email]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251106235339_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Usuarios_Rol] ON [dbo].[Usuarios] ([Rol]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251106235339_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Usuarios_Username] ON [dbo].[Usuarios] ([Username]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251106235339_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Ventas_ClienteId] ON [dbo].[Ventas] ([ClienteId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251106235339_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Ventas_FechaVenta] ON [dbo].[Ventas] ([FechaVenta]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251106235339_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Ventas_VendedorId] ON [dbo].[Ventas] ([VendedorId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251106235339_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [UK_Ventas_NumeroVenta] ON [dbo].[Ventas] ([NumeroVenta]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20251106235339_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251106235339_InitialCreate', N'9.0.10');
END;

COMMIT;
GO

