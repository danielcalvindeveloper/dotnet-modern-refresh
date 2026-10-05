# Roadmap

Laboratorios pequeños, independientes y ejecutables. Se presupone experiencia con programación, REST, DI, SQL y arquitectura. Las comparaciones con Java 8/Spring Boot sirven como referencia para las particularidades de .NET moderno.

| Laboratorio | Foco en C#/.NET moderno | Resultado ejecutable | Referencia Java/Spring Boot |
| --- | --- | --- | --- |
| **Lab00-HelloApi** | Proyecto SDK, top-level statements, Minimal APIs, JSON y OpenAPI | `GET /api/hola` | Arranque de Spring Boot y mapeo HTTP |
| **Lab01-Controllers** | `ControllerBase`, atributos, `ActionResult`, registro y mapeo | El mismo `GET /api/hola` mediante un controller | `@RestController`, `@RequestMapping`, `@GetMapping`, `ResponseEntity` |
| **Lab02-DependencyInjection** | `IServiceCollection`, registro scoped, constructor injection y lifetimes | `GET /api/hola` con `IHolaService` y `HolaService` | `@Service`, constructor injection, `ApplicationContext` y scopes de beans |
| **Lab03-Configuration** | `IConfiguration`, precedencia, entornos, binding e `IOptions<T>` | `GET /api/hola` con configuración tipada y sobrescritura en Development | `application.properties`/`application.yml`, profiles, `@Value` y `@ConfigurationProperties` |
| **Lab04-Middleware** | Pipeline HTTP, middleware inline, `HttpContext`, `next`, Use/Run/Map | `GET /api/hola` con logs antes y después del controller | Servlet Filter, `FilterChain` e interceptors |
| **Lab05-AsyncAwait** | `Task`, `Task<T>`, `async`/`await` y propagación entre capas | `GET /api/hola` con espera I/O simulada y controller asíncrono | `CompletableFuture` y modelo tradicional thread-per-request |
| **Lab06-DataAccess / Lab06a-Dapper** | Repository, conexión SQLite, SQL parametrizado, mapping y métodos async de Dapper | `GET /api/clientes` y `GET /api/clientes/{id}` con base local inicializada | JdbcTemplate/JDBC |
| **Lab06-DataAccess / Lab06b-EFCore** | DbContext directo en el service, DbSet, convenciones, LINQ, tracking y EnsureCreated | Los mismos endpoints y datos con EF Core y SQLite | Modelo de persistencia y tracking comparable a JPA/Hibernate |
| **Lab06-DataAccess / Lab06c-EFCore-Relations-Migrations** | Escritura, relación 1:N, Include, Change Tracker y migrations | GET de clientes/reservas y POST de reserva con Id generado | Navegaciones, persistencia y evolución del esquema |
| Lab07-ErrorHandling | Middleware, `IExceptionHandler` y `ProblemDetails` | Errores HTTP uniformes | `@ControllerAdvice` y exception handlers |
| Lab08-Testing | xUnit y `WebApplicationFactory` | Tests de integración HTTP | `@SpringBootTest` y herramientas de prueba HTTP |
| Lab09-RealApi | Integración de los conceptos anteriores | API pequeña de tareas con persistencia, validación y tests | Una aplicación Spring Boot de alcance equivalente |

**Estado:** Lab00–Lab05 y Lab06a–Lab06c están implementados. El bloque Lab06-DataAccess permite comparar Dapper y EF Core sobre SQLite, y profundizar relaciones, escritura y migrations. Los detalles de los próximos pasos se decidirán al abordar cada laboratorio.

Para cada incremento: leer el código → ejecutar → cambiar una cosa → verificar. Evitar capas o dependencias que no aporten al concepto del paso.
