using SciFiHub.Domain.Common;

namespace SciFiHub.Domain.Entities;

/// <summary>
/// Entidad Categoría - Clasificación jerárquica de géneros literarios
/// </summary>
public class Categoria : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public Guid? CategoriaPadreId { get; set; }
    public int Orden { get; set; }
    public string Estado { get; set; } = "Activo"; // Cambio: String en lugar de Enum para coincidir con BD

    // Relaciones de navegación
    public virtual Categoria? CategoriaPadre { get; set; }
    public virtual ICollection<Categoria> SubCategorias { get; set; } = new List<Categoria>();
    public virtual ICollection<Libro> Libros { get; set; } = new List<Libro>();

    // Métodos de dominio
    public bool EsCategoriaPrincipal() => CategoriaPadreId == null;
    public bool EstaActiva() => Estado == "Activo" && !IsDeleted;
}
