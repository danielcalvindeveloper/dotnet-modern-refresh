using Lab06f.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab06f.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Turno> Turnos => Set<Turno>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // EF compara el valor ORIGINAL; la aplicación incrementa el valor NUEVO.
        modelBuilder.Entity<Turno>().Property(t => t.Version)
            .HasDefaultValue(1).IsConcurrencyToken();
    }
}
