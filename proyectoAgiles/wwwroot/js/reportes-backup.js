// Funciones para generar y descargar PDFs
window.descargarPDF = function (datosJson, nombreArchivo) {
    try {
        const datos = JSON.parse(datosJson);
        
        // Crear el contenido HTML para el PDF
        const contenidoHTML = generarHTMLFicha(datos);
        
        // Crear un nuevo elemento para imprimir
        const ventanaImpresion = window.open('', '_blank', 'width=800,height=600');
        ventanaImpresion.document.write(contenidoHTML);
        ventanaImpresion.document.close();
        
        // Esperar un poco y luego imprimir
        setTimeout(() => {
            ventanaImpresion.print();
            // Cerrar la ventana después de imprimir
            ventanaImpresion.onafterprint = function() {
                ventanaImpresion.close();
            };
        }, 1000);
        
    } catch (error) {
        console.error('Error al generar PDF:', error);
        alert('Error al generar el PDF: ' + error.message);
    }
};

window.mostrarVistaPrevia = function (datosJson) {
    try {
        const datos = JSON.parse(datosJson);
        
        // Crear el contenido HTML para la vista previa
        const contenidoHTML = generarHTMLVistaPrevia(datos);
        
        // Crear modal para mostrar la vista previa
        const modalHTML = `
            <div id="modal-vista-previa" class="modal fade show" style="display: block; background-color: rgba(0,0,0,0.5);" tabindex="-1">
                <div class="modal-dialog modal-xl">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">Vista Previa - Ficha del Docente</h5>
                            <button type="button" class="btn-close" onclick="cerrarVistaPrevia()"></button>
                        </div>
                        <div class="modal-body">
                            <div class="text-end mb-3">
                                <button class="btn btn-primary" onclick="descargarPDFDesdePrevia('${datosJson.replace(/'/g, "\\'")}', 'Ficha_${datos.docente.nombre.replace(/\s+/g, '_')}_${datos.docente.cedula}.pdf')">
                                    <i class="fas fa-download"></i> Descargar PDF
                                </button>
                            </div>
                            <div id="contenido-ficha" style="background: white; padding: 20px; border: 1px solid #ddd; max-height: 500px; overflow-y: auto;">
                                ${contenidoHTML}
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" onclick="cerrarVistaPrevia()">Cerrar</button>
                        </div>
                    </div>
                </div>
            </div>
        `;
        
        // Remover modal anterior si existe
        const modalAnterior = document.getElementById('modal-vista-previa');
        if (modalAnterior) {
            modalAnterior.remove();
        }
        
        // Agregar modal al body
        document.body.insertAdjacentHTML('beforeend', modalHTML);
        
    } catch (error) {
        console.error('Error al mostrar vista previa:', error);
        alert('Error al mostrar la vista previa: ' + error.message);
    }
};

window.cerrarVistaPrevia = function() {
    const modal = document.getElementById('modal-vista-previa');
    if (modal) {
        modal.remove();
    }
};

window.descargarPDFDesdePrevia = function(datosJson, nombreArchivo) {
    descargarPDF(datosJson, nombreArchivo);
    cerrarVistaPrevia();
};

