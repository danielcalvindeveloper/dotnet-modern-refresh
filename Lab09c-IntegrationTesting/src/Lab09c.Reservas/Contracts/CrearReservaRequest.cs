using System.ComponentModel.DataAnnotations;

namespace Lab09c.Contracts;

public sealed class CrearReservaRequest
{
    [Required]
    [Range(1, int.MaxValue)]
    public int? ClienteId { get; set; }

    [Required]
    public DateTime? Fecha { get; set; }

    [Required]
    [StringLength(20, MinimumLength = 3)]
    public string? Estado { get; set; }
}
