using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ProyectoAgiles.Application.DTOs;

/// <summary>
/// DTO para mostrar información de evaluación de desempeño
/// </summary>
public class EvaluacionDesempenoDto
{
    public int Id { get; set; }
    public string Cedula { get; set; } = string.Empty;
    public string PeriodoAcademico { get; set; } = string.Empty;
    public int Anio { get; set; }
    public int Semestre { get; set; }
    public decimal PuntajeObtenido { get; set; }
    public decimal PuntajeMaximo { get; set; }
    public decimal PorcentajeObtenido { get; set; }
    public bool CumpleMinimo { get; set; }
    public DateTime FechaEvaluacion { get; set; }
    public string TipoEvaluacion { get; set; } = string.Empty;
    public string? Observaciones { get; set; }
    public string Estado { get; set; } = string.Empty;
    public string? Evaluador { get; set; }
    public string? NombreArchivoRespaldo { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// DTO para crear una nueva evaluación de desempeño
/// </summary>
public class CreateEvaluacionDesempenoDto
{
    [Required(ErrorMessage = "La cédula es obligatoria")]
    [StringLength(10, ErrorMessage = "La cédula debe tener máximo 10 caracteres")]
    public string Cedula { get; set; } = string.Empty;

    [Required(ErrorMessage = "El período académico es obligatorio")]
    [StringLength(10, ErrorMessage = "El período académico debe tener máximo 10 caracteres")]
    public string PeriodoAcademico { get; set; } = string.Empty;

    [Required(ErrorMessage = "El año es obligatorio")]
    [Range(2020, 2030, ErrorMessage = "El año debe estar entre 2020 y 2030")]
    public int Anio { get; set; }

    [Required(ErrorMessage = "El semestre es obligatorio")]
    [Range(1, 2, ErrorMessage = "El semestre debe ser 1 o 2")]
    public int Semestre { get; set; }

    [Required(ErrorMessage = "El puntaje obtenido es obligatorio")]
    [Range(0, 100, ErrorMessage = "El puntaje debe estar entre 0 y 100")]
    public decimal PuntajeObtenido { get; set; }

    [Range(0, 100, ErrorMessage = "El puntaje máximo debe estar entre 0 y 100")]
    public decimal PuntajeMaximo { get; set; } = 100;

    [Required(ErrorMessage = "La fecha de evaluación es obligatoria")]
    public DateTime FechaEvaluacion { get; set; }

    [StringLength(50, ErrorMessage = "El tipo de evaluación debe tener máximo 50 caracteres")]
    public string TipoEvaluacion { get; set; } = "Integral";

    [StringLength(500, ErrorMessage = "Las observaciones deben tener máximo 500 caracteres")]
    public string? Observaciones { get; set; }

    [StringLength(20, ErrorMessage = "El estado debe tener máximo 20 caracteres")]
    public string Estado { get; set; } = "Completada";

    [StringLength(100, ErrorMessage = "El evaluador debe tener máximo 100 caracteres")]
    public string? Evaluador { get; set; }
}

/// <summary>
/// DTO para crear evaluación con archivo PDF
/// </summary>
public class CreateEvaluacionWithPdfDto : CreateEvaluacionDesempenoDto
{
    [Required(ErrorMessage = "El archivo PDF es obligatorio")]
    public IFormFile ArchivoPdf { get; set; } = null!;
}

/// <summary>
/// DTO para actualizar una evaluación de desempeño
/// </summary>
public class UpdateEvaluacionDesempenoDto
{
    [Required(ErrorMessage = "El ID es obligatorio")]
    public int Id { get; set; }

    [Required(ErrorMessage = "La cédula es obligatoria")]
    [StringLength(10, ErrorMessage = "La cédula debe tener máximo 10 caracteres")]
    public string Cedula { get; set; } = string.Empty;

    [Required(ErrorMessage = "El período académico es obligatorio")]
    [StringLength(10, ErrorMessage = "El período académico debe tener máximo 10 caracteres")]
    public string PeriodoAcademico { get; set; } = string.Empty;

    [Required(ErrorMessage = "El año es obligatorio")]
    [Range(2020, 2030, ErrorMessage = "El año debe estar entre 2020 y 2030")]
    public int Anio { get; set; }

    [Required(ErrorMessage = "El semestre es obligatorio")]
    [Range(1, 2, ErrorMessage = "El semestre debe ser 1 o 2")]
    public int Semestre { get; set; }

    [Required(ErrorMessage = "El puntaje obtenido es obligatorio")]
    [Range(0, 100, ErrorMessage = "El puntaje debe estar entre 0 y 100")]
    public decimal PuntajeObtenido { get; set; }

    [Range(0, 100, ErrorMessage = "El puntaje máximo debe estar entre 0 y 100")]
    public decimal PuntajeMaximo { get; set; } = 100;

    [Required(ErrorMessage = "La fecha de evaluación es obligatoria")]
    public DateTime FechaEvaluacion { get; set; }

    [StringLength(50, ErrorMessage = "El tipo de evaluación debe tener máximo 50 caracteres")]
    public string TipoEvaluacion { get; set; } = "Integral";

    [StringLength(500, ErrorMessage = "Las observaciones deben tener máximo 500 caracteres")]
    public string? Observaciones { get; set; }

    [StringLength(20, ErrorMessage = "El estado debe tener máximo 20 caracteres")]
    public string Estado { get; set; } = "Completada";

    [StringLength(100, ErrorMessage = "El evaluador debe tener máximo 100 caracteres")]
    public string? Evaluador { get; set; }
}

/// <summary>
/// DTO para actualizar evaluación con archivo PDF
/// </summary>
public class UpdateEvaluacionWithPdfDto : UpdateEvaluacionDesempenoDto
{
    [Required(ErrorMessage = "El archivo PDF es obligatorio")]
    public IFormFile ArchivoPdf { get; set; } = null!;
}

/// <summary>
/// DTO para mostrar resumen de evaluaciones de un docente
/// </summary>
public class ResumenEvaluacionesDto
{
    public string Cedula { get; set; } = string.Empty;
    public int TotalEvaluaciones { get; set; }
    public int EvaluacionesAprobadas { get; set; }
    public decimal PorcentajeAprobacion { get; set; }
    public decimal PromedioGeneral { get; set; }
    public bool CumpleRequisito75Porciento { get; set; }
    public List<EvaluacionDesempenoDto> UltimasCuatroEvaluaciones { get; set; } = new();
}

/// <summary>
/// DTO para verificar cumplimiento del requisito del 75%
/// </summary>
public class VerificacionRequisito75Dto
{
    public string Cedula { get; set; } = string.Empty;
    public bool CumpleRequisito { get; set; }
    public int EvaluacionesAnalizadas { get; set; }
    public int EvaluacionesQueAlcanzan75 { get; set; }
    public decimal PorcentajePromedioUltimasCuatro { get; set; }
    public string Mensaje { get; set; } = string.Empty;
    public List<EvaluacionDesempenoDto> EvaluacionesConsideradas { get; set; } = new();
}
