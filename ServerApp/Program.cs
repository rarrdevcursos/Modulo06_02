using ServerApp.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();

var app = builder.Build();

// CORS abierto para permitir el acceso desde el cliente Blazor en desarrollo.
app.UseCors(policy =>
    policy.AllowAnyOrigin()
          .AllowAnyMethod()
          .AllowAnyHeader());

app.MapGet("/api/productlist", () =>
{
    return new[]
    {
        new Producto(1, "Portátil", 1200.50m, 25, new Categoria(101, "Electrónica")),
        new Producto(2, "Auriculares", 50.00m, 100, new Categoria(102, "Accesorios"))
    };
});

app.Run();

