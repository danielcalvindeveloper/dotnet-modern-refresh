# Lab06b — EF Core

## Objetivo

Resolver los mismos endpoints y clientes de Lab06a con EF Core 10 y SQLite, usando `AppDbContext` directamente desde el service.

## Arquitectura

```mermaid
flowchart LR
    HTTP --> Controller
    Controller --> Service
    Service --> DbContext
    DbContext --> EFCore[EF Core]
    EFCore --> SQLite
```

El middleware conserva los logs antes y después de los endpoints. `docs/datos.mmd` contiene la copia del diagrama para el IDE.

## Conceptos nuevos

`Cliente` es una entidad incluida en el modelo mediante `DbSet<Cliente>`. Por convención, `Id` es clave primaria, la tabla se llama `Clientes` y las propiedades se mapean a columnas. No hace falta configurar este mapping simple.

`AddDbContext<AppDbContext>(options => options.UseSqlite(...))` registra el contexto y el proveedor SQLite en DI. Controller y service mantienen sus contratos HTTP y de lectura de Lab06a.

## DbContext

`AppDbContext` coordina consultas, mapping, tracking y persistencia. Recibe `DbContextOptions<AppDbContext>` por constructor y el service recibe el contexto por DI.

`AddDbContext` usa **Scoped** por defecto: una instancia por petición HTTP, liberada por DI al terminar. EF Core gestiona la apertura y cierre de conexiones según las operaciones; el lifetime del contexto no significa una conexión abierta durante toda la petición.

## DbSet

`Clientes` es el punto de entrada para consultar entidades y registrar altas. No es una lista con todos los clientes ya cargados: la consulta se ejecuta al materializar sus resultados.

## Change Tracking

Las consultas de este laboratorio usan tracking por defecto. EF conserva las entidades materializadas y sus estados en el contexto para detectar modificaciones cuando se llama a `SaveChangesAsync()`.

Los endpoints solamente leen; no necesitan guardar cambios. Las entidades consultadas quedan rastreadas por defecto y el tracking termina con el contexto. Los datos iniciales se preparan con SQL fuera de la API.

## LINQ → SQL

`OrderBy` construye la consulta; `ToListAsync()` la ejecuta y materializa la lista. `SingleOrDefaultAsync(cliente => cliente.Id == id)` genera el filtro SQL parametrizado y devuelve una entidad o `null`.

La categoría `Microsoft.EntityFrameworkCore.Database.Command` está habilitada a Information en `Program.cs`: observar SELECT, WHERE y ORDER BY en la consola. No todo código C# es traducible a SQL.

Las APIs son async, pero se mantiene la limitación de Lab06a: el proveedor `Microsoft.Data.Sqlite` ejecuta las operaciones de I/O de forma síncrona.

## Comparación directa con Lab06a-Dapper

| Responsabilidad | Lab06a — Dapper | Lab06b — EF Core |
| --- | --- | --- |
| Acceso desde el service | `IClienteRepository` | `AppDbContext` |
| Consulta | SQL escrito en el repository | LINQ traducido a SQL por EF |
| Conexión | Apertura explícita y `using` | Gestionada por EF para sus operaciones |
| Materialización | Mapping de columnas por Dapper | Mapping de entidades por el modelo EF |
| Tracking | Los objetos consultados no se rastrean | Entidades rastreadas por defecto |
| Preparación explícita | Script CREATE + script de datos | Migration del modelo + script de datos |

Se conservan SQLite, los datos iniciales y las respuestas HTTP. EF asume además la traducción de consultas y la generación de comandos para persistir cambios.

## EnsureCreated vs migrations

`EnsureCreatedAsync()` crea la base y el esquema si hace falta; no evoluciona un esquema existente ni mantiene un historial de migrations. Era el mecanismo original de este laboratorio introductorio.

