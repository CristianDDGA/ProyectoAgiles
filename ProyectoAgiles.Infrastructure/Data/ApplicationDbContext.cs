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
    public DbSet<TTHH> TTHH { get; set; }
    public DbSet<Investigacion> Investigaciones { get; set; }
    public DbSet<EvaluacionDesempeno> DAC { get; set; }
    public DbSet<DITIC> DITIC { get; set; }
    public DbSet<SolicitudEscalafon> SolicitudesEscalafon { get; set; }

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
                .IsRequired();        });        // Configuración de la entidad Investigacion
        modelBuilder.Entity<Investigacion>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Cedula)
                .IsRequired();
            
            entity.Property(e => e.Titulo)
                .IsRequired();
            
            entity.Property(e => e.Tipo)
                .IsRequired();
            
            entity.Property(e => e.RevistaOEditorial)
                .IsRequired();
            
            entity.Property(e => e.FechaPublicacion)
                .IsRequired();
            
            entity.Property(e => e.CampoConocimiento)
                .IsRequired();
            
            entity.Property(e => e.Filiacion)
                .IsRequired();
            
            entity.Property(e => e.Observacion)
                .IsRequired();
            
            entity.Property(e => e.CreatedAt)
                .IsRequired();
            
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false);
        });        // Configuración de la entidad EvaluacionDesempeno
        modelBuilder.Entity<EvaluacionDesempeno>(entity =>
        {
            entity.ToTable("DAC"); // Nombre específico de la tabla
            
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Cedula)
                .IsRequired()
                .HasMaxLength(10);
            
            entity.Property(e => e.PeriodoAcademico)
                .IsRequired()
                .HasMaxLength(10);
            
            entity.Property(e => e.Anio)
                .IsRequired();
            
            entity.Property(e => e.Semestre)
                .IsRequired();
            
            entity.Property(e => e.PuntajeObtenido)
                .IsRequired()
                .HasColumnType("decimal(5,2)");
            
            entity.Property(e => e.PuntajeMaximo)
                .IsRequired()
                .HasColumnType("decimal(5,2)")
                .HasDefaultValue(100);
            
            entity.Property(e => e.FechaEvaluacion)
                .IsRequired();
            
            entity.Property(e => e.TipoEvaluacion)
                .HasMaxLength(50)
                .HasDefaultValue("Integral");
            
            entity.Property(e => e.Observaciones)
                .HasMaxLength(500);
            
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .HasDefaultValue("Completada");
            
            entity.Property(e => e.Evaluador)
                .HasMaxLength(100);
            
            entity.Property(e => e.NombreArchivoRespaldo)
                .HasMaxLength(255);
            
            entity.Property(e => e.CreatedAt)
                .IsRequired();
            
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false);

            // Índice único compuesto para evitar duplicados de evaluación por período
            entity.HasIndex(e => new { e.Cedula, e.PeriodoAcademico })
                .IsUnique();

            // Filtro global para soft delete
            entity.HasQueryFilter(e => !e.IsDeleted);
        });        // Configuración de la entidad DITIC
        modelBuilder.Entity<DITIC>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Cedula)
                .IsRequired()
                .HasMaxLength(10);
            
            entity.Property(e => e.NombreCapacitacion)
                .IsRequired()
                .HasMaxLength(500);
            
            entity.Property(e => e.Institucion)
                .IsRequired()
                .HasMaxLength(300);
            
            entity.Property(e => e.TipoCapacitacion)
                .IsRequired()
                .HasMaxLength(50);
            
            entity.Property(e => e.Modalidad)
                .HasMaxLength(30)
                .HasDefaultValue("Presencial");
            
            entity.Property(e => e.HorasAcademicas)
                .IsRequired();
            
            entity.Property(e => e.FechaInicio)
                .IsRequired();
            
            entity.Property(e => e.FechaFin)
                .IsRequired();
            
            entity.Property(e => e.Anio)
                .IsRequired();
            
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .HasDefaultValue("Completada");
            
            entity.Property(e => e.Calificacion)
                .HasColumnType("decimal(5,2)");
            
            entity.Property(e => e.CalificacionMinima)
                .HasColumnType("decimal(5,2)")
                .HasDefaultValue(70);
            
            entity.Property(e => e.Descripcion)
                .HasMaxLength(1000);
            
            entity.Property(e => e.NumeroCertificado)
                .HasMaxLength(100);
            
            entity.Property(e => e.Instructor)
                .HasMaxLength(200);
            
            entity.Property(e => e.Observaciones)
                .HasMaxLength(500);
            
            entity.Property(e => e.NombreArchivoCertificado)
                .HasMaxLength(255);
            
            entity.Property(e => e.ExencionPorAutoridad)
                .HasDefaultValue(false);
            
            entity.Property(e => e.CargoAutoridad)
                .HasMaxLength(100);
            
            entity.Property(e => e.CreatedAt)
                .IsRequired();
            
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false);

            // Índice para búsquedas por cédula
            entity.HasIndex(e => e.Cedula);
            
            // Índice para búsquedas por año
            entity.HasIndex(e => e.Anio);
            
            // Índice compuesto para verificación de duplicados
            entity.HasIndex(e => new { e.Cedula, e.NombreCapacitacion, e.Institucion, e.FechaInicio });

            // Filtro global para soft delete
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Configuración de la entidad SolicitudEscalafon
        modelBuilder.Entity<SolicitudEscalafon>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.DocenteCedula)
                .IsRequired()
                .HasMaxLength(10);
            
            entity.Property(e => e.DocenteNombre)
                .IsRequired()
                .HasMaxLength(200);
            
            entity.Property(e => e.DocenteEmail)
                .IsRequired()
                .HasMaxLength(255);
            
            entity.Property(e => e.DocenteTelefono)
                .HasMaxLength(20);
            
            entity.Property(e => e.Facultad)
                .HasMaxLength(200);
            
            entity.Property(e => e.Carrera)
                .HasMaxLength(200);
            
            entity.Property(e => e.NivelActual)
                .IsRequired()
                .HasMaxLength(100);
            
            entity.Property(e => e.NivelSolicitado)
                .IsRequired()
                .HasMaxLength(100);
            
            entity.Property(e => e.Titulos)
                .HasMaxLength(1000);
            
            entity.Property(e => e.Publicaciones)
                .HasMaxLength(1000);
            
            entity.Property(e => e.ProyectosInvestigacion)
                .HasMaxLength(1000);
            
            entity.Property(e => e.Capacitaciones)
                .HasMaxLength(1000);
            
            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(20)
                .HasDefaultValue("Pendiente");
            
            entity.Property(e => e.Observaciones)
                .HasMaxLength(1000);
            
            entity.Property(e => e.MotivoRechazo)
                .HasMaxLength(1000);
            
            entity.Property(e => e.ProcesadoPor)
                .HasMaxLength(100);
            
            entity.Property(e => e.FechaSolicitud)
                .IsRequired();
            
            entity.Property(e => e.CreatedAt)
                .IsRequired();
            
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false);

            // Índice para búsquedas por cédula
            entity.HasIndex(e => e.DocenteCedula);
            
            // Índice para búsquedas por estado
            entity.HasIndex(e => e.Status);

            // Filtro global para soft delete
            entity.HasQueryFilter(e => !e.IsDeleted);
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
