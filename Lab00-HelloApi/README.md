# Lab00 — Hello API

## Objetivo

Ejecutar una API ASP.NET Core sobre .NET 10 LTS con un único endpoint: `GET /api/hola` devuelve JSON.

## Qué conceptos modernos aparecen

Proyecto SDK, top-level statements, Minimal APIs, lambdas, resultado HTTP tipado, serialización con System.Text.Json y OpenAPI integrado mediante el paquete de Microsoft.

## Comparación con Spring Boot

`WebApplication.CreateBuilder(args)` prepara host, configuración, logging y DI; `app.Run()` inicia el servidor, con un papel comparable al arranque de Spring Boot. `MapGet` registra directamente ruta y handler; veremos controllers en Lab01.

## Cómo ejecutar

Desde la raíz, con el SDK .NET 10 instalado:

```powershell
cd Lab00-HelloApi
dotnet run
```

Escucha en `http://localhost:5080`. La primera ejecución restaura paquetes desde NuGet. Detener con `Ctrl+C`.

## Cómo probar

En otra terminal:

```powershell
curl.exe -i http://localhost:5080/api/hola
curl.exe http://localhost:5080/openapi/v1.json
```

El primer comando devuelve HTTP 200, contenido `application/json` y:

```json
{"mensaje":"Hola desde ASP.NET Core"}
```

El segundo devuelve el documento OpenAPI con `/api/hola`. Se publica solamente en `Development`; este laboratorio expone el JSON de OpenAPI sin agregar una UI.

## Qué archivos mirar

- `Lab00-HelloApi.csproj`: `Microsoft.NET.Sdk.Web` aporta el SDK web; `net10.0` fija el framework objetivo. `Nullable` activa análisis de referencias nulas e `ImplicitUsings` agrega imports habituales. `PackageReference` incorpora OpenAPI; el SDK incluye automáticamente los archivos `.cs`.
- `Program.cs`: instrucciones de nivel superior reemplazan el `Main` explícito. `CreateBuilder` prepara el host, `AddOpenApi` registra servicios, `Build` crea la aplicación, `MapOpenApi` publica el contrato y `MapGet` define el handler. `TypedResults.Ok` devuelve 200 y serializa el objeto a JSON. `Run` inicia la aplicación.
- `Properties/launchSettings.json`: perfil local utilizado por `dotnet run`; fija puerto y entorno.

## Documentación oficial de Microsoft

- [SDK y formato de proyecto](https://learn.microsoft.com/en-us/dotnet/core/project-sdk/overview)
- [Top-level statements](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/program-structure/top-level-statements)
- [Minimal APIs](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis?view=aspnetcore-10.0)
- [OpenAPI en ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/openapi/overview?view=aspnetcore-10.0)
