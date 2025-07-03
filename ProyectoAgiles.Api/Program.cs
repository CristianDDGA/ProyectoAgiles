using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ProyectoAgiles.Application.Interfaces;
using ProyectoAgiles.Application.Services;
using ProyectoAgiles.Application.Mappings;
using ProyectoAgiles.Domain.Interfaces;
using ProyectoAgiles.Infrastructure.Data;
using ProyectoAgiles.Infrastructure.Repositories;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configuración de Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "🎓 ProyectoAgiles API",
        Version = "v1.0.0",
        Description = "API REST para la gestión integral del escalafón docente de la Universidad Técnica de Ambato"
    });

    // Configuración para incluir comentarios XML
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    // Configuración avanzada de UI
    c.EnableAnnotations();
    c.DescribeAllParametersInCamelCase();
    c.CustomSchemaIds(type => type.FullName?.Replace("+", "."));

    // Configuración de seguridad JWT
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "🔐 JWT Authorization header usando el esquema Bearer. Ejemplo: 'Bearer {token}'"
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
                }
            },
            Array.Empty<string>()
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
builder.Services.AddScoped<IArchivosUtilizadosRepository, ArchivosUtilizadosRepository>();
builder.Services.AddScoped<IPeriodoPostulacionRepository, PeriodoPostulacionRepository>();

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
builder.Services.AddScoped<IArchivosUtilizadosService, ProyectoAgiles.Infrastructure.Services.ArchivosUtilizadosInfrastructureService>();
builder.Services.AddScoped<IPeriodoPostulacionService, PeriodoPostulacionService>();

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
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ProyectoAgiles API v1.0.0");
        c.RoutePrefix = "swagger";
        c.DocumentTitle = "🎓 ProyectoAgiles API - Sistema de Escalafón Docente";
        c.DefaultModelsExpandDepth(2);
        c.DefaultModelRendering(Swashbuckle.AspNetCore.SwaggerUI.ModelRendering.Model);
        c.DisplayRequestDuration();
        c.EnableFilter();
        c.EnableDeepLinking();
        c.EnableValidator();
        c.SupportedSubmitMethods(
            Swashbuckle.AspNetCore.SwaggerUI.SubmitMethod.Get,
            Swashbuckle.AspNetCore.SwaggerUI.SubmitMethod.Post,
            Swashbuckle.AspNetCore.SwaggerUI.SubmitMethod.Put,
            Swashbuckle.AspNetCore.SwaggerUI.SubmitMethod.Delete,
            Swashbuckle.AspNetCore.SwaggerUI.SubmitMethod.Patch
        );
        c.InjectStylesheet("/swagger-ui/custom.css");
        c.InjectJavascript("/swagger-ui/custom.js");
    });
    app.UseDeveloperExceptionPage();
}

// También habilitar Swagger en producción con seguridad adicional
if (app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ProyectoAgiles API v1.0.0");
        c.RoutePrefix = "api-docs";
        c.DocumentTitle = "🎓 ProyectoAgiles API - Documentación Oficial";
        c.SupportedSubmitMethods(); // Deshabilitar pruebas en producción
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
