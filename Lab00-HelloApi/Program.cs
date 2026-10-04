var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/api/hola", () => TypedResults.Ok(new { mensaje = "Hola desde ASP.NET Core" }));

app.Run();
