using SciFiHub.Domain.Common;

namespace SciFiHub.Domain.Entities;

/// <summary>
/// Entidad Usuario - Representa usuarios del sistema (Administradores, Vendedores, Clientes)
/// </summary>
public class Usuario : BaseEntity
{
    public string NombreCompleto { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Rol { get; set; } = "Cliente"; // Cambio: String en lugar de Enum para coincidir con BD
    public DateTime FechaRegistro { get; set; }
    public DateTime? UltimoAcceso { get; set; }
    public string Estado { get; set; } = "Activo"; // Cambio: String en lugar de Enum para coincidir con BD
    public string? Avatar { get; set; }
    public string? Telefono { get; set; }

    // Relaciones de navegación
    public virtual ICollection<Venta> VentasComoCliente { get; set; } = new List<Venta>();
    public virtual ICollection<Venta> VentasComoVendedor { get; set; } = new List<Venta>();
    public virtual ICollection<CarritoCompra> Carritos { get; set; } = new List<CarritoCompra>();
    public virtual ICollection<AuditoriaInventario> AuditoriasInventario { get; set; } = new List<AuditoriaInventario>();

    // Métodos de dominio
    public bool EsAdministrador() => Rol == "Administrador";
    public bool EsVendedor() => Rol == "Vendedor";
    public bool EsCliente() => Rol == "Cliente";
    public bool EstaActivo() => Estado == "Activo" && !IsDeleted;

    public void Activar()
    {
        Estado = "Activo";
        UpdatedAt = DateTime.UtcNow;
    }

    public void Desactivar()
    {
        Estado = "Inactivo";
        UpdatedAt = DateTime.UtcNow;
    }

    public void ActualizarUltimoAcceso()
    {
        UltimoAcceso = DateTime.UtcNow;
    }
}
