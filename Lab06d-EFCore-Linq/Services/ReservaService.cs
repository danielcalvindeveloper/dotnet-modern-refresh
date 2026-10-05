using System.Linq.Expressions;
using Lab06d.Data;
using Lab06d.Models;
using Lab06d.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Lab06d.Services;

public class ReservaService : IReservaService
{
    private readonly AppDbContext _db;

    public ReservaService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Reserva>> BuscarAsync(string? estado, DateTime? desde)
    {
        IQueryable<Reserva> query = _db.Reservas;
        query = query.AsNoTracking().Include(r => r.Cliente);
        Console.WriteLine("[Construcción] IQueryable<Reserva>; todavía no se ejecutó SQL.");

        if (estado is not null)
        {
            query = query.Where(r => r.Estado == estado);
            Console.WriteLine("[Construcción] Where(Estado); todavía no se ejecutó SQL.");
        }

        if (desde is not null)
        {
            var fechaDesde = desde.Value;
            query = query.Where(r => r.Fecha >= fechaDesde);
            Console.WriteLine("[Construcción] Where(Fecha); todavía no se ejecutó SQL.");
        }

        query = query.OrderBy(r => r.Fecha);
        Console.WriteLine("[Construcción] OrderBy(Fecha); todavía no se ejecutó SQL.");
        Console.WriteLine("[Inspección] ToQueryString(): muestra SQL, no lo ejecuta.");
        Console.WriteLine(query.ToQueryString());

        Console.WriteLine("[Materialización] ToListAsync(): ahora se ejecuta el comando SQL.");
        var reservas = await query.ToListAsync();
        Console.WriteLine($"[Resultado] {reservas.Count} reservas; tracked={_db.ChangeTracker.Entries<Reserva>().Count()} (AsNoTracking).");
        return reservas;
    }

    public async Task<List<Reserva>> ObtenerActivasAsync(DateTime? desde, bool usarSpecification)
    {
        Expression<Func<Reserva, bool>> activas = r => r.Estado != "Cancelada";
        Console.WriteLine($"[Expression] {activas}; Body.NodeType={activas.Body.NodeType}");
        IQueryable<Reserva> query = _db.Reservas.AsNoTracking().Include(r => r.Cliente);

        if (usarSpecification)
        {
            var spec = new ReservasActivasSpecification();
            query = query.Where(spec.Criteria);
            Console.WriteLine("[Construcción] Where(spec.Criteria): mismo criterio, nombrado y reutilizable.");
        }
        else
        {
            query = query.Where(activas);
            Console.WriteLine("[Construcción] Where(activas): expresión traducible, no delegate ejecutado en memoria.");
        }

        if (desde is not null)
        {
            var fechaDesde = desde.Value;
            query = query.Where(r => r.Fecha >= fechaDesde);
        }

        query = query.OrderBy(r => r.Fecha);
        Console.WriteLine("[Inspección] ToQueryString(): no ejecuta SQL.");
        Console.WriteLine(query.ToQueryString());
        Console.WriteLine("[Materialización] ToListAsync(): una consulta SQL.");
        var reservas = await query.ToListAsync();
        Console.WriteLine($"[Resultado] {reservas.Count} activas; tracked={_db.ChangeTracker.Entries<Reserva>().Count()}.");
        return reservas;
    }

    public async Task<ComparacionReservas> CompararAsync()
    {
        Console.WriteLine("[Comparación SQL] Where antes de materializar; AsNoTracking.");
        var enSql = await _db.Reservas.AsNoTracking().Include(r => r.Cliente)
            .Where(r => r.Estado == "Pendiente").OrderBy(r => r.Fecha).ToListAsync();
        Console.WriteLine($"[Comparación SQL] {enSql.Count} reservas; tracked={_db.ChangeTracker.Entries<Reserva>().Count()}.");

        Console.WriteLine("[Comparación memoria] ToListAsync ANTES del Where; consulta sin filtro de Estado, tracking por defecto.");
        IEnumerable<Reserva> todas = await _db.Reservas.Include(r => r.Cliente).ToListAsync();
        Console.WriteLine($"[Comparación memoria] Cargadas={todas.Count()}; tracked={_db.ChangeTracker.Entries<Reserva>().Count()}.");

        Func<Reserva, bool> pendientes = r => r.Estado == "Pendiente";
        IEnumerable<Reserva> filtradas = todas.Where(pendientes);
        Console.WriteLine("[Comparación memoria] Enumerable.Where(Func): filtrado diferido sobre la lista; sin nuevo SQL.");
        var enMemoria = filtradas.OrderBy(r => r.Fecha).ToList();
        Console.WriteLine($"[Comparación memoria] Resultado={enMemoria.Count}; mismo resultado, distinto lugar de filtrado.");
        return new ComparacionReservas { EnSql = enSql, EnMemoria = enMemoria };
    }
}
