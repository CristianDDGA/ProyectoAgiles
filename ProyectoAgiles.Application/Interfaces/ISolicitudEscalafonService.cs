using ProyectoAgiles.Application.DTOs;
using Microsoft.AspNetCore.Http;

namespace ProyectoAgiles.Application.Interfaces;

public interface ISolicitudEscalafonService
{
    Task<IEnumerable<SolicitudEscalafonDto>> GetAllSolicitudesAsync();
    Task<SolicitudEscalafonDto?> GetSolicitudByIdAsync(int id);
    Task<IEnumerable<SolicitudEscalafonDto>> GetSolicitudesByCedulaAsync(string cedula);
    Task<IEnumerable<SolicitudEscalafonDto>> GetSolicitudesByStatusAsync(string status);
    Task<int> GetPendingCountAsync();
    Task<SolicitudEscalafonDto> CreateSolicitudAsync(CreateSolicitudEscalafonDto createDto);
    Task<SolicitudEscalafonDto> UpdateSolicitudStatusAsync(UpdateSolicitudStatusDto updateDto);
    Task<bool> DeleteSolicitudAsync(int id);
    Task<bool> ExisteSolicitudPendienteAsync(string cedula);
    Task<bool> NotificarAprobacionAsync(int solicitudId);
    Task<bool> FinalizarEscalafonAsync(int solicitudId);
    Task<SolicitudEscalafonDto> RechazarSolicitudAsync(int solicitudId, string motivoRechazo, string rechazadoPor, string nivelRechazo);
    Task<SolicitudEscalafonDto> CrearApelacionAsync(int solicitudOriginalId, string observacionesApelacion, string destinatario = "", List<IFormFile>? archivos = null);
    Task<IEnumerable<HistorialEscalafonDto>> GetHistorialEscalafonAsync(string cedula);
}
