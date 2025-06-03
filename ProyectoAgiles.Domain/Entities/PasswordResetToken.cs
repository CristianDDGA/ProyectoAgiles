using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoAgiles.Domain.Entities;

public class PasswordResetToken : BaseEntity
{
    [Required]
    public int UserId { get; set; }

    [Required]
    [MaxLength(255)]
    public string Token { get; set; } = string.Empty;

    [Required]
    public DateTime ExpiryDate { get; set; }

    public bool IsUsed { get; set; } = false;

    // Navigation property
    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;
}
