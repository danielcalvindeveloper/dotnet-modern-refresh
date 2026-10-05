using System.Text.Json.Serialization;

namespace Lab06c.Models;

public class Cliente
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public string? Telefono { get; set; }

    // Evita el ciclo JSON Reserva -> Cliente -> Reservas; no afecta el mapping EF.
    [JsonIgnore]
    public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}
