using Lab08.Models;
using Lab08.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lab08.Controllers;

[ApiController]
[Route("api/reservas")]
public sealed class ReservasController : ControllerBase
{
    private readonly IReservaService _service;
    private readonly ILogger<ReservasController> _logger;

    public ReservasController(IReservaService service, ILogger<ReservasController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet("{id:int}")]
    public ActionResult<Reserva> Obtener(int id)
    {
        using var scope = _logger.BeginScope("Procesando ReservaId={ReservaId}", id);
        _logger.LogInformation("Consultando reserva {ReservaId} desde el Controller", id);
        var reserva = _service.Obtener(id);
        return reserva is null ? NotFound() : Ok(reserva);
    }

    [HttpGet("{id:int}/prueba-error")]
    public IActionResult PruebaError(int id)
    {
        using var scope = _logger.BeginScope("Procesando ReservaId={ReservaId}", id);
        _logger.LogInformation("Solicitando error didáctico para reserva {ReservaId}", id);
        _service.ProvocarError(id);
        return Ok(); // No se alcanza: la excepción llega al handler global.
    }

    [HttpGet("{id:int}/niveles")]
    public IActionResult Niveles(int id)
    {
        using var scope = _logger.BeginScope("Procesando ReservaId={ReservaId}", id);
        _logger.LogInformation("Solicitando los seis niveles para reserva {ReservaId}", id);
        _service.MostrarNiveles(id);
        return Ok(new { mensaje = "Muestras emitidas; revisar los niveles habilitados en consola." });
    }
}
