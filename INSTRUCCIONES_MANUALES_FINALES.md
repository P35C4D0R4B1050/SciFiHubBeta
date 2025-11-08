# ?? INSTRUCCIONES FINALES - SciFiHub

## ? PROBLEMAS YA SOLUCIONADOS (No Requieren Acción)

1. ? **Triggers de SQL Server** - Configurado `UseSqlOutputClause(false)`
2. ? **Sección Styles en _Layout** - Agregado `@RenderSectionAsync("Styles")`
3. ? **Vistas Profile y ChangePassword** - Ya están creadas correctamente
4. ? **Vista Ventas.cshtml** - Recreada con sintaxis correcta

---

## ?? PROBLEMAS QUE REQUIEREN ACCIÓN MANUAL

### Problema 1: Rol Incorrecto al Crear Usuario

**Diagnóstico**:
El problema está en el JavaScript de la vista `Usuarios.cshtml`. Los valores del enum no coinciden.

**Archivo**: `Views/Admin/Usuarios.cshtml`  
**Línea**: ~358-369

**ERROR ACTUAL**:
```html
<select class="form-select form-select-lg" name="Rol" id="rol" required>
    <option value="">Seleccionar rol...</option>
    <option value="2">??? Administrador</option>
    <option value="1">????? Vendedor</option>
    <option value="0">?? Cliente</option>
</select>
```

**CORRECCIÓN NECESARIA**:
```html
<select class="form-select form-select-lg" name="Rol" id="rol" required>
    <option value="">Seleccionar rol...</option>
    <option value="Administrador">??? Administrador - Acceso total</option>
    <option value="Vendedor">????? Vendedor - Gestión de libros y ventas</option>
    <option value="Cliente">?? Cliente - Compra de libros</option>
</select>
```

**EXPLICACIÓN**:
Los enums en C# se están convirtiendo a strings en la base de datos (ver `SciFiHubDbContext.cs` línea 46-49). Por lo tanto, el frontend debe enviar el nombre del enum (string), no el número.

---

### Problema 2: Endpoint de Editoriales Falta Implementar

**Archivo a Modificar**: `Services/Interfaces/ILibroService.cs`

**Agregar**:
```csharp
Task<Result<IEnumerable<string>>> ObtenerEditorialesAsync(CancellationToken cancellationToken = default);
```

**Archivo a Modificar**: `Services/LibroService.cs`

**Agregar al final de la clase**:
```csharp
public async Task<Result<IEnumerable<string>>> ObtenerEditorialesAsync(CancellationToken cancellationToken = default)
{
    try
    {
        var editoriales = await _unitOfWork.Libros.Query()
            .Where(l => !string.IsNullOrEmpty(l.Editorial))
            .Select(l => l.Editorial!)
            .Distinct()
            .OrderBy(e => e)
            .ToListAsync(cancellationToken);

        return Result<IEnumerable<string>>.SuccessResult(editoriales);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error en ObtenerEditorialesAsync");
        return Result<IEnumerable<string>>.FailureResult("Error al obtener editoriales");
    }
}
```

---

### Problema 3: Cargar Categorías y Editoriales en Modal de Libro

**Archivo a Modificar**: `Views/Vendedor/Inventario.cshtml` (y también `Views/Admin/Inventario.cshtml` si existe)

**Buscar** el evento de abrir modal (puede estar cerca del final del archivo en la sección `@section Scripts`)

**Agregar**:
```javascript
// Cargar categorías al abrir el modal
document.getElementById('modalLibro').addEventListener('show.bs.modal', async function() {
    await cargarCategorias();
    await cargarEditoriales();
});

async function cargarCategorias() {
    try {
        const response = await fetch('/api/Libros/categorias');
        const data = await response.json();
        
        if (data.success) {
            const select = document.getElementById('categoriaId');
            select.innerHTML = '<option value="">Seleccionar categoría...</option>';
            
            data.data.forEach(cat => {
                select.innerHTML += `<option value="${cat.id}">${cat.nombre}</option>`;
            });
        }
    } catch (error) {
        console.error('Error al cargar categorías:', error);
    }
}

async function cargarEditoriales() {
    try {
        const response = await fetch('/api/Libros/editoriales');
        const data = await response.json();
        
        if (data.success) {
            const datalist = document.getElementById('editorialesDatalist');
            if (datalist) {
                datalist.innerHTML = '';
                data.data.forEach(ed => {
                    datalist.innerHTML += `<option value="${ed}">`;
                });
            }
        }
    } catch (error) {
        console.error('Error al cargar editoriales:', error);
    }
}
```

**En el HTML del modal de libro**, cambiar el campo de Editorial:
```html
<!-- ANTES -->
<input type="text" class="form-control" name="Editorial" id="editorial">

<!-- DESPUÉS -->
<input type="text" list="editorialesDatalist" class="form-control" name="Editorial" id="editorial">
<datalist id="editorialesDatalist"></datalist>
```

