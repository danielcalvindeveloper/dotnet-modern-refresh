using Lab07a.Contracts;
using Lab07a.Models;
using Lab07a.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lab07a.Controllers;

[ApiController]
[Route("api/reservas")]
public class ReservasController : ControllerBase
{
    private readonly IReservaService _service;

    public ReservasController(IReservaService service)
    {
        _service = service;
    }

    [HttpPost]
    public ActionResult<Reserva> Post(CrearReservaRequest request)
    {
        Console.WriteLine(">>> Controller ejecutado");
        Console.WriteLine($"ModelState: IsValid={ModelState.IsValid}, ErrorCount={ModelState.ErrorCount}");

        // [ApiController] ya rechazó los requests inválidos antes de esta Action.
        // El compilador no deduce las garantías de [Required]; usamos ! aquí.
        var reserva = _service.Crear(request.ClienteId!.Value, request.Fecha!.Value, request.Estado!);
        return Ok(reserva);
    }
}
