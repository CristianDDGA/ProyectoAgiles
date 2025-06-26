using Microsoft.EntityFrameworkCore;
using ProyectoAgiles.Domain.Entities;
using ProyectoAgiles.Domain.Interfaces;
using ProyectoAgiles.Infrastructure.Data;

namespace ProyectoAgiles.Infrastructure.Repositories;

/// <summary>
/// Implementación del repositorio para capacitaciones DITIC
/// </summary>
public class DiticRepository : IDiticRepository
{
    private readonly ApplicationDbContext _context;

    public DiticRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DITIC?> GetByIdAsync(int id)
    {
        return await _context.DITIC.FindAsync(id);
    }

    public async Task<IEnumerable<DITIC>> GetAllAsync()
    {
        return await _context.DITIC
            .OrderByDescending(d => d.FechaInicio)
            .ToListAsync();
    }

    public async Task<DITIC> CreateAsync(DITIC ditic)
    {
        ditic.CreatedAt = DateTime.UtcNow;
        _context.DITIC.Add(ditic);
        await _context.SaveChangesAsync();
        return ditic;
    }

    public async Task<DITIC> UpdateAsync(DITIC ditic)
    {
        ditic.UpdatedAt = DateTime.UtcNow;
        _context.DITIC.Update(ditic);
        await _context.SaveChangesAsync();
        return ditic;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var ditic = await GetByIdAsync(id);
        if (ditic == null)
            return false;

        _context.DITIC.Remove(ditic);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.DITIC.AnyAsync(d => d.Id == id);
    }

    public async Task<IEnumerable<DITIC>> GetByCedulaAsync(string cedula)
    {
        return await _context.DITIC
            .Where(d => d.Cedula == cedula)
            .OrderByDescending(d => d.FechaInicio)
            .ToListAsync();
    }

    public async Task<IEnumerable<DITIC>> GetByCedulaAndYearRangeAsync(string cedula, int añoInicio, int añoFin)
    {
        return await _context.DITIC
            .Where(d => d.Cedula == cedula && d.Anio >= añoInicio && d.Anio <= añoFin)
            .OrderByDescending(d => d.FechaInicio)
            .ToListAsync();
    }

    public async Task<IEnumerable<DITIC>> GetByCedulaLastThreeYearsAsync(string cedula)
    {
        var añoActual = DateTime.Now.Year;
        var añoInicio = añoActual - 2; // Últimos 3 años

        return await _context.DITIC
            .Where(d => d.Cedula == cedula && d.Anio >= añoInicio && d.Anio <= añoActual)
            .OrderByDescending(d => d.FechaInicio)
            .ToListAsync();
    }

    public async Task<IEnumerable<DITIC>> GetApprovedByCedulaAsync(string cedula)
    {
        return await _context.DITIC
            .Where(d => d.Cedula == cedula && 
                       (d.Estado == "Aprobada" || d.Estado == "Completada" || 
                        (d.Calificacion.HasValue && d.Calificacion >= d.CalificacionMinima)))
            .OrderByDescending(d => d.FechaInicio)
            .ToListAsync();
    }

    public async Task<IEnumerable<DITIC>> GetApprovedByCedulaLastThreeYearsAsync(string cedula)
    {
        var añoActual = DateTime.Now.Year;
        var añoInicio = añoActual - 2;

        return await _context.DITIC
            .Where(d => d.Cedula == cedula && 
                       d.Anio >= añoInicio && d.Anio <= añoActual &&
                       (d.Estado == "Aprobada" || d.Estado == "Completada" || 
                        (d.Calificacion.HasValue && d.Calificacion >= d.CalificacionMinima)))
            .OrderByDescending(d => d.FechaInicio)
            .ToListAsync();
    }

    public async Task<IEnumerable<DITIC>> GetPedagogicalByCedulaLastThreeYearsAsync(string cedula)
    {
        var añoActual = DateTime.Now.Year;
        var añoInicio = añoActual - 2;

        // Obtener todas las capacitaciones aprobadas del período y filtrar por EsPedagogica en memoria
        // Esto asegura consistencia con la lógica de la entidad DITIC.EsPedagogica
        var capacitaciones = await _context.DITIC
            .Where(d => d.Cedula == cedula && 
                       d.Anio >= añoInicio && d.Anio <= añoActual &&
                       (d.Estado == "Aprobada" || d.Estado == "Completada" || 
                        (d.Calificacion.HasValue && d.Calificacion >= d.CalificacionMinima)))
            .OrderByDescending(d => d.FechaInicio)
            .ToListAsync();

        // Filtrar por capacitaciones pedagógicas usando la propiedad EsPedagogica de la entidad
        return capacitaciones.Where(d => d.EsPedagogica).ToList();
    }

