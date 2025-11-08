// ========================================
// CARRITO ULTRA SIMPLE - SOLO VISUAL
// ========================================

let carritoData = null;

document.addEventListener('DOMContentLoaded', () => {
    console.log('🛒 Carrito ULTRA SIMPLE');
    
    if (esCliente()) {
        actualizarContadorCarrito();
        
        if (window.location.pathname.includes('/Carrito')) {
            cargarCarrito();
        }
    }
});

function esCliente() {
    return document.getElementById('carrito-contador') !== null;
}

// ========================================
// FORMATO DE MONEDA
// ========================================

function formatearMoneda(valor) {
    return parseFloat(valor || 0).toFixed(2);
}

// ========================================
// AGREGAR AL CARRITO
// ========================================

async function agregarAlCarrito(libroId, cantidad = 1) {
    try {
        const response = await fetch('/api/CarritoApi/agregar', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ LibroId: libroId, Cantidad: cantidad })
        });

        const data = await response.json();

        if (data.success) {
            mostrarNotificacion('✅ Agregado', 'success');
            await actualizarContadorCarrito();
            
            if (window.location.pathname.includes('/Carrito')) {
                await cargarCarrito();
            }
        } else {
            mostrarNotificacion('❌ ' + (data.error || 'Error'), 'error');
        }
    } catch (error) {
        console.error('Error:', error);
        mostrarNotificacion('❌ Error', 'error');
    }
}

// ========================================
// ACTUALIZAR CONTADOR
// ========================================

async function actualizarContadorCarrito() {
    try {
        const response = await fetch('/api/CarritoApi/resumen');
        const data = await response.json();

        const contador = document.getElementById('carrito-contador');
        if (contador && data.success && data.data) {
            const cant = data.data.cantidadItems || 0;
            contador.textContent = cant;
            contador.style.display = cant > 0 ? 'inline-block' : 'none';
        }
    } catch (error) {
        console.error('Error contador:', error);
    }
}

// ========================================
// CARGAR CARRITO
// ========================================

async function cargarCarrito() {
    try {
        const response = await fetch('/api/CarritoApi/obtener');
        const data = await response.json();

        if (data.success && data.data && data.data.items && data.data.items.length > 0) {
            carritoData = data.data;
            renderizarCarrito();
        } else {
            carritoData = null;
            renderizarCarritoVacio();
        }
    } catch (error) {
        console.error('Error:', error);
        renderizarCarritoVacio();
    }
}

// ========================================
// RENDERIZAR CARRITO
// ========================================

