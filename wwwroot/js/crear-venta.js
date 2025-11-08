/**
 * crear-venta.js
 * Manejo de la funcionalidad de creación de ventas
 * Con manejo robusto de errores
 */

let itemsVenta = [];
let totalVenta = 0;
let clienteSeleccionado = null;

/**
 * Extrae mensajes de error detallados de respuestas del servidor
 */
function extraerMensajeError(data) {
    // 1. Errores de validación por campo (ModelState)
    if (data.errors) {
        const errores = Object.entries(data.errors)
            .map(([campo, mensajes]) => {
                const msgs = Array.isArray(mensajes) ? mensajes.join(', ') : mensajes;
                return `• ${campo}: ${msgs}`;
            })
            .join('\n');
        return `Errores de validación:\n${errores}`;
    }
    
    // 2. ProblemDetails de ASP.NET
    if (data.title) {
        let mensaje = data.title;
        if (data.detail) {
            mensaje += `\n${data.detail}`;
        }
        if (data.traceId) {
            mensaje += `\n\nTrace ID: ${data.traceId}`;
        }
        return mensaje;
    }
    
    // 3. Mensaje personalizado del controller
    if (data.error) {
        return data.error;
    }
    
    // 4. Fallback
    return 'Error desconocido. Por favor, contacte al administrador.';
}

/**
 * Guarda un cliente rápido desde el modal
 */
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
    
    console.log('?? Registrando cliente rápido:', cliente);
    
    try {
        const response = await fetch('/api/Usuarios/registro-rapido', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(cliente)
        });
        
        const data = await response.json();
        console.log('?? Respuesta registro cliente:', data);
        
        if (response.ok && data.success) {
            const credenciales = data.credenciales;
            alert(`? Cliente registrado exitosamente\n\n` +
                  `?? Email: ${cliente.email}\n` +
                  `?? Usuario: ${credenciales.username}\n` +
                  `?? Contraseña temporal: ${credenciales.passwordTemporal}\n\n` +
                  `?? IMPORTANTE: Anote estas credenciales y entréguelas al cliente.`);
            
            // Seleccionar el nuevo cliente automáticamente
            clienteSeleccionado = {
                id: data.data.id,
                nombre: data.data.nombreCompleto,
                email: data.data.email,
                username: credenciales.username
            };
            
            document.getElementById('clienteId').value = clienteSeleccionado.id;
            document.getElementById('buscarCliente').value = clienteSeleccionado.nombre;
            
            // Cerrar modal
            const modal = bootstrap.Modal.getInstance(document.getElementById('modalRegistroRapido'));
            modal.hide();
            form.reset();
        } else {
            const errorMsg = extraerMensajeError(data);
            console.error('? Error al registrar cliente:', data);
            alert(`? Error al registrar cliente:\n\n${errorMsg}`);
        }
    } catch (error) {
        console.error('? Error de conexión:', error);
        alert(`? Error de conexión:\n\n${error.message}`);
    }
}

/**
 * Búsqueda de clientes con autocompletado
 */
let searchClienteTimeout;
document.addEventListener('DOMContentLoaded', function() {
    const buscarCliente = document.getElementById('buscarCliente');
    if (!buscarCliente) return;
    
    buscarCliente.addEventListener('input', async function(e) {
        clearTimeout(searchClienteTimeout);
        const query = e.target.value;
        const resultsDiv = document.getElementById('resultadosClientes');
        
        if (query.length < 2) {
            resultsDiv.style.display = 'none';
            return;
        }
        
        searchClienteTimeout = setTimeout(async () => {
            try {
                console.log('?? Buscando clientes:', query);
                const response = await fetch(`/api/Busqueda/clientes?q=${encodeURIComponent(query)}`);
                const data = await response.json();
                console.log('?? Resultados de clientes:', data);
                
                if (data.results && data.results.length > 0) {
                    resultsDiv.innerHTML = data.results.map(cliente => `
                        <a href="#" class="list-group-item list-group-item-action" onclick='seleccionarCliente(${JSON.stringify(cliente)}); return false;'>
                            <strong>${cliente.nombre}</strong><br>
                            <small>Email: ${cliente.email} | Username: ${cliente.username}</small>
                        </a>
                    `).join('');
                    resultsDiv.style.display = 'block';
                } else {
                    resultsDiv.innerHTML = '<div class="list-group-item text-muted">No se encontraron clientes</div>';
                    resultsDiv.style.display = 'block';
                }
            } catch (error) {
                console.error('? Error al buscar clientes:', error);
            }
        }, 300);
    });
});

/**
 * Selecciona un cliente del autocompletado
 */
