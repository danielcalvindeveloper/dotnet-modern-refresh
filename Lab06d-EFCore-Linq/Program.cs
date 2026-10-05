using Lab06d.Data;
using Lab06d.Models;
using Lab06d.Services;
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
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));
builder.Services.AddScoped<IReservaService, ReservaService>();

var app = builder.Build();

app.Use(async (HttpContext context, Func<Task> next) =>
{
    Console.WriteLine($"[HTTP antes] {context.Request.Method} {context.Request.Path}{context.Request.QueryString}");
    await next();
    Console.WriteLine($"[HTTP después] {context.Response.StatusCode}");
});

app.MapControllers();

// Base propia del laboratorio: aplica la migration incluida y agrega pocos datos.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();

    if (!await db.Clientes.AnyAsync())
    {
        db.Clientes.AddRange(
            new Cliente { Id = 1, Nombre = "Ana García", Email = "ana@example.com" },
            new Cliente { Id = 2, Nombre = "Bruno López", Email = "bruno@example.com" });
    }

    if (!await db.Reservas.AnyAsync())
    {
        db.Reservas.AddRange(
            new Reserva { Id = 1, ClienteId = 1, Fecha = new DateTime(2026, 9, 28, 9, 0, 0), Estado = "Pendiente" },
            new Reserva { Id = 2, ClienteId = 1, Fecha = new DateTime(2026, 10, 1, 9, 0, 0), Estado = "Pendiente" },
            new Reserva { Id = 3, ClienteId = 2, Fecha = new DateTime(2026, 10, 3, 11, 0, 0), Estado = "Confirmada" },
            new Reserva { Id = 4, ClienteId = 2, Fecha = new DateTime(2026, 10, 5, 10, 0, 0), Estado = "Cancelada" },
            new Reserva { Id = 5, ClienteId = 1, Fecha = new DateTime(2026, 10, 7, 16, 0, 0), Estado = "Pendiente" });
    }

    await db.SaveChangesAsync();
}

app.Run();
