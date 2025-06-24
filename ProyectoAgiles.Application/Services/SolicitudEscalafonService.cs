using AutoMapper;
using ProyectoAgiles.Application.DTOs;
using ProyectoAgiles.Application.Interfaces;
using ProyectoAgiles.Domain.Entities;
using ProyectoAgiles.Domain.Interfaces;

namespace ProyectoAgiles.Application.Services;

public class SolicitudEscalafonService : ISolicitudEscalafonService
{
    private readonly ISolicitudEscalafonRepository _repository;
    private readonly IMapper _mapper;

    public SolicitudEscalafonService(ISolicitudEscalafonRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<SolicitudEscalafonDto>> GetAllSolicitudesAsync()
    {
        var solicitudes = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<SolicitudEscalafonDto>>(solicitudes);
    }

    public async Task<SolicitudEscalafonDto?> GetSolicitudByIdAsync(int id)
    {
        var solicitud = await _repository.GetByIdAsync(id);
        return solicitud != null ? _mapper.Map<SolicitudEscalafonDto>(solicitud) : null;
    }

    public async Task<IEnumerable<SolicitudEscalafonDto>> GetSolicitudesByCedulaAsync(string cedula)
    {
        var solicitudes = await _repository.GetByCedulaAsync(cedula);
        return _mapper.Map<IEnumerable<SolicitudEscalafonDto>>(solicitudes);
    }

    public async Task<IEnumerable<SolicitudEscalafonDto>> GetSolicitudesByStatusAsync(string status)
    {
        var solicitudes = await _repository.GetByStatusAsync(status);
        return _mapper.Map<IEnumerable<SolicitudEscalafonDto>>(solicitudes);
    }

    public async Task<int> GetPendingCountAsync()
    {
        return await _repository.GetPendingCountAsync();
    }

    public async Task<SolicitudEscalafonDto> CreateSolicitudAsync(CreateSolicitudEscalafonDto createDto)
    {
        // Verificar si ya existe una solicitud pendiente para este docente
        var existePendiente = await _repository.ExistePendienteByCedulaAsync(createDto.DocenteCedula);
        if (existePendiente)
        {
            throw new InvalidOperationException("Ya existe una solicitud de escalafón pendiente para este docente.");
        }

        var solicitud = _mapper.Map<SolicitudEscalafon>(createDto);
        solicitud.FechaSolicitud = DateTime.Now;
        solicitud.Status = "Pendiente";
        solicitud.CreatedAt = DateTime.UtcNow;

        var createdSolicitud = await _repository.AddAsync(solicitud);
        return _mapper.Map<SolicitudEscalafonDto>(createdSolicitud);
    }

    public async Task<SolicitudEscalafonDto> UpdateSolicitudStatusAsync(UpdateSolicitudStatusDto updateDto)
    {
        var solicitud = await _repository.GetByIdAsync(updateDto.Id);
        if (solicitud == null)
        {
            throw new ArgumentException("Solicitud no encontrada");
        }

        solicitud.Status = updateDto.Status;
        solicitud.ProcesadoPor = updateDto.ProcesadoPor;

        if (updateDto.Status == "Aprobado")
        {
            solicitud.FechaAprobacion = DateTime.Now;
        }
        else if (updateDto.Status == "Rechazado")
        {
            solicitud.FechaRechazo = DateTime.Now;
            solicitud.MotivoRechazo = updateDto.MotivoRechazo;
        }

        var updatedSolicitud = await _repository.UpdateAsync(solicitud);
        return _mapper.Map<SolicitudEscalafonDto>(updatedSolicitud);
    }

    public async Task<bool> DeleteSolicitudAsync(int id)
    {
        return await _repository.DeleteAsync(id);
    }

    public async Task<bool> ExisteSolicitudPendienteAsync(string cedula)
    {
        return await _repository.ExistePendienteByCedulaAsync(cedula);
    }
}
