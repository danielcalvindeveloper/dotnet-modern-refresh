using Lab08.Errors;
using Lab08.Services;

var builder = WebApplication.CreateBuilder(args);

// CreateBuilder ya configura logging, providers estándar y lectura de appsettings.
builder.Services.AddControllers();
builder.Services.AddScoped<IReservaService, ReservaService>();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ReservaExceptionHandler>();

var app = builder.Build();

app.UseExceptionHandler();
app.MapControllers();

app.Run();
