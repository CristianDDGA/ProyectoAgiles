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

    public SolicitudEscalafonService(
        ISolicitudEscalafonRepository repository, 
        IMapper mapper, 
        IEmailService emailService,
        IUserRepository userRepository)
    {
        _repository = repository;
        _mapper = mapper;
        _emailService = emailService;
        _userRepository = userRepository;
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
}
