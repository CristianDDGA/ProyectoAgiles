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

    public AuthService(
        IUserRepository userRepository,
        IEmailService emailService,
        IPasswordResetTokenRepository passwordResetTokenRepository)
    {
        _userRepository = userRepository;
        _emailService = emailService;
        _passwordResetTokenRepository = passwordResetTokenRepository;
    }

    public async Task<UserDto?> RegisterAsync(RegisterDto registerDto)
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

        // Crear nuevo usuario con rol de docente
        var user = new User
        {
            FirstName = registerDto.FirstName.Trim(),
            LastName = registerDto.LastName.Trim(),
            Email = registerDto.Email.Trim().ToLower(),
            PasswordHash = registerDto.Password, // Se hashea en el repositorio
            UserType = Domain.Enums.UserType.Docente, // Asignar automáticamente rol de docente
            Cedula = registerDto.Cedula.Trim(),
            IsActive = true
        };

        var createdUser = await _userRepository.AddAsync(user);
        return MapToDto(createdUser);
    }

    public async Task<UserDto?> LoginAsync(LoginDto loginDto)
    {
        var user = await _userRepository.ValidateUserAsync(loginDto.Email, loginDto.Password);
        return user != null ? MapToDto(user) : null;
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
        
        if (user == null || !user.IsActive)
        {
            // Por seguridad, siempre devolvemos el mismo mensaje
            return new ForgotPasswordResponse
            {
                Success = true,
                Message = "Si el email existe en nuestro sistema, recibirás un enlace de recuperación."
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
            }

            // Actualizar contraseña
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(resetPasswordDto.NewPassword);
            await _userRepository.UpdateAsync(user);

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
    }

    private static UserDto MapToDto(User user)
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
            FullName = user.FullName
        };
    }
}
