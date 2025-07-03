using Microsoft.JSInterop;
using ProyectoAgiles.Application.DTOs;
using System.Text.Json;
using System.Net.Http.Json;

namespace proyectoAgiles.Services
{
    public class ReportesService
    {
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _jsRuntime;

        public ReportesService(HttpClient httpClient, IJSRuntime jsRuntime)
        {
            _httpClient = httpClient;
            _jsRuntime = jsRuntime;
        }

        public async Task<List<SolicitudEscalafonDto>> ObtenerSolicitudesAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<SolicitudEscalafonDto>>("/api/solicitudes-escalafon");
                return response ?? new List<SolicitudEscalafonDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener solicitudes: {ex.Message}");
                // Retornar datos de ejemplo para testing
                return GenerarDatosEjemplo();
            }
        }

        public async Task<List<SolicitudEscalafonDto>> ObtenerSolicitudesAprobadasAsync()
        {
            var solicitudes = await ObtenerSolicitudesAsync();
            return solicitudes.Where(s => s.Status == "Aprobada" || s.Status == "Procesada" || s.Status == "Finalizada").ToList();
        }

        private List<SolicitudEscalafonDto> GenerarDatosEjemplo()
        {
            return new List<SolicitudEscalafonDto>
            {
                new SolicitudEscalafonDto
                {
                    Id = 1,
                    DocenteNombre = "Juan Pérez García",
                    DocenteCedula = "1234567890",
                    DocenteEmail = "juan.perez@uta.edu.ec",
                    DocenteTelefono = "0987654321",
                    Facultad = "Facultad de Ingeniería",
                    Carrera = "Ingeniería en Sistemas",
                    NivelActual = "Auxiliar 1",
                    NivelSolicitado = "Auxiliar 2",
                    AnosExperiencia = 5,
                    FechaSolicitud = DateTime.Now.AddMonths(-3),
                    FechaAprobacion = DateTime.Now.AddMonths(-1),
                    Status = "Aprobada",
                    Titulos = "Ing. en Sistemas, MSc. en Informática",
                    Publicaciones = "5 artículos científicos",
                    ProyectosInvestigacion = "Proyecto de IA aplicada",
                    Capacitaciones = "Curso Docker, AWS Certified",
                    Observaciones = "Cumple con todos los requisitos"
                },
                new SolicitudEscalafonDto
                {
                    Id = 2,
                    DocenteNombre = "María González López",
                    DocenteCedula = "0987654321",
                    DocenteEmail = "maria.gonzalez@uta.edu.ec",
                    DocenteTelefono = "0987654322",
                    Facultad = "Facultad de Ciencias Humanas",
                    Carrera = "Psicología",
                    NivelActual = "Auxiliar 2",
                    NivelSolicitado = "Agregado 1",
                    AnosExperiencia = 8,
                    FechaSolicitud = DateTime.Now.AddMonths(-2),
                    FechaAprobacion = DateTime.Now.AddDays(-15),
                    Status = "Aprobada",
                    Titulos = "Psicóloga, MSc. en Psicología Clínica",
                    Publicaciones = "8 artículos científicos",
                    ProyectosInvestigacion = "Estudio sobre comportamiento",
                    Capacitaciones = "Diplomado en Terapia Cognitiva",
                    Observaciones = "Excelente desempeño"
                },
                new SolicitudEscalafonDto
                {
                    Id = 3,
                    DocenteNombre = "Carlos Ramírez Ortiz",
                    DocenteCedula = "1122334455",
                    DocenteEmail = "carlos.ramirez@uta.edu.ec",
                    DocenteTelefono = "0987654323",
                    Facultad = "Facultad de Ciencias Administrativas",
                    Carrera = "Administración de Empresas",
                    NivelActual = "Agregado 1",
                    NivelSolicitado = "Agregado 2",
                    AnosExperiencia = 12,
                    FechaSolicitud = DateTime.Now.AddMonths(-4),
                    FechaAprobacion = DateTime.Now.AddMonths(-2),
                    Status = "Procesada",
                    Titulos = "Ing. Comercial, MBA",
                    Publicaciones = "12 artículos científicos",
                    ProyectosInvestigacion = "Análisis de mercados emergentes",
                    Capacitaciones = "Certificación en Project Management",
                    Observaciones = "Liderazgo en proyectos de investigación"
                }
            };
        }

        public async Task ExportarFichaPDFAsync(SolicitudEscalafonDto solicitud)
        {
            try
            {
                Console.WriteLine($"[ReportesService] Iniciando exportación PDF para: {solicitud.DocenteNombre}");
                
                var fichaData = GenerarDatosFicha(solicitud);
                var json = JsonSerializer.Serialize(fichaData, new JsonSerializerOptions { WriteIndented = true });
                
                var nombreArchivo = $"Ficha_{solicitud.DocenteNombre.Replace(" ", "_")}_{solicitud.DocenteCedula}.pdf";
                Console.WriteLine($"[ReportesService] Nombre archivo: {nombreArchivo}");
                
                await _jsRuntime.InvokeVoidAsync("descargarPDF", json, nombreArchivo);
                
                Console.WriteLine("[ReportesService] Función JS de descarga invocada exitosamente");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ReportesService] Error al exportar ficha: {ex.Message}");
                await _jsRuntime.InvokeVoidAsync("alert", "Error al generar el PDF. Intente nuevamente.");
            }
        }

        public object GenerarDatosFicha(SolicitudEscalafonDto solicitud)
        {
            return new
            {
                titulo = "FICHA DE ASCENSO DOCENTE",
                subtitulo = "Universidad Técnica de Ambato",
                fecha = DateTime.Now.ToString("dd/MM/yyyy"),
                docente = new
                {
                    nombre = solicitud.DocenteNombre,
                    cedula = solicitud.DocenteCedula,
                    email = solicitud.DocenteEmail,
                    telefono = solicitud.DocenteTelefono ?? "No especificado",
                    facultad = solicitud.Facultad ?? "No especificado",
                    carrera = solicitud.Carrera ?? "No especificado"
                },
                escalafon = new
                {
                    nivelActual = solicitud.NivelActual,
                    nivelSolicitado = solicitud.NivelSolicitado,
                    fechaSolicitud = solicitud.FechaSolicitud.ToString("dd/MM/yyyy"),
                    fechaAprobacion = solicitud.FechaAprobacion?.ToString("dd/MM/yyyy") ?? "No especificado",
                    anosExperiencia = solicitud.AnosExperiencia,
                    status = solicitud.Status
                },
                detalles = new
                {
                    titulos = solicitud.Titulos ?? "No especificado",
                    publicaciones = solicitud.Publicaciones ?? "No especificado",
                    proyectosInvestigacion = solicitud.ProyectosInvestigacion ?? "No especificado",
                    capacitaciones = solicitud.Capacitaciones ?? "No especificado",
                    observaciones = solicitud.Observaciones ?? "No especificado"
                }
            };
        }

        public async Task MostrarVistaPrevia(SolicitudEscalafonDto solicitud)
        {
            try
            {
                Console.WriteLine($"[ReportesService] Iniciando vista previa para: {solicitud.DocenteNombre}");
                
                var fichaData = GenerarDatosFicha(solicitud);
                var json = JsonSerializer.Serialize(fichaData, new JsonSerializerOptions { WriteIndented = true });
                
                Console.WriteLine($"[ReportesService] JSON generado: {json.Substring(0, Math.Min(100, json.Length))}...");
                
                await _jsRuntime.InvokeVoidAsync("mostrarVistaPrevia", json);
                
                Console.WriteLine("[ReportesService] Función JS invocada exitosamente");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ReportesService] Error en MostrarVistaPrevia: {ex.Message}");
                await _jsRuntime.InvokeVoidAsync("alert", $"Error al mostrar vista previa: {ex.Message}");
            }
        }
    }
}
