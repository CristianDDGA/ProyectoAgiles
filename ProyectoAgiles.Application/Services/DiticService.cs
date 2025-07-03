using AutoMapper;
using ProyectoAgiles.Application.DTOs;
using ProyectoAgiles.Application.Interfaces;
using ProyectoAgiles.Domain.Entities;
using ProyectoAgiles.Domain.Interfaces;

namespace ProyectoAgiles.Application.Services;

/// <summary>
/// Implementación del servicio para capacitaciones DITIC
/// </summary>
public class DiticService : IDiticService
{
    private readonly IDiticRepository _diticRepository;
    private readonly IMapper _mapper;
    private readonly IArchivosUtilizadosService _archivosUtilizadosService;

    public DiticService(
        IDiticRepository diticRepository, 
        IMapper mapper,
        IArchivosUtilizadosService archivosUtilizadosService)
    {
        _diticRepository = diticRepository;
        _mapper = mapper;
        _archivosUtilizadosService = archivosUtilizadosService;
    }

    public async Task<DiticDto?> GetByIdAsync(int id)
    {
        var ditic = await _diticRepository.GetByIdAsync(id);
        return ditic != null ? _mapper.Map<DiticDto>(ditic) : null;
    }

    public async Task<IEnumerable<DiticDto>> GetAllAsync()
    {
        var ditics = await _diticRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<DiticDto>>(ditics);
    }

    public async Task<DiticDto> CreateAsync(CreateDiticDto createDto)
    {
        // Validaciones de negocio
        await ValidateBusinessRules(createDto);

        var ditic = _mapper.Map<DITIC>(createDto);
        var createdDitic = await _diticRepository.CreateAsync(ditic);
        return _mapper.Map<DiticDto>(createdDitic);
    }

    public async Task<DiticDto> CreateWithPdfAsync(CreateDiticWithPdfDto createDto)
    {
        // Validaciones de negocio
        await ValidateBusinessRules(createDto);

        var ditic = _mapper.Map<DITIC>(createDto);
        
        // Procesar archivo PDF si existe
        if (createDto.ArchivoCertificado != null && createDto.ArchivoCertificado.Length > 0)
        {
            using var memoryStream = new MemoryStream();
            await createDto.ArchivoCertificado.CopyToAsync(memoryStream);
            ditic.ArchivoCertificado = memoryStream.ToArray();
            ditic.NombreArchivoCertificado = createDto.ArchivoCertificado.FileName;
        }

        var createdDitic = await _diticRepository.CreateAsync(ditic);
        return _mapper.Map<DiticDto>(createdDitic);
    }

    public async Task<DiticDto> UpdateAsync(UpdateDiticDto updateDto)
    {
        var existingDitic = await _diticRepository.GetByIdAsync(updateDto.Id);
        if (existingDitic == null)
            throw new ArgumentException($"No se encontró la capacitación con ID {updateDto.Id}");

        // Validaciones de negocio
        await ValidateBusinessRules(updateDto);

        _mapper.Map(updateDto, existingDitic);
        var updatedDitic = await _diticRepository.UpdateAsync(existingDitic);
        return _mapper.Map<DiticDto>(updatedDitic);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _diticRepository.DeleteAsync(id);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _diticRepository.ExistsAsync(id);
    }

    public async Task<IEnumerable<DiticDto>> GetByCedulaAsync(string cedula)
    {
        var ditics = await _diticRepository.GetByCedulaAsync(cedula);
        return _mapper.Map<IEnumerable<DiticDto>>(ditics);
    }

    public async Task<IEnumerable<DiticDto>> GetDisponiblesParaEscalafonAsync(string cedula)
    {
        // Obtener todas las capacitaciones del docente
        var todasLasCapacitaciones = await _diticRepository.GetByCedulaAsync(cedula);
        
        // Obtener los IDs de capacitaciones ya utilizadas en escalafones previos
        var capacitacionesUtilizadas = await _archivosUtilizadosService.ObtenerCapacitacionesUtilizadas(cedula);
        
        // Filtrar para excluir las ya utilizadas
        var capacitacionesDisponibles = todasLasCapacitaciones
            .Where(cap => !capacitacionesUtilizadas.Contains(cap.Id))
            .OrderBy(cap => cap.FechaInicio) // Ordenar por fecha más antigua primero
            .ToList();
        
        Console.WriteLine($"DiticService.GetDisponiblesParaEscalafonAsync:");
        Console.WriteLine($"  - Total capacitaciones del docente: {todasLasCapacitaciones.Count()}");
        Console.WriteLine($"  - Capacitaciones utilizadas: {capacitacionesUtilizadas.Count}");
        Console.WriteLine($"  - Capacitaciones disponibles: {capacitacionesDisponibles.Count}");
        
        return _mapper.Map<IEnumerable<DiticDto>>(capacitacionesDisponibles);
    }

