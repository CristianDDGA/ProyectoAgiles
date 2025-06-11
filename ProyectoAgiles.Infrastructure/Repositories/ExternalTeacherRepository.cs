using Microsoft.EntityFrameworkCore;
using ProyectoAgiles.Domain.Entities;
using ProyectoAgiles.Domain.Interfaces;
using ProyectoAgiles.Infrastructure.Data;

namespace ProyectoAgiles.Infrastructure.Repositories;

public class ExternalTeacherRepository : IExternalTeacherRepository
{
    private readonly ApplicationDbContext _context;

    public ExternalTeacherRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ExternalTeacher?> GetByCedulaAsync(string cedula)
    {
        return await _context.ExternalTeachers
            .FirstOrDefaultAsync(et => et.Cedula == cedula);
    }

    public async Task<IEnumerable<ExternalTeacher>> GetAllAsync()
    {
        return await _context.ExternalTeachers
            .OrderBy(et => et.NombresCompletos)
            .ToListAsync();
    }

    public async Task<ExternalTeacher> AddAsync(ExternalTeacher externalTeacher)
    {
        externalTeacher.CreatedAt = DateTime.UtcNow;
        externalTeacher.UpdatedAt = DateTime.UtcNow;
        
        _context.ExternalTeachers.Add(externalTeacher);
        await _context.SaveChangesAsync();
        return externalTeacher;
    }

    public async Task<ExternalTeacher> UpdateAsync(ExternalTeacher externalTeacher)
    {
        externalTeacher.UpdatedAt = DateTime.UtcNow;
        
        _context.ExternalTeachers.Update(externalTeacher);
        await _context.SaveChangesAsync();
        return externalTeacher;
    }

    public async Task DeleteAsync(int id)
    {
        var externalTeacher = await _context.ExternalTeachers.FindAsync(id);
        if (externalTeacher != null)
        {
            _context.ExternalTeachers.Remove(externalTeacher);
            await _context.SaveChangesAsync();
        }
    }
}
