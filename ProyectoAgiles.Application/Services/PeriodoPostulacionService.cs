using ProyectoAgiles.Application.DTOs;
using ProyectoAgiles.Application.Interfaces;
using ProyectoAgiles.Domain.Entities;
using ProyectoAgiles.Domain.Interfaces;

namespace ProyectoAgiles.Application.Services;

public class PeriodoPostulacionService : IPeriodoPostulacionService
{
    private readonly IPeriodoPostulacionRepository _repository;
    private readonly IEmailService _emailService;
    private readonly IUserRepository _userRepository;

    public PeriodoPostulacionService(
        IPeriodoPostulacionRepository repository,
        IEmailService emailService,
        IUserRepository userRepository)
    {
        _repository = repository;
        _emailService = emailService;
        _userRepository = userRepository;
    }

    public async Task<ApiResponse<IEnumerable<PeriodoPostulacionDto>>> GetAllPeriodsAsync()
    {
        try
        {
            var periods = await _repository.GetAllAsync();
            var periodsDto = periods.Select(MapToDto).ToList();

            return new ApiResponse<IEnumerable<PeriodoPostulacionDto>>
            {
                Success = true,
                Data = periodsDto,
                Message = "Períodos obtenidos exitosamente"
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<IEnumerable<PeriodoPostulacionDto>>
            {
                Success = false,
                Message = "Error al obtener los períodos",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<PeriodoPostulacionDto>> GetPeriodByIdAsync(int id)
    {
        try
        {
            var period = await _repository.GetByIdAsync(id);
            if (period == null)
            {
                return new ApiResponse<PeriodoPostulacionDto>
                {
                    Success = false,
                    Message = "Período no encontrado"
                };
            }

            return new ApiResponse<PeriodoPostulacionDto>
            {
                Success = true,
                Data = MapToDto(period),
                Message = "Período obtenido exitosamente"
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<PeriodoPostulacionDto>
            {
                Success = false,
                Message = "Error al obtener el período",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<PeriodoInfoDto>> GetActivePeriodInfoAsync()
    {
        try
        {
            var activePeriod = await _repository.GetActivePeriodAsync();
            
            if (activePeriod == null)
            {
                return new ApiResponse<PeriodoInfoDto>
                {
                    Success = true,
                    Data = new PeriodoInfoDto
                    {
                        HayPeriodoActivo = false,
                        Mensaje = "No hay períodos de postulación activos en este momento.",
                        EstadoPeriodo = "Sin período activo"
                    },
                    Message = "No hay período activo"
                };
            }

            var info = new PeriodoInfoDto
            {
                HayPeriodoActivo = activePeriod.EstaActivo,
                Mensaje = GetPeriodMessage(activePeriod),
                FechaInicio = activePeriod.FechaInicio,
                FechaFin = activePeriod.FechaFin,
                DiasRestantes = activePeriod.DiasRestantes,
                EstadoPeriodo = activePeriod.EstadoPeriodo,
                Descripcion = activePeriod.Descripcion
            };

            return new ApiResponse<PeriodoInfoDto>
            {
                Success = true,
                Data = info,
                Message = "Información del período obtenida exitosamente"
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<PeriodoInfoDto>
            {
                Success = false,
                Message = "Error al obtener información del período",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<PeriodoPostulacionDto>> CreatePeriodAsync(CreatePeriodoPostulacionDto createDto)
    {
        try
        {
            // Validaciones
            if (createDto.FechaFin <= createDto.FechaInicio)
            {
                return new ApiResponse<PeriodoPostulacionDto>
                {
                    Success = false,
                    Message = "La fecha de fin debe ser posterior a la fecha de inicio"
                };
            }

            if (createDto.FechaInicio < DateTime.UtcNow.Date)
            {
                return new ApiResponse<PeriodoPostulacionDto>
                {
                    Success = false,
                    Message = "La fecha de inicio no puede ser anterior a hoy"
                };
            }

            // Si se va a activar inmediatamente, desactivar otros períodos
            if (createDto.ActivarInmediatamente)
            {
                await _repository.DeactivateAllPeriodsAsync();
            }

            var period = new PeriodoPostulacion
            {
                FechaInicio = createDto.FechaInicio,
                FechaFin = createDto.FechaFin,
                Descripcion = createDto.Descripcion,
                Activo = createDto.ActivarInmediatamente,
                FechaCreacion = DateTime.UtcNow
            };

            var createdPeriod = await _repository.CreateAsync(period);

            // Send email notification if the period is activated immediately
            if (createDto.ActivarInmediatamente)
            {
                await SendPeriodActivationNotificationAsync(createdPeriod);
            }

            return new ApiResponse<PeriodoPostulacionDto>
            {
                Success = true,
                Data = MapToDto(createdPeriod),
                Message = "Período creado exitosamente"
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<PeriodoPostulacionDto>
            {
                Success = false,
                Message = "Error al crear el período",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<PeriodoPostulacionDto>> UpdatePeriodAsync(int id, UpdatePeriodoPostulacionDto updateDto)
    {
        try
        {
            var period = await _repository.GetByIdAsync(id);
            if (period == null)
            {
                return new ApiResponse<PeriodoPostulacionDto>
                {
                    Success = false,
                    Message = "Período no encontrado"
                };
            }

            // Validaciones
            if (updateDto.FechaFin <= updateDto.FechaInicio)
            {
                return new ApiResponse<PeriodoPostulacionDto>
                {
                    Success = false,
                    Message = "La fecha de fin debe ser posterior a la fecha de inicio"
                };
            }

            // Actualizar propiedades
            period.FechaInicio = updateDto.FechaInicio;
            period.FechaFin = updateDto.FechaFin;
            period.Descripcion = updateDto.Descripcion;

            var updatedPeriod = await _repository.UpdateAsync(period);

            return new ApiResponse<PeriodoPostulacionDto>
            {
                Success = true,
                Data = MapToDto(updatedPeriod),
                Message = "Período actualizado exitosamente"
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<PeriodoPostulacionDto>
            {
                Success = false,
                Message = "Error al actualizar el período",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<bool>> DeletePeriodAsync(int id)
    {
        try
        {
            var result = await _repository.DeleteAsync(id);
            
            if (!result)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Período no encontrado"
                };
            }

            return new ApiResponse<bool>
            {
                Success = true,
                Data = true,
                Message = "Período eliminado exitosamente"
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<bool>
            {
                Success = false,
                Message = "Error al eliminar el período",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<bool>> ActivatePeriodAsync(int id)
    {
        try
        {
            var period = await _repository.GetByIdAsync(id);
            if (period == null)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Período no encontrado"
                };
            }

            var result = await _repository.ActivatePeriodAsync(id);
            
            if (!result)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Error al activar el período"
                };
            }

            // Send email notification
            await SendPeriodActivationNotificationAsync(period);

            return new ApiResponse<bool>
            {
                Success = true,
                Data = true,
                Message = "Período activado exitosamente"
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<bool>
            {
                Success = false,
                Message = "Error al activar el período",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<bool>> DeactivatePeriodAsync(int id)
    {
        try
        {
            var period = await _repository.GetByIdAsync(id);
            if (period == null)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Período no encontrado"
                };
            }

            var result = await _repository.DeactivatePeriodAsync(id);
            
            if (!result)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Error al desactivar el período"
                };
            }

            return new ApiResponse<bool>
            {
                Success = true,
                Data = true,
                Message = "Período desactivado exitosamente"
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<bool>
            {
                Success = false,
                Message = "Error al desactivar el período",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<bool>> DeactivateAllPeriodsAsync()
    {
        try
        {
            await _repository.DeactivateAllPeriodsAsync();

            return new ApiResponse<bool>
            {
                Success = true,
                Data = true,
                Message = "Todos los períodos han sido desactivados"
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<bool>
            {
                Success = false,
                Message = "Error al desactivar los períodos",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<bool>> ValidateCanCreateSolicitudAsync()
    {
        try
        {
            var activePeriod = await _repository.GetActivePeriodAsync();
            
            if (activePeriod == null)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Data = false,
                    Message = "No hay un período de postulación activo en este momento."
                };
            }

            if (!activePeriod.EstaActivo)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Data = false,
                    Message = GetPeriodMessage(activePeriod)
                };
            }

            return new ApiResponse<bool>
            {
                Success = true,
                Data = true,
                Message = $"Puede crear solicitudes. Quedan {activePeriod.DiasRestantes} días del período actual."
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<bool>
            {
                Success = false,
                Data = false,
                Message = "Error al validar el período de postulación",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<IEnumerable<PeriodoPostulacionDto>>> GetPeriodsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            var periods = await _repository.GetPeriodsByDateRangeAsync(startDate, endDate);
            var periodsDto = periods.Select(MapToDto).ToList();

            return new ApiResponse<IEnumerable<PeriodoPostulacionDto>>
            {
                Success = true,
                Data = periodsDto,
                Message = "Períodos obtenidos exitosamente"
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<IEnumerable<PeriodoPostulacionDto>>
            {
                Success = false,
                Message = "Error al obtener los períodos",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    private static PeriodoPostulacionDto MapToDto(PeriodoPostulacion period)
    {
        return new PeriodoPostulacionDto
        {
            Id = period.Id,
            FechaInicio = period.FechaInicio,
            FechaFin = period.FechaFin,
            Activo = period.Activo,
            Descripcion = period.Descripcion,
            FechaCreacion = period.FechaCreacion,
            EstaActivo = period.EstaActivo,
            EstaVigente = period.EstaVigente,
            DiasRestantes = period.DiasRestantes,
            EstadoPeriodo = period.EstadoPeriodo,
            TotalSolicitudes = period.Solicitudes?.Count ?? 0
        };
    }

    private static string GetPeriodMessage(PeriodoPostulacion period)
    {
        var now = DateTime.UtcNow;
        
        if (now < period.FechaInicio)
        {
            var diasHastaInicio = (period.FechaInicio.Date - now.Date).Days;
            return $"El período de postulación iniciará en {diasHastaInicio} días ({period.FechaInicio:dd/MM/yyyy}).";
        }
        
        if (now > period.FechaFin)
        {
            var diasDesdeFin = (now.Date - period.FechaFin.Date).Days;
            return $"El período de postulación finalizó hace {diasDesdeFin} días ({period.FechaFin:dd/MM/yyyy}).";
        }
        
        if (!period.Activo)
        {
            return "Hay un período configurado pero no está activo. Contacte al administrador.";
        }
        
        return $"Período de postulación activo. Quedan {period.DiasRestantes} días (hasta el {period.FechaFin:dd/MM/yyyy}).";
    }

    private async Task SendPeriodActivationNotificationAsync(PeriodoPostulacion period)
    {
        try
        {
            // Get all teachers (UserType = Docente)
            var users = await _userRepository.GetAllAsync();
            var teachers = users.Where(u => u.UserType == ProyectoAgiles.Domain.Enums.UserType.Docente && !string.IsNullOrEmpty(u.Email));

            var subject = "🔔 Nuevo Período de Postulación Abierto - Universidad Técnica de Ambato";
            
            var body = $@"
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background-color: #1e3a8a; color: white; padding: 20px; text-align: center; border-radius: 8px 8px 0 0; }}
                        .content {{ background-color: #f8fafc; padding: 30px; border: 1px solid #e2e8f0; }}
                        .period-info {{ background-color: white; padding: 20px; border-radius: 8px; margin: 20px 0; border-left: 4px solid #10b981; }}
                        .important {{ background-color: #fef3c7; padding: 15px; border-radius: 8px; border-left: 4px solid #f59e0b; margin: 20px 0; }}
                        .footer {{ background-color: #374151; color: white; padding: 15px; text-align: center; border-radius: 0 0 8px 8px; font-size: 12px; }}
                        .button {{ background-color: #10b981; color: white; padding: 12px 24px; text-decoration: none; border-radius: 6px; display: inline-block; margin: 15px 0; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>🎓 Universidad Técnica de Ambato</h1>
                            <h2>Nuevo Período de Postulación</h2>
                        </div>
                        
                        <div class='content'>
                            <p><strong>Estimado/a Docente,</strong></p>
                            
                            <p>Nos complace informarle que se ha <strong>abierto un nuevo período de postulación</strong> para solicitudes de escalafón docente.</p>
                            
                            <div class='period-info'>
                                <h3>📅 Información del Período</h3>
                                <p><strong>Descripción:</strong> {period.Descripcion}</p>
                                <p><strong>Fecha de Inicio:</strong> {period.FechaInicio.ToString("dd/MM/yyyy")}</p>
                                <p><strong>Fecha de Fin:</strong> {period.FechaFin.ToString("dd/MM/yyyy")}</p>
                                <p><strong>Estado:</strong> ✅ Activo</p>
                            </div>
                            
                            <div class='important'>
                                <h4>📝 ¿Qué puede hacer ahora?</h4>
                                <ul>
                                    <li>Iniciar sesión en el sistema</li>
                                    <li>Revisar los requisitos para su nivel objetivo</li>
                                    <li>Preparar la documentación necesaria</li>
                                    <li>Presentar su solicitud de escalafón</li>
                                </ul>
                            </div>
                            
                            <p>
                                <a href='http://localhost:5043/login' class='button'>
                                    🚀 Acceder al Sistema
                                </a>
                            </p>
                            
                            <p><strong>Recuerde:</strong> Este período estará disponible hasta el <strong>{period.FechaFin.ToString("dd/MM/yyyy")}</strong>. Le recomendamos no esperar hasta el último momento para presentar su solicitud.</p>
                            
                            <p>Si tiene alguna pregunta o necesita asistencia, no dude en contactar al departamento de Talento Humano.</p>
                            
                            <p>¡Le deseamos mucho éxito en su proceso de escalafón!</p>
                        </div>
                        
                        <div class='footer'>
                            <p><strong>Universidad Técnica de Ambato</strong><br>
                            Sistema de Gestión de Escalafón Docente<br>
                            Este es un mensaje automático, por favor no responda a este correo.</p>
                        </div>
                    </div>
                </body>
                </html>";

            // Send emails to all teachers
            foreach (var teacher in teachers)
            {
                try
                {
                    await _emailService.SendEmailAsync(teacher.Email, subject, body, true);
                }
                catch (Exception ex)
                {
                    // Log error but continue with other emails
                    Console.WriteLine($"Error sending email to {teacher.Email}: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            // Log error but don't throw to avoid breaking the main operation
            Console.WriteLine($"Error sending period activation notifications: {ex.Message}");
        }
    }
}
