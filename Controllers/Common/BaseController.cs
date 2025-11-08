using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SciFiHub.Domain.Enums;

namespace SciFiHub.Web.Controllers.Common;

/// <summary>
/// Controller base con métodos helper comunes para todos los controllers
/// </summary>
public abstract class BaseController : Controller
{
    /// <summary>
    /// Obtiene el ID del usuario actual autenticado
    /// </summary>
    protected Guid? CurrentUserId
    {
        get
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
        }
    }

    /// <summary>
    /// Obtiene el username del usuario actual
    /// </summary>
    protected string? CurrentUsername => User.FindFirstValue(ClaimTypes.Name);

    /// <summary>
    /// Obtiene el email del usuario actual
    /// </summary>
    protected string? CurrentUserEmail => User.FindFirstValue(ClaimTypes.Email);

    /// <summary>
    /// Obtiene el nombre completo del usuario actual
    /// </summary>
    protected string? CurrentUserFullName => User.FindFirstValue("NombreCompleto");

    /// <summary>
    /// Obtiene el rol del usuario actual
    /// </summary>
    protected RolUsuario? CurrentUserRole
    {
        get
        {
            var roleClaim = User.FindFirstValue(ClaimTypes.Role);
            return Enum.TryParse<RolUsuario>(roleClaim, out var role) ? role : null;
        }
    }

    /// <summary>
    /// Verifica si el usuario actual es Administrador
    /// </summary>
    protected bool IsAdmin => CurrentUserRole == RolUsuario.Administrador;

    /// <summary>
    /// Verifica si el usuario actual es Vendedor
    /// </summary>
    protected bool IsVendedor => CurrentUserRole == RolUsuario.Vendedor;

    /// <summary>
    /// Verifica si el usuario actual es Cliente
    /// </summary>
    protected bool IsCliente => CurrentUserRole == RolUsuario.Cliente;

    /// <summary>
    /// Verifica si el usuario tiene permisos de administración o ventas
    /// </summary>
    protected bool CanManageStore => IsAdmin || IsVendedor;

    /// <summary>
    /// Agrega un mensaje de éxito a TempData
    /// </summary>
    protected void AddSuccessMessage(string message)
    {
        TempData["SuccessMessage"] = message;
    }

    /// <summary>
    /// Agrega un mensaje de error a TempData
    /// </summary>
    protected void AddErrorMessage(string message)
    {
        TempData["ErrorMessage"] = message;
    }

    /// <summary>
    /// Agrega un mensaje de información a TempData
    /// </summary>
    protected void AddInfoMessage(string message)
    {
        TempData["InfoMessage"] = message;
    }

    /// <summary>
    /// Agrega un mensaje de advertencia a TempData
    /// </summary>
    protected void AddWarningMessage(string message)
    {
        TempData["WarningMessage"] = message;
    }

    /// <summary>
    /// Retorna JSON con resultado de éxito
    /// </summary>
    protected JsonResult JsonSuccess(object? data = null, string? message = null)
    {
        return Json(new
        {
            success = true,
            message,
            data
        });
    }

    /// <summary>
    /// Retorna JSON con resultado de error
    /// </summary>
    protected JsonResult JsonError(string message, object? errors = null)
    {
        return Json(new
        {
            success = false,
            message,
            errors
        });
    }

    /// <summary>
    /// Valida que el usuario esté autenticado, si no redirige al login
    /// </summary>
    protected IActionResult? RequireAuthentication()
    {
        if (!User.Identity?.IsAuthenticated ?? true)
        {
            return RedirectToAction("Login", "Auth");
        }
        return null;
    }

    /// <summary>
    /// Valida que el usuario tenga un rol específico
    /// </summary>
    protected IActionResult? RequireRole(RolUsuario requiredRole)
    {
        if (CurrentUserRole != requiredRole)
        {
            return RedirectToAction("AccessDenied", "Auth");
        }
        return null;
    }

    /// <summary>
    /// Valida que el usuario tenga alguno de los roles especificados
    /// </summary>
    protected IActionResult? RequireAnyRole(params RolUsuario[] roles)
    {
        if (!roles.Contains(CurrentUserRole ?? (RolUsuario)(-1)))
        {
            return RedirectToAction("AccessDenied", "Auth");
        }
        return null;
    }
}
