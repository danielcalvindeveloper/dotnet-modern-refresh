using Lab03.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lab03.Controllers;

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
    public ActionResult Get()
    {
        return Ok(new { mensaje = _holaService.ObtenerSaludo() });
    }
}
