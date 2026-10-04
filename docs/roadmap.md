# Roadmap

Laboratorios pequeños, independientes y ejecutables. Se presupone experiencia con programación, REST, DI, SQL y arquitectura. Las comparaciones con Java 8/Spring Boot sirven como referencia para las particularidades de .NET moderno.

| Laboratorio | Foco en C#/.NET moderno | Resultado ejecutable | Referencia Java/Spring Boot |
| --- | --- | --- | --- |
| **Lab00-HelloApi** | Proyecto SDK, top-level statements, Minimal APIs, JSON y OpenAPI | `GET /api/hola` | Arranque de Spring Boot y mapeo HTTP |
| **Lab01-Controllers** | `ControllerBase`, atributos, `ActionResult`, registro y mapeo | El mismo `GET /api/hola` mediante un controller | `@RestController`, `@RequestMapping`, `@GetMapping`, `ResponseEntity` |
| **Lab02-DependencyInjection** | `IServiceCollection`, registro scoped, constructor injection y lifetimes | `GET /api/hola` con `IHolaService` y `HolaService` | `@Service`, constructor injection, `ApplicationContext` y scopes de beans |
| Lab03-Configuration | Providers, precedencia, entornos y patrón Options | Configuración tipada con override por entorno | `application.properties`, perfiles y `@ConfigurationProperties` |
| Lab04-Async | `Task<T>`, `async`/`await`, `CancellationToken` y `HttpClient` | Endpoint con I/O asíncrono y cancelación | Futures; diferencias con llamadas bloqueantes |
| Lab05-DataAccess | EF Core, LINQ, tracking y migraciones | Persistencia mínima con SQLite | JPA/Hibernate y consultas tipadas |
| Lab06-Validation | DTOs con records, nullable reference types y validación | Entrada válida/inválida con respuesta verificable | DTOs y Bean Validation; nullable aporta análisis estático |
| Lab07-ErrorHandling | Middleware, `IExceptionHandler` y `ProblemDetails` | Errores HTTP uniformes | `@ControllerAdvice` y exception handlers |
| Lab08-Testing | xUnit y `WebApplicationFactory` | Tests de integración HTTP | `@SpringBootTest` y herramientas de prueba HTTP |
| Lab09-RealApi | Integración de los conceptos anteriores | API pequeña de tareas con persistencia, validación y tests | Una aplicación Spring Boot de alcance equivalente |

**Estado:** Lab00, Lab01 y Lab02 están implementados. Los detalles de los próximos pasos se decidirán al abordar cada laboratorio.

Para cada incremento: leer el código → ejecutar → cambiar una cosa → verificar. Evitar capas o dependencias que no aporten al concepto del paso.
