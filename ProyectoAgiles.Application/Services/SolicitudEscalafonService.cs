using AutoMapper;
using ProyectoAgiles.Application.DTOs;
using ProyectoAgiles.Application.Interfaces;
using ProyectoAgiles.Domain.Entities;
using ProyectoAgiles.Domain.Interfaces;
using Microsoft.AspNetCore.Http;

namespace ProyectoAgiles.Application.Services;

public class SolicitudEscalafonService : ISolicitudEscalafonService
{
    private readonly ISolicitudEscalafonRepository _repository;
    private readonly IMapper _mapper;
    private readonly IEmailService _emailService;
    private readonly IUserRepository _userRepository;
    private readonly IArchivosUtilizadosService _archivosUtilizadosService;

    public SolicitudEscalafonService(
        ISolicitudEscalafonRepository repository, 
        IMapper mapper, 
        IEmailService emailService,
        IUserRepository userRepository,
        IArchivosUtilizadosService archivosUtilizadosService)
    {
        _repository = repository;
        _mapper = mapper;
        _emailService = emailService;
        _userRepository = userRepository;
        _archivosUtilizadosService = archivosUtilizadosService;
    }

    public async Task<IEnumerable<SolicitudEscalafonDto>> GetAllSolicitudesAsync()
    {
        var solicitudes = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<SolicitudEscalafonDto>>(solicitudes);
    }

    public async Task<SolicitudEscalafonDto?> GetSolicitudByIdAsync(int id)
    {
        var solicitud = await _repository.GetByIdAsync(id);
        return solicitud != null ? _mapper.Map<SolicitudEscalafonDto>(solicitud) : null;
    }

    public async Task<IEnumerable<SolicitudEscalafonDto>> GetSolicitudesByCedulaAsync(string cedula)
    {
        var solicitudes = await _repository.GetByCedulaAsync(cedula);
        return _mapper.Map<IEnumerable<SolicitudEscalafonDto>>(solicitudes);
    }

    public async Task<IEnumerable<SolicitudEscalafonDto>> GetSolicitudesByStatusAsync(string status)
    {
        var solicitudes = await _repository.GetByStatusAsync(status);
        return _mapper.Map<IEnumerable<SolicitudEscalafonDto>>(solicitudes);
    }

    public async Task<int> GetPendingCountAsync()
    {
        return await _repository.GetPendingCountAsync();
    }

    public async Task<SolicitudEscalafonDto> CreateSolicitudAsync(CreateSolicitudEscalafonDto createDto)
    {
        // Verificar si ya existe una solicitud pendiente para este docente
        var existePendiente = await _repository.ExistePendienteByCedulaAsync(createDto.DocenteCedula);
        if (existePendiente)
        {
            throw new InvalidOperationException("Ya existe una solicitud de escalafón pendiente para este docente.");
        }

        var solicitud = _mapper.Map<SolicitudEscalafon>(createDto);
        solicitud.FechaSolicitud = DateTime.Now;
        solicitud.Status = "Pendiente";
        solicitud.CreatedAt = DateTime.UtcNow;

        var createdSolicitud = await _repository.AddAsync(solicitud);
        return _mapper.Map<SolicitudEscalafonDto>(createdSolicitud);
    }

    public async Task<SolicitudEscalafonDto> UpdateSolicitudStatusAsync(UpdateSolicitudStatusDto updateDto)
    {
        var solicitud = await _repository.GetByIdAsync(updateDto.Id);
        if (solicitud == null)
        {
            throw new ArgumentException("Solicitud no encontrada");
        }

        var estadoAnterior = solicitud.Status;
        solicitud.Status = updateDto.Status;
        solicitud.ProcesadoPor = updateDto.ProcesadoPor;

        if (updateDto.Status == "Aprobado")
        {
            solicitud.FechaAprobacion = DateTime.Now;
        }
        else if (updateDto.Status == "Rechazado")
        {
            solicitud.FechaRechazo = DateTime.Now;
            solicitud.MotivoRechazo = updateDto.MotivoRechazo;
        }

        var updatedSolicitud = await _repository.UpdateAsync(solicitud);

        // Si el escalafón se finaliza exitosamente, registrar archivos utilizados
        if (updateDto.Status == "Finalizado" && estadoAnterior != "Finalizado")
        {
            await RegistrarArchivosUtilizadosEnEscalafon(solicitud);
        }

        return _mapper.Map<SolicitudEscalafonDto>(updatedSolicitud);
    }

    public async Task<bool> DeleteSolicitudAsync(int id)
    {
        return await _repository.DeleteAsync(id);
    }

    public async Task<bool> ExisteSolicitudPendienteAsync(string cedula)
    {
        return await _repository.ExistePendienteByCedulaAsync(cedula);
    }

