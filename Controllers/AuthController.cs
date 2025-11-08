using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SciFiHub.Web.Controllers.Common;
using SciFiHub.Web.DTOs.Usuario;
using SciFiHub.Web.Services.Interfaces;

namespace SciFiHub.Web.Controllers;

/// <summary>
/// Controller para autenticación y gestión de sesiones
/// </summary>
public class AuthController : BaseController
{
    private readonly IAuthService _authService;
    private readonly IUsuarioService _usuarioService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IAuthService authService,
        IUsuarioService usuarioService,
        ILogger<AuthController> logger)
    {
        _authService = authService;
        _usuarioService = usuarioService;
        _logger = logger;
    }

    // GET: /Auth/Login
    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    // POST: /Auth/Login
    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginDTO model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _authService.LoginAsync(model);

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Error al iniciar sesión");
            return View(model);
        }

        var usuario = result.Data!;

        // Crear claims del usuario
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Name, usuario.Username),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Role, usuario.Rol.ToString()),
            new Claim("NombreCompleto", usuario.NombreCompleto)
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        var authProperties = new AuthenticationProperties
        {
            IsPersistent = model.RememberMe,
            ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(30) : DateTimeOffset.UtcNow.AddHours(8),
            AllowRefresh = true
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            claimsPrincipal,
            authProperties);

        _logger.LogInformation("Usuario {Username} inició sesión exitosamente", usuario.Username);

        // Redirigir según el rol del usuario
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return usuario.Rol switch
        {
            Domain.Enums.RolUsuario.Administrador => RedirectToAction("Index", "Admin"),
            Domain.Enums.RolUsuario.Vendedor => RedirectToAction("Index", "Vendedor"),
            Domain.Enums.RolUsuario.Cliente => RedirectToAction("Index", "Catalogo"),
            _ => RedirectToAction("Index", "Home")
        };
    }

    // GET: /Auth/Register
    [AllowAnonymous]
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    // POST: /Auth/Register
    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(CrearUsuarioDTO crearUsuarioDto)
    {
        try
        {
            // Validación manual
            if (!ModelState.IsValid)
            {
                return View(crearUsuarioDto);
            }

            // Validaciones adicionales de negocio (unicidad)
            var emailExists = await _usuarioService.EmailExisteAsync(crearUsuarioDto.Email);
            if (emailExists)
            {
                ModelState.AddModelError("Email", "El email ya está registrado");
                return View(crearUsuarioDto);
            }

            var usernameExists = await _usuarioService.UsernameExisteAsync(crearUsuarioDto.Username);
            if (usernameExists)
            {
                ModelState.AddModelError("Username", "El nombre de usuario ya está en uso");
                return View(crearUsuarioDto);
            }

            // Crear nuevo DTO con rol de Cliente forzado
            var dtoConRol = new CrearUsuarioDTO
            {
                NombreCompleto = crearUsuarioDto.NombreCompleto,
                Email = crearUsuarioDto.Email,
                Username = crearUsuarioDto.Username,
                Password = crearUsuarioDto.Password,
                ConfirmPassword = crearUsuarioDto.ConfirmPassword,
                RolString = "Cliente",
                Telefono = crearUsuarioDto.Telefono
            };

            var result = await _authService.RegisterAsync(dtoConRol);

            if (!result.Success)
            {
                AddErrorMessage(result.ErrorMessage ?? "Error al registrar el usuario");
                return View(crearUsuarioDto);
            }

            AddSuccessMessage("Registro exitoso. Por favor inicia sesión.");
            return RedirectToAction(nameof(Login));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en Register");
            AddErrorMessage("Error al procesar el registro");
            return View(crearUsuarioDto);
        }
    }

    // POST: /Auth/Logout
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        var username = User.Identity?.Name ?? "Usuario desconocido";

        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        _logger.LogInformation("Usuario {Username} cerró sesión", username);

        return RedirectToAction("Index", "Home");
    }

    // GET: /Auth/AccessDenied
    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    // GET: /Auth/Profile
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        if (!CurrentUserId.HasValue)
        {
            return RedirectToAction(nameof(Login));
        }

        var result = await _authService.GetUsuarioPorIdAsync(CurrentUserId.Value);

        if (!result.Success)
        {
            return NotFound();
        }

        return View(result.Data);
    }

    // GET: /Auth/ChangePassword
    [Authorize]
    [HttpGet]
    public IActionResult ChangePassword()
    {
        return View();
    }

    // POST: /Auth/ChangePassword
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(CambiarPasswordDTO model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (!CurrentUserId.HasValue)
        {
            return RedirectToAction(nameof(Login));
        }

        var modelConId = model with { UsuarioId = CurrentUserId.Value };

        var result = await _authService.CambiarPasswordAsync(modelConId);

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Error al cambiar la contraseña");
            return View(model);
        }

        AddSuccessMessage("Contraseña cambiada exitosamente");
        return RedirectToAction(nameof(Profile));
    }

    // Helper para obtener información del usuario actual
    [Authorize]
    [HttpGet]
    public IActionResult GetCurrentUser()
    {
        var userInfo = new
        {
            Id = CurrentUserId,
            Username = User.Identity?.Name,
            Email = CurrentUserEmail,
            Role = CurrentUserRole,
            IsAuthenticated = User.Identity?.IsAuthenticated ?? false
        };

        return Json(userInfo);
    }
}
