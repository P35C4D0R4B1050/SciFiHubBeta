using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SciFiHub.Web.DTOs.Venta;
using SciFiHub.Web.Services.Interfaces;

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
                page.Margin(1.5f, Unit.Centimetre);
                
                page.Content().Column(column =>
                {
                    column.Item().Element(content => ComposeHeader(content, venta));
                    column.Item().Element(content => ComposeContent(content, venta));
                });
                
                page.Footer().AlignCenter().Text("Pág. 1").FontSize(8);
            });
        }).GeneratePdf();
    }
    
    void ComposeHeader(IContainer container, VentaDTO venta)
    {
        container.Column(column =>
        {
            // Encabezado principal
            column.Item().Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("SCIFIHUB").FontSize(18).SemiBold().FontColor("#1E3A8A");
                    col.Item().Text("Av. Ramón Castilla 456").FontSize(8);
                    col.Item().Text("Ayacucho - Huamanga - Ayacucho").FontSize(8);
                    col.Item().Text("Región Ayacucho, Perú").FontSize(8);
                    col.Item().Text("Teléf.: +51 966 123 456 | ventas@scifihub.com").FontSize(8);
                });
                
                row.ConstantItem(140).Border(2).BorderColor("#1E3A8A").Padding(8).Column(col =>
                {
                    col.Item().AlignCenter().Text("R.U.C.: 20601234567").FontSize(9).SemiBold();
                    col.Item().AlignCenter().Text("BOLETA DE VENTA").SemiBold().FontSize(11);
                    col.Item().AlignCenter().Text("ELECTRÓNICA").FontSize(8);
                    col.Item().AlignCenter().PaddingTop(3).Text(venta.NumeroVenta).FontSize(10).SemiBold();
                });
            });
            
            // Información del cliente
            column.Item().PaddingTop(10).Background("#F3F4F6").Padding(6).Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text($"Señor(es): {venta.ClienteNombre}").FontSize(9);
                    col.Item().Text($"Doc. Nº: {venta.ClienteId.ToString().Substring(0, 13)}").FontSize(9);
                });
                
                row.RelativeItem().Column(col =>
                {
                    col.Item().AlignRight().Text($"Fecha: {venta.FechaVenta:dd/MM/yyyy}").FontSize(9);
                    col.Item().AlignRight().Text($"Cód. cliente: {venta.ClienteId.ToString().Substring(0, 8)}").FontSize(9);
                });
            });
            
            // Dirección
            var direccion = venta.DireccionEnvio?.Calle ?? "No especificada";
            column.Item().PaddingTop(3).Background("#F3F4F6").Padding(6).Text($"Dirección: {direccion}").FontSize(9);
        });
    }
    
    void ComposeContent(IContainer container, VentaDTO venta)
    {
        container.PaddingVertical(10).Column(column =>
        {
            // Tabla de items
            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(35);
                    columns.ConstantColumn(85);
                    columns.RelativeColumn(3);
                    columns.ConstantColumn(50);
                    columns.ConstantColumn(70);
                    columns.ConstantColumn(50);
                    columns.ConstantColumn(70);
                });
                
                // Encabezados
                table.Header(header =>
                {
                    header.Cell().Background("#E5E7EB").Border(1).BorderColor("#9CA3AF").Padding(3).AlignCenter().Text("ITEM").SemiBold().FontSize(8);
                    header.Cell().Background("#E5E7EB").Border(1).BorderColor("#9CA3AF").Padding(3).Text("Código").SemiBold().FontSize(8);
                    header.Cell().Background("#E5E7EB").Border(1).BorderColor("#9CA3AF").Padding(3).Text("DESCRIPCIÓN").SemiBold().FontSize(8);
                    header.Cell().Background("#E5E7EB").Border(1).BorderColor("#9CA3AF").Padding(3).AlignCenter().Text("CANT.").SemiBold().FontSize(8);
                    header.Cell().Background("#E5E7EB").Border(1).BorderColor("#9CA3AF").Padding(3).AlignRight().Text("P. UNIT.").SemiBold().FontSize(8);
                    header.Cell().Background("#E5E7EB").Border(1).BorderColor("#9CA3AF").Padding(3).AlignCenter().Text("DESC.").SemiBold().FontSize(8);
                    header.Cell().Background("#E5E7EB").Border(1).BorderColor("#9CA3AF").Padding(3).AlignRight().Text("SUBTOT.").SemiBold().FontSize(8);
                });
                
                // Items
                int itemNum = 1;
                foreach (var detalle in venta.Detalles)
                {
                    table.Cell().Border(1).BorderColor("#D1D5DB").Padding(3).AlignCenter().Text(itemNum.ToString()).FontSize(8);
                    table.Cell().Border(1).BorderColor("#D1D5DB").Padding(3).Text(detalle.LibroISBN ?? "N/A").FontSize(7);
                    table.Cell().Border(1).BorderColor("#D1D5DB").Padding(3).Text(detalle.LibroTitulo).FontSize(8);
                    table.Cell().Border(1).BorderColor("#D1D5DB").Padding(3).AlignCenter().Text(detalle.Cantidad.ToString()).FontSize(8);
                    table.Cell().Border(1).BorderColor("#D1D5DB").Padding(3).AlignRight().Text($"S/ {detalle.PrecioUnitario:N2}").FontSize(8);
                    table.Cell().Border(1).BorderColor("#D1D5DB").Padding(3).AlignCenter().Text(detalle.Descuento.ToString("N2")).FontSize(8);
                    table.Cell().Border(1).BorderColor("#D1D5DB").Padding(3).AlignRight().Text($"S/ {detalle.Subtotal:N2}").FontSize(8);
                    itemNum++;
                }
                
                // Filas vacías reducidas
                int filasVacias = Math.Min(5, 10 - (venta.Detalles?.Count ?? 0));
                for (int i = 0; i < filasVacias; i++)
                {
                    table.Cell().Border(1).BorderColor("#D1D5DB").Padding(3).Text(" ");
                    table.Cell().Border(1).BorderColor("#D1D5DB").Padding(3).Text(" ");
                    table.Cell().Border(1).BorderColor("#D1D5DB").Padding(3).Text(" ");
                    table.Cell().Border(1).BorderColor("#D1D5DB").Padding(3).Text(" ");
                    table.Cell().Border(1).BorderColor("#D1D5DB").Padding(3).Text(" ");
                    table.Cell().Border(1).BorderColor("#D1D5DB").Padding(3).Text(" ");
                    table.Cell().Border(1).BorderColor("#D1D5DB").Padding(3).Text(" ");
                }
            });
            
            // Totales
            column.Item().PaddingTop(10).Row(row =>
            {
                row.RelativeItem().PaddingRight(10).Text($"Son: {ConvertirNumeroALetras((int)Math.Floor(venta.Total))} Y 00/100 SOLES").FontSize(9);
                
                row.ConstantItem(200).Column(col =>
                {
                    col.Item().Row(r =>
                    {
                        r.RelativeItem().AlignRight().Text("SUB. TOTAL:").SemiBold().FontSize(9);
                        r.ConstantItem(70).AlignRight().Text($"S/ {venta.Subtotal:N2}").FontSize(9);
                    });
                    col.Item().Row(r =>
                    {
                        r.RelativeItem().AlignRight().Text("I.G.V (18%):").SemiBold().FontSize(9);
                        r.ConstantItem(70).AlignRight().Text($"S/ {venta.IGV:N2}").FontSize(9);
                    });
                    col.Item().PaddingTop(3).Background("#F3F4F6").Padding(4).Row(r =>
                    {
                        r.RelativeItem().AlignRight().Text("TOTAL:").SemiBold().FontSize(10);
                        r.ConstantItem(70).AlignRight().Text($"S/ {venta.Total:N2}").SemiBold().FontSize(10);
                    });
                });
            });
            
            // Observaciones
            column.Item().PaddingTop(12).Row(row =>
            {
                row.ConstantItem(100).Border(1).BorderColor("#D1D5DB").Height(100).AlignCenter().AlignMiddle().Text("QR CODE").FontSize(9).FontColor("#9CA3AF");
                
                row.RelativeItem().PaddingLeft(12).Column(col =>
                {
                    col.Item().Text("Observaciones:").SemiBold().FontSize(9);
                    col.Item().Text("Representación impresa de la Boleta Electrónica.").FontSize(8);
                    col.Item().Text($"Método de Pago: {venta.MetodoPago}").FontSize(8);
                    col.Item().Text($"Estado: {venta.EstadoVenta}").FontSize(8);
                    
                    if (!string.IsNullOrWhiteSpace(venta.NotasVenta))
                    {
                        col.Item().PaddingTop(3).Text($"Notas: {venta.NotasVenta}").FontSize(8);
                    }
                });
            });
            
            // Pie de boleta
            column.Item().PaddingTop(12).AlignCenter().Column(col =>
            {
                col.Item().Text("¡Gracias por su preferencia!").FontSize(9).Italic();
                col.Item().Text("SciFiHub - Tu tienda de ciencia ficción en Ayacucho").FontSize(8);
                col.Item().Text("www.scifihub.com").FontSize(8).FontColor("#3B82F6");
            });
        });
    }
    
    private string ConvertirNumeroALetras(int numero)
    {
        if (numero == 0) return "CERO";
        if (numero < 10)
        {
            string[] unidades = { "", "UNO", "DOS", "TRES", "CUATRO", "CINCO", "SEIS", "SIETE", "OCHO", "NUEVE" };
            return unidades[numero];
        }
        if (numero < 100)
        {
            string[] decenas = { "", "DIEZ", "VEINTE", "TREINTA", "CUARENTA", "CINCUENTA", "SESENTA", "SETENTA", "OCHENTA", "NOVENTA" };
            int dec = numero / 10;
            int uni = numero % 10;
            if (numero >= 11 && numero <= 15)
            {
                string[] especiales = { "ONCE", "DOCE", "TRECE", "CATORCE", "QUINCE" };
                return especiales[numero - 11];
            }
            if (numero >= 16 && numero <= 19) return "DIECI" + ConvertirNumeroALetras(uni);
            if (numero >= 21 && numero <= 29) return "VEINTI" + ConvertirNumeroALetras(uni);
            return decenas[dec] + (uni > 0 ? " Y " + ConvertirNumeroALetras(uni) : "");
        }
        if (numero < 1000)
        {
            string[] centenas = { "", "CIENTO", "DOSCIENTOS", "TRESCIENTOS", "CUATROCIENTOS", "QUINIENTOS", "SEISCIENTOS", "SETECIENTOS", "OCHOCIENTOS", "NOVECIENTOS" };
            int cen = numero / 100;
            int resto = numero % 100;
            if (numero == 100) return "CIEN";
            return centenas[cen] + (resto > 0 ? " " + ConvertirNumeroALetras(resto) : "");
        }
        
        return numero.ToString();
    }
}
