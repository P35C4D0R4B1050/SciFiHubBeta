# ?? CÓDIGO COMPLETO PARA COPIAR Y PEGAR

## ?? INSTRUCCIONES

Copia cada bloque de código en el archivo correspondiente.

---

## 1?? INSTALAR PAQUETE QUESTPDF

```powershell
cd E:\Proyecto\SciFiHub\SciFiHub
dotnet add package QuestPDF
```

---

## 2?? CREAR: Services/Interfaces/IPdfService.cs

```csharp
using SciFiHub.Web.DTOs.Venta;

namespace SciFiHub.Web.Services.Interfaces;

public interface IPdfService
{
    byte[] GenerarBoletaPDF(VentaDTO venta);
}
```

---

## 3?? CREAR: Services/PdfService.cs

```csharp
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SciFiHub.Web.DTOs.Venta;

namespace SciFiHub.Web.Services;

public class PdfService : IPdfService
{
    public byte[] GenerarBoletaPDF(VentaDTO venta)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                
                page.Header().Element(ComposeHeader);
                page.Content().Element(content => ComposeContent(content, venta));
                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Pág. ");
                    x.CurrentPageNumber();
                    x.Span(" de ");
                    x.TotalPages();
                });
            });
        }).GeneratePdf();
    }
    
    void ComposeHeader(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                column.Item().Text("SCIFIHUB LIBROS E.I.R.L.").FontSize(20).SemiBold().FontColor("#1E3A8A");
                column.Item().Text("RUC: 20987654321").FontSize(9);
                column.Item().Text("Jr. Los Libros 456 - Ayacucho").FontSize(9);
                column.Item().Text("Región: Ayacucho - Provincia: Huamanga - Distrito: Ayacucho").FontSize(9);
                column.Item().Text("Teléfono: (066) 312-456 | Email: ventas@scifihub.pe").FontSize(9);
            });
            
            row.ConstantItem(150).Border(1).BorderColor("#1E3A8A").Padding(10).Column(column =>
            {
                column.Item().AlignCenter().Text("BOLETA DE VENTA").SemiBold().FontSize(12);
                column.Item().AlignCenter().Text("ELECTRÓNICA").FontSize(9);
            });
        });
    }
    
    void ComposeContent(IContainer container, VentaDTO venta)
    {
        container.PaddingVertical(20).Column(column =>
        {
            // Información de la venta
            column.Item().Background("#F3F4F6").Padding(10).Column(col =>
            {
                col.Item().Text($"N° {venta.NumeroVenta}").FontSize(14).SemiBold();
                col.Item().Text($"Fecha: {venta.FechaVenta:dd/MM/yyyy HH:mm}").FontSize(10);
                col.Item().Text($"Cliente: {venta.ClienteNombre}").FontSize(10);
                col.Item().Text($"Método de Pago: {venta.MetodoPagoNombre}").FontSize(10);
            });
            
            // Tabla de items
            column.Item().PaddingTop(20).Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(40);
                    columns.RelativeColumn(3);
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });
                
                // Encabezados
                table.Header(header =>
                {
                    header.Cell().Background("#3B82F6").Padding(5).Text("Cant.").FontColor("#FFFFFF").SemiBold().FontSize(10);
                    header.Cell().Background("#3B82F6").Padding(5).Text("Descripción").FontColor("#FFFFFF").SemiBold().FontSize(10);
                    header.Cell().Background("#3B82F6").Padding(5).AlignRight().Text("P. Unit.").FontColor("#FFFFFF").SemiBold().FontSize(10);
                    header.Cell().Background("#3B82F6").Padding(5).AlignRight().Text("Desc.").FontColor("#FFFFFF").SemiBold().FontSize(10);
                    header.Cell().Background("#3B82F6").Padding(5).AlignRight().Text("Subtotal").FontColor("#FFFFFF").SemiBold().FontSize(10);
                });
                
                // Items
                foreach (var detalle in venta.Detalles)
                {
                    table.Cell().BorderBottom(1).BorderColor("#E5E7EB").Padding(5).AlignCenter().Text(detalle.Cantidad.ToString()).FontSize(9);
                    table.Cell().BorderBottom(1).BorderColor("#E5E7EB").Padding(5).Text(detalle.LibroTitulo).FontSize(9);
                    table.Cell().BorderBottom(1).BorderColor("#E5E7EB").Padding(5).AlignRight().Text($"S/ {detalle.PrecioUnitario:N2}").FontSize(9);
                    table.Cell().BorderBottom(1).BorderColor("#E5E7EB").Padding(5).AlignRight().Text($"S/ {detalle.Descuento:N2}").FontSize(9);
                    table.Cell().BorderBottom(1).BorderColor("#E5E7EB").Padding(5).AlignRight().Text($"S/ {detalle.Subtotal:N2}").FontSize(9);
                }
            });
            
            // Totales
            column.Item().PaddingTop(20).AlignRight().Column(col =>
            {
                col.Item().Text($"Subtotal: S/ {venta.Subtotal:N2}").FontSize(11);
                col.Item().Text($"IGV (18%): S/ {venta.IGV:N2}").FontSize(11);
                col.Item().PaddingTop(5).Background("#F3F4F6").Padding(8).Text($"TOTAL: S/ {venta.Total:N2}").FontSize(14).SemiBold().FontColor("#1E3A8A");
            });
            
            // Pie de página
            column.Item().PaddingTop(30).Column(col =>
            {
                col.Item().AlignCenter().Text("Gracias por su compra").FontSize(10).Italic();
                col.Item().AlignCenter().Text("SciFiHub - Tu librería de ciencia ficción").FontSize(9);
            });
        });
    }
}
```

