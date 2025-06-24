namespace ProyectoAgiles.Application.DTOs;

public class SolicitudEscalafonDto
{
    public int Id { get; set; }
    public string DocenteCedula { get; set; } = string.Empty;
    public string DocenteNombre { get; set; } = string.Empty;
    public string DocenteEmail { get; set; } = string.Empty;
    public string? DocenteTelefono { get; set; }
    public string? Facultad { get; set; }
    public string? Carrera { get; set; }
    public string NivelActual { get; set; } = string.Empty;
    public string NivelSolicitado { get; set; } = string.Empty;
    public int AnosExperiencia { get; set; }
    public string? Titulos { get; set; }
    public string? Publicaciones { get; set; }
    public string? ProyectosInvestigacion { get; set; }
    public string? Capacitaciones { get; set; }
    public DateTime FechaSolicitud { get; set; }
    public DateTime? FechaAprobacion { get; set; }
    public DateTime? FechaRechazo { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Observaciones { get; set; }
    public string? MotivoRechazo { get; set; }
    public string? ProcesadoPor { get; set; }
}

public class CreateSolicitudEscalafonDto
{
    public string DocenteCedula { get; set; } = string.Empty;
    public string DocenteNombre { get; set; } = string.Empty;
    public string DocenteEmail { get; set; } = string.Empty;
    public string? DocenteTelefono { get; set; }
    public string? Facultad { get; set; }
    public string? Carrera { get; set; }
    public string NivelActual { get; set; } = string.Empty;
    public string NivelSolicitado { get; set; } = string.Empty;
    public int AnosExperiencia { get; set; }
    public string? Titulos { get; set; }
    public string? Publicaciones { get; set; }
    public string? ProyectosInvestigacion { get; set; }
    public string? Capacitaciones { get; set; }
    public string? Observaciones { get; set; }
}

public class UpdateSolicitudStatusDto
{
    public int Id { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? MotivoRechazo { get; set; }
    public string? ProcesadoPor { get; set; }
}
