using Lab06f.Models;

namespace Lab06f.Services;

public interface ITurnoService
{
    Task<Turno?> ObtenerAsync(int turnoId);
    Task<Turno?> ResetAsync(int turnoId);
    Task<ResultadoReserva> ReservarSinControlAsync(int turnoId, string operacion);
    Task<ResultadoReserva> ReservarConControlAsync(int turnoId, string operacion);
}
