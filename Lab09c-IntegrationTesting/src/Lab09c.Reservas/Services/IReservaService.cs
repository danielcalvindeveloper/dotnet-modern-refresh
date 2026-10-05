using Lab09c.Models;

namespace Lab09c.Services;

public interface IReservaService
{
    Reserva? Obtener(int id);
    Reserva Crear(int clienteId, DateTime fecha, string estado);
}
