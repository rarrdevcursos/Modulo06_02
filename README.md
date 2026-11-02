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

| Método | Ruta            | Descripción                    |
|--------|-----------------|--------------------------------|
| GET    | `/api/products` | Lista de productos de ejemplo  |

Respuesta:

```json
[
  { "id": 1, "nombre": "Portátil", "precio": 1200.5, "stock": 25 },
  { "id": 2, "nombre": "Auriculares", "precio": 50, "stock": 100 }
]
```

## Funcionalidades

- Catálogo de productos consumido desde la API con `HttpClient`.
- Manejo de errores: códigos HTTP no exitosos, timeout, JSON inválido y API caída.
- Interfaz responsiva con MudBlazor y tema claro/oscuro.
- Páginas de inicio, contador, clima y 404.
- CORS configurado en el servidor para los orígenes del cliente.

## Configuración

La URL del API se define en `ClientAppdotnet/wwwroot/appsettings.json`:

```json
{
  "Api": {
    "BaseUrl": "http://localhost:5039/"
  }
}
```

Si cambias los puertos, actualiza también la política CORS en `ServerApp/Program.cs`.
