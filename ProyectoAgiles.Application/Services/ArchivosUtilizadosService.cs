using ProyectoAgiles.Application.DTOs;
using ProyectoAgiles.Application.Interfaces;
using ProyectoAgiles.Domain.Interfaces;

namespace ProyectoAgiles.Application.Services;

/// <summary>
/// Servicio para gestionar archivos utilizados en escalafones (capa de aplicación)
/// </summary>
public class ArchivosUtilizadosService : IArchivosUtilizadosService
{
    private readonly IArchivosUtilizadosRepository _repository;

    public ArchivosUtilizadosService(IArchivosUtilizadosRepository repository)
    {
        _repository = repository;
    }

    public Task RegistrarArchivosUtilizados(int solicitudEscalafonId, string docenteCedula, string nivelOrigen, string nivelDestino)
    {
        // Este método se implementará en la capa de infraestructura
        // que tiene acceso al DbContext
        throw new NotImplementedException("Este método debe ser implementado en la capa de infraestructura");
    }

    public async Task<List<int>> ObtenerInvestigacionesUtilizadas(string docenteCedula)
    {
        var archivos = await _repository.GetByDocenteCedulaAsync(docenteCedula);
        return archivos
            .Where(a => a.TipoRecurso == "Investigacion" && a.EstadoAscenso == "Aprobado")
            .Select(a => a.RecursoId)
            .Distinct()
            .ToList();
    }

    public async Task<List<int>> ObtenerEvaluacionesUtilizadas(string docenteCedula)
    {
        var archivos = await _repository.GetByDocenteCedulaAsync(docenteCedula);
        return archivos
            .Where(a => a.TipoRecurso == "EvaluacionDesempeno" && a.EstadoAscenso == "Aprobado")
            .Select(a => a.RecursoId)
            .Distinct()
            .ToList();
    }

    public async Task<List<int>> ObtenerCapacitacionesUtilizadas(string docenteCedula)
    {
        var archivos = await _repository.GetByDocenteCedulaAsync(docenteCedula);
        return archivos
            .Where(a => a.TipoRecurso == "Capacitacion" && a.EstadoAscenso == "Aprobado")
            .Select(a => a.RecursoId)
            .Distinct()
            .ToList();
    }

    public async Task<List<ArchivosUtilizadosDto>> ObtenerHistorialArchivos(string docenteCedula)
    {
        var archivos = await _repository.GetByDocenteCedulaAsync(docenteCedula);
        return archivos
            .OrderByDescending(a => a.FechaUtilizacion)
            .Select(a => new ArchivosUtilizadosDto
            {
                Id = a.Id,
                SolicitudEscalafonId = a.SolicitudEscalafonId,
                TipoRecurso = a.TipoRecurso,
                RecursoId = a.RecursoId,
                DocenteCedula = a.DocenteCedula,
                NivelOrigen = a.NivelOrigen,
                NivelDestino = a.NivelDestino,
                FechaUtilizacion = a.FechaUtilizacion,
                Descripcion = a.Descripcion,
                EstadoAscenso = a.EstadoAscenso,
                TituloRecurso = a.Descripcion,
                DetallesRecurso = $"{a.TipoRecurso} utilizado para ascender de {a.NivelOrigen} a {a.NivelDestino}"
            })
            .ToList();
    }

    public async Task<bool> ArchivoYaUtilizado(string docenteCedula, string tipoRecurso, int recursoId)
    {
        return await _repository.IsRecursoUtilizadoAsync(docenteCedula, tipoRecurso, recursoId);
    }

    public async Task<Dictionary<string, int>> ObtenerEstadisticasArchivosUtilizados(string docenteCedula)
    {
        var archivos = await _repository.GetByDocenteCedulaAsync(docenteCedula);
        return archivos
            .Where(a => a.EstadoAscenso == "Aprobado")
            .GroupBy(a => a.TipoRecurso)
            .ToDictionary(g => g.Key, g => g.Count());
    }
}
