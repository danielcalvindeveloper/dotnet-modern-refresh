# Lab06d — EF Core: LINQ como consulta traducible

## Objetivo y ejecución

Estudiar **dónde y cuándo se ejecuta una consulta**, manteniendo Controller → Service → AppDbContext → EF Core → SQLite y la relación Cliente 1:N Reserva de Lab06c. No es una introducción a los operadores LINQ.

Desde la raíz, con .NET 10:

```powershell
cd Lab06d-EFCore-Linq
dotnet run
```

Escucha en `http://localhost:5091`. El arranque aplica la migration inicial incluida mediante `MigrateAsync` y agrega dos clientes y cinco reservas si faltan los datos. La base propia queda en `Data/clientes.db`, excluida de Git; no modifica la de Lab06c. Se conserva migrations, sin `EnsureCreated`. Para regenerar migrations se incluye la herramienta local: `dotnet tool restore` y luego los comandos aprendidos en Lab06c.

## IQueryable, composición y ejecución diferida

`IQueryable<Reserva>` representa una consulta con un árbol de expresión y un proveedor que sabe ejecutarla. Aquí ese proveedor es EF Core; declarar `IQueryable` por sí solo no garantiza ejecución en una base de datos.

En `BuscarAsync`, partimos de `_db.Reservas`, agregamos `AsNoTracking` e `Include`, y componemos incrementalmente `Where(Estado)`, `Where(Fecha)` y `OrderBy(Fecha)`. Cada operador construye una nueva consulta; las asignaciones no ejecutan SELECT. Los filtros opcionales producen **una expresión final**, no una consulta SQL por cada `Where`.

```text
IQueryable → Where → Where → OrderBy → ToListAsync → SQL → objetos Reserva
```

La **materialización** ocurre con `await query.ToListAsync()`: EF ejecuta SQL, lee filas y crea los objetos de una lista. Enumerar una consulta EF o consumirla con operaciones como `CountAsync` también puede ejecutar SQL; no solo `ToListAsync`. El service devuelve listas ya materializadas.

## IEnumerable vs IQueryable

Ambos permiten LINQ, pero sus operadores reciben representaciones distintas:

| Fuente/operador | Predicado | Ejecución del filtro en este laboratorio |
| --- | --- | --- |
| `IQueryable` / `Queryable.Where` | `Expression<Func<Reserva, bool>>` | EF traduce el criterio a SQL |
| Lista como `IEnumerable` / `Enumerable.Where` | `Func<Reserva, bool>` | Código .NET sobre objetos en memoria |

`IEnumerable<T>` es un contrato de enumeración; **no significa necesariamente una lista ya cargada**. `IQueryable<T>` también lo implementa. Cambiar al uso de operadores `Enumerable` no implica por sí mismo materializar; por ejemplo, `AsEnumerable()` no ejecuta la consulta, pero los filtros agregados después se evalúan al enumerar en .NET.

`CompararAsync` muestra el límite de forma explícita:

```csharp
// WHERE llega a SQLite antes de leer filas.
var enSql = await _db.Reservas
    .Where(r => r.Estado == "Pendiente")
    .ToListAsync();

// SELECT ya trajo todas las filas; este Where no puede retroactivamente cambiarlo.
IEnumerable<Reserva> todas = await _db.Reservas.ToListAsync();
Func<Reserva, bool> pendientes = r => r.Estado == "Pendiente";
var enMemoria = todas.Where(pendientes).ToList();
```

El endpoint `/comparar` hace intencionalmente **dos SELECT** y devuelve `enSql` y `enMemoria` con los mismos tres Ids. El primero tiene `WHERE`; el segundo carga cinco reservas sin ese filtro. El `Where` en memoria también es diferido, pero trabaja sobre la lista al llamar al `ToList()` final, sin otro SELECT. Los counts en consola ilustran el recorrido, no un benchmark.

## Func, Expression y traducción

