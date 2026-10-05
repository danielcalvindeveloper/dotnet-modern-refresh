using Lab06e.Data;
using Lab06e.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab06e.Services;

public class ReservaService : IReservaService
{
    private readonly AppDbContext _db;

    public ReservaService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Reserva> SaveChangesUnicoAsync()
    {
        var reserva = NuevaReserva();
        var movimiento = new MovimientoReserva { Reserva = reserva, Descripcion = "SaveChanges único: reserva creada." };
        _db.Reservas.Add(reserva);
        _db.MovimientosReserva.Add(movimiento);

        Console.WriteLine(">>> SaveChanges único: dos entidades, sin BeginTransaction manual");
        await _db.SaveChangesAsync();
        Console.WriteLine($">>> Guardado: ReservaId={reserva.Id}, MovimientoId={movimiento.Id}");
        return reserva;
    }

    public async Task<Reserva> DosSaveChangesSinTransaccionAsync()
    {
        var reserva = NuevaReserva();
        _db.Reservas.Add(reserva);
        Console.WriteLine(">>> SaveChanges #1 (sin transacción explícita)");
        await _db.SaveChangesAsync();
        Console.WriteLine($">>> Primer cambio confirmado: ReservaId={reserva.Id}");

        ProvocarError(); // Lanza siempre: el código siguiente no llega a ejecutarse.

        _db.MovimientosReserva.Add(new MovimientoReserva { Reserva = reserva, Descripcion = "Segundo SaveChanges." });
        Console.WriteLine(">>> SaveChanges #2");
        await _db.SaveChangesAsync();
        return reserva;
    }

    public async Task<Reserva> ConTransaccionAsync(bool provocarError)
    {
        Console.WriteLine(">>> BeginTransaction");
        await using var transaction = await _db.Database.BeginTransactionAsync();
        var reserva = NuevaReserva();

        try
        {
            _db.Reservas.Add(reserva);
            Console.WriteLine(">>> SaveChanges #1 (dentro de la transacción)");
            await _db.SaveChangesAsync();
            Console.WriteLine($">>> Después de SaveChanges #1: ReservaId={reserva.Id}, State={_db.Entry(reserva).State}; aún sin Commit");

            if (provocarError)
            {
                ProvocarError();
            }

            var movimiento = new MovimientoReserva { Reserva = reserva, Descripcion = "Transacción explícita: reserva creada." };
            _db.MovimientosReserva.Add(movimiento);
            Console.WriteLine(">>> SaveChanges #2 (dentro de la transacción)");
            await _db.SaveChangesAsync();

            Console.WriteLine(">>> Commit");
            await transaction.CommitAsync();
            Console.WriteLine($">>> Confirmado: ReservaId={reserva.Id}, MovimientoId={movimiento.Id}");
            return reserva;
        }
        catch
        {
            Console.WriteLine(">>> Rollback");
            await transaction.RollbackAsync();
            Console.WriteLine($">>> Después de Rollback: ReservaId={reserva.Id}, State={_db.Entry(reserva).State}");
            // Rollback no restaura el tracker. Propagamos el error y finaliza este scope.
            throw;
        }
    }

    public async Task<List<Reserva>> ObtenerReservasAsync()
    {
        Console.WriteLine(">>> Verificación SQLite: nueva request/contexto, AsNoTracking + Include(Movimientos)");
        return await _db.Reservas.AsNoTracking().Include(r => r.Movimientos)
            .OrderBy(r => r.Id).ToListAsync();
    }

    private static Reserva NuevaReserva()
    {
        return new Reserva { ClienteId = 1, Fecha = DateTime.UtcNow, Estado = "Pendiente" };
    }

    private static void ProvocarError()
    {
        Console.WriteLine(">>> ERROR DELIBERADO después del primer SaveChanges");
        throw new InvalidOperationException("Error deliberado después del primer SaveChanges.");
    }
}
