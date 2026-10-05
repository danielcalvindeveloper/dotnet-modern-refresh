using Lab06e.Data;
using Lab06e.Models;
using Lab06e.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var dataDirectory = Path.Combine(builder.Environment.ContentRootPath, "Data");
Directory.CreateDirectory(dataDirectory);
var connectionString = new SqliteConnectionStringBuilder
{
    DataSource = Path.Combine(dataDirectory, "reservas.db")
}.ToString();

builder.Logging.AddFilter(DbLoggerCategory.Database.Command.Name, LogLevel.Information);
builder.Logging.AddFilter(DbLoggerCategory.Database.Transaction.Name, LogLevel.Debug);
builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));
builder.Services.AddScoped<IReservaService, ReservaService>();
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        Console.WriteLine("[Exception Handler] HTTP 500");
        context.ProblemDetails.Title = "La operación falló.";
        context.ProblemDetails.Detail = "Error deliberado del laboratorio. Consultar reservas en una request nueva para comprobar qué se guardó.";
        context.ProblemDetails.Instance = context.HttpContext.Request.Path.ToString();
    };
});

var app = builder.Build();

app.Use(async (HttpContext context, Func<Task> next) =>
{
    Console.WriteLine($"[HTTP antes] {context.Request.Method} {context.Request.Path}");
    await next();
    Console.WriteLine($"[HTTP después] {context.Response.StatusCode}");
});

app.UseExceptionHandler();
app.MapControllers();

// Esquema versionado como en Lab06c/Lab06d; base propia, inicialmente sin reservas.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
    if (!await db.Clientes.AnyAsync())
    {
        db.Clientes.Add(new Cliente { Id = 1, Nombre = "Ana García", Email = "ana@example.com" });
        await db.SaveChangesAsync();
    }
}

app.Run();
