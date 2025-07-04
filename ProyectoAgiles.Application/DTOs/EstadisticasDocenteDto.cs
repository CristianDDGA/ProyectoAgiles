namespace ProyectoAgiles.Application.DTOs
{
    public class EstadisticasDocenteDto
    {
        public string DocenteNombre { get; set; } = string.Empty;
        public string DocenteCedula { get; set; } = string.Empty;
        public SeccionesEstadisticasDto Secciones { get; set; } = new SeccionesEstadisticasDto();
    }

    public class SeccionesEstadisticasDto
    {
        public SeccionEstadisticaDto Experiencia { get; set; } = new SeccionEstadisticaDto();
        public SeccionEstadisticaDto Titulacion { get; set; } = new SeccionEstadisticaDto();
        public SeccionEstadisticaDto Publicaciones { get; set; } = new SeccionEstadisticaDto();
        public SeccionEstadisticaDto Capacitacion { get; set; } = new SeccionEstadisticaDto();
        public SeccionEstadisticaDto Evaluacion { get; set; } = new SeccionEstadisticaDto();
    }

    public class SeccionEstadisticaDto
    {
        public DatosSeccionDto Datos { get; set; } = new DatosSeccionDto();
    }

    public class DatosSeccionDto
    {
        public bool Cumple { get; set; }
        public string Observaciones { get; set; } = string.Empty;
        public int? Valor { get; set; }
        public int? Requerido { get; set; }
        public string? Detalle { get; set; }
    }

    public class DocumentoAdjuntoDto
    {
        public int Id { get; set; }
        public string NombreArchivo { get; set; } = string.Empty;
        public string TipoDocumento { get; set; } = string.Empty;
        public string RutaArchivo { get; set; } = string.Empty;
        public DateTime FechaSubida { get; set; }
        public long TamanoArchivo { get; set; }
    }

    public class VerificacionRequisitosDto
    {
        public int Id { get; set; }
        public int SolicitudId { get; set; }
        public bool CumpleExperiencia { get; set; }
        public bool CumpleTitulacion { get; set; }
        public bool CumplePublicaciones { get; set; }
        public bool CumpleCapacitacion { get; set; }
        public bool CumpleEvaluacion { get; set; }
        public bool CumpleTodos { get; set; }
        public string? ObservacionesExperiencia { get; set; }
        public string? ObservacionesTitulacion { get; set; }
        public string? ObservacionesPublicaciones { get; set; }
        public string? ObservacionesCapacitacion { get; set; }
        public string? ObservacionesEvaluacion { get; set; }
        public string? VerificadoPor { get; set; }
        public DateTime? FechaVerificacion { get; set; }
    }

    public class RespuestaSolicitudDto
    {
        public int SolicitudId { get; set; }
        public bool Aprobada { get; set; }
        public string? MotivoRechazo { get; set; }
        public string? ComentariosAdicionales { get; set; }
        public string UsuarioAprobacion { get; set; } = string.Empty;
        public bool EnviarNotificacion { get; set; } = true;
    }

    public class NotificacionCorreoDto
    {
        public int SolicitudId { get; set; }
        public string DocenteEmail { get; set; } = string.Empty;
        public string DocenteNombre { get; set; } = string.Empty;
        public bool Aprobada { get; set; }
        public string? MotivoRechazo { get; set; }
        public string? ComentariosAdicionales { get; set; }
        public string NivelSolicitado { get; set; } = string.Empty;
        public DateTime FechaSolicitud { get; set; }
    }
}
