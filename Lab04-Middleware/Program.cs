using Lab04.Configuration;
using Lab04.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.Configure<SaludoOptions>(builder.Configuration.GetSection("Saludo"));
builder.Services.AddScoped<IHolaService, HolaService>();

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
