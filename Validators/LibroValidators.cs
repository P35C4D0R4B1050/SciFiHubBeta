using FluentValidation;
using SciFiHub.Domain.Interfaces;
using SciFiHub.Web.DTOs.Libro;

namespace SciFiHub.Web.Validators;

/// <summary>
/// Validador para CrearLibroDTO
/// </summary>
public class CrearLibroValidator : AbstractValidator<CrearLibroDTO>
{
    private readonly IUnitOfWork _unitOfWork;

    public CrearLibroValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.ISBN)
            .NotEmpty().WithMessage("El ISBN es requerido")
            .MaximumLength(20).WithMessage("El ISBN no puede exceder 20 caracteres")
            .MustAsync(async (isbn, cancellation) => !await _unitOfWork.Libros.ISBNExisteAsync(isbn, null, cancellation))
            .WithMessage("El ISBN ya está registrado");

        RuleFor(x => x.Titulo)
            .NotEmpty().WithMessage("El título es requerido")
            .MaximumLength(200).WithMessage("El título no puede exceder 200 caracteres");

        RuleFor(x => x.Autor)
            .NotEmpty().WithMessage("El autor es requerido")
            .MaximumLength(150).WithMessage("El autor no puede exceder 150 caracteres");

        RuleFor(x => x.Editorial)
            .NotEmpty().WithMessage("La editorial es requerida")
            .MaximumLength(100).WithMessage("La editorial no puede exceder 100 caracteres");

        RuleFor(x => x.CategoriaId)
            .NotEmpty().WithMessage("La categoría es requerida")
            .MustAsync(async (id, cancellation) => await _unitOfWork.Categorias.ExistsAsync(id, cancellation))
            .WithMessage("La categoría seleccionada no existe");

        RuleFor(x => x.AñoPublicacion)
            .InclusiveBetween(1800, 2100).WithMessage("El año debe estar entre 1800 y 2100");

        RuleFor(x => x.Paginas)
            .GreaterThan(0).WithMessage("El número de páginas debe ser mayor a 0");

        RuleFor(x => x.Precio)
            .GreaterThan(0).WithMessage("El precio debe ser mayor a 0");

        RuleFor(x => x.PrecioOferta)
            .LessThan(x => x.Precio).WithMessage("El precio de oferta debe ser menor al precio regular")
            .GreaterThan(0).WithMessage("El precio de oferta debe ser mayor a 0")
            .When(x => x.PrecioOferta.HasValue);

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0).WithMessage("El stock debe ser mayor o igual a 0");

        RuleFor(x => x.Sinopsis)
            .MaximumLength(2000).WithMessage("La sinopsis no puede exceder 2000 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Sinopsis));

        RuleFor(x => x.Descripcion)
            .MaximumLength(4000).WithMessage("La descripción no puede exceder 4000 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Descripcion));
    }
}

/// <summary>
/// Validador para ActualizarLibroDTO
/// </summary>
public class ActualizarLibroValidator : AbstractValidator<ActualizarLibroDTO>
{
    private readonly IUnitOfWork _unitOfWork;

    public ActualizarLibroValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El ID es requerido")
            .MustAsync(async (id, cancellation) => await _unitOfWork.Libros.ExistsAsync(id, cancellation))
            .WithMessage("El libro no existe");

        RuleFor(x => x.ISBN)
            .NotEmpty().WithMessage("El ISBN es requerido")
            .MaximumLength(20).WithMessage("El ISBN no puede exceder 20 caracteres")
            .MustAsync(async (dto, isbn, cancellation) => !await _unitOfWork.Libros.ISBNExisteAsync(isbn, dto.Id, cancellation))
            .WithMessage("El ISBN ya está registrado por otro libro");

        RuleFor(x => x.Titulo)
            .NotEmpty().WithMessage("El título es requerido")
            .MaximumLength(200).WithMessage("El título no puede exceder 200 caracteres");

        RuleFor(x => x.Autor)
            .NotEmpty().WithMessage("El autor es requerido")
            .MaximumLength(150).WithMessage("El autor no puede exceder 150 caracteres");

        RuleFor(x => x.Editorial)
            .NotEmpty().WithMessage("La editorial es requerida")
            .MaximumLength(100).WithMessage("La editorial no puede exceder 100 caracteres");

        RuleFor(x => x.CategoriaId)
            .NotEmpty().WithMessage("La categoría es requerida")
            .MustAsync(async (id, cancellation) => await _unitOfWork.Categorias.ExistsAsync(id, cancellation))
            .WithMessage("La categoría seleccionada no existe");

        RuleFor(x => x.AñoPublicacion)
            .InclusiveBetween(1800, 2100).WithMessage("El año debe estar entre 1800 y 2100");

        RuleFor(x => x.Paginas)
            .GreaterThan(0).WithMessage("El número de páginas debe ser mayor a 0");

        RuleFor(x => x.Precio)
            .GreaterThan(0).WithMessage("El precio debe ser mayor a 0");

        RuleFor(x => x.PrecioOferta)
            .LessThan(x => x.Precio).WithMessage("El precio de oferta debe ser menor al precio regular")
            .GreaterThan(0).WithMessage("El precio de oferta debe ser mayor a 0")
            .When(x => x.PrecioOferta.HasValue);
    }
}

/// <summary>
/// Validador para ActualizarStockDTO
/// </summary>
public class ActualizarStockValidator : AbstractValidator<ActualizarStockDTO>
{
    private readonly IUnitOfWork _unitOfWork;

    public ActualizarStockValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.LibroId)
            .NotEmpty().WithMessage("El ID del libro es requerido")
            .MustAsync(async (id, cancellation) => await _unitOfWork.Libros.ExistsAsync(id, cancellation))
            .WithMessage("El libro no existe");

        RuleFor(x => x.Cantidad)
            .NotEmpty().WithMessage("La cantidad es requerida")
            .GreaterThan(0).WithMessage("La cantidad debe ser mayor a 0");

        RuleFor(x => x.Motivo)
            .MaximumLength(500).WithMessage("El motivo no puede exceder 500 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Motivo));
    }
}
