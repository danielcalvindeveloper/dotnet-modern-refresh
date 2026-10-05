# C# y .NET moderno: laboratorio incremental

Actualización práctica para alguien con experiencia profesional en C#/.NET y Java 8/Spring Boot. El foco está en los cambios del lenguaje, del runtime y de ASP.NET Core: leer código, ejecutarlo y modificarlo en pasos pequeños.

## Base de esta iteración

- .NET 10, LTS actual. Entorno comprobado: SDK `10.0.401`, runtime ASP.NET Core `10.0.12`.
- Proyectos independientes y ejecutables con la CLI; cada laboratorio incorpora pocos conceptos.
- Comparaciones puntuales con Java/Spring Boot cuando ayudan a reconocer diferencias.

La elección de versión sigue la [política oficial de soporte de .NET](https://dotnet.microsoft.com/en-us/platform/support/policy).

## Empezar

```powershell
cd Lab00-HelloApi
dotnet run
```

Probar `http://localhost:5080/api/hola`. Ver las instrucciones y archivos clave en [Lab00](Lab00-HelloApi/README.md).

## Recorrido

El [roadmap](docs/roadmap.md) define los incrementos previstos. Están implementados [Lab00-HelloApi](Lab00-HelloApi/README.md), [Lab01-Controllers](Lab01-Controllers/README.md), [Lab02-DependencyInjection](Lab02-DependencyInjection/README.md), [Lab03-Configuration](Lab03-Configuration/README.md), [Lab04-Middleware](Lab04-Middleware/README.md), [Lab05-AsyncAwait](Lab05-AsyncAwait/README.md), [Lab06a-Dapper](Lab06a-Dapper/README.md), [Lab06b-EFCore](Lab06b-EFCore/README.md) y [Lab06c-EFCore-Relations-Migrations](Lab06c-EFCore-Relations-Migrations/README.md). Estos últimos forman el bloque **Lab06-DataAccess**; los siguientes laboratorios están pendientes.

Cada paso se completa leyendo y ejecutando el ejemplo, haciendo una pequeña variación y comprobando su comportamiento antes de avanzar.
