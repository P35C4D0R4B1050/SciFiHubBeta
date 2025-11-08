# ?? INSTRUCCIONES FINALES - EJECUTAR EN ORDEN

## ? ESTADO ACTUAL

- ? Código fuente creado
- ? Servicio de PDF implementado
- ? JavaScript de ventas creado
- ? Falta instalar paquete NuGet
- ? Falta modificar vistas

---

## ?? PASO 1: INSTALAR QUESTPDF

Abre PowerShell en `E:\Proyecto\SciFiHub\SciFiHub` y ejecuta:

```powershell
dotnet add package QuestPDF
```

**Resultado Esperado:**
```
info : Se agregó el paquete 'QuestPDF' versión X.X.X
```

---

## ?? PASO 2: COMPILAR

```powershell
dotnet clean
dotnet build
```

**Resultado Esperado:**
```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

---

## ?? PASO 3: MODIFICAR VISTAS

### 3.1. Views/Vendedor/Ventas.cshtml

Buscar donde se muestran las acciones (columna de botones) y **REEMPLAZAR** por:

```html
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
    
    @if (venta.EstadoVenta == SciFiHub.Domain.Enums.EstadoVenta.Pendiente)
    {
        <!-- Completar -->
        <button onclick="completarVenta('@venta.Id')" 
                class="btn btn-sm btn-success" 
                title="Completar venta">
            <i class="bi bi-check-circle"></i>
        </button>
        
        <!-- Cancelar -->
        <button onclick="cancelarVenta('@venta.Id')" 
                class="btn btn-sm btn-danger" 
                title="Cancelar venta">
            <i class="bi bi-x-circle"></i>
        </button>
    }
</td>
```

**Y al final de la vista**, antes de `</div>` final, agregar:

```html
@section Scripts {
    <script src="~/js/ventas.js" asp-append-version="true"></script>
}
```

---

### 3.2. Views/Vendedor/DetalleVenta.cshtml

Buscar la sección de acciones y **AGREGAR** (o reemplazar si ya existe):

```html
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
            
            @if (Model.EstadoVenta == SciFiHub.Domain.Enums.EstadoVenta.Pendiente)
            {
                <!-- Completar -->
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

@section Scripts {
    <script src="~/js/ventas.js" asp-append-version="true"></script>
}
```

---

### 3.3. Views/Cliente/DetalleCompra.cshtml

Buscar donde están los botones y **AGREGAR**:

```html
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

@section Scripts {
    <script>
        function descargarPDF(ventaId) {
            window.open(`/api/Ventas/${ventaId}/pdf`, '_blank');
        }
    </script>
}
```

---

## ? PASO 4: COMPILAR Y PROBAR

```powershell
dotnet clean
dotnet build
dotnet run
```

---

## ?? PASO 5: PRUEBAS

### Como Vendedor/Admin:

1. **Login**: `vendedor.test` / `Test123!` (o tu usuario)
2. **Ir a Ventas**
3. **Registrar una nueva venta**:
   - Seleccionar cliente
   - Agregar libros
   - Completar formulario
   - Registrar
4. **Verificar**:
   - ? Venta se registra sin error
   - ? Stock se actualiza en BD
5. **Click en botón PDF** (icono de PDF)
   - ? Se abre PDF en nueva pestaña
   - ? Datos correctos de la empresa
   - ? Items y totales correctos
6. **Click en botón Completar** (check verde)
   - ? Venta cambia a Completada
7. **Crear otra venta en estado Pendiente**
8. **Click en botón Cancelar** (X roja)
   - Ingresar motivo
   - Confirmar
   - ? Venta se cancela
   - ? Stock se restaura

### Como Cliente:

1. **Login**: Cliente existente
2. **Ir a Catálogo**
3. **Click en un libro**
4. **Click "Agregar al Carrito"**:
   - ? Contador en navbar se actualiza
   - ? Notificación de éxito
5. **Ir a Mis Compras**
6. **Click en "Ver" en una compra**
7. **Click en "Ver/Descargar Boleta"**:
   - ? Se abre PDF
   - ? Datos correctos

---

## ?? VERIFICACIÓN DE STOCK

Antes de crear venta, anotar stock:

```sql
SELECT Id, Titulo, Stock FROM Libros WHERE Titulo LIKE '%Dune%'
```

Después de crear venta con 2 unidades:

```sql
SELECT Id, Titulo, Stock FROM Libros WHERE Titulo LIKE '%Dune%'
-- Stock debe haber disminuido en 2
```

Después de cancelar venta:

```sql
SELECT Id, Titulo, Stock FROM Libros WHERE Titulo LIKE '%Dune%'
-- Stock debe haber regresado al valor original
```

---

## ?? SI HAY PROBLEMAS

### Error: "QuestPDF no se encontró"

Ejecutar nuevamente:

```powershell
dotnet add package QuestPDF --version 2024.12.0
```

### Error: "No se puede agregar al carrito"

1. Abrir DevTools (F12) ? Console
2. Verificar que aparezca: "? Carrito de compras inicializado"
3. Si no aparece, verificar que el usuario sea Cliente
4. Verificar en Network que `carrito-cliente.js` se cargó

### Error: "PDF no se genera"

1. Verificar logs en consola de la app
2. Buscar errores relacionados con QuestPDF
3. Verificar que el servicio esté registrado en Program.cs

---

## ?? RESUMEN DE ARCHIVOS MODIFICADOS/CREADOS

### Creados:
- ? `Services/Interfaces/IPdfService.cs`
- ? `Services/PdfService.cs`
- ? `wwwroot/js/ventas.js`

### Modificados:
- ? `Program.cs` - Agregado registro de `IPdfService`
- ? `Controllers/Api/VentasController.cs` - Agregados endpoints
- ? `Services/VentaService.cs` - Mejorado manejo de errores y stock
- ? `Views/Vendedor/Ventas.cshtml` - Agregar botones
- ? `Views/Vendedor/DetalleVenta.cshtml` - Agregar botones
- ? `Views/Cliente/DetalleCompra.cshtml` - Agregar botón PDF

---

## ? CHECKLIST FINAL

### Instalación:
- [ ] ? Ejecutar `dotnet add package QuestPDF`
- [ ] ? Compilar sin errores

### Funcionalidad Vendedor/Admin:
- [ ] ? Registrar venta sin error
- [ ] ? Stock se actualiza correctamente
- [ ] ? Se puede completar una venta
- [ ] ? Se puede cancelar una venta
- [ ] ? Stock se restaura al cancelar
- [ ] ? Se puede ver/descargar PDF
- [ ] ? PDF muestra datos de la empresa

### Funcionalidad Cliente:
- [ ] ? Puede agregar al carrito desde catálogo
- [ ] ? Puede agregar al carrito desde detalle
- [ ] ? Contador se actualiza
- [ ] ? Puede ver mis compras
- [ ] ? Puede ver/descargar PDF de sus compras

---

**SIGUIENTE PASO:** Ejecutar `dotnet add package QuestPDF` y luego compilar.

**Tiempo Estimado Total:** 15-20 minutos para aplicar cambios en vistas.
