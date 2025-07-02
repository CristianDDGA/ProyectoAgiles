using Microsoft.EntityFrameworkCore;
using ProyectoAgiles.Domain.Entities;
using ProyectoAgiles.Domain.Interfaces;
using ProyectoAgiles.Infrastructure.Data;

namespace ProyectoAgiles.Infrastructure.Repositories;

/// <summary>
/// Repositorio para la gestión de archivos utilizados en escalafones
/// </summary>
public class ArchivosUtilizadosRepository : Repository<ArchivosUtilizadosEscalafon>, IArchivosUtilizadosRepository
{
    public ArchivosUtilizadosRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<List<ArchivosUtilizadosEscalafon>> GetByDocenteCedulaAsync(string cedula)
    {
        return await _context.ArchivosUtilizadosEscalafon
            .Where(a => a.DocenteCedula == cedula)
            .Include(a => a.SolicitudEscalafon)
            .OrderByDescending(a => a.FechaUtilizacion)
            .ToListAsync();
    }

    public async Task<List<ArchivosUtilizadosEscalafon>> GetByTipoRecursoAsync(string tipoRecurso)
    {
        return await _context.ArchivosUtilizadosEscalafon
            .Where(a => a.TipoRecurso == tipoRecurso)
            .Include(a => a.SolicitudEscalafon)
            .ToListAsync();
    }

    public async Task<List<ArchivosUtilizadosEscalafon>> GetByRecursoAsync(string tipoRecurso, int recursoId)
    {
        return await _context.ArchivosUtilizadosEscalafon
            .Where(a => a.TipoRecurso == tipoRecurso && a.RecursoId == recursoId)
            .Include(a => a.SolicitudEscalafon)
            .ToListAsync();
    }

    public async Task<bool> IsRecursoUtilizadoAsync(string docenteCedula, string tipoRecurso, int recursoId)
    {
        return await _context.ArchivosUtilizadosEscalafon
            .AnyAsync(a => a.DocenteCedula == docenteCedula && 
                          a.TipoRecurso == tipoRecurso && 
                          a.RecursoId == recursoId && 
                          a.EstadoAscenso == "Aprobado");
    }

    public async Task<List<ArchivosUtilizadosEscalafon>> GetBySolicitudEscalafonIdAsync(int solicitudEscalafonId)
    {
        return await _context.ArchivosUtilizadosEscalafon
            .Where(a => a.SolicitudEscalafonId == solicitudEscalafonId)
            .Include(a => a.SolicitudEscalafon)
            .OrderBy(a => a.TipoRecurso)
            .ToListAsync();
    }
}
