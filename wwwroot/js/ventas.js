// ========================================
// GESTIÓN DE VENTAS - ADMIN/VENDEDOR
// ========================================

console.log('?? Inicializando gestión de ventas...');

// ========================================
// CANCELAR VENTA
// ========================================

async function cancelarVenta(ventaId) {
    console.log('?? Iniciando cancelación de venta:', ventaId);
    
    const motivo = prompt('Ingrese el motivo de la cancelación:');
    if (!motivo || motivo.trim() === '') {
        alert('? Debe ingresar un motivo para cancelar la venta');
        return;
    }
    
    if (!confirm('¿Está seguro de cancelar esta venta? Esta acción restaurará el stock y no se puede deshacer.')) {
        return;
    }
    
    try {
        const response = await fetch('/api/Ventas/cancelar', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                ventaId: ventaId,
                motivo: motivo.trim()
            })
        });
        
        const data = await response.json();
        console.log('?? Respuesta cancelar:', data);
        
        if (data.success) {
            alert('? Venta cancelada exitosamente. El stock ha sido restaurado.');
            window.location.reload();
        } else {
            alert('? Error: ' + data.error);
        }
    } catch (error) {
        console.error('? Error al cancelar venta:', error);
        alert('? Error al cancelar la venta. Por favor intente nuevamente.');
    }
}

// ========================================
// COMPLETAR VENTA
// ========================================

async function completarVenta(ventaId) {
    console.log('? Completando venta:', ventaId);
    
    if (!confirm('¿Confirmar que la venta fue completada exitosamente?')) {
        return;
    }
    
    try {
        const response = await fetch(`/api/Ventas/${ventaId}/completar`, {
            method: 'POST'
        });
        
        const data = await response.json();
        
        if (data.success) {
            alert('? Venta completada exitosamente');
            window.location.reload();
        } else {
            alert('? Error: ' + data.error);
        }
    } catch (error) {
        console.error('? Error al completar venta:', error);
        alert('? Error al completar la venta');
    }
}

// ========================================
// DESCARGAR PDF
// ========================================

function descargarPDF(ventaId) {
    console.log('?? Descargando PDF de venta:', ventaId);
    window.open(`/api/Ventas/${ventaId}/pdf`, '_blank');
}

// ========================================
// EXPORTAR FUNCIONES GLOBALES
// ========================================

window.cancelarVenta = cancelarVenta;
window.completarVenta = completarVenta;
window.descargarPDF = descargarPDF;

console.log('? Gestión de ventas inicializada');
