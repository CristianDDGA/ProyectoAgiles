using Microsoft.AspNetCore.Mvc;
using ProyectoAgiles.Application.Interfaces;

namespace ProyectoAgiles.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IUserService _userService;

    public DashboardController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("stats")]
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

    [HttpGet("recent-activities")]
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
