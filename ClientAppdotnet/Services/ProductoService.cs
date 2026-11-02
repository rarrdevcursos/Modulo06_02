using System.Text.Json;
using ClientAppdotnet.Models;

namespace ClientAppdotnet.Services;

public sealed class ProductoService : IProductoService
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    private readonly HttpClient _http;
    private readonly ILogger<ProductoService> _logger;

    public ProductoService(HttpClient http, ILogger<ProductoService> logger)
    {
        _http = http;
        _logger = logger;
    }

    public async Task<IReadOnlyList<Producto>> ObtenerProductosAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var response = await _http.GetAsync("api/productlist", cancellationToken);

            // Lanza HttpRequestException si el estado no es 2xx.
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(cancellationToken);

            // La deserialización explícita permite detectar JSON malformado
            // y respuestas con una estructura distinta a la esperada.
            var productos = JsonSerializer.Deserialize<List<Producto>>(json, SerializerOptions);

            return productos ?? [];
        }
        catch (HttpRequestException ex) when (ex.StatusCode is not null)
        {
            _logger.LogError(
                ex,
                "El API de productos respondió con error {StatusCode}.",
                (int)ex.StatusCode.Value);

            throw new ProductoServiceException(
                $"El API de productos respondió con error {(int)ex.StatusCode.Value}.", ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "La respuesta del API de productos no es un JSON válido o tiene otra estructura.");

            throw new ProductoServiceException(
                "La respuesta del API de productos no tiene el formato esperado.", ex);
        }
        catch (TaskCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("La solicitud al API de productos superó el tiempo de espera.");

            throw new ProductoServiceException(
                "El API de productos tardó demasiado en responder. Inténtalo de nuevo.");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "No se pudo conectar con el API de productos.");

            throw new ProductoServiceException(
                "No se pudo conectar con el API de productos. Verifica que el servidor esté en ejecución.", ex);
        }
    }
}

public sealed class ProductoServiceException : Exception
{
    public ProductoServiceException(string message) : base(message)
    {
    }

    public ProductoServiceException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
