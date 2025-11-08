using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SciFiHub.Domain.Interfaces;
using SciFiHub.Web.DTOs.Common;
using SciFiHub.Web.DTOs.Usuario;
using SciFiHub.Web.Services.Interfaces;

namespace SciFiHub.Web.Services;

/// <summary>
/// Implementación del servicio de gestión de usuarios
/// </summary>
public class UsuarioService : IUsuarioService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<UsuarioService> _logger;

    public UsuarioService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<UsuarioService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<bool> EmailExisteAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _unitOfWork.Usuarios.EmailExistsAsync(email, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en EmailExisteAsync");
            return false;
        }
    }

    public async Task<bool> UsernameExisteAsync(string username, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _unitOfWork.Usuarios.UsernameExistsAsync(username, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en UsernameExisteAsync");
            return false;
        }
    }

    public async Task<Result<UsuarioDTO>> CrearUsuarioAsync(CrearUsuarioDTO crearUsuarioDto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Iniciando creación de usuario: {Username}", crearUsuarioDto.Username);
            
            // Validar email único
            if (await _unitOfWork.Usuarios.EmailExistsAsync(crearUsuarioDto.Email, cancellationToken))
            {
                _logger.LogWarning("Intento de crear usuario con email duplicado: {Email}", crearUsuarioDto.Email);
                return Result<UsuarioDTO>.FailureResult("El email ya está registrado");
            }

            // Validar username único
            if (await _unitOfWork.Usuarios.UsernameExistsAsync(crearUsuarioDto.Username, cancellationToken))
            {
                _logger.LogWarning("Intento de crear usuario con username duplicado: {Username}", crearUsuarioDto.Username);
                return Result<UsuarioDTO>.FailureResult("El username ya está en uso");
            }

            var usuario = _mapper.Map<Domain.Entities.Usuario>(crearUsuarioDto);
            usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(crearUsuarioDto.Password);
            usuario.FechaRegistro = DateTime.UtcNow;

            await _unitOfWork.Usuarios.AddAsync(usuario, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            var usuarioDto = _mapper.Map<UsuarioDTO>(usuario);
            _logger.LogInformation("Usuario {Username} creado exitosamente con ID: {Id}", usuario.Username, usuario.Id);

            return Result<UsuarioDTO>.SuccessResult(usuarioDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en CrearUsuarioAsync para usuario {Username}", crearUsuarioDto.Username);
            return Result<UsuarioDTO>.FailureResult($"Error al crear el usuario: {ex.Message}");
        }
    }

    public async Task<Result<UsuarioDTO>> ActualizarUsuarioAsync(ActualizarUsuarioDTO actualizarUsuarioDto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Iniciando actualización de usuario: {Id}", actualizarUsuarioDto.Id);
            
            var usuario = await _unitOfWork.Usuarios.GetByIdAsync(actualizarUsuarioDto.Id, cancellationToken);

            if (usuario == null)
            {
                return Result<UsuarioDTO>.FailureResult("Usuario no encontrado");
            }

            // Validar email único (excepto el propio usuario)
            var usuarioConEmail = await _unitOfWork.Usuarios.GetByEmailAsync(actualizarUsuarioDto.Email, cancellationToken);
            if (usuarioConEmail != null && usuarioConEmail.Id != usuario.Id)
            {
                _logger.LogWarning("Intento de usar email duplicado en actualización: {Email}", actualizarUsuarioDto.Email);
                return Result<UsuarioDTO>.FailureResult("El email ya está registrado por otro usuario");
            }

            // Validar que no se cambie el rol del último administrador
            string rolAdministrador = Domain.Enums.RolUsuario.Administrador.ToString();
            if (usuario.Rol == rolAdministrador && actualizarUsuarioDto.Rol.ToString() != rolAdministrador)
            {
                var totalAdmins = await _unitOfWork.Usuarios.Query()
                    .CountAsync(u => u.Rol == rolAdministrador && !u.IsDeleted, cancellationToken);
                
                if (totalAdmins <= 1)
                {
                    _logger.LogWarning("Intento de cambiar el rol del último administrador del sistema");
                    return Result<UsuarioDTO>.FailureResult("No se puede cambiar el rol del único administrador del sistema");
                }
            }

            // Actualizar propiedades usando AutoMapper
            _mapper.Map(actualizarUsuarioDto, usuario);

            await _unitOfWork.Usuarios.UpdateAsync(usuario, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            var usuarioDto = _mapper.Map<UsuarioDTO>(usuario);
            _logger.LogInformation("Usuario {Username} actualizado exitosamente", usuario.Username);

            return Result<UsuarioDTO>.SuccessResult(usuarioDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en ActualizarUsuarioAsync para usuario {Id}", actualizarUsuarioDto.Id);
            return Result<UsuarioDTO>.FailureResult($"Error al actualizar el usuario: {ex.Message}");
        }
    }

    public async Task<Result> EliminarUsuarioAsync(Guid usuarioId, CancellationToken cancellationToken = default)
    {
        try
        {
            var usuario = await _unitOfWork.Usuarios.GetByIdAsync(usuarioId, cancellationToken);

            if (usuario == null)
            {
                return Result.FailureResult("Usuario no encontrado");
            }

            // Verificar que no sea el último administrador
            string rolAdministrador = Domain.Enums.RolUsuario.Administrador.ToString();
            if (usuario.Rol == rolAdministrador)
            {
                var totalAdmins = await _unitOfWork.Usuarios.Query()
                    .CountAsync(u => u.Rol == rolAdministrador && !u.IsDeleted, cancellationToken);
                
                if (totalAdmins <= 1)
                {
                    _logger.LogWarning("Intento de eliminar el último administrador del sistema: {Username}", usuario.Username);
                    return Result.FailureResult("No se puede eliminar el último administrador del sistema");
                }
            }

            // Soft delete
            await _unitOfWork.Usuarios.DeleteAsync(usuario, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            _logger.LogInformation("Usuario {Username} eliminado (soft delete)", usuario.Username);

            return Result.SuccessResult();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en EliminarUsuarioAsync para usuario {UsuarioId}", usuarioId);
            return Result.FailureResult($"Error al eliminar el usuario: {ex.Message}");
        }
    }

    public async Task<Result<UsuarioDTO>> ObtenerUsuarioPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var usuario = await _unitOfWork.Usuarios.GetByIdAsync(id, cancellationToken);

            if (usuario == null)
            {
                return Result<UsuarioDTO>.FailureResult("Usuario no encontrado");
            }

            var usuarioDto = _mapper.Map<UsuarioDTO>(usuario);
            return Result<UsuarioDTO>.SuccessResult(usuarioDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en ObtenerUsuarioPorIdAsync");
            return Result<UsuarioDTO>.FailureResult("Error al obtener el usuario");
        }
    }

    public async Task<Result<UsuarioDTO>> ObtenerUsuarioPorUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        try
        {
            var usuario = await _unitOfWork.Usuarios.GetByUsernameAsync(username, cancellationToken);

            if (usuario == null)
            {
                return Result<UsuarioDTO>.FailureResult("Usuario no encontrado");
            }

            var usuarioDto = _mapper.Map<UsuarioDTO>(usuario);
            return Result<UsuarioDTO>.SuccessResult(usuarioDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en ObtenerUsuarioPorUsernameAsync");
            return Result<UsuarioDTO>.FailureResult("Error al obtener el usuario");
        }
    }

    public async Task<Result<UsuarioDTO>> ObtenerUsuarioPorEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            var usuario = await _unitOfWork.Usuarios.GetByEmailAsync(email, cancellationToken);

            if (usuario == null)
            {
                return Result<UsuarioDTO>.FailureResult("Usuario no encontrado");
            }

            var usuarioDto = _mapper.Map<UsuarioDTO>(usuario);
            return Result<UsuarioDTO>.SuccessResult(usuarioDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en ObtenerUsuarioPorEmailAsync");
            return Result<UsuarioDTO>.FailureResult("Error al obtener el usuario");
        }
    }

    public async Task<bool> ValidarCredencialesAsync(string usernameOrEmail, string password, CancellationToken cancellationToken = default)
    {
        try
        {
            var usuario = await _unitOfWork.Usuarios.GetByUsernameAsync(usernameOrEmail, cancellationToken)
                       ?? await _unitOfWork.Usuarios.GetByEmailAsync(usernameOrEmail, cancellationToken);

            if (usuario == null)
            {
                return false;
            }

            return BCrypt.Net.BCrypt.Verify(password, usuario.PasswordHash);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en ValidarCredencialesAsync");
            return false;
        }
    }

    public async Task<Result<PagedResult<UsuarioDTO>>> ObtenerUsuariosAsync(UsuariosFiltroDTO filtro, CancellationToken cancellationToken = default)
    {
        return await ObtenerUsuariosPaginadosAsync(filtro, cancellationToken);
    }

    public async Task<Result<PagedResult<UsuarioDTO>>> ObtenerUsuariosPaginadosAsync(UsuariosFiltroDTO filtro, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = _unitOfWork.Usuarios.Query();

            // Aplicar filtros
            if (!string.IsNullOrWhiteSpace(filtro.TextoBusqueda))
            {
                query = query.Where(u =>
                    u.NombreCompleto.Contains(filtro.TextoBusqueda) ||
                    u.Email.Contains(filtro.TextoBusqueda) ||
                    u.Username.Contains(filtro.TextoBusqueda));
            }

            if (filtro.Rol.HasValue)
            {
                string rolFiltro = filtro.Rol.Value.ToString();
                query = query.Where(u => u.Rol == rolFiltro);
            }

            if (filtro.Estado.HasValue)
            {
                string estadoFiltro = filtro.Estado.Value.ToString();
                query = query.Where(u => u.Estado == estadoFiltro);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var usuarios = await query
                .OrderBy(u => u.NombreCompleto)
                .Skip((filtro.PageNumber - 1) * filtro.PageSize)
                .Take(filtro.PageSize)
                .ToListAsync(cancellationToken);

            var usuariosDto = _mapper.Map<IEnumerable<UsuarioDTO>>(usuarios);

            var result = new PagedResult<UsuarioDTO>
            {
                Items = usuariosDto,
                PageNumber = filtro.PageNumber,
                PageSize = filtro.PageSize,
                TotalCount = totalCount
            };

            return Result<PagedResult<UsuarioDTO>>.SuccessResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en ObtenerUsuariosPaginadosAsync");
            return Result<PagedResult<UsuarioDTO>>.FailureResult("Error al obtener usuarios");
        }
    }

    public async Task<Result<IEnumerable<UsuarioDTO>>> ObtenerClientesActivosAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var clientes = await _unitOfWork.Usuarios.GetClientesActivosAsync(cancellationToken);
            var clientesDto = _mapper.Map<IEnumerable<UsuarioDTO>>(clientes);

            return Result<IEnumerable<UsuarioDTO>>.SuccessResult(clientesDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en ObtenerClientesActivosAsync");
            return Result<IEnumerable<UsuarioDTO>>.FailureResult("Error al obtener clientes activos");
        }
    }

    public async Task<Result> CambiarEstadoUsuarioAsync(Guid usuarioId, Domain.Enums.EstadoUsuario nuevoEstado, CancellationToken cancellationToken = default)
    {
        try
        {
            var usuario = await _unitOfWork.Usuarios.GetByIdAsync(usuarioId, cancellationToken);

            if (usuario == null)
            {
                return Result.FailureResult("Usuario no encontrado");
            }

            usuario.Estado = nuevoEstado.ToString();
            await _unitOfWork.Usuarios.UpdateAsync(usuario, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            _logger.LogInformation("Estado de usuario {Username} cambiado a {Estado}", usuario.Username, nuevoEstado);

            return Result.SuccessResult();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en CambiarEstadoUsuarioAsync");
            return Result.FailureResult("Error al cambiar el estado del usuario");
        }
    }
    
    public async Task<Result<IEnumerable<UsuarioDTO>>> ObtenerUsuariosEliminadosAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // Obtener usuarios con IsDeleted = true, ignorando el query filter
            var usuarios = await _unitOfWork.Usuarios.Query()
                .IgnoreQueryFilters()
                .Where(u => u.IsDeleted)
                .OrderByDescending(u => u.UpdatedAt)
                .ToListAsync(cancellationToken);

            var usuariosDto = _mapper.Map<IEnumerable<UsuarioDTO>>(usuarios);

            return Result<IEnumerable<UsuarioDTO>>.SuccessResult(usuariosDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en ObtenerUsuariosEliminadosAsync");
            return Result<IEnumerable<UsuarioDTO>>.FailureResult("Error al obtener usuarios eliminados");
        }
    }
    
    public async Task<Result> EliminarDefinitivamenteAsync(Guid usuarioId, CancellationToken cancellationToken = default)
    {
        try
        {
            // Obtener usuario ignorando el filtro de soft delete
            var usuario = await _unitOfWork.Usuarios.Query()
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.Id == usuarioId, cancellationToken);

            if (usuario == null)
            {
                return Result.FailureResult("Usuario no encontrado");
            }

            if (!usuario.IsDeleted)
            {
                return Result.FailureResult("Solo se pueden eliminar definitivamente usuarios que ya están en la papelera");
            }

            // Eliminar definitivamente usando el método HardDeleteAsync del repositorio
            await _unitOfWork.Usuarios.HardDeleteAsync(usuario, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            _logger.LogWarning("Usuario {Username} (ID: {Id}) eliminado definitivamente de la BD", usuario.Username, usuario.Id);

            return Result.SuccessResult();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en EliminarDefinitivamenteAsync para usuario {UsuarioId}", usuarioId);
            return Result.FailureResult($"Error al eliminar definitivamente: {ex.Message}");
        }
    }
    
    public async Task<Result> RestaurarUsuarioAsync(Guid usuarioId, CancellationToken cancellationToken = default)
    {
        try
        {
            var usuario = await _unitOfWork.Usuarios.Query()
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.Id == usuarioId, cancellationToken);

            if (usuario == null)
            {
                return Result.FailureResult("Usuario no encontrado");
            }

            if (!usuario.IsDeleted)
            {
                return Result.FailureResult("El usuario no está eliminado");
            }

            // Restaurar
            usuario.IsDeleted = false;
            usuario.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Usuarios.UpdateAsync(usuario, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            _logger.LogInformation("Usuario {Username} restaurado exitosamente", usuario.Username);

            return Result.SuccessResult();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en RestaurarUsuarioAsync para usuario {UsuarioId}", usuarioId);
            return Result.FailureResult("Error al restaurar el usuario");
        }
    }
}
