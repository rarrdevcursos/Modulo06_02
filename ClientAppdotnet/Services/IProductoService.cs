using ClientAppdotnet.Models;

namespace ClientAppdotnet.Services;

public interface IProductoService
{
    Task<IReadOnlyList<Producto>> ObtenerProductosAsync(CancellationToken cancellationToken = default);
}
