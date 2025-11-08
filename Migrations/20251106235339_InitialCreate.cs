using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SciFiHub.Web.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "Categorias",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CategoriaPadreId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Categorias_Categorias_CategoriaPadreId",
                        column: x => x.CategoriaPadreId,
                        principalSchema: "dbo",
                        principalTable: "Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NombreCompleto = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Rol = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UltimoAcceso = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Avatar = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Telefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Libros",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ISBN = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Autor = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Editorial = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    CategoriaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AnioPublicacion = table.Column<int>(type: "int", nullable: true),
                    Sinopsis = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Descripcion = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    Precio = table.Column<decimal>(type: "DECIMAL(18,2)", nullable: false),
                    PrecioOferta = table.Column<decimal>(type: "DECIMAL(18,2)", nullable: true),
                    Stock = table.Column<int>(type: "int", nullable: false),
                    ImagenPortada = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ImagenesSecundarias = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    Peso = table.Column<decimal>(type: "DECIMAL(10,2)", nullable: true),
                    Dimensiones = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NumeroPaginas = table.Column<int>(type: "int", nullable: true),
                    Idioma = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Español"),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Destacado = table.Column<bool>(type: "bit", nullable: false),
                    FechaIngreso = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Libros", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Libros_Categorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalSchema: "dbo",
                        principalTable: "Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarritoCompras",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClienteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarritoCompras", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarritoCompras_Usuarios_ClienteId",
                        column: x => x.ClienteId,
                        principalSchema: "dbo",
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Ventas",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NumeroVenta = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ClienteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VendedorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FechaVenta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Subtotal = table.Column<decimal>(type: "DECIMAL(18,2)", nullable: false),
                    Descuento = table.Column<decimal>(type: "DECIMAL(18,2)", nullable: false),
                    IGV = table.Column<decimal>(type: "DECIMAL(18,2)", nullable: false),
                    Total = table.Column<decimal>(type: "DECIMAL(18,2)", nullable: false),
                    EstadoVenta = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MetodoPago = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    DireccionCalle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DireccionCiudad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DireccionDepartamento = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DireccionCodigoPostal = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    DireccionPais = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DireccionReferencia = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    NotasVenta = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ventas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ventas_Usuarios_ClienteId",
                        column: x => x.ClienteId,
                        principalSchema: "dbo",
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ventas_Usuarios_VendedorId",
                        column: x => x.VendedorId,
                        principalSchema: "dbo",
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AuditoriaInventario",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LibroId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TipoMovimiento = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StockAnterior = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    StockNuevo = table.Column<int>(type: "int", nullable: false),
                    Motivo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ReferenciaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FechaMovimiento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditoriaInventario", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditoriaInventario_Libros_LibroId",
                        column: x => x.LibroId,
                        principalSchema: "dbo",
                        principalTable: "Libros",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AuditoriaInventario_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalSchema: "dbo",
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DetallesCarrito",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CarritoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LibroId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "DECIMAL(18,2)", nullable: false),
                    FechaAgregado = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetallesCarrito", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetallesCarrito_CarritoCompras_CarritoId",
                        column: x => x.CarritoId,
                        principalSchema: "dbo",
                        principalTable: "CarritoCompras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DetallesCarrito_Libros_LibroId",
                        column: x => x.LibroId,
                        principalSchema: "dbo",
                        principalTable: "Libros",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DetallesVenta",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VentaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LibroId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "DECIMAL(18,2)", nullable: false),
                    Descuento = table.Column<decimal>(type: "DECIMAL(18,2)", nullable: false),
                    Subtotal = table.Column<decimal>(type: "DECIMAL(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetallesVenta", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetallesVenta_Libros_LibroId",
                        column: x => x.LibroId,
                        principalSchema: "dbo",
                        principalTable: "Libros",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DetallesVenta_Ventas_VentaId",
                        column: x => x.VentaId,
                        principalSchema: "dbo",
                        principalTable: "Ventas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditoriaInventario_FechaMovimiento",
                schema: "dbo",
                table: "AuditoriaInventario",
                column: "FechaMovimiento");

            migrationBuilder.CreateIndex(
                name: "IX_AuditoriaInventario_LibroId_Fecha",
                schema: "dbo",
                table: "AuditoriaInventario",
                columns: new[] { "LibroId", "FechaMovimiento" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditoriaInventario_TipoMovimiento",
                schema: "dbo",
                table: "AuditoriaInventario",
                column: "TipoMovimiento");

            migrationBuilder.CreateIndex(
                name: "IX_AuditoriaInventario_UsuarioId",
                schema: "dbo",
                table: "AuditoriaInventario",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_CarritoCompras_ClienteId_Activo",
                schema: "dbo",
                table: "CarritoCompras",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_CarritoCompras_Estado",
                schema: "dbo",
                table: "CarritoCompras",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_Categorias_CategoriaPadreId",
                schema: "dbo",
                table: "Categorias",
                column: "CategoriaPadreId");

            migrationBuilder.CreateIndex(
                name: "UK_Categorias_Nombre",
                schema: "dbo",
                table: "Categorias",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DetallesCarrito_CarritoId",
                schema: "dbo",
                table: "DetallesCarrito",
                column: "CarritoId");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesCarrito_LibroId",
                schema: "dbo",
                table: "DetallesCarrito",
                column: "LibroId");

            migrationBuilder.CreateIndex(
                name: "UK_DetallesCarrito_CarritoLibro",
                schema: "dbo",
                table: "DetallesCarrito",
                columns: new[] { "CarritoId", "LibroId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DetallesVenta_LibroId",
                schema: "dbo",
                table: "DetallesVenta",
                column: "LibroId");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesVenta_VentaId",
                schema: "dbo",
                table: "DetallesVenta",
                column: "VentaId");

            migrationBuilder.CreateIndex(
                name: "IX_Libros_Autor",
                schema: "dbo",
                table: "Libros",
                column: "Autor");

            migrationBuilder.CreateIndex(
                name: "IX_Libros_CategoriaId",
                schema: "dbo",
                table: "Libros",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Libros_Destacado",
                schema: "dbo",
                table: "Libros",
                column: "Destacado");

            migrationBuilder.CreateIndex(
                name: "IX_Libros_ISBN",
                schema: "dbo",
                table: "Libros",
                column: "ISBN",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Libros_Titulo",
                schema: "dbo",
                table: "Libros",
                column: "Titulo");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Email",
                schema: "dbo",
                table: "Usuarios",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Rol",
                schema: "dbo",
                table: "Usuarios",
                column: "Rol");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Username",
                schema: "dbo",
                table: "Usuarios",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_ClienteId",
                schema: "dbo",
                table: "Ventas",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_FechaVenta",
                schema: "dbo",
                table: "Ventas",
                column: "FechaVenta");

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_VendedorId",
                schema: "dbo",
                table: "Ventas",
                column: "VendedorId");

            migrationBuilder.CreateIndex(
                name: "UK_Ventas_NumeroVenta",
                schema: "dbo",
                table: "Ventas",
                column: "NumeroVenta",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditoriaInventario",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "DetallesCarrito",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "DetallesVenta",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "CarritoCompras",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Libros",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Ventas",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Categorias",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Usuarios",
                schema: "dbo");
        }
    }
}
