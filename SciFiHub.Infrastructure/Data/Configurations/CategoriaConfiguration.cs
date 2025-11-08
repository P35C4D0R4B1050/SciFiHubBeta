using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SciFiHub.Domain.Entities;

namespace SciFiHub.Infrastructure.Data.Configurations;

public class CategoriaConfiguration : IEntityTypeConfiguration<Categoria>
{
    public void Configure(EntityTypeBuilder<Categoria> builder)
    {
        builder.ToTable("Categorias");

        builder.HasKey(c => c.Id);

        // ? TODAS las columnas de BaseEntity EXISTEN en BD
        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.UpdatedAt)
            .IsRequired(false);

        builder.Property(c => c.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(c => c.CreatedBy)
            .IsRequired(false);

        builder.Property(c => c.UpdatedBy)
            .IsRequired(false);

        builder.Property(c => c.DeletedAt)
            .IsRequired(false);

        // Propiedades específicas
        builder.Property(c => c.Nombre)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Descripcion)
            .HasMaxLength(500);

        builder.Property(c => c.Orden)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(c => c.Estado)
            .IsRequired()
            .HasMaxLength(20);

        // Índices
        builder.HasIndex(c => c.Nombre)
            .HasDatabaseName("UK_Categorias_Nombre")
            .IsUnique();

        builder.HasIndex(c => c.CategoriaPadreId)
            .HasDatabaseName("IX_Categorias_CategoriaPadreId")
            .HasFilter("[IsDeleted] = 0");

        builder.HasIndex(c => new { c.Estado, c.Orden })
            .HasDatabaseName("IX_Categorias_Estado")
            .HasFilter("[IsDeleted] = 0");

        // Relaciones - Autorelación para jerarquía
        builder.HasOne(c => c.CategoriaPadre)
            .WithMany(c => c.SubCategorias)
            .HasForeignKey(c => c.CategoriaPadreId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.SubCategorias)
            .WithOne(c => c.CategoriaPadre)
            .HasForeignKey(c => c.CategoriaPadreId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
