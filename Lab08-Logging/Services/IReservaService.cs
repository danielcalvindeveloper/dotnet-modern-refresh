using Lab08.Models;

namespace Lab08.Services;

public interface IReservaService
{
    Reserva? Obtener(int id);
    void ProvocarError(int id);
    void MostrarNiveles(int id);
}
