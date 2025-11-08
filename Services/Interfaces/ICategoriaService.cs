using SciFiHub.Web.DTOs.Common;
using SciFiHub.Web.DTOs.Categoria;

namespace SciFiHub.Web.Services.Interfaces;

/// <summary>
/// Servicio para gestión de categorías jerárquicas
/// </summary>
public interface ICategoriaService
{
    // ===================================
    // CRUD de Categorías
    // ===================================
    
    /// <summary>
    /// Crear una nueva categoría
    /// </summary>
    Task<Result<CategoriaDTO>> CrearCategoriaAsync(CrearCategoriaDTO crearCategoriaDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Actualizar categoría existente
    /// </summary>
    Task<Result<CategoriaDTO>> ActualizarCategoriaAsync(ActualizarCategoriaDTO actualizarCategoriaDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Eliminar categoría (soft delete) - Valida que no tenga libros ni subcategorías
    /// </summary>
    Task<Result> EliminarCategoriaAsync(Guid categoriaId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtener categoría por ID
    /// </summary>
    Task<Result<CategoriaDTO>> ObtenerCategoriaPorIdAsync(Guid categoriaId, CancellationToken cancellationToken = default);

    // ===================================
    // Consultas y Listados
    // ===================================
    
    /// <summary>
    /// Obtener todas las categorías activas
    /// </summary>
    Task<Result<IEnumerable<CategoriaDTO>>> ObtenerCategoriasActivasAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtener categorías principales (sin padre)
    /// </summary>
    Task<Result<IEnumerable<CategoriaDTO>>> ObtenerCategoriasPrincipalesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtener subcategorías de una categoría
    /// </summary>
    Task<Result<IEnumerable<CategoriaDTO>>> ObtenerSubCategoriasAsync(Guid categoriaPadreId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtener árbol completo de categorías (jerárquico)
    /// </summary>
    Task<Result<IEnumerable<CategoriaDTO>>> ObtenerArbolCategoriasAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtener categorías simples para select/dropdown
    /// </summary>
    Task<Result<IEnumerable<CategoriaSimpleDTO>>> ObtenerCategoriasParaSelectAsync(CancellationToken cancellationToken = default);

    // ===================================
    // Validaciones
    // ===================================
    
    /// <summary>
    /// Verificar si una categoría tiene libros asociados
    /// </summary>
    Task<Result<bool>> TieneLibrosAsync(Guid categoriaId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verificar si una categoría tiene subcategorías
    /// </summary>
    Task<Result<bool>> TieneSubCategoriasAsync(Guid categoriaId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verificar si el nombre de categoría ya existe
    /// </summary>
    Task<Result<bool>> NombreExisteAsync(string nombre, Guid? excludeId = null, CancellationToken cancellationToken = default);
}
