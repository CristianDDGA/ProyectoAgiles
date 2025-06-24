using AutoMapper;
using ProyectoAgiles.Application.DTOs;
using ProyectoAgiles.Domain.Entities;

namespace ProyectoAgiles.Application.Mappings;

/// <summary>
/// Perfil de mapeo para las capacitaciones DITIC
/// </summary>
public class DiticMappingProfile : Profile
{
    public DiticMappingProfile()
    {
        // Mapeo de entidad a DTO de lectura
        CreateMap<DITIC, DiticDto>()
            .ForMember(dest => dest.Aprobada, opt => opt.MapFrom(src => src.Aprobada))
            .ForMember(dest => dest.EsPedagogica, opt => opt.MapFrom(src => src.EsPedagogica))
            .ForMember(dest => dest.AñosComoAutoridad, opt => opt.MapFrom(src => src.AñosComoAutoridad))
            .ForMember(dest => dest.CumpleExencionAutoridad, opt => opt.MapFrom(src => src.CumpleExencionAutoridad));

        // Mapeo de DTO de creación a entidad
        CreateMap<CreateDiticDto, DITIC>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.ArchivoCertificado, opt => opt.Ignore())
            .ForMember(dest => dest.NombreArchivoCertificado, opt => opt.Ignore());

        // Mapeo de DTO de creación con PDF a entidad
        CreateMap<CreateDiticWithPdfDto, DITIC>()
            .IncludeBase<CreateDiticDto, DITIC>()
            .ForMember(dest => dest.ArchivoCertificado, opt => opt.Ignore())
            .ForMember(dest => dest.NombreArchivoCertificado, opt => opt.Ignore());        // Mapeo de DTO de actualización a entidad
        CreateMap<UpdateDiticDto, DITIC>()
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.ArchivoCertificado, opt => opt.Ignore())
            .ForMember(dest => dest.NombreArchivoCertificado, opt => opt.Ignore());
    }
}
