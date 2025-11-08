using AutoMapper;
using BCrypt.Net;
using SciFiHub.Domain.Interfaces;
using SciFiHub.Web.DTOs.Common;
using SciFiHub.Web.DTOs.Usuario;
using SciFiHub.Web.Services.Interfaces;

namespace SciFiHub.Web.Services;

/// <summary>
/// Implementación del servicio de autenticación
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<AuthService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<UsuarioDTO>> LoginAsync(LoginDTO loginDto, CancellationToken cancellationToken = default)
    {
        try
        {
            // Buscar usuario por username o email
            var usuario = await _unitOfWork.Usuarios.GetByUsernameAsync(loginDto.UsernameOrEmail, cancellationToken)
                       ?? await _unitOfWork.Usuarios.GetByEmailAsync(loginDto.UsernameOrEmail, cancellationToken);

            if (usuario == null)
            {
                _logger.LogWarning("Intento de login fallido: Usuario {UsernameOrEmail} no encontrado", loginDto.UsernameOrEmail);
                return Result<UsuarioDTO>.FailureResult("Usuario o contraseña incorrectos");
            }

            // Verificar contraseña
            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, usuario.PasswordHash))
            {
                _logger.LogWarning("Intento de login fallido: Contraseña incorrecta para usuario {Username}", usuario.Username);
                return Result<UsuarioDTO>.FailureResult("Usuario o contraseña incorrectos");
            }

            // Verificar que el usuario esté activo
            if (usuario.Estado != "Activo")
            {
                _logger.LogWarning("Intento de login fallido: Usuario {Username} está inactivo", usuario.Username);
                return Result<UsuarioDTO>.FailureResult("La cuenta está inactiva. Contacte al administrador");
            }

            var usuarioDto = _mapper.Map<UsuarioDTO>(usuario);
            _logger.LogInformation("Login exitoso para usuario {Username}", usuario.Username);

            return Result<UsuarioDTO>.SuccessResult(usuarioDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en LoginAsync");
            return Result<UsuarioDTO>.FailureResult("Error al procesar el login");
        }
    }

    public async Task<Result<UsuarioDTO>> RegisterAsync(CrearUsuarioDTO crearUsuarioDto, CancellationToken cancellationToken = default)
    {
        try
        {
            // Verificar si el email ya existe
            if (await _unitOfWork.Usuarios.EmailExistsAsync(crearUsuarioDto.Email, cancellationToken))
            {
                return Result<UsuarioDTO>.FailureResult("El email ya está registrado");
            }

            // Verificar si el username ya existe
            if (await _unitOfWork.Usuarios.UsernameExistsAsync(crearUsuarioDto.Username, cancellationToken))
            {
                return Result<UsuarioDTO>.FailureResult("El username ya está en uso");
            }

            // Crear nuevo usuario
            var usuario = new Domain.Entities.Usuario
            {
                NombreCompleto = crearUsuarioDto.NombreCompleto,
                Email = crearUsuarioDto.Email,
                Username = crearUsuarioDto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(crearUsuarioDto.Password),
                Rol = "Cliente",
                Estado = "Activo",
                FechaRegistro = DateTime.UtcNow,
                IsDeleted = false
            };

            // Guardar en la base de datos
            await _unitOfWork.Usuarios.AddAsync(usuario, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            var usuarioDto = _mapper.Map<UsuarioDTO>(usuario);
            _logger.LogInformation("Usuario {Username} registrado exitosamente", usuario.Username);

            return Result<UsuarioDTO>.SuccessResult(usuarioDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en RegisterAsync");
            return Result<UsuarioDTO>.FailureResult("Error al registrar el usuario");
        }
    }

    public async Task<Result> CambiarPasswordAsync(CambiarPasswordDTO cambiarPasswordDto, CancellationToken cancellationToken = default)
    {
        try
        {
            var usuario = await _unitOfWork.Usuarios.GetByIdAsync(cambiarPasswordDto.UsuarioId, cancellationToken);

            if (usuario == null)
            {
                return Result.FailureResult("Usuario no encontrado");
            }

            // Verificar contraseña actual
            if (!BCrypt.Net.BCrypt.Verify(cambiarPasswordDto.PasswordActual, usuario.PasswordHash))
            {
                return Result.FailureResult("La contraseña actual es incorrecta");
            }

            // Actualizar contraseña
            usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(cambiarPasswordDto.NuevaPassword);
            await _unitOfWork.Usuarios.UpdateAsync(usuario, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            _logger.LogInformation("Contraseña cambiada exitosamente para usuario {Username}", usuario.Username);

            return Result.SuccessResult();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en CambiarPasswordAsync");
            return Result.FailureResult("Error al cambiar la contraseña");
        }
    }

    public async Task<Result<UsuarioDTO>> GetUsuarioPorIdAsync(Guid usuarioId, CancellationToken cancellationToken = default)
    {
        try
        {
            var usuario = await _unitOfWork.Usuarios.GetByIdAsync(usuarioId, cancellationToken);

            if (usuario == null)
            {
                return Result<UsuarioDTO>.FailureResult("Usuario no encontrado");
            }

            var usuarioDto = _mapper.Map<UsuarioDTO>(usuario);
            return Result<UsuarioDTO>.SuccessResult(usuarioDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en GetUsuarioPorIdAsync");
            return Result<UsuarioDTO>.FailureResult("Error al obtener el usuario");
        }
    }

    public async Task<Result<UsuarioDTO>> GetUsuarioPorUsernameAsync(string username, CancellationToken cancellationToken = default)
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
            _logger.LogError(ex, "Error en GetUsuarioPorUsernameAsync");
            return Result<UsuarioDTO>.FailureResult("Error al obtener el usuario");
        }
    }

    public async Task<bool> UsuarioTieneRolAsync(Guid usuarioId, Domain.Enums.RolUsuario rol, CancellationToken cancellationToken = default)
    {
        try
        {
            var usuario = await _unitOfWork.Usuarios.GetByIdAsync(usuarioId, cancellationToken);
            string rolString = rol == Domain.Enums.RolUsuario.Administrador ? "Administrador" :
                             rol == Domain.Enums.RolUsuario.Vendedor ? "Vendedor" : "Cliente";
            return usuario != null && usuario.Rol == rolString;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en UsuarioTieneRolAsync");
            return false;
        }
    }
}
