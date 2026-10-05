using Lab06f.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab06f.Controllers;

[ApiController]
[Route("api/concurrencia")]
public class ConcurrenciaController : ControllerBase
{
    private readonly ITurnoService _service;

    public ConcurrenciaController(ITurnoService service) => _service = service;

    [HttpGet("turnos/{turnoId:int}")]
    public async Task<IActionResult> ObtenerAsync(int turnoId)
    {
        var turno = await _service.ObtenerAsync(turnoId);
        return turno is null ? NotFound() : Ok(turno);
    }

    [HttpPost("reset/{turnoId:int}")]
    public async Task<IActionResult> ResetAsync(int turnoId)
    {
        var turno = await _service.ResetAsync(turnoId);
        return turno is null ? NotFound() : Ok(turno);
    }

    [HttpPost("reservar-sin-control/{turnoId:int}")]
    public Task<IActionResult> SinControlAsync(int turnoId) => ReservarAsync(turnoId, false);

    [HttpPost("reservar-con-control/{turnoId:int}")]
    public Task<IActionResult> ConControlAsync(int turnoId) => ReservarAsync(turnoId, true);

    private async Task<IActionResult> ReservarAsync(int turnoId, bool conControl)
    {
        var operacion = Request.Headers["X-Operacion"].FirstOrDefault()
            ?? Guid.NewGuid().ToString("N")[..6];
        Response.Headers["X-Operacion"] = operacion;

        try
        {
            var resultado = conControl
                ? await _service.ReservarConControlAsync(turnoId, operacion)
                : await _service.ReservarSinControlAsync(turnoId, operacion);
            if (resultado.Turno is null) return NotFound();
            if (!resultado.Reservado)
                return Problem(statusCode: 409, title: "No hay cupos disponibles.",
                    detail: "El turno ya está completo.", instance: Request.Path);

            return Ok(new { operacion, reservado = true, turno = resultado.Turno });
        }
        catch (DbUpdateConcurrencyException exception)
        {
            var original = exception.Entries.Single().Property("Version").OriginalValue;
            Console.WriteLine($"[{operacion}] DbUpdateConcurrencyException: 0 filas actualizadas con Version original={original}; HTTP 409.");
            return Problem(statusCode: 409, title: "Otro request modificó el turno.",
                detail: "El último cupo pudo ser tomado por otra reserva. Consultar el estado actual.",
                instance: Request.Path);
        }
    }
}
