using AutoMapper;
using SciFiHub.Domain.Interfaces;
using SciFiHub.Web.DTOs.Common;
using SciFiHub.Web.DTOs.Categoria;
using SciFiHub.Web.Services.Interfaces;

namespace SciFiHub.Web.Services;

/// <summary>
/// Implementación del servicio de gestión de categorías
/// </summary>
public class CategoriaService : ICategoriaService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<CategoriaService> _logger;

    public CategoriaService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<CategoriaService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<CategoriaDTO>> CrearCategoriaAsync(CrearCategoriaDTO crearCategoriaDto, CancellationToken cancellationToken = default)
    {
        try
        {
            // Validar nombre único
            if (await _unitOfWork.Categorias.NombreExisteAsync(crearCategoriaDto.Nombre, null, cancellationToken))
            {
                return Result<CategoriaDTO>.FailureResult("El nombre de categoría ya existe");
            }

            // Validar categoría padre si se proporciona
            if (crearCategoriaDto.CategoriaPadreId.HasValue &&
                !await _unitOfWork.Categorias.ExistsAsync(crearCategoriaDto.CategoriaPadreId.Value, cancellationToken))
            {
                return Result<CategoriaDTO>.FailureResult("La categoría padre no existe");
            }

            var categoria = _mapper.Map<Domain.Entities.Categoria>(crearCategoriaDto);

            await _unitOfWork.Categorias.AddAsync(categoria, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            var categoriaDto = _mapper.Map<CategoriaDTO>(categoria);
            _logger.LogInformation("Categoría {Nombre} creada exitosamente", categoria.Nombre);

            return Result<CategoriaDTO>.SuccessResult(categoriaDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en CrearCategoriaAsync");
            return Result<CategoriaDTO>.FailureResult("Error al crear la categoría");
        }
    }

    public async Task<Result<CategoriaDTO>> ActualizarCategoriaAsync(ActualizarCategoriaDTO actualizarCategoriaDto, CancellationToken cancellationToken = default)
    {
        try
        {
            var categoria = await _unitOfWork.Categorias.GetByIdAsync(actualizarCategoriaDto.Id, cancellationToken);

            if (categoria == null)
            {
                return Result<CategoriaDTO>.FailureResult("Categoría no encontrada");
            }

            // Validar nombre único (excepto la propia categoría)
            if (await _unitOfWork.Categorias.NombreExisteAsync(actualizarCategoriaDto.Nombre, categoria.Id, cancellationToken))
            {
                return Result<CategoriaDTO>.FailureResult("El nombre de categoría ya existe");
            }

            // Validar categoría padre
            if (actualizarCategoriaDto.CategoriaPadreId.HasValue)
            {
                // No puede ser su propia padre
                if (actualizarCategoriaDto.CategoriaPadreId.Value == categoria.Id)
                {
                    return Result<CategoriaDTO>.FailureResult("Una categoría no puede ser su propia categoría padre");
                }

                if (!await _unitOfWork.Categorias.ExistsAsync(actualizarCategoriaDto.CategoriaPadreId.Value, cancellationToken))
                {
                    return Result<CategoriaDTO>.FailureResult("La categoría padre no existe");
                }
            }

            // Actualizar propiedades
            _mapper.Map(actualizarCategoriaDto, categoria);

            await _unitOfWork.Categorias.UpdateAsync(categoria, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            var categoriaDto = _mapper.Map<CategoriaDTO>(categoria);
            _logger.LogInformation("Categoría {Nombre} actualizada exitosamente", categoria.Nombre);

            return Result<CategoriaDTO>.SuccessResult(categoriaDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en ActualizarCategoriaAsync");
            return Result<CategoriaDTO>.FailureResult("Error al actualizar la categoría");
        }
    }

    public async Task<Result> EliminarCategoriaAsync(Guid categoriaId, CancellationToken cancellationToken = default)
    {
        try
        {
            var categoria = await _unitOfWork.Categorias.GetByIdAsync(categoriaId, cancellationToken);

            if (categoria == null)
            {
                return Result.FailureResult("Categoría no encontrada");
            }

            // Validar que no tenga libros asociados
            if (await _unitOfWork.Categorias.TieneLibrosAsync(categoriaId, cancellationToken))
            {
                return Result.FailureResult("No se puede eliminar la categoría porque tiene libros asociados");
            }

            // Validar que no tenga subcategorías
            if (await _unitOfWork.Categorias.TieneSubCategoriasAsync(categoriaId, cancellationToken))
            {
                return Result.FailureResult("No se puede eliminar la categoría porque tiene subcategorías");
            }

            // Soft delete
            await _unitOfWork.Categorias.DeleteAsync(categoria, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            _logger.LogInformation("Categoría {Nombre} eliminada (soft delete)", categoria.Nombre);

            return Result.SuccessResult();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en EliminarCategoriaAsync");
            return Result.FailureResult("Error al eliminar la categoría");
        }
    }

    public async Task<Result<CategoriaDTO>> ObtenerCategoriaPorIdAsync(Guid categoriaId, CancellationToken cancellationToken = default)
    {
        try
        {
            var categoria = await _unitOfWork.Categorias.GetByIdAsync(categoriaId, cancellationToken);

            if (categoria == null)
            {
                return Result<CategoriaDTO>.FailureResult("Categoría no encontrada");
            }

            var categoriaDto = _mapper.Map<CategoriaDTO>(categoria);
            return Result<CategoriaDTO>.SuccessResult(categoriaDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en ObtenerCategoriaPorIdAsync");
            return Result<CategoriaDTO>.FailureResult("Error al obtener la categoría");
        }
    }

    public async Task<Result<IEnumerable<CategoriaDTO>>> ObtenerCategoriasActivasAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var categorias = await _unitOfWork.Categorias.GetCategoriasActivasAsync(cancellationToken);
            var categoriasDto = _mapper.Map<IEnumerable<CategoriaDTO>>(categorias);

            return Result<IEnumerable<CategoriaDTO>>.SuccessResult(categoriasDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en ObtenerCategoriasActivasAsync");
            return Result<IEnumerable<CategoriaDTO>>.FailureResult("Error al obtener categorías activas");
        }
    }

    public async Task<Result<IEnumerable<CategoriaDTO>>> ObtenerCategoriasPrincipalesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var categorias = await _unitOfWork.Categorias.GetCategoriasPrincipalesAsync(cancellationToken);
            var categoriasDto = _mapper.Map<IEnumerable<CategoriaDTO>>(categorias);

            return Result<IEnumerable<CategoriaDTO>>.SuccessResult(categoriasDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en ObtenerCategoriasPrincipalesAsync");
            return Result<IEnumerable<CategoriaDTO>>.FailureResult("Error al obtener categorías principales");
        }
    }

    public async Task<Result<IEnumerable<CategoriaDTO>>> ObtenerSubCategoriasAsync(Guid categoriaPadreId, CancellationToken cancellationToken = default)
    {
        try
        {
            var categorias = await _unitOfWork.Categorias.GetSubCategoriasAsync(categoriaPadreId, cancellationToken);
            var categoriasDto = _mapper.Map<IEnumerable<CategoriaDTO>>(categorias);

            return Result<IEnumerable<CategoriaDTO>>.SuccessResult(categoriasDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en ObtenerSubCategoriasAsync");
            return Result<IEnumerable<CategoriaDTO>>.FailureResult("Error al obtener subcategorías");
        }
    }

    public async Task<Result<IEnumerable<CategoriaDTO>>> ObtenerArbolCategoriasAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var categoriasPrincipales = await _unitOfWork.Categorias.GetCategoriasPrincipalesAsync(cancellationToken);
            var categoriasDto = new List<CategoriaDTO>();

            foreach (var categoria in categoriasPrincipales)
            {
                var categoriaDto = _mapper.Map<CategoriaDTO>(categoria);
                // Se pueden cargar subcategorías recursivamente si se necesita
                categoriasDto.Add(categoriaDto);
            }

            return Result<IEnumerable<CategoriaDTO>>.SuccessResult(categoriasDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en ObtenerArbolCategoriasAsync");
            return Result<IEnumerable<CategoriaDTO>>.FailureResult("Error al obtener árbol de categorías");
        }
    }

    public async Task<Result<IEnumerable<CategoriaSimpleDTO>>> ObtenerCategoriasParaSelectAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var categorias = await _unitOfWork.Categorias.GetCategoriasActivasAsync(cancellationToken);
            var categoriasDto = _mapper.Map<IEnumerable<CategoriaSimpleDTO>>(categorias);

            return Result<IEnumerable<CategoriaSimpleDTO>>.SuccessResult(categoriasDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en ObtenerCategoriasParaSelectAsync");
            return Result<IEnumerable<CategoriaSimpleDTO>>.FailureResult("Error al obtener categorías para select");
        }
    }

    public async Task<Result<bool>> TieneLibrosAsync(Guid categoriaId, CancellationToken cancellationToken = default)
    {
        try
        {
            var tieneLibros = await _unitOfWork.Categorias.TieneLibrosAsync(categoriaId, cancellationToken);
            return Result<bool>.SuccessResult(tieneLibros);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en TieneLibrosAsync");
            return Result<bool>.FailureResult("Error al verificar libros");
        }
    }

    public async Task<Result<bool>> TieneSubCategoriasAsync(Guid categoriaId, CancellationToken cancellationToken = default)
    {
        try
        {
            var tieneSubCategorias = await _unitOfWork.Categorias.TieneSubCategoriasAsync(categoriaId, cancellationToken);
            return Result<bool>.SuccessResult(tieneSubCategorias);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en TieneSubCategoriasAsync");
            return Result<bool>.FailureResult("Error al verificar subcategorías");
        }
    }

    public async Task<Result<bool>> NombreExisteAsync(string nombre, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var existe = await _unitOfWork.Categorias.NombreExisteAsync(nombre, excludeId, cancellationToken);
            return Result<bool>.SuccessResult(existe);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en NombreExisteAsync");
            return Result<bool>.FailureResult("Error al verificar nombre");
        }
    }
}
