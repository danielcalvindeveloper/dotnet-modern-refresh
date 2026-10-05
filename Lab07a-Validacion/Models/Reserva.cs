namespace Lab07a.Models;

public class Reserva
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public DateTime Fecha { get; set; }
    public string Estado { get; set; } = string.Empty;
}
