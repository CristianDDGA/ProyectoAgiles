using AutoMapper;
using ProyectoAgiles.Application.DTOs;
using ProyectoAgiles.Domain.Entities;

namespace ProyectoAgiles.Application.Mappings;

/// <summary>
/// Perfil de AutoMapper para mapear entidades y DTOs de solicitudes de escalafón
/// </summary>
public class SolicitudEscalafonMappingProfile : Profile
{
    public SolicitudEscalafonMappingProfile()
    {
        // Mapeo de SolicitudEscalafon a SolicitudEscalafonDto
        CreateMap<SolicitudEscalafon, SolicitudEscalafonDto>();

        // Mapeo de CreateSolicitudEscalafonDto a SolicitudEscalafon
        CreateMap<CreateSolicitudEscalafonDto, SolicitudEscalafon>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.FechaSolicitud, opt => opt.Ignore())
            .ForMember(dest => dest.FechaAprobacion, opt => opt.Ignore())
            .ForMember(dest => dest.FechaRechazo, opt => opt.Ignore())
            .ForMember(dest => dest.FechaEnvioConsejo, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.ObservacionesConsejo, opt => opt.Ignore())
            .ForMember(dest => dest.MotivoRechazo, opt => opt.Ignore())
            .ForMember(dest => dest.MotivoRechazoConsejo, opt => opt.Ignore())
            .ForMember(dest => dest.ProcesadoPor, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());
    }
}