---

### Problema 4: Number Pickers para Año y Páginas

**Archivo a Modificar**: `Views/Vendedor/Inventario.cshtml` (línea ~200-250 aproximadamente, en el formulario del modal)

**Buscar** los campos de Año de Publicación y Páginas

**REEMPLAZAR**:
```html
<!-- Año de Publicación - ANTES -->
<div class="col-md-3">
    <label>Año Publicación</label>
    <input type="number" class="form-control" name="AñoPublicacion" id="anioPublicacion">
</div>

<!-- Año de Publicación - DESPUÉS -->
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

<!-- Páginas - ANTES -->
<div class="col-md-3">
    <label>Páginas</label>
    <input type="number" class="form-control" name="Paginas" id="paginas">
</div>

<!-- Páginas - DESPUÉS -->
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

**Agregar en el JavaScript** (sección `@section Scripts`):
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

## ?? LISTA DE VERIFICACIÓN

Después de hacer estos cambios manualmente:

- [ ] **Paso 1**: Corregir valores del select de Rol en `Views/Admin/Usuarios.cshtml`
- [ ] **Paso 2**: Agregar método `ObtenerEditorialesAsync` en `ILibroService.cs`
- [ ] **Paso 3**: Implementar método en `LibroService.cs`
- [ ] **Paso 4**: Agregar funciones `cargarCategorias()` y `cargarEditoriales()` en `Inventario.cshtml`
- [ ] **Paso 5**: Cambiar input de Editorial a usar datalist
- [ ] **Paso 6**: Agregar number pickers para Año y Páginas
- [ ] **Paso 7**: Agregar funciones JavaScript `cambiarAnio()` y `cambiarPaginas()`
- [ ] **Paso 8**: Compilar y probar

---

## ?? PRUEBAS A REALIZAR

Después de implementar los cambios:

1. **Crear Usuario**:
   - ? Crear como Administrador
   - ? Crear como Vendedor
   - ? Crear como Cliente
   - ? Verificar en BD que el rol se guardó correctamente

2. **Editar/Eliminar Usuario**:
   - ? Editar información
   - ? Eliminar usuario (no debe permitir eliminar último admin)

3. **Crear Libro**:
   - ? Verificar que se cargan las categorías
   - ? Verificar que se cargan las editoriales
   - ? Probar number pickers de Año y Páginas

4. **Vistas de Perfil**:
   - ? Acceder a "Mi Perfil"
   - ? Acceder a "Cambiar Contraseña"
   - ? Cambiar contraseña exitosamente

---

## ?? PROBLEMAS CONOCIDOS Y SOLUCIONES

### Error: "La aplicación está en depuración"
**Solución**: Detener el debugger, hacer los cambios, y volver a iniciar.

### Error: "Hot reload no funcionó"
**Solución**: 
1. Guardar todos los archivos
2. Detener la aplicación
3. Hacer Clean Solution
4. Rebuild Solution
5. Iniciar nuevamente

### Error en base de datos después de cambios
**Solución**:
Si modificaste algo en la BD y tienes errores, ejecuta:
```sql
-- Verificar que los enums se están guardando como strings
SELECT TOP 10 Rol, Estado FROM Usuarios;
-- Deberían verse como 'Administrador', 'Vendedor', 'Cliente'
-- NO como números
```

---

## ?? RESUMEN FINAL

| Cambio | Archivo | Línea Aprox | Tiempo Est. |
|--------|---------|-------------|-------------|
| Valores del select Rol | Views/Admin/Usuarios.cshtml | 358-369 | 2 min |
| Método ObtenerEditoriales | ILibroService.cs | Final | 1 min |
| Implementación método | LibroService.cs | Final | 3 min |
| cargarCategorias() | Inventario.cshtml | Scripts | 5 min |
| cargarEditoriales() | Inventario.cshtml | Scripts | 5 min |
| Input Editorial datalist | Inventario.cshtml | ~200 | 2 min |
| Number picker Año | Inventario.cshtml | ~220 | 5 min |
| Number picker Páginas | Inventario.cshtml | ~230 | 5 min |
| Funciones JavaScript | Inventario.cshtml | Scripts | 3 min |

**Tiempo Total Estimado**: ~30 minutos

---

## ? VERIFICACIÓN FINAL

Ejecutar estos comandos después de todos los cambios:

```bash
# Limpiar y recompilar
dotnet clean
dotnet build

# Verificar que no hay errores
dotnet build --no-restore

# Ejecutar la aplicación
dotnet run
```

---

**Nota Importante**: Todos estos cambios son MANUALES porque la aplicación está en modo depuración y los cambios automáticos no se aplican correctamente.

**Última Actualización**: Enero 2025  
**Estado**: Listo para implementación manual
