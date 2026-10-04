using Dapper;
using Lab06a.Repositories;
using Lab06a.Services;
using Microsoft.Data.Sqlite;

var builder = WebApplication.CreateBuilder(args);

var dataDirectory = Path.Combine(builder.Environment.ContentRootPath, "Data");
Directory.CreateDirectory(dataDirectory);
var connectionString = new SqliteConnectionStringBuilder
{
    DataSource = Path.Combine(dataDirectory, "clientes.db")
}.ToString();

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddScoped<IClienteRepository>(_ => new ClienteRepository(connectionString));
builder.Services.AddScoped<IClienteService, ClienteService>();

var app = builder.Build();

app.Use(async (HttpContext context, Func<Task> next) =>
{
    Console.WriteLine($"[Middleware antes] {context.Request.Method} {context.Request.Path}");
    await next();
    Console.WriteLine($"[Middleware después] HTTP {context.Response.StatusCode}");
});

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();

// Inicialización local, antes de aceptar peticiones. Es segura al repetir el arranque.
using (var connection = new SqliteConnection(connectionString))
{
    await connection.OpenAsync();
    await connection.ExecuteAsync("""
        CREATE TABLE IF NOT EXISTS Clientes (
            Id INTEGER PRIMARY KEY,
            Nombre TEXT NOT NULL,
            Email TEXT NOT NULL
        );

        INSERT OR IGNORE INTO Clientes (Id, Nombre, Email) VALUES
            (1, 'Ana García', 'ana@example.com'),
            (2, 'Bruno López', 'bruno@example.com'),
            (3, 'Carla Pérez', 'carla@example.com');
        """);
} // Dispose cierra la conexión de inicialización.

app.Run();
