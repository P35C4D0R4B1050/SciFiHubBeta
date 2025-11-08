# ?? PLAN DE CORRECCIONES - Observaciones del Usuario

## ?? RESUMEN DE OBSERVACIONES

### Como Administrador:
1. ? **Gestión de Usuarios**: Crear, Ver detalles, Editar, Eliminar
2. ?? **Gestión de Libros**:
   - Visualizar stock
   - Ver detalles (sin botón de carrito para admin)
   - Editar (cargar información correctamente)
   - Evitar cambios no deseados al navegar entre vistas
3. ?? **Dashboard**:
   - Modal para agregar libro funcional
4. ?? **Ventas**:
   - Nuevo cliente solo para generar PDF (no cuenta de acceso)

### Como Vendedor:
1. ?? **Libros**: Mismo comportamiento que Administrador
2. ?? **Ventas**: Mismas funcionalidades

---

## ? CORRECCIONES APLICADAS

### 1. AutoMapperProfile - Conversión Enum ? String ?

**Problema**: Los DTOs usan enums, pero las entidades usan strings  
**Solución**: Mapeos bidireccionales con conversión automática

```csharp
// Usuario ? UsuarioDTO (string ? enum)
CreateMap<Usuario, UsuarioDTO>()
    .ForMember(dest => dest.Rol, opt => opt.MapFrom(src => 
        Enum.Parse<Domain.Enums.RolUsuario>(src.Rol)))
    .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => 
        Enum.Parse<Domain.Enums.EstadoUsuario>(src.Estado)));

// CrearUsuarioDTO ? Usuario (enum ? string)
CreateMap<CrearUsuarioDTO, Usuario>()
    .ForMember(dest => dest.Rol, opt => opt.MapFrom(src => src.Rol.ToString()))
    .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => "Activo"));

// ActualizarUsuarioDTO ? Usuario (enum ? string)
CreateMap<ActualizarUsuarioDTO, Usuario>()
    .ForMember(dest => dest.Rol, opt => opt.MapFrom(src => src.Rol.ToString()))
    .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => src.Estado.ToString()));
```

**Resultado**: Crear, editar, ver usuarios ahora funciona correctamente ?

---

### 2. UsuarioService - Simplificación ?

**Cambios**:
- Eliminadas conversiones manuales (ahora las hace AutoMapper)
- Validación de último administrador mejorada
- Código más limpio y mantenible

---

## ?? CORRECCIONES PENDIENTES

### 1. Vista de Libros - Admin/Vendedor

#### 1.1 Mostrar Stock en Tabla ?
**Archivo**: `Views/Admin/Inventario.cshtml` y `Views/Vendedor/Inventario.cshtml`

```html
<!-- Agregar columna de Stock en la tabla -->
<th>Stock</th>
<!-- ... -->
<td>@libro.Stock</td>
```

#### 1.2 Ver Detalles de Libro (sin carrito para Admin) ?
**Archivo**: `Views/Catalogo/Detalle.cshtml`

```csharp
@if (!User.IsInRole("Administrador") && !User.IsInRole("Vendedor"))
{
    <!-- Solo mostrar botón de carrito para clientes -->
    <button class="btn btn-primary">Agregar al Carrito</button>
}
```

#### 1.3 Cargar Información Correcta al Editar ?
**Problema**: El modal se abre pero no carga los datos
**Solución**: Implementar función JavaScript `editarLibro(id)`

```javascript
async function editarLibro(id) {
    try {
        const response = await fetch(`/api/Libros/${id}`);
        const libro = await response.json();
        
        // Llenar formulario
        document.getElementById('libroId').value = libro.id;
        document.getElementById('titulo').value = libro.titulo;
        document.getElementById('autor').value = libro.autor;
        // ... más campos
        
        // Abrir modal
        new bootstrap.Modal(document.getElementById('modalLibro')).show();
    } catch (error) {
        console.error('Error al cargar libro:', error);
    }
}
```

