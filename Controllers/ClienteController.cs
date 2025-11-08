using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SciFiHub.Web.Controllers.Common;
using SciFiHub.Web.DTOs.Venta;
using SciFiHub.Web.Services.Interfaces;

namespace SciFiHub.Web.Controllers;

[Authorize(Roles = "Cliente")]
public class ClienteController : BaseController
{
    private readonly IVentaService _ventaService;
    private readonly IPdfService _pdfService;
    private readonly ILogger<ClienteController> _logger;

    public ClienteController(
        IVentaService ventaService,
        IPdfService pdfService,
        ILogger<ClienteController> logger)
    {
        _ventaService = ventaService;
        _pdfService = pdfService;
        _logger = logger;
    }

    // GET: /Cliente/MisCompras
    public async Task<IActionResult> MisCompras()
    {
        try
        {
            var clienteId = CurrentUserId;
            
            if (clienteId == null)
            {
                AddErrorMessage("Sesión expirada. Por favor inicia sesión nuevamente.");
                return RedirectToAction("Login", "Auth");
            }

            var result = await _ventaService.ObtenerVentasPorClienteAsync(clienteId.Value);

            if (!result.Success)
            {
                AddErrorMessage("Error al cargar tus compras");
                return View(Enumerable.Empty<VentaDTO>());
            }

            return View(result.Data ?? Enumerable.Empty<VentaDTO>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar compras del cliente");
            AddErrorMessage("Error al cargar tus compras");
            return View(Enumerable.Empty<VentaDTO>());
        }
    }

    // GET: /Cliente/DetalleCompra/{id}
    public async Task<IActionResult> DetalleCompra(Guid id)
    {
        try
        {
            var clienteId = CurrentUserId;
            
            if (clienteId == null)
            {
                AddErrorMessage("Sesión expirada. Por favor inicia sesión nuevamente.");
                return RedirectToAction("Login", "Auth");
            }

            var result = await _ventaService.ObtenerVentaPorIdAsync(id);

            if (!result.Success || result.Data == null)
            {
                AddErrorMessage("Compra no encontrada");
                return RedirectToAction(nameof(MisCompras));
            }

            // Verificar que la venta pertenece al cliente
            if (result.Data.ClienteId != clienteId.Value)
            {
                AddErrorMessage("No tienes permiso para ver esta compra");
                return RedirectToAction(nameof(MisCompras));
            }

            return View(result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar detalle de compra {VentaId}", id);
            AddErrorMessage("Error al cargar el detalle de la compra");
            return RedirectToAction(nameof(MisCompras));
        }
    }

    // GET: /Cliente/DescargarPDF/{id}
    public async Task<IActionResult> DescargarPDF(Guid id)
    {
        try
        {
            var clienteId = CurrentUserId;
            
            if (clienteId == null)
            {
                return Unauthorized();
            }

            var result = await _ventaService.ObtenerVentaPorIdAsync(id);

            if (!result.Success || result.Data == null)
            {
                AddErrorMessage("Venta no encontrada");
                return RedirectToAction(nameof(MisCompras));
            }

            // Verificar que la venta pertenece al cliente
            if (result.Data.ClienteId != clienteId.Value)
            {
                AddErrorMessage("No tienes permiso para descargar esta boleta");
                return RedirectToAction(nameof(MisCompras));
            }

            var pdfBytes = _pdfService.GenerarBoletaPDF(result.Data);
            var fileName = $"Boleta_{result.Data.NumeroVenta}.pdf";

            return File(pdfBytes, "application/pdf", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar PDF de venta {VentaId}", id);
            AddErrorMessage("Error al generar el PDF de la boleta");
            return RedirectToAction(nameof(MisCompras));
        }
    }
}