`Func<Reserva, bool>` es un delegate ejecutable sobre una reserva. `Expression<Func<Reserva, bool>>` representa la lambda como **datos/estructura inspeccionable**:

```csharp
Expression<Func<Reserva, bool>> activas =
    r => r.Estado != "Cancelada";

var query = _db.Reservas.Where(activas);
```

Conceptualmente:

```text
Lambda
  ↓
NotEqual
 /      \
Estado  "Cancelada"

Expression Tree → EF Core → SQL → WHERE Estado <> 'Cancelada'
```

`ObtenerActivasAsync` imprime la expresión y `Body.NodeType=NotEqual`. La misma sintaxis de lambda puede convertirse en delegate o expresión según el tipo esperado. LINQ es la sintaxis/API; el árbol es la representación que permite a EF inspeccionar miembros, operadores y valores y al proveedor SQLite traducirlos a SQL. Pasar un `Func` a `Where` puede seleccionar `Enumerable.Where` y llevar el filtro a .NET.

No todo código C# es traducible. Un filtro de una consulta EF que el proveedor no puede traducir normalmente produce una excepción, no una traducción mágica de cualquier método .NET.

```mermaid
flowchart LR
    A[LINQ] --> B[IQueryable]
    B --> C[Expression Tree]
    C --> D[EF Core]
    D --> E[SQL]
    E --> F[SQLite]
    F --> G[Objetos .NET]
```

## SQL: inspección vs ejecución

`ToQueryString()` genera una representación SQL para depuración; puede traducir la expresión, pero **no ejecuta SQL ni materializa resultados**. No es una API para obtener un script de ejecución de producción.

Seguir los marcadores de consola de una búsqueda con ambos filtros: construcción → inspección → materialización → resultado. Solo después del marcador de materialización aparece `Executed DbCommand` del logging de EF. El SELECT impreso por `ToQueryString` y el SELECT del log son dos vistas de la misma consulta, no dos ejecuciones.

SQL simplificado del ejemplo combinado:

```sql
SELECT ... FROM Reservas
WHERE Estado = @estado AND Fecha >= @desde
ORDER BY Fecha
```

Se conserva `Include(r => r.Cliente)` para devolver información básica del cliente, por lo que el SQL real también tiene un JOIN. `[JsonIgnore]` en `Cliente.Reservas` evita el ciclo JSON como en Lab06c.

## AsNoTracking y Lab06c

Las búsquedas y consultas de activas usan `AsNoTracking`: son lecturas y no necesitan que el contexto rastree cambios. Las consultas de entidades tienen tracking por defecto; es lo que permitió observar `Added`/`Unchanged` y persistir cambios en Lab06c.

Con un contexto nuevo por request, la consola muestra `tracked=0` después de las lecturas sin tracking. `/comparar` conserva intencionalmente el comportamiento por defecto en la carga temprana: quedan cinco reservas tracked. Contamos `Entries<Reserva>()`, no todos los tipos de entidad. No se llama a `SaveChanges` en estas consultas.

Se usan métodos async de EF Core; SQLite conserva la limitación de I/O síncrono del proveedor explicada en los laboratorios anteriores.

## ¿Qué aporta Specification si ya tenemos LINQ?

Primero, el criterio puede escribirse directamente y componerse:

```csharp
_db.Reservas
    .Where(r => r.Estado != "Cancelada")
    .Where(r => r.Fecha >= desde);
```

Extraer `activas` como expresión permite reutilizarlo. `ReservasActivasSpecification` da un **nombre con significado** a ese criterio, mediante una interfaz con una sola propiedad `Criteria`. Se aplica exactamente como otra expresión:

```csharp
var spec = new ReservasActivasSpecification();
var reservas = await _db.Reservas.Where(spec.Criteria).ToListAsync();
```

