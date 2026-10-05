using Lab09c.Contracts;
using Lab09c.Models;
using Lab09c.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lab09c.Controllers;

[ApiController]
[Route("api/reservas")]
public sealed class ReservasController : ControllerBase
{
    private readonly IReservaService _service;

    public ReservasController(IReservaService service)
    {
        _service = service;
    }

    [HttpGet("{id:int}")]
    public ActionResult<Reserva> Obtener(int id)
    {
        var reserva = _service.Obtener(id);
        return reserva is null ? NotFound() : Ok(reserva);
    }

    [HttpPost]
    public ActionResult<Reserva> Crear(CrearReservaRequest request)
    {
        Console.WriteLine(">>> Controller ejecutado: Crear reserva");
        // [ApiController] ya rechazó los requests inválidos, como en Lab07a.
        var reserva = _service.Crear(request.ClienteId!.Value, request.Fecha!.Value, request.Estado!);
        return CreatedAtAction(nameof(Obtener), new { id = reserva.Id }, reserva);
    }
}
