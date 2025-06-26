using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ProyectoAgiles.Application.Interfaces;
using ProyectoAgiles.Application.Services;
using ProyectoAgiles.Application.Mappings;
using ProyectoAgiles.Domain.Interfaces;
using ProyectoAgiles.Infrastructure.Data;
using ProyectoAgiles.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configuración de Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ProyectoAgiles API",
        Version = "v1",
        Description = "API para el Sistema de Gestión de Escalafón Docente - Universidad Técnica de Ambato",
        Contact = new OpenApiContact
        {
            Name = "Equipo de Desarrollo",
            Email = "desarrollo@uta.edu.ec"
        }
    });

    // Configuración para incluir comentarios XML (opcional)
    // var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    // var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    // c.IncludeXmlComments(xmlPath);

    // Configuración de seguridad (si necesitas autenticación)
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando el esquema Bearer. Ejemplo: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

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
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ProyectoAgiles API v1");
        c.RoutePrefix = "swagger"; // Para acceder en /swagger
        c.DocumentTitle = "ProyectoAgiles API - Documentación";
        c.DefaultModelsExpandDepth(-1); // Ocultar modelos por defecto
        c.DisplayRequestDuration();
        c.EnableFilter();
        c.EnableDeepLinking();
    });
    app.UseDeveloperExceptionPage();
}

// También habilitar Swagger en producción (opcional)
if (app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ProyectoAgiles API v1");
        c.RoutePrefix = "api-docs"; // Para acceder en /api-docs en producción
    });
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
