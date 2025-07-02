using AutoMapper;
using ProyectoAgiles.Application.DTOs;
using ProyectoAgiles.Application.Interfaces;
using ProyectoAgiles.Domain.Entities;
using ProyectoAgiles.Domain.Interfaces;

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

        var subject = "Notificación de Aprobación - Solicitud de Escalafón";
        var body = $@"
            <html>
            <body>
                <h2>Estimado/a {solicitud.DocenteNombre},</h2>
                <p>Nos complace informarle que su solicitud de escalafón ha sido <strong>APROBADA</strong> por la Comisión Académica.</p>
                
                <h3>Detalles de la solicitud:</h3>
                <ul>
                    <li><strong>Nivel actual:</strong> {solicitud.NivelActual}</li>
                    <li><strong>Nivel solicitado:</strong> {solicitud.NivelSolicitado}</li>
                    <li><strong>Fecha de solicitud:</strong> {solicitud.FechaSolicitud:dd/MM/yyyy}</li>
                    <li><strong>Fecha de aprobación:</strong> {solicitud.FechaAprobacion:dd/MM/yyyy}</li>
                </ul>
                
                {(string.IsNullOrEmpty(solicitud.Observaciones) ? "" : $"<p><strong>Observaciones:</strong> {solicitud.Observaciones}</p>")}
                
                <p>Felicitaciones por este logro académico. Su nueva categoría entrará en vigencia según los procedimientos establecidos por la institución.</p>
                
                <p>Si tiene alguna consulta, no dude en contactarnos.</p>
                
                <p>Atentamente,<br>
                Comisión Académica<br>
                Universidad</p>
            </body>
            </html>";

        return await _emailService.SendAdminNotificationEmailAsync(solicitud.DocenteEmail, subject, body, true);
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

            // Actualizar el estado de la solicitud a "Finalizada"
            solicitud.Status = "Finalizada";
            solicitud.FechaAprobacion = DateTime.Now;
            solicitud.UpdatedAt = DateTime.UtcNow;

            // Actualizar el nivel del docente
            docente.Nivel = solicitud.NivelSolicitado;
            docente.UpdatedAt = DateTime.UtcNow;

            // Guardar cambios en ambas entidades
            await _repository.UpdateAsync(solicitud);
            await _userRepository.UpdateAsync(docente);

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
    public async Task<SolicitudEscalafonDto> CrearApelacionAsync(int solicitudOriginalId, string observacionesApelacion)
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
            Status = "PendienteApelacion",
            Observaciones = $"APELACIÓN DE SOLICITUD #{solicitudOriginalId}: {observacionesApelacion}",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var nuevaSolicitud = await _repository.AddAsync(solicitudApelacion);

        // Marcar la solicitud original como apelada
        solicitudOriginal.Observaciones = $"{solicitudOriginal.Observaciones}\n\nAPELADA: Nueva solicitud #{nuevaSolicitud.Id}";
        await _repository.UpdateAsync(solicitudOriginal);

        return _mapper.Map<SolicitudEscalafonDto>(nuevaSolicitud);
    }

    /// <summary>
    /// Envía correo de notificación de rechazo
    /// </summary>
    private async Task EnviarCorreoRechazoAsync(SolicitudEscalafon solicitud, string nivelRechazo, string rechazadoPor)
    {
        try
        {
            var nivelTexto = nivelRechazo switch
            {
                "PresidenteComision" => "Presidente de la Comisión Académica",
                "DireccionTalentoHumano" => "Dirección de Talento Humano",
                "ComisionAcademica" => "Comisión Académica de Escalafón",
                _ => "Administración"
            };

            var subject = $"Solicitud de Escalafón Rechazada - {nivelTexto}";
            var body = $@"
            <html>
            <head>
                <style>
                    body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                    .header {{ background-color: #d32f2f; color: white; padding: 20px; text-align: center; }}
                    .content {{ padding: 20px; }}
                    .details {{ background-color: #f5f5f5; padding: 15px; margin: 15px 0; border-radius: 5px; }}
                    .footer {{ background-color: #f0f0f0; padding: 15px; text-align: center; font-size: 12px; }}
                    .warning {{ color: #d32f2f; font-weight: bold; }}
                    .appeal-info {{ background-color: #e3f2fd; padding: 15px; margin: 15px 0; border-left: 4px solid #2196f3; }}
                </style>
            </head>
            <body>
                <div class='header'>
                    <h2>🚫 Solicitud de Escalafón Rechazada</h2>
                </div>
                
                <div class='content'>
                    <h3>Estimado/a {solicitud.DocenteNombre},</h3>
                    
                    <p>Lamentamos informarle que su solicitud de escalafón ha sido <span class='warning'>RECHAZADA</span> por <strong>{nivelTexto}</strong>.</p>
                    
                    <div class='details'>
                        <h4>📋 Detalles de la solicitud:</h4>
                        <ul>
                            <li><strong>Número de solicitud:</strong> #{solicitud.Id}</li>
                            <li><strong>Nivel actual:</strong> {solicitud.NivelActual}</li>
                            <li><strong>Nivel solicitado:</strong> {solicitud.NivelSolicitado}</li>
                            <li><strong>Fecha de solicitud:</strong> {solicitud.FechaSolicitud:dd/MM/yyyy}</li>
                            <li><strong>Fecha de rechazo:</strong> {solicitud.FechaRechazo:dd/MM/yyyy HH:mm}</li>
                            <li><strong>Rechazado por:</strong> {rechazadoPor}</li>
                            <li><strong>Nivel de rechazo:</strong> {nivelTexto}</li>
                        </ul>
                    </div>
                    
                    <div class='details'>
                        <h4>📝 Motivo del rechazo:</h4>
                        <p><em>{solicitud.MotivoRechazo}</em></p>
                    </div>
                    
                    <div class='appeal-info'>
                        <h4>📢 Derecho de Apelación</h4>
                        <p>Usted tiene derecho a apelar esta decisión. Para ello:</p>
                        <ol>
                            <li>Ingrese a su dashboard en el sistema</li>
                            <li>Vaya a la sección ""Mis Solicitudes""</li>
                            <li>Busque la solicitud rechazada</li>
                            <li>Haga clic en el botón ""Apelar""</li>
                            <li>Proporcione la documentación adicional o justificación necesaria</li>
                        </ol>
                        <p><strong>Nota:</strong> Puede presentar su apelación en cualquier momento desde su dashboard.</p>
                    </div>
                    
                    <p>Si tiene alguna consulta sobre este proceso, no dude en contactarnos.</p>
                </div>
                
                <div class='footer'>
                    <p>Atentamente,<br>
                    <strong>Sistema de Escalafón Docente</strong><br>
                    Universidad Técnica de Ambato<br>
                    📧 escalafon@uta.edu.ec | 📞 03-2848487</p>
                </div>
            </body>
            </html>";

            await _emailService.SendEmailAsync(solicitud.DocenteEmail, subject, body);
        }
        catch (Exception ex)
        {
            // Log error but don't fail the main process
            Console.WriteLine($"Error enviando correo de rechazo: {ex.Message}");
        }
    }
}
