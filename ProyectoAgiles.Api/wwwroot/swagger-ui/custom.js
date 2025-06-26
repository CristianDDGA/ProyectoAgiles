// JavaScript personalizado para Swagger UI - ProyectoAgiles API

(function() {
    'use strict';
    
    // Esperar a que Swagger UI esté completamente cargado
    window.addEventListener('load', function() {
        initializeCustomFeatures();
    });
    
    function initializeCustomFeatures() {
        // Agregar información adicional al header
        addCustomHeader();
        
        // Mejorar la experiencia de autorización
        enhanceAuthorizationExperience();
        
        // Agregar tooltips informativos
        addTooltips();
        
        // Personalizar mensajes de respuesta
        customizeResponseMessages();
        
        // Agregar estadísticas de la API
        addApiStatistics();
    }
    
    function addCustomHeader() {
        const topbar = document.querySelector('.topbar');
        if (topbar && !document.querySelector('.custom-header-info')) {
            const headerInfo = document.createElement('div');
            headerInfo.className = 'custom-header-info';
            headerInfo.innerHTML = `
                <div style="
                    display: flex; 
                    align-items: center; 
                    justify-content: space-between; 
                    padding: 10px 20px; 
                    background: rgba(255, 255, 255, 0.1);
                    backdrop-filter: blur(10px);
                    border-radius: 8px;
                    margin: 10px 20px;
                ">
                    <div style="color: white;">
                        <strong>🌐 Entorno:</strong> ${getEnvironment()}
                    </div>
                    <div style="color: white;">
                        <strong>📅 Última actualización:</strong> ${getCurrentDate()}
                    </div>
                    <div style="color: white;">
                        <strong>⚡ Estado:</strong> <span style="color: #10b981;">Operativo</span>
                    </div>
                </div>
            `;
            topbar.appendChild(headerInfo);
        }
    }
    
    function enhanceAuthorizationExperience() {
        // Observar cambios en el botón de autorización
        const observer = new MutationObserver(function(mutations) {
            mutations.forEach(function(mutation) {
                if (mutation.type === 'childList') {
                    const authButton = document.querySelector('.btn.authorize');
                    if (authButton && !authButton.classList.contains('enhanced')) {
                        authButton.classList.add('enhanced');
                        authButton.addEventListener('click', function() {
                            setTimeout(showAuthInstructions, 500);
                        });
                    }
                }
            });
        });
        
        observer.observe(document.body, {
            childList: true,
            subtree: true
        });
    }
    
    function showAuthInstructions() {
        const authModal = document.querySelector('.auth-container');
        if (authModal && !document.querySelector('.auth-instructions')) {
            const instructions = document.createElement('div');
            instructions.className = 'auth-instructions';
            instructions.innerHTML = `
                <div style="
                    background: #e7f3ff; 
                    border: 1px solid #3b82f6; 
                    border-radius: 8px; 
                    padding: 15px; 
                    margin-bottom: 15px;
                ">
                    <h4 style="margin: 0 0 10px 0; color: #1e3a8a;">
                        🔐 Instrucciones de Autenticación
                    </h4>
                    <ol style="margin: 0; padding-left: 20px; color: #475569;">
                        <li>Usa el endpoint <code>/api/Auth/login</code> para obtener tu token</li>
                        <li>Copia el token de la respuesta JSON</li>
                        <li>Pega el token en el campo de abajo (sin "Bearer")</li>
                        <li>Haz clic en "Authorize" y luego "Close"</li>
                    </ol>
                    <p style="margin: 10px 0 0 0; font-size: 14px; color: #64748b;">
                        💡 <strong>Tip:</strong> El token expira en 24 horas
                    </p>
                </div>
            `;
            authModal.insertBefore(instructions, authModal.firstChild);
        }
    }
    
    function addTooltips() {
        // Agregar tooltips a los métodos HTTP
        const httpMethods = {
            'GET': 'Obtener datos del servidor',
            'POST': 'Crear nuevos recursos',
            'PUT': 'Actualizar recursos existentes',
            'DELETE': 'Eliminar recursos',
            'PATCH': 'Actualización parcial de recursos'
        };
        
        Object.keys(httpMethods).forEach(method => {
            const elements = document.querySelectorAll(`.opblock-${method.toLowerCase()}`);
            elements.forEach(el => {
                if (!el.hasAttribute('title')) {
                    el.setAttribute('title', httpMethods[method]);
                }
            });
        });
    }
    
    function customizeResponseMessages() {
        // Personalizar mensajes de códigos de estado HTTP
        const statusMessages = {
            '200': '✅ Éxito - Operación completada correctamente',
            '201': '✅ Creado - Recurso creado exitosamente',
            '400': '❌ Solicitud incorrecta - Verifique los datos enviados',
            '401': '🔒 No autorizado - Token requerido o inválido',
            '403': '🚫 Prohibido - Sin permisos para esta operación',
            '404': '🔍 No encontrado - Recurso no existe',
            '500': '💥 Error del servidor - Contacte al administrador'
        };
        
        setTimeout(() => {
            Object.keys(statusMessages).forEach(code => {
                const statusElements = document.querySelectorAll(`[data-code="${code}"]`);
                statusElements.forEach(el => {
                    if (!el.classList.contains('customized')) {
                        el.classList.add('customized');
                        el.setAttribute('title', statusMessages[code]);
                    }
                });
            });
        }, 1000);
    }
    
    function addApiStatistics() {
        setTimeout(() => {
            const infoSection = document.querySelector('.information-container');
            if (infoSection && !document.querySelector('.api-stats')) {
                const stats = document.createElement('div');
                stats.className = 'api-stats';
                stats.innerHTML = `
                    <div style="
                        background: linear-gradient(135deg, #f8fafc 0%, #ffffff 100%);
                        border: 1px solid #e2e8f0;
                        border-radius: 12px;
                        padding: 20px;
                        margin: 20px 0;
                        box-shadow: 0 4px 15px rgba(0, 0, 0, 0.1);
                    ">
                        <h3 style="color: #1e3a8a; margin-bottom: 15px;">📊 Estadísticas de la API</h3>
                        <div style="display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: 15px;">
                            <div style="text-align: center; padding: 15px; background: #e7f3ff; border-radius: 8px;">
                                <div style="font-size: 24px; font-weight: bold; color: #1e3a8a;">${countEndpoints()}</div>
                                <div style="color: #64748b;">Endpoints Disponibles</div>
                            </div>
                            <div style="text-align: center; padding: 15px; background: #f0fdf4; border-radius: 8px;">
                                <div style="font-size: 24px; font-weight: bold; color: #059669;">${countControllers()}</div>
                                <div style="color: #64748b;">Controladores</div>
                            </div>
                            <div style="text-align: center; padding: 15px; background: #fef3c7; border-radius: 8px;">
                                <div style="font-size: 24px; font-weight: bold; color: #d97706;">${countModels()}</div>
                                <div style="color: #64748b;">Modelos de Datos</div>
                            </div>
                        </div>
                    </div>
                `;
                infoSection.appendChild(stats);
            }
        }, 2000);
    }
    
    // Funciones auxiliares
    function getEnvironment() {
        return window.location.hostname === 'localhost' ? '🔧 Desarrollo' : '🚀 Producción';
    }
    
    function getCurrentDate() {
        return new Date().toLocaleDateString('es-ES', {
            year: 'numeric',
            month: 'long',
            day: 'numeric'
        });
    }
    
    function countEndpoints() {
        return document.querySelectorAll('.opblock').length || '12+';
    }
    
    function countControllers() {
        return document.querySelectorAll('.opblock-tag-section').length || '9';
    }
    
    function countModels() {
        return document.querySelectorAll('.model-container').length || '25+';
    }
    
    // Mejorar la experiencia del usuario con efectos visuales
    function addVisualEnhancements() {
        const style = document.createElement('style');
        style.textContent = `
            .opblock:hover {
                transform: translateY(-2px);
                transition: transform 0.2s ease;
            }
            
            .btn:hover {
                transform: scale(1.05);
                transition: transform 0.2s ease;
            }
            
            .auth-wrapper .authorize:hover {
                animation: pulse 1s infinite;
            }
            
            @keyframes pulse {
                0% { box-shadow: 0 0 0 0 rgba(16, 185, 129, 0.7); }
                70% { box-shadow: 0 0 0 10px rgba(16, 185, 129, 0); }
                100% { box-shadow: 0 0 0 0 rgba(16, 185, 129, 0); }
            }
        `;
        document.head.appendChild(style);
    }
    
    // Inicializar mejoras visuales
    addVisualEnhancements();
    
    // Agregar mensaje de bienvenida en la consola
    console.log(`
    🎓 ProyectoAgiles API - Sistema de Escalafón Docente
    ====================================================
    
    ✨ Swagger UI mejorado y personalizado
    🔧 Desarrollado para la Universidad Técnica de Ambato
    📚 Documentación interactiva disponible
    
    🚀 ¡Explora la API y mejora la gestión universitaria!
    `);
    
})();
