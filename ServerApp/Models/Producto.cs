namespace ServerApp.Models;

public sealed record Categoria(int Id, string Nombre);

public sealed record Producto(
    int Id,
    string Nombre,
    decimal Precio,
    int Stock,
    Categoria Categoria);
