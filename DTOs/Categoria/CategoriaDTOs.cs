using System.ComponentModel.DataAnnotations;
using SciFiHub.Domain.Enums;

namespace SciFiHub.Web.DTOs.Categoria;

/// <summary>
/// DTO para crear una nueva categoría
/// </summary>
public record CrearCategoriaDTO
{
    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
    public string Nombre { get; init; } = string.Empty;

    [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
    public string? Descripcion { get; init; }

    public Guid? CategoriaPadreId { get; init; }

    [Range(0, 9999, ErrorMessage = "El orden debe estar entre 0 y 9999")]
    public int Orden { get; init; } = 0;

    public EstadoCategoria Estado { get; init; } = EstadoCategoria.Activo;
}

/// <summary>
/// DTO para actualizar una categoría
/// </summary>
public record ActualizarCategoriaDTO
{
    [Required(ErrorMessage = "El ID es requerido")]
    public Guid Id { get; init; }

    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
    public string Nombre { get; init; } = string.Empty;

    [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
    public string? Descripcion { get; init; }

    public Guid? CategoriaPadreId { get; init; }

    [Range(0, 9999, ErrorMessage = "El orden debe estar entre 0 y 9999")]
    public int Orden { get; init; }

    public EstadoCategoria Estado { get; init; }
}

/// <summary>
/// DTO para mostrar información de categoría
/// </summary>
public record CategoriaDTO
{
    public Guid Id { get; init; }
    public string Nombre { get; init; } = string.Empty;
    public string? Descripcion { get; init; }
    public Guid? CategoriaPadreId { get; init; }
    public string? CategoriaPadreNombre { get; init; }
    public int Orden { get; init; }
    public EstadoCategoria Estado { get; init; }
    public bool EsCategoriaPrincipal => !CategoriaPadreId.HasValue;
    public int CantidadLibros { get; init; }
    public int CantidadSubCategorias { get; init; }
}

/// <summary>
/// DTO simplificado para listados y selects
/// </summary>
public record CategoriaSimpleDTO
{
    public Guid Id { get; init; }
    public string Nombre { get; init; } = string.Empty;
    public Guid? CategoriaPadreId { get; init; }
    public bool EsCategoriaPrincipal => !CategoriaPadreId.HasValue;
}
