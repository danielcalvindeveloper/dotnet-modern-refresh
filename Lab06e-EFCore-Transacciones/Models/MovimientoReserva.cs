using System.Text.Json.Serialization;

namespace Lab06e.Models;

public class MovimientoReserva
{
    public int Id { get; set; }
    public int ReservaId { get; set; }
    public string Descripcion { get; set; } = string.Empty;

    [JsonIgnore]
    public Reserva Reserva { get; set; } = null!;
}
