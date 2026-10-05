using System.Collections.Concurrent;
using Lab09c.Models;

namespace Lab09c.Services;

public sealed class ReservaService : IReservaService
{
    // Estado de esta instancia, sin campos static: cada host empieza con la misma reserva.
    private readonly ConcurrentDictionary<int, Reserva> _reservas = new();
    private int _ultimoId = 1;

    public ReservaService()
    {
        _reservas[1] = new Reserva
        {
            Id = 1,
            ClienteId = 1,
            Fecha = new DateTime(2026, 10, 6, 10, 0, 0),
            Estado = "Pendiente"
        };
    }

    public Reserva? Obtener(int id)
    {
        return _reservas.TryGetValue(id, out var reserva) ? reserva : null;
    }

    public Reserva Crear(int clienteId, DateTime fecha, string estado)
    {
        var reserva = new Reserva
        {
            Id = Interlocked.Increment(ref _ultimoId),
            ClienteId = clienteId,
            Fecha = fecha,
            Estado = estado
        };
        _reservas[reserva.Id] = reserva;
        return reserva;
    }
}
