using System.ComponentModel.DataAnnotations;

namespace ProyectoAgiles.Domain.Entities;

/// <summary>
/// Entidad para las evaluaciones de desempeño académico de los docentes
/// </summary>
public class EvaluacionDesempeno : BaseEntity
{
    /// <summary>
    /// Cédula del docente evaluado
    /// </summary>
    [Required]
    [StringLength(10)]
    public string Cedula { get; set; } = string.Empty;

    /// <summary>
    /// Período académico de la evaluación (ejemplo: "2023-1", "2023-2", "2024-1")
    /// </summary>
    [Required]
    [StringLength(10)]
    public string PeriodoAcademico { get; set; } = string.Empty;

    /// <summary>
    /// Año de la evaluación
    /// </summary>
    [Required]
    public int Anio { get; set; }

    /// <summary>
    /// Semestre o período (1 o 2)
    /// </summary>
    [Required]
    [Range(1, 2)]
    public int Semestre { get; set; }

    /// <summary>
    /// Puntaje obtenido en la evaluación integral
    /// </summary>
    [Required]
    [Range(0, 100)]
    public decimal PuntajeObtenido { get; set; }

    /// <summary>
    /// Puntaje máximo posible de la evaluación
    /// </summary>
    [Required]
    [Range(0, 100)]
    public decimal PuntajeMaximo { get; set; } = 100;

    /// <summary>
    /// Porcentaje obtenido en la evaluación (calculado automáticamente)
    /// </summary>
    public decimal PorcentajeObtenido => PuntajeMaximo > 0 ? (PuntajeObtenido / PuntajeMaximo) * 100 : 0;

    /// <summary>
    /// Indica si cumple con el mínimo requerido (75%)
    /// </summary>
    public bool CumpleMinimo => PorcentajeObtenido >= 75;

    /// <summary>
    /// Fecha de la evaluación
    /// </summary>
    [Required]
    public DateTime FechaEvaluacion { get; set; }

    /// <summary>
    /// Tipo de evaluación (Docencia, Investigación, Vinculación, Gestión)
    /// </summary>
    [StringLength(50)]
    public string TipoEvaluacion { get; set; } = "Integral";

    /// <summary>
    /// Observaciones adicionales
    /// </summary>
    [StringLength(500)]
    public string? Observaciones { get; set; }

    /// <summary>
    /// Estado de la evaluación (Pendiente, Completada, Aprobada, Rechazada)
    /// </summary>
    [StringLength(20)]
    public string Estado { get; set; } = "Completada";

    /// <summary>
    /// Evaluador responsable
    /// </summary>
    [StringLength(100)]
    public string? Evaluador { get; set; }

    /// <summary>
    /// Archivo de respaldo (PDF o documento)
    /// </summary>
    public byte[]? ArchivoRespaldo { get; set; }

    /// <summary>
    /// Nombre del archivo de respaldo
    /// </summary>
    [StringLength(255)]
    public string? NombreArchivoRespaldo { get; set; }
}
