using Lab09b.Repositories;

namespace Lab09b.Services;

public sealed class ReservaService
{
    private readonly IReservaRepository _repository;

    public ReservaService(IReservaRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> ReservarAsync(int turnoId, int cantidadSolicitada)
    {
        int cuposDisponibles = await _repository.ObtenerCuposDisponiblesAsync(turnoId);

        // Las mismas reglas de disponibilidad que en Lab09a.
        if (cuposDisponibles <= 0 || cantidadSolicitada <= 0 || cantidadSolicitada > cuposDisponibles)
        {
            return false;
        }

        await _repository.GuardarReservaAsync(turnoId, cantidadSolicitada);
        return true;
    }
}
