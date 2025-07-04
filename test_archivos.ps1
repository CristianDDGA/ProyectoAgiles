# Script de prueba para verificar el funcionamiento de archivos de apelación

# Función para probar la subida de archivos
function Test-FileUpload {
    $filePath = "C:\Users\ASUS\OneDrive\Escritorio\ProyectoAgiles\archivo_prueba_apelacion.txt"
    $uri = "http://localhost:5200/api/archivos/upload"
    
    # Crear el contenido multipart/form-data
    $boundary = [System.Guid]::NewGuid().ToString()
    $fileName = "archivo_prueba_apelacion.txt"
    $fileContent = Get-Content -Path $filePath -Raw
    
    $body = @"
--$boundary
Content-Disposition: form-data; name="file"; filename="$fileName"
Content-Type: text/plain

$fileContent
--$boundary--
"@
    
    try {
        $response = Invoke-WebRequest -Uri $uri -Method POST -Body $body -ContentType "multipart/form-data; boundary=$boundary" -UseBasicParsing
        Write-Host "✅ Archivo subido exitosamente"
        Write-Host "Código de estado: $($response.StatusCode)"
        Write-Host "Respuesta: $($response.Content)"
        return $response.Content
    } catch {
        Write-Host "❌ Error al subir archivo: $($_.Exception.Message)"
        return $null
    }
}

# Función para probar la descarga de archivos
function Test-FileDownload {
    param($fileName)
    
    $uri = "http://localhost:5200/api/archivos/download/$fileName"
    
    try {
        $response = Invoke-WebRequest -Uri $uri -UseBasicParsing
        Write-Host "✅ Archivo descargado exitosamente"
        Write-Host "Código de estado: $($response.StatusCode)"
        Write-Host "Contenido: $($response.Content.Substring(0, [Math]::Min(100, $response.Content.Length)))..."
        return $response
    } catch {
        Write-Host "❌ Error al descargar archivo: $($_.Exception.Message)"
        return $null
    }
}

# Ejecutar las pruebas
Write-Host "=== Prueba de funcionalidad de archivos ===" -ForegroundColor Green
Write-Host ""

Write-Host "1. Probando subida de archivo..." -ForegroundColor Yellow
$uploadResponse = Test-FileUpload

if ($uploadResponse) {
    Write-Host ""
    Write-Host "2. Probando descarga de archivo..." -ForegroundColor Yellow
    $downloadResponse = Test-FileDownload -fileName "archivo_prueba_apelacion.txt"
}

Write-Host ""
Write-Host "=== Fin de pruebas ===" -ForegroundColor Green
