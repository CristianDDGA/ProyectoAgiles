using ProyectoAgiles.Application.DTOs;
using ProyectoAgiles.Application.Interfaces;
using ProyectoAgiles.Domain.Entities;
using ProyectoAgiles.Domain.Interfaces;

namespace ProyectoAgiles.Application.Services;

public class InvestigacionService : IInvestigacionService
{
    private readonly IInvestigacionRepository _investigacionRepository;

    public InvestigacionService(IInvestigacionRepository investigacionRepository)
    {
        _investigacionRepository = investigacionRepository;
    }

    public async Task<IEnumerable<InvestigacionDto>> GetAllAsync()
    {
        var investigaciones = await _investigacionRepository.GetAllAsync();
        return investigaciones.Select(MapToDto);
    }

    public async Task<InvestigacionDto?> GetByIdAsync(int id)
    {
        var investigacion = await _investigacionRepository.GetByIdAsync(id);
        return investigacion != null ? MapToDto(investigacion) : null;
    }

    public async Task<IEnumerable<InvestigacionDto>> GetByCedulaAsync(string cedula)
    {
        var investigaciones = await _investigacionRepository.GetByCedulaAsync(cedula);
        return investigaciones.Select(MapToDto);
    }

    public async Task<IEnumerable<InvestigacionDto>> GetByTipoAsync(string tipo)
    {
        var investigaciones = await _investigacionRepository.GetByTipoAsync(tipo);
        return investigaciones.Select(MapToDto);
    }

    public async Task<IEnumerable<InvestigacionDto>> GetByCampoConocimientoAsync(string campoConocimiento)
    {
        var investigaciones = await _investigacionRepository.GetByCampoConocimientoAsync(campoConocimiento);
        return investigaciones.Select(MapToDto);
    }

    public async Task<InvestigacionDto> CreateAsync(CreateInvestigacionDto createDto)
    {
        var investigacion = new Investigacion
        {
            Cedula = createDto.Cedula,
            Titulo = createDto.Titulo,
            Tipo = createDto.Tipo,
            RevistaOEditorial = createDto.RevistaOEditorial,
            FechaPublicacion = createDto.FechaPublicacion,
            CampoConocimiento = createDto.CampoConocimiento,
            Filiacion = createDto.Filiacion,
            Observacion = createDto.Observacion
        };

        var createdInvestigacion = await _investigacionRepository.CreateAsync(investigacion);
        return MapToDto(createdInvestigacion);
    }    public async Task<InvestigacionDto> CreateWithPdfAsync(CreateInvestigacionWithPdfDto createDto)
    {
        byte[]? archivoPdfBytes = null;
        if (createDto.ArchivoPdf != null)
        {
            using var ms = new MemoryStream();
            await createDto.ArchivoPdf.CopyToAsync(ms);
            archivoPdfBytes = ms.ToArray();
            
            // Log para depuración
            Console.WriteLine($"CreateWithPdfAsync - PDF procesado: {archivoPdfBytes.Length} bytes");
        }
        else
        {
            Console.WriteLine("CreateWithPdfAsync - No se recibió archivo PDF");
        }
        
        var investigacion = new Investigacion
        {
            Cedula = createDto.Cedula,
            Titulo = createDto.Titulo,
            Tipo = createDto.Tipo,
            RevistaOEditorial = createDto.RevistaOEditorial,
            FechaPublicacion = createDto.FechaPublicacion,
            CampoConocimiento = createDto.CampoConocimiento,
            Filiacion = createDto.Filiacion,
            Observacion = createDto.Observacion,
            ArchivoPdf = archivoPdfBytes
        };
        var createdInvestigacion = await _investigacionRepository.CreateAsync(investigacion);
        
        // Log para verificar que se guardó
        Console.WriteLine($"CreateWithPdfAsync - Investigación creada ID: {createdInvestigacion.Id}, PDF guardado: {createdInvestigacion.ArchivoPdf?.Length ?? 0} bytes");
        
        return MapToDto(createdInvestigacion);
    }

    public async Task<InvestigacionDto> UpdateAsync(UpdateInvestigacionDto updateDto)
    {
        var existingInvestigacion = await _investigacionRepository.GetByIdAsync(updateDto.Id);
        if (existingInvestigacion == null)
            throw new ArgumentException($"No se encontró la investigación con ID {updateDto.Id}");

        existingInvestigacion.Cedula = updateDto.Cedula;
        existingInvestigacion.Titulo = updateDto.Titulo;
        existingInvestigacion.Tipo = updateDto.Tipo;
        existingInvestigacion.RevistaOEditorial = updateDto.RevistaOEditorial;
        existingInvestigacion.FechaPublicacion = updateDto.FechaPublicacion;
        existingInvestigacion.CampoConocimiento = updateDto.CampoConocimiento;
        existingInvestigacion.Filiacion = updateDto.Filiacion;
        existingInvestigacion.Observacion = updateDto.Observacion;

        var updatedInvestigacion = await _investigacionRepository.UpdateAsync(existingInvestigacion);
        return MapToDto(updatedInvestigacion);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _investigacionRepository.DeleteAsync(id);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _investigacionRepository.ExistsAsync(id);
    }    public async Task<byte[]?> GetPdfByIdAsync(int id)
    {
        Console.WriteLine($"GetPdfByIdAsync - Buscando investigación con ID: {id}");
        
        var investigacion = await _investigacionRepository.GetByIdAsync(id);
        
        if (investigacion == null)
        {
            Console.WriteLine($"GetPdfByIdAsync - No se encontró investigación con ID: {id}");
            return null;
        }
        
        Console.WriteLine($"GetPdfByIdAsync - Investigación encontrada. PDF: {investigacion.ArchivoPdf?.Length ?? 0} bytes");
        
        return investigacion.ArchivoPdf;
    }

    private static InvestigacionDto MapToDto(Investigacion investigacion)
    {
        return new InvestigacionDto
        {
            Id = investigacion.Id,
            Cedula = investigacion.Cedula,
            Titulo = investigacion.Titulo,
            Tipo = investigacion.Tipo,
            RevistaOEditorial = investigacion.RevistaOEditorial,
            FechaPublicacion = investigacion.FechaPublicacion,
            CampoConocimiento = investigacion.CampoConocimiento,
            Filiacion = investigacion.Filiacion,
            Observacion = investigacion.Observacion,
            CreatedAt = investigacion.CreatedAt,
            UpdatedAt = investigacion.UpdatedAt,
            TienePdf = investigacion.ArchivoPdf != null && investigacion.ArchivoPdf.Length > 0
        };
    }
}