---

## 4?? CREAR: wwwroot/js/ventas.js

```javascript
// ========================================
// GESTIÓN DE VENTAS - ADMIN/VENDEDOR
// ========================================

console.log('?? Inicializando gestión de ventas...');

// ========================================
// CANCELAR VENTA
// ========================================

async function cancelarVenta(ventaId) {
    console.log('?? Iniciando cancelación de venta:', ventaId);
    
    const motivo = prompt('Ingrese el motivo de la cancelación:');
    if (!motivo || motivo.trim() === '') {
        alert('? Debe ingresar un motivo para cancelar la venta');
        return;
    }
    
    if (!confirm('¿Está seguro de cancelar esta venta? Esta acción restaurará el stock y no se puede deshacer.')) {
        return;
    }
    
    try {
        const response = await fetch('/api/Ventas/cancelar', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                ventaId: ventaId,
                motivo: motivo.trim()
            })
        });
        
        const data = await response.json();
        console.log('?? Respuesta cancelar:', data);
        
        if (data.success) {
            alert('? Venta cancelada exitosamente. El stock ha sido restaurado.');
            window.location.reload();
        } else {
            alert('? Error: ' + data.error);
        }
    } catch (error) {
        console.error('? Error al cancelar venta:', error);
        alert('? Error al cancelar la venta. Por favor intente nuevamente.');
    }
}

// ========================================
// COMPLETAR VENTA
// ========================================

async function completarVenta(ventaId) {
    console.log('? Completando venta:', ventaId);
    
    if (!confirm('¿Confirmar que la venta fue completada exitosamente?')) {
        return;
    }
    
    try {
        const response = await fetch(`/api/Ventas/${ventaId}/completar`, {
            method: 'POST'
        });
        
        const data = await response.json();
        
        if (data.success) {
            alert('? Venta completada exitosamente');
            window.location.reload();
        } else {
            alert('? Error: ' + data.error);
        }
    } catch (error) {
        console.error('? Error al completar venta:', error);
        alert('? Error al completar la venta');
    }
}

// ========================================
// DESCARGAR PDF
// ========================================

function descargarPDF(ventaId) {
    console.log('?? Descargando PDF de venta:', ventaId);
    window.open(`/api/Ventas/${ventaId}/pdf`, '_blank');
}

// ========================================
// EXPORTAR FUNCIONES GLOBALES
// ========================================

window.cancelarVenta = cancelarVenta;
window.completarVenta = completarVenta;
window.descargarPDF = descargarPDF;

console.log('? Gestión de ventas inicializada');
```

