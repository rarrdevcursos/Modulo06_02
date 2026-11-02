var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("ClientApp", policy =>
        policy.WithOrigins("http://localhost:5267", "https://localhost:7262")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

app.UseCors("ClientApp");

app.MapGet("/api/products", () =>

{

    return new[]

    {

new { Id = 1, Nombre = "Portátil", Precio = 1200.50, Stock = 25 },

new { Id = 2, Nombre = "Auriculares", Precio = 50.00, Stock = 100 }

};

});

app.Run();