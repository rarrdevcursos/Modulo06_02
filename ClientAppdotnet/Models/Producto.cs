namespace ClientAppdotnet.Models;

public class Producto
{
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public double Precio { get; set; }

    public int Stock { get; set; }
}
