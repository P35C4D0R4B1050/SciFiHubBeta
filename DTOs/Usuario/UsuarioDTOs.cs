using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using SciFiHub.Domain.Enums;

namespace SciFiHub.Web.DTOs.Usuario;

/// <summary>
/// DTO para crear un nuevo usuario
/// </summary>
public record CrearUsuarioDTO
{
    [Required(ErrorMessage = "El nombre completo es requerido")]
    [StringLength(200, ErrorMessage = "El nombre no puede exceder 200 caracteres")]
    public string NombreCompleto { get; init; } = string.Empty;

    [Required(ErrorMessage = "El email es requerido")]
    [EmailAddress(ErrorMessage = "El email no es válido")]
    [StringLength(100, ErrorMessage = "El email no puede exceder 100 caracteres")]
    public string Email { get; init; } = string.Empty;

    [Required(ErrorMessage = "El username es requerido")]
    [StringLength(50, ErrorMessage = "El username no puede exceder 50 caracteres")]
    public string Username { get; init; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es requerida")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener entre 6 y 100 caracteres")]
    public string Password { get; init; } = string.Empty;

    [Required(ErrorMessage = "Debe confirmar la contraseña")]
    [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
    public string ConfirmPassword { get; init; } = string.Empty;

    [Required(ErrorMessage = "El rol es requerido")]
    [JsonPropertyName("rol")]
    public string? RolString { get; set; }
    
    // Propiedad calculada que convierte el string a enum
    [JsonIgnore]
    public RolUsuario Rol
    {
        get => string.IsNullOrEmpty(RolString) 
            ? RolUsuario.Cliente 
            : Enum.TryParse<RolUsuario>(RolString, true, out var result) 
                ? result 
                : RolUsuario.Cliente;
        set => RolString = value.ToString();
    }

    [Phone(ErrorMessage = "El teléfono no es válido")]
    [StringLength(20, ErrorMessage = "El teléfono no puede exceder 20 caracteres")]
    public string? Telefono { get; init; }
}

/// <summary>
/// DTO para actualizar un usuario existente
/// </summary>
public record ActualizarUsuarioDTO
{
    [Required(ErrorMessage = "El ID es requerido")]
    public Guid Id { get; init; }

    [Required(ErrorMessage = "El nombre completo es requerido")]
    [StringLength(200, ErrorMessage = "El nombre no puede exceder 200 caracteres")]
    public string NombreCompleto { get; init; } = string.Empty;

    [Required(ErrorMessage = "El email es requerido")]
    [EmailAddress(ErrorMessage = "El email no es válido")]
    [StringLength(100, ErrorMessage = "El email no puede exceder 100 caracteres")]
    public string Email { get; init; } = string.Empty;

    public string? Telefono { get; init; }
    
    [Required(ErrorMessage = "El rol es requerido")]
    public RolUsuario Rol { get; init; }
    
    public EstadoUsuario Estado { get; init; }
}

/// <summary>
/// DTO para mostrar información de usuario
/// </summary>
public record UsuarioDTO
{
    public Guid Id { get; init; }
    public string NombreCompleto { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Username { get; init; } = string.Empty;
    public RolUsuario Rol { get; init; }
    public EstadoUsuario Estado { get; init; }
    public string? Telefono { get; init; }
    public DateTime FechaRegistro { get; init; }
    public DateTime CreatedAt { get; init; }
    public string RolNombre => Rol.ToString();
    public string EstadoNombre => Estado.ToString();
}

/// <summary>
/// DTO para login
/// </summary>
public record LoginDTO
{
    [Required(ErrorMessage = "El username o email es requerido")]
    public string UsernameOrEmail { get; init; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es requerida")]
    public string Password { get; init; } = string.Empty;

    public bool RememberMe { get; init; }
}

/// <summary>
/// DTO para cambiar contraseña
/// </summary>
public record CambiarPasswordDTO
{
    [Required(ErrorMessage = "El ID de usuario es requerido")]
    public Guid UsuarioId { get; init; }

    [Required(ErrorMessage = "La contraseña actual es requerida")]
    public string PasswordActual { get; init; } = string.Empty;

    [Required(ErrorMessage = "La nueva contraseña es requerida")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
    public string NuevaPassword { get; init; } = string.Empty;

    [Required(ErrorMessage = "La confirmación de contraseña es requerida")]
    [Compare(nameof(NuevaPassword), ErrorMessage = "Las contraseñas no coinciden")]
    public string ConfirmNuevaPassword { get; init; } = string.Empty;
}

/// <summary>
/// DTO para filtros de búsqueda de usuarios
/// </summary>
public record UsuariosFiltroDTO
{
    public string? TextoBusqueda { get; init; }
    public RolUsuario? Rol { get; init; }
    public EstadoUsuario? Estado { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

/// <summary>
/// DTO para registro rápido de clientes desde ventas
/// </summary>
public record RegistroRapidoClienteDTO
{
    [Required(ErrorMessage = "El nombre completo es requerido")]
    [StringLength(200, ErrorMessage = "El nombre no puede exceder 200 caracteres")]
    public string NombreCompleto { get; init; } = string.Empty;

    [Required(ErrorMessage = "El email es requerido")]
    [EmailAddress(ErrorMessage = "El email no es válido")]
    [StringLength(100, ErrorMessage = "El email no puede exceder 100 caracteres")]
    public string Email { get; init; } = string.Empty;

    [Phone(ErrorMessage = "El teléfono no es válido")]
    public string? Telefono { get; init; }
}