function generarHTMLVistaPrevia(datos) {
    return `
        <div class="ficha-docente">
            <div class="text-center mb-4">
                <h2 class="text-primary">${datos.titulo}</h2>
                <h4 class="text-muted">${datos.subtitulo}</h4>
            </div>
            
            <div class="row">
                <div class="col-md-6">
                    <div class="card mb-3">
                        <div class="card-header bg-primary text-white">
                            <h5 class="mb-0">Información Personal</h5>
                        </div>
                        <div class="card-body">
                            <p><strong>Nombre:</strong> ${datos.docente.nombre}</p>
                            <p><strong>Cédula:</strong> ${datos.docente.cedula}</p>
                            <p><strong>Email:</strong> ${datos.docente.email}</p>
                            <p><strong>Teléfono:</strong> ${datos.docente.telefono}</p>
                            <p><strong>Facultad:</strong> ${datos.docente.facultad}</p>
                            <p><strong>Carrera:</strong> ${datos.docente.carrera}</p>
                        </div>
                    </div>
                </div>
                
                <div class="col-md-6">
                    <div class="card mb-3">
                        <div class="card-header bg-success text-white">
                            <h5 class="mb-0">Información de Escalafón</h5>
                        </div>
                        <div class="card-body">
                            <p><strong>Nivel Actual:</strong> ${datos.escalafon.nivelActual}</p>
                            <p><strong>Nivel Solicitado:</strong> ${datos.escalafon.nivelSolicitado}</p>
                            <p><strong>Fecha de Solicitud:</strong> ${datos.escalafon.fechaSolicitud}</p>
                            <p><strong>Fecha de Aprobación:</strong> ${datos.escalafon.fechaAprobacion}</p>
                            <p><strong>Años de Experiencia:</strong> ${datos.escalafon.anosExperiencia}</p>
                            <p><strong>Estado:</strong> <span class="badge bg-success">${datos.escalafon.status}</span></p>
                        </div>
                    </div>
                </div>
            </div>
            
            <div class="card">
                <div class="card-header bg-info text-white">
                    <h5 class="mb-0">Detalles Académicos</h5>
                </div>
                <div class="card-body">
                    <p><strong>Títulos:</strong> ${datos.detalles.titulos}</p>
                    <p><strong>Publicaciones:</strong> ${datos.detalles.publicaciones}</p>
                    <p><strong>Proyectos de Investigación:</strong> ${datos.detalles.proyectosInvestigacion}</p>
                    <p><strong>Capacitaciones:</strong> ${datos.detalles.capacitaciones}</p>
                    <p><strong>Observaciones:</strong> ${datos.detalles.observaciones}</p>
                </div>
            </div>
            
            <div class="text-end mt-3">
                <small class="text-muted">Documento generado el ${datos.fecha}</small>
            </div>
        </div>
    `;
}

function generarHTMLFicha(datos) {
    return `
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset="UTF-8">
            <title>Ficha de Ascenso Docente</title>
            <style>
                body { 
                    font-family: Arial, sans-serif; 
                    margin: 20px; 
                    line-height: 1.6;
                }
                .header { 
                    text-align: center; 
                    margin-bottom: 30px; 
                    border-bottom: 2px solid #333;
                    padding-bottom: 20px;
                }
                .header h1 { 
                    color: #2c3e50; 
                    margin: 0; 
                }
                .header h2 { 
                    color: #34495e; 
                    margin: 5px 0; 
                    font-weight: normal;
                }
                .seccion { 
                    margin-bottom: 25px; 
                    padding: 15px;
                    border: 1px solid #ddd;
                    border-radius: 5px;
                }
                .seccion h3 { 
                    color: #2c3e50; 
                    border-bottom: 1px solid #bdc3c7;
                    padding-bottom: 5px;
                    margin-top: 0;
                }
                .campo { 
                    margin-bottom: 10px; 
                    display: flex;
                    align-items: flex-start;
                }
                .campo strong { 
                    min-width: 150px; 
                    color: #2c3e50;
                }
                .campo span {
                    flex: 1;
                    padding-left: 10px;
                }
                .status-badge {
                    display: inline-block;
                    padding: 5px 10px;
                    border-radius: 15px;
                    font-size: 12px;
                    font-weight: bold;
                    text-transform: uppercase;
                    background-color: #27ae60; 
                    color: white;
                }
                .fecha-generacion {
                    text-align: right;
                    font-size: 12px;
                    color: #7f8c8d;
                    margin-top: 30px;
                }
                @media print {
                    body { margin: 0; }
                    .no-print { display: none !important; }
                }
            </style>
        </head>
        <body>
            <div class="header">
                <h1>${datos.titulo}</h1>
                <h2>${datos.subtitulo}</h2>
            </div>
            
            <div class="seccion">
                <h3>Información Personal</h3>
                <div class="campo">
                    <strong>Nombre Completo:</strong>
                    <span>${datos.docente.nombre}</span>
                </div>
                <div class="campo">
                    <strong>Cédula:</strong>
                    <span>${datos.docente.cedula}</span>
                </div>
                <div class="campo">
                    <strong>Email:</strong>
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
                <h3>Información de Escalafón</h3>
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
                    <span>${datos.escalafon.anosExperiencia}</span>
                </div>
                <div class="campo">
                    <strong>Estado:</strong>
                    <span>
                        <span class="status-badge">
                            ${datos.escalafon.status}
                        </span>
                    </span>
                </div>
            </div>
            
            <div class="seccion">
                <h3>Detalles Académicos</h3>
                <div class="campo">
                    <strong>Títulos:</strong>
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
                Documento generado el ${datos.fecha}
            </div>
        </body>
        </html>
    `;
}
