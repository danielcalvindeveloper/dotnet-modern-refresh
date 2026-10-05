using Lab07b.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<IPruebaService, PruebaService>();
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        // Se ejecuta al preparar la respuesta; la captura la hace UseExceptionHandler.
        Console.WriteLine("[Exception Handler] Preparando ProblemDetails");
        context.ProblemDetails.Title = "Se produjo un error inesperado.";
        context.ProblemDetails.Detail = "Error simulado en este laboratorio. La solicitud no pudo completarse.";
        context.ProblemDetails.Instance = context.HttpContext.Request.Path.ToString();
    };
});

var app = builder.Build();

// Este middleware envuelve también al handler: next retorna tras manejar el error.
app.Use(async (HttpContext context, Func<Task> next) =>
{
    Console.WriteLine($"[Middleware antes] {context.Request.Method} {context.Request.Path}");
    await next();
    Console.WriteLine($"HTTP {context.Response.StatusCode}");
    Console.WriteLine("[Middleware después]");
});

// Antes de los endpoints; activo en Development y Production para observar el mismo JSON.
app.UseExceptionHandler();
app.MapControllers();

app.Run();
