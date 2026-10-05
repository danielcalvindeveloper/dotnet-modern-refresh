using Lab06c.Models;

namespace Lab06c.Services;

public interface IClienteService
{
    Task<IEnumerable<Cliente>> ObtenerTodosAsync();
    Task<Cliente?> ObtenerPorIdAsync(int id);
}
