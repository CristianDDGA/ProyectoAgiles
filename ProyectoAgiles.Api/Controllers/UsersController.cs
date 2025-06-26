using Microsoft.AspNetCore.Mvc;
using ProyectoAgiles.Application.DTOs;
using ProyectoAgiles.Application.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace ProyectoAgiles.Api.Controllers;

/// <summary>
/// 👥 Controlador de Gestión de Usuarios
/// </summary>
/// <remarks>
/// Este controlador maneja todas las operaciones CRUD relacionadas con los usuarios del sistema,
/// incluyendo gestión de perfiles, actualización de datos y administración de estados.
/// 
/// <para>
/// <strong>Funcionalidades principales:</strong>
/// </para>
/// <list type="bullet">
/// <item><description>👤 Consulta y gestión de usuarios</description></item>
/// <item><description>✏️ Actualización de información personal</description></item>
/// <item><description>🗑️ Eliminación de usuarios</description></item>
/// <item><description>🔄 Cambio de estados de usuario</description></item>
/// <item><description>📈 Gestión de niveles académicos</description></item>
/// </list>
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Tags("👥 Usuarios")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// 📋 Obtener todos los usuarios del sistema
    /// </summary>
    /// <remarks>
    /// Recupera una lista completa de todos los usuarios registrados en el sistema.
    /// 
    /// <para><strong>Información incluida por cada usuario:</strong></para>
    /// <list type="bullet">
    /// <item><description>👤 Datos personales (nombre, apellido, cédula)</description></item>
    /// <item><description>📧 Información de contacto</description></item>
    /// <item><description>🏢 Rol y permisos</description></item>
    /// <item><description>📅 Fechas de registro y actividad</description></item>
    /// <item><description>✅ Estado del usuario</description></item>
    /// </list>
    /// 
    /// <para><strong>💡 Uso típico:</strong> Administración de usuarios, reportes, dashboards</para>
    /// </remarks>
    /// <returns>Lista completa de usuarios del sistema</returns>
    /// <response code="200">✅ Lista de usuarios obtenida exitosamente</response>
    /// <response code="500">💥 Error interno del servidor</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
        Summary = "Obtener lista de todos los usuarios",
        Description = "Recupera una colección completa de todos los usuarios registrados en el sistema",
        OperationId = "GetAllUsers",
        Tags = new[] { "👥 Usuarios" }
    )]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
    {
        try
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
        }
    }

    /// <summary>
    /// 👤 Obtener usuario específico por ID
    /// </summary>
    /// <remarks>
    /// Recupera información detallada de un usuario específico mediante su identificador único.
    /// 
    /// <para><strong>Datos incluidos:</strong></para>
    /// <list type="bullet">
    /// <item><description>📋 Perfil completo del usuario</description></item>
    /// <item><description>🎓 Información académica y nivel</description></item>
    /// <item><description>📞 Datos de contacto</description></item>
    /// <item><description>🔐 Permisos y roles asignados</description></item>
    /// </list>
    /// </remarks>
    /// <param name="id">Identificador único del usuario</param>
    /// <returns>Información completa del usuario</returns>
    /// <response code="200">✅ Usuario encontrado exitosamente</response>
    /// <response code="404">❌ Usuario no encontrado</response>
    /// <response code="500">💥 Error interno del servidor</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
        Summary = "Obtener usuario por ID",
        Description = "Recupera la información detallada de un usuario específico",
        OperationId = "GetUserById",
        Tags = new[] { "👥 Usuarios" }
    )]
    public async Task<ActionResult<UserDto>> GetUser(int id)
    {
        try
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "Usuario no encontrado" });
            }

            return Ok(user);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
        }
    }

    /// <summary>
    /// ✏️ Actualizar información de usuario
    /// </summary>
    /// <remarks>
    /// Actualiza la información de un usuario existente con validación completa de datos.
    /// 
    /// <para><strong>Campos actualizables:</strong></para>
    /// <list type="bullet">
    /// <item><description>📝 Datos personales (nombre, apellido)</description></item>
    /// <item><description>📧 Información de contacto</description></item>
    /// <item><description>🔐 Credenciales de acceso</description></item>
    /// <item><description>🏢 Rol y permisos</description></item>
    /// </list>
    /// 
    /// <para><strong>Validaciones aplicadas:</strong></para>
    /// <list type="bullet">
    /// <item><description>✅ Unicidad de email y cédula</description></item>
    /// <item><description>🔒 Validación de contraseña segura</description></item>
    /// <item><description>📋 Verificación de datos obligatorios</description></item>
    /// </list>
    /// </remarks>
    /// <param name="id">ID del usuario a actualizar</param>
    /// <param name="updateDto">Nuevos datos del usuario</param>
    /// <returns>Usuario actualizado con la nueva información</returns>
    /// <response code="200">✅ Usuario actualizado exitosamente</response>
    /// <response code="400">❌ Datos inválidos o email/cédula ya existente</response>
    /// <response code="404">❌ Usuario no encontrado</response>
    /// <response code="500">💥 Error interno del servidor</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
        Summary = "Actualizar información de usuario",
        Description = "Modifica los datos de un usuario existente con validación completa",
        OperationId = "UpdateUser",
        Tags = new[] { "👥 Usuarios" }
    )]
    public async Task<ActionResult<UserDto>> UpdateUser(int id, [FromBody] RegisterDto updateDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userService.UpdateUserAsync(id, updateDto);
            if (user == null)
            {
                return NotFound(new { message = "Usuario no encontrado" });
            }

            return Ok(user);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
        }
    }

    /// <summary>
    /// 🗑️ Eliminar usuario del sistema
    /// </summary>
    /// <remarks>
    /// Elimina permanentemente un usuario del sistema. Esta acción es irreversible.
    /// 
    /// <para><strong>⚠️ Consideraciones importantes:</strong></para>
    /// <list type="bullet">
    /// <item><description>🚨 La eliminación es permanente e irreversible</description></item>
    /// <item><description>📋 Se eliminan todos los datos asociados</description></item>
    /// <item><description>🔄 Considera desactivar en lugar de eliminar</description></item>
    /// <item><description>📊 Los datos históricos pueden perderse</description></item>
    /// </list>
    /// 
    /// <para><strong>💡 Alternativa recomendada:</strong> Usar toggle-status para desactivar</para>
    /// </remarks>
    /// <param name="id">ID del usuario a eliminar</param>
    /// <returns>Confirmación de eliminación</returns>
    /// <response code="204">✅ Usuario eliminado exitosamente</response>
    /// <response code="404">❌ Usuario no encontrado</response>
    /// <response code="500">💥 Error interno del servidor</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
        Summary = "Eliminar usuario del sistema",
        Description = "Elimina permanentemente un usuario y todos sus datos asociados",
        OperationId = "DeleteUser",
        Tags = new[] { "👥 Usuarios" }
    )]
    public async Task<ActionResult> DeleteUser(int id)
    {
        try
        {
            var result = await _userService.DeleteUserAsync(id);
            if (!result)
            {
                return NotFound(new { message = "Usuario no encontrado" });
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
        }
    }

    /// <summary>
    /// 🔄 Alternar estado de usuario (Activar/Desactivar)
    /// </summary>
    /// <remarks>
    /// Cambia el estado de un usuario entre activo e inactivo de forma segura.
    /// 
    /// <para><strong>Estados del usuario:</strong></para>
    /// <list type="bullet">
    /// <item><description>✅ Activo: Usuario puede acceder al sistema</description></item>
    /// <item><description>❌ Inactivo: Acceso bloqueado, datos preservados</description></item>
    /// </list>
    /// 
    /// <para><strong>Ventajas sobre eliminación:</strong></para>
    /// <list type="bullet">
    /// <item><description>📊 Preserva datos históricos</description></item>
    /// <item><description>🔄 Acción reversible</description></item>
    /// <item><description>🔐 Control de acceso granular</description></item>
    /// </list>
    /// </remarks>
    /// <param name="id">ID del usuario cuyo estado se va a cambiar</param>
    /// <returns>Confirmación del cambio de estado</returns>
    /// <response code="200">✅ Estado del usuario cambiado exitosamente</response>
    /// <response code="404">❌ Usuario no encontrado</response>
    /// <response code="500">💥 Error interno del servidor</response>
    [HttpPatch("{id}/toggle-status")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
        Summary = "Alternar estado de usuario",
        Description = "Cambia el estado de un usuario entre activo e inactivo",
        OperationId = "ToggleUserStatus",
        Tags = new[] { "👥 Usuarios" }
    )]
    public async Task<ActionResult> ToggleUserStatus(int id)
    {
        try
        {
            var result = await _userService.ToggleUserStatusAsync(id);
            if (!result)
            {
                return NotFound(new { message = "Usuario no encontrado" });
            }

            return Ok(new { message = "Estado del usuario actualizado correctamente" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
        }
    }

    /// <summary>
    /// 📈 Ascender docente a siguiente nivel académico
    /// </summary>
    /// <remarks>
    /// Promueve a un docente al siguiente nivel académico en la escala universitaria.
    /// 
    /// <para><strong>Jerarquía de niveles académicos:</strong></para>
    /// <list type="number">
    /// <item><description>🎓 Titular Auxiliar 1 (Nivel inicial)</description></item>
    /// <item><description>🎓 Titular Auxiliar 2</description></item>
    /// <item><description>🎓 Titular Principal</description></item>
    /// <item><description>🎓 Titular Agregado (Nivel máximo)</description></item>
    /// </list>
    /// 
    /// <para><strong>Requisitos:</strong></para>
    /// <list type="bullet">
    /// <item><description>👨‍🏫 Solo aplicable a usuarios con rol "Docente"</description></item>
    /// <item><description>📈 No estar en el nivel máximo</description></item>
    /// <item><description>✅ Usuario activo en el sistema</description></item>
    /// </list>
    /// </remarks>
    /// <param name="id">ID del docente a ascender</param>
    /// <returns>Confirmación del ascenso</returns>
    /// <response code="200">✅ Docente ascendido exitosamente</response>
    /// <response code="400">❌ Usuario no es docente o ya está en nivel máximo</response>
    /// <response code="404">❌ Usuario no encontrado</response>
    /// <response code="500">💥 Error interno del servidor</response>
    [HttpPost("{id}/subir-nivel")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
        Summary = "Ascender docente a siguiente nivel",
        Description = "Promueve a un docente al siguiente nivel académico en la jerarquía universitaria",
        OperationId = "PromoteTeacher",
        Tags = new[] { "🎓 Académico" }
    )]
    public async Task<IActionResult> SubirNivel(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
            return NotFound();
        if (user.UserType != ProyectoAgiles.Domain.Enums.UserType.Docente)
            return BadRequest("Solo los docentes pueden subir de nivel.");

        // Lógica simple: cambiar el nivel a un valor superior (ejemplo)
        var niveles = new[] { "titular auxiliar 1", "titular auxiliar 2", "titular principal", "titular agregado" };
        var actual = Array.IndexOf(niveles, user.Nivel);
        if (actual < 0 || actual == niveles.Length - 1)
            return BadRequest("Ya tienes el nivel más alto o nivel desconocido.");
        user.Nivel = niveles[actual + 1];
        await _userService.UpdateUserNivelAsync(id, user.Nivel);
        return Ok();
    }

    /// <summary>
    /// 🔍 Buscar usuario por número de cédula
    /// </summary>
    /// <remarks>
    /// Localiza un usuario específico utilizando su número de cédula de identidad.
    /// 
    /// <para><strong>Ventajas de búsqueda por cédula:</strong></para>
    /// <list type="bullet">
    /// <item><description>🆔 Identificador único e inmutable</description></item>
    /// <item><description>📋 Búsqueda precisa y confiable</description></item>
    /// <item><description>🏛️ Validación oficial</description></item>
    /// </list>
    /// 
    /// <para><strong>💡 Casos de uso:</strong></para>
    /// <list type="bullet">
    /// <item><description>🔍 Búsqueda administrativa</description></item>
    /// <item><description>📊 Verificación de identidad</description></item>
    /// <item><description>🎯 Consultas específicas</description></item>
    /// </list>
    /// </remarks>
    /// <param name="cedula">Número de cédula del usuario a buscar</param>
    /// <returns>Información del usuario encontrado</returns>
    /// <response code="200">✅ Usuario encontrado exitosamente</response>
    /// <response code="404">❌ No se encontró usuario con esa cédula</response>
    /// <response code="500">💥 Error interno del servidor</response>
    [HttpGet("by-cedula/{cedula}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
        Summary = "Buscar usuario por cédula",
        Description = "Localiza un usuario específico mediante su número de cédula de identidad",
        OperationId = "GetUserByCedula",
        Tags = new[] { "🔍 Búsquedas" }
    )]
    public async Task<ActionResult<UserDto>> GetUserByCedula(string cedula)
    {
        try
        {
            var users = await _userService.GetAllUsersAsync();
            var user = users.FirstOrDefault(u => u.Cedula == cedula);
            
            if (user == null)
            {
                return NotFound(new { message = "Usuario no encontrado" });
            }

            return Ok(user);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
        }
    }

    /// <summary>
    /// 🎓 Ascender docente por cédula con notificación
    /// </summary>
    /// <remarks>
    /// Promueve a un docente al siguiente nivel académico usando su cédula como identificador,
    /// proporcionando una respuesta detallada del proceso.
    /// 
    /// <para><strong>Proceso de ascenso:</strong></para>
    /// <list type="number">
    /// <item><description>🔍 Localización del docente por cédula</description></item>
    /// <item><description>✅ Validación de elegibilidad</description></item>
    /// <item><description>📈 Promoción al siguiente nivel</description></item>
    /// <item><description>🎉 Notificación de felicitación</description></item>
    /// </list>
    /// 
    /// <para><strong>Información en la respuesta:</strong></para>
    /// <list type="bullet">
    /// <item><description>🎊 Mensaje de felicitación personalizado</description></item>
    /// <item><description>📊 Nivel académico anterior</description></item>
    /// <item><description>🆙 Nuevo nivel alcanzado</description></item>
    /// </list>
    /// 
    /// <para><strong>💡 Ejemplo de respuesta exitosa:</strong></para>
    /// <code>
    /// {
    ///   "message": "¡Felicidades! Has subido de nivel a titular principal",
    ///   "nivelAnterior": "titular auxiliar 2",
    ///   "nuevoNivel": "titular principal"
    /// }
    /// </code>
    /// </remarks>
    /// <param name="cedula">Número de cédula del docente a ascender</param>
    /// <returns>Confirmación detallada del ascenso con niveles anterior y nuevo</returns>
    /// <response code="200">✅ Docente ascendido exitosamente con detalles</response>
    /// <response code="400">❌ Usuario no es docente o ya está en nivel máximo</response>
    /// <response code="404">❌ Usuario no encontrado con esa cédula</response>
    /// <response code="500">💥 Error interno del servidor</response>
    [HttpPost("by-cedula/{cedula}/subir-nivel")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
        Summary = "Ascender docente por cédula con notificación",
        Description = "Promueve a un docente usando su cédula y proporciona detalles completos del ascenso",
        OperationId = "PromoteTeacherByCedula",
        Tags = new[] { "🎓 Académico" }
    )]
    public async Task<IActionResult> SubirNivelPorCedula(string cedula)
    {
        try
        {
            var users = await _userService.GetAllUsersAsync();
            var user = users.FirstOrDefault(u => u.Cedula == cedula);
            
            if (user == null)
                return NotFound(new { message = "Usuario no encontrado" });
                
            if (user.UserType != ProyectoAgiles.Domain.Enums.UserType.Docente)
                return BadRequest(new { message = "Solo los docentes pueden subir de nivel." });

            // Lógica para subir de nivel
            var niveles = new[] { "titular auxiliar 1", "titular auxiliar 2", "titular principal", "titular agregado" };
            var nivelActual = user.Nivel?.ToLower() ?? "titular auxiliar 1";
            var actual = Array.IndexOf(niveles, nivelActual);
            
            if (actual < 0)
            {
                // Si no se encuentra el nivel, asumir titular auxiliar 1
                actual = 0;
            }
            
            if (actual == niveles.Length - 1)
                return BadRequest(new { message = "Ya tienes el nivel más alto." });
                
            var nuevoNivel = niveles[actual + 1];
            await _userService.UpdateUserNivelAsync(user.Id, nuevoNivel);
            
            return Ok(new { 
                message = $"¡Felicidades! Has subido de nivel a {nuevoNivel}",
                nivelAnterior = nivelActual,
                nuevoNivel = nuevoNivel
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
        }
    }
}
