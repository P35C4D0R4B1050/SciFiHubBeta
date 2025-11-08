# ?? PLAN DE IMPLEMENTACIÓN COMPLETO - SciFiHub

## ?? RESUMEN DE TAREAS

### ? Tareas Identificadas:

1. **Categorías no se cargan en modal de libro** (Admin y Vendedor)
2. **Registro rápido de clientes en ventas** (Admin y Vendedor)
3. **Error al crear/editar usuarios** (Admin)
4. **Toggle para ver usuarios eliminados + eliminación definitiva** (Admin)

---

## ?? SOLUCIÓN 1: Categorías No Se Cargan

### Diagnóstico:
El endpoint `/api/Libros/categorias` existe y funciona. El problema está en:
1. La función JavaScript no se ejecuta correctamente
2. Posible problema de timing (el modal se abre antes de cargar)

### Solución:

**Archivo**: `Views/Vendedor/Inventario.cshtml` y `Views/Admin/Inventario.cshtml`

```javascript
// MEJORAR LA FUNCIÓN cargarCategorias()
async function cargarCategorias() {
    try {
        console.log('?? Iniciando carga de categorías...');
        
        const response = await fetch('/api/Libros/categorias');
        
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        
        const data = await response.json();
        
        console.log('?? Datos recibidos del servidor:', data);
        
        if (data.success && data.data && Array.isArray(data.data) && data.data.length > 0) {
            const select = document.getElementById('categoriaId');
            
            if (!select) {
                console.error('? Select categoriaId no encontrado en el DOM');
                return;
            }
            
            // Limpiar opciones existentes
            select.innerHTML = '';
            
            // Agregar opción por defecto
            const optionDefault = document.createElement('option');
            optionDefault.value = '';
            optionDefault.textContent = 'Seleccionar categoría...';
            select.appendChild(optionDefault);
            
            // Agregar categorías
            data.data.forEach(cat => {
                const option = document.createElement('option');
                option.value = cat.id;
                option.textContent = cat.nombre;
                select.appendChild(option);
            });
            
            console.log(`? ${data.data.length} categorías cargadas exitosamente`);
        } else {
            console.warn('?? No se encontraron categorías o respuesta inválida');
            const select = document.getElementById('categoriaId');
            if (select) {
                select.innerHTML = '<option value="">No hay categorías disponibles</option>';
            }
        }
    } catch (error) {
        console.error('? Error al cargar categorías:', error);
        alert('Error al cargar categorías. Verifique:\n1. Que existe el endpoint /api/Libros/categorias\n2. Que hay categorías en la BD\n3. La consola del navegador para más detalles.');
    }
}

// ASEGURAR QUE SE LLAME AL ABRIR EL MODAL
document.addEventListener('DOMContentLoaded', function() {
    const modalLibro = document.getElementById('modalLibro');
    
    if (modalLibro) {
        modalLibro.addEventListener('show.bs.modal', async function(event) {
            console.log('?? Modal de libro abierto, cargando categorías...');
            await cargarCategorias();
            await cargarEditoriales();
        });
        
        // También cargar al inicio para tenerlas listas
        cargarCategorias();
    } else {
        console.error('? Modal modalLibro no encontrado');
    }
});
```

### Verificación SQL:
```sql
-- Verificar que existan categorías activas
SELECT Id, Nombre, Estado, IsDeleted, CategoriaPadreId
FROM Categorias
WHERE IsDeleted = 0 AND Estado = 'Activa';

-- Si no hay, insertar categorías de prueba
INSERT INTO Categorias (Id, Nombre, Descripcion, Estado, IsDeleted, CreatedAt, CategoriaPadreId)
VALUES 
(NEWID(), 'Ciencia Ficción', 'Novelas de ciencia ficción', 'Activa', 0, GETUTCDATE(), NULL),
(NEWID(), 'Fantasía', 'Mundos fantásticos', 'Activa', 0, GETUTCDATE(), NULL),
(NEWID(), 'Cyberpunk', 'Futuros tecnológicos', 'Activa', 0, GETUTCDATE(), NULL),
(NEWID(), 'Distopía', 'Sociedades oscuras', 'Activa', 0, GETUTCDATE(), NULL),
(NEWID(), 'Space Opera', 'Aventuras espaciales', 'Activa', 0, GETUTCDATE(), NULL);
```

