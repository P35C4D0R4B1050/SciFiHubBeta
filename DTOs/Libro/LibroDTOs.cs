using System.ComponentModel.DataAnnotations;
using SciFiHub.Domain.Enums;

namespace SciFiHub.Web.DTOs.Libro;

/// <summary>
/// DTO para crear un nuevo libro
/// </summary>
public record CrearLibroDTO
{
    [Required(ErrorMessage = "El ISBN es requerido")]
    [StringLength(20, ErrorMessage = "El ISBN no puede exceder 20 caracteres")]
    public string ISBN { get; init; } = string.Empty;

    [Required(ErrorMessage = "El título es requerido")]
    [StringLength(200, ErrorMessage = "El título no puede exceder 200 caracteres")]
    public string Titulo { get; init; } = string.Empty;

    [Required(ErrorMessage = "El autor es requerido")]
    [StringLength(150, ErrorMessage = "El autor no puede exceder 150 caracteres")]
    public string Autor { get; init; } = string.Empty;

    [Required(ErrorMessage = "La editorial es requerida")]
    [StringLength(100, ErrorMessage = "La editorial no puede exceder 100 caracteres")]
    public string Editorial { get; init; } = string.Empty;

    [Required(ErrorMessage = "La categoría es requerida")]
    public Guid CategoriaId { get; init; }

    [Required(ErrorMessage = "El año de publicación es requerido")]
    [Range(1800, 2100, ErrorMessage = "El año debe estar entre 1800 y 2100")]
    public int AñoPublicacion { get; init; }

    [Required(ErrorMessage = "El número de páginas es requerido")]
    [Range(1, 10000, ErrorMessage = "El número de páginas debe estar entre 1 y 10000")]
    public int Paginas { get; init; }

    [StringLength(10, ErrorMessage = "El idioma no puede exceder 10 caracteres")]
    public string Idioma { get; init; } = "Español";

    [Required(ErrorMessage = "El precio es requerido")]
    [Range(0.01, 999999.99, ErrorMessage = "El precio debe ser mayor a 0")]
    public decimal Precio { get; init; }

    public decimal? PrecioOferta { get; init; }

    [Required(ErrorMessage = "El stock inicial es requerido")]
    [Range(0, 999999, ErrorMessage = "El stock debe ser mayor o igual a 0")]
    public int Stock { get; init; }

    public string? ImagenPortada { get; init; }

    [StringLength(2000, ErrorMessage = "La sinopsis no puede exceder 2000 caracteres")]
    public string? Sinopsis { get; init; }

    [StringLength(4000, ErrorMessage = "La descripción no puede exceder 4000 caracteres")]
    public string? Descripcion { get; init; }

    public bool Destacado { get; init; }
    public EstadoLibro Estado { get; init; } = EstadoLibro.Disponible;
}

/// <summary>
/// DTO para actualizar un libro existente
/// </summary>
public record ActualizarLibroDTO
{
    [Required(ErrorMessage = "El ID es requerido")]
    public Guid Id { get; init; }

    [Required(ErrorMessage = "El ISBN es requerido")]
    [StringLength(20, ErrorMessage = "El ISBN no puede exceder 20 caracteres")]
    public string ISBN { get; init; } = string.Empty;

    [Required(ErrorMessage = "El título es requerido")]
    [StringLength(200, ErrorMessage = "El título no puede exceder 200 caracteres")]
    public string Titulo { get; init; } = string.Empty;

    [Required(ErrorMessage = "El autor es requerido")]
    [StringLength(150, ErrorMessage = "El autor no puede exceder 150 caracteres")]
    public string Autor { get; init; } = string.Empty;

    [Required(ErrorMessage = "La editorial es requerida")]
    [StringLength(100, ErrorMessage = "La editorial no puede exceder 100 caracteres")]
    public string Editorial { get; init; } = string.Empty;

    [Required(ErrorMessage = "La categoría es requerida")]
    public Guid CategoriaId { get; init; }

    [Required(ErrorMessage = "El año de publicación es requerido")]
    [Range(1800, 2100, ErrorMessage = "El año debe estar entre 1800 y 2100")]
    public int AñoPublicacion { get; init; }

    [Required(ErrorMessage = "El número de páginas es requerido")]
    [Range(1, 10000, ErrorMessage = "El número de páginas debe estar entre 1 y 10000")]
    public int Paginas { get; init; }

