# Informe reflexivo: uso de IA (Microsoft Copilot) en el proyecto

> **Nota de transparencia.** Este informe documenta el proceso real de desarrollo y las
> decisiones tomadas con ayuda de un asistente de IA. Ajusta el nombre de la herramienta
> y adjunta las capturas de pantalla de tu entorno (Visual Studio / Visual Studio Code)
> según el asistente que hayas utilizado finalmente. Los prompts que aparecen aquí son
> representativos del trabajo realizado, no transcripciones literales.

## 1. Contexto

Proyecto full stack con **Blazor WebAssembly** (`ClientAppdotnet`) y **Minimal API**
(`ServerApp`) en .NET 10. El objetivo era consumir `GET /api/productlist` desde el
cliente, mostrar los productos con una interfaz profesional y publicar todo en GitHub.

El asistente de IA se usó como apoyo en cuatro frentes: generar el código de integración,
depurar los errores que aparecieron al conectar cliente y servidor, definir las
estructuras JSON y optimizar el rendimiento.

## 2. Metodología de trabajo

En cada paso seguí el mismo ciclo:

1. **Describir el problema** con el contexto suficiente (framework, versión, error exacto).
2. **Pedir una propuesta** al asistente, no una solución cerrada.
3. **Revisar y validar** la propuesta compilando y ejecutando ambos proyectos.
4. **Decidir**: aceptar, ajustar o descartar.
5. **Registrar** el resultado en Git con un mensaje descriptivo.

Este ciclo evitó aceptar código a ciegas: todo lo que está en el repositorio se compiló y
se probó contra la API en ejecución.

## 3. Bitácora paso a paso

### 3.1 Repositorio en GitHub (5 pts)

**Prompt representativo:** «Tengo una solución .NET con dos proyectos; ayúdame a
inicializar Git, añadir un `.gitignore` para .NET y publicarla en un repositorio remoto».

**Qué aportó el asistente:** la secuencia de comandos (`git init`, `git add`, `commit`,
`remote add`, `push`) y un `.gitignore` específico para .NET que excluye `bin/`, `obj/`,
`.vs/` y archivos de usuario.

**Decisión y resultado:** acepté la propuesta tal cual. El repositorio quedó publicado en
`https://github.com/rarrdevcursos/Modulo06_02.git` con tres commits descriptivos y el
árbol de trabajo limpio.

### 3.2 Integración front-end / back-end (5 pts)

**Prompt representativo:** «Crea un servicio en Blazor WASM que consuma
`http://localhost:5039/api/productlist` y muestre los datos en una tabla con MudBlazor».

**Qué aportó el asistente:** la interfaz `IProductoService`, la implementación
`ProductoService`, los modelos espejo (`Producto`, `Categoria`), el registro del
`HttpClient` con `BaseAddress` y `Timeout`, y la tabla `MudTable` con estados de carga,
error y lista vacía.

**Decisión y resultado:** acepté la estructura por interfaz (facilita pruebas y cambios),
pero ajusté la presentación: migré de Bootstrap a **MudBlazor 9.12** y apliqué un tema
claro/oscuro propio. Verificado con compilación sin errores y respuesta HTTP 200 del
cliente.

### 3.3 Depuración de problemas de integración (5 pts)

Estos fueron los problemas reales y cómo se resolvieron:

#### a) Ruta del endpoint incorrecta

- **Síntoma:** el cliente llamaba a `/api/products`, pero la API exponía otra ruta.
- **Prompt:** «El cliente recibe 404 al llamar a la API; estas son la URL y el `Program.cs` del servidor».
- **Propuesta del asistente:** unificar el contrato en `/api/productlist` en servidor,
  servicio y documentación, en lugar de mantener dos rutas.
- **Resultado:** corregido en el commit `42ef9f1`; la llamada devuelve 200.

#### b) CORS bloqueado

- **Síntoma:** el navegador bloqueaba la petición por política CORS.
- **Prompt:** «La consola del navegador muestra un error de CORS al llamar al API desde el cliente Blazor».
- **Propuesta:** `builder.Services.AddCors()` y `app.UseCors(...)` con
  `AllowAnyOrigin/AnyMethod/AnyHeader` para desarrollo.
- **Decisión:** aceptado para desarrollo, dejando comentado en el código que en producción
  debe restringirse a orígenes concretos. Es una decisión consciente, no una copia ciega.

#### c) Contenido mixto (HTTPS → HTTP)

- **Síntoma:** el cliente se ejecutaba en HTTPS y el API en HTTP; el navegador bloqueaba
  la petición por *mixed content*.
- **Propuesta:** ejecutar ambos con el perfil `http` y documentarlo en el README.
- **Resultado:** documentado en la sección «Ejecución» del README.

#### d) JSON malformado o con estructura inesperada

