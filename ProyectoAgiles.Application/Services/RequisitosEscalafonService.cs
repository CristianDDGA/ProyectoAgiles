using ProyectoAgiles.Application.DTOs;
using ProyectoAgiles.Application.Interfaces;

namespace ProyectoAgiles.Application.Services;

/// <summary>
/// Servicio para gestionar los requisitos de escalafón según el nivel docente
/// </summary>
public class RequisitosEscalafonService : IRequisitosEscalafonService
{
    /// <summary>
    /// Obtiene los requisitos para ascender al siguiente nivel según el nivel actual del docente
    /// </summary>
    /// <param name="nivelActual">Nivel académico actual del docente</param>
    /// <returns>Configuración de requisitos para el siguiente nivel</returns>
    public RequisitoEscalafonConfigDto GetRequisitosParaNivel(string nivelActual)
    {
        var nivelNormalizado = nivelActual.ToLower().Trim();
        
        return nivelNormalizado switch
        {
            // De Titular Auxiliar 1 a Titular Auxiliar 2
            var nivel when nivel.Contains("titular auxiliar 1") || nivel.Contains("auxiliar 1") => 
                new RequisitoEscalafonConfigDto
                {
                    NivelActual = "Titular Auxiliar 1",
                    NivelObjetivo = "Titular Auxiliar 2",
                    AnosExperienciaRequeridos = 4,
                    ObrasRelevantesMinimoTotal = 1,
                    ObrasRelevantesConUTA = 1,
                    PorcentajeEvaluacionMinimo = 75,
                    PeriodosEvaluacionRequeridos = 4,
                    HorasCapacitacionRequeridas = 96,
                    HorasCapacitacionPedagogicas = 24,
                    MesesProyectosInvestigacion = 0, // No requerido para este nivel
                    RequiereProyectosInvestigacion = false,
                    DescripcionExperiencia = "Experiencia mínima de 4 años como personal académico titular auxiliar 1 en la práctica docente en la Universidad Técnica de Ambato.",
                    DescripcionObras = "Haber creado o publicado una obra de relevancia o un artículo indexado, con filiación UTA, en el campo amplio de conocimiento vinculado a sus actividades de docencia o investigación. La publicación deberá ser realizada durante el ejercicio de sus actividades en el grado que se encuentra.",
                    DescripcionEvaluacion = "Haber obtenido como mínimo el setenta y cinco por ciento (75%) del puntaje de la evaluación integral en los últimos cuatro periodos académicos de evaluación de desempeño en los que ejerció la docencia.",
                    DescripcionCapacitacion = "Haber realizado y aprobado noventa y seis horas de capacitación y actualización profesional y científica en los últimos tres (3) años en una Universidad o Escuela Politécnica acreditada. Al menos 24 Horas (25%) de estas deberán ser de actualización pedagógica. El cumplimiento de este literal se exceptúa si se ha desempeñado como autoridad en una Universidad o Escuela Politécnica por más de dos (2) años.",
                    DescripcionProyectos = "" // No aplica para este nivel
                },
            
            // De Titular Auxiliar 2 a Titular Agregado 1
            var nivel when nivel.Contains("titular auxiliar 2") || nivel.Contains("auxiliar 2") => 
                new RequisitoEscalafonConfigDto
                {
                    NivelActual = "Titular Auxiliar 2",
                    NivelObjetivo = "Titular Agregado 1",
                    AnosExperienciaRequeridos = 4,
                    ObrasRelevantesMinimoTotal = 2,
                    ObrasRelevantesConUTA = 2,
                    PorcentajeEvaluacionMinimo = 75,
                    PeriodosEvaluacionRequeridos = 4,
                    HorasCapacitacionRequeridas = 96,
                    HorasCapacitacionPedagogicas = 24,
                    MesesProyectosInvestigacion = 12,
                    RequiereProyectosInvestigacion = true,
                    DescripcionExperiencia = "Experiencia mínima de cuatro años como personal académico titular auxiliar 2 en la Universidad Técnica de Ambato.",
                    DescripcionObras = "Haber creado o publicado al menos dos (2) obras de relevancia o artículos indexados, con filiación UTA, en el campo amplio de conocimiento vinculado a sus actividades de docencia o investigación. Al menos una de las publicaciones deberá ser realizada durante el ejercicio de sus actividades en el grado que se encuentra.",
                    DescripcionEvaluacion = "Haber obtenido como mínimo el setenta y cinco por ciento (75%) del puntaje de la evaluación integral en los últimos cuatro periodos académicos de evaluación de desempeño en los que ejerció la docencia.",
                    DescripcionCapacitacion = "Haber realizado y aprobado noventa y seis horas (96) de capacitación y actualización profesional y científica en los últimos tres (3) años en una Universidad o Escuela Politécnica acreditada. Al menos 24 Horas (25%) de estas deberán ser de actualización pedagógica. El cumplimiento de este literal se exceptúa si se ha desempeñado como autoridad en una Universidad o Escuela Politécnica por más de dos (2) años.",
                    DescripcionProyectos = "Haber participado al menos doce (12) meses, en proyectos de investigación y/o vinculación con la sociedad, con filiación/aval UTA, durante el ejercicio de sus actividades en el grado que se encuentra."
                },
            
            // De Titular Agregado 1 a Titular Agregado 2
            var nivel when nivel.Contains("titular agregado 1") || nivel.Contains("agregado 1") => 
                new RequisitoEscalafonConfigDto
                {
                    NivelActual = "Titular Agregado 1",
                    NivelObjetivo = "Titular Agregado 2",
                    AnosExperienciaRequeridos = 4,
                    ObrasRelevantesMinimoTotal = 3,
                    ObrasRelevantesConUTA = 3,
                    PorcentajeEvaluacionMinimo = 75,
                    PeriodosEvaluacionRequeridos = 4,
                    HorasCapacitacionRequeridas = 128,
                    HorasCapacitacionPedagogicas = 32,
                    MesesProyectosInvestigacion = 24,
                    RequiereProyectosInvestigacion = true,
                    DescripcionExperiencia = "Experiencia mínima de cuatro años como personal académico titular agregado 1 en la Universidad Técnica de Ambato.",
                    DescripcionObras = "Haber creado o publicado al menos tres (3) obras de relevancia o artículos indexados, con filiación UTA, en el campo amplio de conocimiento vinculado a sus actividades de docencia o investigación. Al menos una de las publicaciones deberá ser realizada durante el ejercicio de sus actividades en el grado que se encuentra.",
                    DescripcionEvaluacion = "Haber obtenido como mínimo el setenta y cinco (75%) del puntaje de la evaluación integral en los últimos cuatro periodos académicos de evaluación de desempeño en los que ejerció la docencia.",
                    DescripcionCapacitacion = "Haber realizado y aprobado ciento veinte y ocho (128) horas acumuladas de capacitación y actualización profesional y científica, en los últimos tres (3) años en una Universidad o Escuela Politécnica acreditada, de las cuales al menos treinta y dos (32) horas (25%) habrán sido en actualización pedagógica. El cumplimiento de este literal se exceptúa si se ha desempeñado como autoridad en una Universidad o Escuela Politécnica por más de dos (2) años.",
                    DescripcionProyectos = "Haber participado en uno o más proyectos de investigación o vinculación, con la sociedad con filiación/aval UTA, por un total mínimo de 24 meses consecutivos o no, durante el ejercicio de sus actividades en el grado que se encuentra. El tiempo de Coordinador principal de un proyecto de investigación equivaldrá al doble de tiempo de la participación como investigador en el proyecto; y el tiempo de coordinador subrogante de un proyecto de investigación equivaldrá a 1.5 veces del tiempo de participación como investigador en el proyecto."
                },
            
            // De Titular Agregado 2 a Titular Agregado 3
            var nivel when nivel.Contains("titular agregado 2") || nivel.Contains("agregado 2") => 
                new RequisitoEscalafonConfigDto
                {
                    NivelActual = "Titular Agregado 2",
                    NivelObjetivo = "Titular Agregado 3",
                    AnosExperienciaRequeridos = 4,
                    ObrasRelevantesMinimoTotal = 5,
                    ObrasRelevantesConUTA = 5,
                    PorcentajeEvaluacionMinimo = 75,
                    PeriodosEvaluacionRequeridos = 4,
                    HorasCapacitacionRequeridas = 160,
                    HorasCapacitacionPedagogicas = 40,
                    MesesProyectosInvestigacion = 24,
                    RequiereProyectosInvestigacion = true,
                    DescripcionExperiencia = "Experiencia mínima de cuatro años como personal académico titular agregado 2 en la Universidad Técnica de Ambato.",
                    DescripcionObras = "Haber creado o publicado al menos cinco (5) obras de relevancia o artículos indexados, con filiación UTA, en el campo amplio de conocimiento vinculado a sus actividades de docencia o investigación. Al menos una de las publicaciones deberá ser realizada durante el ejercicio de sus actividades en el grado que se encuentra.",
                    DescripcionEvaluacion = "Haber obtenido como mínimo el setenta y cinco (75%) del puntaje de la evaluación integral en los últimos cuatro periodos académicos de evaluación de desempeño en los que ejerció la docencia.",
                    DescripcionCapacitacion = "Haber realizado y aprobado ciento sesenta horas (160) horas acumuladas de capacitación y actualización profesional y científica, en los últimos tres (3) años en una Universidad o Escuela Politécnica acreditada de las cuales cuarenta (40) horas (25%) habrán sido en actualización pedagógica. El cumplimiento de este literal se exceptúa si se ha desempeñado como autoridad en una Universidad o Escuela Politécnica, por más de dos (2) años.",
                    DescripcionProyectos = "Haber participado en uno o más proyectos de investigación o vinculación con la sociedad por un total mínimo de 24 meses consecutivos o no, con filiación/aval UTA, durante el ejercicio de sus actividades en el grado que se encuentra. El tiempo como coordinador principal de un proyecto de investigación equivaldrá al doble de tiempo de la participación como investigador en el proyecto; y el tiempo como coordinador subrogante de un proyecto de investigación equivaldrá a 1.5 veces del tiempo de participación como investigador en el proyecto."
                },
            
            // Nivel por defecto (si no se encuentra coincidencia)
            _ => new RequisitoEscalafonConfigDto
            {
                NivelActual = nivelActual,
                NivelObjetivo = "Nivel siguiente no definido",
                AnosExperienciaRequeridos = 4,
                ObrasRelevantesMinimoTotal = 1,
                ObrasRelevantesConUTA = 1,
                PorcentajeEvaluacionMinimo = 75,
                PeriodosEvaluacionRequeridos = 4,
                HorasCapacitacionRequeridas = 96,
                HorasCapacitacionPedagogicas = 24,
                MesesProyectosInvestigacion = 0,
                RequiereProyectosInvestigacion = false,
                DescripcionExperiencia = "Requisitos de experiencia no definidos para este nivel.",
                DescripcionObras = "Requisitos de obras no definidos para este nivel.",
                DescripcionEvaluacion = "Requisitos de evaluación no definidos para este nivel.",
                DescripcionCapacitacion = "Requisitos de capacitación no definidos para este nivel.",
                DescripcionProyectos = ""
            }
        };
    }

