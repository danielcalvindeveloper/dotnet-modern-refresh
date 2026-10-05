using Lab06c.Data;
using Lab06c.Models;
using Lab06c.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var dataDirectory = Path.Combine(builder.Environment.ContentRootPath, "Data");
Directory.CreateDirectory(dataDirectory);
var connectionString = new SqliteConnectionStringBuilder
{
    DataSource = Path.Combine(dataDirectory, "clientes.db")
}.ToString();

builder.Logging.AddFilter(DbLoggerCategory.Database.Command.Name, LogLevel.Information);
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IReservaService, ReservaService>();

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

// El esquema se prepara con dotnet ef database update; aquí solo agregamos datos.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (!await db.Clientes.AnyAsync())
    {
        db.Clientes.AddRange(
            new Cliente { Id = 1, Nombre = "Ana García", Email = "ana@example.com" },
            new Cliente { Id = 2, Nombre = "Bruno López", Email = "bruno@example.com" },
            new Cliente { Id = 3, Nombre = "Carla Pérez", Email = "carla@example.com" });
        await db.SaveChangesAsync();
    }
}

app.Run();
