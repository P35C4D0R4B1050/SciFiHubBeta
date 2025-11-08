using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SciFiHub.Domain.Entities;

namespace SciFiHub.Infrastructure.Data.Configurations;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("Usuarios");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.NombreCompleto)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(u => u.Username)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(u => u.Rol)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("Cliente");

        builder.Property(u => u.Estado)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("Activo");

        builder.Property(u => u.Avatar)
            .HasMaxLength(500);

        builder.Property(u => u.Telefono)
            .HasMaxLength(20);

        // Índices
        builder.HasIndex(u => u.Email)
            .HasDatabaseName("IX_Usuarios_Email")
            .IsUnique();

        builder.HasIndex(u => u.Username)
            .HasDatabaseName("IX_Usuarios_Username")
            .IsUnique();

        builder.HasIndex(u => u.Rol)
            .HasDatabaseName("IX_Usuarios_Rol");

        // Relaciones
        builder.HasMany(u => u.VentasComoCliente)
            .WithOne(v => v.Cliente)
            .HasForeignKey(v => v.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.VentasComoVendedor)
            .WithOne(v => v.Vendedor)
            .HasForeignKey(v => v.VendedorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.Carritos)
            .WithOne(c => c.Cliente)
            .HasForeignKey(c => c.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
