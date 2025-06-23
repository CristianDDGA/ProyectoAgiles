using ProyectoAgiles.Domain.Entities;

namespace ProyectoAgiles.Domain.Interfaces;

public interface IInvestigacionRepository
{
    Task<IEnumerable<Investigacion>> GetAllAsync();
    Task<Investigacion?> GetByIdAsync(int id);
    Task<IEnumerable<Investigacion>> GetByCedulaAsync(string cedula);
    Task<IEnumerable<Investigacion>> GetByTipoAsync(string tipo);
    Task<IEnumerable<Investigacion>> GetByCampoConocimientoAsync(string campoConocimiento);
    Task<Investigacion> CreateAsync(Investigacion investigacion);
    Task<Investigacion> UpdateAsync(Investigacion investigacion);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
