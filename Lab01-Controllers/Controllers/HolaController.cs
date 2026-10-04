using Microsoft.AspNetCore.Mvc;

namespace Lab01.Controllers;

[ApiController]
[Route("api/hola")]
public class HolaController : ControllerBase
{
    [HttpGet]
    public ActionResult Get()
    {
        return Ok(new { mensaje = "Hola desde ASP.NET Core" });
    }
}
