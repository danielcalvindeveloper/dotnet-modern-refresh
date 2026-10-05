using Lab06e.Models;
using Lab06e.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lab06e.Controllers;

[ApiController]
[Route("api/transacciones")]
public class TransaccionesController : ControllerBase
{
    private readonly IReservaService _service;

    public TransaccionesController(IReservaService service)
    {
        _service = service;
    }

    [HttpPost("savechanges-unico")]
    public async Task<ActionResult<Reserva>> SaveChangesUnicoAsync()
    {
        return Ok(await _service.SaveChangesUnicoAsync());
    }

    [HttpPost("dos-savechanges-sin-transaccion")]
    public async Task<ActionResult<Reserva>> DosSaveChangesSinTransaccionAsync()
    {
        return Ok(await _service.DosSaveChangesSinTransaccionAsync());
    }

    [HttpPost("transaccion-ok")]
    public async Task<ActionResult<Reserva>> TransaccionOkAsync()
    {
        return Ok(await _service.ConTransaccionAsync(provocarError: false));
    }

    [HttpPost("transaccion-error")]
    public async Task<ActionResult<Reserva>> TransaccionErrorAsync()
    {
        return Ok(await _service.ConTransaccionAsync(provocarError: true));
    }

    [HttpGet("reservas")]
    public async Task<ActionResult<List<Reserva>>> ReservasAsync()
    {
        return Ok(await _service.ObtenerReservasAsync());
    }
}
