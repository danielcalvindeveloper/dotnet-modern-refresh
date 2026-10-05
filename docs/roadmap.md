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
| **Lab06-DataAccess / Lab06d-EFCore-Linq** | IQueryable vs IEnumerable, ejecución diferida, materialización, Expression, SQL, AsNoTracking y Specification mínima | Búsquedas con filtros opcionales; activas con expresión/Specification y comparación SQL vs memoria | Streams sobre objetos vs consultas construidas para persistencia; criterios reutilizables |
| **Lab06-DataAccess / Lab06e-EFCore-Transacciones** | Atomicidad de SaveChanges, transacción explícita, commit/rollback y tracker después del rollback | Cuatro POST y un GET para contrastar cambios persistidos con estado en memoria | Frontera transaccional de una operación de negocio |
| **Lab06-DataAccess / Lab06f-EFCore-Concurrencia** | Race condition, lost update, token Version compatible con SQLite y DbUpdateConcurrencyException | Dos requests simultáneos sin control y con control; éxito vs conflicto HTTP 409 | Versionado optimista de una entidad compartida |
| **Lab07a-Validacion** | Data Annotations, binding, ModelState y validación automática con ApiController | `POST /api/reservas` en memoria; 200 válido y 400 antes de la Action si es inválido | Bean Validation y `@Valid` |
| **Lab07b-Errores-ProblemDetails** | `UseExceptionHandler`, `AddProblemDetails` y propagación de excepciones por el pipeline | GET normal con 200 y GET con excepción deliberada: 500 + ProblemDetails | `@ControllerAdvice` y exception handlers |
| Lab08-Testing | xUnit y `WebApplicationFactory` | Tests de integración HTTP | `@SpringBootTest` y herramientas de prueba HTTP |
| Lab09-RealApi | Integración de los conceptos anteriores | API pequeña de tareas con persistencia, validación y tests | Una aplicación Spring Boot de alcance equivalente |

**Estado:** Lab00–Lab05, Lab06a–Lab06f y Lab07a–Lab07b están implementados. El bloque Lab06-DataAccess compara Dapper y EF Core sobre SQLite, profundiza relaciones/migrations, composición LINQ, transacciones y concurrencia optimista. Lab06e estudia atomicidad; Lab06f muestra la carrera sin control y la detección de conflictos mediante un token de versión. Lab07a se enfoca en validación HTTP; Lab07b en excepciones globales y ProblemDetails. Los detalles y nombres de los próximos pasos se decidirán al abordar cada laboratorio.

Para cada incremento: leer el código → ejecutar → cambiar una cosa → verificar. Evitar capas o dependencias que no aporten al concepto del paso.
