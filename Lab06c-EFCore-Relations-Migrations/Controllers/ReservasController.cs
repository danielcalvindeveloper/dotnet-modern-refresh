using Lab06c.Contracts;
using Lab06c.Models;
using Lab06c.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lab06c.Controllers;

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
    public async Task<ActionResult<IEnumerable<Reserva>>> GetAsync()
    {
        return Ok(await _service.ObtenerTodosAsync());
    }

    [HttpGet("{id:int}", Name = "ObtenerReservaPorId")]
    public async Task<ActionResult<Reserva>> GetPorIdAsync(int id)
    {
        var reserva = await _service.ObtenerPorIdAsync(id);
        if (reserva is null)
        {
            return NotFound();
        }

        return Ok(reserva);
    }

    [HttpPost]
    public async Task<ActionResult<Reserva>> PostAsync(CrearReservaRequest request)
    {
        var reserva = await _service.CrearAsync(request.ClienteId, request.Fecha, request.Estado);
        if (reserva is null)
        {
            return NotFound(new { mensaje = $"No existe el cliente {request.ClienteId}." });
        }

        return CreatedAtRoute("ObtenerReservaPorId", new { id = reserva.Id }, reserva);
    }

[HttpPut("{id}/confirmar")]
public async Task<ActionResult<Reserva>> ConfirmarAsync(int id)
    {
        var reserva = await _service.ConfirmarAsync(id);

        if (reserva is null)
            return NotFound();

        return Ok(reserva);
    }    
}
