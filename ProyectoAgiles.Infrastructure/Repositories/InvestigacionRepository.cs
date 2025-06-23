using Microsoft.EntityFrameworkCore;
using ProyectoAgiles.Domain.Entities;
using ProyectoAgiles.Domain.Interfaces;
using ProyectoAgiles.Infrastructure.Data;

namespace ProyectoAgiles.Infrastructure.Repositories;

public class InvestigacionRepository : IInvestigacionRepository
{
    private readonly ApplicationDbContext _context;

    public InvestigacionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Investigacion>> GetAllAsync()
    {
        return await _context.Investigaciones
            .Where(i => !i.IsDeleted)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();
    }

    public async Task<Investigacion?> GetByIdAsync(int id)
    {
        return await _context.Investigaciones
            .FirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted);
    }

    public async Task<IEnumerable<Investigacion>> GetByCedulaAsync(string cedula)
    {
        return await _context.Investigaciones
            .Where(i => i.Cedula == cedula && !i.IsDeleted)
            .OrderByDescending(i => i.FechaPublicacion)
            .ToListAsync();
    }

    public async Task<IEnumerable<Investigacion>> GetByTipoAsync(string tipo)
    {
        return await _context.Investigaciones
            .Where(i => i.Tipo == tipo && !i.IsDeleted)
            .OrderByDescending(i => i.FechaPublicacion)
            .ToListAsync();
    }

    public async Task<IEnumerable<Investigacion>> GetByCampoConocimientoAsync(string campoConocimiento)
    {
        return await _context.Investigaciones
            .Where(i => i.CampoConocimiento == campoConocimiento && !i.IsDeleted)
            .OrderByDescending(i => i.FechaPublicacion)
            .ToListAsync();
    }

    public async Task<Investigacion> CreateAsync(Investigacion investigacion)
    {
        investigacion.CreatedAt = DateTime.UtcNow;
        _context.Investigaciones.Add(investigacion);
        await _context.SaveChangesAsync();
        return investigacion;
    }

    public async Task<Investigacion> UpdateAsync(Investigacion investigacion)
    {
        investigacion.UpdatedAt = DateTime.UtcNow;
        _context.Entry(investigacion).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return investigacion;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var investigacion = await GetByIdAsync(id);
        if (investigacion == null)
            return false;

        investigacion.IsDeleted = true;
        investigacion.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Investigaciones
            .AnyAsync(i => i.Id == id && !i.IsDeleted);
    }
}
