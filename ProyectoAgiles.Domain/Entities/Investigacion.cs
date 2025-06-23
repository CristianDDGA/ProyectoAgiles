using System.ComponentModel.DataAnnotations;

namespace ProyectoAgiles.Domain.Entities;

public class Investigacion : BaseEntity
{
    [Required]
    [MaxLength(10)]
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

    // Nuevo campo para almacenar el PDF
    public byte[]? ArchivoPdf { get; set; }
}
