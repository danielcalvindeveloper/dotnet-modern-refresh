using Lab06f.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab06f.Data;

// Mismo dominio y misma tabla, pero sin token: permite observar el problema inicial.
public class SinControlDbContext : DbContext
{
    public SinControlDbContext(DbContextOptions<SinControlDbContext> options) : base(options) { }

    public DbSet<Turno> Turnos => Set<Turno>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Turno>().Property(t => t.Version)
            .HasDefaultValue(1).IsConcurrencyToken(false);
    }
}
