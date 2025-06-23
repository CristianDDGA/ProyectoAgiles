using ProyectoAgiles.Application.DTOs;
using ProyectoAgiles.Application.Interfaces;
using ProyectoAgiles.Domain.Entities;
using ProyectoAgiles.Domain.Interfaces;
using System.Security.Cryptography;

namespace ProyectoAgiles.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;
    private readonly IPasswordResetTokenRepository _passwordResetTokenRepository;
    private readonly IFileService _fileService;

    public AuthService(
        IUserRepository userRepository,
        IEmailService emailService,
        IPasswordResetTokenRepository passwordResetTokenRepository,
        IFileService fileService)
    {
        _userRepository = userRepository;
        _emailService = emailService;
        _passwordResetTokenRepository = passwordResetTokenRepository;
        _fileService = fileService;
    }    public async Task<UserDto?> RegisterAsync(RegisterDto registerDto)
    {
        // Verificar si el email ya existe
        if (await _userRepository.EmailExistsAsync(registerDto.Email))
        {
            throw new InvalidOperationException("El email ya está registrado");
        }

        // Verificar si la cédula ya existe
        if (await _userRepository.CedulaExistsAsync(registerDto.Cedula))
        {
            throw new InvalidOperationException("La cédula ya está registrada");
        }

        // Procesar archivo de documento de identidad si existe
        string? documentPath = null;
        if (registerDto.IdentityDocument != null && 
            !string.IsNullOrEmpty(registerDto.IdentityDocumentFileName) &&
            !string.IsNullOrEmpty(registerDto.IdentityDocumentContentType))
        {
            try
            {
                documentPath = await _fileService.SaveFileAsync(
                    registerDto.IdentityDocument,
                    registerDto.IdentityDocumentFileName,
                    registerDto.IdentityDocumentContentType,
                    "uploads/documents"
                );
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al procesar el documento de identidad: {ex.Message}");
            }
        }

        // Crear nuevo usuario con rol de docente
        var user = new User
        {
            FirstName = registerDto.FirstName.Trim(),
            LastName = registerDto.LastName.Trim(),
            Email = registerDto.Email.Trim().ToLower(),
            PasswordHash = registerDto.Password, // Se hashea en el repositorio
            UserType = Domain.Enums.UserType.Docente, // Asignar automáticamente rol de docente
            Cedula = registerDto.Cedula.Trim(),
            IdentityDocumentPath = documentPath,
            IsActive = true
        };        var createdUser = await _userRepository.AddAsync(user);
        return MapToDto(createdUser);
    }    public async Task<LoginResponse> LoginAsync(LoginDto loginDto)
    {
        var user = await _userRepository.GetByEmailAsync(loginDto.Email);
        
        if (user == null)
        {
            return new LoginResponse
            {
                Success = false,
                Message = "Email o contraseña incorrectos"
            };
        }
        
        // Verificar si la cuenta está bloqueada
        if (user.IsLocked)
        {
            // Si hay un tiempo de bloqueo definido, verificar si ya expiró
            if (user.LockoutEnd.HasValue && user.LockoutEnd.Value <= DateTime.UtcNow)
            {
                // El bloqueo ha expirado, desbloquear la cuenta
                await UnlockUserAccountAsync(user);
            }
            else
            {
                // La cuenta sigue bloqueada
                return new LoginResponse
                {
                    Success = false,
                    Message = "Tu cuenta está bloqueada por múltiples intentos fallidos. Para desbloquearla, utiliza la opción 'Olvidé mi contraseña' o contacta al administrador.",
                    IsAccountLocked = true,
                    FailedAttempts = user.FailedLoginAttempts
                };
            }
        }
        
        // Validar la contraseña
        var validatedUser = await _userRepository.ValidateUserAsync(loginDto.Email, loginDto.Password);
        
        if (validatedUser != null)
        {
            // Login exitoso - resetear contadores de fallos
            if (user.FailedLoginAttempts > 0)
            {
                await ResetFailedLoginAttemptsAsync(user);
            }
            return new LoginResponse
            {
                Success = true,
                Message = "Inicio de sesión exitoso",
                User = MapToDto(validatedUser)
            };
        }        else
        {
            // Login fallido - incrementar contador
            await IncrementFailedLoginAttemptsAsync(user);
            
            // Verificar si ahora está bloqueada después del incremento
            var updatedUser = await _userRepository.GetByIdAsync(user.Id);
            if (updatedUser?.IsLocked == true)
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = "Tu cuenta ha sido bloqueada por múltiples intentos fallidos. Para desbloquearla, utiliza la opción 'Olvidé mi contraseña' o contacta al administrador.",
                    IsAccountLocked = true,
                    FailedAttempts = updatedUser.FailedLoginAttempts
                };
            }
            
            // Calcular intentos restantes (3 intentos máximos)
            int remainingAttempts = 3 - (user.FailedLoginAttempts + 1);
            
            return new LoginResponse
            {
                Success = false,
                Message = remainingAttempts > 0 
                    ? $"Email o contraseña incorrectos. Intentos restantes: {remainingAttempts}" 
                    : "Email o contraseña incorrectos",
                FailedAttempts = user.FailedLoginAttempts + 1
            };
        }
    }

    public async Task<UserDto?> GetUserByIdAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        return user != null ? MapToDto(user) : null;
    }

    public async Task<UserDto?> GetUserByEmailAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        return user != null ? MapToDto(user) : null;
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _userRepository.EmailExistsAsync(email);
    }

    public async Task<bool> CedulaExistsAsync(string cedula)
    {
        return await _userRepository.CedulaExistsAsync(cedula);
    }    public async Task<ForgotPasswordResponse> ForgotPasswordAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        
        if (user == null)
        {
            return new ForgotPasswordResponse
            {
                Success = false,
                Message = "El correo electrónico no está registrado en nuestro sistema."
            };
        }
        
        if (!user.IsActive)
        {
            return new ForgotPasswordResponse
            {
                Success = false,
                Message = "La cuenta está desactivada. Contacta al administrador."
            };
        }

        try
        {
            // Generar token seguro
            var token = GenerateSecureToken();
            
            // Crear el registro del token
            var resetToken = new PasswordResetToken
            {
                UserId = user.Id,
                Token = token,
                ExpiryDate = DateTime.UtcNow.AddHours(1), // Token válido por 1 hora
                IsUsed = false
            };

            await _passwordResetTokenRepository.AddAsync(resetToken);

            // Enviar email con el token
            var emailSent = await _emailService.SendPasswordResetEmailAsync(
                user.Email, 
                token, 
                user.FullName
            );

            if (!emailSent)
            {
                // Si no se pudo enviar el email, eliminar el token
                await _passwordResetTokenRepository.DeleteAsync(resetToken.Id);
            }

            return new ForgotPasswordResponse
            {
                Success = true,
                Message = "Si el email existe en nuestro sistema, recibirás un enlace de recuperación."
            };
        }
        catch (Exception)
        {
            return new ForgotPasswordResponse
            {
                Success = true,
                Message = "Si el email existe en nuestro sistema, recibirás un enlace de recuperación."
            };
        }
    }

    public async Task<ResetPasswordResponse> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
    {
        try
        {
            // Buscar token válido
            var resetToken = await _passwordResetTokenRepository
                .GetValidTokenAsync(resetPasswordDto.Token, resetPasswordDto.Email);

            if (resetToken == null)
            {
                return new ResetPasswordResponse
                {
                    Success = false,
                    Message = "El enlace de recuperación no es válido o ha expirado."
                };
            }

            // Obtener usuario
            var user = resetToken.User;
            if (user == null || !user.IsActive)
            {
                return new ResetPasswordResponse
                {
                    Success = false,
                    Message = "Usuario no encontrado o inactivo."
                };
            }            // Actualizar contraseña
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(resetPasswordDto.NewPassword);
            
            // Desbloquear la cuenta automáticamente al restablecer la contraseña
            await UnlockUserAccountAsync(user);

            // Marcar token como usado
            resetToken.IsUsed = true;
            await _passwordResetTokenRepository.UpdateAsync(resetToken);

            // Limpiar tokens expirados (tarea de mantenimiento)
            await _passwordResetTokenRepository.CleanupExpiredTokensAsync();

            return new ResetPasswordResponse
            {
                Success = true,
                Message = "Tu contraseña ha sido actualizada exitosamente."
            };
        }
        catch (Exception)
        {
            return new ResetPasswordResponse
            {
                Success = false,
                Message = "Ocurrió un error al procesar tu solicitud. Intenta nuevamente."
            };
        }
    }

    private static string GenerateSecureToken()
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[32];
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
    }    private static UserDto MapToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            UserType = user.UserType,
            Cedula = user.Cedula,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            FullName = user.FullName,
            IdentityDocumentPath = user.IdentityDocumentPath,
            Nivel = user.Nivel // Mapeo de nivel
        };
    }    private async Task IncrementFailedLoginAttemptsAsync(User user)
    {
        user.FailedLoginAttempts++;
        user.LastFailedLogin = DateTime.UtcNow;
        
        // Si alcanza 3 intentos fallidos, bloquear la cuenta al cuarto intento
        if (user.FailedLoginAttempts >= 3)
        {
            user.IsLocked = true;
            user.LockoutEnd = DateTime.UtcNow.AddHours(24); // Bloquear por 24 horas
        }
        
        await _userRepository.UpdateAsync(user);
    }
    
    private async Task ResetFailedLoginAttemptsAsync(User user)
    {
        user.FailedLoginAttempts = 0;
        user.LastFailedLogin = null;
        user.IsLocked = false;
        user.LockoutEnd = null;
        
        await _userRepository.UpdateAsync(user);
    }
    
    private async Task UnlockUserAccountAsync(User user)
    {
        user.IsLocked = false;
        user.LockoutEnd = null;
        user.FailedLoginAttempts = 0;
        user.LastFailedLogin = null;
        
        await _userRepository.UpdateAsync(user);
    }
}