    public async Task<IEnumerable<DiticDto>> GetByCedulaLastThreeYearsAsync(string cedula)
    {
        var ditics = await _diticRepository.GetByCedulaLastThreeYearsAsync(cedula);
        return _mapper.Map<IEnumerable<DiticDto>>(ditics);
    }

    public async Task<VerificacionRequisitoDiticDto> VerifyRequirementAsync(string cedula)
    {
        var capacitacionesAprobadas = await _diticRepository.GetApprovedByCedulaLastThreeYearsAsync(cedula);
        var capacitacionesPedagogicas = await _diticRepository.GetPedagogicalByCedulaLastThreeYearsAsync(cedula);
        var tieneExencion = await _diticRepository.HasAuthorityExemptionAsync(cedula);
        var exencionDetalle = await _diticRepository.GetAuthorityExemptionByCedulaAsync(cedula);

        var horasTotal = capacitacionesAprobadas.Sum(c => c.HorasAcademicas);
        var horasPedagogicas = capacitacionesPedagogicas.Sum(c => c.HorasAcademicas);
        var porcentajePedagogico = horasTotal > 0 ? (decimal)horasPedagogicas / horasTotal * 100 : 0;

        var cumpleHorasTotales = horasTotal >= 96;
        var cumpleHorasPedagogicas = horasPedagogicas >= 24;
        var cumpleRequisito = (cumpleHorasTotales && cumpleHorasPedagogicas) || tieneExencion;

        var mensaje = GenerarMensajeVerificacion(cumpleRequisito, cumpleHorasTotales, cumpleHorasPedagogicas, 
                                               tieneExencion, horasTotal, horasPedagogicas, exencionDetalle);

        return new VerificacionRequisitoDiticDto
        {
            Cedula = cedula,
            CumpleRequisito = cumpleRequisito,
            CumpleHorasTotales = cumpleHorasTotales,
            CumpleHorasPedagogicas = cumpleHorasPedagogicas,
            TieneExencionAutoridad = tieneExencion,
            HorasRequeridas = 96,
            HorasPedagogicasRequeridas = 24,
            HorasObtenidas = horasTotal,
            HorasPedagogicasObtenidas = horasPedagogicas,
            PorcentajePedagogico = porcentajePedagogico,
            CapacitacionesAnalizadas = capacitacionesAprobadas.Count(),
            MensajeDetallado = mensaje,
            CargoAutoridad = exencionDetalle?.CargoAutoridad,
            AñosComoAutoridad = exencionDetalle?.AñosComoAutoridad,
            CapacitacionesConsideradas = _mapper.Map<List<DiticDto>>(capacitacionesAprobadas)
        };
    }

    public async Task<ResumenCapacitacionesDto> GetSummaryByCedulaAsync(string cedula)
    {
        var todasCapacitaciones = await _diticRepository.GetByCedulaAsync(cedula);
        var capacitacionesAprobadas = todasCapacitaciones.Where(c => c.Aprobada);
        var capacitacionesUltimosTresAnios = await _diticRepository.GetApprovedByCedulaLastThreeYearsAsync(cedula);
        var capacitacionesPedagogicas = await _diticRepository.GetPedagogicalByCedulaLastThreeYearsAsync(cedula);
        var tieneExencion = await _diticRepository.HasAuthorityExemptionAsync(cedula);

        var horasTotales = capacitacionesAprobadas.Sum(c => c.HorasAcademicas);
        var horasPedagogicas = capacitacionesAprobadas.Where(c => c.EsPedagogica).Sum(c => c.HorasAcademicas);
        var horasUltimosTresAnios = capacitacionesUltimosTresAnios.Sum(c => c.HorasAcademicas);
        var horasPedagogicasUltimosTresAnios = capacitacionesPedagogicas.Sum(c => c.HorasAcademicas);

        var cumpleHoras = horasUltimosTresAnios >= 96;
        var cumplePedagogico = horasPedagogicasUltimosTresAnios >= 24;
        var cumpleRequisito = (cumpleHoras && cumplePedagogico) || tieneExencion;

        var porcentajePedagogico = horasUltimosTresAnios > 0 ? 
            (decimal)horasPedagogicasUltimosTresAnios / horasUltimosTresAnios * 100 : 0;

        var mensaje = GenerarMensajeResumen(cumpleRequisito, cumpleHoras, cumplePedagogico, 
                                          tieneExencion, horasUltimosTresAnios, horasPedagogicasUltimosTresAnios);

        return new ResumenCapacitacionesDto
        {
            Cedula = cedula,
            TotalCapacitaciones = todasCapacitaciones.Count(),
            CapacitacionesAprobadas = capacitacionesAprobadas.Count(),
            HorasTotales = horasTotales,
            HorasPedagogicas = horasPedagogicas,
            HorasUltimosTresAnios = horasUltimosTresAnios,
            HorasPedagogicasUltimosTresAnios = horasPedagogicasUltimosTresAnios,
            CumpleRequisitoHoras = cumpleHoras,
            CumpleRequisitoPedagogico = cumplePedagogico,
            CumpleExencionAutoridad = tieneExencion,
            CumpleRequisito = cumpleRequisito,
            PorcentajePedagogico = porcentajePedagogico,
            MensajeEstado = mensaje,
            CapacitacionesUltimosTresAnios = _mapper.Map<List<DiticDto>>(capacitacionesUltimosTresAnios)
        };
    }

