using System.ComponentModel.DataAnnotations;

namespace Lab07a.Contracts;

public class CrearReservaRequest
{
    // Nullable permite distinguir ausente/null de un número como 0.
    [Required(ErrorMessage = "ClienteId es obligatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "ClienteId debe ser mayor que cero.")]
    public int? ClienteId { get; set; }

    [Required(ErrorMessage = "Fecha es obligatoria.")]
    public DateTime? Fecha { get; set; }

    // Sin valor por defecto: omitir Estado debe fallar la validación.
    [Required(ErrorMessage = "Estado es obligatorio.")]
    [StringLength(20, MinimumLength = 3, ErrorMessage = "Estado debe tener entre 3 y 20 caracteres.")]
    public string? Estado { get; set; }
}
