using Lab07b.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lab07b.Controllers;

[ApiController]
[Route("api")]
public class PruebaController : ControllerBase
{
    private readonly IPruebaService _service;

    public PruebaController(IPruebaService service)
    {
        _service = service;
    }

    [HttpGet("prueba-error")]
    public IActionResult PruebaError()
    {
        Console.WriteLine("[Controller] GET /api/prueba-error");
        _service.ProvocarError();
        return Ok(); // No se alcanza: la excepción se propaga hacia el middleware.
    }

    [HttpGet("prueba-ok")]
    public IActionResult PruebaOk()
    {
        Console.WriteLine("[Controller] GET /api/prueba-ok");
        return Ok(new { mensaje = "Todo funciona." });
    }
}
