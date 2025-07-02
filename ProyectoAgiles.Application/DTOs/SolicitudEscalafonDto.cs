using Microsoft.AspNetCore.Http;

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
    public DateTime? FechaEnvioConsejo { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Observaciones { get; set; }
    public string? ObservacionesConsejo { get; set; }
    public string? MotivoRechazo { get; set; }
    public string? MotivoRechazoConsejo { get; set; }
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

public class HistorialEscalafonDto
{
    public int Id { get; set; }
    public string NivelAnterior { get; set; } = string.Empty;
    public string NivelNuevo { get; set; } = string.Empty;
    public DateTime FechaPromocion { get; set; }
    public string EstadoSolicitud { get; set; } = string.Empty;
    public List<string> DocumentosUtilizados { get; set; } = new();
    public DocumentosDetallados DocumentosDetalles { get; set; } = new();
    public string ObservacionesFinales { get; set; } = string.Empty;
    public string AprobadoPor { get; set; } = string.Empty;
}

public class DocumentosDetallados
{
    public List<InvestigacionUtilizada> Investigaciones { get; set; } = new();
    public List<EvaluacionUtilizada> Evaluaciones { get; set; } = new();
    public List<CapacitacionUtilizada> Capacitaciones { get; set; } = new();
    public VerificacionRequisitos VerificacionRequisitos { get; set; } = new();
}

public class InvestigacionUtilizada
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string RevistaOEditorial { get; set; } = string.Empty;
    public DateTime FechaPublicacion { get; set; }
    public string Filiacion { get; set; } = string.Empty;
    public bool TieneFiliacionUTA { get; set; }
}

public class EvaluacionUtilizada
{
    public int Id { get; set; }
    public string PeriodoAcademico { get; set; } = string.Empty;
    public int Anio { get; set; }
    public int Semestre { get; set; }
    public decimal PuntajeObtenido { get; set; }
    public decimal PuntajeMaximo { get; set; }
    public decimal Porcentaje { get; set; }
    public string Estado { get; set; } = string.Empty;
}

public class CapacitacionUtilizada
{
    public int Id { get; set; }
    public string NombreCurso { get; set; } = string.Empty;
    public string Facilitador { get; set; } = string.Empty;
    public int HorasAcademicas { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public bool EsPedagogica { get; set; }
}

public class VerificacionRequisitos
{
    public int TotalInvestigaciones { get; set; }
    public int InvestigacionesConUTA { get; set; }
    public int TotalHorasCapacitacion { get; set; }
    public int HorasPedagogicas { get; set; }
    public decimal PromedioEvaluaciones { get; set; }
    public int PeriodosEvaluados { get; set; }
    public bool CumpleTodosRequisitos { get; set; }
}

public class RechazarSolicitudDto
{
    public string MotivoRechazo { get; set; } = string.Empty;
    public string RechazadoPor { get; set; } = string.Empty;
    public string NivelRechazo { get; set; } = string.Empty;
}

public class CrearApelacionDto
{
    public string ObservacionesApelacion { get; set; } = string.Empty;
    public string Destinatario { get; set; } = string.Empty;
    public List<IFormFile>? Archivos { get; set; }
}

public class AceptarApelacionDto
{
    public string AceptadoPor { get; set; } = string.Empty;
    public string ObservacionesAceptacion { get; set; } = string.Empty;
}

public class RechazarApelacionDto
{
    public string RechazadoPor { get; set; } = string.Empty;
    public string MotivoRechazoApelacion { get; set; } = string.Empty;
}
