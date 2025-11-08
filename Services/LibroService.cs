using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SciFiHub.Domain.Interfaces;
using SciFiHub.Web.DTOs.Common;
using SciFiHub.Web.DTOs.Libro;
using SciFiHub.Web.Services.Interfaces;

namespace SciFiHub.Web.Services;

/// <summary>
/// Implementación del servicio de gestión de libros
/// </summary>
public class LibroService : ILibroService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<LibroService> _logger;

    public LibroService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<LibroService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<LibroDTO>> CrearLibroAsync(CrearLibroDTO crearLibroDto, CancellationToken cancellationToken = default)
    {
        try
        {
            // Validar ISBN único
            if (await _unitOfWork.Libros.ISBNExisteAsync(crearLibroDto.ISBN, null, cancellationToken))
            {
                return Result<LibroDTO>.FailureResult("El ISBN ya está registrado");
            }

            // Validar que la categoría existe
            if (!await _unitOfWork.Categorias.ExistsAsync(crearLibroDto.CategoriaId, cancellationToken))
            {
                return Result<LibroDTO>.FailureResult("La categoría seleccionada no existe");
            }

            var libro = _mapper.Map<Domain.Entities.Libro>(crearLibroDto);

            await _unitOfWork.Libros.AddAsync(libro, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            // Recargar con categoría para el DTO
            libro = await _unitOfWork.Libros.Query()
                .Include(l => l.Categoria)
                .FirstOrDefaultAsync(l => l.Id == libro.Id, cancellationToken);

            var libroDto = _mapper.Map<LibroDTO>(libro);
            _logger.LogInformation("Libro {Titulo} creado exitosamente", libro!.Titulo);

            return Result<LibroDTO>.SuccessResult(libroDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en CrearLibroAsync");
            return Result<LibroDTO>.FailureResult("Error al crear el libro");
        }
    }

    public async Task<Result<LibroDTO>> ActualizarLibroAsync(ActualizarLibroDTO actualizarLibroDto, CancellationToken cancellationToken = default)
    {
        try
        {
            var libro = await _unitOfWork.Libros.GetByIdAsync(actualizarLibroDto.Id, cancellationToken);

            if (libro == null)
            {
                return Result<LibroDTO>.FailureResult("Libro no encontrado");
            }

            // Validar ISBN único (excepto el propio libro)
            if (await _unitOfWork.Libros.ISBNExisteAsync(actualizarLibroDto.ISBN, libro.Id, cancellationToken))
            {
                return Result<LibroDTO>.FailureResult("El ISBN ya está registrado por otro libro");
            }

            // Validar categoría
            if (!await _unitOfWork.Categorias.ExistsAsync(actualizarLibroDto.CategoriaId, cancellationToken))
            {
                return Result<LibroDTO>.FailureResult("La categoría seleccionada no existe");
            }

            // Actualizar propiedades
            _mapper.Map(actualizarLibroDto, libro);

            await _unitOfWork.Libros.UpdateAsync(libro, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            // Recargar con categoría
            libro = await _unitOfWork.Libros.Query()
                .Include(l => l.Categoria)
                .FirstOrDefaultAsync(l => l.Id == libro.Id, cancellationToken);

            var libroDto = _mapper.Map<LibroDTO>(libro);
            _logger.LogInformation("Libro {Titulo} actualizado exitosamente", libro!.Titulo);

            return Result<LibroDTO>.SuccessResult(libroDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en ActualizarLibroAsync");
            return Result<LibroDTO>.FailureResult("Error al actualizar el libro");
        }
    }

    public async Task<Result> EliminarLibroAsync(Guid libroId, CancellationToken cancellationToken = default)
    {
        try
        {
            var libro = await _unitOfWork.Libros.GetByIdAsync(libroId, cancellationToken);

            if (libro == null)
            {
                return Result.FailureResult("Libro no encontrado");
            }

            // Soft delete
            await _unitOfWork.Libros.DeleteAsync(libro, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            _logger.LogInformation("Libro {Titulo} eliminado (soft delete)", libro.Titulo);

            return Result.SuccessResult();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en EliminarLibroAsync");
            return Result.FailureResult("Error al eliminar el libro");
        }
    }

    public async Task<Result<LibroDTO>> ObtenerLibroPorIdAsync(Guid libroId, CancellationToken cancellationToken = default)
    {
        try
        {
            var libro = await _unitOfWork.Libros.Query()
                .Include(l => l.Categoria)
                .FirstOrDefaultAsync(l => l.Id == libroId, cancellationToken);

            if (libro == null)
            {
                return Result<LibroDTO>.FailureResult("Libro no encontrado");
            }

            var libroDto = _mapper.Map<LibroDTO>(libro);
            return Result<LibroDTO>.SuccessResult(libroDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en ObtenerLibroPorIdAsync");
            return Result<LibroDTO>.FailureResult("Error al obtener el libro");
        }
    }

    public async Task<Result<PagedResult<LibroCardDTO>>> ObtenerLibrosAsync(LibrosFiltroDTO filtro, CancellationToken cancellationToken = default)
    {
        return await ObtenerLibrosPaginadosAsync(filtro, cancellationToken);
    }

    public async Task<Result<PagedResult<LibroCardDTO>>> ObtenerLibrosPaginadosAsync(LibrosFiltroDTO filtro, CancellationToken cancellationToken = default)
    {
        try
        {
            var (libros, totalCount) = await _unitOfWork.Libros.GetPagedConFiltrosAsync(
                filtro.PageNumber,
                filtro.PageSize,
                filtro.TextoBusqueda,
                filtro.CategoriaId,
                filtro.PrecioMin,
                filtro.PrecioMax,
                filtro.Destacado,
                filtro.SoloDisponibles,
                filtro.OrdenarPor,
                filtro.OrdenAscendente,
                cancellationToken);

            var librosDto = _mapper.Map<IEnumerable<LibroCardDTO>>(libros);

            var result = new PagedResult<LibroCardDTO>
            {
                Items = librosDto,
                PageNumber = filtro.PageNumber,
                PageSize = filtro.PageSize,
                TotalCount = totalCount
            };

            return Result<PagedResult<LibroCardDTO>>.SuccessResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en ObtenerLibrosPaginadosAsync");
            return Result<PagedResult<LibroCardDTO>>.FailureResult("Error al obtener libros");
        }
    }

    public async Task<Result<IEnumerable<LibroCardDTO>>> BuscarLibrosAsync(string textoBusqueda, CancellationToken cancellationToken = default)
    {
        try
        {
            var libros = await _unitOfWork.Libros.BuscarPorCriterioAsync(textoBusqueda, null, true, cancellationToken);
            var librosDto = _mapper.Map<IEnumerable<LibroCardDTO>>(libros);

            return Result<IEnumerable<LibroCardDTO>>.SuccessResult(librosDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en BuscarLibrosAsync");
            return Result<IEnumerable<LibroCardDTO>>.FailureResult("Error al buscar libros");
        }
    }

    public async Task<Result<IEnumerable<LibroCardDTO>>> ObtenerPorCategoriaAsync(Guid categoriaId, CancellationToken cancellationToken = default)
    {
        try
        {
            var libros = await _unitOfWork.Libros.GetPorCategoriaAsync(categoriaId, cancellationToken);
            var librosDto = _mapper.Map<IEnumerable<LibroCardDTO>>(libros);

            return Result<IEnumerable<LibroCardDTO>>.SuccessResult(librosDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en ObtenerPorCategoriaAsync");
            return Result<IEnumerable<LibroCardDTO>>.FailureResult("Error al obtener libros por categoría");
        }
    }

    public async Task<Result<IEnumerable<LibroCardDTO>>> ObtenerPorAutorAsync(string autor, CancellationToken cancellationToken = default)
    {
        try
        {
            var libros = await _unitOfWork.Libros.GetPorAutorAsync(autor, cancellationToken);
            var librosDto = _mapper.Map<IEnumerable<LibroCardDTO>>(libros);

            return Result<IEnumerable<LibroCardDTO>>.SuccessResult(librosDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en ObtenerPorAutorAsync");
            return Result<IEnumerable<LibroCardDTO>>.FailureResult("Error al obtener libros por autor");
        }
    }

    public async Task<Result<IEnumerable<LibroCardDTO>>> ObtenerDestacadosAsync(int cantidad = 10, CancellationToken cancellationToken = default)
    {
        try
        {
            var libros = await _unitOfWork.Libros.GetDestacadosAsync(cantidad, cancellationToken);
            var librosDto = _mapper.Map<IEnumerable<LibroCardDTO>>(libros);

            return Result<IEnumerable<LibroCardDTO>>.SuccessResult(librosDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en ObtenerDestacadosAsync");
            return Result<IEnumerable<LibroCardDTO>>.FailureResult("Error al obtener libros destacados");
        }
    }

    public async Task<Result<IEnumerable<LibroCardDTO>>> ObtenerNovedadesAsync(int cantidad = 10, CancellationToken cancellationToken = default)
    {
        try
        {
            var libros = await _unitOfWork.Libros.GetNovedadesAsync(cantidad, cancellationToken);
            var librosDto = _mapper.Map<IEnumerable<LibroCardDTO>>(libros);

            return Result<IEnumerable<LibroCardDTO>>.SuccessResult(librosDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en ObtenerNovedadesAsync");
            return Result<IEnumerable<LibroCardDTO>>.FailureResult("Error al obtener novedades");
        }
    }

    public async Task<Result<IEnumerable<LibroCardDTO>>> ObtenerMasVendidosAsync(int cantidad = 10, CancellationToken cancellationToken = default)
    {
        try
        {
            var libros = await _unitOfWork.Libros.GetMasVendidosAsync(cantidad, null, cancellationToken);
            var librosDto = _mapper.Map<IEnumerable<LibroCardDTO>>(libros);

            return Result<IEnumerable<LibroCardDTO>>.SuccessResult(librosDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en ObtenerMasVendidosAsync");
            return Result<IEnumerable<LibroCardDTO>>.FailureResult("Error al obtener libros más vendidos");
        }
    }

    public async Task<Result<IEnumerable<LibroCardDTO>>> ObtenerOfertasAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var libros = await _unitOfWork.Libros.GetOfertasAsync(cancellationToken);
            var librosDto = _mapper.Map<IEnumerable<LibroCardDTO>>(libros);

            return Result<IEnumerable<LibroCardDTO>>.SuccessResult(librosDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en ObtenerOfertasAsync");
            return Result<IEnumerable<LibroCardDTO>>.FailureResult("Error al obtener ofertas");
        }
    }

    public async Task<Result> ActualizarStockAsync(ActualizarStockDTO actualizarStockDto, CancellationToken cancellationToken = default)
    {
        try
        {
            await _unitOfWork.Libros.ActualizarStockAsync(
                actualizarStockDto.LibroId,
                actualizarStockDto.Cantidad,
                actualizarStockDto.TipoMovimiento,
                actualizarStockDto.Motivo,
                cancellationToken);

            await _unitOfWork.CommitAsync(cancellationToken);

            _logger.LogInformation("Stock actualizado para libro {LibroId}: {TipoMovimiento} {Cantidad}",
                actualizarStockDto.LibroId, actualizarStockDto.TipoMovimiento, actualizarStockDto.Cantidad);

            return Result.SuccessResult();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en ActualizarStockAsync");
            return Result.FailureResult(ex.Message);
        }
    }

    public async Task<Result<bool>> VerificarStockDisponibleAsync(Guid libroId, int cantidadRequerida, CancellationToken cancellationToken = default)
    {
        try
        {
            var hayStock = await _unitOfWork.Libros.VerificarStockAsync(libroId, cantidadRequerida, cancellationToken);
            return Result<bool>.SuccessResult(hayStock);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en VerificarStockDisponibleAsync");
            return Result<bool>.FailureResult("Error al verificar stock");
        }
    }

    public async Task<Result<IEnumerable<LibroDTO>>> ObtenerStockBajoAsync(int umbral = 10, CancellationToken cancellationToken = default)
    {
        try
        {
            var libros = await _unitOfWork.Libros.GetStockBajoAsync(umbral, cancellationToken);
            var librosDto = _mapper.Map<IEnumerable<LibroDTO>>(libros);

            return Result<IEnumerable<LibroDTO>>.SuccessResult(librosDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en ObtenerStockBajoAsync");
            return Result<IEnumerable<LibroDTO>>.FailureResult("Error al obtener libros con stock bajo");
        }
    }

    public async Task<Result<bool>> ISBNExisteAsync(string isbn, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var existe = await _unitOfWork.Libros.ISBNExisteAsync(isbn, excludeId, cancellationToken);
            return Result<bool>.SuccessResult(existe);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en ISBNExisteAsync");
            return Result<bool>.FailureResult("Error al verificar ISBN");
        }
    }

    public async Task<Result<IEnumerable<string>>> ObtenerEditorialesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var editoriales = await _unitOfWork.Libros.Query()
                .Where(l => !string.IsNullOrEmpty(l.Editorial))
                .Select(l => l.Editorial!)
                .Distinct()
                .OrderBy(e => e)
                .ToListAsync(cancellationToken);

            return Result<IEnumerable<string>>.SuccessResult(editoriales);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en ObtenerEditorialesAsync");
            return Result<IEnumerable<string>>.FailureResult("Error al obtener editoriales");
        }
    }
}
