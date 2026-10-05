using Lab06c.Data;
using Lab06c.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab06c.Services;

public class ClienteService : IClienteService
{
    private readonly AppDbContext _db;

    public ClienteService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Cliente>> ObtenerTodosAsync()
    {
        return await _db.Clientes.OrderBy(cliente => cliente.Id).ToListAsync();
    }

    public async Task<Cliente?> ObtenerPorIdAsync(int id)
    {
        return await _db.Clientes.SingleOrDefaultAsync(cliente => cliente.Id == id);
    }
}
