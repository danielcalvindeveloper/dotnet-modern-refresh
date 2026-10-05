namespace Lab09b.Repositories;

// Contrato de una dependencia externa; este laboratorio no implementa persistencia.
public interface IReservaRepository
{
    Task<int> ObtenerCuposDisponiblesAsync(int turnoId);
    Task GuardarReservaAsync(int turnoId, int cantidadSolicitada);
}
