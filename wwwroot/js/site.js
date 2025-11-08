// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// SciFiHub - Site-wide JavaScript

// =====================================================
// CONFIGURACIÓN GLOBAL DE AJAX
// =====================================================

// Agregar token anti-falsificación a todas las peticiones AJAX
$.ajaxSetup({
    beforeSend: function(xhr, settings) {
        if (settings.type === 'POST' || settings.type === 'PUT' || settings.type === 'DELETE') {
            const token = $('input[name="__RequestVerificationToken"]').val();
            if (token) {
                xhr.setRequestHeader('RequestVerificationToken', token);
            }
        }
    }
});

// =====================================================
// FUNCIONES GLOBALES
// =====================================================

// Mostrar toast notification
function showToast(message, type = 'info') {
    const toastHtml = `
        <div class="toast align-items-center text-white bg-${type} border-0" role="alert" aria-live="assertive" aria-atomic="true">
            <div class="d-flex">
                <div class="toast-body">
                    ${message}
                </div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
            </div>
        </div>
    `;
    
    const toastContainer = $('#toast-container');
    if (toastContainer.length === 0) {
        $('body').append('<div id="toast-container" class="toast-container position-fixed bottom-0 end-0 p-3"></div>');
    }
    
    const $toast = $(toastHtml);
    $('#toast-container').append($toast);
    
    const toast = new bootstrap.Toast($toast[0], { delay: 3000 });
    toast.show();
    
    $toast.on('hidden.bs.toast', function () {
        $(this).remove();
    });
}

// =====================================================
// CARRITO DE COMPRAS
// =====================================================

const Carrito = {
    // Agregar item al carrito
    agregarItem: function(libroId, cantidad = 1) {
        $.ajax({
            url: '/Carrito/AgregarItem',
            type: 'POST',
            data: {
                LibroId: libroId,
                Cantidad: cantidad,
                __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
            },
            success: function(response) {
                if (response.success) {
                    showToast('Libro agregado al carrito correctamente', 'success');
                    Carrito.actualizarBadge();
                } else {
                    showToast(response.message || 'Error al agregar al carrito', 'danger');
                }
            },
            error: function() {
                showToast('Error de conexión. Intenta nuevamente.', 'danger');
            }
        });
    },
    
    // Actualizar cantidad de item
    actualizarCantidad: function(libroId, nuevaCantidad) {
        $.ajax({
            url: '/Carrito/ActualizarCantidad',
            type: 'POST',
            data: {
                LibroId: libroId,
                NuevaCantidad: nuevaCantidad,
                __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
            },
            success: function(response) {
                if (response.success) {
                    showToast('Cantidad actualizada', 'success');
                    location.reload();
                } else {
                    showToast(response.message || 'Error al actualizar cantidad', 'danger');
                }
            },
            error: function() {
                showToast('Error de conexión. Intenta nuevamente.', 'danger');
            }
        });
    },
    
    // Eliminar item del carrito
    eliminarItem: function(libroId) {
        if (!confirm('¿Estás seguro de eliminar este libro del carrito?')) {
            return;
        }
        
        $.ajax({
            url: '/Carrito/EliminarItem',
            type: 'POST',
            data: { 
                LibroId: libroId,
                __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
            },
            success: function(response) {
                if (response.success) {
                    showToast('Libro eliminado del carrito', 'success');
                    location.reload();
                } else {
                    showToast(response.message || 'Error al eliminar del carrito', 'danger');
                }
            },
            error: function() {
                showToast('Error de conexión. Intenta nuevamente.', 'danger');
            }
        });
    },
    
    // Actualizar badge del carrito
    actualizarBadge: function() {
        $.get('/Carrito/ObtenerResumen', function(response) {
            if (response.success && response.data) {
                $('#carrito-badge').text(response.data.cantidadItems);
            }
        });
    }
};

// =====================================================
// INICIALIZACIÓN
// =====================================================

$(document).ready(function() {
    
    // Actualizar badge del carrito al cargar
    if ($('#carrito-badge').length > 0) {
        Carrito.actualizarBadge();
    }
    
    // Event handler para botones de agregar al carrito
    $(document).on('click', '.btn-add-carrito', function(e) {
        e.preventDefault();
        const libroId = $(this).data('libro-id');
        Carrito.agregarItem(libroId, 1);
    });
    
    // Event handler para botón de agregar con cantidad (detalle)
    $(document).on('click', '.btn-add-carrito-detalle', function(e) {
        e.preventDefault();
        const libroId = $(this).data('libro-id');
        const cantidad = parseInt($('#cantidad').val()) || 1;
        Carrito.agregarItem(libroId, cantidad);
    });
    
    // Event handler para actualizar cantidad en carrito
    $(document).on('change', '.cantidad-input', function() {
        const libroId = $(this).data('libro-id');
        const nuevaCantidad = parseInt($(this).val());
        if (nuevaCantidad > 0) {
            Carrito.actualizarCantidad(libroId, nuevaCantidad);
        }
    });
    
    // Event handler para eliminar item
    $(document).on('click', '.btn-eliminar-item', function(e) {
        e.preventDefault();
        const libroId = $(this).data('libro-id');
        Carrito.eliminarItem(libroId);
    });
    
    // Auto-dismiss de alerts después de 5 segundos (solo las que tienen clase alert-dismissible)
    setTimeout(function() {
        $('.alert.alert-dismissible:not(.alert-permanent)').fadeOut('slow');
    }, 5000);
    
    // Confirmar antes de cerrar sesión
    $('form[action*="Logout"]').submit(function(e) {
        if (!confirm('¿Estás seguro de cerrar sesión?')) {
            e.preventDefault();
            return false;
        }
    });
    
    // Validación de formularios Bootstrap
    const forms = document.querySelectorAll('.needs-validation');
    Array.from(forms).forEach(form => {
        form.addEventListener('submit', event => {
            if (!form.checkValidity()) {
                event.preventDefault();
                event.stopPropagation();
            }
            form.classList.add('was-validated');
        }, false);
    });
});
