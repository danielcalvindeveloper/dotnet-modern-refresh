using Lab08.Models;

namespace Lab08.Services;

public sealed class ReservaService : IReservaService
{
    private readonly ILogger<ReservaService> _logger;
    private static readonly Reserva Ejemplo = new()
    {
        Id = 1, ClienteId = 1, Fecha = new DateTime(2026, 10, 6, 10, 0, 0), Estado = "Pendiente"
    };

    public ReservaService(ILogger<ReservaService> logger) => _logger = logger;

    public Reserva? Obtener(int id)
    {
        _logger.LogTrace("Entrando en Obtener para ReservaId={ReservaId}", id);
        _logger.LogDebug("Buscando reserva {ReservaId} en memoria", id);
        if (id != Ejemplo.Id)
        {
            _logger.LogWarning("Reserva {ReservaId} no encontrada", id);
            return null;
        }

        _logger.LogInformation("Reserva {ReservaId} encontrada en estado {Estado}", id, Ejemplo.Estado);
        return Ejemplo;
    }

    public void ProvocarError(int id)
    {
        _logger.LogInformation("Provocando un error didáctico para reserva {ReservaId}", id);
        throw new InvalidOperationException("Error deliberado en ReservaService para estudiar logging.");
    }

    public void MostrarNiveles(int id)
    {
        // Sólo muestras de severidad: Error/Critical aquí no representan fallos reales.
        _logger.LogTrace("DEMO Trace para reserva {ReservaId}", id);
        _logger.LogDebug("DEMO Debug para reserva {ReservaId}", id);
        _logger.LogInformation("DEMO Information para reserva {ReservaId}", id);
        _logger.LogWarning("DEMO Warning para reserva {ReservaId}", id);
        _logger.LogError("DEMO Error para reserva {ReservaId}", id);
        _logger.LogCritical("DEMO Critical para reserva {ReservaId}", id);
    }
}
