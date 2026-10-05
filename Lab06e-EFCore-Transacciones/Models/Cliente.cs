using System.Text.Json.Serialization;

namespace Lab06e.Models;

public class Cliente
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    [JsonIgnore]
    public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}
