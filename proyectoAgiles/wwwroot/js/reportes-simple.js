// Funciones para generar y descargar PDFs - Versión simplificada
window.descargarPDF = function (datosJson, nombreArchivo) {
    try {
        console.log('Iniciando descarga PDF...');
        console.log('Datos recibidos:', datosJson);
        console.log('Nombre archivo:', nombreArchivo);
        
        const datos = JSON.parse(datosJson);
        console.log('Datos parseados:', datos);
        
        // Crear el contenido HTML para el PDF
        const contenidoHTML = generarHTMLFicha(datos);
        console.log('HTML generado para PDF');
        
        // Crear ventana nueva para imprimir
        const ventanaImpresion = window.open('', '_blank', 'width=800,height=900,scrollbars=yes');
        
        if (!ventanaImpresion) {
            console.error('No se pudo abrir ventana de impresión');
            alert('Por favor, permite ventanas emergentes para descargar el PDF');
            return;
        }
        
        console.log('Ventana de impresión abierta');
        
        ventanaImpresion.document.write(contenidoHTML);
        ventanaImpresion.document.close();
        
        console.log('Contenido escrito en ventana de impresión');
        
        // Esperar a que se cargue y luego imprimir
        ventanaImpresion.addEventListener('load', function() {
            console.log('Ventana cargada, iniciando impresión...');
            setTimeout(() => {
                ventanaImpresion.print();
                console.log('Comando de impresión ejecutado');
            }, 1000);
        });
        
        // Fallback: intentar imprimir después de un tiempo
        setTimeout(() => {
            if (ventanaImpresion && !ventanaImpresion.closed) {
                console.log('Fallback: intentando imprimir...');
                ventanaImpresion.print();
            }
        }, 2000);
        
        console.log('Descarga PDF iniciada exitosamente');
        
    } catch (error) {
        console.error('Error detallado al generar PDF:', error);
        alert('Error al generar el PDF: ' + error.message);
    }
};

window.mostrarVistaPrevia = function (datosJson) {
    try {
        console.log('Mostrando vista previa...');
        console.log('Datos recibidos:', datosJson);
        
        const datos = JSON.parse(datosJson);
        console.log('Datos parseados:', datos);
        
        // Remover modal existente si existe
        const modalExistente = document.getElementById('modal-vista-previa');
        if (modalExistente) {
            modalExistente.remove();
        }
        
        // Crear modal simple
        const modalHTML = `
            <div id="modal-vista-previa" style="position: fixed; top: 0; left: 0; width: 100%; height: 100%; background: rgba(0,0,0,0.7); z-index: 9999; display: flex; align-items: center; justify-content: center;">
                <div style="background: white; border-radius: 10px; max-width: 90%; max-height: 90%; overflow: auto; position: relative; box-shadow: 0 4px 20px rgba(0,0,0,0.3);">
                    <div style="padding: 20px; border-bottom: 1px solid #ddd; display: flex; justify-content: space-between; align-items: center; background: #f8f9fa; border-radius: 10px 10px 0 0;">
                        <h3 style="margin: 0; color: #dc3545;">Vista Previa - ${datos.docente.nombre}</h3>
                        <div>
                            <button onclick="descargarPDFDesdePrevia()" 
                                    style="background: #dc3545; color: white; border: none; padding: 8px 16px; border-radius: 5px; margin-right: 10px; cursor: pointer; font-weight: bold;">
                                📄 Descargar PDF
                            </button>
                            <button onclick="cerrarVistaPrevia()" 
                                    style="background: #6c757d; color: white; border: none; padding: 8px 16px; border-radius: 5px; cursor: pointer; font-weight: bold;">
                                ✕ Cerrar
                            </button>
                        </div>
                    </div>
                    <div style="padding: 20px; max-height: 60vh; overflow-y: auto;">
                        ${generarHTMLVistaPrevia(datos)}
                    </div>
                </div>
            </div>
        `;
        
        // Agregar modal al body
        document.body.insertAdjacentHTML('beforeend', modalHTML);
        console.log('Modal agregado al DOM');
        
        // Guardar los datos en una variable global para acceso posterior
        window.datosVistaPreviaActual = datos;
        console.log('Datos guardados globalmente');
        
        // Agregar event listener para cerrar con clic fuera del modal
        const modal = document.getElementById('modal-vista-previa');
        if (modal) {
            console.log('Modal encontrado, agregando event listeners');
            modal.addEventListener('click', function(e) {
                if (e.target === modal) {
                    cerrarVistaPrevia();
                }
            });
        } else {
            console.error('Modal no encontrado después de crearlo');
        }
        
        // Agregar event listener para cerrar con Escape
        document.addEventListener('keydown', function(e) {
            if (e.key === 'Escape') {
                cerrarVistaPrevia();
            }
        });
        
        console.log('Vista previa mostrada exitosamente');
        
    } catch (error) {
        console.error('Error detallado al mostrar vista previa:', error);
        alert('Error al mostrar la vista previa: ' + error.message);
    }
};

