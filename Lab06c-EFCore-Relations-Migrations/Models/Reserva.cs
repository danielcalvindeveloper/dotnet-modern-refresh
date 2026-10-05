namespace Lab06c.Models;

public class Reserva
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public DateTime Fecha { get; set; }
    public string Estado { get; set; } = string.Empty;

    // EF carga esta navegación con Include; al crear la asignamos explícitamente.
    public Cliente Cliente { get; set; } = null!;
}
