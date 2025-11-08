using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SciFiHub.Domain.Entities;
using SciFiHub.Domain.Enums;

namespace SciFiHub.Infrastructure.Data.Configurations;

public class VentaConfiguration : IEntityTypeConfiguration<Venta>
{
    public void Configure(EntityTypeBuilder<Venta> builder)
    {
        builder.ToTable("Ventas");

        // ? CONFIGURACIÓN EXPLÍCITA DE PROPIEDADES DE BASEENTITY
        builder.Property(v => v.Id)
            .IsRequired();

        builder.HasKey(v => v.Id);

        builder.Property(v => v.CreatedAt)
            .IsRequired();

        builder.Property(v => v.UpdatedAt)
            .IsRequired(false);

        builder.Property(v => v.CreatedBy)
            .IsRequired(false);

        builder.Property(v => v.UpdatedBy)
            .IsRequired(false);

        builder.Property(v => v.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(v => v.DeletedAt)
            .IsRequired(false);

        // Propiedades específicas de Venta
        builder.Property(v => v.NumeroVenta)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(v => v.Subtotal)
            .HasColumnType("DECIMAL(18,2)");

        builder.Property(v => v.Descuento)
            .HasColumnType("DECIMAL(18,2)");

        builder.Property(v => v.IGV)
            .HasColumnType("DECIMAL(18,2)");

        builder.Property(v => v.Total)
            .HasColumnType("DECIMAL(18,2)");

        // ? CONFIGURACIÓN DE ENUMS COMO STRINGS
        builder.Property(v => v.EstadoVenta)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(v => v.MetodoPago)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(v => v.DireccionCalle)
            .HasMaxLength(200);

        builder.Property(v => v.DireccionCiudad)
            .HasMaxLength(100);

        builder.Property(v => v.DireccionDepartamento)
            .HasMaxLength(100);

        builder.Property(v => v.DireccionCodigoPostal)
            .HasMaxLength(10);

        builder.Property(v => v.DireccionPais)
            .HasMaxLength(50);

        builder.Property(v => v.DireccionReferencia)
            .HasMaxLength(300);

        builder.Property(v => v.NotasVenta)
            .HasMaxLength(1000);

        // Índices
        builder.HasIndex(v => v.NumeroVenta)
            .HasDatabaseName("UK_Ventas_NumeroVenta")
            .IsUnique();

        builder.HasIndex(v => v.ClienteId)
            .HasDatabaseName("IX_Ventas_ClienteId");

        builder.HasIndex(v => v.VendedorId)
            .HasDatabaseName("IX_Ventas_VendedorId");

        builder.HasIndex(v => v.FechaVenta)
            .HasDatabaseName("IX_Ventas_FechaVenta");

        // Propiedades ignoradas
        builder.Ignore(v => v.Direccion);
        builder.Ignore(v => v.CantidadTotalItems);

        // Relaciones
        builder.HasMany(v => v.Detalles)
            .WithOne(d => d.Venta)
            .HasForeignKey(d => d.VentaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
