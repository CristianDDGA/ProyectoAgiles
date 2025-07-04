// PDF Generator Functions
// Función para abrir un PDF generado a partir de HTML en una nueva pestaña
window.openPDFInNewTab = function(htmlContent, fileName = 'informe.pdf') {
    try {
        // Crear un iframe invisible para generar el PDF
        const iframe = document.createElement('iframe');
        iframe.style.visibility = 'hidden';
        iframe.style.position = 'absolute';
        iframe.style.left = '-9999px';
        iframe.style.width = '0';
        iframe.style.height = '0';
        
        document.body.appendChild(iframe);
        
        // Obtener el documento del iframe
        const doc = iframe.contentDocument || iframe.contentWindow.document;
        
        // Escribir el HTML con estilos CSS para el PDF
        const fullHTML = `
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset="UTF-8">
                <title>${fileName}</title>
                <style>
                    body {
                        font-family: Arial, sans-serif;
                        margin: 20px;
                        line-height: 1.6;
                        color: #333;
                    }
                    .header {
                        text-align: center;
                        margin-bottom: 30px;
                        border-bottom: 2px solid #007bff;
                        padding-bottom: 15px;
                    }
                    .header h1 {
                        color: #007bff;
                        margin: 0;
                        font-size: 24px;
                    }
                    .content {
                        margin: 20px 0;
                    }
                    .section {
                        margin-bottom: 20px;
                        padding: 15px;
                        border-left: 4px solid #007bff;
                        background-color: #f8f9fa;
                    }
                    .section h3 {
                        color: #007bff;
                        margin-top: 0;
                    }
                    .info-row {
                        display: flex;
                        margin-bottom: 10px;
                    }
                    .info-label {
                        font-weight: bold;
                        min-width: 200px;
                        color: #495057;
                    }
                    .info-value {
                        flex: 1;
                    }
                    .footer {
                        margin-top: 40px;
                        text-align: center;
                        border-top: 1px solid #dee2e6;
                        padding-top: 20px;
                        font-size: 12px;
                        color: #6c757d;
                    }
                    @media print {
                        body { margin: 0; }
                        .header { break-after: avoid; }
                        .section { break-inside: avoid; }
                    }
                </style>
            </head>
            <body>
                ${htmlContent}
            </body>
            </html>
        `;
        
        doc.open();
        doc.write(fullHTML);
        doc.close();
        
        // Esperar a que el contenido se cargue
        setTimeout(() => {
            try {
                // Abrir la ventana de impresión que permitirá guardar como PDF
                iframe.contentWindow.print();
                
                // Remover el iframe después de un tiempo
                setTimeout(() => {
                    document.body.removeChild(iframe);
                }, 1000);
                
            } catch (printError) {
                console.error('Error al imprimir/generar PDF:', printError);
                // Fallback: abrir en nueva ventana
                const newWindow = window.open('', '_blank');
                newWindow.document.write(fullHTML);
                newWindow.document.close();
            }
        }, 500);
        
    } catch (error) {
        console.error('Error al generar PDF:', error);
        alert('Error al generar el PDF. Por favor, inténtelo de nuevo.');
    }
};

// Función alternativa usando jsPDF (si se incluye la librería)
window.generatePDFWithJsPDF = function(htmlContent, fileName = 'informe.pdf') {
    try {
        if (typeof window.jsPDF === 'undefined') {
            console.warn('jsPDF no está disponible, usando método alternativo');
            window.openPDFInNewTab(htmlContent, fileName);
            return;
        }
        
        const { jsPDF } = window.jsPDF;
        const doc = new jsPDF();
        
        // Configurar el documento
        doc.setFontSize(12);
        
        // Convertir HTML a texto plano para jsPDF (simplificado)
        const tempDiv = document.createElement('div');
        tempDiv.innerHTML = htmlContent;
        const textContent = tempDiv.textContent || tempDiv.innerText || '';
        
        // Dividir el texto en líneas
        const lines = doc.splitTextToSize(textContent, 180);
        let y = 20;
        
        lines.forEach(line => {
            if (y > 270) {
                doc.addPage();
                y = 20;
            }
            doc.text(line, 20, y);
            y += 7;
        });
        
        // Abrir en nueva pestaña
        const pdfOutput = doc.output('datauristring');
        window.open(pdfOutput, '_blank');
        
    } catch (error) {
        console.error('Error al generar PDF con jsPDF:', error);
        // Fallback
        window.openPDFInNewTab(htmlContent, fileName);
    }
};

