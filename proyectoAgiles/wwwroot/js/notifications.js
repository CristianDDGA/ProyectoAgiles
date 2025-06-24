// Sistema de notificaciones toast profesionales para UTA
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
    },

    warning: function(title, message, duration = 6000) {
        return this.show('warning', title, message, duration);
    }
};
