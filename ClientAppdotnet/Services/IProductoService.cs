using ClientAppdotnet.Models;

namespace ClientAppdotnet.Services;

public interface IProductoService
{
    /// <param name="forzarRecarga">
    /// Omite la caché en memoria y vuelve a consultar el API.
    /// </param>
    Task<IReadOnlyList<Producto>> ObtenerProductosAsync(
        bool forzarRecarga = false,
        CancellationToken cancellationToken = default);
}
