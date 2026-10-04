using Lab06b.Models;

namespace Lab06b.Services;

public interface IClienteService
{
    Task<IEnumerable<Cliente>> ObtenerTodosAsync();
    Task<Cliente?> ObtenerPorIdAsync(int id);
}
