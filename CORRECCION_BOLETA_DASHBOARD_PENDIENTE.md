# ? CORRECCIONES APLICADAS - Boleta y Dashboard

## ?? CORRECCIONES COMPLETADAS

### 1. ? Boleta con Fondo Visible y Scrollbar

**Archivo:** `Views/Vendedor/DetalleVenta.cshtml`

**Mejoras:**
- ? Contenedor con fondo gris claro (#f5f5f5)
- ? Scrollbar personalizado
- ? Máximo 85vh de altura
- ? Sombra sutil en la boleta
- ? Borde visible para distinguirla del fondo

**CSS Agregado:**
```css
.boleta-container {
    max-height: 85vh;
    overflow-y: auto;
    background-color: #f5f5f5;
    padding: 20px;
    border-radius: 8px;
}

.boleta-container::-webkit-scrollbar {
    width: 10px;
}

.boleta-container::-webkit-scrollbar-thumb {
    background: #888;
    border-radius: 10px;
}
```

---

### 2. ?? Dashboard con Datos Reales (EN PROGRESO)

#### ? Controller Actualizado
**Archivo:** `Controllers/AdminController.cs`

**Cambios:**
```csharp
[Authorize(Roles = "Admin")]
public async Task<IActionResult> Dashboard()
{
    // Obtener estadísticas del mes
    var estadisticas = await _ventaService.ObtenerEstadisticasDelMesAsync();
    ViewBag.Estadisticas = estadisticas.Data;
    
    // Obtener libros con stock bajo (<= 9 unidades)
    var filtroStockBajo = new LibrosFiltroDTO {
        PageSize = 10,
        // PENDIENTE: Filtrar por stock bajo
    };
    var stockBajo = await _libroService.ObtenerLibrosPaginadosAsync(filtroStockBajo);
    ViewBag.StockBajo = stockBajo.Data.Items;
    
    // Obtener top 5 libros más vendidos
    var masVendidos = await _ventaService.ObtenerLibrosMasVendidosAsync(5);
    ViewBag.MasVendidos = masVendidos.Data;
    
    return View();
}
```

#### ? API Endpoint para Gráfico
**Archivo:** `Controllers/Api/VentasController.cs`

**Nuevo Endpoint:**
```csharp
[HttpGet("estadisticas-mes")]
[Authorize(Roles = "Administrador,Admin")]
public async Task<IActionResult> ObtenerEstadisticasMes()
{
    var primerDia = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
    var ultimoDia = primerDia.AddMonths(1).AddDays(-1);
    
    var filtro = new VentasFiltroDTO {
        PageSize = 1000,
        FechaInicio = primerDia,
        FechaFin = ultimoDia
    };
    
    var result = await _ventaService.ObtenerVentasAsync(filtro);
    return Ok(new { success = true, data = result.Data });
}
```

#### ?? Gráfico con Datos Reales (PARCIAL)
**Archivo:** `Views/Admin/Dashboard.cshtml`

**Características:**
- ? Obtiene datos desde `/api/Ventas/estadisticas-mes`
- ? Calcula ventas por semana automáticamente
- ? Muestra totales en soles (S/)
- ? Fallback con datos vacíos si hay error
- ?? Falta cerrar la sección `@section Scripts`

---

## ?? ERRORES PENDIENTES

### Error 1: Dashboard.cshtml - Sección Scripts Sin Cerrar
**Archivo:** `Views/Admin/Dashboard.cshtml`  
**Línea:** ~228  
**Error:** `RZ1006: The section block is missing a closing "}" character`

**Solución:** Agregar al final del archivo:
```javascript
    </script>
}
```

---

### Error 2: AdminController - Propiedades de Filtro
**Archivo:** `Controllers/AdminController.cs`  
**Líneas:** 66-67  
**Error:** `LibrosFiltroDTO' no contiene una definición para 'StockMinimo' y 'StockMaximo'`

**Solución Temporal:** Comentar filtro de stock o usar otro método:
```csharp
// Opción A: Obtener todos y filtrar en memoria
var todosLosLibros = await _libroService.ObtenerLibrosPaginadosAsync(new LibrosFiltroDTO { PageSize = 1000 });
ViewBag.StockBajo = todosLosLibros.Data.Items.Where(l => l.Stock <= 9).Take(10);

// Opción B: Crear método específico en ILibroService
Task<Result<IEnumerable<LibroDTO>>> ObtenerLibrosConStockBajoAsync(int stockMax = 9, int cantidad = 10);
```

---

## ?? ESTADO ACTUAL

| Funcionalidad | Estado |
|---------------|--------|
| Boleta con scroll | ? Completado |
| Boleta con fondo visible | ? Completado |
| Dashboard - Estadísticas reales | ? Completado |
| Dashboard - Stock bajo | ?? Pendiente (filtro) |
| Dashboard - Libros más vendidos | ? Completado |
| Dashboard - Gráfico con datos reales | ?? Pendiente (cerrar scripts) |
| API endpoint estadísticas-mes | ? Completado |

---

## ?? PASOS PARA COMPLETAR

### Paso 1: Cerrar sección Scripts en Dashboard.cshtml

Agregar al final de `Views/Admin/Dashboard.cshtml` (después de la última línea existente):

```javascript
                            data: ventasPorSemana,
                            borderColor: 'rgb(75, 192, 192)',
                            backgroundColor: 'rgba(75, 192, 192, 0.2)',
                            tension: 0.1,
                            fill: true
                        }]
                    },
                    options: {
                        responsive: true,
                        maintainAspectRatio: true,
                        plugins: {
                            legend: { position: 'top' },
                            title: {
                                display: true,
                                text: 'Evolución de Ventas Mensuales',
                                font: { size: 16 }
                            },
                            tooltip: {
                                callbacks: {
                                    label: function(context) {
                                        return 'Total: S/ ' + context.parsed.y.toFixed(2);
                                    }
                                }
                            }
                        },
                        scales: {
                            y: {
                                beginAtZero: true,
                                ticks: {
                                    callback: function(value) {
                                        return 'S/ ' + value.toFixed(0);
                                    }
                                }
                            }
                        }
                    }
                });
            })
            .catch(error => {
                console.error('Error al cargar estadísticas:', error);
                const ctx = document.getElementById('ventasChart');
                new Chart(ctx, {
                    type: 'line',
                    data: {
                        labels: ['Semana 1', 'Semana 2', 'Semana 3', 'Semana 4'],
                        datasets: [{
                            label: 'Ventas (S/)',
                            data: [0, 0, 0, 0],
                            borderColor: 'rgb(75, 192, 192)',
                            backgroundColor: 'rgba(75, 192, 192, 0.2)',
                            tension: 0.1
                        }]
                    },
                    options: {
                        responsive: true,
                        plugins: {
                            legend: { position: 'top' },
                            title: {
                                display: true,
                                text: 'Evolución de Ventas Mensuales'
                            }
                        },
                        scales: {
                            y: { beginAtZero: true }
                        }
                    }
                });
            });
        
        function calcularVentasPorSemana(data) {
            if (!data || !data.data || !data.data.items) {
                return [0, 0, 0, 0];
            }
            
            const ventas = data.data.items;
            const ventasPorSemana = [0, 0, 0, 0];
            const hoy = new Date();
            const primerDia = new Date(hoy.getFullYear(), hoy.getMonth(), 1);
            
            ventas.forEach(venta => {
                const fechaVenta = new Date(venta.fechaVenta);
                const diaDiferencia = Math.floor((fechaVenta - primerDia) / (1000 * 60 * 60 * 24));
                const semana = Math.min(Math.floor(diaDiferencia / 7), 3);
                
                if (semana >= 0 && semana < 4) {
                    ventasPorSemana[semana] += venta.total || 0;
                }
            });
            
            return ventasPorSemana;
        }
        
        // Cargar categorías y editoriales al abrir el modal
        document.getElementById('modalNuevoLibro')?.addEventListener('show.bs.modal', async function() {
            await cargarCategoriasDashboard();
            await cargarEditorialesDashboard();
        });
        
        async function cargarCategoriasDashboard() {
            try {
                const response = await fetch('/api/Libros/categorias');
                const data = await response.json();
                
                if (data.success && data.data) {
                    const select = document.getElementById('categoriaIdDashboard');
                    select.innerHTML = '<option value="">Seleccionar categoría...</option>';
                    
                    data.data.forEach(cat => {
                        const option = document.createElement('option');
                        option.value = cat.id;
                        option.textContent = cat.nombre;
                        select.appendChild(option);
                    });
                }
            } catch (error) {
                console.error('Error al cargar categorías:', error);
            }
        }
        
        async function cargarEditorialesDashboard() {
            try {
                const response = await fetch('/api/Libros/editoriales');
                const data = await response.json();
                
                if (data.success && data.data) {
                    const datalist = document.getElementById('editorialesDatalistDashboard');
                    datalist.innerHTML = '';
                    data.data.forEach(ed => {
                        const option = document.createElement('option');
                        option.value = ed;
                        datalist.appendChild(option);
                    });
                }
            } catch (error) {
                console.error('Error al cargar editoriales:', error);
            }
        }
        
        async function guardarLibroDashboard() {
            const form = document.getElementById('formNuevoLibro');
            
            if (!form.checkValidity()) {
                form.reportValidity();
                return;
            }
            
            const formData = new FormData(form);
            
            const libro = {
                isbn: formData.get('ISBN'),
                titulo: formData.get('Titulo'),
                autor: formData.get('Autor'),
                editorial: formData.get('Editorial') || 'Sin editorial',
                categoriaId: formData.get('CategoriaId'),
                añoPublicacion: parseInt(formData.get('AñoPublicacion')) || new Date().getFullYear(),
                paginas: parseInt(formData.get('Paginas')) || 100,
                idioma: 'Español',
                precio: parseFloat(formData.get('Precio')),
                precioOferta: parseFloat(formData.get('PrecioOferta')) || null,
                stock: parseInt(formData.get('Stock')),
                estado: parseInt(formData.get('Estado')),
                sinopsis: formData.get('Sinopsis') || null,
                descripcion: formData.get('Descripcion') || null,
                destacado: formData.get('Destacado') === 'on'
            };
            
            try {
                const response = await fetch('/api/Libros', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(libro)
                });
                
                const data = await response.json();
                
                if (data.success) {
                    alert('? Libro creado exitosamente');
                    bootstrap.Modal.getInstance(document.getElementById('modalNuevoLibro')).hide();
                    form.reset();
                    location.reload();
                } else {
                    alert('? Error: ' + data.error);
                }
            } catch (error) {
                console.error('Error:', error);
                alert('? Error al guardar el libro');
            }
        }
    </script>
}
```

### Paso 2: Corregir filtro de stock bajo

En `Controllers/AdminController.cs`, líneas 59-75, reemplazar con:

```csharp
// Obtener todos los libros y filtrar en memoria (temporal)
var todosLibrosFiltro = new LibrosFiltroDTO
{
    PageNumber = 1,
    PageSize = 1000, // Suficiente para análisis
    Estado = EstadoLibro.Disponible
};

var todosLibrosResult = await _libroService.ObtenerLibrosPaginadosAsync(todosLibrosFiltro);

if (todosLibrosResult.Success && todosLibrosResult.Data != null)
{
    ViewBag.StockBajo = todosLibrosResult.Data.Items
        .Where(l => l.Stock <= 9)
        .OrderBy(l => l.Stock)
        .Take(10);
}
else
{
    ViewBag.StockBajo = Enumerable.Empty<LibroDTO>();
}
```

---

## ? COMPILAR Y PROBAR

```powershell
dotnet build
dotnet run
```

**Probar:**
1. Login como Admin
2. Ir a Dashboard
3. Verificar:
   - ? Estadísticas correctas
   - ? Stock bajo (si hay libros con stock <= 9)
   - ? Libros más vendidos
   - ? Gráfico de ventas del mes

---

**Estado:** ?? Casi completado - Requiere cierre manual de scripts
