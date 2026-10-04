using Lab03.Configuration;
using Lab03.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.Configure<SaludoOptions>(builder.Configuration.GetSection("Saludo"));
builder.Services.AddScoped<IHolaService, HolaService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();

app.Run();
