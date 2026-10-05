using Lab06d.Data;
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

app.Run();
