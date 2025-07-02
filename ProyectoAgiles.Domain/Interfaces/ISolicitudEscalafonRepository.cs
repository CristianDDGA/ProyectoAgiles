using ProyectoAgiles.Domain.Entities;

namespace ProyectoAgiles.Domain.Interfaces;

public interface ISolicitudEscalafonRepository
{
    Task<IEnumerable<SolicitudEscalafon>> GetAllAsync();
    Task<SolicitudEscalafon?> GetByIdAsync(int id);
    Task<IEnumerable<SolicitudEscalafon>> GetByCedulaAsync(string cedula);
    Task<IEnumerable<SolicitudEscalafon>> GetByStatusAsync(string status);
    Task<int> GetPendingCountAsync();
    Task<SolicitudEscalafon> AddAsync(SolicitudEscalafon solicitud);
    Task<SolicitudEscalafon> UpdateAsync(SolicitudEscalafon solicitud);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistePendienteByCedulaAsync(string cedula);
    Task<IEnumerable<SolicitudEscalafon>> GetHistorialEscalafonAsync(string cedula);
}