function seleccionarCliente(cliente) {
    console.log('? Cliente seleccionado:', cliente);
    clienteSeleccionado = cliente;
    document.getElementById('clienteId').value = cliente.id;
    document.getElementById('buscarCliente').value = cliente.nombre;
    document.getElementById('resultadosClientes').style.display = 'none';
}

/**
 * Búsqueda de libros con autocompletado
 */
let searchLibroTimeout;
document.addEventListener('DOMContentLoaded', function() {
    const buscarLibro = document.getElementById('buscarLibro');
    if (!buscarLibro) return;
    
    buscarLibro.addEventListener('input', async function(e) {
        clearTimeout(searchLibroTimeout);
        const query = e.target.value;
        const resultsDiv = document.getElementById('resultadosLibros');
        
        if (query.length < 2) {
            resultsDiv.style.display = 'none';
            return;
        }
        
        searchLibroTimeout = setTimeout(async () => {
            try {
                console.log('?? Buscando libros:', query);
                const response = await fetch(`/api/Busqueda/libros?q=${encodeURIComponent(query)}`);
                const data = await response.json();
                console.log('?? Resultados de libros:', data);
                
                if (data.results && data.results.length > 0) {
                    resultsDiv.innerHTML = data.results.map(libro => `
                        <a href="#" class="list-group-item list-group-item-action" onclick='agregarItem(${JSON.stringify(libro)}); return false;'>
                            <strong>${libro.titulo}</strong> - ${libro.autor}<br>
                            <small>Precio: S/ ${libro.precio.toFixed(2)} | Stock: ${libro.stock}</small>
                        </a>
                    `).join('');
                    resultsDiv.style.display = 'block';
                } else {
                    resultsDiv.innerHTML = '<div class="list-group-item text-muted">No se encontraron libros</div>';
                    resultsDiv.style.display = 'block';
                }
            } catch (error) {
                console.error('? Error al buscar libros:', error);
            }
        }, 300);
    });
});

/**
 * Agrega un libro a la lista de items de la venta
 */
function agregarItem(libro) {
    console.log('?? Agregando libro:', libro);
    
    // Validar stock
    if (libro.stock <= 0) {
        alert(`? El libro "${libro.titulo}" no tiene stock disponible`);
        return;
    }
    
    // Verificar si ya existe
    const existente = itemsVenta.find(i => i.id === libro.id);
    if (existente) {
        if (existente.cantidad >= libro.stock) {
            alert(`? No hay más stock disponible de "${libro.titulo}"\nStock disponible: ${libro.stock}`);
            return;
        }
        existente.cantidad++;
        existente.subtotal = existente.cantidad * existente.precio;
        console.log('? Cantidad incrementada:', existente);
    } else {
        itemsVenta.push({
            id: libro.id,
            titulo: libro.titulo,
            autor: libro.autor,
            cantidad: 1,
            precio: libro.precio,
            subtotal: libro.precio,
            stockDisponible: libro.stock
        });
        console.log('? Libro agregado a la lista');
    }
    
    actualizarTablaItems();
    document.getElementById('buscarLibro').value = '';
    document.getElementById('resultadosLibros').style.display = 'none';
}

/**
 * Actualiza la tabla de items
 */
function actualizarTablaItems() {
    const tbody = document.querySelector('#tablaItems tbody');
    
    if (itemsVenta.length === 0) {
        tbody.innerHTML = '<tr><td colspan="5" class="text-center text-muted">No hay items agregados</td></tr>';
        totalVenta = 0;
        document.getElementById('totalVenta').textContent = '0.00';
        return;
    }
    
    tbody.innerHTML = itemsVenta.map((item, index) => `
        <tr>
            <td>
                <strong>${item.titulo}</strong><br>
                <small class="text-muted">${item.autor}</small>
            </td>
            <td>
                <div class="input-group input-group-sm" style="width: 120px;">
                    <button class="btn btn-outline-secondary" type="button" onclick="cambiarCantidad(${index}, -1)">-</button>
                    <input type="number" class="form-control text-center" value="${item.cantidad}" readonly>
                    <button class="btn btn-outline-secondary" type="button" onclick="cambiarCantidad(${index}, 1)">+</button>
                </div>
                <small class="text-muted">Stock: ${item.stockDisponible}</small>
            </td>
            <td>S/ ${item.precio.toFixed(2)}</td>
            <td><strong>S/ ${item.subtotal.toFixed(2)}</strong></td>
            <td>
                <button class="btn btn-sm btn-outline-danger" onclick="eliminarItem(${index})" title="Eliminar">
                    <i class="bi bi-trash"></i>
                </button>
            </td>
        </tr>
    `).join('');
    
    totalVenta = itemsVenta.reduce((sum, item) => sum + item.subtotal, 0);
    document.getElementById('totalVenta').textContent = totalVenta.toFixed(2);
    
    console.log('?? Total actualizado:', totalVenta);
}

