using Lab07a.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Una instancia por proceso para conservar el contador de Ids en memoria.
builder.Services.AddSingleton<IReservaService, ReservaService>();

var app = builder.Build();

app.MapControllers();

app.Run();
