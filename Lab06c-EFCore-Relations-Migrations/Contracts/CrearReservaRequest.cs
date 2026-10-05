namespace Lab06c.Contracts;

public class CrearReservaRequest
{
    public int ClienteId { get; set; }
    public DateTime Fecha { get; set; }
    public string Estado { get; set; } = "Pendiente";
}
