# FullStack .NET 10 · Blazor WebAssembly + Minimal API

Solución de ejemplo full stack con un cliente **Blazor WebAssembly** y una **Minimal API** en .NET 10.

## Estructura

```
Modulo06_02/
├── ClientAppdotnet/        # Cliente Blazor WebAssembly (.NET 10 + MudBlazor 9.12)
│   ├── Layout/             # Layout principal y menú de navegación
│   ├── Models/             # Modelos compartidos del cliente
│   ├── Pages/              # Inicio, productos, contador, clima y 404
│   ├── Services/           # Servicio de productos y manejo de errores
│   ├── Theme/              # Tema claro/oscuro de MudBlazor
│   └── wwwroot/            # index.html, estilos, scripts y datos de ejemplo
├── ServerApp/              # Minimal API (.NET 10)
└── FullStackSolutiondotnet.slnx
```

## Requisitos

- [.NET SDK 10](https://dotnet.microsoft.com/download)

## Ejecución

Abrir dos terminales.

**Backend**

```powershell
cd ServerApp
dotnet run --launch-profile http
```

Disponible en `http://localhost:5039`.

**Frontend**

```powershell
cd ClientAppdotnet
dotnet run --launch-profile http
```

Disponible en `http://localhost:5267`.

> Usa el perfil `http` en el cliente. El API se sirve por HTTP y el navegador bloquea
> peticiones desde una página HTTPS por *mixed content*.

## Endpoints

| Método | Ruta              | Descripción                              |
|--------|-------------------|------------------------------------------|
| GET    | `/api/productlist` | Lista de productos de ejemplo (caché 30 s) |

Respuesta:

```json
[
  {
    "id": 1,
    "nombre": "Portátil",
    "precio": 1200.50,
    "stock": 25,
    "categoria": { "id": 101, "nombre": "Electrónica" }
  },
  {
    "id": 2,
    "nombre": "Auriculares",
    "precio": 50.00,
    "stock": 100,
    "categoria": { "id": 102, "nombre": "Accesorios" }
  }
]
```

## Funcionalidades

- Catálogo de productos consumido desde la API con `HttpClient`.
- Cada producto incluye una categoría anidada (`categoria`) expuesta como chip en la tabla.
- Manejo de errores: códigos HTTP no exitosos, timeout, JSON malformado, estructura inesperada y API caída.
- Interfaz responsiva con MudBlazor y tema claro/oscuro.
- Páginas de inicio, contador, clima y 404.
- CORS configurado en el servidor para permitir el cliente en desarrollo.

## Rendimiento

- **Compresión de respuestas** (Brotli/Gzip) en el API: el JSON del catálogo pasa de
  210 a 155 bytes con Brotli (≈ 26 % menos de transferencia).
- **Caché de salida** (`OutputCache`, 30 s) en `GET /api/productlist`: las peticiones
  repetidas se sirven desde memoria, sin volver a serializar (cabecera `Age`).
- **Caché en memoria del cliente** (60 s, configurable en `Api:CacheSegundos`): evita
  repetir la petición al navegar entre páginas; el botón «Actualizar» fuerza la recarga.
- **Deserialización por secuencia** (`JsonSerializer.DeserializeAsync`) en lugar de
  materializar el JSON en una cadena intermedia.
- **Un único `HttpClient`** con `Timeout` y `CancellationToken` en todas las llamadas.

El detalle del proceso, los problemas resueltos y las decisiones tomadas están en
[INFORME_COPILOT.md](INFORME_COPILOT.md).

## Configuración

La URL del API se define en `ClientAppdotnet/wwwroot/appsettings.json`:

```json
{
  "Api": {
    "BaseUrl": "http://localhost:5039/",
    "CacheSegundos": 60
  }
}
```

Si cambias los puertos, actualiza la URL en `appsettings.json`. `CacheSegundos` controla
cuánto tiempo el cliente reutiliza el catálogo antes de volver a consultar el API.
