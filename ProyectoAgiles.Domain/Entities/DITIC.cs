using System.ComponentModel.DataAnnotations;

namespace ProyectoAgiles.Domain.Entities;

/// <summary>
/// Entidad para las capacitaciones y actualización profesional DITIC
/// </summary>
public class DITIC : BaseEntity
{
    /// <summary>
    /// Cédula del docente
    /// </summary>
    [Required]
    [StringLength(10)]
    public string Cedula { get; set; } = string.Empty;

    /// <summary>
    /// Nombre de la capacitación o curso
    /// </summary>
    [Required]
    [StringLength(500)]
    public string NombreCapacitacion { get; set; } = string.Empty;

    /// <summary>
    /// Institución que otorga la capacitación
    /// </summary>
    [Required]
    [StringLength(300)]
    public string Institucion { get; set; } = string.Empty;

    /// <summary>
    /// Tipo de capacitación (Técnica, Pedagógica, Científica, etc.)
    /// </summary>
    [Required]
    [StringLength(50)]
    public string TipoCapacitacion { get; set; } = string.Empty;

    /// <summary>
    /// Modalidad (Presencial, Virtual, Semipresencial)
    /// </summary>
    [StringLength(30)]
    public string Modalidad { get; set; } = "Presencial";

    /// <summary>
    /// Número de horas académicas de la capacitación
    /// </summary>
    [Required]
    [Range(1, 1000)]
    public int HorasAcademicas { get; set; }

    /// <summary>
    /// Fecha de inicio de la capacitación
    /// </summary>
    [Required]
    public DateTime FechaInicio { get; set; }

    /// <summary>
    /// Fecha de finalización de la capacitación
    /// </summary>
    [Required]
    public DateTime FechaFin { get; set; }

    /// <summary>
    /// Año de la capacitación (para filtros)
    /// </summary>
    [Required]
    public int Anio { get; set; }

    /// <summary>
    /// Estado de la capacitación (Completada, En Progreso, Aprobada, Reprobada)
    /// </summary>
    [StringLength(20)]
    public string Estado { get; set; } = "Completada";

    /// <summary>
    /// Calificación obtenida (si aplica)
    /// </summary>
    [Range(0, 100)]
    public decimal? Calificacion { get; set; }

    /// <summary>
    /// Calificación mínima para aprobar
    /// </summary>
    [Range(0, 100)]
    public decimal CalificacionMinima { get; set; } = 70;

    /// <summary>
    /// Indica si la capacitación fue aprobada
    /// </summary>
    public bool Aprobada => Calificacion.HasValue ? Calificacion >= CalificacionMinima : Estado == "Aprobada";

    /// <summary>
    /// Indica si es capacitación pedagógica (para cumplir el 25% requerido)
    /// </summary>
    public bool EsPedagogica => 
        // Verificar PRIMERO si es explícitamente NO pedagógica por tipo
        !(TipoCapacitacion.ToLower().Contains("tecnica") || 
          TipoCapacitacion.ToLower().Contains("cientifica") ||
          TipoCapacitacion.ToLower().Contains("administrativa") ||
          TipoCapacitacion.ToLower().Contains("informatica") ||
          TipoCapacitacion.ToLower().Contains("financiera")) &&
        
        // LUEGO verificar si es pedagógica por tipo o nombre
        (TipoCapacitacion.ToLower().Contains("pedagog") || 
         TipoCapacitacion.ToLower().Contains("didact") || 
         TipoCapacitacion.ToLower().Contains("docencia") ||
         TipoCapacitacion.ToLower().Contains("enseñanza") ||
         
         // Verificar por nombre SOLO si no es técnica/científica
         NombreCapacitacion.ToLower().Contains("pedagog") ||
         NombreCapacitacion.ToLower().Contains("didact") ||
         NombreCapacitacion.ToLower().Contains("docencia") ||
         NombreCapacitacion.ToLower().Contains("enseñanza") ||
         NombreCapacitacion.ToLower().Contains("metodologias activas") ||
         NombreCapacitacion.ToLower().Contains("estrategias didacticas") ||
         NombreCapacitacion.ToLower().Contains("evaluacion educativa") ||
         NombreCapacitacion.ToLower().Contains("planificacion curricular"));

    /// <summary>
    /// Descripción del contenido de la capacitación
    /// </summary>
    [StringLength(1000)]
    public string? Descripcion { get; set; }

    /// <summary>
    /// Número de certificado (si tiene)
    /// </summary>
    [StringLength(100)]
    public string? NumeroCertificado { get; set; }

    /// <summary>
    /// Instructor o facilitador
    /// </summary>
    [StringLength(200)]
    public string? Instructor { get; set; }

    /// <summary>
    /// Observaciones adicionales
    /// </summary>
    [StringLength(500)]
    public string? Observaciones { get; set; }

    /// <summary>
    /// Archivo de certificado (PDF)
    /// </summary>
    public byte[]? ArchivoCertificado { get; set; }

    /// <summary>
    /// Nombre del archivo de certificado
    /// </summary>
    [StringLength(255)]
    public string? NombreArchivoCertificado { get; set; }

    /// <summary>
    /// Indica si fue eximido por cargo de autoridad (más de 2 años)
    /// </summary>
    public bool ExencionPorAutoridad { get; set; } = false;

    /// <summary>
    /// Cargo de autoridad desempeñado (si aplica)
    /// </summary>
    [StringLength(100)]
    public string? CargoAutoridad { get; set; }

    /// <summary>
    /// Fecha de inicio como autoridad
    /// </summary>
    public DateTime? FechaInicioAutoridad { get; set; }

    /// <summary>
    /// Fecha de fin como autoridad
    /// </summary>
    public DateTime? FechaFinAutoridad { get; set; }

    /// <summary>
    /// Años desempeñados como autoridad (calculado)
    /// </summary>
    public decimal AñosComoAutoridad
    {
        get
        {
            if (!FechaInicioAutoridad.HasValue) return 0;
            var fechaFin = FechaFinAutoridad ?? DateTime.Now;
            var diferencia = fechaFin - FechaInicioAutoridad.Value;
            return (decimal)diferencia.TotalDays / 365.25m;
        }
    }

    /// <summary>
    /// Indica si cumple con la exención por autoridad (más de 2 años)
    /// </summary>
    public bool CumpleExencionAutoridad => ExencionPorAutoridad && AñosComoAutoridad > 2;
}
