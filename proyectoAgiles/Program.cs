using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using proyectoAgiles;
using proyectoAgiles.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => 
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var apiBaseUrl = configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5200";
    return new HttpClient { BaseAddress = new Uri(apiBaseUrl) };
});

// Configuración de API y servicios
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UserSessionService>();

// Leer la configuración del API desde appsettings.json
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

await builder.Build().RunAsync();