    [StringLength(10, ErrorMessage = "El idioma no puede exceder 10 caracteres")]
    public string Idioma { get; init; } = "Español";

    [Required(ErrorMessage = "El precio es requerido")]
    [Range(0.01, 999999.99, ErrorMessage = "El precio debe ser mayor a 0")]
    public decimal Precio { get; init; }

    public decimal? PrecioOferta { get; init; }

    [Required(ErrorMessage = "El stock es requerido")]
    [Range(0, 999999, ErrorMessage = "El stock debe ser mayor o igual a 0")]
    public int Stock { get; init; }

    public string? ImagenPortada { get; init; }

    [StringLength(2000, ErrorMessage = "La sinopsis no puede exceder 2000 caracteres")]
    public string? Sinopsis { get; init; }

    [StringLength(4000, ErrorMessage = "La descripción no puede exceder 4000 caracteres")]
    public string? Descripcion { get; init; }

    public bool Destacado { get; init; }
    public EstadoLibro Estado { get; init; }
}

/// <summary>
/// DTO para mostrar información de libro (vista completa)
/// </summary>
public record LibroDTO
{
    public Guid Id { get; init; }
    public string ISBN { get; init; } = string.Empty;
    public string Titulo { get; init; } = string.Empty;
    public string Autor { get; init; } = string.Empty;
    public string Editorial { get; init; } = string.Empty;
    public Guid CategoriaId { get; init; }
    public string CategoriaNombre { get; init; } = string.Empty;
    public int AñoPublicacion { get; init; }
    public int Paginas { get; init; }
    public string Idioma { get; init; } = string.Empty;
    public decimal Precio { get; init; }
    public decimal? PrecioOferta { get; init; }
    public int Stock { get; init; }
    public string? ImagenPortada { get; init; }
    public string? Sinopsis { get; init; }
    public string? Descripcion { get; init; }
    public bool Destacado { get; init; }
    public EstadoLibro Estado { get; init; }
    public bool Disponible => Stock > 0 && Estado == EstadoLibro.Disponible;
    public decimal PrecioFinal => PrecioOferta ?? Precio;
    public bool TieneOferta => PrecioOferta.HasValue && PrecioOferta < Precio;
    public decimal? PorcentajeDescuento => TieneOferta ? Math.Round((1 - (PrecioOferta!.Value / Precio)) * 100, 0) : null;
}

/// <summary>
/// DTO simplificado para cards de catálogo
/// </summary>
public record LibroCardDTO
{
    public Guid Id { get; init; }
    public string Titulo { get; init; } = string.Empty;
    public string Autor { get; init; } = string.Empty;
    public string? ImagenPortada { get; init; }
    public decimal Precio { get; init; }
    public decimal? PrecioOferta { get; init; }
    public int Stock { get; init; }
    public bool Destacado { get; init; }
    public bool Disponible { get; init; }
    public string CategoriaNombre { get; init; } = string.Empty;
    public decimal PrecioFinal => PrecioOferta ?? Precio;
    public bool TieneOferta => PrecioOferta.HasValue && PrecioOferta < Precio;
    public decimal? PorcentajeDescuento => TieneOferta ? Math.Round((1 - (PrecioOferta!.Value / Precio)) * 100, 0) : null;
}

/// <summary>
/// DTO para actualizar solo el stock
/// </summary>
public record ActualizarStockDTO
{
    [Required(ErrorMessage = "El ID del libro es requerido")]
    public Guid LibroId { get; init; }

    [Required(ErrorMessage = "La cantidad es requerida")]
    public int Cantidad { get; init; }

    [Required(ErrorMessage = "El tipo de movimiento es requerido")]
    public TipoMovimientoInventario TipoMovimiento { get; init; }

    [StringLength(500, ErrorMessage = "El motivo no puede exceder 500 caracteres")]
    public string? Motivo { get; init; }
}

/// <summary>
/// DTO para filtros de búsqueda de libros
/// </summary>
public record LibrosFiltroDTO
{
    public string? TextoBusqueda { get; init; }
    public Guid? CategoriaId { get; init; }
    public decimal? PrecioMin { get; init; }
    public decimal? PrecioMax { get; init; }
    public bool? Destacado { get; init; }
    public bool? SoloDisponibles { get; init; } = true;
    public string? OrdenarPor { get; init; } // "precio", "titulo", "autor", "fecha"
    public bool OrdenAscendente { get; init; } = true;
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 12;
}
