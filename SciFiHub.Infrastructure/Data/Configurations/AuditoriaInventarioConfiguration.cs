using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SciFiHub.Domain.Entities;

namespace SciFiHub.Infrastructure.Data.Configurations;

public class AuditoriaInventarioConfiguration : IEntityTypeConfiguration<AuditoriaInventario>
{
    public void Configure(EntityTypeBuilder<AuditoriaInventario> builder)
    {
        // ? CORREGIDO: Tabla en SINGULAR como está en BD
        builder.ToTable("AuditoriaInventario");

        builder.HasKey(a => a.Id);

        // ?? COLUMNAS QUE NO EXISTEN EN BD - IGNORAR
        builder.Ignore(a => a.CreatedAt);
        builder.Ignore(a => a.UpdatedAt);
        builder.Ignore(a => a.IsDeleted);

        // ? COLUMNAS QUE SÍ EXISTEN EN BD
        builder.Property(a => a.CreatedBy)
            .IsRequired(false);

        builder.Property(a => a.UpdatedBy)
            .IsRequired(false);

        builder.Property(a => a.DeletedAt)
            .IsRequired(false);

        // ? Configuración de enums
        builder.Property(a => a.TipoMovimiento)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(a => a.Motivo)
            .HasMaxLength(500);

        // Índices
        builder.HasIndex(a => a.LibroId)
            .HasDatabaseName("IX_AuditoriaInventario_LibroId");

        builder.HasIndex(a => a.FechaMovimiento)
            .HasDatabaseName("IX_AuditoriaInventario_FechaMovimiento");

        builder.HasIndex(a => a.TipoMovimiento)
            .HasDatabaseName("IX_AuditoriaInventario_TipoMovimiento");

        // Relaciones
        builder.HasOne(a => a.Libro)
            .WithMany(l => l.AuditoriasInventario)
            .HasForeignKey(a => a.LibroId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Usuario)
            .WithMany()
            .HasForeignKey(a => a.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
