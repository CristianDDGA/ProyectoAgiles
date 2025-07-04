// Sistema de notificaciones toast profesionales para UTA
console.log('notifications.js cargado correctamente');

window.toastNotifications = {
    show: function(type, title, message, duration = 5000) {
        // Crear el contenedor si no existe
        let container = document.querySelector('.toast-container');
        if (!container) {
            container = document.createElement('div');
            container.className = 'toast-container';
            document.body.appendChild(container);
        }

        // Crear el toast
        const toast = document.createElement('div');
        toast.className = `toast-notification toast-${type}`;
        
        // Determinar el icono según el tipo
        let icon = '';
        switch(type) {
            case 'success':
                icon = 'fas fa-check-circle';
                break;
            case 'error':
                icon = 'fas fa-exclamation-circle';
                break;
            case 'info':
                icon = 'fas fa-info-circle';
                break;
            case 'warning':
                icon = 'fas fa-exclamation-triangle';
                break;
            default:
                icon = 'fas fa-info-circle';
        }

        toast.innerHTML = `
            <div class="toast-icon">
                <i class="${icon}"></i>
            </div>
            <div class="toast-content">
                <div class="toast-title">${title}</div>
                <div class="toast-message">${message}</div>
            </div>
            <button class="toast-close" onclick="toastNotifications.remove(this.parentElement)">
                <i class="fas fa-times"></i>
            </button>
        `;

        // Agregar al contenedor
        container.appendChild(toast);

        // Auto-remove después del tiempo especificado
        setTimeout(() => {
            this.remove(toast);
        }, duration);

        return toast;
    },

    remove: function(toast) {
        if (toast && toast.parentElement) {
            toast.classList.add('removing');
            setTimeout(() => {
                if (toast.parentElement) {
                    toast.parentElement.removeChild(toast);
                }
            }, 300);
        }
    },

    success: function(title, message, duration = 5000) {
        return this.show('success', title, message, duration);
    },

    error: function(title, message, duration = 7000) {
        return this.show('error', title, message, duration);
    },

    info: function(title, message, duration = 5000) {
        return this.show('info', title, message, duration);
    },    warning: function(title, message, duration = 6000) {
        return this.show('warning', title, message, duration);
    },

    // Función de prueba para verificar el contraste
    test: function() {
        console.log('Probando notificaciones con mejor contraste...');
        this.success('¡Éxito!', 'Esta es una notificación de éxito con mejor contraste y legibilidad.');
        setTimeout(() => {
            this.error('Error', 'Esta es una notificación de error con texto más legible.');
        }, 1000);
        setTimeout(() => {
            this.info('Información', 'Esta es una notificación informativa con fondo mejorado.');
        }, 2000);
        setTimeout(() => {
            this.warning('Advertencia', 'Esta es una notificación de advertencia con mejor visibilidad.');
        }, 3000);
    },

    // Función para mostrar confirmación con modal
    confirm: function(title, message, confirmText = 'Continuar', cancelText = 'Cancelar') {
        return new Promise((resolve) => {
            // Crear el modal de confirmación
            const modal = document.createElement('div');
            modal.className = 'confirmation-modal-overlay';
            modal.innerHTML = `
                <div class="confirmation-modal">
                    <div class="confirmation-header">
                        <div class="confirmation-icon">
                            <i class="fas fa-question-circle"></i>
                        </div>
                        <h4 class="confirmation-title">${title}</h4>
                    </div>
                    <div class="confirmation-body">
                        <div class="confirmation-message">${message}</div>
                    </div>
                    <div class="confirmation-actions">
                        <button class="btn-cancel" onclick="toastNotifications.closeConfirm(this, false)">
                            <i class="fas fa-times"></i>
                            ${cancelText}
                        </button>
                        <button class="btn-confirm" onclick="toastNotifications.closeConfirm(this, true)">
                            <i class="fas fa-check"></i>
                            ${confirmText}
                        </button>
                    </div>
                </div>
            `;

            // Agregar al body
            document.body.appendChild(modal);
            
            // Guardar el resolver para usarlo en closeConfirm
            modal._resolve = resolve;

            // Mostrar con animación
            setTimeout(() => {
                modal.classList.add('show');
            }, 10);

            // Cerrar con Escape
            const handleEscape = (e) => {
                if (e.key === 'Escape') {
                    document.removeEventListener('keydown', handleEscape);
                    this.closeConfirm(modal.querySelector('.btn-cancel'), false);
                }
            };
            document.addEventListener('keydown', handleEscape);
        });
    },

    closeConfirm: function(element, result) {
        const modal = element.closest('.confirmation-modal-overlay');
        if (modal && modal._resolve) {
            modal.classList.remove('show');
            setTimeout(() => {
                modal._resolve(result);
                document.body.removeChild(modal);
            }, 300);
        }
    },
};

console.log('toastNotifications inicializado:', window.toastNotifications);