---

## ?? SOLUCIÓN 2: Registro Rápido de Clientes

### Implementación Completa:

#### PASO 1: Crear DTO

**Archivo**: `DTOs/Usuario/UsuarioDTOs.cs`

```csharp
/// <summary>
/// DTO para registro rápido de clientes desde ventas
/// </summary>
public record RegistroRapidoClienteDTO
{
    [Required(ErrorMessage = "El nombre completo es requerido")]
    [StringLength(200, ErrorMessage = "El nombre no puede exceder 200 caracteres")]
    public string NombreCompleto { get; init; } = string.Empty;

    [Required(ErrorMessage = "El email es requerido")]
    [EmailAddress(ErrorMessage = "El email no es válido")]
    [StringLength(100, ErrorMessage = "El email no puede exceder 100 caracteres")]
    public string Email { get; init; } = string.Empty;

    [Phone(ErrorMessage = "El teléfono no es válido")]
    public string? Telefono { get; init; }
}
```

#### PASO 2: Endpoint API

**Archivo**: `Controllers/Api/UsuariosController.cs`

```csharp
// POST: api/Usuarios/registro-rapido
[HttpPost("registro-rapido")]
[AllowAnonymous] // Permitir desde ventas sin autenticación específica
public async Task<IActionResult> RegistroRapido([FromBody] RegistroRapidoClienteDTO model)
{
    try
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { error = "Datos inválidos", errors = ModelState });
        }

        // Generar username desde el email
        var username = GenerarUsernameDesdeEmail(model.Email);
        
        // Generar contraseña temporal
        var passwordTemporal = GenerarPasswordTemporal();

        // Crear DTO completo
        var crearUsuarioDto = new CrearUsuarioDTO
        {
            NombreCompleto = model.NombreCompleto,
            Email = model.Email,
            Username = username,
            Password = passwordTemporal,
            ConfirmPassword = passwordTemporal,
            Rol = RolUsuario.Cliente,
            Telefono = model.Telefono
        };

        var result = await _usuarioService.CrearUsuarioAsync(crearUsuarioDto);

        if (!result.Success)
        {
            return BadRequest(new { error = result.ErrorMessage });
        }

        return Ok(new
        {
            success = true,
            message = "Cliente registrado exitosamente",
            data = result.Data,
            credenciales = new
            {
                username = username,
                passwordTemporal = passwordTemporal,
                mensaje = "Credenciales generadas automáticamente. Se recomienda que el cliente cambie su contraseña."
            }
        });
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error en RegistroRapido");
        return StatusCode(500, new { error = "Error al registrar el cliente" });
    }
}

private string GenerarUsernameDesdeEmail(string email)
{
    // Obtener la parte antes del @
    var username = email.Split('@')[0];
    
    // Agregar timestamp para unicidad
    var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
    
    return $"{username}_{timestamp}";
}

private string GenerarPasswordTemporal()
{
    // Generar contraseña de 10 caracteres
    return Guid.NewGuid().ToString("N").Substring(0, 10);
}
```

#### PASO 3: Modal en Vista

**Archivos**: `Views/Vendedor/CrearVenta.cshtml` y `Views/Admin/CrearVenta.cshtml` (si existe)

