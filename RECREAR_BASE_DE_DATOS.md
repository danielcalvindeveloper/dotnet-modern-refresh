# Recrear las bases de Lab06a–Lab06e

Cada laboratorio tiene su propia base SQLite: un archivo local, sin servidor. Los comandos de esta guía son para PowerShell y el SDK **.NET 10**. Lab06f queda fuera de esta revisión.

## Qué se guarda en Git

Se versionan modelos/configuración, migrations con sus archivos Designer y `ModelSnapshot`, scripts SQL y documentación. El archivo SQLite y sus auxiliares (`.db-wal`, `.db-shm`, `.sqlite`, `.sqlite3`) se generan localmente y están ignorados: contienen el estado de tus experimentos, que puede ser distinto al de otro desarrollador.

Una migration describe un cambio de **estructura**: tablas, columnas, claves e índices. EF aplica las migrations en orden y anota cuáles ejecutó en `__EFMigrationsHistory`. El snapshot permite comparar el modelo al generar el próximo cambio; no es una copia de los datos. Aplicar las migrations incluidas reconstruye el esquema de una base nueva.

Los **datos iniciales** son otro paso: los clientes o reservas necesarios para probar los ejemplos. Los scripts `datos-iniciales.sql` preparan ese escenario; no se usa `HasData` ni se inserta el escenario en las migrations.

## Inventario real

Las rutas de esta tabla son relativas a la raíz del repositorio:

| Laboratorio | Archivo local | Cómo crear el esquema | Datos necesarios |
| --- | --- | --- | --- |
| `Lab06a-Dapper` | `Data/clientes.db` dentro del lab | `scripts/esquema.sql`: CREATE explícito, sin EF | Tres clientes para consultar |
| `Lab06b-EFCore` | `Data/clientes.db` dentro del lab | Migration `InitialCreate` | Los mismos tres clientes |
| `Lab06c-EFCore-Relations-Migrations` | `Data/clientes.db` dentro del lab | `InitialCreate` + `AgregarTelefonoCliente` | Tres clientes; reservas creadas mediante POST |
| `Lab06d-EFCore-Linq` | `Data/clientes.db` dentro del lab | `InitialCreate` | Dos clientes y cinco reservas con estados/fechas distintos |
| `Lab06e-EFCore-Transacciones` | `Data/reservas.db` dentro del lab | `InitialCreate` | Cliente 1; reservas y movimientos creados por los experimentos |

`Program.cs` mantiene las rutas bajo el content root de cada proyecto. La API no crea el esquema, aplica migrations ni inserta datos al arrancar. Preparar antes de `dotnet run`.

## Restaurar proyectos y herramientas

Abrir una terminal **en la raíz del repositorio**. Cada bloque siguiente indica un `Set-Location`; ejecutarlo desde la raíz, no desde otro laboratorio.

`dotnet restore` descarga los paquetes del `.csproj`. Para Lab06b–Lab06e, `dotnet tool restore` restaura `dotnet-ef` **10.0.12** desde el `.config/dotnet-tools.json` del laboratorio. No hace falta una instalación global de EF.

`dotnet ef database update` compila el proyecto y aplica las migrations que falten. Si el archivo no existe, crea la base y sus tablas. Desde un clon limpio no ejecutar `migrations add`: las migrations ya están incluidas.

## Ejecutar SQL sin instalar sqlite3

Se incluye [tools/SqliteScripts](tools/SqliteScripts/Program.cs), una herramienta de consola independiente de las APIs. Usa `Microsoft.Data.Sqlite` 10.0.12, el mismo proveedor ya utilizado por los labs; basta el SDK .NET. No es un DbSeeder ni código de arranque de los laboratorios.

Desde una carpeta Lab06a–Lab06e:

```powershell
dotnet restore ../tools/SqliteScripts/SqliteScripts.csproj
dotnet run --project ../tools/SqliteScripts -- Data/clientes.db scripts/datos-iniciales.sql
```

Los dos argumentos son **archivo de base** y **archivo SQL**, relativos a la terminal actual. En Lab06e utilizar `Data/reservas.db`. La herramienta crea el directorio si falta, abre la base, habilita foreign keys y ejecuta juntos los comandos en una transacción. Un error detiene el comando y no confirma parcialmente el script. Lee el SQL antes de crear la base; no busca migrations ni inserta nada implícitamente.

Los scripts agregan Ids faltantes con `ON CONFLICT(Id) DO NOTHING`. Repetirlos no duplica ni sobrescribe las filas de esos Ids. **No son un reset:** conservan modificaciones previas y filas adicionales. Para repetir exactamente el escenario original, usar una base nueva como se explica más abajo.

Si ya tenés `sqlite3`, también podés ejecutar el mismo SQL desde PowerShell, por ejemplo:

```powershell
sqlite3.exe -cmd 'PRAGMA foreign_keys=ON;' Data/clientes.db '.read scripts/datos-iniciales.sql'
```

La CLI es opcional; también sirve un editor SQLite que permita abrir el archivo y ejecutar el script completo. No mezclar comandos de la CLI como `.read` con el SQL puro de los archivos.

## Desde un clon limpio: Dapper

Desde la raíz:

```powershell
Set-Location .\Lab06a-Dapper
dotnet restore
dotnet restore ../tools/SqliteScripts/SqliteScripts.csproj
dotnet run --project ../tools/SqliteScripts -- Data/clientes.db scripts/esquema.sql
dotnet run --project ../tools/SqliteScripts -- Data/clientes.db scripts/datos-iniciales.sql
dotnet run
```

Lab06a ejecutaba `CREATE TABLE IF NOT EXISTS Clientes` e INSERT explícitos en `Program.cs`. Esas instrucciones ahora están en scripts para ejecutarlas manualmente. Dapper no administra migrations de EF ni necesita `dotnet-ef`. El CREATE repetible crea tablas faltantes; no evoluciona un esquema incompatible.

