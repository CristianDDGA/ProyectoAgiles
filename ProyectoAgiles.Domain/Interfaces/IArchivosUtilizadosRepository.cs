using ProyectoAgiles.Domain.Entities;
using ProyectoAgiles.Domain.Interfaces;

namespace ProyectoAgiles.Domain.Interfaces;

/// <summary>
/// Interfaz del repositorio para archivos utilizados en escalafones
/// </summary>
public interface IArchivosUtilizadosRepository : IRepository<ArchivosUtilizadosEscalafon>
{
    /// <summary>
    /// Obtiene archivos utilizados por cédula del docente
    /// </summary>
    Task<List<ArchivosUtilizadosEscalafon>> GetByDocenteCedulaAsync(string cedula);

    /// <summary>
    /// Obtiene archivos utilizados por tipo de recurso
    /// </summary>
    Task<List<ArchivosUtilizadosEscalafon>> GetByTipoRecursoAsync(string tipoRecurso);

    /// <summary>
    /// Obtiene archivos utilizados para un recurso específico
    /// </summary>
    Task<List<ArchivosUtilizadosEscalafon>> GetByRecursoAsync(string tipoRecurso, int recursoId);

    /// <summary>
    /// Verifica si un recurso ya fue utilizado
    /// </summary>
    Task<bool> IsRecursoUtilizadoAsync(string docenteCedula, string tipoRecurso, int recursoId);

    /// <summary>
    /// Obtiene archivos utilizados por ID de solicitud de escalafón
    /// </summary>
    Task<List<ArchivosUtilizadosEscalafon>> GetBySolicitudEscalafonIdAsync(int solicitudEscalafonId);
}