```html
<!-- MODIFICAR EL SELECT DE CLIENTES -->
<div class="col-md-12">
    <label class="form-label fw-bold">Cliente <span class="text-danger">*</span></label>
    <div class="input-group">
        <select class="form-select" name="ClienteId" id="clienteId" required>
            <option value="">Seleccionar cliente...</option>
        </select>
        <button type="button" class="btn btn-success" 
                data-bs-toggle="modal" data-bs-target="#modalRegistroRapido"
                title="Registrar nuevo cliente">
            <i class="bi bi-person-plus"></i> Nuevo Cliente
        </button>
    </div>
</div>

<!-- MODAL DE REGISTRO RÁPIDO -->
<div class="modal fade" id="modalRegistroRapido" tabindex="-1">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header bg-success text-white">
                <h5 class="modal-title">
                    <i class="bi bi-person-plus me-2"></i>
                    Registro Rápido de Cliente
                </h5>
                <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
            </div>
            <div class="modal-body">
                <form id="formRegistroRapido">
                    <div class="mb-3">
                        <label class="form-label fw-bold">
                            Nombre Completo <span class="text-danger">*</span>
                        </label>
                        <input type="text" class="form-control" name="NombreCompleto" 
                               id="nombreCompletoRapido" required
                               placeholder="Ej: Juan Pérez García">
                    </div>
                    
                    <div class="mb-3">
                        <label class="form-label fw-bold">
                            Email <span class="text-danger">*</span>
                        </label>
                        <input type="email" class="form-control" name="Email" 
                               id="emailRapido" required
                               placeholder="Ej: juan.perez@ejemplo.com">
                        <div class="form-text">
                            Se usará para generar el usuario automáticamente
                        </div>
                    </div>
                    
                    <div class="mb-3">
                        <label class="form-label fw-bold">
                            Teléfono
                        </label>
                        <input type="tel" class="form-control" name="Telefono" 
                               id="telefonoRapido"
                               placeholder="Ej: 999888777">
                    </div>
                    
                    <div class="alert alert-info">
                        <i class="bi bi-info-circle me-2"></i>
                        <strong>Nota:</strong> Se generará automáticamente:
                        <ul class="mb-0 mt-2">
                            <li>Usuario: Se creará desde el email</li>
                            <li>Contraseña temporal: Se generará aleatoriamente</li>
                        </ul>
                    </div>
                </form>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">
                    <i class="bi bi-x-circle me-2"></i>Cancelar
                </button>
                <button type="button" class="btn btn-success" onclick="guardarClienteRapido()">
                    <i class="bi bi-save me-2"></i>Registrar Cliente
                </button>
            </div>
        </div>
    </div>
</div>

<!-- JAVASCRIPT -->
<script>
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
        telefono: formData.get('Telefono') || null
    };
    
    try {
        const response = await fetch('/api/Usuarios/registro-rapido', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(cliente)
        });
        
        const data = await response.json();
        
        if (response.ok && data.success) {
            // Mostrar credenciales generadas
            const credenciales = data.credenciales;
            alert(`? Cliente registrado exitosamente\n\n` +
                  `?? Email: ${cliente.email}\n` +
                  `?? Usuario: ${credenciales.username}\n` +
                  `?? Contraseña temporal: ${credenciales.passwordTemporal}\n\n` +
                  `?? IMPORTANTE: Anote estas credenciales y entréguelas al cliente.`);
            
            // Recargar clientes
            await cargarClientes();
            
            // Seleccionar el nuevo cliente
            const selectCliente = document.getElementById('clienteId');
            if (selectCliente && data.data && data.data.id) {
                selectCliente.value = data.data.id;
            }
            
            // Cerrar modal
            const modal = bootstrap.Modal.getInstance(document.getElementById('modalRegistroRapido'));
            modal.hide();
            
            // Limpiar formulario
            form.reset();
        } else {
            const errorMsg = data.error || 'Error desconocido al registrar el cliente';
            alert(`? Error: ${errorMsg}`);
        }
    } catch (error) {
        console.error('Error:', error);
        alert(`? Error de conexión: ${error.message}`);
    }
}

// Función para recargar clientes
async function cargarClientes() {
    try {
        const response = await fetch('/api/Usuarios/clientes-activos');
        const data = await response.json();
        
        if (data.success) {
            const select = document.getElementById('clienteId');
            const valorActual = select.value;
            
            select.innerHTML = '<option value="">Seleccionar cliente...</option>';
            
            data.data.forEach(cliente => {
                const option = document.createElement('option');
                option.value = cliente.id;
                option.textContent = `${cliente.nombreCompleto} (${cliente.email})`;
                select.appendChild(option);
            });
            
            // Restaurar selección si existía
            if (valorActual) {
                select.value = valorActual;
            }
        }
    } catch (error) {
        console.error('Error al cargar clientes:', error);
    }
}

// Cargar clientes al inicio
document.addEventListener('DOMContentLoaded', function() {
    cargarClientes();
});
</script>
```