- **Síntoma:** una respuesta no válida rompía la página sin mensaje claro.
- **Prompt:** «¿Cómo distingo un error HTTP, un timeout y un JSON inválido al consumir la API?».
- **Propuesta:** `EnsureSuccessStatusCode`, deserialización explícita con `JsonException`,
  captura de `TaskCanceledException` para timeouts y una excepción propia
  (`ProductoServiceException`) con mensajes para el usuario.
- **Decisión:** aceptada. Añadí registro con `ILogger` en cada rama para poder diagnosticar.

#### e) Puertos ocupados al recompilar

- **Síntoma:** la compilación fallaba porque una instancia anterior seguía usando el `.exe`.
- **Propuesta:** identificar el proceso que escucha en 5039/5267 y detenerlo antes de
  recompilar.
- **Resultado:** resuelto; el flujo de verificación quedó automatizado.

#### f) Texto con caracteres raros al validar (`Port�til`)

- **Diagnóstico:** no era un error del API, sino la codificación de la consola de PowerShell.
- **Decisión:** validar leyendo la respuesta como UTF-8 y comparando los valores, en lugar
  de fiarse de la representación por pantalla. El JSON era correcto.

### 3.4 Estructuras JSON para la comunicación (5 pts)

**Prompt representativo:** «Necesito que cada producto incluya un objeto anidado
`categoria` con `id` y `nombre`, y que el cliente lo muestre en la tabla».

**Qué aportó el asistente:** el `record` del servidor, la clase espejo en el cliente, la
configuración `JsonSerializerDefaults.Web` (camelCase) y el ejemplo de respuesta.

**Decisión y resultado:** aceptado con un ajuste: en el cliente usé
`JsonSerializer.DeserializeAsync` sobre la secuencia y mantuve el manejo explícito de
`JsonException`. Verificado contra la API en ejecución:

```json
[{"id":1,"nombre":"Portátil","precio":1200.50,"stock":25,
  "categoria":{"id":101,"nombre":"Electrónica"}}, ...]
```

La categoría se muestra como chip en `FetchProducts.razor` y el contrato quedó documentado
en el README.

### 3.5 Optimización del rendimiento (5 pts)

**Prompt representativo:** «¿Qué mejoras de rendimiento puedo aplicar a esta integración
sin cambiar el contrato del API?».

**Propuestas del asistente y decisiones:**

| Mejora | Decisión | Efecto |
|---|---|---|
| Compresión Brotli/Gzip | Aceptada | JSON de 210 → 155 bytes con Brotli (−26 %) |
| Caché de salida en el API (30 s) | Aceptada | Peticiones repetidas servidas desde memoria (`Age`) |
| Caché en memoria en el cliente (60 s) | Aceptada, configurable | Evita repetir la llamada al navegar |
| Deserialización por secuencia | Aceptada | Evita una cadena intermedia y asignaciones |
| Un único `HttpClient` + `CancellationToken` | Ya existía, se mantuvo | Menos conexiones y cancelación correcta |
| Subir el `Timeout` a 60 s para «evitar errores» | **Rechazada** | Oculta el problema; es mejor un error claro a los 10 s |

**Medición real** (API en ejecución, `curl`):

```
Accept-Encoding: identity  → 210 bytes
Accept-Encoding: gzip      → 163 bytes
Accept-Encoding: br        → 155 bytes
Segunda petición           → Age: 8 (servida desde la caché de salida)
```

Con dos productos el ahorro absoluto es pequeño; el objetivo era dejar el mecanismo
correcto para que el beneficio escale con el tamaño del catálogo.

## 4. Reflexión final

- **Lo que mejor funcionó:** dar contexto técnico preciso (framework, versión, error,
  archivo implicado) y pedir explicaciones, no solo código.
- **Lo que evité:** aceptar cambios sin compilar, desactivar validaciones para «que
  funcione» o subir timeouts para esconder fallos.
- **Lo que aprendí:** la IA acelera la generación y el diagnóstico, pero la verificación
  (compilar, ejecutar, medir) sigue siendo responsabilidad mía. Los tres errores más
  costosos (ruta, CORS y contenido mixto) se resolvieron entendiendo el motivo, no
  copiando la solución.
- **Qué haría distinto:** escribir pruebas automatizadas para el servicio y documentar
  cada prompt desde el primer momento, para que la evidencia del uso de la herramienta
  quedara registrada junto al código.

## 5. Evidencia

- Repositorio: `https://github.com/rarrdevcursos/Modulo06_02.git`
- Commits:
  - `bc40509` — solución inicial con Blazor WASM, MudBlazor y Minimal API.
  - `42ef9f1` — corrección de ruta, CORS y manejo de JSON malformado.
  - `a46dc82` — categoría anidada en API, cliente y README.
  - Optimizaciones de rendimiento: compresión, caché de salida y caché en cliente.
- Archivos clave: `ServerApp/Program.cs`, `ClientAppdotnet/Services/ProductoService.cs`,
  `ClientAppdotnet/Pages/FetchProducts.razor`, `ClientAppdotnet/Models/*.cs`, `README.md`.
