using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SciFiHub.Domain.Entities;

namespace SciFiHub.Infrastructure.Data.Configurations;

public class CarritoCompraConfiguration : IEntityTypeConfiguration<CarritoCompra>
{
    public void Configure(EntityTypeBuilder<CarritoCompra> builder)
    {
        builder.ToTable("CarritoCompras");

        builder.HasKey(c => c.Id);

        // ⚠️ COLUMNAS QUE NO EXISTEN EN BD - IGNORAR
        builder.Ignore(c => c.CreatedAt);
        builder.Ignore(c => c.UpdatedAt);

        // ✅ COLUMNAS QUE SÍ EXISTEN EN BD
        builder.Property(c => c.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(c => c.CreatedBy)
            .IsRequired(false);

        builder.Property(c => c.UpdatedBy)
            .IsRequired(false);

        builder.Property(c => c.DeletedAt)
            .IsRequired(false);

        // ✅ Configuración de enums
        builder.Property(c => c.Estado)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(20);

        // Índices
        builder.HasIndex(c => new { c.ClienteId, c.Estado })
            .HasDatabaseName("IX_CarritoCompras_ClienteId_Activo")
            .IsUnique()
            .HasFilter("[Estado] = 'Activo'");

        builder.HasIndex(c => c.Estado)
            .HasDatabaseName("IX_CarritoCompras_Estado");

        // Propiedades ignoradas (calculadas)
        builder.Ignore(c => c.CantidadTotalItems);
        builder.Ignore(c => c.SubtotalCarrito);
        builder.Ignore(c => c.EstaVacio);

        // Relaciones
        builder.HasMany(c => c.Detalles)
            .WithOne(d => d.Carrito)
            .HasForeignKey(d => d.CarritoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