---

## ?? SOLUCIÓN 3: Error al Crear/Editar Usuarios

### Diagnóstico:
El error "Error desconocido al guardar el usuario" indica que el backend está devolviendo un error sin el campo `error` o `message`.

### Solución:

**Archivo**: `Controllers/Api/UsuariosController.cs`

Verificar que TODOS los endpoints devuelvan la estructura correcta:

```csharp
// POST: api/Usuarios
[HttpPost]
public async Task<IActionResult> CrearUsuario([FromBody] CrearUsuarioDTO model)
{
    try
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            
            return BadRequest(new { 
                success = false,
                error = "Datos inválidos", 
                errors = errors 
            });
        }

        var result = await _usuarioService.CrearUsuarioAsync(model);

        if (!result.Success)
        {
            return BadRequest(new { 
                success = false,
                error = result.ErrorMessage 
            });
        }

        return Ok(new
        {
            success = true,
            message = "Usuario creado exitosamente",
            data = result.Data
        });
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error al crear usuario");
        return StatusCode(500, new { 
            success = false,
            error = $"Error interno del servidor: {ex.Message}" 
        });
    }
}

// PUT: api/Usuarios/{id}
[HttpPut("{id}")]
public async Task<IActionResult> ActualizarUsuario(Guid id, [FromBody] ActualizarUsuarioDTO model)
{
    try
    {
        if (id != model.Id)
        {
            return BadRequest(new { 
                success = false,
                error = "El ID no coincide" 
            });
        }

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            
            return BadRequest(new { 
                success = false,
                error = "Datos inválidos", 
                errors = errors 
            });
        }

        var result = await _usuarioService.ActualizarUsuarioAsync(model);

        if (!result.Success)
        {
            return BadRequest(new { 
                success = false,
                error = result.ErrorMessage 
            });
        }

        return Ok(new
        {
            success = true,
            message = "Usuario actualizado exitosamente",
            data = result.Data
        });
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error al actualizar usuario {UsuarioId}", id);
        return StatusCode(500, new { 
            success = false,
            error = $"Error interno del servidor: {ex.Message}" 
        });
    }
}
```

---

## ?? SOLUCIÓN 4: Toggle Usuarios Eliminados + Eliminación Definitiva

### Implementación Completa:

#### PASO 1: Endpoint para Obtener Usuarios Eliminados

**Archivo**: `Controllers/Api/UsuariosController.cs`

