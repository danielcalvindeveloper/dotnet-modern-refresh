namespace Lab09c.Models;

public sealed class Reserva
{
    public int Id { get; init; }
    public int ClienteId { get; init; }
    public DateTime Fecha { get; init; }
    public string Estado { get; init; } = string.Empty;
}
