// ========================================
// INVENTARIO ADMIN - SciFiHub
// ========================================

let categorias = [];
let editoriales = [];

// ========================================
// EVENTOS DE INICIALIZACIÓN
// ========================================

document.addEventListener('DOMContentLoaded', function() {
    console.log('?? Inventario Admin cargado');
    
    // Cargar datos iniciales
    cargarCategorias();
    cargarEditoriales();
    
    // Event listeners
    configurarEventListeners();
});

// Evento al abrir modal
document.getElementById('modalLibro')?.addEventListener('show.bs.modal', async function() {
    console.log('?? Modal de libro abierto');
    await cargarCategorias();
    await cargarEditoriales();
});

// Evento al cerrar modal
document.getElementById('modalLibro')?.addEventListener('hidden.bs.modal', function() {
    resetearFormulario();
});

// ========================================
// CONFIGURACIÓN DE EVENT LISTENERS
// ========================================

function configurarEventListeners() {
    // Búsqueda
    const searchInput = document.getElementById('searchInput');
    if (searchInput) {
        let searchTimeout;
        searchInput.addEventListener('input', function(e) {
            clearTimeout(searchTimeout);
            const query = e.target.value.trim();
            
            if (query.length < 2) {
                document.getElementById('searchResults').style.display = 'none';
                mostrarTodos();
                return;
            }
            
            searchTimeout = setTimeout(() => buscarLibros(query), 300);
        });
    }
    
    // Cálculo de descuento
    const precio = document.getElementById('precio');
    const precioOferta = document.getElementById('precioOferta');
    
    if (precio) precio.addEventListener('input', calcularDescuento);
    if (precioOferta) precioOferta.addEventListener('input', calcularDescuento);
}

// ========================================
// CARGAR DATOS
// ========================================

async function cargarCategorias() {
    try {
        console.log('?? Iniciando carga de categorías...');
        
        const response = await fetch('/api/Libros/categorias');
        
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        
        const data = await response.json();
        console.log('?? Datos recibidos:', data);
        
        if (data.success && data.data && Array.isArray(data.data) && data.data.length > 0) {
            categorias = data.data;
            const select = document.getElementById('categoriaId');
            
            if (!select) {
                console.error('? Select categoriaId no encontrado');
                return;
            }
            
            // Limpiar y agregar opciones
            select.innerHTML = '<option value="">Seleccionar categoría...</option>';
            
            data.data.forEach(cat => {
                const option = document.createElement('option');
                option.value = cat.id;
                option.textContent = cat.nombre;
                select.appendChild(option);
            });
            
            console.log(`? ${data.data.length} categorías cargadas`);
        } else {
            console.warn('?? No se encontraron categorías');
            mostrarAlertaCategorias();
        }
    } catch (error) {
        console.error('? Error al cargar categorías:', error);
        mostrarAlertaCategorias();
    }
}

async function cargarEditoriales() {
    try {
        const response = await fetch('/api/Libros/editoriales');
        
        if (!response.ok) {
            console.warn('?? Endpoint de editoriales no disponible');
            return;
        }
        
        const data = await response.json();
        
        if (data.success && data.data) {
            editoriales = data.data;
            const datalist = document.getElementById('editorialesDatalist');
            
            if (datalist) {
                datalist.innerHTML = '';
                data.data.forEach(ed => {
                    const option = document.createElement('option');
                    option.value = ed;
                    datalist.appendChild(option);
                });
                
                console.log(`? ${data.data.length} editoriales cargadas`);
            }
        }
    } catch (error) {
        console.error('Error al cargar editoriales:', error);
    }
}

// ========================================
// CRUD LIBROS
// ========================================

