# ?? SOLUCIONES IMPLEMENTADAS Y PENDIENTES - SciFiHub

## ? PROBLEMAS SOLUCIONADOS

### 1. **Error de Triggers en SQL Server** ?
**Problema**: `Could not save changes because the target table has database triggers`

**Causa**: EF Core usa OUTPUT clause que es incompatible con triggers de SQL Server

**Solución Implementada**:
```csharp
// En SciFiHubDbContext.cs
private void ConfigurarTriggersCompatibilidad(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Usuario>().ToTable(tb => tb.UseSqlOutputClause(false));
    modelBuilder.Entity<Libro>().ToTable(tb => tb.UseSqlOutputClause(false));
    modelBuilder.Entity<Venta>().ToTable(tb => tb.UseSqlOutputClause(false));
    modelBuilder.Entity<DetalleVenta>().ToTable(tb => tb.UseSqlOutputClause(false));
    modelBuilder.Entity<AuditoriaInventario>().ToTable(tb => tb.UseSqlOutputClause(false));
}
```

**Archivos Modificados**:
- `SciFiHub.Infrastructure/Data/SciFiHubDbContext.cs`

---

### 2. **Sección "Styles" No Renderizada** ?
**Problema**: Error al usar `@section Styles` en vistas

**Solución Implementada**:
```razor
<!-- En _Layout.cshtml -->
<head>
    <!-- ...otros links... -->
    @await RenderSectionAsync("Styles", required: false)
</head>
```

**Archivos Modificados**:
- `Views/Shared/_Layout.cshtml`

---

## ? PROBLEMAS PENDIENTES

### 3. **Rol Incorrecto al Crear Usuario**
**Problema**: Se crea con rol Cliente sin importar el rol seleccionado

**Diagnóstico**: El AutoMapper está configurado correctamente. El problema puede estar en:
1. El valor que se envía desde el frontend
2. La configuración de conversión del enum

**Solución a Implementar**:
Agregar logging en `CrearUsuarioAsync`:
```csharp
_logger.LogInformation("Rol recibido en DTO: {Rol} ({RolValue})", 
    crearUsuarioDto.Rol, (int)crearUsuarioDto.Rol);

var usuario = _mapper.Map<Domain.Entities.Usuario>(crearUsuarioDto);

_logger.LogInformation("Rol mapeado en Usuario: {Rol} ({RolValue})", 
    usuario.Rol, (int)usuario.Rol);
```

**Verificar en JavaScript**:
```javascript
// En Usuarios.cshtml - función guardarUsuario()
console.log('Rol seleccionado:', formData.get('Rol'));
console.log('Rol parseado:', parseInt(formData.get('Rol')));
```

---

### 4. **Categorías No Se Muestran en Crear Libro**
**Problema**: El select de categorías está vacío

**Solución a Implementar**:
1. Crear endpoint para obtener categorías:
```csharp
// En LibrosController.cs (API)
[HttpGet("categorias")]
public async Task<IActionResult> ObtenerCategorias()
{
    var categorias = await _categoriaService.ObtenerCategoriasActivasAsync();
    return Ok(new { success = true, data = categorias.Data });
}
```

2. Cargar categorías al abrir el modal:
```javascript
// En Inventario.cshtml
document.getElementById('modalLibro').addEventListener('show.bs.modal', async function() {
    await cargarCategorias();
});

async function cargarCategorias() {
    const response = await fetch('/api/Libros/categorias');
    const data = await response.json();
    
    const select = document.getElementById('categoriaId');
    select.innerHTML = '<option value="">Seleccionar categoría...</option>';
    
    data.data.forEach(cat => {
        select.innerHTML += `<option value="${cat.id}">${cat.nombre}</option>`;
    });
}
```

---

### 5. **Autocomplete de Editoriales**
**Problema**: Editorial es campo de texto libre

**Solución a Implementar**:
1. Crear endpoint para obtener editoriales únicas:
```csharp
// En LibrosController.cs (API)
[HttpGet("editoriales")]
public async Task<IActionResult> ObtenerEditoriales()
{
    var editoriales = await _context.Libros
        .Where(l => !string.IsNullOrEmpty(l.Editorial))
        .Select(l => l.Editorial)
        .Distinct()
        .OrderBy(e => e)
        .ToListAsync();
    
    return Ok(new { success = true, data = editoriales });
}
```

2. Implementar autocomplete con datalist HTML5:
```html
<input type="text" list="editorialesDatalist" name="Editorial" id="editorial">
<datalist id="editorialesDatalist">
    <!-- Se llenará con JavaScript -->
</datalist>

<div id="nuevaEditorialContainer" style="display:none;">
    <label>Nueva Editorial:</label>
    <input type="text" id="nuevaEditorial">
</div>
```

