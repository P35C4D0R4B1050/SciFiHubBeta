using SciFiHub.Web.DTOs.Venta;

namespace SciFiHub.Web.Services.Interfaces;

public interface IPdfService
{
    byte[] GenerarBoletaPDF(VentaDTO venta);
}
