using Microsoft.AspNetCore.Mvc;
using ProyectoAgiles.Domain.Interfaces;
using ProyectoAgiles.Domain.Entities;

namespace ProyectoAgiles.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TTHHController : ControllerBase
    {
        private readonly ITTHHRepository _tthhRepository;

        public TTHHController(ITTHHRepository tthhRepository)
        {
            _tthhRepository = tthhRepository;
        }

        /// <summary>
        /// Obtiene información TTHH por cédula
        /// </summary>
        /// <param name="cedula">Cédula del docente</param>
        /// <returns>Información TTHH del docente</returns>
        [HttpGet("by-cedula/{cedula}")]
        public async Task<ActionResult<TTHHDto>> GetByCedula(string cedula)
        {
            try
            {
                var tthh = await _tthhRepository.GetByCedulaAsync(cedula);
                
                if (tthh == null)
                {
                    return NotFound($"No se encontró información TTHH para la cédula {cedula}");
                }

                var tthhDto = new TTHHDto
                {
                    Id = tthh.Id,
                    Cedula = tthh.Cedula,
                    FechaInicio = tthh.FechaInicio,
                    AniosCumplidos = tthh.AniosCumplidos,
                    Observacion = tthh.Observacion,
                    CreatedAt = tthh.CreatedAt,
                    UpdatedAt = tthh.UpdatedAt
                };

                return Ok(tthhDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene todos los registros TTHH
        /// </summary>
        /// <returns>Lista de registros TTHH</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TTHHDto>>> GetAll()
        {
            try
            {
                var tthhList = await _tthhRepository.GetAllAsync();
                
                var tthhDtos = tthhList.Select(tthh => new TTHHDto
                {
                    Id = tthh.Id,
                    Cedula = tthh.Cedula,
                    FechaInicio = tthh.FechaInicio,
                    AniosCumplidos = tthh.AniosCumplidos,
                    Observacion = tthh.Observacion,
                    CreatedAt = tthh.CreatedAt,
                    UpdatedAt = tthh.UpdatedAt
                }).ToList();

                return Ok(tthhDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Crea un nuevo registro TTHH
        /// </summary>
        /// <param name="createDto">Datos para crear el registro TTHH</param>
        /// <returns>Registro TTHH creado</returns>
        [HttpPost]
        public async Task<ActionResult<TTHHDto>> Create([FromBody] CreateTTHHDto createDto)
        {
            try
            {
                // Verificar que no exista ya un registro para esta cédula
                var existingTthh = await _tthhRepository.GetByCedulaAsync(createDto.Cedula);
                if (existingTthh != null)
                {
                    return BadRequest($"Ya existe un registro TTHH para la cédula {createDto.Cedula}");
                }

                var tthh = new TTHH
                {
                    Cedula = createDto.Cedula,
                    FechaInicio = createDto.FechaInicio,
                    Observacion = createDto.Observacion
                };

                var createdTthh = await _tthhRepository.AddAsync(tthh);

                var tthhDto = new TTHHDto
                {
                    Id = createdTthh.Id,
                    Cedula = createdTthh.Cedula,
                    FechaInicio = createdTthh.FechaInicio,
                    AniosCumplidos = createdTthh.AniosCumplidos,
                    Observacion = createdTthh.Observacion,
                    CreatedAt = createdTthh.CreatedAt,
                    UpdatedAt = createdTthh.UpdatedAt
                };

                return CreatedAtAction(nameof(GetByCedula), new { cedula = createdTthh.Cedula }, tthhDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }
    }

    // DTOs para TTHH
    public class TTHHDto
    {
        public int Id { get; set; }
        public string Cedula { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public double AniosCumplidos { get; set; }
        public string Observacion { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateTTHHDto
    {
        public string Cedula { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public string Observacion { get; set; } = string.Empty;
    }
}
