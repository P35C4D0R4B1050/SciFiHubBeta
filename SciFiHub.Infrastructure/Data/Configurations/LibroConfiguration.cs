using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SciFiHub.Domain.Entities;

namespace SciFiHub.Infrastructure.Data.Configurations;

public class LibroConfiguration : IEntityTypeConfiguration<Libro>
{
    public void Configure(EntityTypeBuilder<Libro> builder)
    {
        builder.ToTable("Libros");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.ISBN)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(l => l.Titulo)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(l => l.Autor)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(l => l.Editorial)
            .HasMaxLength(150);

        builder.Property(l => l.Sinopsis)
            .HasMaxLength(1000);

        builder.Property(l => l.Descripcion)
            .HasColumnType("NVARCHAR(MAX)");

        builder.Property(l => l.Precio)
            .HasColumnType("DECIMAL(18,2)");

        builder.Property(l => l.PrecioOferta)
            .HasColumnType("DECIMAL(18,2)");

        builder.Property(l => l.ImagenPortada)
            .HasMaxLength(500);

        builder.Property(l => l.ImagenesSecundarias)
            .HasColumnType("NVARCHAR(MAX)");

        builder.Property(l => l.Peso)
            .HasColumnType("DECIMAL(10,2)");

        builder.Property(l => l.Dimensiones)
            .HasMaxLength(50);

        builder.Property(l => l.Idioma)
            .HasMaxLength(50)
            .HasDefaultValue("Español");

        // Índices
        builder.HasIndex(l => l.ISBN)
            .HasDatabaseName("IX_Libros_ISBN")
            .IsUnique();

        builder.HasIndex(l => l.Titulo)
            .HasDatabaseName("IX_Libros_Titulo");

        builder.HasIndex(l => l.Autor)
            .HasDatabaseName("IX_Libros_Autor");

        builder.HasIndex(l => l.CategoriaId)
            .HasDatabaseName("IX_Libros_CategoriaId");

        builder.HasIndex(l => l.Destacado)
            .HasDatabaseName("IX_Libros_Destacado");

        // Propiedades ignoradas (calculadas)
        builder.Ignore(l => l.PrecioFinal);
        builder.Ignore(l => l.TieneOferta);
        builder.Ignore(l => l.PorcentajeDescuento);
        builder.Ignore(l => l.EstaDisponible);
        builder.Ignore(l => l.EstadoStock);

        // Relaciones
        builder.HasOne(l => l.Categoria)
            .WithMany(c => c.Libros)
            .HasForeignKey(l => l.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
