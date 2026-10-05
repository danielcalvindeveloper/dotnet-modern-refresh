using Lab06c.Models;

namespace Lab06c.Services;

public interface IReservaService
{
    Task<IEnumerable<Reserva>> ObtenerTodosAsync();
    Task<Reserva?> ObtenerPorIdAsync(int id);
    Task<Reserva?> CrearAsync(int clienteId, DateTime fecha, string estado);
    Task<Reserva?> ConfirmarAsync(int id);
}