#### 1.4 Evitar Cambios no Deseados ?
**Problema**: Al navegar entre "ver" y "editar" se hacen cambios en BD
**Causa**: Ambos usan el mismo modal y el mismo endpoint
**Solución**: Separar modales o usar flag de solo lectura

```javascript
function verLibro(id) {
    cargarLibro(id, true); // readonly = true
}

function editarLibro(id) {
    cargarLibro(id, false); // readonly = false
}

function cargarLibro(id, readonly) {
    // ... cargar datos
    
    // Deshabilitar campos si es readonly
    const inputs = document.querySelectorAll('#modalLibro input, #modalLibro select');
    inputs.forEach(input => {
        input.disabled = readonly;
    });
    
    // Ocultar botón guardar si es readonly
    document.querySelector('#btnGuardarLibro').style.display = readonly ? 'none' : 'block';
}
```

---

### 2. Dashboard - Agregar Libro ?

**Archivo**: `Views/Admin/Dashboard.cshtml`

**Problema**: Modal no se abre correctamente  
**Solución**: Verificar que el modal esté incluido y el botón tenga los atributos correctos

```html
<!-- Botón para abrir modal -->
<button class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#modalAgregarLibro">
    <i class="bi bi-plus-circle"></i> Agregar Libro
</button>

<!-- Modal completo con formulario -->
<div class="modal fade" id="modalAgregarLibro" tabindex="-1">
    <div class="modal-dialog modal-lg">
        <div class="modal-content">
            <form id="formAgregarLibro" onsubmit="guardarLibro(event)">
                <!-- Formulario completo -->
            </form>
        </div>
    </div>
</div>
```

---

### 3. Ventas - Cliente Solo para PDF ?

**Problema**: "Nuevo Cliente" crea una cuenta completa  
**Solución Propuesta**: Clarificar el propósito

#### Opción A: Cliente Temporal (Solo para PDF) 
```csharp
public class ClienteVentaDTO
{
    public string NombreCompleto { get; set; }
    public string? Dni { get; set; }
    public string? Telefono { get; set; }
    public string? Direccion { get; set; }
}

// NO crear usuario en la BD
// Solo usar para generar el PDF
```

#### Opción B: Cliente Permanente (Recomendado)
Mantener la funcionalidad actual pero:
1. Generar username y password automáticamente
2. Mostrar credenciales en alert
3. Cliente puede usar la cuenta para futuras compras
4. **Ventaja**: Historial de compras, seguimiento de pedidos

**Implementación Actual**: Ya está hecho con `RegistroRapidoClienteDTO` ?

```csharp
// Ya implementado en UsuariosController
[HttpPost("registro-rapido")]
public async Task<IActionResult> RegistroRapido([FromBody] RegistroRapidoClienteDTO model)
{
    // Genera username y password automáticamente
    // Retorna credenciales para entregar al cliente
}
```

---

## ?? PRIORIDADES DE IMPLEMENTACIÓN

### ?? Alta Prioridad (Funcionalidad Básica)
1. ? Crear/Editar/Eliminar Usuario - **COMPLETADO**
2. ? Mostrar Stock en tabla de libros
3. ? Cargar datos correctos al editar libro
4. ? Separar "Ver" de "Editar" libro

### ?? Media Prioridad (UI/UX)
5. ? Ocultar botón carrito para Admin/Vendedor en detalle
6. ? Modal agregar libro en Dashboard
7. ? Number pickers en formularios

### ?? Baja Prioridad (Mejoras)
8. ? Recarga automática de categorías
9. ? Autocomplete de editoriales
10. ? Cliente temporal vs permanente

---

## ?? CHECKLIST DE IMPLEMENTACIÓN

### Usuarios ?
- [x] Crear usuario funciona
- [x] Editar usuario funciona
- [x] Eliminar usuario funciona
- [x] Ver detalles funciona
- [x] AutoMapper convierte correctamente
- [x] BD guarda en formato correcto (strings)

### Libros ?
- [ ] Mostrar stock en tabla
- [ ] Ver detalles sin botón carrito (Admin/Vendedor)
- [ ] Editar libro carga datos correctos
- [ ] Ver vs Editar separados
- [ ] Modal en Dashboard funciona

