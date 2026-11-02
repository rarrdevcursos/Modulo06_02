using System.Net.Http.Json;
using System.Text.Json;
using ClientAppdotnet.Models;

namespace ClientAppdotnet.Services;

public sealed class ProductoService : IProductoService
{
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
            using var response = await _http.GetAsync("api/products", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "El API de productos respondió {StatusCode} ({ReasonPhrase}).",
                    (int)response.StatusCode,
                    response.ReasonPhrase);

                throw new ProductoServiceException(
                    $"El API de productos respondió con error {(int)response.StatusCode}.");
            }

            var productos = await response.Content.ReadFromJsonAsync<List<Producto>>(
                cancellationToken: cancellationToken);

            return productos ?? [];
        }
        catch (TaskCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("La solicitud al API de productos superó el tiempo de espera.");

            throw new ProductoServiceException(
                "El API de productos tardó demasiado en responder. Inténtalo de nuevo.");
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "La respuesta del API de productos no es JSON válido.");

            throw new ProductoServiceException(
                "La respuesta del API de productos no tiene el formato esperado.", ex);
        }
        catch (NotSupportedException ex)
        {
            _logger.LogError(ex, "La respuesta del API de productos no usa un tipo de contenido JSON.");

            throw new ProductoServiceException(
                "La respuesta del API de productos no tiene un formato JSON válido.", ex);
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