    public async Task<IEnumerable<EstadisticasCapacitacionDto>> GetStatisticsByTypeAsync(string cedula)
    {
        var capacitaciones = await _diticRepository.GetApprovedByCedulaAsync(cedula);
        var horasTotal = capacitaciones.Sum(c => c.HorasAcademicas);

        return capacitaciones
            .GroupBy(c => c.TipoCapacitacion)
            .Select(g => new EstadisticasCapacitacionDto
            {
                TipoCapacitacion = g.Key,
                Cantidad = g.Count(),
                HorasTotales = g.Sum(c => c.HorasAcademicas),
                PorcentajeDelTotal = horasTotal > 0 ? (decimal)g.Sum(c => c.HorasAcademicas) / horasTotal * 100 : 0
            })
            .OrderByDescending(e => e.HorasTotales);
    }

    public async Task<int> GetTotalHoursAsync(string cedula)
    {
        return await _diticRepository.GetTotalHoursByCedulaAsync(cedula);
    }

    public async Task<int> GetTotalHoursLastThreeYearsAsync(string cedula)
    {
        return await _diticRepository.GetTotalHoursByCedulaLastThreeYearsAsync(cedula);
    }

    public async Task<int> GetPedagogicalHoursLastThreeYearsAsync(string cedula)
    {
        return await _diticRepository.GetPedagogicalHoursByCedulaLastThreeYearsAsync(cedula);
    }

    public async Task<byte[]?> GetCertificatePdfAsync(int id)
    {
        var ditic = await _diticRepository.GetByIdAsync(id);
        return ditic?.ArchivoCertificado;
    }

    public async Task<bool> UpdateCertificatePdfAsync(int id, byte[] pdfContent, string fileName)
    {
        var ditic = await _diticRepository.GetByIdAsync(id);
        if (ditic == null)
            return false;

        ditic.ArchivoCertificado = pdfContent;
        ditic.NombreArchivoCertificado = fileName;
        ditic.UpdatedAt = DateTime.UtcNow;

        await _diticRepository.UpdateAsync(ditic);
        return true;
    }

    public async Task<bool> DeleteCertificatePdfAsync(int id)
    {
        var ditic = await _diticRepository.GetByIdAsync(id);
        if (ditic == null)
            return false;

        ditic.ArchivoCertificado = null;
        ditic.NombreArchivoCertificado = null;
        ditic.UpdatedAt = DateTime.UtcNow;

        await _diticRepository.UpdateAsync(ditic);
        return true;
    }

    public async Task<int> ImportFromExternalSystemAsync(string cedula)
    {
        return await _diticRepository.ImportFromExternalSystemAsync(cedula);
    }

    public async Task<bool> ValidateExternalDataAsync(string cedula)
    {
        return await _diticRepository.ValidateExternalDataAsync(cedula);
    }

    public async Task<IEnumerable<DiticDto>> SearchAsync(string? searchTerm, string? cedula = null, 
                                                        string? tipo = null, string? institucion = null, 
                                                        int? año = null, string? estado = null)
    {
        var ditics = await _diticRepository.SearchAsync(searchTerm, cedula, tipo, institucion, año, estado);
        return _mapper.Map<IEnumerable<DiticDto>>(ditics);
    }

