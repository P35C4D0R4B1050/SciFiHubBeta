// ========================================
// CATÁLOGO - FILTROS Y BÚSQUEDA
// ========================================

console.log('?? Inicializando catálogo...');

document.addEventListener('DOMContentLoaded', () => {
    inicializarFiltros();
});

// ========================================
// INICIALIZAR FILTROS
// ========================================

function inicializarFiltros() {
    // Checkbox "Solo disponibles"
    const checkboxDisponibles = document.getElementById('soloDisponibles');
    if (checkboxDisponibles) {
        checkboxDisponibles.addEventListener('change', function() {
            console.log('? Checkbox "Solo disponibles" cambiado:', this.checked);
            aplicarFiltros();
        });
    }
    
    // Select de categoría
    const selectCategoria = document.getElementById('categoriaFiltro');
    if (selectCategoria) {
        selectCategoria.addEventListener('change', function() {
            console.log('?? Categoría seleccionada:', this.value);
            aplicarFiltros();
        });
    }
    
    // Búsqueda
    const inputBusqueda = document.getElementById('textoBusqueda');
    if (inputBusqueda) {
        // Debounce para evitar muchas requests
        let timeout = null;
        inputBusqueda.addEventListener('input', function() {
            clearTimeout(timeout);
            timeout = setTimeout(() => {
                console.log('?? Búsqueda:', this.value);
                aplicarFiltros();
            }, 500);
        });
    }
    
    console.log('? Filtros inicializados');
}

// ========================================
// APLICAR FILTROS
// ========================================

function aplicarFiltros() {
    const params = new URLSearchParams(window.location.search);
    
    // Checkbox solo disponibles
    const checkboxDisponibles = document.getElementById('soloDisponibles');
    if (checkboxDisponibles) {
        if (checkboxDisponibles.checked) {
            params.set('soloDisponibles', 'true');
        } else {
            params.delete('soloDisponibles');
        }
    }
    
    // Categoría
    const selectCategoria = document.getElementById('categoriaFiltro');
    if (selectCategoria && selectCategoria.value) {
        params.set('categoriaId', selectCategoria.value);
    } else {
        params.delete('categoriaId');
    }
    
    // Búsqueda
    const inputBusqueda = document.getElementById('textoBusqueda');
    if (inputBusqueda && inputBusqueda.value.trim()) {
        params.set('textoBusqueda', inputBusqueda.value.trim());
    } else {
        params.delete('textoBusqueda');
    }
    
    // Mantener página 1 al aplicar filtros
    params.set('pageNumber', '1');
    
    // Recargar página con nuevos parámetros
    const newUrl = `${window.location.pathname}?${params.toString()}`;
    console.log('?? Aplicando filtros:', newUrl);
    window.location.href = newUrl;
}

// ========================================
// LIMPIAR FILTROS
// ========================================

function limpiarFiltros() {
    console.log('?? Limpiando filtros...');
    window.location.href = window.location.pathname;
}

// ========================================
// EXPORTAR FUNCIONES
// ========================================

window.aplicarFiltros = aplicarFiltros;
window.limpiarFiltros = limpiarFiltros;

console.log('? Catálogo inicializado');
