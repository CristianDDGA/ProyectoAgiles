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

    public async Task<bool> SendApelacionRechazoEmailAsync(string docenteEmail, string docenteNombre, string motivoRechazo, string rechazadoPor)
    {
        try
        {
            Console.WriteLine("=".PadRight(80, '='));
            Console.WriteLine("📧 EMAIL SIMULADO - APELACIÓN RECHAZADA");
            Console.WriteLine("=".PadRight(80, '='));
            Console.WriteLine($"Para: {docenteEmail}");
            Console.WriteLine($"Docente: {docenteNombre}");
            Console.WriteLine($"Rechazada por: {rechazadoPor}");
            Console.WriteLine($"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}");
            Console.WriteLine("=".PadRight(80, '='));
            Console.WriteLine("ASUNTO: Resolución de Apelación - Universidad Técnica de Ambato");
            Console.WriteLine();
            Console.WriteLine($"Estimado/a {docenteNombre},");
            Console.WriteLine();
            Console.WriteLine("Su apelación ha sido RECHAZADA por la Comisión Académica de Escalafón.");
            Console.WriteLine();
            Console.WriteLine("MOTIVO DEL RECHAZO:");
            Console.WriteLine($"{motivoRechazo}");
            Console.WriteLine();
            Console.WriteLine("Su solicitud queda en estado 'Rechazado Definitivo'.");
            Console.WriteLine();
            Console.WriteLine("Atentamente,");
            Console.WriteLine("Comisión Académica de Escalafón");
            Console.WriteLine("Universidad Técnica de Ambato");
            Console.WriteLine("=".PadRight(80, '='));

            await Task.Delay(100);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error en MockEmailService (Rechazo Apelación): {ex.Message}");
            return false;
        }
    }

    public async Task<bool> SendApelacionAceptadaEmailAsync(string docenteEmail, string docenteNombre, string observaciones, string aceptadoPor)
    {
        try
        {
            Console.WriteLine("=".PadRight(80, '='));
            Console.WriteLine("📧 EMAIL SIMULADO - APELACIÓN ACEPTADA");
            Console.WriteLine("=".PadRight(80, '='));
            Console.WriteLine($"Para: {docenteEmail}");
            Console.WriteLine($"Docente: {docenteNombre}");
            Console.WriteLine($"Aceptada por: {aceptadoPor}");
            Console.WriteLine($"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}");
            Console.WriteLine("=".PadRight(80, '='));
            Console.WriteLine("ASUNTO: Apelación Aceptada - Universidad Técnica de Ambato");
            Console.WriteLine();
            Console.WriteLine($"Estimado/a {docenteNombre},");
            Console.WriteLine();
            Console.WriteLine("¡Su apelación ha sido ACEPTADA por la Comisión Académica de Escalafón!");
            Console.WriteLine();
            Console.WriteLine("Su solicitud regresará al estado 'Pendiente' para ser reevaluada.");
            Console.WriteLine();
            if (!string.IsNullOrEmpty(observaciones))
            {
                Console.WriteLine("OBSERVACIONES:");
                Console.WriteLine($"{observaciones}");
                Console.WriteLine();
            }
            Console.WriteLine("Atentamente,");
            Console.WriteLine("Comisión Académica de Escalafón");
            Console.WriteLine("Universidad Técnica de Ambato");
            Console.WriteLine("=".PadRight(80, '='));

            await Task.Delay(100);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error en MockEmailService (Aceptación Apelación): {ex.Message}");
            return false;
        }
    }
}
