using Lab06d.Models;

namespace Lab06d.Services;

public interface IReservaService
{
    Task<List<Reserva>> BuscarAsync(string? estado, DateTime? desde);
    Task<List<Reserva>> ObtenerActivasAsync(DateTime? desde, bool usarSpecification);
    Task<ComparacionReservas> CompararAsync();
}
