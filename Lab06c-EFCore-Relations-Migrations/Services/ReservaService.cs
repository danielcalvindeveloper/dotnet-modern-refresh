using Lab06c.Data;
using Lab06c.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab06c.Services;

public class ReservaService : IReservaService
{
    private readonly AppDbContext _db;

    public ReservaService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Reserva>> ObtenerTodosAsync()
    {
        return await _db.Reservas.Include(reserva => reserva.Cliente)
            .OrderBy(reserva => reserva.Id).ToListAsync();
    }

    public async Task<Reserva?> ObtenerPorIdAsync(int id)
    {
        return await _db.Reservas.Include(reserva => reserva.Cliente)
            .SingleOrDefaultAsync(reserva => reserva.Id == id);
    }

    public async Task<Reserva?> CrearAsync(int clienteId, DateTime fecha, string estado)
    {
        var cliente = await _db.Clientes.SingleOrDefaultAsync(c => c.Id == clienteId);
        if (cliente is null)
        {
            return null;
        }

        var reserva = new Reserva
        {
            ClienteId = clienteId,
            Fecha = fecha,
            Estado = estado,
            Cliente = cliente
        };

        _db.Reservas.Add(reserva);
        Console.WriteLine($"[Tracking antes de SaveChanges] Estado={_db.Entry(reserva).State}, Id={reserva.Id}");

        await _db.SaveChangesAsync();

        Console.WriteLine($"[Tracking después de SaveChanges] Estado={_db.Entry(reserva).State}, Id={reserva.Id}");
        return reserva;
    }

    public async Task<Reserva?> ConfirmarAsync(int id)
    {
        var reserva = await _db.Reservas.SingleOrDefaultAsync(r => r.Id == id);

        if (reserva is null)
            return null;

        Console.WriteLine(
            $"[Antes del cambio] Estado tracking={_db.Entry(reserva).State}, " +
            $"Estado reserva={reserva.Estado}");

        reserva.Estado = "Confirmada";

        Console.WriteLine(
            $"[Después del cambio] Estado tracking={_db.Entry(reserva).State}, " +
            $"Estado reserva={reserva.Estado}");

        await _db.SaveChangesAsync();

        Console.WriteLine(
            $"[Después de SaveChanges] Estado tracking={_db.Entry(reserva).State}, " +
            $"Estado reserva={reserva.Estado}");

        return reserva;
    }

}
