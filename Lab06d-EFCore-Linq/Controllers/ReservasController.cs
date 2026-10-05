using Lab06d.Models;
using Lab06d.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lab06d.Controllers;

[ApiController]
[Route("api/reservas")]
public class ReservasController : ControllerBase
{
    private readonly IReservaService _service;

    public ReservasController(IReservaService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<Reserva>>> GetAsync()
    {
        return Ok(await _service.BuscarAsync(null, null));
    }

    [HttpGet("buscar")]
    public async Task<ActionResult<List<Reserva>>> BuscarAsync([FromQuery] string? estado, [FromQuery] DateTime? desde)
    {
        return Ok(await _service.BuscarAsync(estado, desde));
    }

    [HttpGet("activas")]
    public async Task<ActionResult<List<Reserva>>> ActivasAsync([FromQuery] DateTime? desde, [FromQuery] bool usarSpecification = false)
    {
        return Ok(await _service.ObtenerActivasAsync(desde, usarSpecification));
    }

    [HttpGet("comparar")]
    public async Task<ActionResult<ComparacionReservas>> CompararAsync()
    {
        return Ok(await _service.CompararAsync());
    }
}