```javascript
async function cargarEditoriales() {
    const response = await fetch('/api/Libros/editoriales');
    const data = await response.json();
    
    const datalist = document.getElementById('editorialesDatalist');
    datalist.innerHTML = '';
    
    data.data.forEach(ed => {
        datalist.innerHTML += `<option value="${ed}">`;
    });
}

document.getElementById('editorial').addEventListener('input', function(e) {
    const valor = e.target.value;
    const opciones = Array.from(document.getElementById('editorialesDatalist').options)
        .map(opt => opt.value);
    
    if (valor && !opciones.includes(valor)) {
        document.getElementById('nuevaEditorialContainer').style.display = 'block';
    } else {
        document.getElementById('nuevaEditorialContainer').style.display = 'none';
    }
});
```

---

### 6. **Number Pickers Faltantes**
**Problema**: Año de Publicación y Páginas no tienen number pickers

**Solución a Implementar**:
```html
<!-- Año de Publicación -->
<div class="col-md-3">
    <label class="form-label fw-bold">Año Publicación</label>
    <div class="number-picker">
        <button type="button" class="btn" onclick="cambiarAnio(-1)">
            <i class="bi bi-dash"></i>
        </button>
        <input type="number" class="form-control" name="AñoPublicacion" id="anioPublicacion" 
               min="1800" max="2025" value="2025">
        <button type="button" class="btn" onclick="cambiarAnio(1)">
            <i class="bi bi-plus"></i>
        </button>
    </div>
</div>

<!-- Páginas -->
<div class="col-md-3">
    <label class="form-label fw-bold">Páginas</label>
    <div class="number-picker">
        <button type="button" class="btn" onclick="cambiarPaginas(-10)">
            <i class="bi bi-dash"></i>
        </button>
        <input type="number" class="form-control" name="Paginas" id="paginas" min="1" value="100">
        <button type="button" class="btn" onclick="cambiarPaginas(10)">
            <i class="bi bi-plus"></i>
        </button>
    </div>
</div>
```

```javascript
function cambiarAnio(delta) {
    const input = document.getElementById('anioPublicacion');
    const valor = parseInt(input.value) || 2025;
    const nuevoValor = Math.max(1800, Math.min(2025, valor + delta));
    input.value = nuevoValor;
}

function cambiarPaginas(delta) {
    const input = document.getElementById('paginas');
    const valor = parseInt(input.value) || 100;
    const nuevoValor = Math.max(1, valor + delta);
    input.value = nuevoValor;
}
```

---

### 7. **Registro Rápido de Cliente en Ventas**
**Problema**: No existe funcionalidad para registrar cliente desde CrearVenta

**Solución a Implementar**:

1. Crear endpoint API:
```csharp
// En UsuariosController.cs
[HttpPost("registro-rapido")]
public async Task<IActionResult> RegistroRapido([FromBody] RegistroRapidoDTO model)
{
    var crearUsuarioDto = new CrearUsuarioDTO
    {
        NombreCompleto = model.NombreCompleto,
        Email = model.Email,
        Username = GenerarUsername(model.Email),
        Password = GenerarPasswordTemporal(),
        ConfirmPassword = GenerarPasswordTemporal(),
        Rol = RolUsuario.Cliente,
        Telefono = model.Telefono
    };
    
    var result = await _usuarioService.CrearUsuarioAsync(crearUsuarioDto);
    
    if (result.Success)
    {
        // Enviar email con credenciales (opcional)
        return Ok(new { success = true, data = result.Data });
    }
    
    return BadRequest(new { error = result.ErrorMessage });
}

private string GenerarUsername(string email) => email.Split('@')[0];
private string GenerarPasswordTemporal() => Guid.NewGuid().ToString("N").Substring(0, 8);
```

2. Modal en CrearVenta.cshtml:
```html
<!-- Botón junto al select de cliente -->
<button type="button" class="btn btn-sm btn-outline-success" 
        data-bs-toggle="modal" data-bs-target="#modalRegistroRapido">
    <i class="bi bi-person-plus"></i> Nuevo Cliente
</button>

<!-- Modal de Registro Rápido -->
<div class="modal fade" id="modalRegistroRapido">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header bg-success text-white">
                <h5>Registro Rápido de Cliente</h5>
                <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
            </div>
            <div class="modal-body">
                <form id="formRegistroRapido">
                    <div class="mb-3">
                        <label class="form-label">Nombre Completo *</label>
                        <input type="text" class="form-control" name="NombreCompleto" required>
                    </div>
                    <div class="mb-3">
                        <label class="form-label">Email *</label>
                        <input type="email" class="form-control" name="Email" required>
                    </div>
                    <div class="mb-3">
                        <label class="form-label">Teléfono</label>
                        <input type="tel" class="form-control" name="Telefono">
                    </div>
                    <div class="alert alert-info">
                        <i class="bi bi-info-circle"></i>
                        Se generará un usuario y contraseña automática que se enviará por email.
                    </div>
                </form>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancelar</button>
                <button type="button" class="btn btn-success" onclick="guardarClienteRapido()">
                    <i class="bi bi-save"></i> Registrar
                </button>
            </div>
        </div>
    </div>
</div>
```

