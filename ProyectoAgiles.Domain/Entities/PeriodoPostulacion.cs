using System.ComponentModel.DataAnnotations;

namespace ProyectoAgiles.Domain.Entities;

public class PeriodoPostulacion
{
    public int Id { get; set; }
    
    [Required]
    public DateTime FechaInicio { get; set; }
    
    [Required]
    public DateTime FechaFin { get; set; }
    
    [Required]
    public bool Activo { get; set; } = false;
    
    [MaxLength(500)]
    public string? Descripcion { get; set; }
    
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    
    // Navegación opcional a las solicitudes creadas en este período
    public virtual ICollection<SolicitudEscalafon>? Solicitudes { get; set; }
    
    // Propiedades calculadas
    public bool EstaActivo => Activo && DateTime.UtcNow >= FechaInicio && DateTime.UtcNow <= FechaFin;
    
    public bool EstaVigente => DateTime.UtcNow >= FechaInicio && DateTime.UtcNow <= FechaFin;
    
    public int DiasRestantes
    {
        get
        {
            if (!EstaVigente) return 0;
            var dias = (FechaFin.Date - DateTime.UtcNow.Date).Days;
            return Math.Max(0, dias);
        }
    }
    
    public string EstadoPeriodo
    {
        get
        {
            var ahora = DateTime.UtcNow;
            if (ahora < FechaInicio)
                return "Próximamente";
            if (ahora > FechaFin)
                return "Finalizado";
            if (!Activo)
                return "Inactivo";
            return "Activo";
        }
    }
}
