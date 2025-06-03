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
    public UserType UserType { get; set; }

    [Required]
    [MaxLength(10)]
    public string Cedula { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public string FullName => $"{FirstName} {LastName}";
}