```csharp
// GET: api/Usuarios/eliminados
[HttpGet("eliminados")]
public async Task<IActionResult> ObtenerUsuariosEliminados()
{
    try
    {
        // Usar query sin filtro de soft delete
        var usuariosEliminados = await _usuarioService.ObtenerUsuariosEliminadosAsync();

        if (!usuariosEliminados.Success)
        {
            return BadRequest(new { error = usuariosEliminados.ErrorMessage });
        }

        return Ok(new
        {
            success = true,
            data = usuariosEliminados.Data
        });
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error al obtener usuarios eliminados");
        return StatusCode(500, new { error = "Error al obtener usuarios eliminados" });
    }
}

// DELETE: api/Usuarios/{id}/definitivo
[HttpDelete("{id}/definitivo")]
public async Task<IActionResult> EliminarDefinitivo(Guid id)
{
    try
    {
        var result = await _usuarioService.EliminarDefinitivamenteAsync(id);

        if (!result.Success)
        {
            return BadRequest(new { error = result.ErrorMessage });
        }

        return Ok(new
        {
            success = true,
            message = "Usuario eliminado definitivamente de la base de datos"
        });
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error al eliminar definitivamente usuario {UsuarioId}", id);
        return StatusCode(500, new { error = "Error al eliminar definitivamente el usuario" });
    }
}

// POST: api/Usuarios/{id}/restaurar
[HttpPost("{id}/restaurar")]
public async Task<IActionResult> RestaurarUsuario(Guid id)
{
    try
    {
        var result = await _usuarioService.RestaurarUsuarioAsync(id);

        if (!result.Success)
        {
            return BadRequest(new { error = result.ErrorMessage });
        }

        return Ok(new
        {
            success = true,
            message = "Usuario restaurado exitosamente"
        });
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error al restaurar usuario {UsuarioId}", id);
        return StatusCode(500, new { error = "Error al restaurar el usuario" });
    }
}
```

#### PASO 2: Métodos en el Servicio

**Archivo**: `Services/Interfaces/IUsuarioService.cs`

```csharp
Task<Result<IEnumerable<UsuarioDTO>>> ObtenerUsuariosEliminadosAsync(CancellationToken cancellationToken = default);
Task<Result> EliminarDefinitivamenteAsync(Guid usuarioId, CancellationToken cancellationToken = default);
Task<Result> RestaurarUsuarioAsync(Guid usuarioId, CancellationToken cancellationToken = default);
```

**Archivo**: `Services/UsuarioService.cs`

```csharp
public async Task<Result<IEnumerable<UsuarioDTO>>> ObtenerUsuariosEliminadosAsync(CancellationToken cancellationToken = default)
{
    try
    {
        // Obtener usuarios con IsDeleted = true, ignorando el query filter
        var usuarios = await _unitOfWork.Usuarios.Query()
            .IgnoreQueryFilters()
            .Where(u => u.IsDeleted)
            .OrderByDescending(u => u.UpdatedAt)
            .ToListAsync(cancellationToken);

        var usuariosDto = _mapper.Map<IEnumerable<UsuarioDTO>>(usuarios);

        return Result<IEnumerable<UsuarioDTO>>.SuccessResult(usuariosDto);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error en ObtenerUsuariosEliminadosAsync");
        return Result<IEnumerable<UsuarioDTO>>.FailureResult("Error al obtener usuarios eliminados");
    }
}

public async Task<Result> EliminarDefinitivamenteAsync(Guid usuarioId, CancellationToken cancellationToken = default)
{
    try
    {
        // Obtener usuario ignorando el filtro de soft delete
        var usuario = await _unitOfWork.Usuarios.Query()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Id == usuarioId, cancellationToken);

        if (usuario == null)
        {
            return Result.FailureResult("Usuario no encontrado");
        }

        if (!usuario.IsDeleted)
        {
            return Result.FailureResult("Solo se pueden eliminar definitivamente usuarios que ya están en la papelera");
        }

        // Eliminar de la base de datos (hard delete)
        _unitOfWork.Usuarios.Query().Remove(usuario);
        await _unitOfWork.CommitAsync(cancellationToken);

        _logger.LogWarning("Usuario {Username} eliminado definitivamente de la BD", usuario.Username);

        return Result.SuccessResult();
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error en EliminarDefinitivamenteAsync");
        return Result.FailureResult($"Error al eliminar definitivamente: {ex.Message}");
    }
}

public async Task<Result> RestaurarUsuarioAsync(Guid usuarioId, CancellationToken cancellationToken = default)
{
    try
    {
        var usuario = await _unitOfWork.Usuarios.Query()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Id == usuarioId, cancellationToken);

        if (usuario == null)
        {
            return Result.FailureResult("Usuario no encontrado");
        }

        if (!usuario.IsDeleted)
        {
            return Result.FailureResult("El usuario no está eliminado");
        }

        // Restaurar
        usuario.IsDeleted = false;
        usuario.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Usuarios.UpdateAsync(usuario, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        _logger.LogInformation("Usuario {Username} restaurado", usuario.Username);

        return Result.SuccessResult();
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error en RestaurarUsuarioAsync");
        return Result.FailureResult("Error al restaurar el usuario");
    }
}
```

