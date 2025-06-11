using Microsoft.EntityFrameworkCore;
using ProyectoAgiles.Domain.Entities;

namespace ProyectoAgiles.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }    public DbSet<User> Users { get; set; }
    public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }
    public DbSet<ExternalTeacher> ExternalTeachers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuración de la entidad User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(100);
            
            entity.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(100);
            
            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(255);
            
            entity.HasIndex(e => e.Email)
                .IsUnique();
            
            entity.Property(e => e.PasswordHash)
                .IsRequired()
                .HasMaxLength(255);
              entity.Property(e => e.UserType)
                .IsRequired()
                .HasConversion<int>();
            
            entity.Property(e => e.Cedula)
                .IsRequired()
                .HasMaxLength(10);
            
            entity.Property(e => e.CreatedAt)
                .IsRequired();
            
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true);
            
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false);

            // Filtro global para soft delete
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Configuración de la entidad PasswordResetToken
        modelBuilder.Entity<PasswordResetToken>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Token)
                .IsRequired()
                .HasMaxLength(255);
            
            entity.Property(e => e.ExpiryDate)
                .IsRequired();
            
            entity.Property(e => e.CreatedAt)
                .IsRequired();
            
            entity.Property(e => e.IsUsed)
                .HasDefaultValue(false);
            
            // Configurar relación con User
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);        });

        // Configuración de la entidad ExternalTeacher
        modelBuilder.Entity<ExternalTeacher>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Cedula)
                .IsRequired()
                .HasMaxLength(10);
            
            entity.HasIndex(e => e.Cedula)
                .IsUnique();
            
            entity.Property(e => e.Universidad)
                .IsRequired()
                .HasMaxLength(200);
            
            entity.Property(e => e.NombresCompletos)
                .IsRequired()
                .HasMaxLength(200);
            
            entity.Property(e => e.CreatedAt)
                .IsRequired();
            
            entity.Property(e => e.UpdatedAt)
                .IsRequired();
        });

        // Datos semilla para el administrador por defecto
        SeedData(modelBuilder);
    }    private void SeedData(ModelBuilder modelBuilder)
    {        // Crear un usuario administrador por defecto
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                FirstName = "Admin",
                LastName = "Sistema",
                Email = "admin@sistema.com",
                PasswordHash = "$2a$11$xQVm8QpRGyV.rqm/JJt7p.3J7N6pC0qFb0q5RHbj6h8D2Cz0C.L9i", // Contraseña: Admin123!
                UserType = Domain.Enums.UserType.Admin,
                Cedula = "0000000000", // Cédula por defecto para admin
                IsActive = true,
                CreatedAt = new DateTime(2025, 6, 3, 0, 0, 0, DateTimeKind.Utc)
            }
        );        // Datos de ejemplo para docentes externos
        modelBuilder.Entity<ExternalTeacher>().HasData(
            new ExternalTeacher
            {
                Id = 1,
                Cedula = "1750000001",
                Universidad = "Universidad Técnica de Ambato",
                NombresCompletos = "María Elena García Pérez",
                CreatedAt = new DateTime(2025, 6, 11, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2025, 6, 11, 0, 0, 0, DateTimeKind.Utc)
            },
            new ExternalTeacher
            {
                Id = 2,
                Cedula = "1750000002",
                Universidad = "Universidad Técnica de Ambato",
                NombresCompletos = "Carlos Alberto Mendoza Silva",
                CreatedAt = new DateTime(2025, 6, 11, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2025, 6, 11, 0, 0, 0, DateTimeKind.Utc)
            },
            new ExternalTeacher
            {
                Id = 3,
                Cedula = "1750000003",
                Universidad = "Universidad Técnica de Ambato",
                NombresCompletos = "Ana Cristina López Vargas",
                CreatedAt = new DateTime(2025, 6, 11, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2025, 6, 11, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}
