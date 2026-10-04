using Lab05.Configuration;
using Microsoft.Extensions.Options;

namespace Lab05.Services;

public class HolaService : IHolaService
{
    private readonly SaludoOptions _options;

    public HolaService(IOptions<SaludoOptions> options)
    {
        _options = options.Value;
    }

    public async Task<string> ObtenerSaludoAsync()
    {
        // Simulación didáctica de una espera I/O; no realiza I/O real.
        await Task.Delay(250);

        return $"{_options.Mensaje} ({_options.Aplicacion})";
    }
}
