using SciFiHub.Web.DTOs.Common;
using SciFiHub.Web.DTOs.Usuario;
using SciFiHub.Domain.Enums;

namespace SciFiHub.Web.Services.Interfaces;

/// <summary>
/// Servicio para gestión completa de usuarios (Admin)
/// </summary>
public interface IUsuarioService
{
    /// <summary>
    /// Crear un nuevo usuario (por Admin/Vendedor)
    /// </summary>
    Task<Result<UsuarioDTO>> CrearUsuarioAsync(CrearUsuarioDTO crearUsuarioDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Actualizar usuario existente
    /// </summary>
    Task<Result<UsuarioDTO>> ActualizarUsuarioAsync(ActualizarUsuarioDTO actualizarUsuarioDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Eliminar usuario (soft delete)
    /// </summary>
    Task<Result> EliminarUsuarioAsync(Guid usuarioId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtener usuario por ID
    /// </summary>
    Task<Result<UsuarioDTO>> ObtenerUsuarioPorIdAsync(Guid usuarioId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtener usuario por Username
    /// </summary>
    Task<Result<UsuarioDTO>> ObtenerUsuarioPorUsernameAsync(string username, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtener usuario por Email
    /// </summary>
    Task<Result<UsuarioDTO>> ObtenerUsuarioPorEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtener todos los usuarios con filtros
    /// </summary>
    Task<Result<PagedResult<UsuarioDTO>>> ObtenerUsuariosAsync(UsuariosFiltroDTO filtro, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtener todos los usuarios con filtros y paginación
    /// </summary>
    Task<Result<PagedResult<UsuarioDTO>>> ObtenerUsuariosPaginadosAsync(UsuariosFiltroDTO filtro, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtener clientes activos (para selects)
    /// </summary>
    Task<Result<IEnumerable<UsuarioDTO>>> ObtenerClientesActivosAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Cambiar estado de usuario (Activo/Inactivo)
    /// </summary>
    Task<Result> CambiarEstadoUsuarioAsync(Guid usuarioId, Domain.Enums.EstadoUsuario nuevoEstado, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica si el email ya existe
    /// </summary>
    Task<bool> EmailExisteAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica si el username ya existe
    /// </summary>
    Task<bool> UsernameExisteAsync(string username, CancellationToken cancellationToken = default);

    /// <summary>
    /// Valida credenciales de usuario
    /// </summary>
    Task<bool> ValidarCredencialesAsync(string usernameOrEmail, string password, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Obtener usuarios eliminados (soft delete)
    /// </summary>
    Task<Result<IEnumerable<UsuarioDTO>>> ObtenerUsuariosEliminadosAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Eliminar usuario definitivamente de la BD (hard delete)
    /// </summary>
    Task<Result> EliminarDefinitivamenteAsync(Guid usuarioId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Restaurar usuario eliminado
    /// </summary>
    Task<Result> RestaurarUsuarioAsync(Guid usuarioId, CancellationToken cancellationToken = default);
}
