using Lab06f.Data;
using Lab06f.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab06f.Services;

public class TurnoService : ITurnoService
{
    private readonly AppDbContext _db;
    private readonly SinControlDbContext _sinControl;

    public TurnoService(AppDbContext db, SinControlDbContext sinControl)
    {
        _db = db;
        _sinControl = sinControl;
    }

    public Task<Turno?> ObtenerAsync(int turnoId) =>
        _db.Turnos.AsNoTracking().SingleOrDefaultAsync(t => t.Id == turnoId);

    public async Task<Turno?> ResetAsync(int turnoId)
    {
        var turno = await _db.Turnos.SingleOrDefaultAsync(t => t.Id == turnoId);
        if (turno is null) return null;

        // Solo repetir el experimento: ejecutar cuando no haya reservas en vuelo.
        turno.Capacidad = 1;
        turno.CuposOcupados = 0;
        turno.Version = 1;
        await _db.SaveChangesAsync();
        Console.WriteLine($"[Reset] Turno={turno.Id}, CuposOcupados=0, Version=1");
        return turno;
    }

    public Task<ResultadoReserva> ReservarSinControlAsync(int turnoId, string operacion) =>
        ReservarAsync(_sinControl, turnoId, operacion, conControl: false);

    public Task<ResultadoReserva> ReservarConControlAsync(int turnoId, string operacion) =>
        ReservarAsync(_db, turnoId, operacion, conControl: true);

    private static async Task<ResultadoReserva> ReservarAsync(
        DbContext db, int turnoId, string operacion, bool conControl)
    {
        Console.WriteLine($"[{operacion}] Leyendo turno {turnoId}; conControl={conControl}");
        var turno = await db.Set<Turno>().SingleOrDefaultAsync(t => t.Id == turnoId);
        if (turno is null) return new ResultadoReserva();

        var versionOriginal = turno.Version;
        Console.WriteLine($"[{operacion}] Leído: Capacidad={turno.Capacidad}, CuposOcupados={turno.CuposOcupados}, Version={versionOriginal}");
        if (turno.CuposOcupados >= turno.Capacidad)
        {
            Console.WriteLine($"[{operacion}] Sin disponibilidad; no se guarda.");
            return new ResultadoReserva { Turno = turno };
        }

        Console.WriteLine($"[{operacion}] Hay disponibilidad; delay didáctico de 2000 ms.");
        await Task.Delay(2000); // Amplía la carrera; no es una solución de sincronización.

        turno.CuposOcupados++;
        turno.Version++; // Versionado por aplicación en ambas variantes; solo una lo compara.
        Console.WriteLine($"[{operacion}] SaveChanges: CuposOcupados={turno.CuposOcupados}, Version nueva={turno.Version}, original={versionOriginal}");
        await db.SaveChangesAsync();
        Console.WriteLine($"[{operacion}] ÉXITO: guardado aceptado.");
        return new ResultadoReserva { Turno = turno, Reservado = true };
    }
}
