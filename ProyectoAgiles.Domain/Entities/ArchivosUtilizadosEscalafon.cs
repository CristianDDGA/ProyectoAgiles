using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoAgiles.Domain.Entities;

/// <summary>
/// Entidad que registra qué archivos/recursos fueron utilizados en un ascenso específico
/// para evitar que se reutilicen en futuros ascensos
/// </summary>
public class ArchivosUtilizadosEscalafon : BaseEntity
{
    /// <summary>
    /// ID de la solicitud de escalafón donde se utilizó el archivo
    /// </summary>
    [Required]
    public int SolicitudEscalafonId { get; set; }

    /// <summary>
    /// Navegación a la solicitud de escalafón
    /// </summary>
    [ForeignKey("SolicitudEscalafonId")]
    public SolicitudEscalafon SolicitudEscalafon { get; set; } = null!;

    /// <summary>
    /// Tipo de recurso utilizado (Investigacion, EvaluacionDesempeno, Capacitacion)
    /// </summary>
    [Required]
    [StringLength(50)]
    public string TipoRecurso { get; set; } = string.Empty;

    /// <summary>
    /// ID del recurso específico que se utilizó
    /// </summary>
    [Required]
    public int RecursoId { get; set; }

    /// <summary>
    /// Cédula del docente (para optimizar consultas)
    /// </summary>
    [Required]
    [StringLength(10)]
    public string DocenteCedula { get; set; } = string.Empty;

    /// <summary>
    /// Nivel desde el cual se ascendió
    /// </summary>
    [Required]
    [StringLength(100)]
    public string NivelOrigen { get; set; } = string.Empty;

    /// <summary>
    /// Nivel al cual se ascendió
    /// </summary>
    [Required]
    [StringLength(100)]
    public string NivelDestino { get; set; } = string.Empty;

    /// <summary>
    /// Fecha en que se utilizó el archivo para el ascenso
    /// </summary>
    [Required]
    public DateTime FechaUtilizacion { get; set; }

    /// <summary>
    /// Descripción adicional del recurso utilizado
    /// </summary>
    [StringLength(500)]
    public string? Descripcion { get; set; }

    /// <summary>
    /// Estado del ascenso (Aprobado, Rechazado, etc.)
    /// Solo se considera "usado" si el ascenso fue exitoso
    /// </summary>
    [Required]
    [StringLength(50)]
    public string EstadoAscenso { get; set; } = string.Empty;
}
