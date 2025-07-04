namespace proyectoAgiles.Services;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }
}

public class PeriodoPostulacionDto
{
    public int Id { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public bool Activo { get; set; }
    public string? Descripcion { get; set; }
    public DateTime FechaCreacion { get; set; }
    public string EstadoPeriodo { get; set; } = string.Empty;
    public int DiasRestantes { get; set; }
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
    
    // Propiedades adicionales para compatibilidad con el UI existente
    public PeriodoPostulacionDto? PeriodoActivo { get; set; }
    public bool PuedeCrearSolicitud => HayPeriodoActivo;
}

public class PeriodoInfoFrontendDto
{
    public PeriodoPostulacionDto? PeriodoActivo { get; set; }
    public bool PuedeCrearSolicitud { get; set; }
    public string Mensaje { get; set; } = string.Empty;
    public int DiasRestantes { get; set; }
    public bool HayPeriodoActivo { get; set; }
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public string EstadoPeriodo { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
}

public class CreatePeriodoPostulacionDto
{
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public string? Descripcion { get; set; }
    public bool ActivarInmediatamente { get; set; } = false;
}

public class UpdatePeriodoPostulacionDto
{
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public string? Descripcion { get; set; }
    // Nota: El backend no soporta cambiar el estado activo en Update
    // Se debe usar los endpoints específicos de activar/desactivar
}
