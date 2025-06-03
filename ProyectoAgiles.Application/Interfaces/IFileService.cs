namespace ProyectoAgiles.Application.Interfaces;

public interface IFileService
{
    /// <summary>
    /// Guarda un archivo en el servidor y retorna la ruta relativa
    /// </summary>
    /// <param name="fileBytes">Contenido del archivo en bytes</param>
    /// <param name="fileName">Nombre original del archivo</param>
    /// <param name="contentType">Tipo de contenido MIME</param>
    /// <param name="folder">Carpeta donde guardar el archivo (relativa a wwwroot)</param>
    /// <returns>Ruta relativa del archivo guardado</returns>
    Task<string> SaveFileAsync(byte[] fileBytes, string fileName, string contentType, string folder = "uploads");

    /// <summary>
    /// Elimina un archivo del servidor
    /// </summary>
    /// <param name="filePath">Ruta relativa del archivo a eliminar</param>
    /// <returns>True si se eliminó correctamente</returns>
    Task<bool> DeleteFileAsync(string filePath);

    /// <summary>
    /// Valida si un archivo es válido según tipo y tamaño
    /// </summary>
    /// <param name="fileBytes">Contenido del archivo</param>
    /// <param name="fileName">Nombre del archivo</param>
    /// <param name="contentType">Tipo de contenido</param>
    /// <param name="maxSizeInBytes">Tamaño máximo permitido en bytes</param>
    /// <returns>True si el archivo es válido</returns>
    bool ValidateFile(byte[] fileBytes, string fileName, string contentType, long maxSizeInBytes = 10 * 1024 * 1024);

    /// <summary>
    /// Obtiene los tipos de archivo permitidos para documentos de identidad
    /// </summary>
    /// <returns>Lista de tipos MIME permitidos</returns>
    List<string> GetAllowedContentTypes();
}
