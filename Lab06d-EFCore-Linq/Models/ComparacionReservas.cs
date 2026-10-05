namespace Lab06d.Models;

public class ComparacionReservas
{
    public List<Reserva> EnSql { get; set; } = new List<Reserva>();
    public List<Reserva> EnMemoria { get; set; } = new List<Reserva>();
}
