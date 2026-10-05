using Lab07a.Models;

namespace Lab07a.Services;

public interface IReservaService
{
    Reserva Crear(int clienteId, DateTime fecha, string estado);
}