---

## 5?? MODIFICAR: Program.cs

Buscar donde se registran los servicios (después de `builder.Services.AddScoped<IVentaService, VentaService>();`) y agregar:

```csharp
builder.Services.AddScoped<IPdfService, PdfService>();
```

---

## 6?? MODIFICAR: Controllers/Api/VentasController.cs

Agregar IPdfService al constructor y agregar estos métodos:

```csharp
// En el constructor, agregar:
private readonly IPdfService _pdfService;

public VentasController(
    IVentaService ventaService,
    IPdfService pdfService,  // NUEVO
    ILogger<VentasController> logger)
{
    _ventaService = ventaService;
    _pdfService = pdfService;  // NUEVO
    _logger = logger;
}

// Agregar estos métodos:

[HttpPost("cancelar")]
public async Task<IActionResult> CancelarVenta([FromBody] CancelarVentaDTO dto)
{
    try
    {
        _logger.LogInformation("Cancelando venta {VentaId}", dto.VentaId);
        
        var result = await _ventaService.CancelarVentaAsync(dto);
        
        if (!result.Success)
        {
            return BadRequest(new { success = false, error = result.ErrorMessage });
        }
        
        return Ok(new { success = true, message = "Venta cancelada exitosamente" });
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error al cancelar venta");
        return StatusCode(500, new { success = false, error = "Error al cancelar la venta" });
    }
}

[HttpGet("{id}/pdf")]
[AllowAnonymous]
public async Task<IActionResult> DescargarPDF(Guid id)
{
    try
    {
        _logger.LogInformation("Generando PDF para venta {VentaId}", id);
        
        var result = await _ventaService.ObtenerVentaPorIdAsync(id);
        
        if (!result.Success || result.Data == null)
        {
            return NotFound(new { success = false, error = "Venta no encontrada" });
        }
        
        var pdf = _pdfService.GenerarBoletaPDF(result.Data);
        
        return File(pdf, "application/pdf", $"Boleta-{result.Data.NumeroVenta}.pdf");
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error al generar PDF");
        return StatusCode(500, new { success = false, error = "Error al generar PDF" });
    }
}

[HttpPost("{id}/completar")]
public async Task<IActionResult> CompletarVenta(Guid id)
{
    try
    {
        var result = await _ventaService.CompletarVentaAsync(id);
        
        if (!result.Success)
        {
            return BadRequest(new { success = false, error = result.ErrorMessage });
        }
        
        return Ok(new { success = true, message = "Venta completada exitosamente" });
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error al completar venta");
        return StatusCode(500, new { success = false, error = "Error al completar la venta" });
    }
}
```

---

## 7?? MODIFICAR: Views/Vendedor/Ventas.cshtml

Buscar donde se muestran las acciones de cada venta y agregar los botones:

```html
<!-- Dentro del foreach de ventas, en la columna de acciones -->
<td>
    <!-- Ver detalle -->
    <a href="@Url.Action("DetalleVenta", new { id = venta.Id })" 
       class="btn btn-sm btn-info" 
       title="Ver detalle">
        <i class="bi bi-eye"></i>
    </a>
    
    <!-- PDF -->
    <button onclick="descargarPDF('@venta.Id')" 
            class="btn btn-sm btn-primary" 
            title="Descargar PDF">
        <i class="bi bi-file-pdf"></i>
    </button>
    
    <!-- Completar (solo si está Pendiente) -->
    @if (venta.EstadoVenta == SciFiHub.Domain.Enums.EstadoVenta.Pendiente)
    {
        <button onclick="completarVenta('@venta.Id')" 
                class="btn btn-sm btn-success" 
                title="Completar venta">
            <i class="bi bi-check-circle"></i>
        </button>
        
        <!-- Cancelar (solo si está Pendiente) -->
        <button onclick="cancelarVenta('@venta.Id')" 
                class="btn btn-sm btn-danger" 
                title="Cancelar venta">
            <i class="bi bi-x-circle"></i>
        </button>
    }
</td>

<!-- Al final de la vista, antes de @section Scripts, agregar: -->
<script src="~/js/ventas.js" asp-append-version="true"></script>
```

