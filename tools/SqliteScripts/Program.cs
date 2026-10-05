using Microsoft.Data.Sqlite;

// Herramienta de preparación manual, independiente de las APIs y sin EF Core.
if (args.Length != 2)
{
    Console.Error.WriteLine("Uso: dotnet run --project tools/SqliteScripts -- <base.db> <script.sql>");
    return 1;
}

var databasePath = Path.GetFullPath(args[0]);
var scriptPath = Path.GetFullPath(args[1]);
var sql = await File.ReadAllTextAsync(scriptPath); // Validar el archivo antes de crear la base.
Directory.CreateDirectory(Path.GetDirectoryName(databasePath)!);

await using var connection = new SqliteConnection(new SqliteConnectionStringBuilder
{
    DataSource = databasePath,
    ForeignKeys = true
}.ToString());
await connection.OpenAsync();

// Aplicar juntos los comandos del script; los scripts no contienen BEGIN/COMMIT.
await using var transaction = await connection.BeginTransactionAsync();
await using var command = connection.CreateCommand();
command.Transaction = (SqliteTransaction)transaction;
command.CommandText = sql;
await command.ExecuteNonQueryAsync();
await transaction.CommitAsync();
Console.WriteLine($"SQL aplicado: {scriptPath}");
Console.WriteLine($"Base: {databasePath}");
return 0;