#### PASO 3: Vista con Toggle

**Archivo**: `Views/Admin/Usuarios.cshtml`

Agregar después del encabezado:

```html
<!-- Toggle entre usuarios activos y eliminados -->
<div class="row mb-3">
    <div class="col-md-12">
        <div class="form-check form-switch">
            <input class="form-check-input" type="checkbox" id="toggleEliminados" 
                   onchange="toggleVistaEliminados()">
            <label class="form-check-label fw-bold" for="toggleEliminados">
                <i class="bi bi-trash"></i> Ver usuarios eliminados (Papelera)
            </label>
        </div>
    </div>
</div>

<!-- Tabla de usuarios activos (existente) -->
<div id="tablaUsuariosActivos" style="display: block;">
    <!-- ... tabla existente ... -->
</div>

<!-- Tabla de usuarios eliminados (nueva) -->
<div id="tablaUsuariosEliminados" style="display: none;">
    <div class="card shadow-sm">
        <div class="card-header bg-warning">
            <h5 class="mb-0">
                <i class="bi bi-trash me-2"></i>
                Usuarios Eliminados (Papelera)
            </h5>
        </div>
        <div class="card-body p-0">
            <div class="table-responsive">
                <table class="table table-hover align-middle mb-0" id="tablaEliminados">
                    <thead class="table-light">
                        <tr>
                            <th class="ps-4">Nombre Completo</th>
                            <th>Email</th>
                            <th>Username</th>
                            <th>Rol</th>
                            <th>Fecha Eliminación</th>
                            <th class="text-center pe-4">Acciones</th>
                        </tr>
                    </thead>
                    <tbody id="tbodyEliminados">
                        <!-- Se llenará dinámicamente -->
                    </tbody>
                </table>
            </div>
        </div>
    </div>
</div>

<script>
let vistaEliminados = false;

async function toggleVistaEliminados() {
    vistaEliminados = document.getElementById('toggleEliminados').checked;
    
    if (vistaEliminados) {
        document.getElementById('tablaUsuariosActivos').style.display = 'none';
        document.getElementById('tablaUsuariosEliminados').style.display = 'block';
        await cargarUsuariosEliminados();
    } else {
        document.getElementById('tablaUsuariosActivos').style.display = 'block';
        document.getElementById('tablaUsuariosEliminados').style.display = 'none';
    }
}

async function cargarUsuariosEliminados() {
    try {
        const response = await fetch('/api/Usuarios/eliminados');
        const data = await response.json();
        
        const tbody = document.getElementById('tbodyEliminados');
        
        if (data.success && data.data && data.data.length > 0) {
            tbody.innerHTML = data.data.map(usuario => {
                const rolColor = getRolColor(usuario.rolNombre);
                
                return `
                    <tr>
                        <td class="ps-4">
                            <strong>${usuario.nombreCompleto}</strong>
                        </td>
                        <td>
                            <i class="bi bi-envelope text-muted me-1"></i>
                            <small>${usuario.email}</small>
                        </td>
                        <td>
                            <code>${usuario.username}</code>
                        </td>
                        <td>
                            <span class="badge bg-${rolColor}">${usuario.rolNombre}</span>
                        </td>
                        <td>
                            <small class="text-muted">
                                ${new Date(usuario.updatedAt).toLocaleString('es-PE')}
                            </small>
                        </td>
                        <td class="text-center pe-4">
                            <div class="btn-group" role="group">
                                <button type="button" 
                                        class="btn btn-sm btn-outline-success" 
                                        onclick="restaurarUsuario('${usuario.id}', '${usuario.nombreCompleto}')"
                                        title="Restaurar">
                                    <i class="bi bi-arrow-counterclockwise"></i> Restaurar
                                </button>
                                <button type="button" 
                                        class="btn btn-sm btn-outline-danger" 
                                        onclick="eliminarDefinitivo('${usuario.id}', '${usuario.nombreCompleto}')"
                                        title="Eliminar definitivamente">
                                    <i class="bi bi-trash-fill"></i> Eliminar Definitivo
                                </button>
                            </div>
                        </td>
                    </tr>
                `;
            }).join('');
        } else {
            tbody.innerHTML = `
                <tr>
                    <td colspan="6" class="text-center text-muted py-5">
                        <i class="bi bi-inbox fs-1 d-block mb-3"></i>
                        <h5>No hay usuarios en la papelera</h5>
                    </td>
                </tr>
            `;
        }
    } catch (error) {
        console.error('Error al cargar usuarios eliminados:', error);
        alert('Error al cargar usuarios eliminados');
    }
}

async function restaurarUsuario(id, nombre) {
    if (!confirm(`¿Desea restaurar al usuario "${nombre}"?\n\nEl usuario volverá a estar activo en el sistema.`)) {
        return;
    }
    
    try {
        const response = await fetch(`/api/Usuarios/${id}/restaurar`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' }
        });
        
        const data = await response.json();
        
        if (response.ok && data.success) {
            alert('? Usuario restaurado exitosamente');
            await cargarUsuariosEliminados();
        } else {
            alert('? Error: ' + (data.error || 'Error desconocido'));
        }
    } catch (error) {
        console.error('Error:', error);
        alert('? Error de conexión');
    }
}

async function eliminarDefinitivo(id, nombre) {
    if (!confirm(`?? ADVERTENCIA ??\n\n¿Está ABSOLUTAMENTE SEGURO de eliminar DEFINITIVAMENTE al usuario "${nombre}"?\n\nEsta acción NO SE PUEDE DESHACER.\nEl usuario será eliminado permanentemente de la base de datos.`)) {
        return;
    }
    
    // Segunda confirmación
    const confirmacion = prompt(`Para confirmar, escriba el nombre completo del usuario:\n"${nombre}"`);
    
    if (confirmacion !== nombre) {
        alert('? Confirmación incorrecta. Operación cancelada.');
        return;
    }
    
    try {
        const response = await fetch(`/api/Usuarios/${id}/definitivo`, {
            method: 'DELETE',
            headers: { 'Content-Type': 'application/json' }
        });
        
        const data = await response.json();
        
        if (response.ok && data.success) {
            alert('? Usuario eliminado definitivamente de la base de datos');
            await cargarUsuariosEliminados();
        } else {
            alert('? Error: ' + (data.error || 'Error desconocido'));
        }
    } catch (error) {
        console.error('Error:', error);
        alert('? Error de conexión');
    }
}
</script>
```

---

## ? CHECKLIST DE IMPLEMENTACIÓN

- [ ] Mejorar función cargarCategorias() en Inventario.cshtml
- [ ] Crear DTO RegistroRapidoClienteDTO
- [ ] Crear endpoint /api/Usuarios/registro-rapido
- [ ] Agregar modal de registro rápido en CrearVenta.cshtml
- [ ] Corregir estructura de respuestas en UsuariosController
- [ ] Crear métodos para usuarios eliminados en IUsuarioService
- [ ] Implementar métodos en UsuarioService
- [ ] Crear endpoints para eliminados/restaurar/definitivo
- [ ] Agregar toggle y tabla de eliminados en Usuarios.cshtml

---

**Total Estimado**: 4-5 horas de trabajo
**Prioridad Alta**: Categorías, Error usuarios
**Prioridad Media**: Registro rápido
**Prioridad Baja**: Vista eliminados (mejora UX)