    public async Task<int> GetTotalHoursByCedulaAsync(string cedula)
    {
        return await _context.DITIC
            .Where(d => d.Cedula == cedula && 
                       (d.Estado == "Aprobada" || d.Estado == "Completada" || 
                        (d.Calificacion.HasValue && d.Calificacion >= d.CalificacionMinima)))
            .SumAsync(d => d.HorasAcademicas);
    }

    public async Task<int> GetTotalHoursByCedulaLastThreeYearsAsync(string cedula)
    {
        var añoActual = DateTime.Now.Year;
        var añoInicio = añoActual - 2;

        return await _context.DITIC
            .Where(d => d.Cedula == cedula && 
                       d.Anio >= añoInicio && d.Anio <= añoActual &&
                       (d.Estado == "Aprobada" || d.Estado == "Completada" || 
                        (d.Calificacion.HasValue && d.Calificacion >= d.CalificacionMinima)))
            .SumAsync(d => d.HorasAcademicas);
    }

    public async Task<int> GetPedagogicalHoursByCedulaLastThreeYearsAsync(string cedula)
    {
        var añoActual = DateTime.Now.Year;
        var añoInicio = añoActual - 2;

        // Obtener todas las capacitaciones aprobadas del período y filtrar por EsPedagogica en memoria
        // Esto asegura consistencia con la lógica de la entidad DITIC.EsPedagogica
        var capacitaciones = await _context.DITIC
            .Where(d => d.Cedula == cedula && 
                       d.Anio >= añoInicio && d.Anio <= añoActual &&
                       (d.Estado == "Aprobada" || d.Estado == "Completada" || 
                        (d.Calificacion.HasValue && d.Calificacion >= d.CalificacionMinima)))
            .ToListAsync();

        // Filtrar por capacitaciones pedagógicas y sumar horas usando la propiedad EsPedagogica de la entidad
        return capacitaciones.Where(d => d.EsPedagogica).Sum(d => d.HorasAcademicas);
    }

    public async Task<int> GetCountByCedulaAsync(string cedula)
    {
        return await _context.DITIC
            .Where(d => d.Cedula == cedula)
            .CountAsync();
    }

    public async Task<DITIC?> GetAuthorityExemptionByCedulaAsync(string cedula)
    {
        return await _context.DITIC
            .Where(d => d.Cedula == cedula && d.ExencionPorAutoridad &&
                       d.FechaInicioAutoridad.HasValue)
            .OrderByDescending(d => d.FechaInicioAutoridad)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> HasAuthorityExemptionAsync(string cedula)
    {
        return await _context.DITIC
            .AnyAsync(d => d.Cedula == cedula && d.ExencionPorAutoridad &&
                          d.FechaInicioAutoridad.HasValue &&
                          (d.FechaFinAutoridad.HasValue ? 
                           EF.Functions.DateDiffDay(d.FechaInicioAutoridad.Value, d.FechaFinAutoridad.Value) > 730 :
                           EF.Functions.DateDiffDay(d.FechaInicioAutoridad.Value, DateTime.Now) > 730));
    }

    public async Task<IEnumerable<DITIC>> GetByTypeAsync(string tipoCapacitacion)
    {
        return await _context.DITIC
            .Where(d => d.TipoCapacitacion.ToLower().Contains(tipoCapacitacion.ToLower()))
            .OrderByDescending(d => d.FechaInicio)
            .ToListAsync();
    }

    public async Task<IEnumerable<DITIC>> GetByInstitutionAsync(string institucion)
    {
        return await _context.DITIC
            .Where(d => d.Institucion.ToLower().Contains(institucion.ToLower()))
            .OrderByDescending(d => d.FechaInicio)
            .ToListAsync();
    }

    public async Task<IEnumerable<DITIC>> GetByYearAsync(int año)
    {
        return await _context.DITIC
            .Where(d => d.Anio == año)
            .OrderByDescending(d => d.FechaInicio)
            .ToListAsync();
    }

    public async Task<IEnumerable<DITIC>> GetByStatusAsync(string estado)
    {
        return await _context.DITIC
            .Where(d => d.Estado.ToLower() == estado.ToLower())
            .OrderByDescending(d => d.FechaInicio)
            .ToListAsync();
    }

    public async Task<IEnumerable<DITIC>> SearchAsync(string? searchTerm, string? cedula = null, 
                                                     string? tipo = null, string? institucion = null, 
                                                     int? año = null, string? estado = null)
    {
        var query = _context.DITIC.AsQueryable();

        if (!string.IsNullOrEmpty(cedula))
            query = query.Where(d => d.Cedula == cedula);

        if (!string.IsNullOrEmpty(tipo))
            query = query.Where(d => d.TipoCapacitacion.ToLower().Contains(tipo.ToLower()));

        if (!string.IsNullOrEmpty(institucion))
            query = query.Where(d => d.Institucion.ToLower().Contains(institucion.ToLower()));

        if (año.HasValue)
            query = query.Where(d => d.Anio == año.Value);

        if (!string.IsNullOrEmpty(estado))
            query = query.Where(d => d.Estado.ToLower() == estado.ToLower());

        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(d => 
                d.NombreCapacitacion.ToLower().Contains(searchTerm.ToLower()) ||
                d.Institucion.ToLower().Contains(searchTerm.ToLower()) ||
                d.TipoCapacitacion.ToLower().Contains(searchTerm.ToLower()) ||
                (d.Instructor != null && d.Instructor.ToLower().Contains(searchTerm.ToLower())) ||
                (d.Descripcion != null && d.Descripcion.ToLower().Contains(searchTerm.ToLower())));
        }

        return await query
            .OrderByDescending(d => d.FechaInicio)
            .ToListAsync();
    }

