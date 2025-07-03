// Funciones para gestión de apelaciones
window.apelacionesManager = {
    // Mostrar PDF de apelación en modal
    mostrarPdfApelacion: function(pdfDataUrl, nombreArchivo) {
        return new Promise((resolve, reject) => {
            try {
                // Crear modal dinámico
                const modalId = 'modal-pdf-apelacion';
                
                // Eliminar modal existente si existe
                const existingModal = document.getElementById(modalId);
                if (existingModal) {
                    existingModal.remove();
                }
                
                // Crear nuevo modal
                const modalHtml = `
                    <div class="modal fade modal-pdf-apelacion" id="${modalId}" tabindex="-1" aria-labelledby="modalPdfApelacionLabel" aria-hidden="true">
                        <div class="modal-dialog modal-xl">
                            <div class="modal-content">
                                <div class="modal-header bg-warning text-dark">
                                    <h5 class="modal-title" id="modalPdfApelacionLabel">
                                        <i class="fas fa-file-pdf"></i> Documento de Apelación
                                    </h5>
                                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                                </div>
                                <div class="modal-body p-0">
                                    <div class="pdf-container" style="height: 70vh; overflow: auto;">
                                        <iframe src="${pdfDataUrl}" 
                                                type="application/pdf" 
                                                style="width: 100%; height: 100%; border: none;"
                                                title="${nombreArchivo}">
                                            <p>Su navegador no puede mostrar archivos PDF. 
                                               <a href="${pdfDataUrl}" target="_blank">Haga clic aquí para descargar el archivo</a>
                                            </p>
                                        </iframe>
                                    </div>
                                </div>
                                <div class="modal-footer">
                                    <button type="button" class="btn btn-primary" onclick="window.apelacionesManager.descargarPdf('${pdfDataUrl}', '${nombreArchivo}')">
                                        <i class="fas fa-download"></i> Descargar
                                    </button>
                                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">
                                        <i class="fas fa-times"></i> Cerrar
                                    </button>
                                </div>
                            </div>
                        </div>
                    </div>
                `;
                
                // Agregar modal al DOM
                document.body.insertAdjacentHTML('beforeend', modalHtml);
                
                // Mostrar modal
                const modal = new bootstrap.Modal(document.getElementById(modalId));
                modal.show();
                
                // Limpiar modal cuando se cierre
                document.getElementById(modalId).addEventListener('hidden.bs.modal', function () {
                    this.remove();
                });
                
                resolve(true);
            } catch (error) {
                console.error('Error al mostrar PDF de apelación:', error);
                reject(error);
            }
        });
    },
    
    // Descargar PDF
    descargarPdf: function(pdfDataUrl, nombreArchivo) {
        try {
            const link = document.createElement('a');
            link.href = pdfDataUrl;
            link.download = nombreArchivo;
            link.style.display = 'none';
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
            
            // Mostrar notificación de éxito
            if (window.toastNotifications) {
                window.toastNotifications.success('Descarga iniciada', 'El archivo se está descargando...');
            }
        } catch (error) {
            console.error('Error al descargar PDF:', error);
            if (window.toastNotifications) {
                window.toastNotifications.error('Error', 'No se pudo descargar el archivo');
            }
        }
    },
    
    // Mostrar modal de confirmación para aceptar apelación
    confirmarAceptarApelacion: function(solicitudId, nombreDocente) {
        return new Promise((resolve, reject) => {
            const modalId = 'modal-aceptar-apelacion';
            
            // Eliminar modal existente
            const existingModal = document.getElementById(modalId);
            if (existingModal) {
                existingModal.remove();
            }
            
            const modalHtml = `
                <div class="modal fade modal-apelacion" id="${modalId}" tabindex="-1" aria-labelledby="modalAceptarApelacionLabel" aria-hidden="true">
                    <div class="modal-dialog modal-lg">
                        <div class="modal-content">
                            <div class="modal-header bg-success text-white">
                                <h5 class="modal-title" id="modalAceptarApelacionLabel">
                                    <i class="fas fa-check-circle"></i> Aceptar Apelación
                                </h5>
                                <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Close"></button>
                            </div>
                            <div class="modal-body">
                                <div class="alert alert-info mb-4">
                                    <i class="fas fa-info-circle me-2"></i>
                                    <strong>Importante:</strong> Al aceptar esta apelación, la solicitud regresará al estado "Pendiente" 
                                    para ser reevaluada desde el inicio del proceso.
                                </div>
                                
                                <div class="mb-3">
                                    <label class="form-label fw-bold">Docente:</label>
                                    <p class="form-control-plaintext bg-light p-2 rounded">${nombreDocente}</p>
                                </div>
                                
                                <div class="mb-3">
                                    <label for="observacionesAceptacion${solicitudId}" class="form-label fw-bold">
                                        Observaciones sobre la aceptación:
                                    </label>
                                    <textarea class="form-control" id="observacionesAceptacion${solicitudId}" rows="4" 
                                              placeholder="Ingrese las observaciones sobre por qué se acepta la apelación...">
                                    </textarea>
                                    <small class="form-text text-muted">
                                        Estas observaciones quedarán registradas en el historial de la solicitud.
                                    </small>
                                </div>
                            </div>
                            <div class="modal-footer">
                                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">
                                    <i class="fas fa-times"></i> Cancelar
                                </button>
                                <button type="button" class="btn btn-success" onclick="window.apelacionesManager.procesarAceptacion(${solicitudId})">
                                    <i class="fas fa-check"></i> Confirmar Aceptación
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            `;
            
            document.body.insertAdjacentHTML('beforeend', modalHtml);
            const modal = new bootstrap.Modal(document.getElementById(modalId));
            modal.show();
            
            // Guardar el callback para procesar la aceptación
            window.apelacionesManager._callbackAceptacion = (data) => {
                resolve(data ? JSON.stringify(data) : null);
            };
            
            // Manejar cancelación
            document.getElementById(modalId).addEventListener('hidden.bs.modal', function () {
                if (window.apelacionesManager._callbackAceptacion) {
                    const callback = window.apelacionesManager._callbackAceptacion;
                    window.apelacionesManager._callbackAceptacion = null;
                    callback(null); // Enviar null en lugar de reject
                }
                this.remove();
            });
        });
    },
    
    // Procesar aceptación de apelación
    procesarAceptacion: function(solicitudId) {
        const observaciones = document.getElementById(`observacionesAceptacion${solicitudId}`).value;
        
        // Cerrar modal
        const modal = bootstrap.Modal.getInstance(document.getElementById('modal-aceptar-apelacion'));
        modal.hide();
        
        // Ejecutar callback con datos
        if (window.apelacionesManager._callbackAceptacion) {
            const callback = window.apelacionesManager._callbackAceptacion;
            window.apelacionesManager._callbackAceptacion = null;
            callback({
                solicitudId: solicitudId,
                observaciones: observaciones.trim(),
                accion: 'aceptar'
            });
        }
    },
    
    // Mostrar modal de confirmación para rechazar apelación
    confirmarRechazarApelacion: function(solicitudId, nombreDocente) {
        return new Promise((resolve, reject) => {
            const modalId = 'modal-rechazar-apelacion';
            
            // Eliminar modal existente
            const existingModal = document.getElementById(modalId);
            if (existingModal) {
                existingModal.remove();
            }
            
            const modalHtml = `
                <div class="modal fade modal-apelacion" id="${modalId}" tabindex="-1" aria-labelledby="modalRechazarApelacionLabel" aria-hidden="true">
                    <div class="modal-dialog modal-lg">
                        <div class="modal-content">
                            <div class="modal-header bg-danger text-white">
                                <h5 class="modal-title" id="modalRechazarApelacionLabel">
                                    <i class="fas fa-times-circle"></i> Rechazar Apelación
                                </h5>
                                <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Close"></button>
                            </div>
                            <div class="modal-body">
                                <div class="alert alert-warning mb-4">
                                    <i class="fas fa-exclamation-triangle me-2"></i>
                                    <strong>Atención:</strong> Al rechazar esta apelación, la solicitud quedará en estado 
                                    "Rechazado Definitivo" y el docente será notificado automáticamente por correo electrónico.
                                </div>
                                
                                <div class="mb-3">
                                    <label class="form-label fw-bold">Docente:</label>
                                    <p class="form-control-plaintext bg-light p-2 rounded">${nombreDocente}</p>
                                </div>
                                
                                <div class="mb-3">
                                    <label for="motivoRechazo${solicitudId}" class="form-label fw-bold">
                                        Motivo del rechazo <span class="text-danger">*</span>:
                                    </label>
                                    <textarea class="form-control" id="motivoRechazo${solicitudId}" rows="5" 
                                              placeholder="Ingrese el motivo detallado del rechazo de la apelación..." required>
                                    </textarea>
                                    <small class="form-text text-muted">
                                        <i class="fas fa-info-circle me-1"></i>
                                        Este motivo será enviado al docente por correo electrónico y quedará registrado permanentemente.
                                    </small>
                                </div>
                                
                                <div class="form-check mb-3">
                                    <input class="form-check-input" type="checkbox" id="confirmoRechazo${solicitudId}">
                                    <label class="form-check-label" for="confirmoRechazo${solicitudId}">
                                        <strong>Confirmo que he revisado la apelación y el motivo del rechazo es fundamentado</strong>
                                    </label>
                                </div>
                            </div>
                            <div class="modal-footer">
                                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">
                                    <i class="fas fa-times"></i> Cancelar
                                </button>
                                <button type="button" class="btn btn-danger" onclick="window.apelacionesManager.procesarRechazo(${solicitudId})">
                                    <i class="fas fa-ban"></i> Confirmar Rechazo
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            `;
            
            document.body.insertAdjacentHTML('beforeend', modalHtml);
            const modal = new bootstrap.Modal(document.getElementById(modalId));
            modal.show();
            
            // Guardar el callback para procesar el rechazo
            window.apelacionesManager._callbackRechazo = (data) => {
                resolve(data ? JSON.stringify(data) : null);
            };
            
            // Manejar cancelación
            document.getElementById(modalId).addEventListener('hidden.bs.modal', function () {
                if (window.apelacionesManager._callbackRechazo) {
                    const callback = window.apelacionesManager._callbackRechazo;
                    window.apelacionesManager._callbackRechazo = null;
                    callback(null); // Enviar null en lugar de reject
                }
                this.remove();
            });
        });
    },
    
    // Procesar rechazo de apelación
    procesarRechazo: function(solicitudId) {
        const motivo = document.getElementById(`motivoRechazo${solicitudId}`).value;
        const confirmado = document.getElementById(`confirmoRechazo${solicitudId}`).checked;
        
        // Validaciones
        if (!motivo.trim()) {
            if (window.toastNotifications) {
                window.toastNotifications.error('Error', 'Debe ingresar un motivo para el rechazo');
            }
            // Resaltar el campo
            document.getElementById(`motivoRechazo${solicitudId}`).classList.add('is-invalid');
            return;
        }
        
        if (!confirmado) {
            if (window.toastNotifications) {
                window.toastNotifications.error('Error', 'Debe confirmar que ha revisado la apelación');
            }
            // Resaltar el checkbox
            document.getElementById(`confirmoRechazo${solicitudId}`).classList.add('is-invalid');
            return;
        }
        
        if (motivo.trim().length < 10) {
            if (window.toastNotifications) {
                window.toastNotifications.error('Error', 'El motivo del rechazo debe tener al menos 10 caracteres');
            }
            document.getElementById(`motivoRechazo${solicitudId}`).classList.add('is-invalid');
            return;
        }
        
        // Cerrar modal
        const modal = bootstrap.Modal.getInstance(document.getElementById('modal-rechazar-apelacion'));
        modal.hide();
        
        // Ejecutar callback con datos
        if (window.apelacionesManager._callbackRechazo) {
            const callback = window.apelacionesManager._callbackRechazo;
            window.apelacionesManager._callbackRechazo = null;
            callback({
                solicitudId: solicitudId,
                motivo: motivo.trim(),
                accion: 'rechazar'
            });
        }
    }
};

// Función de compatibilidad para mostrar PDF (para usar desde Blazor)
window.mostrarPdfApelacion = function(pdfDataUrl, nombreArchivo) {
    return window.apelacionesManager.mostrarPdfApelacion(pdfDataUrl, nombreArchivo);
};

console.log('Módulo de apelaciones cargado correctamente');