    /// <summary>
    /// Obtiene todos los niveles académicos disponibles en orden jerárquico
    /// </summary>
    /// <returns>Lista de niveles académicos ordenados</returns>
    public List<string> GetNivelesAcademicos()
    {
        return new List<string>
        {
            "Titular Auxiliar 1",
            "Titular Auxiliar 2",
            "Titular Agregado 1",
            "Titular Agregado 2",
            "Titular Agregado 3",
            "Titular Principal"
        };
    }

    /// <summary>
    /// Obtiene el siguiente nivel académico en la jerarquía
    /// </summary>
    /// <param name="nivelActual">Nivel académico actual</param>
    /// <returns>Siguiente nivel en la jerarquía o null si es el máximo</returns>
    public string? GetSiguienteNivel(string nivelActual)
    {
        var niveles = GetNivelesAcademicos();
        var nivelNormalizado = nivelActual.Trim();
        
        // Buscar coincidencia parcial para manejar variaciones en nombres
        var indiceActual = -1;
        for (int i = 0; i < niveles.Count; i++)
        {
            if (niveles[i].Contains(nivelNormalizado, StringComparison.OrdinalIgnoreCase) ||
                nivelNormalizado.Contains(niveles[i], StringComparison.OrdinalIgnoreCase))
            {
                indiceActual = i;
                break;
            }
        }
        
        // Retornar el siguiente nivel si existe
        return indiceActual >= 0 && indiceActual < niveles.Count - 1 
            ? niveles[indiceActual + 1] 
            : null;
    }

    /// <summary>
    /// Verifica si un nivel puede ascender al siguiente
    /// </summary>
    /// <param name="nivelActual">Nivel académico actual</param>
    /// <returns>True si puede ascender, false si es el nivel máximo</returns>
    public bool PuedeAscender(string nivelActual)
    {
        return GetSiguienteNivel(nivelActual) != null;
    }
}
