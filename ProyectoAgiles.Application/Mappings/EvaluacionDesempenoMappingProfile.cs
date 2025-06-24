using AutoMapper;
using ProyectoAgiles.Application.DTOs;
using ProyectoAgiles.Domain.Entities;

namespace ProyectoAgiles.Application.Mappings;

/// <summary>
/// Perfil de AutoMapper para mapear entidades y DTOs de evaluaciones de desempeño
/// </summary>
public class EvaluacionDesempenoMappingProfile : Profile
{
    public EvaluacionDesempenoMappingProfile()
    {
        // Mapeo de EvaluacionDesempeno a EvaluacionDesempenoDto
        CreateMap<EvaluacionDesempeno, EvaluacionDesempenoDto>()
            .ForMember(dest => dest.PorcentajeObtenido, opt => opt.MapFrom(src => src.PorcentajeObtenido))
            .ForMember(dest => dest.CumpleMinimo, opt => opt.MapFrom(src => src.CumpleMinimo));

        // Mapeo de CreateEvaluacionDesempenoDto a EvaluacionDesempeno
        CreateMap<CreateEvaluacionDesempenoDto, EvaluacionDesempeno>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.ArchivoRespaldo, opt => opt.Ignore())
            .ForMember(dest => dest.NombreArchivoRespaldo, opt => opt.Ignore());

        // Mapeo de CreateEvaluacionWithPdfDto a EvaluacionDesempeno
        CreateMap<CreateEvaluacionWithPdfDto, EvaluacionDesempeno>()
            .IncludeBase<CreateEvaluacionDesempenoDto, EvaluacionDesempeno>()
            .ForMember(dest => dest.ArchivoRespaldo, opt => opt.Ignore()) // Se maneja en el servicio
            .ForMember(dest => dest.NombreArchivoRespaldo, opt => opt.Ignore()); // Se maneja en el servicio

        // Mapeo de UpdateEvaluacionDesempenoDto a EvaluacionDesempeno
        CreateMap<UpdateEvaluacionDesempenoDto, EvaluacionDesempeno>()
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.ArchivoRespaldo, opt => opt.Ignore())
            .ForMember(dest => dest.NombreArchivoRespaldo, opt => opt.Ignore());

        // Mapeo de UpdateEvaluacionWithPdfDto a EvaluacionDesempeno
        CreateMap<UpdateEvaluacionWithPdfDto, EvaluacionDesempeno>()
            .IncludeBase<UpdateEvaluacionDesempenoDto, EvaluacionDesempeno>()
            .ForMember(dest => dest.ArchivoRespaldo, opt => opt.Ignore()) // Se maneja en el servicio
            .ForMember(dest => dest.NombreArchivoRespaldo, opt => opt.Ignore()); // Se maneja en el servicio
    }
}