window.cerrarVistaPrevia = function() {
    console.log('Cerrando vista previa...');
    const modal = document.getElementById('modal-vista-previa');
    if (modal) {
        modal.remove();
        console.log('Modal cerrado exitosamente');
    } else {
        console.log('No se encontró modal para cerrar');
    }
    
    // Limpiar datos globales
    window.datosVistaPreviaActual = null;
};

window.descargarPDFDesdePrevia = function() {
    try {
        if (window.datosVistaPreviaActual) {
            const nombreArchivo = `Ficha_${window.datosVistaPreviaActual.docente.nombre.replace(/\s+/g, '_')}.pdf`;
            descargarPDF(JSON.stringify(window.datosVistaPreviaActual), nombreArchivo);
        }
        cerrarVistaPrevia();
    } catch (error) {
        console.error('Error al descargar PDF desde vista previa:', error);
        alert('Error al descargar el PDF: ' + error.message);
    }
};

function generarHTMLFicha(datos) {
    return `
        <!DOCTYPE html>
        <html lang="es">
        <head>
            <meta charset="UTF-8">
            <meta name="viewport" content="width=device-width, initial-scale=1.0">
            <title>Ficha de Ascenso Docente - ${datos.docente.nombre}</title>
            <style>
                body { 
                    font-family: 'Arial', sans-serif; 
                    margin: 20px; 
                    line-height: 1.6;
                    color: #333;
                }
                .header { 
                    text-align: center; 
                    margin-bottom: 30px; 
                    border-bottom: 3px solid #dc3545;
                    padding-bottom: 20px;
                }
                .header h1 { 
                    color: #dc3545; 
                    margin: 0;
                    font-size: 28px;
                }
                .header h2 { 
                    color: #6c757d; 
                    margin: 5px 0; 
                    font-weight: normal;
                    font-size: 18px;
                }
                .seccion { 
                    margin-bottom: 25px; 
                    padding: 20px;
                    border: 1px solid #ddd;
                    border-radius: 8px;
                    background: #f8f9fa;
                }
                .seccion h3 { 
                    color: #dc3545; 
                    border-bottom: 2px solid #dc3545;
                    padding-bottom: 8px;
                    margin-top: 0;
                    font-size: 20px;
                }
                .campo { 
                    margin-bottom: 12px; 
                    display: flex;
                    align-items: flex-start;
                }
                .campo strong { 
                    min-width: 180px; 
                    color: #495057;
                    font-weight: 600;
                }
                .campo span {
                    flex: 1;
                    padding-left: 15px;
                }
                .status-badge {
                    display: inline-block;
                    padding: 6px 12px;
                    border-radius: 20px;
                    font-size: 12px;
                    font-weight: bold;
                    text-transform: uppercase;
                }
                .status-aprobada { background-color: #28a745; color: white; }
                .status-pendiente { background-color: #ffc107; color: #212529; }
                .status-rechazada { background-color: #dc3545; color: white; }
                .fecha-generacion {
                    text-align: center;
                    font-size: 12px;
                    color: #6c757d;
                    margin-top: 40px;
                    padding-top: 20px;
                    border-top: 1px solid #ddd;
                }
                @media print {
                    body { margin: 0; }
                    .no-print { display: none !important; }
                }
            </style>
        </head>
        <body>
            <div class="header">
                <h1>FICHA DE ASCENSO DOCENTE</h1>
                <h2>Universidad Técnica de Ambato</h2>
            </div>
            
            <div class="seccion">
                <h3>📋 Información Personal</h3>
                <div class="campo">
                    <strong>Nombre Completo:</strong>
                    <span>${datos.docente.nombre}</span>
                </div>
                <div class="campo">
                    <strong>Cédula de Identidad:</strong>
                    <span>${datos.docente.cedula}</span>
                </div>
                <div class="campo">
                    <strong>Correo Electrónico:</strong>
                    <span>${datos.docente.email}</span>
                </div>
                <div class="campo">
                    <strong>Teléfono:</strong>
                    <span>${datos.docente.telefono}</span>
                </div>
                <div class="campo">
                    <strong>Facultad:</strong>
                    <span>${datos.docente.facultad}</span>
                </div>
                <div class="campo">
                    <strong>Carrera:</strong>
                    <span>${datos.docente.carrera}</span>
                </div>
            </div>
            
            <div class="seccion">
                <h3>🎓 Información de Escalafón</h3>
                <div class="campo">
                    <strong>Nivel Actual:</strong>
                    <span>${datos.escalafon.nivelActual}</span>
                </div>
                <div class="campo">
                    <strong>Nivel Solicitado:</strong>
                    <span>${datos.escalafon.nivelSolicitado}</span>
                </div>
                <div class="campo">
                    <strong>Fecha de Solicitud:</strong>
                    <span>${datos.escalafon.fechaSolicitud}</span>
                </div>
                <div class="campo">
                    <strong>Fecha de Aprobación:</strong>
                    <span>${datos.escalafon.fechaAprobacion}</span>
                </div>
                <div class="campo">
                    <strong>Años de Experiencia:</strong>
                    <span>${datos.escalafon.anosExperiencia} años</span>
                </div>
                <div class="campo">
                    <strong>Estado:</strong>
                    <span>
                        <span class="status-badge status-${datos.escalafon.status.toLowerCase()}">
                            ${datos.escalafon.status}
                        </span>
                    </span>
                </div>
            </div>
            
            <div class="seccion">
                <h3>📚 Información Académica</h3>
                <div class="campo">
                    <strong>Títulos Académicos:</strong>
                    <span>${datos.detalles.titulos}</span>
                </div>
                <div class="campo">
                    <strong>Publicaciones:</strong>
                    <span>${datos.detalles.publicaciones}</span>
                </div>
                <div class="campo">
                    <strong>Proyectos de Investigación:</strong>
                    <span>${datos.detalles.proyectosInvestigacion}</span>
                </div>
                <div class="campo">
                    <strong>Capacitaciones:</strong>
                    <span>${datos.detalles.capacitaciones}</span>
                </div>
                <div class="campo">
                    <strong>Observaciones:</strong>
                    <span>${datos.detalles.observaciones}</span>
                </div>
            </div>
            
            <div class="fecha-generacion">
                <strong>Documento generado el ${datos.fecha}</strong><br>
                Sistema de Gestión Académica - Universidad Técnica de Ambato
            </div>
        </body>
        </html>
    `;
}

