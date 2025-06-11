using ProyectoAgiles.Domain.Entities;

namespace ProyectoAgiles.Domain.Interfaces;

public interface IExternalTeacherRepository
{
    Task<ExternalTeacher?> GetByCedulaAsync(string cedula);
    Task<IEnumerable<ExternalTeacher>> GetAllAsync();
    Task<ExternalTeacher> AddAsync(ExternalTeacher externalTeacher);
    Task<ExternalTeacher> UpdateAsync(ExternalTeacher externalTeacher);
    Task DeleteAsync(int id);
}