```javascript
async function guardarClienteRapido() {
    const form = document.getElementById('formRegistroRapido');
    if (!form.checkValidity()) {
        form.reportValidity();
        return;
    }
    
    const formData = new FormData(form);
    const cliente = {
        nombreCompleto: formData.get('NombreCompleto'),
        email: formData.get('Email'),
        telefono: formData.get('Telefono')
    };
    
    try {
        const response = await fetch('/api/Usuarios/registro-rapido', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(cliente)
        });
        
        const data = await response.json();
        
        if (data.success) {
            alert('? Cliente registrado exitosamente');
            
            // Recargar select de clientes
            await cargarClientes();
            
            // Seleccionar el nuevo cliente
            document.getElementById('clienteId').value = data.data.id;
            
            // Cerrar modal
            bootstrap.Modal.getInstance(document.getElementById('modalRegistroRapido')).hide();
            form.reset();
        } else {
            alert('Error: ' + data.error);
        }
    } catch (error) {
        console.error('Error:', error);
        alert('Error al registrar el cliente');
    }
}
```

---

## ?? PLAN DE IMPLEMENTACIÓN

### Prioridad ALTA (Crítico):
1. ? **Triggers de SQL Server** - SOLUCIONADO
2. ? **Sección Styles** - SOLUCIONADO
3. ? **Rol incorrecto al crear usuario** - PENDIENTE
4. ? **Categorías en Crear Libro** - PENDIENTE

### Prioridad MEDIA:
5. ? **Autocomplete de Editoriales** - PENDIENTE
6. ? **Number Pickers** - PENDIENTE

### Prioridad BAJA:
7. ? **Registro rápido de cliente** - PENDIENTE (funcionalidad nueva)

---

## ?? PASOS PARA DEPURACIÓN

### Problema del Rol:
1. Abrir DevTools en navegador
2. Ir a Network tab
3. Crear un usuario
4. Ver la request a `/api/Usuarios`
5. Verificar el JSON enviado: ¿tiene el campo `rol` correcto?
6. Si el JSON es correcto pero se guarda mal, el problema está en el backend
7. Si el JSON está mal, el problema está en el JavaScript

### Problema de Categorías:
1. Verificar que existe el endpoint `/api/Libros/categorias`
2. Llamarlo manualmente en el navegador
3. Ver si devuelve categorías
4. Si devuelve vacío, verificar la base de datos
5. Si devuelve datos, verificar que el JavaScript las carga correctamente

---

## ?? ARCHIVOS A MODIFICAR

### Ya Modificados: ?
- `SciFiHub.Infrastructure/Data/SciFiHubDbContext.cs`
- `Views/Shared/_Layout.cshtml`

### Pendientes: ?
- `Services/UsuarioService.cs` (agregar logging)
- `Views/Admin/Usuarios.cshtml` (debugging JavaScript)
- `Controllers/Api/LibrosController.cs` (endpoint categorías y editoriales)
- `Views/Vendedor/Inventario.cshtml` (categorías, editoriales, number pickers)
- `Views/Admin/Inventario.cshtml` (mismo que Vendedor)
- `Controllers/Api/UsuariosController.cs` (endpoint registro rápido)
- `Views/Vendedor/CrearVenta.cshtml` (modal registro rápido)
- `DTOs/Usuario/UsuarioDTOs.cs` (DTO RegistroRapidoDTO si es necesario)

---

## ? ESTADO ACTUAL

| Problema | Estado | Prioridad |
|----------|--------|-----------|
| Triggers SQL Server | ? SOLUCIONADO | Alta |
| Sección Styles | ? SOLUCIONADO | Alta |
| Rol incorrecto | ? PENDIENTE | Alta |
| Categorías vacías | ? PENDIENTE | Alta |
| Autocomplete editoriales | ? PENDIENTE | Media |
| Number pickers | ? PENDIENTE | Media |
| Registro rápido cliente | ? PENDIENTE | Baja |

**Progreso**: 2/7 (29%)

---

**Última Actualización**: Enero 2025  
**Compilación**: ? Exitosa  
**Siguiente Acción**: Depurar problema de roles