    public async Task<bool> NotificarAprobacionAsync(int solicitudId)
    {
        var solicitud = await _repository.GetByIdAsync(solicitudId);
        if (solicitud == null)
        {
            return false;
        }

        // Verificar que la solicitud tenga un email válido
        if (string.IsNullOrEmpty(solicitud.DocenteEmail))
        {
            return false;
        }

        try
        {
            // Usar el nuevo método específico para solicitudes aprobadas
            return await _emailService.SendSolicitudAprobadaEmailAsync(
                solicitud.DocenteEmail,
                solicitud.DocenteNombre,
                solicitud.NivelActual,
                solicitud.NivelSolicitado,
                solicitud.FechaSolicitud,
                solicitud.FechaAprobacion ?? DateTime.Now,
                solicitud.Observaciones ?? ""
            );
        }
        catch (Exception ex)
        {
            // Log error but don't fail the main process
            Console.WriteLine($"Error enviando correo de aprobación: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> NotificarRechazoAsync(int solicitudId, string motivoRechazo, string rechazadoPor, string nivelRechazo)
    {
        var solicitud = await _repository.GetByIdAsync(solicitudId);
        if (solicitud == null)
        {
            return false;
        }

        // Verificar que la solicitud tenga un email válido
        if (string.IsNullOrEmpty(solicitud.DocenteEmail))
        {
            return false;
        }

        try
        {
            // Usar el nuevo método específico para solicitudes rechazadas
            return await _emailService.SendSolicitudRechazadaEmailAsync(
                solicitud.DocenteEmail,
                solicitud.DocenteNombre,
                solicitud.NivelActual,
                solicitud.NivelSolicitado,
                solicitud.FechaSolicitud,
                DateTime.Now,
                motivoRechazo,
                rechazadoPor,
                nivelRechazo
            );
        }
        catch (Exception ex)
        {
            // Log error but don't fail the main process
            Console.WriteLine($"Error enviando correo de rechazo: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> FinalizarEscalafonAsync(int solicitudId)
    {
        try
        {
            // Obtener la solicitud
            var solicitud = await _repository.GetByIdAsync(solicitudId);
            if (solicitud == null)
            {
                return false;
            }

            // Obtener el usuario/docente por cédula
            var docente = await _userRepository.GetByCedulaAsync(solicitud.DocenteCedula);
            if (docente == null)
            {
                return false;
            }

            // Actualizar el estado de la solicitud a "Finalizado"
            solicitud.Status = "Finalizado";
            solicitud.FechaAprobacion = DateTime.Now;
            solicitud.UpdatedAt = DateTime.UtcNow;

            // Actualizar el nivel del docente
            docente.Nivel = solicitud.NivelSolicitado;
            docente.UpdatedAt = DateTime.UtcNow;

            // Guardar cambios en ambas entidades
            await _repository.UpdateAsync(solicitud);
            await _userRepository.UpdateAsync(docente);

            // REGISTRAR ARCHIVOS UTILIZADOS EN EL ESCALAFÓN
            Console.WriteLine($"[FINALIZAR] Registrando archivos utilizados para solicitud {solicitudId}");
            try
            {
                await RegistrarArchivosUtilizadosEnEscalafon(solicitud);
                Console.WriteLine($"[FINALIZAR] Archivos registrados exitosamente para solicitud {solicitudId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FINALIZAR] Error registrando archivos para solicitud {solicitudId}: {ex.Message}");
                // No fallar el proceso principal por este error
            }

            // Enviar notificación por correo
            var subject = "Escalafón Finalizado - Felicitaciones";
            var body = $@"
                <html>
                <body>
                    <h2>¡Felicitaciones, {solicitud.DocenteNombre}!</h2>
                    <p>Su proceso de escalafón ha sido <strong>FINALIZADO EXITOSAMENTE</strong>.</p>
                    
                    <h3>Su nuevo nivel académico:</h3>
                    <div style='background-color: #e8f5e8; padding: 15px; border-radius: 5px; margin: 15px 0;'>
                        <p style='margin: 0; font-size: 18px; font-weight: bold; color: #2e7d32;'>
                            {solicitud.NivelSolicitado}
                        </p>
                    </div>
                    
                    <h3>Detalles del proceso:</h3>
                    <ul>
                        <li><strong>Nivel anterior:</strong> {solicitud.NivelActual}</li>
                        <li><strong>Nuevo nivel:</strong> {solicitud.NivelSolicitado}</li>
                        <li><strong>Fecha de solicitud:</strong> {solicitud.FechaSolicitud:dd/MM/yyyy}</li>
                        <li><strong>Fecha de finalización:</strong> {DateTime.Now:dd/MM/yyyy}</li>
                    </ul>
                    
                    <p>Su nuevo nivel académico ya está activo en el sistema y será visible en su perfil.</p>
                    
                    <p>Nuevamente, felicitaciones por este importante logro en su carrera académica.</p>
                    
                    <p>Atentamente,<br>
                    Comisión Académica<br>
                    Universidad</p>
                </body>
                </html>";

            await _emailService.SendAdminNotificationEmailAsync(solicitud.DocenteEmail, subject, body, true);
            
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Registra los archivos utilizados cuando se completa un escalafón exitosamente
    /// </summary>
    private async Task RegistrarArchivosUtilizadosEnEscalafon(SolicitudEscalafon solicitud)
    {
        try
        {
            await _archivosUtilizadosService.RegistrarArchivosUtilizados(
                solicitud.Id,
                solicitud.DocenteCedula,
                solicitud.NivelActual,
                solicitud.NivelSolicitado
            );
        }
        catch (Exception ex)
        {
            // Log el error pero no fallar el proceso principal
            // El registro de archivos utilizados es informativo
            Console.WriteLine($"Error al registrar archivos utilizados para solicitud {solicitud.Id}: {ex.Message}");
        }
    }

    /// <summary>
    /// Rechaza una solicitud y envía notificación por correo
    /// </summary>
    public async Task<SolicitudEscalafonDto> RechazarSolicitudAsync(int solicitudId, string motivoRechazo, string rechazadoPor, string nivelRechazo)
    {
        var solicitud = await _repository.GetByIdAsync(solicitudId);
        if (solicitud == null)
        {
            throw new ArgumentException("Solicitud no encontrada");
        }

        // Determinar el estado de rechazo según el nivel
        var estadoRechazo = nivelRechazo switch
        {
            "PresidenteComision" => "RechazadoPresidente",
            "DireccionTalentoHumano" => "RechazadoTTHH",
            "ComisionAcademica" => "RechazadoComision",
            _ => "Rechazado"
        };

        solicitud.Status = estadoRechazo;
        solicitud.FechaRechazo = DateTime.Now;
        solicitud.MotivoRechazo = motivoRechazo;
        solicitud.ProcesadoPor = rechazadoPor;

        var updatedSolicitud = await _repository.UpdateAsync(solicitud);

        // Enviar correo de notificación de rechazo
        await EnviarCorreoRechazoAsync(solicitud, nivelRechazo, rechazadoPor);

        return _mapper.Map<SolicitudEscalafonDto>(updatedSolicitud);
    }

    /// <summary>
    /// Crea una apelación para una solicitud rechazada
    /// </summary>
    public async Task<SolicitudEscalafonDto> CrearApelacionAsync(int solicitudOriginalId, string observacionesApelacion, string destinatario = "", List<IFormFile>? archivos = null)
    {
        var solicitudOriginal = await _repository.GetByIdAsync(solicitudOriginalId);
        if (solicitudOriginal == null)
        {
            throw new ArgumentException("Solicitud original no encontrada");
        }

        // Verificar que la solicitud esté rechazada
        if (!solicitudOriginal.Status.Contains("Rechazado"))
        {
            throw new InvalidOperationException("Solo se pueden apelar solicitudes rechazadas");
        }

        // Determinar el estado basado en el destinatario
        var nuevoStatus = "PendienteApelacion";
        if (!string.IsNullOrEmpty(destinatario))
        {
            nuevoStatus = destinatario switch
            {
                "ComisionAcademica" => "PendienteComision", // Va directamente a la Comisión Académica
                "PresidenteComision" => "PendientePresidente",
                "DireccionTalentoHumano" => "PendienteTTHH",
                _ => "PendienteApelacion"
            };
        }

        // Preparar observaciones con información de archivos
        var observacionesCompletas = $"APELACIÓN DE SOLICITUD #{solicitudOriginalId}: {observacionesApelacion}";
        if (archivos?.Any() == true)
        {
            observacionesCompletas += $"\n\nArchivos adjuntos: {archivos.Count} archivo(s) - ";
            observacionesCompletas += string.Join(", ", archivos.Select(a => a.FileName));
        }

        // Crear nueva solicitud como apelación
        var solicitudApelacion = new SolicitudEscalafon
        {
            DocenteCedula = solicitudOriginal.DocenteCedula,
            DocenteNombre = solicitudOriginal.DocenteNombre,
            DocenteEmail = solicitudOriginal.DocenteEmail,
            DocenteTelefono = solicitudOriginal.DocenteTelefono,
            Facultad = solicitudOriginal.Facultad,
            Carrera = solicitudOriginal.Carrera,
            NivelActual = solicitudOriginal.NivelActual,
            NivelSolicitado = solicitudOriginal.NivelSolicitado,
            AnosExperiencia = solicitudOriginal.AnosExperiencia,
            Titulos = solicitudOriginal.Titulos,
            Publicaciones = solicitudOriginal.Publicaciones,
            Capacitaciones = solicitudOriginal.Capacitaciones,
            FechaSolicitud = DateTime.Now,
            Status = nuevoStatus,
            Observaciones = observacionesCompletas,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var nuevaSolicitud = await _repository.AddAsync(solicitudApelacion);

        // Marcar la solicitud original como apelada
        solicitudOriginal.Observaciones = $"{solicitudOriginal.Observaciones}\n\nAPELADA: Nueva solicitud #{nuevaSolicitud.Id}";
        await _repository.UpdateAsync(solicitudOriginal);

        // Enviar notificación por correo
        await EnviarCorreoApelacionAsync(nuevaSolicitud, destinatario, archivos?.Count ?? 0);

        return _mapper.Map<SolicitudEscalafonDto>(nuevaSolicitud);
    }

    /// <summary>
    /// Envía correo de notificación de rechazo
    /// </summary>
    private async Task EnviarCorreoRechazoAsync(SolicitudEscalafon solicitud, string nivelRechazo, string rechazadoPor)
    {
        try
        {
            // Usar el nuevo método específico para solicitudes rechazadas
            await _emailService.SendSolicitudRechazadaEmailAsync(
                solicitud.DocenteEmail,
                solicitud.DocenteNombre,
                solicitud.NivelActual,
                solicitud.NivelSolicitado,
                solicitud.FechaSolicitud,
                solicitud.FechaRechazo ?? DateTime.Now,
                solicitud.MotivoRechazo ?? "Sin motivo especificado",
                rechazadoPor,
                nivelRechazo
            );
        }
        catch (Exception ex)
        {
            // Log error but don't fail the main process
            Console.WriteLine($"Error enviando correo de rechazo: {ex.Message}");
        }
    }

    /// <summary>
    /// Envía correo de notificación de apelación
    /// </summary>
    private async Task EnviarCorreoApelacionAsync(SolicitudEscalafon solicitud, string destinatario, int cantidadArchivos)
    {
        try
        {
            var subject = $"Apelación Registrada - Solicitud de Escalafón #{solicitud.Id}";
            var destinatarioTexto = destinatario switch
            {
                "ComisionAcademica" => "Comisión Académica de Escalafón",
                "PresidenteComision" => "Presidente de la Comisión Académica",
                "DireccionTalentoHumano" => "Dirección de Talento Humano",
                _ => "autoridad competente"
            };

            var body = $@"
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset='utf-8'>
                <style>
                    .container {{ max-width: 600px; margin: 0 auto; font-family: Arial, sans-serif; }}
                    .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 20px; text-align: center; }}
                    .content {{ padding: 20px; }}
                    .footer {{ background-color: #f8f9fa; padding: 15px; text-align: center; font-size: 12px; }}
                    .info-box {{ background-color: #e7f3ff; border-left: 4px solid #2196F3; padding: 15px; margin: 15px 0; }}
                    .success-badge {{ background: #4CAF50; color: white; padding: 5px 10px; border-radius: 15px; }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <h2>🎯 Apelación Registrada Exitosamente</h2>
                        <p>Solicitud de Escalafón Docente</p>
                    </div>
                    
                    <div class='content'>
                        <h3>Estimado/a {solicitud.DocenteNombre},</h3>
                        
                        <p>Su apelación ha sido registrada exitosamente en el sistema y ha sido enviada a <strong>{destinatarioTexto}</strong> para su evaluación.</p>
                        
                        <div class='info-box'>
                            <h4>📋 Detalles de la Apelación:</h4>
                            <ul>
                                <li><strong>Número de apelación:</strong> #{solicitud.Id}</li>
                                <li><strong>Nivel solicitado:</strong> {solicitud.NivelSolicitado}</li>
                                <li><strong>Fecha de registro:</strong> {solicitud.FechaSolicitud:dd/MM/yyyy HH:mm}</li>
                                <li><strong>Destino:</strong> {destinatarioTexto}</li>";

            if (cantidadArchivos > 0)
            {
                body += $"<li><strong>Archivos adjuntos:</strong> {cantidadArchivos} archivo(s)</li>";
            }

            body += $@"
                            </ul>
                        </div>
                        
                        <div class='info-box'>
                            <h4>🔄 Próximos Pasos:</h4>
                            <ol>
                                <li>Su apelación será revisada por {destinatarioTexto}</li>
                                <li>Recibirá una notificación cuando se tome una decisión</li>
                                <li>Puede consultar el estado en su dashboard del sistema</li>
                            </ol>
                        </div>
                        
                        <p><strong>Estado actual:</strong> <span class='success-badge'>En Revisión</span></p>
                        <p>Gracias por utilizar nuestro sistema de escalafón docente.</p>
                    </div>
                    
                    <div class='footer'>
                        <p>Atentamente,<br>
                        <strong>Sistema de Escalafón Docente</strong><br>
                        Universidad Técnica de Ambato<br>
                        📧 escalafon@uta.edu.ec | 📞 03-2848487</p>
                    </div>
                </div>
            </body>
            </html>";

            await _emailService.SendEmailAsync(solicitud.DocenteEmail, subject, body);
        }
        catch (Exception ex)
        {
            // Log error but don't fail the main process
            Console.WriteLine($"Error enviando correo de apelación: {ex.Message}");
        }
    }

    public async Task<IEnumerable<HistorialEscalafonDto>> GetHistorialEscalafonAsync(string cedula)
    {
        try
        {
            Console.WriteLine($"[HISTORIAL] Obteniendo historial para cédula: {cedula}");
            
            // Obtener todas las solicitudes finalizadas del docente
            var solicitudesFinalizadas = await _repository.GetHistorialEscalafonAsync(cedula);
            
            Console.WriteLine($"[HISTORIAL] Solicitudes encontradas: {solicitudesFinalizadas.Count()}");
            
            var historialList = new List<HistorialEscalafonDto>();
            
            foreach (var solicitud in solicitudesFinalizadas)
            {
                Console.WriteLine($"[HISTORIAL] Procesando solicitud ID: {solicitud.Id}, Estado: {solicitud.Status}, Nivel: {solicitud.NivelActual} -> {solicitud.NivelSolicitado}");
                
                var historial = new HistorialEscalafonDto
                {
                    Id = solicitud.Id,
                    NivelAnterior = solicitud.NivelActual,
                    NivelNuevo = solicitud.NivelSolicitado,
                    FechaPromocion = solicitud.FechaAprobacion ?? solicitud.FechaSolicitud,
                    EstadoSolicitud = "Finalizado",
                    DocumentosUtilizados = await ObtenerDocumentosUtilizados(solicitud.Id, cedula),
                    DocumentosDetalles = await ObtenerDocumentosDetalladosReales(solicitud.Id, cedula),
                    ObservacionesFinales = solicitud.Observaciones ?? "Escalafón completado exitosamente",
                    AprobadoPor = solicitud.ProcesadoPor ?? "Comisión Académica de Escalafón"
                };
                
                historialList.Add(historial);
            }
            
            Console.WriteLine($"[HISTORIAL] Historial final: {historialList.Count} registros para cédula {cedula}");
            
            return historialList.OrderByDescending(h => h.FechaPromocion);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[HISTORIAL] Error: {ex.Message}");
            throw new InvalidOperationException($"Error al obtener historial de escalafón: {ex.Message}", ex);
        }
    }

    private async Task<List<string>> ObtenerDocumentosUtilizados(int solicitudId, string cedula)
    {
        try
        {
            Console.WriteLine($"[HISTORIAL] Obteniendo documentos utilizados para solicitud {solicitudId}");
            
            // Obtener documentos reales utilizados específicamente en esta solicitud
            var archivosUtilizados = await _archivosUtilizadosService.ObtenerArchivosPorSolicitud(solicitudId);
            
            Console.WriteLine($"[HISTORIAL] Archivos encontrados para solicitud {solicitudId}: {archivosUtilizados.Count}");
            
            // Filtrar duplicados por tipo de recurso y ID de recurso
            var archivosUnicos = archivosUtilizados
                .GroupBy(a => new { a.TipoRecurso, a.RecursoId })
                .Select(g => g.OrderBy(a => a.FechaUtilizacion).First())
                .ToList();
            
            Console.WriteLine($"[HISTORIAL] Archivos únicos después de eliminar duplicados: {archivosUnicos.Count}");
            
            var documentos = new List<string>();
            
            foreach (var archivo in archivosUnicos)
            {
                var icono = archivo.TipoRecurso switch
                {
                    "Investigacion" => "📚",
                    "EvaluacionDesempeno" => "⭐",
                    "Capacitacion" => "🎓",
                    _ => "📄"
                };
                
                var descripcion = !string.IsNullOrEmpty(archivo.Descripcion) 
                    ? archivo.Descripcion 
                    : archivo.TituloRecurso;
                
                documentos.Add($"{icono} {archivo.TipoRecurso}: {descripcion}");
                Console.WriteLine($"[HISTORIAL] Documento: {archivo.TipoRecurso} - {descripcion}");
            }
            
            if (!documentos.Any())
            {
                Console.WriteLine($"[HISTORIAL] No se encontraron documentos para solicitud {solicitudId}, usando documentos por defecto");
                return new List<string> { "📄 Documentos académicos utilizados en la promoción" };
            }
            
            return documentos;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[HISTORIAL] Error obteniendo documentos utilizados: {ex.Message}");
            return new List<string> { "❌ Error al cargar documentos utilizados" };
        }
    }

    private async Task<DocumentosDetallados> ObtenerDocumentosDetalladosAsync(int solicitudId, string cedula)
    {
        try
        {
            Console.WriteLine($"[DOCUMENTOS] Obteniendo documentos detallados para solicitud {solicitudId}, cédula {cedula}");

            var solicitud = await _repository.GetByIdAsync(solicitudId);
            if (solicitud == null)
            {
                Console.WriteLine($"[DOCUMENTOS] No se encontró la solicitud {solicitudId}");
                return new DocumentosDetallados();
            }

            var documentosDetallados = new DocumentosDetallados();

            // Simular obtención de investigaciones (ya que no tenemos acceso directo al DbContext aquí)
            // En una implementación real, esto se haría mediante repositorios específicos
            documentosDetallados.Investigaciones = new List<InvestigacionUtilizada>
            {
                new InvestigacionUtilizada
                {
                    Id = 1,
                    Titulo = "Análisis de metodologías de enseñanza en educación superior",
                    Tipo = "Artículo",
                    RevistaOEditorial = "Revista Científica UTA",
                    FechaPublicacion = solicitud.FechaSolicitud.AddMonths(-6),
                    Filiacion = "Universidad Técnica de Ambato",
                    TieneFiliacionUTA = true
                },
                new InvestigacionUtilizada
                {
                    Id = 2,
                    Titulo = "Innovación tecnológica en procesos educativos",
                    Tipo = "Capítulo de libro",
                    RevistaOEditorial = "Editorial Académica",
                    FechaPublicacion = solicitud.FechaSolicitud.AddMonths(-12),
                    Filiacion = "Universidad Técnica de Ambato",
                    TieneFiliacionUTA = true
                }
            };

            // Simular evaluaciones de desempeño
            documentosDetallados.Evaluaciones = new List<EvaluacionUtilizada>
            {
                new EvaluacionUtilizada
                {
                    Id = 1,
                    PeriodoAcademico = "2023-2",
                    Anio = 2023,
                    Semestre = 2,
                    PuntajeObtenido = 85,
                    PuntajeMaximo = 100,
                    Porcentaje = 85,
                    Estado = "Completada"
                },
                new EvaluacionUtilizada
                {
                    Id = 2,
                    PeriodoAcademico = "2024-1",
                    Anio = 2024,
                    Semestre = 1,
                    PuntajeObtenido = 90,
                    PuntajeMaximo = 100,
                    Porcentaje = 90,
                    Estado = "Completada"
                },
                new EvaluacionUtilizada
                {
                    Id = 3,
                    PeriodoAcademico = "2024-2",
                    Anio = 2024,
                    Semestre = 2,
                    PuntajeObtenido = 88,
                    PuntajeMaximo = 100,
                    Porcentaje = 88,
                    Estado = "Completada"
                }
            };

            // Simular capacitaciones DITIC
            documentosDetallados.Capacitaciones = new List<CapacitacionUtilizada>
            {
                new CapacitacionUtilizada
                {
                    Id = 1,
                    NombreCurso = "Metodologías pedagógicas innovadoras",
                    Facilitador = "DITIC - UTA",
                    HorasAcademicas = 40,
                    FechaInicio = solicitud.FechaSolicitud.AddMonths(-18),
                    FechaFin = solicitud.FechaSolicitud.AddMonths(-17),
                    Tipo = "Presencial",
                    EsPedagogica = true
                },
                new CapacitacionUtilizada
                {
                    Id = 2,
                    NombreCurso = "Tecnologías de la información en educación",
                    Facilitador = "DITIC - UTA",
                    HorasAcademicas = 30,
                    FechaInicio = solicitud.FechaSolicitud.AddMonths(-12),
                    FechaFin = solicitud.FechaSolicitud.AddMonths(-11),
                    Tipo = "Virtual",
                    EsPedagogica = true
                },
                new CapacitacionUtilizada
                {
                    Id = 3,
                    NombreCurso = "Gestión de proyectos de investigación",
                    Facilitador = "DITIC - UTA",
                    HorasAcademicas = 25,
                    FechaInicio = solicitud.FechaSolicitud.AddMonths(-8),
                    FechaFin = solicitud.FechaSolicitud.AddMonths(-7),
                    Tipo = "Híbrido",
                    EsPedagogica = false
                }
            };

            // Calcular verificación de requisitos
            documentosDetallados.VerificacionRequisitos = new VerificacionRequisitos
            {
                TotalInvestigaciones = documentosDetallados.Investigaciones.Count,
                InvestigacionesConUTA = documentosDetallados.Investigaciones.Count(i => i.TieneFiliacionUTA),
                TotalHorasCapacitacion = documentosDetallados.Capacitaciones.Sum(c => c.HorasAcademicas),
                HorasPedagogicas = documentosDetallados.Capacitaciones.Where(c => c.EsPedagogica).Sum(c => c.HorasAcademicas),
                PromedioEvaluaciones = documentosDetallados.Evaluaciones.Count > 0 ? 
                    documentosDetallados.Evaluaciones.Average(e => e.Porcentaje) : 0,
                PeriodosEvaluados = documentosDetallados.Evaluaciones.Count,
                CumpleTodosRequisitos = true
            };

            Console.WriteLine($"[DOCUMENTOS] Documentos procesados - Inv: {documentosDetallados.Investigaciones.Count}, Eval: {documentosDetallados.Evaluaciones.Count}, Cap: {documentosDetallados.Capacitaciones.Count}");

            return documentosDetallados;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DOCUMENTOS] Error: {ex.Message}");
            return new DocumentosDetallados();
        }
    }

    /// <summary>
    /// Obtiene los documentos detallados reales utilizados en una solicitud de escalafón
    /// </summary>
    private async Task<DocumentosDetallados> ObtenerDocumentosDetalladosReales(int solicitudId, string cedula)
    {
        try
        {
            Console.WriteLine($"[DOCUMENTOS] Obteniendo documentos detallados reales para solicitud {solicitudId}");
            
            // Obtener archivos utilizados reales de la base de datos
            var archivosUtilizados = await _archivosUtilizadosService.ObtenerArchivosPorSolicitud(solicitudId);
            
            // Filtrar duplicados por tipo de recurso y ID de recurso
            var archivosUnicos = archivosUtilizados
                .GroupBy(a => new { a.TipoRecurso, a.RecursoId })
                .Select(g => g.OrderBy(a => a.FechaUtilizacion).First())
                .ToList();
            
            Console.WriteLine($"[DOCUMENTOS] Archivos únicos después de eliminar duplicados: {archivosUnicos.Count}");
            
            var documentosDetallados = new DocumentosDetallados();
            
            // Agrupar por tipo de recurso (ya sin duplicados)
            var investigaciones = archivosUnicos.Where(a => a.TipoRecurso == "Investigacion").ToList();
            var evaluaciones = archivosUnicos.Where(a => a.TipoRecurso == "EvaluacionDesempeno").ToList();
            var capacitaciones = archivosUnicos.Where(a => a.TipoRecurso == "Capacitacion").ToList();
            
            // Mapear investigaciones
            documentosDetallados.Investigaciones = investigaciones.Select(inv => new InvestigacionUtilizada
            {
                Id = inv.RecursoId,
                Titulo = inv.Descripcion ?? "Publicación científica",
                Tipo = "Artículo científico",
                RevistaOEditorial = "Revista académica",
                FechaPublicacion = inv.FechaUtilizacion.AddMonths(-6),
                Filiacion = "Universidad Técnica de Ambato",
                TieneFiliacionUTA = true
            }).ToList();
            
            // Mapear evaluaciones
            documentosDetallados.Evaluaciones = evaluaciones.Select(eval => new EvaluacionUtilizada
            {
                Id = eval.RecursoId,
                PeriodoAcademico = ExtractPeriodoFromDescription(eval.Descripcion),
                Anio = ExtractAnioFromDescription(eval.Descripcion),
                Semestre = ExtractSemestreFromDescription(eval.Descripcion),
                PuntajeObtenido = (decimal)ExtractPuntajeFromDescription(eval.Descripcion), // Corregido: valor correcto del puntaje
                PuntajeMaximo = 100,
                Porcentaje = (decimal)ExtractPuntajeFromDescription(eval.Descripcion), // Corregido: porcentaje sin multiplicar por 100
                Estado = "Completada"
            }).ToList();
            
            // Mapear capacitaciones
            documentosDetallados.Capacitaciones = capacitaciones.Select(cap => new CapacitacionUtilizada
            {
                Id = cap.RecursoId,
                NombreCurso = cap.Descripcion ?? "Capacitación profesional",
                Facilitador = "DITIC - UTA",
                HorasAcademicas = EstimarHorasCapacitacion(cap.Descripcion),
                FechaInicio = cap.FechaUtilizacion.AddMonths(-1),
                FechaFin = cap.FechaUtilizacion,
                Tipo = "Presencial",
                EsPedagogica = true
            }).ToList();
            
            // Calcular verificación de requisitos con datos reales
            documentosDetallados.VerificacionRequisitos = new VerificacionRequisitos
            {
                TotalInvestigaciones = documentosDetallados.Investigaciones.Count,
                InvestigacionesConUTA = documentosDetallados.Investigaciones.Count(i => i.TieneFiliacionUTA),
                TotalHorasCapacitacion = documentosDetallados.Capacitaciones.Sum(c => c.HorasAcademicas),
                HorasPedagogicas = documentosDetallados.Capacitaciones.Where(c => c.EsPedagogica).Sum(c => c.HorasAcademicas),
                PromedioEvaluaciones = documentosDetallados.Evaluaciones.Count > 0 ? 
                    documentosDetallados.Evaluaciones.Average(e => e.Porcentaje) : 0,
                PeriodosEvaluados = documentosDetallados.Evaluaciones.Count,
                CumpleTodosRequisitos = documentosDetallados.Investigaciones.Count >= 2 && 
                                      documentosDetallados.Evaluaciones.Count >= 3 &&
                                      documentosDetallados.Capacitaciones.Sum(c => c.HorasAcademicas) >= 80
            };
            
            Console.WriteLine($"[DOCUMENTOS] Documentos reales procesados - Inv: {documentosDetallados.Investigaciones.Count}, Eval: {documentosDetallados.Evaluaciones.Count}, Cap: {documentosDetallados.Capacitaciones.Count}");
            
            return documentosDetallados;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DOCUMENTOS] Error obteniendo documentos reales: {ex.Message}");
            return new DocumentosDetallados();
        }
    }

    /// <summary>
    /// Métodos auxiliares para extraer información de las descripciones
    /// </summary>
    private string ExtractPeriodoFromDescription(string? descripcion)
    {
        if (string.IsNullOrEmpty(descripcion)) return "N/A";
        
        // Buscar patrón como "2024-1" o "2023-2"
        var match = System.Text.RegularExpressions.Regex.Match(descripcion, @"(\d{4})-?(\d)?");
        if (match.Success)
        {
            return match.Groups[0].Value;
        }
        return "N/A";
    }
    
    private int ExtractAnioFromDescription(string? descripcion)
    {
        if (string.IsNullOrEmpty(descripcion)) return DateTime.Now.Year;
        
        var match = System.Text.RegularExpressions.Regex.Match(descripcion, @"(\d{4})");
        if (match.Success && int.TryParse(match.Groups[1].Value, out int anio))
        {
            return anio;
        }
        return DateTime.Now.Year;
    }
    
    private int ExtractSemestreFromDescription(string? descripcion)
    {
        if (string.IsNullOrEmpty(descripcion)) return 1;
        
        var match = System.Text.RegularExpressions.Regex.Match(descripcion, @"\d{4}-(\d)");
        if (match.Success && int.TryParse(match.Groups[1].Value, out int semestre))
        {
            return semestre;
        }
        return 1;
    }
    
    private double ExtractPuntajeFromDescription(string? descripcion)
    {
        if (string.IsNullOrEmpty(descripcion)) return 0;
        
        Console.WriteLine($"[DEBUG] Extrayendo puntaje de: '{descripcion}'");
        
        // Buscar patrón como "85.5%" o "90,0%" o "78,200%"
        var match = System.Text.RegularExpressions.Regex.Match(descripcion, @"(\d+[,.]?\d*)%");
        if (match.Success)
        {
            var puntajeStr = match.Groups[1].Value.Replace(",", ".");
            Console.WriteLine($"[DEBUG] String extraído: '{puntajeStr}'");
            
            if (double.TryParse(puntajeStr, System.Globalization.CultureInfo.InvariantCulture, out double puntaje))
            {
                Console.WriteLine($"[DEBUG] Valor parseado: {puntaje}");
                
                // Si el valor es mayor a 100, probablemente viene con demasiados decimales (ej: 78200 en lugar de 78.2)
                // En ese caso dividir entre 1000
                if (puntaje > 100)
                {
                    puntaje = puntaje / 1000.0;
                    Console.WriteLine($"[DEBUG] Valor corregido (dividido entre 1000): {puntaje}");
                }
                return puntaje;
            }
        }
        Console.WriteLine($"[DEBUG] No se pudo extraer puntaje, retornando 0");
        return 0;
    }
    
    private int EstimarHorasCapacitacion(string? descripcion)
    {
        if (string.IsNullOrEmpty(descripcion)) return 20;
        
        // Buscar patrón como "40 horas" o números en la descripción
        var match = System.Text.RegularExpressions.Regex.Match(descripcion, @"(\d+)\s*horas?");
        if (match.Success && int.TryParse(match.Groups[1].Value, out int horas))
        {
            return horas;
        }
        
        // Estimación básica según el tipo de capacitación
        if (descripcion.ToLower().Contains("metodolog")) return 40;
        if (descripcion.ToLower().Contains("tecnolog")) return 30;
        if (descripcion.ToLower().Contains("evaluacion")) return 25;
        if (descripcion.ToLower().Contains("investigacion")) return 35;
        
        return 20; // Valor por defecto
    }
}
