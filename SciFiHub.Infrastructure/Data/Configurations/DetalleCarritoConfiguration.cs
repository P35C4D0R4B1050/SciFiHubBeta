using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SciFiHub.Domain.Entities;

namespace SciFiHub.Infrastructure.Data.Configurations;

public class DetalleCarritoConfiguration : IEntityTypeConfiguration<DetalleCarrito>
{
    public void Configure(EntityTypeBuilder<DetalleCarrito> builder)
    {
        builder.ToTable("DetallesCarrito");

        builder.HasKey(d => d.Id);

        // ⚠️ COLUMNAS QUE NO EXISTEN EN BD - IGNORAR
        builder.Ignore(d => d.CreatedAt);
        builder.Ignore(d => d.UpdatedAt);
        builder.Ignore(d => d.IsDeleted);

        // ✅ COLUMNAS QUE SÍ EXISTEN EN BD
        builder.Property(d => d.CreatedBy)
            .IsRequired(false);

        builder.Property(d => d.UpdatedBy)
            .IsRequired(false);

        builder.Property(d => d.DeletedAt)
            .IsRequired(false);

        builder.Property(d => d.PrecioUnitario)
            .HasColumnType("DECIMAL(18,2)");

        // Índices
        builder.HasIndex(d => d.CarritoId)
            .HasDatabaseName("IX_DetallesCarrito_CarritoId");

        builder.HasIndex(d => d.LibroId)
            .HasDatabaseName("IX_DetallesCarrito_LibroId");

        builder.HasIndex(d => new { d.CarritoId, d.LibroId })
            .HasDatabaseName("UK_DetallesCarrito_CarritoLibro")
            .IsUnique();

        // Propiedades ignoradas (calculadas)
        builder.Ignore(d => d.Subtotal);

        // Relaciones
        builder.HasOne(d => d.Libro)
            .WithMany(l => l.DetallesCarrito)
            .HasForeignKey(d => d.LibroId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
