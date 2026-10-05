# Lab07a — Validación de entrada HTTP

## Objetivo

Observar la validación automática de un `POST /api/reservas` con Controllers y `[ApiController]` en .NET 10. El service crea un objeto en memoria y devuelve **200** con JSON; el contador de Ids vive durante el proceso. No persiste reservas ni comprueba existencia de clientes: aquí se validan los datos de entrada.

## Data Annotations y campos obligatorios

Son atributos de `System.ComponentModel.DataAnnotations` que declaran reglas en el DTO; MVC los evalúa al validar el modelo.

| Propiedad | Tipo del DTO | Reglas |
| --- | --- | --- |
| ClienteId | `int?` | `[Required]` y `[Range(1, int.MaxValue)]` |
| Fecha | `DateTime?` | `[Required]` |
| Estado | `string?` | `[Required]` y `[StringLength(20, MinimumLength = 3)]` |

Al omitir un `int` o `DateTime` no nullable, el objeto puede quedar con `0` o `DateTime.MinValue`: `[Required]` no detecta esos valores como ausentes. Con `int?` y `DateTime?`, omisión o `null` deja `null` y `[Required]` falla. `ClienteId = 0` está presente, pero falla `[Range]`. Una fecha explícita igual a `DateTime.MinValue` satisface `[Required]`: no estamos imponiendo reglas de calendario.

`Estado` también es nullable para estudiar el atributo explícito, sin depender de la validación implícita de referencias no nullable. No tiene un valor inicial que oculte su ausencia. `[Required]` rechaza `null`, cadena vacía o solo espacios; `[StringLength]` comprueba la longitud, no un catálogo de estados permitidos.

## Binding, Validation y ModelState

Antes de la Action, MVC lee el body JSON con el input formatter y lo convierte al DTO (**binding**); después evalúa sus atributos (**validation**). JSON mal formado o `"clienteId":"abc"` impide leer/convertir correctamente; `"clienteId":0` se convierte bien, pero viola una regla.

`ModelState` contiene el estado del binding y de la validación, con errores agrupados por clave. `IsValid` indica si hay errores y `ErrorCount` cuenta esos errores. La Action imprime ambos para una entrada válida:

```text
>>> Controller ejecutado
ModelState: IsValid=True, ErrorCount=0
```

Con `[ApiController]`, el filtro incorporado comprueba `ModelState` antes de ejecutar la Action. Ante errores devuelve automáticamente **400** con `application/problem+json`, usando `ValidationProblemDetails` y un diccionario `errors`. Por eso un request inválido no imprime ninguna de esas dos líneas ni llama al service. Inspeccionar `errors` en las respuestas es la demostración del estado inválido; sus claves pueden ser propiedades como `ClienteId` o rutas JSON como `$.clienteId`.

No necesitamos escribir esto dentro de la Action:

```csharp
if (!ModelState.IsValid)
{
    return BadRequest(ModelState);
}
```

Los mensajes de binding los genera el framework; los mensajes de los atributos están en el DTO. Se conserva la respuesta estándar. `!` en la Action solo informa al compilador de nulabilidad: la comprobación en ejecución la hacen `[Required]` y `[ApiController]`.

## Cómo ejecutar

Desde la raíz:

```powershell
cd Lab07a-Validacion
dotnet run
```

Escucha en `http://localhost:5089`. DI conecta Controller → `IReservaService` → `ReservaService`. El singleton conserva únicamente el contador; `Interlocked.Increment` asigna Ids sin duplicarlos entre requests concurrentes. El service es síncrono porque no realiza I/O.

## Cómo probar

En otra terminal PowerShell, definir este helper. Enviar JSON por stdin evita problemas de comillas de argumentos nativos en Windows PowerShell; `-i` muestra status y headers:

```powershell
function Enviar-Reserva([string] $json) {
    $json | curl.exe -sS -i -H 'Content-Type: application/json' --data-binary '@-' http://localhost:5089/api/reservas
}

# Válido: HTTP 200; Action SÍ; ModelState válido.
Enviar-Reserva '{"clienteId":1,"fecha":"2026-10-05T10:00:00","estado":"Pendiente"}'

# ClienteId ausente: HTTP 400; Action NO; Required.
Enviar-Reserva '{"fecha":"2026-10-05T10:00:00","estado":"Pendiente"}'

# ClienteId = 0: HTTP 400; Action NO; Range.
Enviar-Reserva '{"clienteId":0,"fecha":"2026-10-05T10:00:00","estado":"Pendiente"}'

# Fecha ausente: HTTP 400; Action NO; Required.
Enviar-Reserva '{"clienteId":1,"estado":"Pendiente"}'

# Estado ausente: HTTP 400; Action NO; Required.
Enviar-Reserva '{"clienteId":1,"fecha":"2026-10-05T10:00:00"}'

# Estado de 21 caracteres: HTTP 400; Action NO; StringLength.
Enviar-Reserva '{"clienteId":1,"fecha":"2026-10-05T10:00:00","estado":"123456789012345678901"}'

# JSON sin cierre: HTTP 400; Action NO; error de lectura/binding.
Enviar-Reserva '{"clienteId":1,"fecha":"2026-10-05T10:00:00","estado":"Pendiente"'

# Tipo incompatible: HTTP 400; Action NO; error de conversión/binding.
Enviar-Reserva '{"clienteId":"abc","fecha":"2026-10-05T10:00:00","estado":"Pendiente"}'
```

También puede sustituirse cualquier campo obligatorio por `null`: devuelve 400 antes de la Action. Comparar el diccionario `errors` y la consola de la API entre los ejemplos.

## Archivos para mirar

- `Contracts/CrearReservaRequest.cs`: tipos nullable y atributos.
- `Controllers/ReservasController.cs`: `[ApiController]`, logs de ModelState y Action.
- `Program.cs`: registro de controllers y DI.
- `Services/ReservaService.cs` y `Models/Reserva.cs`: creación mínima en memoria.

## Documentación oficial de Microsoft

- [Validación y ModelState](https://learn.microsoft.com/en-us/aspnet/core/mvc/models/validation?view=aspnetcore-10.0)
- [ApiController y respuestas 400 automáticas](https://learn.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-10.0#automatic-http-400-responses)
- [Model binding e input formatters](https://learn.microsoft.com/en-us/aspnet/core/mvc/models/model-binding?view=aspnetcore-10.0#input-formatters)
- [RequiredAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.requiredattribute?view=net-10.0)
- [RangeAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.rangeattribute?view=net-10.0)
- [StringLengthAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.stringlengthattribute?view=net-10.0)
