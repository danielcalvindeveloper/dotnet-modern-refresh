using Lab06e.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab06e.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Reserva> Reservas => Set<Reserva>();
    public DbSet<MovimientoReserva> MovimientosReserva => Set<MovimientoReserva>();
}
