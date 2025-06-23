namespace ProyectoAgiles.Domain.Entities;

public class TTHH : BaseEntity
{
    public string Cedula { get; set; } = string.Empty; // Cedula del docente
    public DateTime FechaInicio { get; set; } // Fecha de inicio de funciones
    public double AniosCumplidos => (DateTime.UtcNow - FechaInicio).TotalDays / 365.25; // Cálculo automático
    public string Observacion { get; set; } = string.Empty;
}
