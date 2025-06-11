namespace ProyectoAgiles.Domain.Entities;

public class ExternalTeacher
{
    public int Id { get; set; }
    public string Cedula { get; set; } = string.Empty;
    public string Universidad { get; set; } = string.Empty;
    public string NombresCompletos { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