function generarHTMLVistaPrevia(datos) {
    return `
        <div style="max-width: 800px; font-family: Arial, sans-serif;">
            <div style="text-align: center; margin-bottom: 20px; padding-bottom: 15px; border-bottom: 2px solid #dc3545;">
                <h2 style="color: #dc3545; margin: 0;">FICHA DE ASCENSO DOCENTE</h2>
                <p style="color: #6c757d; margin: 5px 0; font-size: 14px;">Universidad Técnica de Ambato</p>
            </div>
            
            <div style="margin-bottom: 20px; padding: 15px; background: #f8f9fa; border-radius: 8px; border-left: 4px solid #dc3545;">
                <h4 style="color: #dc3545; margin-top: 0; margin-bottom: 15px;">📋 Información Personal</h4>
                <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 10px; font-size: 14px;">
                    <p style="margin: 5px 0;"><strong>Nombre:</strong> ${datos.docente.nombre}</p>
                    <p style="margin: 5px 0;"><strong>Cédula:</strong> ${datos.docente.cedula}</p>
                    <p style="margin: 5px 0;"><strong>Email:</strong> ${datos.docente.email}</p>
                    <p style="margin: 5px 0;"><strong>Teléfono:</strong> ${datos.docente.telefono}</p>
                    <p style="margin: 5px 0;"><strong>Facultad:</strong> ${datos.docente.facultad}</p>
                    <p style="margin: 5px 0;"><strong>Carrera:</strong> ${datos.docente.carrera}</p>
                </div>
            </div>
            
            <div style="margin-bottom: 20px; padding: 15px; background: #f8f9fa; border-radius: 8px; border-left: 4px solid #dc3545;">
                <h4 style="color: #dc3545; margin-top: 0; margin-bottom: 15px;">🎓 Información de Escalafón</h4>
                <div style="font-size: 14px;">
                    <p style="margin: 8px 0;"><strong>Promoción:</strong> ${datos.escalafon.nivelActual} → ${datos.escalafon.nivelSolicitado}</p>
                    <p style="margin: 8px 0;"><strong>Experiencia:</strong> ${datos.escalafon.anosExperiencia} años</p>
                    <p style="margin: 8px 0;"><strong>Estado:</strong> 
                        <span style="background: ${datos.escalafon.status === 'Aprobada' ? '#28a745' : datos.escalafon.status === 'Pendiente' ? '#ffc107' : '#dc3545'}; 
                                     color: ${datos.escalafon.status === 'Pendiente' ? '#000' : '#fff'}; 
                                     padding: 3px 8px; border-radius: 12px; font-size: 12px; font-weight: bold;">
                            ${datos.escalafon.status}
                        </span>
                    </p>
                    <p style="margin: 8px 0;"><strong>Fecha de Solicitud:</strong> ${datos.escalafon.fechaSolicitud}</p>
                    <p style="margin: 8px 0;"><strong>Fecha de Aprobación:</strong> ${datos.escalafon.fechaAprobacion}</p>
                </div>
            </div>
            
            <div style="margin-bottom: 20px; padding: 15px; background: #f8f9fa; border-radius: 8px; border-left: 4px solid #dc3545;">
                <h4 style="color: #dc3545; margin-top: 0; margin-bottom: 15px;">📚 Información Académica</h4>
                <div style="font-size: 14px;">
                    <p style="margin: 8px 0;"><strong>Títulos:</strong> ${datos.detalles.titulos}</p>
                    <p style="margin: 8px 0;"><strong>Publicaciones:</strong> ${datos.detalles.publicaciones}</p>
                    <p style="margin: 8px 0;"><strong>Proyectos de Investigación:</strong> ${datos.detalles.proyectosInvestigacion}</p>
                    <p style="margin: 8px 0;"><strong>Capacitaciones:</strong> ${datos.detalles.capacitaciones}</p>
                    <p style="margin: 8px 0;"><strong>Observaciones:</strong> ${datos.detalles.observaciones}</p>
                </div>
            </div>
            
            <div style="text-align: center; margin-top: 30px; padding-top: 20px; border-top: 1px solid #ddd; font-size: 12px; color: #6c757d;">
                <strong>Documento generado el ${datos.fecha}</strong><br>
                Sistema de Gestión Académica - Universidad Técnica de Ambato
            </div>
        </div>
    `;
}
