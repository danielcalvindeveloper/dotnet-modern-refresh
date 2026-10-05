namespace Lab09a.Services;

public sealed class ReservaService
{
    // Evalúa disponibilidad; no crea una reserva ni modifica el número de lugares.
    public bool PuedeReservar(int lugaresDisponibles, int cantidadSolicitada)
    {
        return lugaresDisponibles > 0
            && cantidadSolicitada > 0
            && cantidadSolicitada <= lugaresDisponibles;
    }
}