## Desde un clon limpio: EF Core

Ejemplo completo para Lab06d, desde la raíz:

```powershell
Set-Location .\Lab06d-EFCore-Linq
dotnet restore
dotnet tool restore
dotnet ef database update
dotnet restore ../tools/SqliteScripts/SqliteScripts.csproj
dotnet run --project ../tools/SqliteScripts -- Data/clientes.db scripts/datos-iniciales.sql
dotnet run
```

Lab06b y Lab06c usan la misma secuencia, entrando en sus carpetas de la tabla. Lab06e usa `Data/reservas.db`. Cada README contiene su bloque completo, su puerto y los endpoints de comprobación.

`database update` crea **estructura**, no los clientes/reservas de los scripts. Si la API responde con una lista vacía, verificar que se ejecutó el script sobre el archivo de ese laboratorio. Si aparece `no such table`, falta preparar el esquema o se usó otro archivo. No agregar `MigrateAsync` a `Program.cs` para corregirlo.

## Recrear desde cero cuando ya existe una base

No se borra ninguna base como parte de la preparación habitual. Para conservar tus experimentos y crear una nueva:

1. Detener la API con `Ctrl+C` y cerrar cualquier herramienta que tenga abierta esa base.
2. Guardar el archivo y sus auxiliares juntos en un respaldo.
3. Volver a ejecutar la secuencia de preparación del README, omitiendo sólo el `cd` si ya estás dentro del lab.

Este bloque **opcional** se ejecuta dentro de la carpeta del laboratorio seleccionado. Muestra las rutas antes de mover sólo los archivos de esa base; no toca las bases de otros labs:

```powershell
$laboratorio = Split-Path (Get-Location).Path -Leaf
if ($laboratorio -notin @('Lab06a-Dapper', 'Lab06b-EFCore', 'Lab06c-EFCore-Relations-Migrations', 'Lab06d-EFCore-Linq', 'Lab06e-EFCore-Transacciones')) {
    throw 'Entrar primero en la carpeta del laboratorio Lab06a–Lab06e.'
}
$archivo = if ($laboratorio -eq 'Lab06e-EFCore-Transacciones') { 'reservas.db' } else { 'clientes.db' }
$data = Join-Path (Get-Location).Path 'Data'
$respaldo = Join-Path $data ('respaldo-' + [DateTime]::UtcNow.ToString('yyyyMMdd-HHmmssfff'))
$origenes = @($archivo, "$archivo-shm", "$archivo-wal") | ForEach-Object { Join-Path $data $_ }
$origenes
$respaldo
# Ejecutar únicamente con API/editor cerrados y tras revisar las rutas:
New-Item -ItemType Directory -Path $respaldo -ErrorAction Stop | Out-Null
foreach ($origen in $origenes) {
    if (Test-Path -LiteralPath $origen) {
        Move-Item -LiteralPath $origen -Destination $respaldo -ErrorAction Stop
    }
}
```

El respaldo queda bajo `Data/respaldo-...`; sus archivos también se ignoran por Git. Mantener juntos `.db`, `.db-wal` y `.db-shm` cuando existan; no copiar sólo el archivo principal mientras haya conexiones abiertas. Al preparar de nuevo se crea otro archivo con el nombre original, sin eliminar el respaldo.

### Particularidad de Lab06b

La versión anterior usaba `EnsureCreatedAsync`, que crea un esquema sin historial de migrations y no lo evoluciona. Se agregó `InitialCreate` para preparar explícitamente el mismo modelo de Cliente, sin cambiar las consultas ni el objetivo del laboratorio.

Una base local antigua creada con EnsureCreated ya puede contener `Clientes` pero carecer del historial. No asumir que `database update` reconocerá esa tabla: puede fallar con `table Clientes already exists`. Para estos experimentos, respaldar la base antigua y crear una nueva con la secuencia indicada. No borrar ni intentar convertir automáticamente los datos antiguos. El contraste pedagógico EnsureCreated/migrations se mantiene explicado en Lab06b.

## Comprobar el escenario preparado

Después de ejecutar la API, en otra terminal:

```powershell
# Lab06a: tres clientes.
curl.exe -i http://localhost:5086/api/clientes
# Lab06b: los mismos tres clientes.
curl.exe -i http://localhost:5087/api/clientes
# Lab06c: tres clientes; reservas inicialmente [].
curl.exe -i http://localhost:5088/api/clientes
curl.exe -i http://localhost:5088/api/reservas
# Lab06d: cinco reservas; filtros combinados devuelven Ids 2 y 5.
curl.exe -i 'http://localhost:5091/api/reservas/buscar?estado=Pendiente&desde=2026-10-01'
# Lab06e: inicialmente []; los POST del README crean reservas y movimientos.
curl.exe -i http://localhost:5092/api/transacciones/reservas
```

Probar únicamente los puertos de los labs que hayas iniciado. Los resultados iniciales indicados suponen bases nuevas; aplicar de nuevo datos-iniciales no elimina los resultados de pruebas anteriores.

## Documentación oficial de Microsoft

- [Migrations y snapshot](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/)
- [Aplicar migrations](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/applying)
- [Herramientas locales de .NET](https://learn.microsoft.com/en-us/dotnet/core/tools/local-tools-how-to-use)
- [CLI de EF Core](https://learn.microsoft.com/en-us/ef/core/cli/dotnet)
- [EnsureCreated y sus diferencias con migrations](https://learn.microsoft.com/en-us/ef/core/managing-schemas/ensure-created)
- [Microsoft.Data.Sqlite: comandos múltiples](https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/batching)
