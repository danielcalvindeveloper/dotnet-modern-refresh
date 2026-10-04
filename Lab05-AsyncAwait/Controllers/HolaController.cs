using Lab05.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lab05.Controllers;

[ApiController]
[Route("api/hola")]
public class HolaController : ControllerBase
{
    private readonly IHolaService _holaService;

    public HolaController(IHolaService holaService)
    {
        _holaService = holaService;
    }

    [HttpGet]
    public async Task<ActionResult> GetAsync()
    {
        var saludo = await _holaService.ObtenerSaludoAsync();

        return Ok(new { mensaje = saludo });
    }
}
