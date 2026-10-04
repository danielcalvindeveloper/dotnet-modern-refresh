# Lab01 — Controllers

## Objetivo

Implementar el mismo `GET /api/hola` de Lab00 sobre .NET 10, con idéntico JSON, mediante un controller.

## Conceptos nuevos

| Lab00 | Lab01 |
| --- | --- |
| `MapGet("/api/hola", lambda)` | `[Route("api/hola")]` + `[HttpGet]` + método `Get()` |
| `TypedResults.Ok(objeto)` | `Ok(objeto)` heredado de `ControllerBase` |
| Handler registrado en `Program.cs` | Controller descubierto y mapeado por ASP.NET Core |

- `ControllerBase`: base para APIs; aporta acceso al contexto HTTP y helpers como `Ok`, `NotFound` y `BadRequest`.
- `[ApiController]`: activa convenciones de API y exige routing por atributos.
- `[Route("api/hola")]`: define la ruta del controller. `[HttpGet]` selecciona GET para la acción; no agrega segmentos. El nombre `Get` no define la ruta.
- `ActionResult`: representa un resultado HTTP. Aquí `Ok(objeto)` crea un `OkObjectResult` con estado 200; el formatter serializa el objeto a JSON.
- `AddControllers()`: registra los componentes del framework para controllers. `MapControllers()`: incorpora sus acciones al routing usando los atributos.

## Comparación con Spring Boot

El controller cumple el papel de una clase `@RestController`; `[Route]` y `[HttpGet]` son comparables a `@RequestMapping` y `@GetMapping`. `Ok(objeto)` se parece a `ResponseEntity.ok(objeto)`. En ASP.NET Core el registro y el mapeo se ven explícitamente en `Program.cs`.

## Cómo ejecutar

Desde la raíz, con el SDK .NET 10:

```powershell
cd Lab01-Controllers
dotnet run
```

Escucha en `http://localhost:5081`; permite ejecutar Lab00 en el puerto 5080 al mismo tiempo. Detener con `Ctrl+C`.

## Cómo probar

En otra terminal:

```powershell
curl.exe -i http://localhost:5081/api/hola
curl.exe http://localhost:5081/openapi/v1.json
```

La API devuelve HTTP 200, `application/json` y:

```json
{"mensaje":"Hola desde ASP.NET Core"}
```

OpenAPI conserva el endpoint de documentación de Lab00, disponible en `Development`. Al usar `ActionResult` sin un tipo de cuerpo declarado, el contrato no infiere el esquema del objeto anónimo.

## Archivos importantes

- `Controllers/HolaController.cs`: atributos, acción y resultado HTTP; comparar con el `MapGet` de Lab00.
- `Program.cs`: `AddControllers()` antes de `Build()` y `MapControllers()` antes de `Run()`.
- `Lab01-Controllers.csproj`: mismo framework y paquete OpenAPI que Lab00.
- `Properties/launchSettings.json`: puerto 5081 y entorno local.

## Documentación oficial de Microsoft

- [APIs con controllers y ControllerBase](https://learn.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-10.0)
- [Routing por atributos](https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/routing?view=aspnetcore-10.0)
- [Tipos de retorno y resultados HTTP](https://learn.microsoft.com/en-us/aspnet/core/web-api/action-return-types?view=aspnetcore-10.0)
