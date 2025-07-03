using ProyectoAgiles.Application.DTOs;

namespace ProyectoAgiles.Application.Interfaces;

public interface IInvestigacionService
{
    Task<IEnumerable<InvestigacionDto>> GetAllAsync();
    Task<InvestigacionDto?> GetByIdAsync(int id);
    Task<IEnumerable<InvestigacionDto>> GetByCedulaAsync(string cedula);
    Task<IEnumerable<InvestigacionDto>> GetDisponiblesParaEscalafonAsync(string cedula);
    Task<IEnumerable<InvestigacionDto>> GetByTipoAsync(string tipo);
    Task<IEnumerable<InvestigacionDto>> GetByCampoConocimientoAsync(string campoConocimiento);    Task<InvestigacionDto> CreateAsync(CreateInvestigacionDto createDto);
    Task<InvestigacionDto> CreateWithPdfAsync(CreateInvestigacionWithPdfDto createDto);
    Task<InvestigacionDto> UpdateAsync(UpdateInvestigacionDto updateDto);
    Task<InvestigacionDto> UpdateWithPdfAsync(UpdateInvestigacionWithPdfDto updateDto);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<byte[]?> GetPdfByIdAsync(int id);
}