    public async Task<bool> ValidateTrainingHoursAsync(CreateDiticDto createDto)
    {
        await Task.CompletedTask;
        
        // Validar que las horas estén en un rango razonable
        if (createDto.HorasAcademicas < 1 || createDto.HorasAcademicas > 1000)
            return false;

        // Validar que las fechas sean coherentes con las horas
        var duracionDias = (createDto.FechaFin - createDto.FechaInicio).Days + 1;
        var horasPorDia = (decimal)createDto.HorasAcademicas / duracionDias;

        // Máximo 8 horas académicas por día es razonable
        return horasPorDia <= 8;
    }

    public async Task<bool> ValidateDateRangeAsync(DateTime fechaInicio, DateTime fechaFin)
    {
        await Task.CompletedTask;
        
        // La fecha de fin debe ser posterior a la fecha de inicio
        if (fechaFin <= fechaInicio)
            return false;

        // No debe ser una capacitación de más de 2 años
        if ((fechaFin - fechaInicio).TotalDays > 730)
            return false;

        // No debe ser una fecha futura muy lejana
        if (fechaInicio > DateTime.Now.AddYears(1))
            return false;

        return true;
    }

    public async Task<bool> ValidateAuthorityExemptionAsync(CreateDiticDto createDto)
    {
        await Task.CompletedTask;
        
        if (!createDto.ExencionPorAutoridad)
            return true;

        // Si se reclama exención por autoridad, debe tener cargo y fechas
        if (string.IsNullOrEmpty(createDto.CargoAutoridad) || 
            !createDto.FechaInicioAutoridad.HasValue)
            return false;

        // Debe tener al menos 2 años como autoridad
        var fechaFin = createDto.FechaFinAutoridad ?? DateTime.Now;
        var duracion = fechaFin - createDto.FechaInicioAutoridad.Value;
        
        return duracion.TotalDays >= 730; // 2 años
    }

    private async Task ValidateBusinessRules(CreateDiticDto createDto)
    {
        // Validar fechas
        if (!await ValidateDateRangeAsync(createDto.FechaInicio, createDto.FechaFin))
            throw new ArgumentException("Las fechas de la capacitación no son válidas");

        // Validar horas
        if (!await ValidateTrainingHoursAsync(createDto))
            throw new ArgumentException("Las horas de la capacitación no son válidas");

        // Validar exención por autoridad
        if (!await ValidateAuthorityExemptionAsync(createDto))
            throw new ArgumentException("Los datos de exención por autoridad no son válidos");

        // El año debe coincidir con las fechas
        if (createDto.Anio != createDto.FechaInicio.Year && createDto.Anio != createDto.FechaFin.Year)
            throw new ArgumentException("El año no coincide con las fechas de la capacitación");
    }

    private string GenerarMensajeVerificacion(bool cumpleRequisito, bool cumpleHoras, bool cumplePedagogico, 
                                            bool tieneExencion, int horasTotal, int horasPedagogicas, 
                                            DITIC? exencionDetalle)
    {
        if (cumpleRequisito)
        {
            if (tieneExencion)
            {
                return $"✅ CUMPLE - Exento por cargo de autoridad: {exencionDetalle?.CargoAutoridad} " +
                       $"({exencionDetalle?.AñosComoAutoridad:F1} años)";
            }
            else
            {
                return $"✅ CUMPLE - {horasTotal} horas totales ({horasPedagogicas} pedagógicas) en los últimos 3 años";
            }
        }
        else
        {
            var faltantes = new List<string>();
            
            if (!cumpleHoras)
                faltantes.Add($"faltan {96 - horasTotal} horas (tiene {horasTotal}/96)");
            
            if (!cumplePedagogico)
                faltantes.Add($"faltan {24 - horasPedagogicas} horas pedagógicas (tiene {horasPedagogicas}/24)");

            return $"❌ NO CUMPLE - {string.Join(", ", faltantes)}";
        }
    }

    private string GenerarMensajeResumen(bool cumpleRequisito, bool cumpleHoras, bool cumplePedagogico, 
                                       bool tieneExencion, int horasUltimosTresAnios, int horasPedagogicas)
    {
        if (cumpleRequisito)
        {
            if (tieneExencion)
                return "Cumple por exención de autoridad";
            else
                return $"Cumple requisito - {horasUltimosTresAnios}h totales, {horasPedagogicas}h pedagógicas";
        }
        else
        {
            return $"No cumple - {horasUltimosTresAnios}/96h totales, {horasPedagogicas}/24h pedagógicas";
        }
    }
}
