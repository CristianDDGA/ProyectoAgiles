using Microsoft.EntityFrameworkCore;
using ProyectoAgiles.Domain.Entities;
using ProyectoAgiles.Domain.Interfaces;
using ProyectoAgiles.Infrastructure.Data;

namespace ProyectoAgiles.Infrastructure.Repositories;

public class PeriodoPostulacionRepository : IPeriodoPostulacionRepository
{
    private readonly ApplicationDbContext _context;

    public PeriodoPostulacionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<PeriodoPostulacion>> GetAllAsync()
    {
        return await _context.PeriodosPostulacion
            .Include(p => p.Solicitudes)
            .OrderByDescending(p => p.FechaCreacion)
            .ToListAsync();
    }

    public async Task<PeriodoPostulacion?> GetByIdAsync(int id)
    {
        return await _context.PeriodosPostulacion
            .Include(p => p.Solicitudes)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<PeriodoPostulacion?> GetActivePeriodAsync()
    {
        var now = DateTime.UtcNow;
        return await _context.PeriodosPostulacion
            .FirstOrDefaultAsync(p => p.Activo && p.FechaInicio <= now && p.FechaFin >= now);
    }

    public async Task<PeriodoPostulacion> CreateAsync(PeriodoPostulacion periodo)
    {
        _context.PeriodosPostulacion.Add(periodo);
        await _context.SaveChangesAsync();
        return periodo;
    }

    public async Task<PeriodoPostulacion> UpdateAsync(PeriodoPostulacion periodo)
    {
        _context.PeriodosPostulacion.Update(periodo);
        await _context.SaveChangesAsync();
        return periodo;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var periodo = await _context.PeriodosPostulacion.FindAsync(id);
        if (periodo == null)
            return false;

        _context.PeriodosPostulacion.Remove(periodo);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeactivateAllPeriodsAsync()
    {
        var activePeriods = await _context.PeriodosPostulacion
            .Where(p => p.Activo)
            .ToListAsync();

        foreach (var period in activePeriods)
        {
            period.Activo = false;
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ActivatePeriodAsync(int id)
    {
        // Primero desactivar todos los períodos
        await DeactivateAllPeriodsAsync();

        // Luego activar el período específico
        var periodo = await _context.PeriodosPostulacion.FindAsync(id);
        if (periodo == null)
            return false;

        periodo.Activo = true;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeactivatePeriodAsync(int id)
    {
        var periodo = await _context.PeriodosPostulacion.FindAsync(id);
        if (periodo == null)
            return false;

        periodo.Activo = false;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<PeriodoPostulacion>> GetPeriodsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.PeriodosPostulacion
            .Where(p => p.FechaInicio >= startDate && p.FechaFin <= endDate)
            .OrderByDescending(p => p.FechaInicio)
            .ToListAsync();
    }
}
