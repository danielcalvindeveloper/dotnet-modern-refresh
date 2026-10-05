using Lab09c.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Una colección por aplicación: las reservas sobreviven entre requests del mismo host.
builder.Services.AddSingleton<IReservaService, ReservaService>();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();
app.Use(async (HttpContext context, RequestDelegate next) =>
{
    // Permite comprobar desde el test que se ejecutó middleware real.
    context.Response.Headers["X-Lab09c"] = "middleware";
    await next(context);
});
app.UseRouting();
app.MapControllers();

app.Run();

// Hace accesible el punto de entrada generado por top-level statements al proyecto de tests.
public partial class Program { }
