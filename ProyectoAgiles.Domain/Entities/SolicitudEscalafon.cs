using System.ComponentModel.DataAnnotations;

namespace ProyectoAgiles.Domain.Entities;

/// <summary>
/// Entidad para las solicitudes de escalafón docente
/// </summary>
public class SolicitudEscalafon : BaseEntity
{
    /// <summary>
    /// Cédula del docente que solicita el escalafón
    /// </summary>
    [Required]
    [StringLength(10)]
    public string DocenteCedula { get; set; } = string.Empty;

    /// <summary>
    /// Nombre completo del docente
    /// </summary>
    [Required]
    [StringLength(200)]
    public string DocenteNombre { get; set; } = string.Empty;

    /// <summary>
    /// Email del docente
    /// </summary>
    [Required]
    [StringLength(255)]
    public string DocenteEmail { get; set; } = string.Empty;

    /// <summary>
    /// Teléfono del docente
    /// </summary>
    [StringLength(20)]
    public string? DocenteTelefono { get; set; }

    /// <summary>
    /// Facultad del docente
    /// </summary>
    [StringLength(200)]
    public string? Facultad { get; set; }

    /// <summary>
    /// Carrera del docente
    /// </summary>
    [StringLength(200)]
    public string? Carrera { get; set; }

    /// <summary>
    /// Nivel actual del docente
    /// </summary>
    [Required]
    [StringLength(100)]
    public string NivelActual { get; set; } = string.Empty;

    /// <summary>
    /// Nivel solicitado
    /// </summary>
    [Required]
    [StringLength(100)]
    public string NivelSolicitado { get; set; } = string.Empty;

    /// <summary>
    /// Años de experiencia del docente
    /// </summary>
    public int AnosExperiencia { get; set; }

    /// <summary>
    /// Títulos del docente
    /// </summary>
    [StringLength(1000)]
    public string? Titulos { get; set; }

    /// <summary>
    /// Publicaciones del docente
    /// </summary>
    [StringLength(1000)]
    public string? Publicaciones { get; set; }

    /// <summary>
    /// Proyectos de investigación del docente
    /// </summary>
    [StringLength(1000)]
    public string? ProyectosInvestigacion { get; set; }

    /// <summary>
    /// Capacitaciones del docente
    /// </summary>
    [StringLength(1000)]
    public string? Capacitaciones { get; set; }

    /// <summary>
    /// Fecha de la solicitud
    /// </summary>
    [Required]
    public DateTime FechaSolicitud { get; set; } = DateTime.Now;

    /// <summary>
    /// Fecha de aprobación (si aplica)
    /// </summary>
    public DateTime? FechaAprobacion { get; set; }

    /// <summary>
    /// Fecha de rechazo (si aplica)
    /// </summary>
    public DateTime? FechaRechazo { get; set; }

    /// <summary>
    /// Fecha de envío al consejo (si aplica)
    /// </summary>
    public DateTime? FechaEnvioConsejo { get; set; }

    /// <summary>
    /// Estado de la solicitud
    /// </summary>
    [Required]
    [StringLength(20)]
    public string Status { get; set; } = "Pendiente";

    /// <summary>
    /// Observaciones adicionales
    /// </summary>
    [StringLength(1000)]
    public string? Observaciones { get; set; }

    /// <summary>
    /// Observaciones del consejo
    /// </summary>
    [StringLength(1000)]
    public string? ObservacionesConsejo { get; set; }

    /// <summary>
    /// Motivo del rechazo (si aplica)
    /// </summary>
    [StringLength(1000)]
    public string? MotivoRechazo { get; set; }

    /// <summary>
    /// Motivo del rechazo por parte del consejo
    /// </summary>
    [StringLength(1000)]
    public string? MotivoRechazoConsejo { get; set; }

    /// <summary>
    /// Usuario que procesó la solicitud
    /// </summary>
    [StringLength(100)]
    public string? ProcesadoPor { get; set; }
}