function renderizarCarrito() {
    const container = document.getElementById('carrito-items');
    if (!container) return;

    if (!carritoData || !carritoData.items || carritoData.items.length === 0) {
        renderizarCarritoVacio();
        return;
    }

    let html = '<div class="list-group mb-3">';
    
    carritoData.items.forEach(item => {
        html += `
            <div class="list-group-item">
                <div class="row align-items-center">
                    <div class="col-md-2">
                        ${item.imagenPortada ? `<img src="${item.imagenPortada}" alt="${item.titulo}" class="img-fluid rounded" style="max-height:100px">` : '<div class="bg-secondary rounded" style="height:80px"></div>'}
                    </div>
                    <div class="col-md-3">
                        <h6 class="mb-1">${item.titulo}</h6>
                        <small class="text-muted">${item.autor || 'Sin autor'}</small>
                    </div>
                    <div class="col-md-2 text-center">
                        <small class="text-muted">Precio</small><br>
                        <strong>S/ ${formatearMoneda(item.precioUnitario)}</strong>
                    </div>
                    <div class="col-md-2">
                        <label class="form-label small mb-1">Cantidad</label>
                        <div class="input-group input-group-sm">
                            <button class="btn btn-outline-secondary" 
                                    onclick="cambiarCantidadVisual('${item.libroId}', -1)">
                                <i class="bi bi-dash"></i>
                            </button>
                            <input type="number" 
                                   class="form-control text-center cantidad-input" 
                                   value="${item.cantidad}" 
                                   min="1" 
                                   max="${item.stockDisponible || 999}"
                                   data-libro-id="${item.libroId}"
                                   data-precio="${item.precioUnitario}"
                                   onchange="actualizarResumen()">
                            <button class="btn btn-outline-secondary" 
                                    onclick="cambiarCantidadVisual('${item.libroId}', 1)">
                                <i class="bi bi-plus"></i>
                            </button>
                        </div>
                        <small class="text-muted">Stock: ${item.stockDisponible || 'N/A'}</small>
                    </div>
                    <div class="col-md-2 text-center subtotal-item" data-libro-id="${item.libroId}">
                        <small class="text-muted">Subtotal</small><br>
                        <strong class="text-primary">S/ ${formatearMoneda(item.subtotal)}</strong>
                    </div>
                    <div class="col-md-1 text-end">
                        <button class="btn btn-sm btn-danger" 
                                onclick="eliminarDelCarrito('${item.libroId}')" 
                                title="Eliminar">
                            <i class="bi bi-trash"></i>
                        </button>
                    </div>
                </div>
            </div>
        `;
    });

    html += '</div>';
    
    // Resumen
    html += `
        <div class="card">
            <div class="card-body">
                <h5 class="mb-3">Resumen del Pedido</h5>
                
                <div class="d-flex justify-content-between mb-2">
                    <span>Subtotal:</span>
                    <strong id="resumen-subtotal">S/ ${formatearMoneda(carritoData.subtotal)}</strong>
                </div>
                
                <div class="d-flex justify-content-between mb-2">
                    <span>IGV (18%):</span>
                    <strong id="resumen-igv">S/ ${formatearMoneda(carritoData.igv)}</strong>
                </div>
                
                <hr>
                
                <div class="d-flex justify-content-between mb-3">
                    <h5 class="mb-0">Total a Pagar:</h5>
                    <h4 class="mb-0 text-success" id="resumen-total">S/ ${formatearMoneda(carritoData.total)}</h4>
                </div>
                
                <hr>
                
                <div class="d-grid gap-2">
                    <a href="/Carrito/Checkout" class="btn btn-success btn-lg">
                        <i class="bi bi-credit-card"></i> Proceder al Pago
                    </a>
                    <button type="button" class="btn btn-outline-danger" onclick="vaciarCarrito()">
                        <i class="bi bi-trash"></i> Vaciar Carrito
                    </button>
                    <a href="/Catalogo" class="btn btn-outline-secondary">
                        <i class="bi bi-arrow-left"></i> Seguir Comprando
                    </a>
                </div>
            </div>
        </div>
    `;

    container.innerHTML = html;
    console.log('✅ Carrito renderizado');
}

function renderizarCarritoVacio() {
    const container = document.getElementById('carrito-items');
    if (!container) return;

    container.innerHTML = `
        <div class="alert alert-info text-center">
            <i class="bi bi-cart-x" style="font-size: 3rem;"></i>
            <h4 class="mt-3">Carrito vacío</h4>
            <p>¡Explora nuestro catálogo!</p>
            <a href="/Catalogo" class="btn btn-primary">
                <i class="bi bi-book"></i> Ver Catálogo
            </a>
        </div>
    `;
}

// ========================================
// CAMBIAR CANTIDAD (SOLO VISUAL)
// ========================================

function cambiarCantidadVisual(libroId, cambio) {
    const input = document.querySelector(`input[data-libro-id="${libroId}"]`);
    if (!input) return;
    
    let nuevaCantidad = parseInt(input.value) + cambio;
    if (nuevaCantidad < 1) nuevaCantidad = 1;
    
    const max = parseInt(input.max);
    if (nuevaCantidad > max) {
        mostrarNotificacion(`⚠️ Stock: ${max}`, 'warning');
        return;
    }
    
    input.value = nuevaCantidad;
    actualizarResumen();
}

