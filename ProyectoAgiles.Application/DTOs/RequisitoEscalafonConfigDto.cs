namespace ProyectoAgiles.Application.DTOs;

/// <summary>
/// DTO que define la configuración de requisitos para un nivel de escalafón específico
/// </summary>
public class RequisitoEscalafonConfigDto
{
    /// <summary>
    /// Nivel académico actual del docente
    /// </summary>
    public string NivelActual { get; set; } = string.Empty;

    /// <summary>
    /// Nivel académico al que aspira ascender
    /// </summary>
    public string NivelObjetivo { get; set; } = string.Empty;

    /// <summary>
    /// Años de experiencia mínimos requeridos en el nivel actual
    /// </summary>
    public int AnosExperienciaRequeridos { get; set; }

    /// <summary>
    /// Número mínimo total de obras de relevancia o artículos indexados
    /// </summary>
    public int ObrasRelevantesMinimoTotal { get; set; }

    /// <summary>
    /// Número mínimo de obras con filiación UTA
    /// </summary>
    public int ObrasRelevantesConUTA { get; set; }

    /// <summary>
    /// Porcentaje mínimo requerido en evaluaciones de desempeño
    /// </summary>
    public int PorcentajeEvaluacionMinimo { get; set; }

    /// <summary>
    /// Número de períodos académicos de evaluación a considerar
    /// </summary>
    public int PeriodosEvaluacionRequeridos { get; set; }

    /// <summary>
    /// Horas totales de capacitación requeridas
    /// </summary>
    public int HorasCapacitacionRequeridas { get; set; }

    /// <summary>
    /// Horas mínimas de capacitación pedagógica
    /// </summary>
    public int HorasCapacitacionPedagogicas { get; set; }

    /// <summary>
    /// Meses mínimos de participación en proyectos de investigación/vinculación
    /// </summary>
    public int MesesProyectosInvestigacion { get; set; }

    /// <summary>
    /// Indica si se requiere participación en proyectos de investigación
    /// </summary>
    public bool RequiereProyectosInvestigacion { get; set; }

    /// <summary>
    /// Descripción detallada del requisito de experiencia
    /// </summary>
    public string DescripcionExperiencia { get; set; } = string.Empty;

    /// <summary>
    /// Descripción detallada del requisito de obras relevantes
    /// </summary>
    public string DescripcionObras { get; set; } = string.Empty;

    /// <summary>
    /// Descripción detallada del requisito de evaluación
    /// </summary>
    public string DescripcionEvaluacion { get; set; } = string.Empty;

    /// <summary>
    /// Descripción detallada del requisito de capacitación
    /// </summary>
    public string DescripcionCapacitacion { get; set; } = string.Empty;

    /// <summary>
    /// Descripción detallada del requisito de proyectos de investigación
    /// </summary>
    public string DescripcionProyectos { get; set; } = string.Empty;

    /// <summary>
    /// Porcentaje mínimo de horas pedagógicas sobre el total
    /// </summary>
    public decimal PorcentajePedagogicoMinimo => HorasCapacitacionRequeridas > 0 
        ? (decimal)HorasCapacitacionPedagogicas / HorasCapacitacionRequeridas * 100 
        : 0;

    /// <summary>
    /// Resumen de los requisitos principales
    /// </summary>
    public string ResumenRequisitos => $"Para ascender de {NivelActual} a {NivelObjetivo}: " +
        $"{AnosExperienciaRequeridos} años experiencia, " +
        $"{ObrasRelevantesConUTA} obra(s) con UTA, " +
        $"{PorcentajeEvaluacionMinimo}% evaluación, " +
        $"{HorasCapacitacionRequeridas}h capacitación" +
        (RequiereProyectosInvestigacion ? $", {MesesProyectosInvestigacion} meses investigación" : "");
}
