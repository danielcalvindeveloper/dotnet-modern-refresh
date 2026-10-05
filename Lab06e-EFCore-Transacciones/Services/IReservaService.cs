using Lab06e.Models;

namespace Lab06e.Services;

public interface IReservaService
{
    Task<Reserva> SaveChangesUnicoAsync();
    Task<Reserva> DosSaveChangesSinTransaccionAsync();
    Task<Reserva> ConTransaccionAsync(bool provocarError);
    Task<List<Reserva>> ObtenerReservasAsync();
}