async function editarLibro(id) {
    try {
        console.log('?? Editando libro:', id);
        
        const response = await fetch(`/api/Libros/${id}`);
        const data = await response.json();
        
        if (!data.success) {
            alert('? Error al cargar el libro');
            return;
        }
        
        const libro = data.data;
        
        // Cambiar título del modal
        document.getElementById('tituloModal').textContent = 'Editar Libro';
        document.getElementById('iconoModal').className = 'bi bi-pencil-square me-2';
        
        // Rellenar formulario
        document.getElementById('libroId').value = libro.id;
        document.getElementById('isbn').value = libro.isbn;
        document.getElementById('titulo').value = libro.titulo;
        document.getElementById('autor').value = libro.autor;
        document.getElementById('editorial').value = libro.editorial || '';
        document.getElementById('categoriaId').value = libro.categoriaId;
        document.getElementById('anioPublicacion').value = libro.añoPublicacion || new Date().getFullYear();
        document.getElementById('paginas').value = libro.paginas || 100;
        document.getElementById('precio').value = libro.precio;
        document.getElementById('precioOferta').value = libro.precioOferta || '';
        document.getElementById('stock').value = libro.stock;
        document.getElementById('estado').value = libro.estado;
        document.getElementById('sinopsis').value = libro.sinopsis || '';
        document.getElementById('descripcion').value = libro.descripcion || '';
        document.getElementById('destacado').checked = libro.destacado;
        
        calcularDescuento();
        
        // Mostrar modal
        const modal = new bootstrap.Modal(document.getElementById('modalLibro'));
        modal.show();
        
    } catch (error) {
        console.error('Error:', error);
        alert('? Error al cargar el libro');
    }
}

async function verDetalleLibro(id) {
    window.location.href = `/Catalogo/Detalle/${id}`;
}

async function eliminarLibro(id, titulo) {
    if (!confirm(`¿Está seguro de eliminar el libro?\n\n"${titulo}"\n\nEsta acción no se puede deshacer.`)) {
        return;
    }
    
    try {
        const response = await fetch(`/api/Libros/${id}`, {
            method: 'DELETE',
            headers: { 'Content-Type': 'application/json' }
        });
        
        const data = await response.json();
        
        if (data.success) {
            alert('? Libro eliminado exitosamente');
            window.location.reload();
        } else {
            alert('? Error: ' + data.error);
        }
    } catch (error) {
        console.error('Error:', error);
        alert('? Error al eliminar el libro');
    }
}

