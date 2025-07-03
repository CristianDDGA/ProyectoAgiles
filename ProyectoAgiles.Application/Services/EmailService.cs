using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using ProyectoAgiles.Application.Interfaces;

namespace ProyectoAgiles.Application.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<bool> SendPasswordResetEmailAsync(string toEmail, string resetToken, string userName)
    {
        try
        {
            var smtpHost = _configuration["EmailSettings:SmtpHost"] ?? "smtp.gmail.com";
            var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
            var senderEmail = _configuration["EmailSettings:FromEmail"] ?? "";
            var senderPassword = _configuration["EmailSettings:SmtpPassword"] ?? "";
            var senderName = _configuration["EmailSettings:FromName"] ?? "Universidad Técnica de Ambato";

            if (string.IsNullOrEmpty(senderEmail) || string.IsNullOrEmpty(senderPassword))
            {
                throw new InvalidOperationException("La configuración de email no está completa.");
            }

            // Construir la URL de reset
            var baseUrl = _configuration["AppSettings:BaseUrl"] ?? "http://localhost:5041";
            var resetUrl = $"{baseUrl}/reset-password?token={resetToken}&email={Uri.EscapeDataString(toEmail)}";

            // Crear el mensaje de email
            var mailMessage = new MailMessage
            {
                From = new MailAddress(senderEmail, senderName),
                Subject = "Recuperación de Contraseña - Sistema UTA",
                Body = GeneratePasswordResetEmailBody(userName, resetUrl),
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            // Configurar el cliente SMTP
            using var smtpClient = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(_configuration["EmailSettings:SmtpUsername"], senderPassword),
                EnableSsl = bool.Parse(_configuration["EmailSettings:EnableSsl"] ?? "true")
            };

            await smtpClient.SendMailAsync(mailMessage);
            return true;
        }        catch (Exception ex)
        {
            // En producción, usar un logger aquí
            Console.WriteLine($"=== ERROR ENVIANDO EMAIL ===");
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine($"Tipo: {ex.GetType().Name}");
            Console.WriteLine($"InnerException: {ex.InnerException?.Message}");
            Console.WriteLine($"Host: {_configuration["EmailSettings:SmtpHost"]}:{_configuration["EmailSettings:SmtpPort"]}");
            Console.WriteLine($"Username: {_configuration["EmailSettings:SmtpUsername"]}");
            Console.WriteLine($"EnableSsl: {_configuration["EmailSettings:EnableSsl"]}");
            Console.WriteLine($"=============================");
            return false;
        }
    }

    public async Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = true)
    {
        try
        {
            var smtpHost = _configuration["EmailSettings:SmtpHost"] ?? "smtp.gmail.com";
            var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
            var senderEmail = _configuration["EmailSettings:FromEmail"] ?? "";
            var senderPassword = _configuration["EmailSettings:SmtpPassword"] ?? "";
            var senderName = _configuration["EmailSettings:FromName"] ?? "Universidad Técnica de Ambato";

            if (string.IsNullOrEmpty(senderEmail) || string.IsNullOrEmpty(senderPassword))
            {
                throw new InvalidOperationException("La configuración de email no está completa.");
            }

            var mailMessage = new MailMessage
            {
                From = new MailAddress(senderEmail, senderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };

            mailMessage.To.Add(to);

            using var smtpClient = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(_configuration["EmailSettings:SmtpUsername"], senderPassword),
                EnableSsl = bool.Parse(_configuration["EmailSettings:EnableSsl"] ?? "true")
            };

            await smtpClient.SendMailAsync(mailMessage);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error enviando email: {ex.Message}");
            return false;
        }
    }

    private static string GeneratePasswordResetEmailBody(string userName, string resetUrl)
    {
        return $@"
        <!DOCTYPE html>
        <html>
        <head>
            <style>
                body {{ font-family: Arial, sans-serif; margin: 0; padding: 20px; background-color: #f5f5f5; }}
                .container {{ max-width: 600px; margin: 0 auto; background-color: white; padding: 30px; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }}
                .header {{ text-align: center; margin-bottom: 30px; }}
                .logo {{ color: #ff4757; font-size: 24px; font-weight: bold; }}
                .content {{ line-height: 1.6; color: #333; }}
                .button {{ display: inline-block; background-color: #ff4757; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; margin: 20px 0; }}
                .footer {{ margin-top: 30px; padding-top: 20px; border-top: 1px solid #eee; font-size: 12px; color: #666; text-align: center; }}
            </style>
        </head>
        <body>
            <div class=""container"">
                <div class=""header"">
                    <div class=""logo"">Universidad Técnica de Ambato</div>
                    <h2>Recuperación de Contraseña</h2>
                </div>
                
                <div class=""content"">
                    <p>Hola {userName},</p>
                    
                    <p>Hemos recibido una solicitud para restablecer la contraseña de tu cuenta en el Sistema UTA.</p>
                    
                    <p>Para restablecer tu contraseña, haz clic en el siguiente enlace:</p>
                    
                    <p style=""text-align: center;"">
                        <a href=""{resetUrl}"" class=""button"">Restablecer Contraseña</a>
                    </p>
                    
                    <p><strong>Este enlace expirará en 24 horas por motivos de seguridad.</strong></p>
                    
                    <p>Si no solicitaste este cambio, puedes ignorar este email. Tu contraseña actual permanecerá sin cambios.</p>
                    
                    <p>Si tienes problemas con el enlace, puedes copiar y pegar la siguiente URL en tu navegador:</p>
                    <p style=""word-break: break-all; color: #666; font-size: 12px;"">{resetUrl}</p>
                </div>
                
                <div class=""footer"">
                    <p>Este es un mensaje automático, por favor no respondas a este email.</p>
                    <p>© 2025 Universidad Técnica de Ambato - Sistema de Gestión Académica</p>
                </div>
            </div>
        </body>
        </html>";
    }

    public async Task<bool> SendAdminNotificationEmailAsync(string toEmail, string subject, string body, bool isHtml = true)
    {
        try
        {
            var smtpHost = _configuration["EmailSettings:SmtpHost"] ?? "smtp.gmail.com";
            var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
            var senderEmail = _configuration["EmailSettings:FromEmail"] ?? "";
            var senderPassword = _configuration["EmailSettings:SmtpPassword"] ?? "";
            var senderName = _configuration["EmailSettings:FromName"] ?? "Universidad Técnica de Ambato";

            if (string.IsNullOrEmpty(senderEmail) || string.IsNullOrEmpty(senderPassword))
            {
                throw new InvalidOperationException("La configuración de email no está completa.");
            }

            // Para notificaciones administrativas, usar la URL administrativa
            var adminBaseUrl = _configuration["AppSettings:AdminBaseUrl"] ?? "http://localhost:5022";
            
            // Si el body contiene enlaces relativos, reemplazarlos con la URL administrativa
            var processedBody = body.Replace("{{AdminBaseUrl}}", adminBaseUrl);

            var mailMessage = new MailMessage
            {
                From = new MailAddress(senderEmail, senderName),
                Subject = subject,
                Body = processedBody,
                IsBodyHtml = isHtml
            };

            mailMessage.To.Add(toEmail);

            using var smtpClient = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(_configuration["EmailSettings:SmtpUsername"], senderPassword),
                EnableSsl = bool.Parse(_configuration["EmailSettings:EnableSsl"] ?? "true")
            };

            await smtpClient.SendMailAsync(mailMessage);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error enviando email administrativo: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> SendApelacionRechazoEmailAsync(string docenteEmail, string docenteNombre, string motivoRechazo, string rechazadoPor)
    {
        try
        {
            var subject = "Resolución de Apelación - Universidad Técnica de Ambato";
            var body = GenerateApelacionRechazoEmailBody(docenteNombre, motivoRechazo, rechazadoPor);
            
            return await SendEmailAsync(docenteEmail, subject, body);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error enviando email de rechazo de apelación: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> SendApelacionAceptadaEmailAsync(string docenteEmail, string docenteNombre, string observaciones, string aceptadoPor)
    {
        try
        {
            var subject = "Apelación Aceptada - Universidad Técnica de Ambato";
            var body = GenerateApelacionAceptadaEmailBody(docenteNombre, observaciones, aceptadoPor);
            
            return await SendEmailAsync(docenteEmail, subject, body);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error enviando email de aceptación de apelación: {ex.Message}");
            return false;
        }
    }

    private string GenerateApelacionRechazoEmailBody(string docenteNombre, string motivoRechazo, string rechazadoPor)
    {
        return $@"
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset='UTF-8'>
            <style>
                body {{ font-family: Arial, sans-serif; margin: 0; padding: 20px; background-color: #f5f5f5; }}
                .container {{ max-width: 600px; margin: 0 auto; background-color: white; padding: 30px; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }}
                .header {{ background-color: #dc3545; color: white; padding: 20px; border-radius: 5px; text-align: center; margin-bottom: 30px; }}
                .header h1 {{ margin: 0; font-size: 24px; }}
                .content {{ line-height: 1.6; color: #333; }}
                .alert {{ background-color: #f8d7da; color: #721c24; padding: 15px; border-radius: 5px; margin: 20px 0; border-left: 4px solid #dc3545; }}
                .info-box {{ background-color: #f8f9fa; padding: 20px; border-radius: 5px; margin: 20px 0; border-left: 4px solid #007bff; }}
                .footer {{ background-color: #f8f9fa; padding: 20px; border-radius: 5px; margin-top: 30px; text-align: center; color: #666; }}
                .highlight {{ background-color: #fff3cd; padding: 10px; border-radius: 3px; margin: 10px 0; }}
                .signature {{ margin-top: 30px; padding-top: 20px; border-top: 1px solid #eee; }}
                .logo {{ text-align: center; margin-bottom: 20px; }}
                .logo img {{ max-width: 200px; }}
            </style>
        </head>
        <body>
            <div class='container'>
                <div class='header'>
                    <h1>🚫 Resolución de Apelación</h1>
                    <p>Universidad Técnica de Ambato</p>
                </div>

                <div class='content'>
                    <p><strong>Estimado/a {docenteNombre},</strong></p>
                    
                    <div class='alert'>
                        <strong>Su apelación ha sido RECHAZADA</strong>
                    </div>
                    
                    <p>Nos dirigimos a usted para informarle que su apelación presentada ante la Comisión Académica de Escalafón ha sido revisada y <strong>rechazada</strong> por las siguientes razones:</p>
                    
                    <div class='info-box'>
                        <h3>📋 Motivo del Rechazo:</h3>
                        <p>{motivoRechazo}</p>
                    </div>
                    
                    <div class='highlight'>
                        <strong>⚠️ Importante:</strong> Con esta resolución, su solicitud de escalafón queda en estado <strong>""Rechazado Definitivo""</strong>.
                    </div>
                    
                    <p>Esta decisión ha sido tomada por la Comisión Académica de Escalafón después de una revisión exhaustiva de su apelación y la documentación presentada.</p>
                    
                    <div class='info-box'>
                        <h3>📞 Información de Contacto:</h3>
                        <p>Si requiere información adicional o tiene consultas sobre esta resolución, puede contactar a:</p>
                        <ul>
                            <li><strong>Dirección de Talento Humano</strong></li>
                            <li><strong>Teléfono:</strong> (03) 2848-487</li>
                            <li><strong>Email:</strong> talentohumano@uta.edu.ec</li>
                        </ul>
                    </div>
                    
                    <div class='signature'>
                        <p><strong>Procesado por:</strong> {rechazadoPor}</p>
                        <p><strong>Fecha:</strong> {DateTime.Now:dd 'de' MMMM 'de' yyyy}</p>
                        <p><strong>Hora:</strong> {DateTime.Now:HH:mm}</p>
                    </div>
                </div>
                
                <div class='footer'>
                    <p><strong>Universidad Técnica de Ambato</strong></p>
                    <p>Dirección de Talento Humano - Comisión Académica de Escalafón</p>
                    <p>Este es un mensaje automatizado, por favor no responda a este correo.</p>
                </div>
            </div>
        </body>
        </html>";
    }

    private string GenerateApelacionAceptadaEmailBody(string docenteNombre, string observaciones, string aceptadoPor)
    {
        return $@"
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset='UTF-8'>
            <style>
                body {{ font-family: Arial, sans-serif; margin: 0; padding: 20px; background-color: #f5f5f5; }}
                .container {{ max-width: 600px; margin: 0 auto; background-color: white; padding: 30px; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }}
                .header {{ background-color: #28a745; color: white; padding: 20px; border-radius: 5px; text-align: center; margin-bottom: 30px; }}
                .header h1 {{ margin: 0; font-size: 24px; }}
                .content {{ line-height: 1.6; color: #333; }}
                .alert {{ background-color: #d4edda; color: #155724; padding: 15px; border-radius: 5px; margin: 20px 0; border-left: 4px solid #28a745; }}
                .info-box {{ background-color: #f8f9fa; padding: 20px; border-radius: 5px; margin: 20px 0; border-left: 4px solid #007bff; }}
                .footer {{ background-color: #f8f9fa; padding: 20px; border-radius: 5px; margin-top: 30px; text-align: center; color: #666; }}
                .highlight {{ background-color: #d1ecf1; padding: 10px; border-radius: 3px; margin: 10px 0; }}
                .signature {{ margin-top: 30px; padding-top: 20px; border-top: 1px solid #eee; }}
            </style>
        </head>
        <body>
            <div class='container'>
                <div class='header'>
                    <h1>✅ Apelación Aceptada</h1>
                    <p>Universidad Técnica de Ambato</p>
                </div>

                <div class='content'>
                    <p><strong>Estimado/a {docenteNombre},</strong></p>
                    
                    <div class='alert'>
                        <strong>¡Su apelación ha sido ACEPTADA!</strong>
                    </div>
                    
                    <p>Nos complace informarle que su apelación presentada ante la Comisión Académica de Escalafón ha sido <strong>aceptada</strong>.</p>
                    
                    <div class='highlight'>
                        <strong>📋 Próximos pasos:</strong> Su solicitud de escalafón regresará al estado <strong>""Pendiente""</strong> para ser reevaluada desde el inicio del proceso.
                    </div>
                    
                    {(string.IsNullOrEmpty(observaciones) ? "" : $@"
                    <div class='info-box'>
                        <h3>📝 Observaciones:</h3>
                        <p>{observaciones}</p>
                    </div>
                    ")}
                    
                    <p>La Comisión Académica procederá a realizar una nueva evaluación de su solicitud, considerando los argumentos presentados en su apelación.</p>
                    
                    <div class='info-box'>
                        <h3>📞 Información de Contacto:</h3>
                        <p>Si tiene consultas adicionales sobre el proceso, puede contactar a:</p>
                        <ul>
                            <li><strong>Dirección de Talento Humano</strong></li>
                            <li><strong>Teléfono:</strong> (03) 2848-487</li>
                            <li><strong>Email:</strong> talentohumano@uta.edu.ec</li>
                        </ul>
                    </div>
                    
                    <div class='signature'>
                        <p><strong>Procesado por:</strong> {aceptadoPor}</p>
                        <p><strong>Fecha:</strong> {DateTime.Now:dd 'de' MMMM 'de' yyyy}</p>
                        <p><strong>Hora:</strong> {DateTime.Now:HH:mm}</p>
                    </div>
                </div>
                
                <div class='footer'>
                    <p><strong>Universidad Técnica de Ambato</strong></p>
                    <p>Dirección de Talento Humano - Comisión Académica de Escalafón</p>
                    <p>Este es un mensaje automatizado, por favor no responda a este correo.</p>
                </div>
            </div>
        </body>
        </html>";
    }

    public async Task<bool> SendSolicitudAprobadaEmailAsync(string docenteEmail, string docenteNombre, string nivelActual, string nivelSolicitado, DateTime fechaSolicitud, DateTime fechaAprobacion, string observaciones = "")
    {
        try
        {
            var subject = "✅ Solicitud de Escalafón Aprobada - Universidad Técnica de Ambato";
            var body = GenerateSolicitudAprobadaEmailBody(docenteNombre, nivelActual, nivelSolicitado, fechaSolicitud, fechaAprobacion, observaciones);
            
            return await SendEmailAsync(docenteEmail, subject, body, true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error enviando correo de aprobación de solicitud: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> SendSolicitudRechazadaEmailAsync(string docenteEmail, string docenteNombre, string nivelActual, string nivelSolicitado, DateTime fechaSolicitud, DateTime fechaRechazo, string motivoRechazo, string rechazadoPor, string nivelRechazo)
    {
        try
        {
            var subject = "❌ Solicitud de Escalafón Rechazada - Universidad Técnica de Ambato";
            var body = GenerateSolicitudRechazadaEmailBody(docenteNombre, nivelActual, nivelSolicitado, fechaSolicitud, fechaRechazo, motivoRechazo, rechazadoPor, nivelRechazo);
            
            return await SendEmailAsync(docenteEmail, subject, body, true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error enviando correo de rechazo de solicitud: {ex.Message}");
            return false;
        }
    }

    private string GenerateSolicitudAprobadaEmailBody(string docenteNombre, string nivelActual, string nivelSolicitado, DateTime fechaSolicitud, DateTime fechaAprobacion, string observaciones)
    {
        return $@"
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset='utf-8'>
            <style>
                .container {{ max-width: 600px; margin: 0 auto; font-family: Arial, sans-serif; }}
                .header {{ background: linear-gradient(135deg, #4CAF50 0%, #45a049 100%); color: white; padding: 20px; text-align: center; }}
                .content {{ padding: 20px; }}
                .footer {{ background-color: #f8f9fa; padding: 15px; text-align: center; font-size: 12px; }}
                .info-box {{ background-color: #e8f5e8; border-left: 4px solid #4CAF50; padding: 15px; margin: 15px 0; }}
                .success-badge {{ background: #4CAF50; color: white; padding: 5px 10px; border-radius: 15px; }}
                .highlight {{ background: #d4edda; color: #155724; padding: 10px; border-radius: 5px; margin: 10px 0; }}
                .signature {{ margin-top: 30px; padding-top: 15px; border-top: 1px solid #eee; }}
            </style>
        </head>
        <body>
            <div class='container'>
                <div class='header'>
                    <h2>🎉 ¡Felicidades!</h2>
                    <p>Su solicitud de escalafón ha sido <span class='success-badge'>APROBADA</span></p>
                </div>
                
                <div class='content'>
                    <h3>Estimado/a {docenteNombre},</h3>
                    
                    <p>Nos complace informarle que su solicitud de escalafón docente ha sido <strong>aprobada</strong> por la Comisión Académica de Escalafón.</p>
                    
                    <div class='info-box'>
                        <h3>📋 Detalles de la solicitud:</h3>
                        <ul>
                            <li><strong>Nivel actual:</strong> {nivelActual}</li>
                            <li><strong>Nivel aprobado:</strong> {nivelSolicitado}</li>
                            <li><strong>Fecha de solicitud:</strong> {fechaSolicitud:dd/MM/yyyy}</li>
                            <li><strong>Fecha de aprobación:</strong> {fechaAprobacion:dd/MM/yyyy}</li>
                        </ul>
                    </div>
                    
                    {(string.IsNullOrEmpty(observaciones) ? "" : $@"
                    <div class='info-box'>
                        <h3>📝 Observaciones:</h3>
                        <p>{observaciones}</p>
                    </div>
                    ")}
                    
                    <div class='highlight'>
                        <strong>🏆 ¡Felicitaciones por este logro académico!</strong><br>
                        Su nueva categoría docente entrará en vigencia según los procedimientos establecidos por la institución.
                    </div>
                    
                    <div class='info-box'>
                        <h3>📞 Información de Contacto:</h3>
                        <p>Si tiene alguna consulta sobre el proceso, puede contactar a:</p>
                        <ul>
                            <li><strong>Dirección de Talento Humano</strong></li>
                            <li><strong>Teléfono:</strong> (03) 2848-487</li>
                            <li><strong>Email:</strong> talentohumano@uta.edu.ec</li>
                        </ul>
                    </div>
                    
                    <div class='signature'>
                        <p><strong>Fecha de notificación:</strong> {DateTime.Now:dd 'de' MMMM 'de' yyyy}</p>
                        <p><strong>Hora:</strong> {DateTime.Now:HH:mm}</p>
                    </div>
                </div>
                
                <div class='footer'>
                    <p><strong>Universidad Técnica de Ambato</strong></p>
                    <p>Dirección de Talento Humano - Comisión Académica de Escalafón</p>
                    <p>Este es un mensaje automatizado, por favor no responda a este correo.</p>
                </div>
            </div>
        </body>
        </html>";
    }

    private string GenerateSolicitudRechazadaEmailBody(string docenteNombre, string nivelActual, string nivelSolicitado, DateTime fechaSolicitud, DateTime fechaRechazo, string motivoRechazo, string rechazadoPor, string nivelRechazo)
    {
        var nivelTexto = nivelRechazo switch
        {
            "PresidenteComision" => "Presidente de la Comisión Académica",
            "DireccionTalentoHumano" => "Dirección de Talento Humano",
            "ComisionAcademica" => "Comisión Académica de Escalafón",
            _ => "Administración"
        };

        return $@"
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset='utf-8'>
            <style>
                .container {{ max-width: 600px; margin: 0 auto; font-family: Arial, sans-serif; }}
                .header {{ background: linear-gradient(135deg, #f44336 0%, #d32f2f 100%); color: white; padding: 20px; text-align: center; }}
                .content {{ padding: 20px; }}
                .footer {{ background-color: #f8f9fa; padding: 15px; text-align: center; font-size: 12px; }}
                .info-box {{ background-color: #ffeaa7; border-left: 4px solid #fdcb6e; padding: 15px; margin: 15px 0; }}
                .error-box {{ background-color: #ffebee; border-left: 4px solid #f44336; padding: 15px; margin: 15px 0; }}
                .appeal-box {{ background-color: #e3f2fd; border-left: 4px solid #2196f3; padding: 15px; margin: 15px 0; }}
                .warning {{ color: #d32f2f; font-weight: bold; }}
                .signature {{ margin-top: 30px; padding-top: 15px; border-top: 1px solid #eee; }}
            </style>
        </head>
        <body>
            <div class='container'>
                <div class='header'>
                    <h2>❌ Solicitud de Escalafón Rechazada</h2>
                    <p>Universidad Técnica de Ambato</p>
                </div>
                
                <div class='content'>
                    <h3>Estimado/a {docenteNombre},</h3>
                    
                    <p>Lamentamos informarle que su solicitud de escalafón docente ha sido <span class='warning'>rechazada</span> por <strong>{nivelTexto}</strong>.</p>
                    
                    <div class='info-box'>
                        <h3>📋 Detalles de la solicitud:</h3>
                        <ul>
                            <li><strong>Nivel actual:</strong> {nivelActual}</li>
                            <li><strong>Nivel solicitado:</strong> {nivelSolicitado}</li>
                            <li><strong>Fecha de solicitud:</strong> {fechaSolicitud:dd/MM/yyyy}</li>
                            <li><strong>Fecha de rechazo:</strong> {fechaRechazo:dd/MM/yyyy}</li>
                            <li><strong>Rechazado por:</strong> {rechazadoPor}</li>
                            <li><strong>Nivel de rechazo:</strong> {nivelTexto}</li>
                        </ul>
                    </div>
                    
                    <div class='error-box'>
                        <h3>📝 Motivo del rechazo:</h3>
                        <p>{motivoRechazo}</p>
                    </div>
                    
                    <div class='appeal-box'>
                        <h3>📢 Derecho de Apelación</h3>
                        <p><strong>Usted tiene derecho a apelar esta decisión.</strong> Para presentar una apelación, siga estos pasos:</p>
                        <ol>
                            <li><strong>Ingrese a su dashboard</strong> en el sistema de escalafón docente</li>
                            <li><strong>Vaya a la sección ""Mis Solicitudes""</strong></li>
                            <li><strong>Busque la solicitud rechazada</strong> y haga clic en ""Ver detalles""</li>
                            <li><strong>Presione el botón ""Apelar""</strong> que aparecerá en la interfaz</li>
                            <li><strong>Complete el formulario de apelación</strong> con:
                                <ul>
                                    <li>Justificación detallada de su apelación</li>
                                    <li>Documentación adicional que respalde su caso</li>
                                    <li>Selección del destinatario de la apelación</li>
                                </ul>
                            </li>
                            <li><strong>Adjunte los documentos necesarios</strong> que considere relevantes</li>
                            <li><strong>Envíe la apelación</strong> para su revisión</li>
                        </ol>
                        
                        <p><strong>📅 Tiempo para apelar:</strong> Puede presentar su apelación en cualquier momento desde su dashboard.</p>
                        
                        <p><strong>📋 Documentos recomendados para la apelación:</strong></p>
                        <ul>
                            <li>Documentos adicionales que respalden su solicitud</li>
                            <li>Cartas de recomendación actualizadas</li>
                            <li>Certificados o títulos adicionales</li>
                            <li>Evidencia de experiencia docente o investigativa</li>
                        </ul>
                    </div>
                    
                    <div class='info-box'>
                        <h3>📞 Información de Contacto:</h3>
                        <p>Si tiene consultas sobre el proceso de apelación, puede contactar a:</p>
                        <ul>
                            <li><strong>Dirección de Talento Humano</strong></li>
                            <li><strong>Teléfono:</strong> (03) 2848-487</li>
                            <li><strong>Email:</strong> talentohumano@uta.edu.ec</li>
                            <li><strong>Horario de atención:</strong> Lunes a Viernes, 8:00 AM - 5:00 PM</li>
                        </ul>
                    </div>
                    
                    <div class='signature'>
                        <p><strong>Procesado por:</strong> {rechazadoPor}</p>
                        <p><strong>Fecha de notificación:</strong> {DateTime.Now:dd 'de' MMMM 'de' yyyy}</p>
                        <p><strong>Hora:</strong> {DateTime.Now:HH:mm}</p>
                    </div>
                </div>
                
                <div class='footer'>
                    <p><strong>Universidad Técnica de Ambato</strong></p>
                    <p>Dirección de Talento Humano - Comisión Académica de Escalafón</p>
                    <p>Este es un mensaje automatizado, por favor no responda a este correo.</p>
                </div>
            </div>
        </body>
        </html>";
    }
}
