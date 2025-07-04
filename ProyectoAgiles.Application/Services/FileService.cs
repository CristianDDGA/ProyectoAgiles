using Microsoft.Extensions.Hosting;
using ProyectoAgiles.Application.Interfaces;

namespace ProyectoAgiles.Application.Services;

public class FileService : IFileService
{
    private readonly IHostEnvironment _environment;
    private readonly List<string> _allowedContentTypes;
    private readonly string _webRootPath;

    public FileService(IHostEnvironment environment)
    {
        _environment = environment;
        _webRootPath = Path.Combine(_environment.ContentRootPath, "wwwroot");
        _allowedContentTypes = new List<string>
        {
            "image/jpeg",
            "image/jpg", 
            "image/png",
            "image/gif",
            "image/bmp",
            "application/pdf",
            "application/msword",
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
        };
    }

    public async Task<string> SaveFileAsync(byte[] fileBytes, string fileName, string contentType, string folder = "uploads")
    {
        try
        {
            // Validar el archivo
            if (!ValidateFile(fileBytes, fileName, contentType))
            {
                throw new ArgumentException("Archivo no válido");
            }

            // Crear nombre único para evitar conflictos
            var fileExtension = Path.GetExtension(fileName);
            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";

            // Crear ruta de destino
            var uploadsPath = Path.Combine(_webRootPath, folder);
            
            // Crear directorio si no existe
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            var filePath = Path.Combine(uploadsPath, uniqueFileName);

            // Guardar archivo
            await File.WriteAllBytesAsync(filePath, fileBytes);

            // Retornar ruta relativa
            return Path.Combine(folder, uniqueFileName).Replace("\\", "/");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error al guardar archivo: {ex.Message}", ex);
        }
    }    public Task<bool> DeleteFileAsync(string filePath)
    {
        try
        {
            if (string.IsNullOrEmpty(filePath))
                return Task.FromResult(false);

            var fullPath = Path.Combine(_webRootPath, filePath);
            
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    public bool ValidateFile(byte[] fileBytes, string fileName, string contentType, long maxSizeInBytes = 10 * 1024 * 1024)
    {
        Console.WriteLine($"[FileService] Validando archivo: {fileName}, Tipo: {contentType}, Tamaño: {fileBytes.Length} bytes");
        
        // Validar tamaño
        if (fileBytes.Length > maxSizeInBytes)
        {
            Console.WriteLine($"[FileService] Archivo demasiado grande: {fileBytes.Length} > {maxSizeInBytes}");
            return false;
        }

        // Validar tipo de contenido
        if (!_allowedContentTypes.Contains(contentType.ToLower()))
        {
            Console.WriteLine($"[FileService] Tipo de contenido no permitido: {contentType}");
            Console.WriteLine($"[FileService] Tipos permitidos: {string.Join(", ", _allowedContentTypes)}");
            return false;
        }

        // Validar extensión
        var extension = Path.GetExtension(fileName)?.ToLower();
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".pdf", ".doc", ".docx" };
        
        if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
        {
            Console.WriteLine($"[FileService] Extensión no permitida: {extension}");
            Console.WriteLine($"[FileService] Extensiones permitidas: {string.Join(", ", allowedExtensions)}");
            return false;
        }

        Console.WriteLine($"[FileService] Archivo válido: {fileName}");
        return true;
    }

    public List<string> GetAllowedContentTypes()
    {
        return new List<string>(_allowedContentTypes);
    }

    public async Task<string> SaveFileWithOriginalNameAsync(byte[] fileBytes, string fileName, string contentType, string folder = "uploads")
    {
        try
        {
            Console.WriteLine($"[FileService] Guardando archivo: {fileName}, Tipo: {contentType}, Carpeta: {folder}");
            
            // Validar el archivo
            if (!ValidateFile(fileBytes, fileName, contentType))
            {
                Console.WriteLine($"[FileService] Archivo no válido: {fileName}");
                throw new ArgumentException("Archivo no válido");
            }

            // Crear ruta de destino
            var uploadsPath = Path.Combine(_webRootPath, folder);
            Console.WriteLine($"[FileService] Ruta de destino: {uploadsPath}");
            
            // Crear directorio si no existe
            if (!Directory.Exists(uploadsPath))
            {
                Console.WriteLine($"[FileService] Creando directorio: {uploadsPath}");
                Directory.CreateDirectory(uploadsPath);
            }

            var filePath = Path.Combine(uploadsPath, fileName);
            Console.WriteLine($"[FileService] Ruta completa del archivo: {filePath}");

            // Guardar archivo
            await File.WriteAllBytesAsync(filePath, fileBytes);
            Console.WriteLine($"[FileService] Archivo guardado exitosamente: {filePath}");

            // Retornar ruta relativa
            var relativePath = Path.Combine(folder, fileName).Replace("\\", "/");
            Console.WriteLine($"[FileService] Ruta relativa: {relativePath}");
            return relativePath;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[FileService] Error al guardar archivo: {ex.Message}");
            throw new InvalidOperationException($"Error al guardar archivo: {ex.Message}", ex);
        }
    }
}
