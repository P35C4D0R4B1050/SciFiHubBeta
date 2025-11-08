using Microsoft.EntityFrameworkCore;
using SciFiHub.Domain.Entities;
using SciFiHub.Domain.Enums;

namespace SciFiHub.Infrastructure.Data;

/// <summary>
/// Contexto de base de datos principal de SciFiHub
/// </summary>
public class SciFiHubDbContext : DbContext
{
    public SciFiHubDbContext(DbContextOptions<SciFiHubDbContext> options) : base(options)
    {
    }

    // DbSets
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Libro> Libros => Set<Libro>();
    public DbSet<Venta> Ventas => Set<Venta>();
    public DbSet<DetalleVenta> DetallesVenta => Set<DetalleVenta>();
    public DbSet<CarritoCompra> CarritoCompras => Set<CarritoCompra>();
    public DbSet<DetalleCarrito> DetallesCarrito => Set<DetalleCarrito>();
    public DbSet<AuditoriaInventario> AuditoriasInventario => Set<AuditoriaInventario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configurar esquema por defecto
        modelBuilder.HasDefaultSchema("dbo");

        // Aplicar configuraciones de entidades
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SciFiHubDbContext).Assembly);

        // Configurar conversiones de enums
        ConfigurarEnums(modelBuilder);

        // Configurar filtros globales para soft delete
        ConfigurarFiltrosGlobales(modelBuilder);
        
        // Deshabilitar OUTPUT clause para tablas con triggers
        ConfigurarTriggersCompatibilidad(modelBuilder);
    }
    
    private void ConfigurarEnums(ModelBuilder modelBuilder)
    {
        // Convertir enums a strings en la base de datos
        modelBuilder.Entity<Usuario>()
            .Property(u => u.Rol)
            .HasConversion<string>()
            .HasMaxLength(20);

        modelBuilder.Entity<Usuario>()
            .Property(u => u.Estado)
            .HasConversion<string>()
            .HasMaxLength(20);

        modelBuilder.Entity<Venta>()
            .Property(v => v.EstadoVenta)
            .HasConversion<string>()
            .HasMaxLength(20);

        modelBuilder.Entity<Venta>()
            .Property(v => v.MetodoPago)
            .HasConversion<string>()
            .HasMaxLength(30);

        modelBuilder.Entity<Libro>()
            .Property(l => l.Estado)
            .HasConversion<string>()
            .HasMaxLength(20);

        modelBuilder.Entity<CarritoCompra>()
            .Property(c => c.Estado)
            .HasConversion<string>()
            .HasMaxLength(20);

        modelBuilder.Entity<AuditoriaInventario>()
            .Property(a => a.TipoMovimiento)
            .HasConversion<string>()
            .HasMaxLength(50);

        modelBuilder.Entity<Categoria>()
            .Property(c => c.Estado)
            .HasConversion<string>()
            .HasMaxLength(20);
    }

    private void ConfigurarFiltrosGlobales(ModelBuilder modelBuilder)
    {
        // ✅ Filtro global para soft delete en entidades que TIENEN IsDeleted en BD
        modelBuilder.Entity<Usuario>().HasQueryFilter(u => !u.IsDeleted);
        modelBuilder.Entity<Libro>().HasQueryFilter(l => !l.IsDeleted);
        modelBuilder.Entity<Venta>().HasQueryFilter(v => !v.IsDeleted);
        modelBuilder.Entity<Categoria>().HasQueryFilter(c => !c.IsDeleted);
        modelBuilder.Entity<DetalleVenta>().HasQueryFilter(d => !d.IsDeleted);
        modelBuilder.Entity<CarritoCompra>().HasQueryFilter(c => !c.IsDeleted);
        
        // ⚠️ NO configurar filtros para entidades SIN IsDeleted en BD:
        // - DetalleCarrito (NO tiene IsDeleted)
        // - AuditoriaInventario (NO tiene IsDeleted)
    }

    private void ConfigurarTriggersCompatibilidad(ModelBuilder modelBuilder)
    {
        // Deshabilitar OUTPUT clause para tablas que tienen triggers
        // Esto soluciona el error: "Could not save changes because the target table has database triggers"
        modelBuilder.Entity<Usuario>()
            .ToTable(tb => tb.UseSqlOutputClause(false));
            
        modelBuilder.Entity<Libro>()
            .ToTable(tb => tb.UseSqlOutputClause(false));
            
        modelBuilder.Entity<Venta>()
            .ToTable(tb => tb.UseSqlOutputClause(false));
            
        modelBuilder.Entity<DetalleVenta>()
            .ToTable(tb => tb.UseSqlOutputClause(false));
            
        modelBuilder.Entity<AuditoriaInventario>()
            .ToTable(tb => tb.UseSqlOutputClause(false));
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Establecer automáticamente CreatedAt y UpdatedAt SOLO si las propiedades existen y no están ignoradas
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is Domain.Common.BaseEntity &&
                       (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            var entity = (Domain.Common.BaseEntity)entry.Entity;

            try
            {
                // Intentar actualizar CreatedAt si la propiedad existe en el modelo
                if (entry.State == EntityState.Added)
                {
                    var createdAtProperty = entry.Properties.FirstOrDefault(p => p.Metadata.Name == nameof(Domain.Common.BaseEntity.CreatedAt));
                    if (createdAtProperty != null && !createdAtProperty.Metadata.IsShadowProperty())
                    {
                        entity.CreatedAt = DateTime.UtcNow;
                    }
                }
                // Intentar actualizar UpdatedAt si la propiedad existe en el modelo
                else if (entry.State == EntityState.Modified)
                {
                    var updatedAtProperty = entry.Properties.FirstOrDefault(p => p.Metadata.Name == nameof(Domain.Common.BaseEntity.UpdatedAt));
                    if (updatedAtProperty != null && !updatedAtProperty.Metadata.IsShadowProperty())
                    {
                        entity.UpdatedAt = DateTime.UtcNow;
                    }
                }
            }
            catch
            {
                // Si hay error al acceder a las propiedades, simplemente continuar
                // Esto maneja casos donde la propiedad está ignorada
                continue;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
