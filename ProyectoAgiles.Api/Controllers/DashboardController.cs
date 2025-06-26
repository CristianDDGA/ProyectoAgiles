using Microsoft.AspNetCore.Mvc;
using ProyectoAgiles.Application.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace ProyectoAgiles.Api.Controllers;

/// <summary>
/// 📊 Controlador del Dashboard Principal
/// </summary>
/// <remarks>
/// Este controlador proporciona los datos y métricas principales para el dashboard administrativo,
/// incluyendo estadísticas del sistema y actividades recientes.
/// 
/// <para>
/// <strong>Funcionalidades del dashboard:</strong>
/// </para>
/// <list type="bullet">
/// <item><description>📈 Estadísticas generales del sistema</description></item>
/// <item><description>👥 Métricas de usuarios activos</description></item>
/// <item><description>📋 Actividades recientes</description></item>
/// <item><description>🎯 Indicadores clave de rendimiento</description></item>
/// </list>
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Tags("📊 Dashboard")]
public class DashboardController : ControllerBase
{
    private readonly IUserService _userService;

    public DashboardController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// 📈 Obtener estadísticas del dashboard
    /// </summary>
    /// <remarks>
    /// Proporciona un resumen estadístico completo del sistema para el dashboard principal.
    /// 
    /// <para><strong>Métricas incluidas:</strong></para>
    /// <list type="bullet">
    /// <item><description>👥 Total de usuarios registrados</description></item>
    /// <item><description>✅ Usuarios activos vs inactivos</description></item>
    /// <item><description>🎓 Distribución por roles (Docentes, Admins, etc.)</description></item>
    /// <item><description>📊 Estadísticas de actividad mensual</description></item>
    /// <item><description>📈 Tendencias de crecimiento</description></item>
    /// </list>
    /// 
    /// <para><strong>💡 Uso típico:</strong> Vista principal del dashboard administrativo</para>
    /// </remarks>
    /// <returns>Objeto con todas las estadísticas del sistema</returns>
    /// <response code="200">✅ Estadísticas obtenidas exitosamente</response>
    /// <response code="500">💥 Error interno del servidor</response>
    [HttpGet("stats")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
        Summary = "Obtener estadísticas del dashboard",
        Description = "Recupera métricas y estadísticas principales del sistema para el dashboard",
        OperationId = "GetDashboardStats",
        Tags = new[] { "📊 Dashboard" }
    )]
    public async Task<ActionResult> GetDashboardStats()
    {
        try
        {
            var stats = await _userService.GetDashboardStatsAsync();
            return Ok(stats);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
        }
    }

    /// <summary>
    /// 🕐 Obtener actividades recientes del sistema
    /// </summary>
    /// <remarks>
    /// Recupera una lista de las actividades más recientes en el sistema para monitoreo en tiempo real.
    /// 
    /// <para><strong>Tipos de actividades incluidas:</strong></para>
    /// <list type="bullet">
    /// <item><description>👤 Nuevos registros de usuarios</description></item>
    /// <item><description>🔄 Actualizaciones de perfil</description></item>
    /// <item><description>🎓 Ascensos académicos</description></item>
    /// <item><description>🔐 Inicios de sesión recientes</description></item>
    /// <item><description>📋 Cambios de estado</description></item>
    /// </list>
    /// 
    /// <para><strong>Información por actividad:</strong></para>
    /// <list type="bullet">
    /// <item><description>⏰ Timestamp de la actividad</description></item>
    /// <item><description>👤 Usuario que realizó la acción</description></item>
    /// <item><description>🔄 Tipo de actividad realizada</description></item>
    /// <item><description>📝 Descripción detallada</description></item>
    /// </list>
    /// 
    /// <para><strong>💡 Uso típico:</strong> Feed de actividades, auditoría, monitoreo</para>
    /// </remarks>
    /// <returns>Lista de actividades recientes ordenadas por fecha</returns>
    /// <response code="200">✅ Actividades obtenidas exitosamente</response>
    /// <response code="500">💥 Error interno del servidor</response>
    [HttpGet("recent-activities")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
        Summary = "Obtener actividades recientes",
        Description = "Recupera las actividades más recientes del sistema para monitoreo en tiempo real",
        OperationId = "GetRecentActivities",
        Tags = new[] { "📊 Dashboard" }
    )]
    public async Task<ActionResult> GetRecentActivities()
    {
        try
        {
            var activities = await _userService.GetRecentActivitiesAsync();
            return Ok(activities);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
        }
    }
}
