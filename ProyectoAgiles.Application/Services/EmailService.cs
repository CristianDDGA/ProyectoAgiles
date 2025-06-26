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
}
