using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SciFiHub.Web.DTOs.Usuario;
using SciFiHub.Web.Services.Interfaces;

namespace SciFiHub.Web.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador")]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;
    private readonly ILogger<UsuariosController> _logger;

    public UsuariosController(
        IUsuarioService usuarioService,
        ILogger<UsuariosController> logger)
    {
        _usuarioService = usuarioService;
        _logger = logger;
    }

    // POST: api/Usuarios
    [HttpPost]
    public async Task<IActionResult> CrearUsuario([FromBody] CrearUsuarioDTO model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                
                return BadRequest(new { 
                    success = false,
                    error = "Datos inválidos", 
                    errors = errors 
                });
            }

            var result = await _usuarioService.CrearUsuarioAsync(model);

            if (!result.Success)
            {
                return BadRequest(new { 
                    success = false,
                    error = result.ErrorMessage 
                });
            }

            return Ok(new
            {
                success = true,
                message = "Usuario creado exitosamente",
                data = result.Data
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear usuario");
            return StatusCode(500, new { 
                success = false,
                error = $"Error interno del servidor: {ex.Message}" 
            });
        }
    }

    // PUT: api/Usuarios/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> ActualizarUsuario(Guid id, [FromBody] ActualizarUsuarioDTO model)
    {
        try
        {
            if (id != model.Id)
            {
                return BadRequest(new { 
                    success = false,
                    error = "El ID no coincide" 
                });
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                
                return BadRequest(new { 
                    success = false,
                    error = "Datos inválidos", 
                    errors = errors 
                });
            }

            var result = await _usuarioService.ActualizarUsuarioAsync(model);

            if (!result.Success)
            {
                return BadRequest(new { 
                    success = false,
                    error = result.ErrorMessage 
                });
            }

            return Ok(new
            {
                success = true,
                message = "Usuario actualizado exitosamente",
                data = result.Data
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar usuario {UsuarioId}", id);
            return StatusCode(500, new { 
                success = false,
                error = $"Error interno del servidor: {ex.Message}" 
            });
        }
    }

    // DELETE: api/Usuarios/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> EliminarUsuario(Guid id)
    {
        try
        {
            var result = await _usuarioService.EliminarUsuarioAsync(id);

            if (!result.Success)
            {
                return BadRequest(new { 
                    success = false,
                    error = result.ErrorMessage 
                });
            }

            return Ok(new
            {
                success = true,
                message = "Usuario eliminado exitosamente"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar usuario {UsuarioId}", id);
            return StatusCode(500, new { 
                success = false,
                error = $"Error al eliminar el usuario: {ex.Message}" 
            });
        }
    }

    // GET: api/Usuarios/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerUsuario(Guid id)
    {
        try
        {
            var result = await _usuarioService.ObtenerUsuarioPorIdAsync(id);

            if (!result.Success)
            {
                return NotFound(new { error = "Usuario no encontrado" });
            }

            return Ok(new
            {
                success = true,
                data = result.Data
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener usuario {UsuarioId}", id);
            return StatusCode(500, new { error = "Error al obtener el usuario" });
        }
    }

    // PUT: api/Usuarios/{id}/estado
    [HttpPut("{id}/estado")]
    public async Task<IActionResult> CambiarEstado(Guid id, [FromBody] CambiarEstadoRequest request)
    {
        try
        {
            var result = await _usuarioService.CambiarEstadoUsuarioAsync(id, request.Estado);

            if (!result.Success)
            {
                return BadRequest(new { error = result.ErrorMessage });
            }

            return Ok(new
            {
                success = true,
                message = "Estado del usuario actualizado exitosamente"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cambiar estado del usuario {UsuarioId}", id);
            return StatusCode(500, new { error = "Error al cambiar el estado del usuario" });
        }
    }

    // GET: api/Usuarios/verificar-email
    [HttpGet("verificar-email")]
    public async Task<IActionResult> VerificarEmail([FromQuery] string email)
    {
        try
        {
            var existe = await _usuarioService.EmailExisteAsync(email);

            return Ok(new
            {
                success = true,
                existe = existe
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al verificar email");
            return StatusCode(500, new { error = "Error al verificar email" });
        }
    }

    // GET: api/Usuarios/verificar-username
    [HttpGet("verificar-username")]
    public async Task<IActionResult> VerificarUsername([FromQuery] string username)
    {
        try
        {
            var existe = await _usuarioService.UsernameExisteAsync(username);

            return Ok(new
            {
                success = true,
                existe = existe
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al verificar username");
            return StatusCode(500, new { error = "Error al verificar username" });
        }
    }

    // GET: api/Usuarios/clientes-activos
    [HttpGet("clientes-activos")]
    public async Task<IActionResult> ObtenerClientesActivos()
    {
        try
        {
            var result = await _usuarioService.ObtenerClientesActivosAsync();

            if (!result.Success)
            {
                return BadRequest(new { error = result.ErrorMessage });
            }

            return Ok(new
            {
                success = true,
                data = result.Data
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener clientes activos");
            return StatusCode(500, new { error = "Error al obtener clientes activos" });
        }
    }

    // POST: api/Usuarios/registro-rapido
    [HttpPost("registro-rapido")]
    [Authorize(Roles = "Vendedor,Administrador")]
    public async Task<IActionResult> RegistroRapido([FromBody] RegistroRapidoClienteDTO model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                
                return BadRequest(new { 
                    success = false,
                    error = "Datos inválidos", 
                    errors = errors 
                });
            }

            // Generar username desde el email
            var username = GenerarUsernameDesdeEmail(model.Email);
            
            // Generar contraseña temporal
            var passwordTemporal = GenerarPasswordTemporal();

            // Crear DTO completo
            var crearUsuarioDto = new CrearUsuarioDTO
            {
                NombreCompleto = model.NombreCompleto,
                Email = model.Email,
                Username = username,
                Password = passwordTemporal,
                ConfirmPassword = passwordTemporal,
                RolString = "Cliente",
                Telefono = model.Telefono
            };

            var result = await _usuarioService.CrearUsuarioAsync(crearUsuarioDto);

            if (!result.Success)
            {
                return BadRequest(new { 
                    success = false,
                    error = result.ErrorMessage 
                });
            }

            return Ok(new
            {
                success = true,
                message = "Cliente registrado exitosamente",
                data = result.Data,
                credenciales = new
                {
                    username = username,
                    passwordTemporal = passwordTemporal,
                    mensaje = "Credenciales generadas automáticamente. Entregar al cliente."
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en RegistroRapido");
            return StatusCode(500, new { 
                success = false,
                error = $"Error interno del servidor: {ex.Message}" 
            });
        }
    }
    
    // GET: api/Usuarios/eliminados
    [HttpGet("eliminados")]
    public async Task<IActionResult> ObtenerUsuariosEliminados()
    {
        try
        {
            var result = await _usuarioService.ObtenerUsuariosEliminadosAsync();

            if (!result.Success)
            {
                return BadRequest(new { 
                    success = false,
                    error = result.ErrorMessage 
                });
            }

            return Ok(new
            {
                success = true,
                data = result.Data
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener usuarios eliminados");
            return StatusCode(500, new { 
                success = false,
                error = "Error al obtener usuarios eliminados" 
            });
        }
    }
    
    // DELETE: api/Usuarios/{id}/definitivo
    [HttpDelete("{id}/definitivo")]
    public async Task<IActionResult> EliminarDefinitivo(Guid id)
    {
        try
        {
            var result = await _usuarioService.EliminarDefinitivamenteAsync(id);

            if (!result.Success)
            {
                return BadRequest(new { 
                    success = false,
                    error = result.ErrorMessage 
                });
            }

            return Ok(new
            {
                success = true,
                message = "Usuario eliminado definitivamente de la base de datos"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar definitivamente usuario {UsuarioId}", id);
            return StatusCode(500, new { 
                success = false,
                error = "Error al eliminar definitivamente el usuario" 
            });
        }
    }
    
    // POST: api/Usuarios/{id}/restaurar
    [HttpPost("{id}/restaurar")]
    public async Task<IActionResult> RestaurarUsuario(Guid id)
    {
        try
        {
            var result = await _usuarioService.RestaurarUsuarioAsync(id);

            if (!result.Success)
            {
                return BadRequest(new { 
                    success = false,
                    error = result.ErrorMessage 
                });
            }

            return Ok(new
            {
                success = true,
                message = "Usuario restaurado exitosamente"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al restaurar usuario {UsuarioId}", id);
            return StatusCode(500, new { 
                success = false,
                error = "Error al restaurar el usuario" 
            });
        }
    }
    
    // Métodos privados auxiliares
    private string GenerarUsernameDesdeEmail(string email)
    {
        var username = email.Split('@')[0];
        var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
        return $"{username}_{timestamp}";
    }

    private string GenerarPasswordTemporal()
    {
        return Guid.NewGuid().ToString("N").Substring(0, 10);
    }
}

// Request DTO para cambiar estado
public record CambiarEstadoRequest
{
    public SciFiHub.Domain.Enums.EstadoUsuario Estado { get; init; }
}
