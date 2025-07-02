namespace ProyectoAgiles.Application.DTOs;

/// <summary>
/// DTO para representar los archivos utilizados en escalafones
/// </summary>
public class ArchivosUtilizadosDto
{
    public int Id { get; set; }
    public int SolicitudEscalafonId { get; set; }
    public string TipoRecurso { get; set; } = string.Empty;
    public int RecursoId { get; set; }
    public string DocenteCedula { get; set; } = string.Empty;
    public string NivelOrigen { get; set; } = string.Empty;
    public string NivelDestino { get; set; } = string.Empty;
    public DateTime FechaUtilizacion { get; set; }
    public string? Descripcion { get; set; }
    public string EstadoAscenso { get; set; } = string.Empty;
    
    // Información adicional para el frontend
    public string? TituloRecurso { get; set; }
    public string? DetallesRecurso { get; set; }
}

/// <summary>
/// DTO para el resumen de archivos utilizados por un docente
/// </summary>
public class ResumenArchivosUtilizadosDto
{
    public string DocenteCedula { get; set; } = string.Empty;
    public string DocenteNombre { get; set; } = string.Empty;
    public int TotalInvestigacionesUtilizadas { get; set; }
    public int TotalEvaluacionesUtilizadas { get; set; }
    public int TotalCapacitacionesUtilizadas { get; set; }
    public int TotalAscensosCompletados { get; set; }
    public List<ArchivosUtilizadosDto> HistorialCompleto { get; set; } = new();
    public Dictionary<string, int> EstadisticasPorTipo { get; set; } = new();
}
