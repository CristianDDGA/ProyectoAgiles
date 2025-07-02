using Microsoft.EntityFrameworkCore;
using ProyectoAgiles.Domain.Entities;
using ProyectoAgiles.Domain.Interfaces;
using ProyectoAgiles.Infrastructure.Data;

namespace ProyectoAgiles.Infrastructure.Repositories;

public class SolicitudEscalafonRepository : ISolicitudEscalafonRepository
{
    private readonly ApplicationDbContext _context;

    public SolicitudEscalafonRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<SolicitudEscalafon>> GetAllAsync()
    {
        return await _context.SolicitudesEscalafon
            .Where(s => !s.IsDeleted)
            .OrderByDescending(s => s.FechaSolicitud)
            .ToListAsync();
    }

    public async Task<SolicitudEscalafon?> GetByIdAsync(int id)
    {
        return await _context.SolicitudesEscalafon
            .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
    }

    public async Task<IEnumerable<SolicitudEscalafon>> GetByCedulaAsync(string cedula)
    {
        return await _context.SolicitudesEscalafon
            .Where(s => s.DocenteCedula == cedula && !s.IsDeleted)
            .OrderByDescending(s => s.FechaSolicitud)
            .ToListAsync();
    }

    public async Task<IEnumerable<SolicitudEscalafon>> GetByStatusAsync(string status)
    {
        return await _context.SolicitudesEscalafon
            .Where(s => s.Status == status && !s.IsDeleted)
            .OrderByDescending(s => s.FechaSolicitud)
            .ToListAsync();
    }

    public async Task<int> GetPendingCountAsync()
    {
        return await _context.SolicitudesEscalafon
            .CountAsync(s => s.Status == "Pendiente" && !s.IsDeleted);
    }

    public async Task<SolicitudEscalafon> AddAsync(SolicitudEscalafon solicitud)
    {
        _context.SolicitudesEscalafon.Add(solicitud);
        await _context.SaveChangesAsync();
        return solicitud;
    }

    public async Task<SolicitudEscalafon> UpdateAsync(SolicitudEscalafon solicitud)
    {
        solicitud.UpdatedAt = DateTime.UtcNow;
        _context.SolicitudesEscalafon.Update(solicitud);
        await _context.SaveChangesAsync();
        return solicitud;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var solicitud = await GetByIdAsync(id);
        if (solicitud == null) return false;

        solicitud.IsDeleted = true;
        solicitud.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistePendienteByCedulaAsync(string cedula)
    {
        return await _context.SolicitudesEscalafon
            .AnyAsync(s => s.DocenteCedula == cedula && s.Status == "Pendiente" && !s.IsDeleted);
    }

    public async Task<IEnumerable<SolicitudEscalafon>> GetHistorialEscalafonAsync(string cedula)
    {
        return await _context.SolicitudesEscalafon
            .Where(s => s.DocenteCedula == cedula && 
                       (s.Status == "Finalizado" || s.Status == "Finalizada" || s.Status == "Aprobada") && 
                       !s.IsDeleted)
            .OrderByDescending(s => s.FechaAprobacion ?? s.FechaSolicitud)
            .ToListAsync();
    }
}
