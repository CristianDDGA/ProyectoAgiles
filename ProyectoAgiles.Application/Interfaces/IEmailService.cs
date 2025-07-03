namespace ProyectoAgiles.Application.Interfaces;

public interface IEmailService
{
    Task<bool> SendPasswordResetEmailAsync(string email, string resetToken, string userName);
    Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = true);
    Task<bool> SendAdminNotificationEmailAsync(string email, string subject, string body, bool isHtml = true);
    Task<bool> SendApelacionRechazoEmailAsync(string docenteEmail, string docenteNombre, string motivoRechazo, string rechazadoPor);
    Task<bool> SendApelacionAceptadaEmailAsync(string docenteEmail, string docenteNombre, string observaciones, string aceptadoPor);
}
