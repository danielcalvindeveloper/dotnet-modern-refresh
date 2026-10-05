namespace Lab06f.Models;

public class Turno
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public int Capacidad { get; set; }
    public int CuposOcupados { get; set; }
    public int Version { get; set; } = 1;
}
