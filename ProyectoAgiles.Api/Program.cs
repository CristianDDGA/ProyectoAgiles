using Microsoft.EntityFrameworkCore;
using ProyectoAgiles.Application.Interfaces;
using ProyectoAgiles.Application.Services;
using ProyectoAgiles.Application.Mappings;
using ProyectoAgiles.Domain.Interfaces;
using ProyectoAgiles.Infrastructure.Data;
using ProyectoAgiles.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Configuración de Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configuración de AutoMapper
builder.Services.AddAutoMapper(typeof(EvaluacionDesempenoMappingProfile), typeof(DiticMappingProfile), typeof(SolicitudEscalafonMappingProfile));

// Inyección de dependencias
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();
builder.Services.AddScoped<IExternalTeacherRepository, ExternalTeacherRepository>();
builder.Services.AddScoped<ITTHHRepository, TTHHRepository>();
builder.Services.AddScoped<IInvestigacionRepository, InvestigacionRepository>();
builder.Services.AddScoped<IEvaluacionDesempenoRepository, EvaluacionDesempenoRepository>();
builder.Services.AddScoped<IDiticRepository, DiticRepository>();
builder.Services.AddScoped<ISolicitudEscalafonRepository, SolicitudEscalafonRepository>();

// Servicio para manejo de archivos
builder.Services.AddScoped<IFileService, FileService>();

// Usar EmailService real para envío de correos
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITeacherManagementService, TeacherManagementService>();
builder.Services.AddScoped<IInvestigacionService, InvestigacionService>();
builder.Services.AddScoped<IEvaluacionDesempenoService, EvaluacionDesempenoService>();
builder.Services.AddScoped<IDiticService, DiticService>();
builder.Services.AddScoped<ISolicitudEscalafonService, SolicitudEscalafonService>();

// Configuración de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorApp", policy =>
    {
        policy.WithOrigins(
                "https://localhost:7042", 
                "http://localhost:5042", 
                "http://localhost:5041", 
                "https://localhost:5041",
                "http://localhost:5043",
                "https://localhost:5043"
            )
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configurar el pipeline de HTTP
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseCors("AllowBlazorApp");

app.UseRouting();
app.UseAuthorization();
app.MapControllers();

// Aplicar migraciones automáticamente en desarrollo
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        // Log error if needed
        Console.WriteLine($"Error aplicando migraciones: {ex.Message}");
    }
}

app.Run();
