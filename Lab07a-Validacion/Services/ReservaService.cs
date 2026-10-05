using Lab07a.Models;

namespace Lab07a.Services;

public class ReservaService : IReservaService
{
    private int _ultimoId;

    public Reserva Crear(int clienteId, DateTime fecha, string estado)
    {
        return new Reserva
        {
            Id = Interlocked.Increment(ref _ultimoId),
            ClienteId = clienteId,
            Fecha = fecha,
            Estado = estado
        };
    }
}
