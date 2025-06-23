using ProyectoAgiles.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace ProyectoAgiles.Application.DTOs;

public class RegisterDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public UserType UserType { get; set; }
    public string Cedula { get; set; } = string.Empty;

    // Archivo de documento de identidad
    public byte[]? IdentityDocument { get; set; }
    public string? IdentityDocumentFileName { get; set; }
    public string? IdentityDocumentContentType { get; set; }
}

public class LoginDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class UserDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public UserType UserType { get; set; }    public string Cedula { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string UserTypeText => UserType.ToString();
    
    // Ruta del documento de identidad almacenado
    public string? IdentityDocumentPath { get; set; }
    public string? Nivel { get; set; } // Agregado para exponer el nivel
}

public class ForgotPasswordDto
{
    public string Email { get; set; } = string.Empty;
}

public class ResetPasswordDto
{
    public string Token { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class ForgotPasswordResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class ResetPasswordResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class LoginResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public UserDto? User { get; set; }
    public string Token { get; set; } = string.Empty;
    public bool IsAccountLocked { get; set; } = false;
    public int FailedAttempts { get; set; } = 0;
}