async function guardarLibro() {
    const form = document.getElementById('formLibro');
    
    if (!form.checkValidity()) {
        form.reportValidity();
        return;
    }
    
    const formData = new FormData(form);
    const libroId = document.getElementById('libroId').value;
    const esEdicion = libroId !== '';
    
    const libro = {
        isbn: formData.get('ISBN'),
        titulo: formData.get('Titulo'),
        autor: formData.get('Autor'),
        editorial: formData.get('Editorial') || null,
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
    
    if (esEdicion) {
        libro.id = libroId;
    }
    
    try {
        const url = esEdicion ? `/api/Libros/${libroId}` : '/api/Libros';
        const method = esEdicion ? 'PUT' : 'POST';
        
        const response = await fetch(url, {
            method: method,
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(libro)
        });
        
        const data = await response.json();
        
        if (data.success) {
            alert(`? Libro ${esEdicion ? 'actualizado' : 'creado'} exitosamente`);
            window.location.reload();
        } else {
            alert('? Error: ' + data.error);
        }
    } catch (error) {
        console.error('Error:', error);
        alert('? Error al guardar el libro');
    }
}

// ========================================
// NUMBER PICKERS
// ========================================

function cambiarStock(delta) {
    const input = document.getElementById('stock');
    const valor = parseInt(input.value) || 0;
    const nuevoValor = Math.max(0, valor + delta);
    input.value = nuevoValor;
}

function cambiarAnio(delta) {
    const input = document.getElementById('anioPublicacion');
    const valor = parseInt(input.value) || new Date().getFullYear();
    const nuevoValor = Math.max(1800, Math.min(new Date().getFullYear(), valor + delta));
    input.value = nuevoValor;
}

function cambiarPaginas(delta) {
    const input = document.getElementById('paginas');
    const valor = parseInt(input.value) || 100;
    const nuevoValor = Math.max(1, valor + delta);
    input.value = nuevoValor;
}

// ========================================
// UTILIDADES
// ========================================

function calcularDescuento() {
    const precio = parseFloat(document.getElementById('precio').value) || 0;
    const precioOferta = parseFloat(document.getElementById('precioOferta').value) || 0;
    const infoDiv = document.getElementById('descuentoInfo');
    
    if (precioOferta > 0 && precioOferta < precio) {
        const descuento = ((precio - precioOferta) / precio * 100).toFixed(0);
        infoDiv.innerHTML = `<span class="text-success"><i class="bi bi-tag"></i> Descuento: ${descuento}%</span>`;
    } else {
        infoDiv.innerHTML = '';
    }
}

async function buscarLibros(query) {
    try {
        const response = await fetch(`/api/Busqueda/libros?query=${encodeURIComponent(query)}`);
        const data = await response.json();
        const resultsDiv = document.getElementById('searchResults');
        
        if (data.items && data.items.length > 0) {
            resultsDiv.innerHTML = data.items.map(libro => `
                <button type="button" class="list-group-item list-group-item-action" 
                        onclick="seleccionarLibro('${libro.id}')">
                    <div class="d-flex justify-content-between">
                        <div>
                            <strong>${libro.titulo}</strong><br>
                            <small class="text-muted">${libro.autor} | ${libro.categoria}</small>
                        </div>
                        <div class="text-end">
                            <span class="badge ${libro.disponible ? 'bg-success' : 'bg-danger'}">
                                ${libro.disponible ? 'Disponible' : 'Agotado'}
                            </span>
                        </div>
                    </div>
                </button>
            `).join('');
            resultsDiv.style.display = 'block';
        } else {
            resultsDiv.innerHTML = `
                <div class="list-group-item text-muted text-center py-3">
                    <i class="bi bi-search"></i> No se encontraron resultados
                </div>
            `;
            resultsDiv.style.display = 'block';
        }
    } catch (error) {
        console.error('Error en búsqueda:', error);
    }
}

function seleccionarLibro(id) {
    document.getElementById('searchResults').style.display = 'none';
    document.querySelectorAll('#tablaInventario tbody tr').forEach(row => {
        row.style.display = row.dataset.libroId === id ? '' : 'none';
    });
}

function limpiarBusqueda() {
    document.getElementById('searchInput').value = '';
    document.getElementById('searchResults').style.display = 'none';
    mostrarTodos();
}

function mostrarTodos() {
    document.querySelectorAll('#tablaInventario tbody tr').forEach(row => {
        row.style.display = '';
    });
}

function filtrarPorStock(tipo) {
    document.querySelectorAll('#tablaInventario tbody tr').forEach(row => {
        const stock = parseInt(row.dataset.stock) || 0;
        const disponible = row.dataset.disponible === 'true';
        let mostrar = false;
        
        switch(tipo) {
            case 'todos':
                mostrar = true;
                break;
            case 'disponible':
                mostrar = disponible;
                break;
            case 'bajo':
                mostrar = stock > 0 && stock <= 10;
                break;
            case 'critico':
                mostrar = stock === 0;
                break;
        }
        
        row.style.display = mostrar ? '' : 'none';
    });
}

function resetearFormulario() {
    document.getElementById('formLibro').reset();
    document.getElementById('libroId').value = '';
    document.getElementById('tituloModal').textContent = 'Agregar Nuevo Libro';
    document.getElementById('iconoModal').className = 'bi bi-plus-circle me-2';
    document.getElementById('descuentoInfo').innerHTML = '';
}

function mostrarAlertaCategorias() {
    const select = document.getElementById('categoriaId');
    if (select) {
        select.innerHTML = '<option value="">No hay categorías disponibles</option>';
    }
    
    alert('?? No hay categorías disponibles.\n\nPor favor, cree al menos una categoría antes de agregar libros.');
}

// ========================================
// EXPORTAR FUNCIONES GLOBALES
// ========================================

window.editarLibro = editarLibro;
window.verDetalleLibro = verDetalleLibro;
window.eliminarLibro = eliminarLibro;
window.guardarLibro = guardarLibro;
window.cambiarStock = cambiarStock;
window.cambiarAnio = cambiarAnio;
window.cambiarPaginas = cambiarPaginas;
window.filtrarPorStock = filtrarPorStock;
window.limpiarBusqueda = limpiarBusqueda;
window.seleccionarLibro = seleccionarLibro;