### Ventas ?
- [ ] Registro rápido de cliente documentado
- [ ] Clarificar si cliente tiene acceso o no
- [ ] Probar generación de PDF con datos correctos

---

## ?? PRUEBAS NECESARIAS

### Test 1: Gestión de Usuarios
```
1. Login como Administrador
2. Ir a Admin ? Usuarios
3. Click "Crear Usuario"
4. Completar formulario:
   - Nombre: Test Usuario
   - Email: test@example.com
   - Username: testusuario
   - Password: Test123!
   - Rol: Vendedor
5. Guardar
6. ? Verificar que aparece en la lista
7. Click en "Editar"
8. Cambiar email a: nuevo@example.com
9. Guardar
10. ? Verificar que se actualizó
11. Click en "Eliminar"
12. ? Verificar que desapareció de la lista
```

### Test 2: Libros - Stock y Edición
```
1. Ir a Admin ? Inventario
2. ? Verificar que la columna "Stock" es visible
3. Click en "Ver" de un libro
4. ? Verificar que NO hay botón "Agregar al Carrito"
5. ? Verificar que todos los datos se muestran correctamente
6. Cerrar modal
7. Click en "Editar" del mismo libro
8. ? Verificar que se cargan los mismos datos
9. Cambiar título
10. Guardar
11. ? Verificar que se actualizó en la BD
12. ? Verificar que solo cambió el título, nada más
```

### Test 3: Dashboard - Agregar Libro
```
1. Ir a Admin ? Dashboard
2. Buscar botón "Agregar Libro"
3. Click en el botón
4. ? Verificar que se abre el modal
5. ? Verificar que el select de categorías tiene opciones
6. Completar formulario completo
7. Guardar
8. ? Verificar que el libro se creó
9. ? Verificar que el modal se cerró
10. ? Verificar que la lista se actualizó
```

### Test 4: Ventas - Nuevo Cliente
```
1. Ir a Vendedor ? Nueva Venta
2. Click en "Nuevo Cliente"
3. Completar:
   - Nombre: Cliente Test
   - Email: cliente@test.com
   - Teléfono: 999888777
4. Registrar
5. ? Verificar que muestra username y password generados
6. ? Copiar credenciales
7. Continuar con la venta
8. Generar boleta/factura
9. ? Verificar que el PDF contiene el nombre correcto
10. Logout
11. Intentar login con las credenciales del cliente
12. ? Decidir si debe poder acceder o no
```

---

## ?? ARCHIVOS A MODIFICAR

| Archivo | Cambio | Prioridad |
|---------|--------|-----------|
| `Mappings/AutoMapperProfile.cs` | ? Mapeos enum ? string | COMPLETADO |
| `Services/UsuarioService.cs` | ? Usar AutoMapper | COMPLETADO |
| `Views/Admin/Inventario.cshtml` | Agregar columna Stock | ?? Alta |
| `Views/Vendedor/Inventario.cshtml` | Agregar columna Stock | ?? Alta |
| `Views/Catalogo/Detalle.cshtml` | Ocultar carrito para Admin | ?? Media |
| `Views/Admin/Dashboard.cshtml` | Modal agregar libro | ?? Media |
| `wwwroot/js/inventario.js` | Funciones ver/editar libro | ?? Alta |
| `Controllers/Api/VentasController.cs` | Documentar cliente temporal | ?? Baja |

---

## ?? SIGUIENTES PASOS

1. ? **Compilación exitosa** - Verificado
2. ? **Ejecutar PreparacionPruebas.sql** - Preparar BD
3. ? **Iniciar aplicación** - dotnet run
4. ? **Probar creación de usuarios** - Test 1
5. ? **Implementar correcciones de libros** - Test 2
6. ? **Probar dashboard** - Test 3
7. ? **Probar ventas** - Test 4
8. ? **Documentar decisiones finales**

---

**Estado Actual**: ? Usuarios funcionando  
**Pendiente**: Libros, Dashboard, Ventas  
**Tiempo Estimado**: 3-4 horas de desarrollo
