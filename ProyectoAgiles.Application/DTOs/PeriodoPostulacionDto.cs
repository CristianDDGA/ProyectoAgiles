using System.ComponentModel.DataAnnotations;

namespace ProyectoAgiles.Application.DTOs;

public class PeriodoPostulacionDto
{
    public int Id { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public bool Activo { get; set; }
    public string? Descripcion { get; set; }
    public DateTime FechaCreacion { get; set; }
    public bool EstaActivo { get; set; }
    public bool EstaVigente { get; set; }
    public int DiasRestantes { get; set; }
    public string EstadoPeriodo { get; set; } = string.Empty;
    public int TotalSolicitudes { get; set; }
}

public class CreatePeriodoPostulacionDto
{
    [Required(ErrorMessage = "La fecha de inicio es requerida")]
    public DateTime FechaInicio { get; set; }

    [Required(ErrorMessage = "La fecha de fin es requerida")]
    public DateTime FechaFin { get; set; }

    [MaxLength(500, ErrorMessage = "La descripción no puede tener más de 500 caracteres")]
    public string? Descripcion { get; set; }

    public bool ActivarInmediatamente { get; set; } = false;
}

public class UpdatePeriodoPostulacionDto
{
    [Required(ErrorMessage = "La fecha de inicio es requerida")]
    public DateTime FechaInicio { get; set; }

    [Required(ErrorMessage = "La fecha de fin es requerida")]
    public DateTime FechaFin { get; set; }

    [MaxLength(500, ErrorMessage = "La descripción no puede tener más de 500 caracteres")]
    public string? Descripcion { get; set; }
}

public class PeriodoInfoDto
{
    public bool HayPeriodoActivo { get; set; }
    public string Mensaje { get; set; } = string.Empty;
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public int DiasRestantes { get; set; }
    public string EstadoPeriodo { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
}
