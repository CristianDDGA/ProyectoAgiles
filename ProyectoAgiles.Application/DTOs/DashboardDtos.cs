namespace ProyectoAgiles.Application.DTOs;

public class DashboardStatsDto
{
    public int TotalTeachers { get; set; }
    public int ActiveUsers { get; set; }
    public int TodayRegistrations { get; set; }
}

public class ActivityItemDto
{
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Icon { get; set; } = string.Empty;
}
