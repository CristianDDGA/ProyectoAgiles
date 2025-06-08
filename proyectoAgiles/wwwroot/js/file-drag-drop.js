// Funcionalidad de drag and drop mejorada para archivos
// Este archivo complementa el JavaScript inline en Register.razor

// Función auxiliar para validar archivos
window.validateDroppedFile = function(file) {
    const allowedTypes = ['application/pdf', 'image/jpeg', 'image/jpg', 'image/png', 'image/gif', 'image/bmp'];
    const allowedExtensions = ['.pdf', '.jpg', '.jpeg', '.png', '.gif', '.bmp'];
    
    const fileExtension = file.name.toLowerCase().substring(file.name.lastIndexOf('.'));
    const isValidType = allowedTypes.includes(file.type.toLowerCase()) || allowedExtensions.includes(fileExtension);
    
    if (!isValidType) {
        return { valid: false, message: 'Tipo de archivo no válido. Solo se permiten: PDF, JPG, PNG, GIF, BMP' };
    }
    
    if (file.size > 10 * 1024 * 1024) {
        return { valid: false, message: 'El archivo es demasiado grande. Tamaño máximo: 10MB' };
    }

    if (file.size === 0) {
        return { valid: false, message: 'El archivo está vacío. Por favor selecciona un archivo válido.' };
    }
    
    return { valid: true, message: 'Archivo válido' };
};

// Función para mostrar feedback visual
window.showFileDropFeedback = function(type, message) {
    const dropZone = document.getElementById('dropZone');
    if (!dropZone) return;    if (type === 'success') {
        dropZone.style.borderColor = '#722f37';
        dropZone.style.background = 'rgba(114, 47, 55, 0.1)';
        
        setTimeout(() => {
            dropZone.style.borderColor = '';
            dropZone.style.background = '';
        }, 2000);
    } else if (type === 'error') {
        dropZone.style.borderColor = '#dc3545';
        dropZone.style.background = 'rgba(220, 53, 69, 0.1)';
        
        setTimeout(() => {
            dropZone.style.borderColor = '';
            dropZone.style.background = '';
        }, 3000);
    }
};

// Función para transferir archivo al input file de Blazor
window.transferFileToInput = function(file) {
    const fileInput = document.querySelector('.file-input-hidden');
    if (!fileInput) {
        console.error('No se encontró el input file');
        return false;
    }
    
    try {
        // Crear un nuevo DataTransfer para asignar el archivo
        const dataTransfer = new DataTransfer();
        dataTransfer.items.add(file);
        fileInput.files = dataTransfer.files;
        
        // Disparar el evento change
        const event = new Event('change', { bubbles: true });
        fileInput.dispatchEvent(event);
        
        return true;
    } catch (error) {
        console.error('Error al transferir archivo:', error);
        return false;
    }
};

// Función para limpiar el estado visual del drag and drop
window.clearDragState = function() {
    const dropZone = document.getElementById('dropZone');
    if (dropZone) {
        dropZone.classList.remove('drag-over', 'dragging');
        const overlay = dropZone.querySelector('.drag-overlay');
        if (overlay) {
            overlay.classList.remove('visible');
        }
    }
};

// Función para mostrar mensajes de error integrados
window.showFileError = function(message) {
    let errorContainer = document.querySelector('.file-error-message');
    if (!errorContainer) {
        const dropZone = document.getElementById('dropZone');
        if (dropZone && dropZone.parentNode) {
            errorContainer = document.createElement('div');
            errorContainer.className = 'file-error-message';
            errorContainer.style.display = 'flex';
            errorContainer.style.alignItems = 'center';
            errorContainer.style.gap = '8px';
            errorContainer.style.color = '#dc3545';
            errorContainer.style.fontSize = '14px';
            errorContainer.style.marginTop = '8px';
            errorContainer.style.padding = '8px 12px';
            errorContainer.style.border = '1px solid #dc3545';
            errorContainer.style.borderRadius = '4px';
            errorContainer.style.backgroundColor = 'rgba(220, 53, 69, 0.1)';
            dropZone.parentNode.appendChild(errorContainer);
        }
    }
    
    if (errorContainer) {
        errorContainer.innerHTML = `<i class="fas fa-exclamation-triangle"></i> ${message}`;
        errorContainer.style.display = 'flex';
        
        // Auto-ocultar después de 5 segundos
        setTimeout(() => {
            if (errorContainer) {
                errorContainer.style.display = 'none';
            }
        }, 5000);
    }
};

// Función para ocultar mensajes de error
window.hideFileError = function() {
    const errorContainer = document.querySelector('.file-error-message');
    if (errorContainer) {
        errorContainer.style.display = 'none';
    }
};

// Debug: Verificar que el archivo se cargó correctamente
console.log('file-drag-drop.js cargado correctamente');

// Inicialización cuando el DOM esté listo
document.addEventListener('DOMContentLoaded', function() {
    console.log('DOM listo - funciones de drag and drop disponibles');
});
