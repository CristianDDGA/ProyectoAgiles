using ProyectoAgiles.Application.DTOs;

namespace ProyectoAgiles.Application.Interfaces;

/// <summary>
/// Interfaz para el servicio de gestión de archivos utilizados en escalafones
/// </summary>
public interface IArchivosUtilizadosService
{
    /// <summary>
    /// Registra los archivos utilizados cuando se aprueba un escalafón
    /// </summary>
    Task RegistrarArchivosUtilizados(int solicitudEscalafonId, string docenteCedula, string nivelOrigen, string nivelDestino);

    /// <summary>
    /// Obtiene los IDs de investigaciones ya utilizadas en ascensos previos
    /// </summary>
    Task<List<int>> ObtenerInvestigacionesUtilizadas(string docenteCedula);

    /// <summary>
    /// Obtiene los IDs de evaluaciones ya utilizadas en ascensos previos
    /// </summary>
    Task<List<int>> ObtenerEvaluacionesUtilizadas(string docenteCedula);

    /// <summary>
    /// Obtiene los IDs de capacitaciones ya utilizadas en ascensos previos
    /// </summary>
    Task<List<int>> ObtenerCapacitacionesUtilizadas(string docenteCedula);

    /// <summary>
    /// Obtiene el historial de archivos utilizados en ascensos de un docente
    /// </summary>
    Task<List<ArchivosUtilizadosDto>> ObtenerHistorialArchivos(string docenteCedula);

    /// <summary>
    /// Verifica si un archivo específico ya fue utilizado en un ascenso previo
    /// </summary>
    Task<bool> ArchivoYaUtilizado(string docenteCedula, string tipoRecurso, int recursoId);

    /// <summary>
    /// Obtiene estadísticas de archivos utilizados por tipo
    /// </summary>
    Task<Dictionary<string, int>> ObtenerEstadisticasArchivosUtilizados(string docenteCedula);

    /// <summary>
    /// Obtiene los archivos utilizados en una solicitud específica de escalafón
    /// </summary>
    Task<List<ArchivosUtilizadosDto>> ObtenerArchivosPorSolicitud(int solicitudEscalafonId);
}
