using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ProyectoAgiles.Application.DTOs;

/// <summary>
/// DTO para mostrar información de capacitación DITIC
/// </summary>
public class DiticDto
{
    public int Id { get; set; }
    public string Cedula { get; set; } = string.Empty;
    public string NombreCapacitacion { get; set; } = string.Empty;
    public string Institucion { get; set; } = string.Empty;
    public string TipoCapacitacion { get; set; } = string.Empty;
    public string Modalidad { get; set; } = string.Empty;
    public int HorasAcademicas { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public int Anio { get; set; }
    public string Estado { get; set; } = string.Empty;
    public decimal? Calificacion { get; set; }
    public decimal CalificacionMinima { get; set; }
    public bool Aprobada { get; set; }
    public bool EsPedagogica { get; set; }
    public string? Descripcion { get; set; }
    public string? NumeroCertificado { get; set; }
    public string? Instructor { get; set; }
    public string? Observaciones { get; set; }
    public string? NombreArchivoCertificado { get; set; }
    public bool ExencionPorAutoridad { get; set; }
    public string? CargoAutoridad { get; set; }
    public DateTime? FechaInicioAutoridad { get; set; }
    public DateTime? FechaFinAutoridad { get; set; }
    public decimal AñosComoAutoridad { get; set; }
    public bool CumpleExencionAutoridad { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// DTO para crear una nueva capacitación DITIC
/// </summary>
public class CreateDiticDto
{
    [Required(ErrorMessage = "La cédula es obligatoria")]
    [StringLength(10, ErrorMessage = "La cédula debe tener máximo 10 caracteres")]
    public string Cedula { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre de la capacitación es obligatorio")]
    [StringLength(500, ErrorMessage = "El nombre debe tener máximo 500 caracteres")]
    public string NombreCapacitacion { get; set; } = string.Empty;

    [Required(ErrorMessage = "La institución es obligatoria")]
    [StringLength(300, ErrorMessage = "La institución debe tener máximo 300 caracteres")]
    public string Institucion { get; set; } = string.Empty;

    [Required(ErrorMessage = "El tipo de capacitación es obligatorio")]
    [StringLength(50, ErrorMessage = "El tipo debe tener máximo 50 caracteres")]
    public string TipoCapacitacion { get; set; } = string.Empty;

    [StringLength(30, ErrorMessage = "La modalidad debe tener máximo 30 caracteres")]
    public string Modalidad { get; set; } = "Presencial";

    [Required(ErrorMessage = "Las horas académicas son obligatorias")]
    [Range(1, 1000, ErrorMessage = "Las horas deben estar entre 1 y 1000")]
    public int HorasAcademicas { get; set; }

    [Required(ErrorMessage = "La fecha de inicio es obligatoria")]
    public DateTime FechaInicio { get; set; }

    [Required(ErrorMessage = "La fecha de fin es obligatoria")]
    public DateTime FechaFin { get; set; }

    [Required(ErrorMessage = "El año es obligatorio")]
    [Range(2020, 2030, ErrorMessage = "El año debe estar entre 2020 y 2030")]
    public int Anio { get; set; }

    [StringLength(20, ErrorMessage = "El estado debe tener máximo 20 caracteres")]
    public string Estado { get; set; } = "Completada";

    [Range(0, 100, ErrorMessage = "La calificación debe estar entre 0 y 100")]
    public decimal? Calificacion { get; set; }

    [Range(0, 100, ErrorMessage = "La calificación mínima debe estar entre 0 y 100")]
    public decimal CalificacionMinima { get; set; } = 70;

    [StringLength(1000, ErrorMessage = "La descripción debe tener máximo 1000 caracteres")]
    public string? Descripcion { get; set; }

    [StringLength(100, ErrorMessage = "El número de certificado debe tener máximo 100 caracteres")]
    public string? NumeroCertificado { get; set; }

    [StringLength(200, ErrorMessage = "El instructor debe tener máximo 200 caracteres")]
    public string? Instructor { get; set; }

    [StringLength(500, ErrorMessage = "Las observaciones deben tener máximo 500 caracteres")]
    public string? Observaciones { get; set; }

    public bool ExencionPorAutoridad { get; set; } = false;

    [StringLength(100, ErrorMessage = "El cargo debe tener máximo 100 caracteres")]
    public string? CargoAutoridad { get; set; }

    public DateTime? FechaInicioAutoridad { get; set; }

    public DateTime? FechaFinAutoridad { get; set; }
}

/// <summary>
/// DTO para crear capacitación con certificado PDF
/// </summary>
public class CreateDiticWithPdfDto : CreateDiticDto
{
    public IFormFile? ArchivoCertificado { get; set; }
}

/// <summary>
/// DTO para actualizar una capacitación DITIC
/// </summary>
public class UpdateDiticDto : CreateDiticDto
{
    [Required(ErrorMessage = "El ID es obligatorio")]
    public int Id { get; set; }
}

/// <summary>
/// DTO para mostrar resumen de capacitaciones de un docente
/// </summary>
public class ResumenCapacitacionesDto
{
    public string Cedula { get; set; } = string.Empty;
    public int TotalCapacitaciones { get; set; }
    public int CapacitacionesAprobadas { get; set; }
    public int HorasTotales { get; set; }
    public int HorasPedagogicas { get; set; }
    public int HorasUltimosTresAnios { get; set; }
    public int HorasPedagogicasUltimosTresAnios { get; set; }
    public bool CumpleRequisitoHoras { get; set; } // >= 96 horas en 3 años
    public bool CumpleRequisitoPedagogico { get; set; } // >= 24 horas pedagógicas (25%)
    public bool CumpleExencionAutoridad { get; set; }
    public bool CumpleRequisito { get; set; } // Cumple requisito general
    public decimal PorcentajePedagogico { get; set; }
    public string MensajeEstado { get; set; } = string.Empty;
    public List<DiticDto> CapacitacionesUltimosTresAnios { get; set; } = new();
}

/// <summary>
/// DTO para verificar cumplimiento del requisito DITIC
/// </summary>
public class VerificacionRequisitoDiticDto
{
    public string Cedula { get; set; } = string.Empty;
    public bool CumpleRequisito { get; set; }
    public bool CumpleHorasTotales { get; set; }
    public bool CumpleHorasPedagogicas { get; set; }
    public bool TieneExencionAutoridad { get; set; }
    public int HorasRequeridas { get; set; } = 96;
    public int HorasPedagogicasRequeridas { get; set; } = 24;
    public int HorasObtenidas { get; set; }
    public int HorasPedagogicasObtenidas { get; set; }
    public decimal PorcentajePedagogico { get; set; }
    public int CapacitacionesAnalizadas { get; set; }
    public string MensajeDetallado { get; set; } = string.Empty;
    public string? CargoAutoridad { get; set; }
    public decimal? AñosComoAutoridad { get; set; }
    public List<DiticDto> CapacitacionesConsideradas { get; set; } = new();
}

/// <summary>
/// DTO para estadísticas de capacitaciones por tipo
/// </summary>
public class EstadisticasCapacitacionDto
{
    public string TipoCapacitacion { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public int HorasTotales { get; set; }
    public decimal PorcentajeDelTotal { get; set; }
}
