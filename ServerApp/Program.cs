using System.IO.Compression;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.AspNetCore.ResponseCompression;
using ServerApp.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();

// Rendimiento: comprime las respuestas para reducir el peso del JSON por la red.
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(["application/json"]);
});

builder.Services.Configure<BrotliCompressionProviderOptions>(
    options => options.Level = CompressionLevel.Fastest);
builder.Services.Configure<GzipCompressionProviderOptions>(
    options => options.Level = CompressionLevel.Fastest);

// Rendimiento: reutiliza la respuesta del catálogo durante 30 segundos
// en lugar de volver a construirla y serializarla en cada petición.
builder.Services.AddOutputCache(options =>
{
    options.AddPolicy("catalogo", policy => policy.Expire(TimeSpan.FromSeconds(30)));
});

var app = builder.Build();

app.UseResponseCompression();

// CORS abierto para permitir el acceso desde el cliente Blazor en desarrollo.
app.UseCors(policy =>
    policy.AllowAnyOrigin()
          .AllowAnyMethod()
          .AllowAnyHeader());

app.UseOutputCache();

app.MapGet("/api/productlist", () =>
{
    return TypedResults.Ok(new[]
    {
        new Producto(1, "Portátil", 1200.50m, 25, new Categoria(101, "Electrónica")),
        new Producto(2, "Auriculares", 50.00m, 100, new Categoria(102, "Accesorios"))
    });
})
.CacheOutput("catalogo");

app.Run();
