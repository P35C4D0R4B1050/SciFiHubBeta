using SciFiHub.Web.DTOs.Common;
using SciFiHub.Web.DTOs.Usuario;

namespace SciFiHub.Web.Services.Interfaces;

/// <summary>
/// Servicio de autenticación y gestión de usuarios
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Autenticar usuario con username/email y contraseña
    /// </summary>
    Task<Result<UsuarioDTO>> LoginAsync(LoginDTO loginDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registrar un nuevo usuario (cliente)
    /// </summary>
    Task<Result<UsuarioDTO>> RegisterAsync(CrearUsuarioDTO crearUsuarioDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cambiar contraseña de usuario
    /// </summary>
    Task<Result> CambiarPasswordAsync(CambiarPasswordDTO cambiarPasswordDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtener usuario por ID
    /// </summary>
    Task<Result<UsuarioDTO>> GetUsuarioPorIdAsync(Guid usuarioId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtener usuario por username
    /// </summary>
    Task<Result<UsuarioDTO>> GetUsuarioPorUsernameAsync(string username, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validar si un usuario tiene un rol específico
    /// </summary>
    Task<bool> UsuarioTieneRolAsync(Guid usuarioId, Domain.Enums.RolUsuario rol, CancellationToken cancellationToken = default);
}
