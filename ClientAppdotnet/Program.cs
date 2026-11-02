using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ClientAppdotnet;
using ClientAppdotnet.Services;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

var apiBaseUrl = builder.Configuration["Api:BaseUrl"] ?? "http://localhost:5039/";

// Tiempo durante el cual el catálogo se reutiliza desde la caché en memoria.
var cacheSegundos = int.TryParse(builder.Configuration["Api:CacheSegundos"], out var segundos) && segundos > 0
    ? segundos
    : 60;

builder.Services.AddSingleton<IProductoService>(sp =>
{
    // Un único HttpClient para toda la aplicación evita abrir conexiones de más.
    var httpClient = new HttpClient
    {
        BaseAddress = new Uri(apiBaseUrl, UriKind.Absolute),
        Timeout = TimeSpan.FromSeconds(10)
    };

    var logger = sp.GetRequiredService<ILogger<ProductoService>>();

    return new ProductoService(httpClient, logger, TimeSpan.FromSeconds(cacheSegundos));
});

await builder.Build().RunAsync();
