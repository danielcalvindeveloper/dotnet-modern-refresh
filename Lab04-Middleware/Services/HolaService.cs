using Lab04.Configuration;
using Microsoft.Extensions.Options;

namespace Lab04.Services;

public class HolaService : IHolaService
{
    private readonly SaludoOptions _options;

    public HolaService(IOptions<SaludoOptions> options)
    {
        _options = options.Value;
    }

    public string ObtenerSaludo()
    {
        return $"{_options.Mensaje} ({_options.Aplicacion})";
    }
}