---

## 8?? MODIFICAR: Views/Vendedor/DetalleVenta.cshtml

Agregar botones de PDF y cancelar:

```html
<!-- Buscar la sección de acciones y agregar: -->
<div class="card mb-4">
    <div class="card-header bg-primary text-white">
        <h5 class="mb-0"><i class="bi bi-gear"></i> Acciones</h5>
    </div>
    <div class="card-body">
        <div class="d-grid gap-2">
            <!-- Ver PDF -->
            <button onclick="descargarPDF('@Model.Id')" class="btn btn-primary btn-lg">
                <i class="bi bi-file-pdf"></i> Ver/Descargar PDF
            </button>
            
            <!-- Completar -->
            @if (Model.EstadoVenta == SciFiHub.Domain.Enums.EstadoVenta.Pendiente)
            {
                <button onclick="completarVenta('@Model.Id')" class="btn btn-success">
                    <i class="bi bi-check-circle"></i> Completar Venta
                </button>
                
                <!-- Cancelar -->
                <button onclick="cancelarVenta('@Model.Id')" class="btn btn-danger">
                    <i class="bi bi-x-circle"></i> Cancelar Venta
                </button>
            }
            
            <!-- Volver -->
            <a href="@Url.Action("Ventas")" class="btn btn-outline-secondary">
                <i class="bi bi-arrow-left"></i> Volver a Ventas
            </a>
        </div>
    </div>
</div>

<!-- Al final antes de @section Scripts: -->
<script src="~/js/ventas.js" asp-append-version="true"></script>
```

---

## 9?? MODIFICAR: Views/Cliente/DetalleCompra.cshtml

Agregar botón de PDF:

```html
<!-- Buscar la sección de acciones y agregar: -->
<div class="card mb-4">
    <div class="card-body">
        <div class="d-grid gap-2">
            <!-- Ver/Descargar PDF -->
            <button onclick="descargarPDF('@Model.Id')" class="btn btn-primary btn-lg">
                <i class="bi bi-file-pdf"></i> Ver/Descargar Boleta (PDF)
            </button>
            
            <!-- Volver -->
            <a href="@Url.Action("MisCompras")" class="btn btn-outline-secondary">
                <i class="bi bi-arrow-left"></i> Volver a Mis Compras
            </a>
        </div>
    </div>
</div>

<!-- Al final antes de @section Scripts: -->
<script>
    function descargarPDF(ventaId) {
        window.open(`/api/Ventas/${ventaId}/pdf`, '_blank');
    }
</script>
```

---

## ?? COPIAR VIEWS ADMIN (Mismo contenido que Vendedor)

Copiar el mismo contenido de botones en:
- `Views/Admin/Ventas.cshtml`
- `Views/Admin/DetalleVenta.cshtml` (si existe)

---

## ? VERIFICACIÓN FINAL

Después de aplicar todos los cambios:

```powershell
cd E:\Proyecto\SciFiHub\SciFiHub
dotnet clean
dotnet build
```

Si compila sin errores:

```powershell
dotnet run
```

---

## ?? PRUEBAS

1. **Login como Vendedor/Admin:**
   - Ir a Ventas
   - Crear una nueva venta
   - Ver que el stock se actualiza
   - Click en botón PDF ? Ver boleta
   - Click en Completar ? Cambiar estado
   - Click en Cancelar ? Restaurar stock

2. **Login como Cliente:**
   - Ir al Catálogo
   - Click en un libro
   - Click "Agregar al Carrito"
   - Ver que el contador se actualiza
   - Ir a "Mis Compras"
   - Click en "Ver" en una compra
   - Click en botón PDF

---

**TODO LISTO PARA COPIAR Y PEGAR** ?
