using Microsoft.Extensions.Configuration;
using ProyectoAgiles.Application.Interfaces;

namespace ProyectoAgiles.Application.Services;

public class MockEmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public MockEmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<bool> SendPasswordResetEmailAsync(string toEmail, string resetToken, string userName)
    {
        try
        {
            // Simular el envío de email escribiendo a la consola
            var baseUrl = _configuration["AppSettings:BaseUrl"] ?? "http://localhost:5041";
            var resetUrl = $"{baseUrl}/reset-password?token={resetToken}&email={Uri.EscapeDataString(toEmail)}";

            Console.WriteLine("=".PadRight(80, '='));
            Console.WriteLine("EMAIL SIMULADO - RECUPERACIÓN DE CONTRASEÑA");
            Console.WriteLine("=".PadRight(80, '='));
            Console.WriteLine($"Para: {toEmail}");
            Console.WriteLine($"Asunto: Recuperación de Contraseña - Sistema UTA");
            Console.WriteLine($"Usuario: {userName}");
            Console.WriteLine($"Token: {resetToken}");
            Console.WriteLine($"URL de Reset: {resetUrl}");
            Console.WriteLine("=".PadRight(80, '='));
            Console.WriteLine("CONTENIDO DEL EMAIL:");
            Console.WriteLine($"Hola {userName},");
            Console.WriteLine();
            Console.WriteLine("Hemos recibido una solicitud para restablecer la contraseña de tu cuenta.");
            Console.WriteLine();
            Console.WriteLine("Para restablecer tu contraseña, haz clic en el siguiente enlace:");
            Console.WriteLine(resetUrl);
            Console.WriteLine();
            Console.WriteLine("Este enlace expirará en 1 hora por motivos de seguridad.");
            Console.WriteLine("=".PadRight(80, '='));

            await Task.Delay(100); // Simular operación asíncrona
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error en MockEmailService: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = true)
    {
        try
        {
            Console.WriteLine("=".PadRight(80, '='));
            Console.WriteLine("EMAIL SIMULADO");
            Console.WriteLine("=".PadRight(80, '='));
            Console.WriteLine($"Para: {to}");
            Console.WriteLine($"Asunto: {subject}");
            Console.WriteLine($"Es HTML: {isHtml}");
            Console.WriteLine("Contenido:");
            Console.WriteLine(body);
            Console.WriteLine("=".PadRight(80, '='));

            await Task.Delay(100); // Simular operación asíncrona
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error en MockEmailService: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> SendAdminNotificationEmailAsync(string to, string subject, string body, bool isHtml = true)
    {
        try
        {
            var adminBaseUrl = _configuration["AppSettings:AdminBaseUrl"] ?? "http://localhost:5022";
            var processedBody = body.Replace("{{AdminBaseUrl}}", adminBaseUrl);

            Console.WriteLine("=".PadRight(80, '='));
            Console.WriteLine("EMAIL SIMULADO - NOTIFICACIÓN ADMINISTRATIVA");
            Console.WriteLine("=".PadRight(80, '='));
            Console.WriteLine($"Para: {to}");
            Console.WriteLine($"Asunto: {subject}");
            Console.WriteLine($"Es HTML: {isHtml}");
            Console.WriteLine($"URL Base Admin: {adminBaseUrl}");
            Console.WriteLine("Contenido:");
            Console.WriteLine(processedBody);
            Console.WriteLine("=".PadRight(80, '='));

            await Task.Delay(100); // Simular operación asíncrona
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error en MockEmailService: {ex.Message}");
            return false;
        }
    }
}
