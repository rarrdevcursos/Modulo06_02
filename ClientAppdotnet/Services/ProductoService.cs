using System.Diagnostics;
using System.Text.Json;
using ClientAppdotnet.Models;

namespace ClientAppdotnet.Services;

public sealed class ProductoService : IProductoService
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    private readonly HttpClient _http;
    private readonly ILogger<ProductoService> _logger;
    private readonly TimeSpan _tiempoCache;
    private readonly SemaphoreSlim _candado = new(1, 1);

    private IReadOnlyList<Producto>? _cache;
    private DateTimeOffset _cacheExpira = DateTimeOffset.MinValue;

    public ProductoService(
        HttpClient http,
        ILogger<ProductoService> logger,
        TimeSpan? tiempoCache = null)
    {
        _http = http;
        _logger = logger;
        _tiempoCache = tiempoCache ?? TimeSpan.FromSeconds(60);
    }

    public async Task<IReadOnlyList<Producto>> ObtenerProductosAsync(
        bool forzarRecarga = false,
        CancellationToken cancellationToken = default)
    {
        if (!forzarRecarga && _cache is not null && DateTimeOffset.UtcNow < _cacheExpira)
        {
            _logger.LogDebug("Catálogo de productos servido desde la caché en memoria.");
            return _cache;
        }

        await _candado.WaitAsync(cancellationToken);
        try
        {
            // Otra carga pudo completar la caché mientras se esperaba el candado.
            if (!forzarRecarga && _cache is not null && DateTimeOffset.UtcNow < _cacheExpira)
            {
                return _cache;
            }

            var productos = await DescargarProductosAsync(cancellationToken);

            _cache = productos;
            _cacheExpira = DateTimeOffset.UtcNow.Add(_tiempoCache);

            return productos;
        }
        finally
        {
            _candado.Release();
        }
    }

    private async Task<IReadOnlyList<Producto>> DescargarProductosAsync(CancellationToken cancellationToken)
    {
        var cronometro = Stopwatch.StartNew();

        try
        {
            using var response = await _http.GetAsync("api/productlist", cancellationToken);

            // Lanza HttpRequestException si el estado no es 2xx.
            response.EnsureSuccessStatusCode();

            // Deserialización por secuencia: evita materializar el JSON completo
            // en una cadena intermedia y reduce las asignaciones.
            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

            // Deserializar de forma explícita permite detectar JSON malformado
            // y respuestas con una estructura distinta a la esperada.
            var productos = await JsonSerializer.DeserializeAsync<List<Producto>>(
                stream, SerializerOptions, cancellationToken);

            cronometro.Stop();
            _logger.LogInformation(
                "Catálogo obtenido del API en {Milisegundos} ms ({Bytes} bytes).",
                cronometro.ElapsedMilliseconds,
                response.Content.Headers.ContentLength?.ToString() ?? "sin Content-Length");

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
