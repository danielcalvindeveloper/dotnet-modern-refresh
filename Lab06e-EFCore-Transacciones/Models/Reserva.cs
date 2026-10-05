using System.Text.Json.Serialization;

namespace Lab06e.Models;

public class Reserva
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public DateTime Fecha { get; set; }
    public string Estado { get; set; } = string.Empty;
    public ICollection<MovimientoReserva> Movimientos { get; set; } = new List<MovimientoReserva>();

    [JsonIgnore]
    public Cliente Cliente { get; set; } = null!;
}
