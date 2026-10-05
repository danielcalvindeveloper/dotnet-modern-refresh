using Lab06f.Data;
using Lab06f.Models;
using Lab06f.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var dataDirectory = Path.Combine(builder.Environment.ContentRootPath, "Data");
Directory.CreateDirectory(dataDirectory);
var connectionString = new SqliteConnectionStringBuilder
{
    DataSource = Path.Combine(dataDirectory, "turnos.db")
}.ToString();

builder.Logging.AddFilter(DbLoggerCategory.Database.Command.Name, LogLevel.Information);
builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));
builder.Services.AddDbContext<SinControlDbContext>(options => options.UseSqlite(connectionString));
builder.Services.AddScoped<ITurnoService, TurnoService>();
builder.Services.AddProblemDetails();

var app = builder.Build();
app.UseExceptionHandler(); // Fallback 500 para errores inesperados, no los convierte en conflicto.
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
    if (!await db.Turnos.AnyAsync())
    {
        db.Turnos.Add(new Turno
        {
            Id = 1, Fecha = new DateTime(2026, 10, 6, 10, 0, 0),
            Capacidad = 1, CuposOcupados = 0, Version = 1
        });
        await db.SaveChangesAsync();
    }
}

app.Run();
