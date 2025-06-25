using ProyectoAgiles.Application.DTOs;

namespace proyectoAgiles.Services;

/// <summary>
/// DTO para verificación de requisitos de escalafón dinámicos según el nivel del docente
/// </summary>
public class VerificacionRequisitosEscalafonDto
{
    /// <summary>
    /// Cédula del docente
    /// </summary>
    public string Cedula { get; set; } = string.Empty;

    /// <summary>
    /// Nivel académico actual del docente
    /// </summary>
    public string NivelActual { get; set; } = string.Empty;

    /// <summary>
    /// Nivel académico objetivo (al que aspira)
    /// </summary>
    public string NivelObjetivo { get; set; } = string.Empty;

    /// <summary>
    /// Indica si cumple con todos los requisitos
    /// </summary>
    public bool CumpleTodosRequisitos { get; set; }

    /// <summary>
    /// Mensaje general del resultado de la verificación
    /// </summary>
    public string Mensaje { get; set; } = string.Empty;

    /// <summary>
    /// Verificación del requisito de experiencia
    /// </summary>
    public RequisitoCumplimientoDto Experiencia { get; set; } = new();

    /// <summary>
    /// Verificación del requisito de obras relevantes
    /// </summary>
    public RequisitoCumplimientoDto ObrasRelevantes { get; set; } = new();

    /// <summary>
    /// Verificación del requisito de evaluación de desempeño
    /// </summary>
    public RequisitoCumplimientoDto EvaluacionDesempeno { get; set; } = new();

    /// <summary>
    /// Verificación del requisito de capacitación
    /// </summary>
    public RequisitoCumplimientoDto Capacitacion { get; set; } = new();

    /// <summary>
    /// Verificación del requisito de proyectos de investigación (si aplica)
    /// </summary>
    public RequisitoCumplimientoDto? ProyectosInvestigacion { get; set; }

    /// <summary>
    /// Fecha en que se realizó la verificación
    /// </summary>
    public DateTime FechaVerificacion { get; set; } = DateTime.Now;

    /// <summary>
    /// Configuración de requisitos utilizada para la verificación
    /// </summary>
    public RequisitoEscalafonConfigDto? ConfiguracionRequisitos { get; set; }

    /// <summary>
    /// Lista de requisitos que no se cumplen
    /// </summary>
    public List<string> RequisitosIncumplidos => GetRequisitosIncumplidos();

    /// <summary>
    /// Porcentaje de cumplimiento general (0-100)
    /// </summary>
    public decimal PorcentajeCumplimiento => CalcularPorcentajeCumplimiento();

    private List<string> GetRequisitosIncumplidos()
    {
        var incumplidos = new List<string>();
        
        if (!Experiencia.Cumple) incumplidos.Add("Experiencia mínima");
        if (!ObrasRelevantes.Cumple) incumplidos.Add("Obras relevantes");
        if (!EvaluacionDesempeno.Cumple) incumplidos.Add("Evaluación de desempeño");
        if (!Capacitacion.Cumple) incumplidos.Add("Capacitación");
        if (ProyectosInvestigacion != null && !ProyectosInvestigacion.Cumple) 
            incumplidos.Add("Proyectos de investigación");
        
        return incumplidos;
    }

    private decimal CalcularPorcentajeCumplimiento()
    {
        var totalRequisitos = 4; // Experiencia, Obras, Evaluación, Capacitación
        var cumplidos = 0;

        if (Experiencia.Cumple) cumplidos++;
        if (ObrasRelevantes.Cumple) cumplidos++;
        if (EvaluacionDesempeno.Cumple) cumplidos++;
        if (Capacitacion.Cumple) cumplidos++;

        // Si se requieren proyectos de investigación, agregarlos al total
        if (ProyectosInvestigacion != null)
        {
            totalRequisitos++;
            if (ProyectosInvestigacion.Cumple) cumplidos++;
        }

        return totalRequisitos > 0 ? (decimal)cumplidos / totalRequisitos * 100 : 0;
    }
}
