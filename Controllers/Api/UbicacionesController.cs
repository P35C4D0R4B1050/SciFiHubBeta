using Microsoft.AspNetCore.Mvc;
using SciFiHub.Web.Helpers;

namespace SciFiHub.Web.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class UbicacionesController : ControllerBase
{
    /// <summary>
    /// Obtener lista de departamentos
    /// </summary>
    [HttpGet("departamentos")]
    public IActionResult ObtenerDepartamentos()
    {
        var departamentos = UbicacionPeruHelper.ObtenerDepartamentos()
            .Select(d => new { id = d.Id, nombre = d.Nombre })
            .ToList();

        return Ok(new { success = true, data = departamentos });
    }

    /// <summary>
    /// Obtener provincias de un departamento
    /// </summary>
    [HttpGet("departamentos/{departamentoId}/provincias")]
    public IActionResult ObtenerProvincias(string departamentoId)
    {
        var departamento = UbicacionPeruHelper.ObtenerDepartamentos()
            .FirstOrDefault(d => d.Id == departamentoId);

        if (departamento == null)
        {
            return NotFound(new { success = false, error = "Departamento no encontrado" });
        }

        var provincias = departamento.Provincias
            .Select(p => new { id = p.Id, nombre = p.Nombre })
            .ToList();

        return Ok(new { success = true, data = provincias });
    }

    /// <summary>
    /// Obtener distritos de una provincia
    /// </summary>
    [HttpGet("provincias/{provinciaId}/distritos")]
    public IActionResult ObtenerDistritos(string provinciaId)
    {
        var provincia = UbicacionPeruHelper.ObtenerDepartamentos()
            .SelectMany(d => d.Provincias)
            .FirstOrDefault(p => p.Id == provinciaId);

        if (provincia == null)
        {
            return NotFound(new { success = false, error = "Provincia no encontrada" });
        }

        var distritos = provincia.Distritos
            .Select(d => new { id = d.Id, nombre = d.Nombre })
            .ToList();

        return Ok(new { success = true, data = distritos });
    }
}
