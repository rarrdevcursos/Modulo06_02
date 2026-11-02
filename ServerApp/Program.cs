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
        new { Id = 1, Nombre = "Portátil", Precio = 1200.50, Stock = 25 },
        new { Id = 2, Nombre = "Auriculares", Precio = 50.00, Stock = 100 }
    };
});

app.Run();
