namespace ProyectoAgiles.Domain.Entities;

public class ExternalTeacher
{
    public int Id { get; set; }
    public string Cedula { get; set; } = string.Empty;
    public string Universidad { get; set; } = string.Empty;
    public string NombresCompletos { get; set; } = string.Empty;
    public string Nivel { get; set; } = "no autoridad"; // Solo puede ser 'autoridad' o 'no autoridad'
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
