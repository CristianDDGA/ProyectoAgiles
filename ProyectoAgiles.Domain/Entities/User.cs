using ProyectoAgiles.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace ProyectoAgiles.Domain.Entities;

public class User : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string PasswordHash { get; set; } = string.Empty;    [Required]
    public UserType UserType { get; set; }    [Required]
    [MaxLength(10)]
    public string Cedula { get; set; } = string.Empty;

    // Ruta del documento de identidad almacenado
    [MaxLength(500)]
    public string? IdentityDocumentPath { get; set; }

    public bool IsActive { get; set; } = true;

    // Campos para control de intentos fallidos
    public int FailedLoginAttempts { get; set; } = 0;
    public bool IsLocked { get; set; } = false;
    public DateTime? LockoutEnd { get; set; }
    public DateTime? LastFailedLogin { get; set; }

    public string FullName => $"{FirstName} {LastName}";
    public string? Nivel { get; set; } // Nivel del docente: titular auxiliar 1, titular auxiliar 2, etc.
}