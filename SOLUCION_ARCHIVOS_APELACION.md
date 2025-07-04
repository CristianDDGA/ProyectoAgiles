# Solución: Problema con archivos de apelación no encontrados

## Problema identificado
Los archivos que subían los docentes en las apelaciones no se encontraban cuando se intentaba acceder a ellos desde la gestión administrativa.

## Causas raíz encontradas

1. **Falta de configuración para archivos estáticos en la API**
   - El servidor API no tenía `UseStaticFiles()` configurado
   - Los archivos se guardaban pero no se podían servir

2. **Método de guardado de archivos incorrecto**
   - El método `GuardarArchivosApelacionAsync` no usaba el `FileService` correctamente
   - Se utilizaba `Directory.GetCurrentDirectory()` en lugar de `WebRootPath`

3. **Tipos de contenido y extensiones faltantes**
   - No se incluían tipos MIME para documentos Word (.doc, .docx)
   - Las extensiones permitidas no incluían .doc y .docx

## Soluciones implementadas

### 1. Configuración de archivos estáticos
```csharp
// En Program.cs
app.UseStaticFiles();
```

### 2. Nuevo método en FileService
```csharp
public async Task<string> SaveFileWithOriginalNameAsync(byte[] fileBytes, string fileName, string contentType, string folder = "uploads")
```

### 3. Actualización del método de guardado
```csharp
private async Task GuardarArchivosApelacionAsync(int solicitudId, List<IFormFile> archivos)
{
    var carpetaApelacion = $"uploads/apelaciones/{solicitudId}";
    var rutaGuardada = await _fileService.SaveFileWithOriginalNameAsync(fileBytes, archivo.FileName, archivo.ContentType, carpetaApelacion);
}
```

### 4. Tipos de contenido ampliados
```csharp
_allowedContentTypes = new List<string>
{
    "image/jpeg", "image/jpg", "image/png", "image/gif", "image/bmp",
    "application/pdf",
    "application/msword",
    "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
};
```

### 5. Extensiones permitidas ampliadas
```csharp
var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".pdf", ".doc", ".docx" };
```

### 6. Logging mejorado
- Agregado logging detallado en FileService
- Agregado logging en el controlador de apelaciones
- Información de debugging para identificar problemas

## Estructura de carpetas
```
ProyectoAgiles.Api/
├── wwwroot/
│   └── uploads/
│       └── apelaciones/
│           └── {solicitudId}/
│               ├── archivo1.pdf
│               ├── archivo2.docx
│               └── ...
```

## Endpoints afectados
- `POST /api/solicitudes-escalafon/{id}/apelar` - Creación de apelación con archivos
- `GET /api/solicitudes-escalafon/{id}/apelacion/archivo/{nombreArchivo}` - Obtener archivo

## Cómo probar
1. Ejecutar la API con `dotnet run`
2. Subir una apelación con archivos adjuntos
3. Verificar en los logs que los archivos se guardan correctamente
4. Intentar visualizar los archivos desde la gestión administrativa
5. Los archivos deben mostrarse correctamente en el navegador

## Archivos modificados
- `ProyectoAgiles.Api/Program.cs` - Agregado UseStaticFiles()
- `ProyectoAgiles.Application/Services/FileService.cs` - Nuevo método y logging
- `ProyectoAgiles.Application/Interfaces/IFileService.cs` - Nueva interfaz
- `ProyectoAgiles.Application/Services/SolicitudEscalafonService.cs` - Método de guardado mejorado
- `ProyectoAgiles.Api/Controllers/SolicitudesEscalafonController.cs` - Logging mejorado

## Resultado esperado
✅ Los archivos de apelación se guardan correctamente
✅ Los archivos se pueden visualizar desde la gestión administrativa
✅ Se mantiene el nombre original del archivo
✅ Soporte para PDF, DOC, DOCX, JPG, PNG
✅ Logging detallado para debugging
