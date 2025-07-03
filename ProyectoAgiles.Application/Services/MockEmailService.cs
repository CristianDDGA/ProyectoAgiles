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

    public async Task<bool> SendSolicitudAprobadaEmailAsync(string docenteEmail, string docenteNombre, string nivelActual, string nivelSolicitado, DateTime fechaSolicitud, DateTime fechaAprobacion, string observaciones = "")
    {
        try
        {
            Console.WriteLine("=".PadRight(80, '='));
            Console.WriteLine("EMAIL SIMULADO - SOLICITUD APROBADA");
            Console.WriteLine("=".PadRight(80, '='));
            Console.WriteLine($"Para: {docenteEmail}");
            Console.WriteLine($"Docente: {docenteNombre}");
            Console.WriteLine($"Nivel actual: {nivelActual}");
            Console.WriteLine($"Nivel aprobado: {nivelSolicitado}");
            Console.WriteLine($"Fecha solicitud: {fechaSolicitud:dd/MM/yyyy}");
            Console.WriteLine($"Fecha aprobación: {fechaAprobacion:dd/MM/yyyy}");
            Console.WriteLine("=".PadRight(80, '='));
            Console.WriteLine("ASUNTO: ✅ Solicitud de Escalafón Aprobada - Universidad Técnica de Ambato");
            Console.WriteLine();
            Console.WriteLine($"Estimado/a {docenteNombre},");
            Console.WriteLine();
            Console.WriteLine("¡Felicidades! Su solicitud de escalafón ha sido APROBADA por la Comisión Académica.");
            Console.WriteLine();
            Console.WriteLine("DETALLES:");
            Console.WriteLine($"• Nivel actual: {nivelActual}");
            Console.WriteLine($"• Nivel aprobado: {nivelSolicitado}");
            Console.WriteLine($"• Fecha de solicitud: {fechaSolicitud:dd/MM/yyyy}");
            Console.WriteLine($"• Fecha de aprobación: {fechaAprobacion:dd/MM/yyyy}");
            Console.WriteLine();
            if (!string.IsNullOrEmpty(observaciones))
            {
                Console.WriteLine("OBSERVACIONES:");
                Console.WriteLine($"{observaciones}");
                Console.WriteLine();
            }
            Console.WriteLine("Su nueva categoría docente entrará en vigencia según los procedimientos establecidos.");
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
            Console.WriteLine($"Error en MockEmailService (Aprobación Solicitud): {ex.Message}");
            return false;
        }
    }

    public async Task<bool> SendSolicitudRechazadaEmailAsync(string docenteEmail, string docenteNombre, string nivelActual, string nivelSolicitado, DateTime fechaSolicitud, DateTime fechaRechazo, string motivoRechazo, string rechazadoPor, string nivelRechazo)
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

            Console.WriteLine("=".PadRight(80, '='));
            Console.WriteLine("EMAIL SIMULADO - SOLICITUD RECHAZADA");
            Console.WriteLine("=".PadRight(80, '='));
            Console.WriteLine($"Para: {docenteEmail}");
            Console.WriteLine($"Docente: {docenteNombre}");
            Console.WriteLine($"Nivel actual: {nivelActual}");
            Console.WriteLine($"Nivel solicitado: {nivelSolicitado}");
            Console.WriteLine($"Fecha solicitud: {fechaSolicitud:dd/MM/yyyy}");
            Console.WriteLine($"Fecha rechazo: {fechaRechazo:dd/MM/yyyy}");
            Console.WriteLine($"Rechazado por: {rechazadoPor} ({nivelTexto})");
            Console.WriteLine("=".PadRight(80, '='));
            Console.WriteLine("ASUNTO: ❌ Solicitud de Escalafón Rechazada - Universidad Técnica de Ambato");
            Console.WriteLine();
            Console.WriteLine($"Estimado/a {docenteNombre},");
            Console.WriteLine();
            Console.WriteLine($"Su solicitud de escalafón ha sido rechazada por {nivelTexto}.");
            Console.WriteLine();
            Console.WriteLine("DETALLES:");
            Console.WriteLine($"• Nivel actual: {nivelActual}");
            Console.WriteLine($"• Nivel solicitado: {nivelSolicitado}");
            Console.WriteLine($"• Fecha de solicitud: {fechaSolicitud:dd/MM/yyyy}");
            Console.WriteLine($"• Fecha de rechazo: {fechaRechazo:dd/MM/yyyy}");
            Console.WriteLine($"• Rechazado por: {rechazadoPor}");
            Console.WriteLine($"• Nivel de rechazo: {nivelTexto}");
            Console.WriteLine();
            Console.WriteLine("MOTIVO DEL RECHAZO:");
            Console.WriteLine($"{motivoRechazo}");
            Console.WriteLine();
            Console.WriteLine("📢 DERECHO DE APELACIÓN:");
            Console.WriteLine("Usted tiene derecho a apelar esta decisión. Para ello:");
            Console.WriteLine("1. Ingrese a su dashboard en el sistema");
            Console.WriteLine("2. Vaya a la sección 'Mis Solicitudes'");
            Console.WriteLine("3. Busque la solicitud rechazada");
            Console.WriteLine("4. Haga clic en el botón 'Apelar'");
            Console.WriteLine("5. Complete el formulario con documentación adicional");
            Console.WriteLine();
            Console.WriteLine("Puede presentar su apelación en cualquier momento desde su dashboard.");
            Console.WriteLine();
            Console.WriteLine("Para consultas: talentohumano@uta.edu.ec | (03) 2848-487");
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
            Console.WriteLine($"Error en MockEmailService (Rechazo Solicitud): {ex.Message}");
            return false;
        }
    }
}