function actualizarResumen() {
    let subtotal = 0;
    
    // Recalcular todos los subtotales y el total
    document.querySelectorAll('.cantidad-input').forEach(input => {
        const libroId = input.dataset.libroId;
        const precio = parseFloat(input.dataset.precio);
        const cantidad = parseInt(input.value) || 1;
        const subtotalItem = precio * cantidad;
        
        // Actualizar subtotal del item
        const subtotalElement = document.querySelector(`.subtotal-item[data-libro-id="${libroId}"] strong`);
        if (subtotalElement) {
            subtotalElement.textContent = `S/ ${formatearMoneda(subtotalItem)}`;
        }
        
        subtotal += subtotalItem;
    });
    
    const igv = subtotal * 0.18;
    const total = subtotal + igv;
    
    // Actualizar resumen
    const subtotalEl = document.getElementById('resumen-subtotal');
    const igvEl = document.getElementById('resumen-igv');
    const totalEl = document.getElementById('resumen-total');
    
    if (subtotalEl) subtotalEl.textContent = `S/ ${formatearMoneda(subtotal)}`;
    if (igvEl) igvEl.textContent = `S/ ${formatearMoneda(igv)}`;
    if (totalEl) totalEl.textContent = `S/ ${formatearMoneda(total)}`;
}

// ========================================
// ELIMINAR
// ========================================

async function eliminarDelCarrito(libroId) {
    if (!confirm('¿Eliminar este producto?')) return;
    
    try {
        const response = await fetch(`/api/CarritoApi/eliminar/${libroId}`, {
            method: 'DELETE'
        });

        const data = await response.json();

        if (data.success) {
            mostrarNotificacion('✅ Eliminado', 'success');
            await cargarCarrito();
            await actualizarContadorCarrito();
        } else {
            mostrarNotificacion('❌ Error', 'error');
        }
    } catch (error) {
        console.error('Error:', error);
        mostrarNotificacion('❌ Error', 'error');
    }
}

// ========================================
// VACIAR
// ========================================

async function vaciarCarrito() {
    if (!confirm('¿Vaciar todo el carrito?')) return;
    
    try {
        const response = await fetch('/api/CarritoApi/vaciar', {
            method: 'DELETE'
        });

        const data = await response.json();

        if (data.success) {
            mostrarNotificacion('✅ Vaciado', 'success');
            carritoData = null;
            renderizarCarritoVacio();
            await actualizarContadorCarrito();
        } else {
            mostrarNotificacion('❌ Error', 'error');
        }
    } catch (error) {
        console.error('Error:', error);
        mostrarNotificacion('❌ Error', 'error');
    }
}

// ========================================
// NOTIFICACIONES
// ========================================

function mostrarNotificacion(mensaje, tipo = 'info') {
    const alertClass = tipo === 'success' ? 'alert-success' : 
                      tipo === 'error' ? 'alert-danger' : 
                      tipo === 'warning' ? 'alert-warning' : 'alert-info';
    
    const alertHtml = `
        <div class="alert ${alertClass} alert-dismissible fade show position-fixed top-0 end-0 m-3" 
             style="z-index: 9999; min-width: 300px;">
            ${mensaje}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        </div>
    `;
    
    document.body.insertAdjacentHTML('beforeend', alertHtml);
    
    setTimeout(() => {
        const alerts = document.querySelectorAll('.alert');
        alerts.forEach(alert => {
            if (alert.textContent.includes(mensaje.replace(/<[^>]*>/g, ''))) {
                alert.remove();
            }
        });
    }, 3000);
}

// ========================================
// EXPORTAR
// ========================================

window.agregarAlCarrito = agregarAlCarrito;
window.cambiarCantidadVisual = cambiarCantidadVisual;
window.actualizarResumen = actualizarResumen;
window.eliminarDelCarrito = eliminarDelCarrito;
window.vaciarCarrito = vaciarCarrito;
window.actualizarContadorCarrito = actualizarContadorCarrito;

console.log('✅ Carrito ULTRA SIMPLE cargado');
console.log('ℹ️ Cambios solo visuales hasta checkout');
