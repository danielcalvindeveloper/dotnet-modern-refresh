using Lab06c.Models;
using Lab06c.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lab06c.Controllers;

[ApiController]
[Route("api/clientes")]
public class ClientesController : ControllerBase
{
    private readonly IClienteService _service;

    public ClientesController(IClienteService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Cliente>>> GetAsync()
    {
        return Ok(await _service.ObtenerTodosAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Cliente>> GetPorIdAsync(int id)
    {
        var cliente = await _service.ObtenerPorIdAsync(id);
        if (cliente is null)
        {
            return NotFound();
        }

        return Ok(cliente);
    }
}