    public async Task<int> ImportFromExternalSystemAsync(string cedula)
    {
        // Simulación de importación desde sistema externo
        // En un caso real, aquí se consumiría un API externo o base de datos
        
        var capacitacionesExternas = new List<DITIC>();
        
        // Datos de prueba para importación
        if (cedula == "1234567890") // Cédula de prueba
        {
            capacitacionesExternas.AddRange(new[]
            {
                new DITIC
                {
                    Cedula = cedula,
                    NombreCapacitacion = "Metodologías Activas de Aprendizaje",
                    Institucion = "Universidad Central del Ecuador",
                    TipoCapacitacion = "Pedagógica",
                    Modalidad = "Virtual",
                    HorasAcademicas = 40,
                    FechaInicio = new DateTime(2023, 3, 15),
                    FechaFin = new DateTime(2023, 4, 15),
                    Anio = 2023,
                    Estado = "Aprobada",
                    Calificacion = 85,
                    CalificacionMinima = 70,
                    Descripcion = "Capacitación en metodologías activas para mejorar el proceso de enseñanza-aprendizaje",
                    NumeroCertificado = "UCE-DITIC-2023-001",
                    Instructor = "Dr. María González",
                    CreatedAt = DateTime.UtcNow
                },
                new DITIC
                {
                    Cedula = cedula,
                    NombreCapacitacion = "Tecnologías Emergentes en Educación",
                    Institucion = "Escuela Politécnica Nacional",
                    TipoCapacitacion = "Técnica",
                    Modalidad = "Presencial",
                    HorasAcademicas = 32,
                    FechaInicio = new DateTime(2023, 6, 10),
                    FechaFin = new DateTime(2023, 7, 10),
                    Anio = 2023,
                    Estado = "Completada",
                    Calificacion = 92,
                    CalificacionMinima = 70,
                    Descripcion = "Uso de realidad virtual, inteligencia artificial y blockchain en educación",
                    NumeroCertificado = "EPN-TECH-2023-045",
                    Instructor = "Ing. Carlos Vásquez",
                    CreatedAt = DateTime.UtcNow
                },
                new DITIC
                {
                    Cedula = cedula,
                    NombreCapacitacion = "Evaluación por Competencias",
                    Institucion = "SENESCYT",
                    TipoCapacitacion = "Pedagógica",
                    Modalidad = "Semipresencial",
                    HorasAcademicas = 24,
                    FechaInicio = new DateTime(2022, 9, 5),
                    FechaFin = new DateTime(2022, 10, 5),
                    Anio = 2022,
                    Estado = "Aprobada",
                    Calificacion = 78,
                    CalificacionMinima = 70,
                    Descripcion = "Técnicas modernas de evaluación enfocadas en competencias y destrezas",
                    NumeroCertificado = "SENESCYT-EVAL-2022-128",
                    Instructor = "Mg. Ana Rodríguez",
                    CreatedAt = DateTime.UtcNow
                }
            });
        }

        if (capacitacionesExternas.Any())
        {
            // Verificar cuáles no existen ya en la base de datos
            var capacitacionesNuevas = new List<DITIC>();
            
            foreach (var capacitacion in capacitacionesExternas)
            {
                var existe = await _context.DITIC.AnyAsync(d => 
                    d.Cedula == capacitacion.Cedula &&
                    d.NombreCapacitacion == capacitacion.NombreCapacitacion &&
                    d.Institucion == capacitacion.Institucion &&
                    d.FechaInicio == capacitacion.FechaInicio);

                if (!existe)
                {
                    capacitacionesNuevas.Add(capacitacion);
                }
            }

            if (capacitacionesNuevas.Any())
            {
                _context.DITIC.AddRange(capacitacionesNuevas);
                await _context.SaveChangesAsync();
                return capacitacionesNuevas.Count;
            }
        }

        return 0;
    }

    public async Task<bool> ValidateExternalDataAsync(string cedula)
    {
        // Simulación de validación de datos externos
        // En un caso real, aquí se validaría contra el sistema externo
        
        await Task.Delay(100); // Simular latencia de red
        
        // Para la demo, considerar válido si la cédula tiene el formato correcto
        return !string.IsNullOrEmpty(cedula) && cedula.Length >= 8 && cedula.Length <= 10;
    }
}