**No es la estrategia habitual para evolucionar el esquema de una aplicación real.** Para reproducir el mismo esquema desde un clon se incluye ahora `InitialCreate`, aplicada manualmente. El foco sigue siendo DbContext y consultas; la evolución de migrations se estudia en Lab06c. No mezclar este historial con una base antigua creada mediante EnsureCreated: respaldarla y preparar una nueva según la guía común.

## Preparación de la base de datos

La base local `Data/clientes.db` y sus auxiliares no se versionan. El mismo modelo de Cliente se reconstruye con la migration inicial incluida. Necesita los tres clientes de Lab06a. Una base antigua creada por EnsureCreated requiere el respaldo y la recreación explicados en la guía común.

[Cómo recrear la base de datos y aplicar las migrations](../RECREAR_BASE_DE_DATOS.md). Los [datos iniciales](scripts/datos-iniciales.sql) se ejecutan explícitamente; no se cargan desde `Program.cs`. La herramienta común usa el mismo proveedor SQLite y evita instalar sqlite3 CLI.

```powershell
# Desde una terminal en la raíz del repositorio:
cd Lab06b-EFCore
dotnet restore
dotnet tool restore
dotnet ef database update
dotnet restore ../tools/SqliteScripts/SqliteScripts.csproj
dotnet run --project ../tools/SqliteScripts -- Data/clientes.db scripts/datos-iniciales.sql
dotnet run
```

Los comandos suponen un clon limpio. Repetir el script agrega Ids faltantes y conserva datos existentes; no resetea los experimentos. Para volver exactamente al inicio, seguir el procedimiento de respaldo de la guía común con la API detenida.

## Cómo ejecutar

Desde la raíz, con el SDK .NET 10:

Seguir primero la sección **Preparación de la base de datos** anterior. Después ejecutar `dotnet run` desde `Lab06b-EFCore`.

Development en `http://localhost:5087`. La base es `Lab06b-EFCore/Data/clientes.db`, independiente de Lab06a y excluida de Git. La preparación explícita agrega los mismos tres clientes. La API no inicializa la base al arrancar y reiniciar conserva los datos. Detener con `Ctrl+C`.

## Cómo probar

En otra terminal:

```powershell
curl.exe -i http://localhost:5087/api/clientes
curl.exe -i http://localhost:5087/api/clientes/1
curl.exe -i http://localhost:5087/api/clientes/999
```

Lista y cliente existente devuelven 200; el Id inexistente devuelve 404. Para el cliente 1:

```json
{"id":1,"nombre":"Ana García","email":"ana@example.com"}
```

Comparar el LINQ de `ClienteService.cs` con el SQL registrado en consola y con el repository de Lab06a. OpenAPI sigue en `/openapi/v1.json` durante Development.

## Archivos importantes

- `Models/Cliente.cs`: entidad y mapping por convención.
- `Data/AppDbContext.cs`: contexto, options y `DbSet<Cliente>`.
- `Services/IClienteService.cs` y `ClienteService.cs`: contrato y consultas LINQ async.
- `Controllers/ClientesController.cs`: mismos endpoints y respuestas que Lab06a.
- `Program.cs`: proveedor SQLite, DI scoped, log SQL y middleware.
- `Lab06b-EFCore.csproj`: paquete EF Core SQLite y OpenAPI.

## Documentación oficial de Microsoft

- [DbContext, configuración y DI](https://learn.microsoft.com/en-us/ef/core/dbcontext-configuration/)
- [Entidades y convenciones](https://learn.microsoft.com/en-us/ef/core/modeling/entity-types)
- [Consultas LINQ](https://learn.microsoft.com/en-us/ef/core/querying/)
- [Change Tracking](https://learn.microsoft.com/en-us/ef/core/change-tracking/)
- [Consultas async](https://learn.microsoft.com/en-us/ef/core/miscellaneous/async)
- [EnsureCreated](https://learn.microsoft.com/en-us/ef/core/managing-schemas/ensure-created)
- [Logging integrado](https://learn.microsoft.com/en-us/ef/core/logging-events-diagnostics/extensions-logging)
- [Limitaciones async de SQLite](https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/async)
