using ProyectoAgiles.Application.DTOs;

namespace ProyectoAgiles.Application.Interfaces;

/// <summary>
/// Interfaz para el servicio de gestión de requisitos de escalafón
/// </summary>
public interface IRequisitosEscalafonService
{
    /// <summary>
    /// Obtiene los requisitos para ascender al siguiente nivel según el nivel actual del docente
    /// </summary>
    /// <param name="nivelActual">Nivel académico actual del docente</param>
    /// <returns>Configuración de requisitos para el siguiente nivel</returns>
    RequisitoEscalafonConfigDto GetRequisitosParaNivel(string nivelActual);

    /// <summary>
    /// Obtiene todos los niveles académicos disponibles en orden jerárquico
    /// </summary>
    /// <returns>Lista de niveles académicos ordenados</returns>
    List<string> GetNivelesAcademicos();

    /// <summary>
    /// Obtiene el siguiente nivel académico en la jerarquía
    /// </summary>
    /// <param name="nivelActual">Nivel académico actual</param>
    /// <returns>Siguiente nivel en la jerarquía o null si es el máximo</returns>
    string? GetSiguienteNivel(string nivelActual);

    /// <summary>
    /// Verifica si un nivel puede ascender al siguiente
    /// </summary>
    /// <param name="nivelActual">Nivel académico actual</param>
    /// <returns>True si puede ascender, false si es el nivel máximo</returns>
    bool PuedeAscender(string nivelActual);
}
