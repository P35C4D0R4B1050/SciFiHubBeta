using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SciFiHub.Domain.Entities;

namespace SciFiHub.Infrastructure.Data.Configurations;

public class DetalleVentaConfiguration : IEntityTypeConfiguration<DetalleVenta>
{
    public void Configure(EntityTypeBuilder<DetalleVenta> builder)
    {
        builder.ToTable("DetallesVenta");

        builder.HasKey(d => d.Id);

        // ✅ TODAS las columnas de BaseEntity EXISTEN en BD
        builder.Property(d => d.CreatedAt)
            .IsRequired();

        builder.Property(d => d.UpdatedAt)
            .IsRequired(false);

        builder.Property(d => d.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(d => d.CreatedBy)
            .IsRequired(false);

        builder.Property(d => d.UpdatedBy)
            .IsRequired(false);

        builder.Property(d => d.DeletedAt)
            .IsRequired(false);

        // Propiedades específicas de DetalleVenta
        builder.Property(d => d.PrecioUnitario)
            .HasColumnType("DECIMAL(18,2)");

        builder.Property(d => d.Descuento)
            .HasColumnType("DECIMAL(18,2)");

        builder.Property(d => d.Subtotal)
            .HasColumnType("DECIMAL(18,2)");

        // Índices
        builder.HasIndex(d => d.VentaId)
            .HasDatabaseName("IX_DetallesVenta_VentaId");

        builder.HasIndex(d => d.LibroId)
            .HasDatabaseName("IX_DetallesVenta_LibroId");

        // Propiedades ignoradas (calculadas)
        builder.Ignore(d => d.SubtotalSinDescuento);

        // Relaciones
        builder.HasOne(d => d.Libro)
            .WithMany(l => l.DetallesVenta)
            .HasForeignKey(d => d.LibroId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
