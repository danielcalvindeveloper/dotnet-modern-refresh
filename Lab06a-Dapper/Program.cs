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

app.Run();
