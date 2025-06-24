using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ProyectoAgiles.Application.DTOs;

public class InvestigacionDto
{
    public int Id { get; set; }
    public string Cedula { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string RevistaOEditorial { get; set; } = string.Empty;
    public DateTime FechaPublicacion { get; set; }
    public string CampoConocimiento { get; set; } = string.Empty;
    public string Filiacion { get; set; } = string.Empty;
    public string Observacion { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool TienePdf { get; set; } // Asegura que esté presente y pública
}

public class CreateInvestigacionDto
{
    [Required(ErrorMessage = "La cédula es requerida")]
    [StringLength(10, ErrorMessage = "La cédula debe tener máximo 10 caracteres")]
    public string Cedula { get; set; } = string.Empty;

    [Required(ErrorMessage = "El título es requerido")]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El tipo es requerido")]
    public string Tipo { get; set; } = string.Empty;

    [Required(ErrorMessage = "La revista o editorial es requerida")]
    public string RevistaOEditorial { get; set; } = string.Empty;

    [Required(ErrorMessage = "La fecha de publicación es requerida")]
    public DateTime FechaPublicacion { get; set; }

    [Required(ErrorMessage = "El campo de conocimiento es requerido")]
    public string CampoConocimiento { get; set; } = string.Empty;

    [Required(ErrorMessage = "La filiación es requerida")]
    public string Filiacion { get; set; } = string.Empty;

    [Required(ErrorMessage = "La observación es requerida")]
    public string Observacion { get; set; } = string.Empty;
}

public class UpdateInvestigacionDto
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "La cédula es requerida")]
    [StringLength(10, ErrorMessage = "La cédula debe tener máximo 10 caracteres")]
    public string Cedula { get; set; } = string.Empty;

    [Required(ErrorMessage = "El título es requerido")]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El tipo es requerido")]
    public string Tipo { get; set; } = string.Empty;

    [Required(ErrorMessage = "La revista o editorial es requerida")]
    public string RevistaOEditorial { get; set; } = string.Empty;

    [Required(ErrorMessage = "La fecha de publicación es requerida")]
    public DateTime FechaPublicacion { get; set; }

    [Required(ErrorMessage = "El campo de conocimiento es requerido")]
    public string CampoConocimiento { get; set; } = string.Empty;

    [Required(ErrorMessage = "La filiación es requerida")]
    public string Filiacion { get; set; } = string.Empty;    [Required(ErrorMessage = "La observación es requerida")]
    public string Observacion { get; set; } = string.Empty;
}

public class UpdateInvestigacionWithPdfDto
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "La cédula es requerida")]
    [StringLength(10, ErrorMessage = "La cédula debe tener máximo 10 caracteres")]
    public string Cedula { get; set; } = string.Empty;

    [Required(ErrorMessage = "El título es requerido")]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El tipo es requerido")]
    public string Tipo { get; set; } = string.Empty;

    [Required(ErrorMessage = "La revista o editorial es requerida")]
    public string RevistaOEditorial { get; set; } = string.Empty;

    [Required(ErrorMessage = "La fecha de publicación es requerida")]
    public DateTime FechaPublicacion { get; set; }

    [Required(ErrorMessage = "El campo de conocimiento es requerido")]
    public string CampoConocimiento { get; set; } = string.Empty;

    [Required(ErrorMessage = "La filiación es requerida")]
    public string Filiacion { get; set; } = string.Empty;

    [Required(ErrorMessage = "La observación es requerida")]
    public string Observacion { get; set; } = string.Empty;
    
    public IFormFile? ArchivoPdf { get; set; }
}

public class CreateInvestigacionWithPdfDto
{
    [Required]
    [StringLength(10)]
    public string Cedula { get; set; } = string.Empty;
    [Required]
    public string Titulo { get; set; } = string.Empty;
    [Required]
    public string Tipo { get; set; } = string.Empty;
    [Required]
    public string RevistaOEditorial { get; set; } = string.Empty;
    [Required]
    public DateTime FechaPublicacion { get; set; }
    [Required]
    public string CampoConocimiento { get; set; } = string.Empty;
    [Required]
    public string Filiacion { get; set; } = string.Empty;
    [Required]
    public string Observacion { get; set; } = string.Empty;
    public IFormFile? ArchivoPdf { get; set; }
}