// Función para descargar el PDF directamente
window.downloadPDF = function(htmlContent, fileName = 'informe.pdf') {
    try {
        const blob = new Blob([htmlContent], { type: 'text/html' });
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = fileName.replace('.pdf', '.html');
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        URL.revokeObjectURL(url);
    } catch (error) {
        console.error('Error al descargar:', error);
        alert('Error al descargar el archivo.');
    }
};

// Función para mostrar PDFs en el navegador
window.showPdf = function(pdfDataUrl, fileName = 'archivo.pdf') {
    try {
        // Intentar abrir en una nueva pestaña
        const newWindow = window.open('', '_blank');
        
        if (newWindow) {
            newWindow.document.write(`
                <!DOCTYPE html>
                <html>
                <head>
                    <title>${fileName}</title>
                    <style>
                        body { 
                            margin: 0; 
                            padding: 0; 
                            background-color: #2c2c2c;
                            display: flex;
                            justify-content: center;
                            align-items: center;
                            min-height: 100vh;
                        }
                        iframe { 
                            width: 100%; 
                            height: 100vh; 
                            border: none;
                            background-color: white;
                            border-radius: 8px;
                            box-shadow: 0 4px 8px rgba(0,0,0,0.3);
                        }
                        .pdf-container {
                            width: 95%;
                            height: 95vh;
                            background-color: white;
                            border-radius: 8px;
                            box-shadow: 0 4px 8px rgba(0,0,0,0.3);
                        }
                    </style>
                </head>
                <body>
                    <div class="pdf-container">
                        <iframe src="${pdfDataUrl}" type="application/pdf"></iframe>
                    </div>
                </body>
                </html>
            `);
            newWindow.document.close();
            return true;
        } else {
            // Si no se puede abrir una nueva pestaña, descargar el archivo
            return downloadPdfFile(pdfDataUrl, fileName);
        }
    } catch (error) {
        console.error('Error al mostrar PDF:', error);
        // En caso de error, intentar descargar
        return downloadPdfFile(pdfDataUrl, fileName);
    }
};

// Función auxiliar para descargar archivos PDF
function downloadPdfFile(pdfDataUrl, fileName) {
    try {
        const link = document.createElement('a');
        link.href = pdfDataUrl;
        link.download = fileName;
        link.style.display = 'none';
        
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
        
        return false; // Indica que se descargó en lugar de mostrarse
    } catch (error) {
        console.error('Error al descargar PDF:', error);
        return false;
    }
}

// Función para manejar archivos de apelación específicamente
window.viewApelacionFile = function(solicitudId, fileName) {
    fetch(`/api/solicitudes-escalafon/${solicitudId}/apelacion/archivo/${fileName}`)
        .then(response => {
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            return response.blob();
        })
        .then(blob => {
            const fileUrl = URL.createObjectURL(blob);
            const newWindow = window.open(fileUrl, '_blank');
            
            if (!newWindow) {
                // Si no se puede abrir en nueva pestaña, descargar
                const link = document.createElement('a');
                link.href = fileUrl;
                link.download = fileName;
                link.click();
                URL.revokeObjectURL(fileUrl);
            }
        })
        .catch(error => {
            console.error('Error al cargar archivo de apelación:', error);
            alert('Error al cargar el archivo. Por favor, intente nuevamente.');
        });
};