LINQ/Expression es el mecanismo para expresar la consulta; Specification encapsula, nombra y reutiliza un criterio. No reemplaza a EF ni cambia el lugar de ejecución. Aquí “activa” significa únicamente “no cancelada”. Comparar `/activas` y `/activas?usarSpecification=true`: mismo SQL y mismos resultados. El filtro opcional `desde` se compone fuera de la Specification.

## Por qué no agregamos Repository todavía

Queremos ver DbContext → DbSet → IQueryable → LINQ → Expression Tree → SQL. Un Repository podría encapsular posteriormente consultas como `ObtenerReservasActivasAsync(...)` o `BuscarTurnosDisponiblesAsync(...)`; primero conviene entender qué ejecución y composición estaríamos ocultando. El service consulta directamente el contexto y no hay un evaluador ni framework de Specifications.

## Datos y pruebas

| Id | ClienteId | Fecha | Estado |
| --- | --- | --- | --- |
| 1 | 1 | 2026-09-28 09:00 | Pendiente |
| 2 | 1 | 2026-10-01 09:00 | Pendiente |
| 3 | 2 | 2026-10-03 11:00 | Confirmada |
| 4 | 2 | 2026-10-05 10:00 | Cancelada |
| 5 | 1 | 2026-10-07 16:00 | Pendiente |

En otra terminal PowerShell, todos estos GET deben devolver **200**:

```powershell
# Todas: Ids 1,2,3,4,5.
curl.exe -sS -i http://localhost:5091/api/reservas

# Búsqueda sin filtros: Ids 1,2,3,4,5.
curl.exe -sS -i http://localhost:5091/api/reservas/buscar

# Solo estado: Ids 1,2,5.
curl.exe -sS -i 'http://localhost:5091/api/reservas/buscar?estado=Pendiente'

# Solo desde (inclusive): Ids 2,3,4,5.
curl.exe -sS -i 'http://localhost:5091/api/reservas/buscar?desde=2026-10-01'

# Dos Where componiendo un SELECT: Ids 2,5.
curl.exe -sS -i 'http://localhost:5091/api/reservas/buscar?estado=Pendiente&desde=2026-10-01'

# Expression: Ids 1,2,3,5.
curl.exe -sS -i http://localhost:5091/api/reservas/activas

# Specification: mismos Ids 1,2,3,5.
curl.exe -sS -i 'http://localhost:5091/api/reservas/activas?usarSpecification=true'

# Specification más filtro externo: Ids 2,3,5.
curl.exe -sS -i 'http://localhost:5091/api/reservas/activas?usarSpecification=true&desde=2026-10-01'

# Dos SELECT intencionales: enSql y enMemoria contienen Ids 1,2,5.
curl.exe -sS -i http://localhost:5091/api/reservas/comparar
```

Mirar `Services/ReservaService.cs`, `Specifications/`, `Controllers/ReservasController.cs` y `Data/AppDbContext.cs`. El Mermaid también está en `docs/consultas.mmd`. Detener con `Ctrl+C`.

## Documentación oficial de Microsoft

- [Consultas de EF Core: visión general y ejecución](https://learn.microsoft.com/en-us/ef/core/querying/how-query-works)
- [Queryable.Where y Expression](https://learn.microsoft.com/en-us/dotnet/api/system.linq.queryable.where?view=net-10.0)
- [Enumerable.Where y Func](https://learn.microsoft.com/en-us/dotnet/api/system.linq.enumerable.where?view=net-10.0)
- [Expression Trees](https://learn.microsoft.com/en-us/dotnet/csharp/advanced-topics/expression-trees/)
- [Evaluación en cliente y servidor](https://learn.microsoft.com/en-us/ef/core/querying/client-eval)
- [ToQueryString](https://learn.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.entityframeworkqueryableextensions.toquerystring?view=efcore-10.0)
- [Tracking y AsNoTracking](https://learn.microsoft.com/en-us/ef/core/querying/tracking)
- [Limitaciones del proveedor SQLite](https://learn.microsoft.com/en-us/ef/core/providers/sqlite/limitations)