/**
 * Cambia la cantidad de un item
 */
function cambiarCantidad(index, delta) {
    const item = itemsVenta[index];
    const nuevaCantidad = item.cantidad + delta;
    
    if (nuevaCantidad <= 0) {
        eliminarItem(index);
        return;
    }
    
    if (nuevaCantidad > item.stockDisponible) {
        alert(`? Stock insuficiente.\nStock disponible: ${item.stockDisponible}`);
        return;
    }
    
    item.cantidad = nuevaCantidad;
    item.subtotal = item.cantidad * item.precio;
    
    console.log(`? Cantidad modificada: ${item.titulo} x${item.cantidad}`);
    actualizarTablaItems();
}

/**
 * Elimina un item de la lista
 */
function eliminarItem(index) {
    const item = itemsVenta[index];
    console.log(`??? Eliminando item: ${item.titulo}`);
    itemsVenta.splice(index, 1);
    actualizarTablaItems();
}

/**
 * Registra la venta
 */
document.addEventListener('DOMContentLoaded', function() {
    const form = document.getElementById('formNuevaVenta');
    if (!form) return;
    
    form.addEventListener('submit', async function(event) {
        event.preventDefault();
        
        console.log('?? Iniciando registro de venta...');
        console.log('?? Cliente seleccionado:', clienteSeleccionado);
        console.log('?? Items:', itemsVenta);
        
        // Validaciones
        if (!clienteSeleccionado || !clienteSeleccionado.id) {
            alert('? Debe seleccionar un cliente');
            document.getElementById('buscarCliente').focus();
            return;
        }
        
        if (itemsVenta.length === 0) {
            alert('? Debe agregar al menos un libro a la venta');
            document.getElementById('buscarLibro').focus();
            return;
        }
        
        const formData = new FormData(form);
        const metodoPago = formData.get('MetodoPago');
        const direccionEnvio = formData.get('DireccionEnvio');
        
        if (!metodoPago) {
            alert('? Debe seleccionar un método de pago');
            return;
        }
        
        if (!direccionEnvio || direccionEnvio.trim() === '') {
            alert('? Debe ingresar la dirección de envío');
            return;
        }
        
        const venta = {
            clienteId: clienteSeleccionado.id,
            metodoPago: metodoPago,
            direccionEnvio: direccionEnvio.trim(),
            detalles: itemsVenta.map(item => ({
                libroId: item.id,
                cantidad: item.cantidad,
                precioUnitario: item.precio,
                descuento: 0
            }))
        };
        
        console.log('?? Datos de venta a enviar:', venta);
        
        try {
            // Primero calcular totales
            console.log('?? Calculando totales...');
            const calcResponse = await fetch('/api/Ventas/calcular-totales', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ items: venta.detalles })
            });
            
            const calcData = await calcResponse.json();
            console.log('? Totales calculados:', calcData);
            
            if (!calcResponse.ok || !calcData.success) {
                const errorMsg = extraerMensajeError(calcData);
                alert(`? Error al calcular totales:\n\n${errorMsg}`);
                return;
            }
            
            // Ahora registrar la venta
            console.log('?? Enviando venta...');
            const response = await fetch('/api/Ventas', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(venta)
            });
            
            const data = await response.json();
            console.log('?? Respuesta recibida, status:', response.status);
            console.log('?? Datos de respuesta:', data);
            
            if (response.ok && data.success) {
                alert(`? Venta registrada exitosamente\n\nNúmero de Venta: ${data.numeroVenta || data.data?.numeroVenta || 'N/A'}`);
                window.location.href = '/Vendedor/Ventas';
            } else {
                const errorMsg = extraerMensajeError(data);
                console.error('? Error del servidor completo:', JSON.stringify(data, null, 2));
                alert(`? Error al registrar venta:\n\n${errorMsg}`);
            }
        } catch (error) {
            console.error('? Error inesperado:', error);
            alert(`? Error de conexión:\n\n${error.message}`);
        }
    });
});

// Cerrar autocompletados al hacer click fuera
document.addEventListener('click', function(e) {
    if (!e.target.closest('#buscarCliente') && !e.target.closest('#resultadosClientes')) {
        document.getElementById('resultadosClientes').style.display = 'none';
    }
    if (!e.target.closest('#buscarLibro') && !e.target.closest('#resultadosLibros')) {
        document.getElementById('resultadosLibros').style.display = 'none';
    }
});
