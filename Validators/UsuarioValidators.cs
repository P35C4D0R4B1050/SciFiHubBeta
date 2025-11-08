using FluentValidation;
using SciFiHub.Web.DTOs.Usuario;

namespace SciFiHub.Web.Validators;

/// <summary>
/// Validador para LoginDTO
/// </summary>
public class LoginValidator : AbstractValidator<LoginDTO>
{
    public LoginValidator()
    {
        RuleFor(x => x.UsernameOrEmail)
            .NotEmpty().WithMessage("El usuario o email es requerido")
            .MinimumLength(3).WithMessage("Debe tener al menos 3 caracteres");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es requerida");
    }
}

/// <summary>
/// Validador para CrearUsuarioDTO (SOLO VALIDACIONES SÍNCRONAS)
/// </summary>
public class CrearUsuarioValidator : AbstractValidator<CrearUsuarioDTO>
{
    public CrearUsuarioValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("El nombre de usuario es requerido")
            .MinimumLength(3).WithMessage("El username debe tener al menos 3 caracteres")
            .MaximumLength(50).WithMessage("El username no puede exceder 50 caracteres")
            .Matches("^[a-zA-Z0-9_]+$").WithMessage("El username solo puede contener letras, números y guiones bajos");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es requerido")
            .EmailAddress().WithMessage("El formato del email no es válido")
            .MaximumLength(100).WithMessage("El email no puede exceder 100 caracteres");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es requerida")
            .MinimumLength(6).WithMessage("La contraseña debe tener al menos 6 caracteres")
            .Matches("[A-Z]").WithMessage("La contraseña debe contener al menos una mayúscula")
            .Matches("[a-z]").WithMessage("La contraseña debe contener al menos una minúscula")
            .Matches("[0-9]").WithMessage("La contraseña debe contener al menos un número");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Debe confirmar la contraseña")
            .Equal(x => x.Password).WithMessage("Las contraseñas no coinciden");

        RuleFor(x => x.NombreCompleto)
            .NotEmpty().WithMessage("El nombre completo es requerido")
            .MinimumLength(3).WithMessage("El nombre debe tener al menos 3 caracteres")
            .MaximumLength(200).WithMessage("El nombre no puede exceder 200 caracteres");

        RuleFor(x => x.Telefono)
            .MaximumLength(20).WithMessage("El teléfono no puede exceder 20 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Telefono));
    }
}

/// <summary>
/// Validador para ActualizarUsuarioDTO
/// </summary>
public class ActualizarUsuarioValidator : AbstractValidator<ActualizarUsuarioDTO>
{
    public ActualizarUsuarioValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El ID es requerido");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es requerido")
            .EmailAddress().WithMessage("El formato del email no es válido")
            .MaximumLength(100).WithMessage("El email no puede exceder 100 caracteres");

        RuleFor(x => x.NombreCompleto)
            .NotEmpty().WithMessage("El nombre completo es requerido")
            .MinimumLength(3).WithMessage("El nombre debe tener al menos 3 caracteres")
            .MaximumLength(200).WithMessage("El nombre no puede exceder 200 caracteres");

        RuleFor(x => x.Telefono)
            .MaximumLength(20).WithMessage("El teléfono no puede exceder 20 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Telefono));
    }
}

/// <summary>
/// Validador para CambiarPasswordDTO
/// </summary>
public class CambiarPasswordValidator : AbstractValidator<CambiarPasswordDTO>
{
    public CambiarPasswordValidator()
    {
        RuleFor(x => x.UsuarioId)
            .NotEmpty().WithMessage("El ID del usuario es requerido");

        RuleFor(x => x.PasswordActual)
            .NotEmpty().WithMessage("La contraseña actual es requerida");

        RuleFor(x => x.NuevaPassword)
            .NotEmpty().WithMessage("La nueva contraseña es requerida")
            .MinimumLength(6).WithMessage("La contraseña debe tener al menos 6 caracteres")
            .Matches("[A-Z]").WithMessage("La contraseña debe contener al menos una mayúscula")
            .Matches("[a-z]").WithMessage("La contraseña debe contener al menos una minúscula")
            .Matches("[0-9]").WithMessage("La contraseña debe contener al menos un número")
            .NotEqual(x => x.PasswordActual).WithMessage("La nueva contraseña debe ser diferente a la actual");

        RuleFor(x => x.ConfirmNuevaPassword)
            .NotEmpty().WithMessage("Debe confirmar la nueva contraseña")
            .Equal(x => x.NuevaPassword).WithMessage("Las contraseñas no coinciden");
    }
}
